using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.Ingredients;
using YesChef.Player;
using YesChef.UI;

namespace YesChef.Stations
{
    /// <summary>
    /// Prepares one vegetable at a time and exposes contextual place/pick actions.
    /// </summary>
    public sealed class TableStation : BaseStationUIController
    {
        private const float PrepareDurationSeconds = 2f;

        [SerializeField] private Transform itemAnchor;
        [SerializeField] private WorldTimerUI worldTimerUI;

        private IngredientInstance preparedItem;
        private float remainingPrepareTime;
        private GameObject spawnedVisual;
        private GameObject currentVisualPrefab;

        public override string PopupTitle => "Prep Table";
        public bool IsProcessing => preparedItem != null && remainingPrepareTime > 0f;

        private void Update()
        {
            if (!IsProcessing)
            {
                UpdateTimerUI();
                return;
            }

            remainingPrepareTime = Mathf.Max(0f, remainingPrepareTime - Time.deltaTime);
            if (remainingPrepareTime <= 0f && preparedItem != null)
            {
                preparedItem.SetState(IngredientProcessState.Prepared);
                RefreshVisual();
                NotifyContextChanged();
            }

            UpdateTimerUI();
        }

        private void OnDisable()
        {
            ClearVisual();
            UpdateTimerUI();
        }

        public override void GetContextActions(PlayerCarryController playerCarryController, System.Collections.Generic.List<ContextActionData> actions)
        {
            if (preparedItem != null && !IsProcessing)
            {
                bool canPickUp = playerCarryController != null && !playerCarryController.HasItem();
                actions.Add(new ContextActionData("Pick Vegetable", canPickUp, () => PickPreparedVegetable(playerCarryController)));
                return;
            }

            if (preparedItem != null || playerCarryController == null)
            {
                return;
            }

            IngredientInstance heldItem = PeekHeldItem(playerCarryController);
            if (!IsIngredient(heldItem, IngredientType.Vegetable, IngredientProcessState.Raw))
            {
                return;
            }

            actions.Add(new ContextActionData("Place Vegetable", true, () => StartPreparingVegetable(playerCarryController)));
        }

        private void StartPreparingVegetable(PlayerCarryController playerCarryController)
        {
            if (preparedItem != null || playerCarryController == null)
            {
                return;
            }

            IngredientInstance heldItem = PeekHeldItem(playerCarryController);
            if (!IsIngredient(heldItem, IngredientType.Vegetable, IngredientProcessState.Raw))
            {
                return;
            }

            preparedItem = TakeHeldItem(playerCarryController);
            remainingPrepareTime = PrepareDurationSeconds;
            RefreshVisual();
            UpdateTimerUI();
            NotifyContextChanged();
        }

        private void PickPreparedVegetable(PlayerCarryController carryController)
        {
            if (preparedItem == null || IsProcessing || carryController == null || carryController.HasItem())
            {
                return;
            }

            if (!TryGiveItem(carryController, preparedItem))
            {
                return;
            }

            preparedItem = null;
            remainingPrepareTime = 0f;
            ClearVisual();
            UpdateTimerUI();
            NotifyContextChanged();
        }

        private void RefreshVisual()
        {
            GameObject targetPrefab = preparedItem?.Data?.GetPrefabForState(preparedItem.State);
            if (targetPrefab == currentVisualPrefab && spawnedVisual != null)
            {
                return;
            }

            ClearVisual();

            if (targetPrefab == null)
            {
                return;
            }

            Transform anchor = itemAnchor != null ? itemAnchor : transform;
            spawnedVisual = Instantiate(targetPrefab, anchor);
            spawnedVisual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            spawnedVisual.transform.localScale = Vector3.one;
            currentVisualPrefab = targetPrefab;
        }

        private void ClearVisual()
        {
            if (spawnedVisual != null)
            {
                Destroy(spawnedVisual);
                spawnedVisual = null;
            }

            currentVisualPrefab = null;
        }

        private void UpdateTimerUI()
        {
            if (worldTimerUI == null)
            {
                return;
            }

            bool showTimer = IsProcessing;
            worldTimerUI.SetVisible(showTimer);
            if (showTimer)
            {
                worldTimerUI.SetTime(remainingPrepareTime);
            }
        }
    }
}
