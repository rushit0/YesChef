using System;
using System.Collections;
using TMPro;
using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.Ingredients;
using YesChef.Managers;
using YesChef.Player;
using YesChef.UI;

namespace YesChef.Orders {
    /// <summary>
    /// Customer-facing delivery window that owns one active order at a time.
    /// It exposes a contextual delivery action and renders icon-based order requirements.
    /// </summary>
    public sealed class CustomerWindow : MonoBehaviour, IInteractable, IContextActionSource {
        private const float RespawnDelaySeconds = 5f;

        [Header("Optional UI")]
        [SerializeField] private Transform popupAnchor;
        [SerializeField] private Transform ingredientIconContainer;
        [SerializeField] private OrderIngredientIconView ingredientIconPrefab;
        [SerializeField] private TMP_Text timerLabel;
        [SerializeField] private TMP_Text statusLabel;

        private readonly System.Collections.Generic.List<OrderIngredientIconView> spawnedIcons = new();

        private ScoreManager scoreManager;
        private Coroutine respawnRoutine;

        public string PopupTitle => "Customer Order";
        public Transform PopupAnchor => popupAnchor != null ? popupAnchor : transform;
        public OrderData CurrentOrder { get; private set; }

        public event Action<CustomerWindow> RespawnRequested;
        public event Action ContextActionsChanged;

        public void Initialize(ScoreManager manager) {
            scoreManager = manager;
        }

        public void SetOrder(OrderData order) {
            if (respawnRoutine != null) {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }

            CurrentOrder = order;
            RefreshDisplay("Waiting");
            NotifyContextChanged();
        }

        public void Interact(GameObject interactor) {
            PlayerCarryController carryController = interactor != null ? interactor.GetComponent<PlayerCarryController>() : null;
            DeliverIngredient(carryController);
        }

        public void GetContextActions(PlayerCarryController playerCarryController, System.Collections.Generic.List<ContextActionData> actions) {
            if (CurrentOrder == null || CurrentOrder.IsComplete || playerCarryController == null) {
                return;
            }

            IngredientInstance heldItem = playerCarryController.PeekItem();
            if (!CurrentOrder.CanAcceptIngredient(heldItem)) {
                return;
            }

            actions.Add(new ContextActionData("Deliver Ingredient", true, () => DeliverIngredient(playerCarryController)));
        }

        private void DeliverIngredient(PlayerCarryController carryController) {
            if (CurrentOrder == null || CurrentOrder.IsComplete || carryController == null) {
                return;
            }

            IngredientInstance heldItem = carryController.PeekItem();
            if (heldItem == null || !CurrentOrder.TryMatchIngredient(heldItem)) {
                return;
            }

            carryController.DropItem();

            if (CurrentOrder.IsComplete) {
                CompleteOrder();
                return;
            }

            RefreshDisplay("Delivered");
            NotifyContextChanged();
        }

        private void Update() {
            if (CurrentOrder == null || CurrentOrder.IsComplete) {
                return;
            }

            CurrentOrder.AdvanceTime(Time.deltaTime);
            UpdateTimerLabel();
        }

        private void CompleteOrder() {
            int score = CurrentOrder.CalculateScore();
            scoreManager?.AddScore(score);
            RefreshDisplay($"Complete ({score:+#;-#;0})");
            respawnRoutine = StartCoroutine(RequestRespawnRoutine());
            NotifyContextChanged();
        }

        private IEnumerator RequestRespawnRoutine() {
            yield return new WaitForSeconds(RespawnDelaySeconds);

            CurrentOrder = null;
            respawnRoutine = null;
            RefreshDisplay("Next customer");
            NotifyContextChanged();
            RespawnRequested?.Invoke(this);
        }

        private void RefreshDisplay(string status) {
            if (statusLabel != null) {
                statusLabel.text = status;
            }

            UpdateTimerLabel();
            RefreshIngredientIcons();
        }

        private void UpdateTimerLabel() {
            if (timerLabel == null) {
                return;
            }

            timerLabel.text = CurrentOrder == null
                ? "Time: --"
                : $"Time: {CurrentOrder.OpenTime:0.0}s";
        }

        private void RefreshIngredientIcons() {
            foreach (OrderIngredientIconView iconView in spawnedIcons) {
                if (iconView != null) {
                    Destroy(iconView.gameObject);
                }
            }

            spawnedIcons.Clear();

            if (ingredientIconContainer == null || ingredientIconPrefab == null || CurrentOrder == null) {
                return;
            }

            for (int index = 0; index < CurrentOrder.RequiredIngredients.Count; index++) {
                OrderData.RequiredIngredient requirement = CurrentOrder.RequiredIngredients[index];
                OrderIngredientIconView iconView = Instantiate(ingredientIconPrefab, ingredientIconContainer);
                iconView.Bind(requirement.IngredientData.Icon, requirement.RequiredState);
                spawnedIcons.Add(iconView);
            }
        }

        private void NotifyContextChanged() {
            ContextActionsChanged?.Invoke();
        }
    }
}
