using System.Collections;
using UnityEngine;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    /// <summary>
    /// Prepares raw vegetables and returns the prepared ingredient to the player when possible.
    /// </summary>
    public sealed class TableStation : StationBase
    {
        private const float PrepareDurationSeconds = 2f;

        private IngredientInstance preparedItem;
        private PlayerCarryController processingPlayer;
        private Coroutine processingRoutine;

        public bool IsProcessing => processingRoutine != null;

        private void OnDisable()
        {
            if (processingRoutine != null)
            {
                StopCoroutine(processingRoutine);
                processingRoutine = null;
            }
        }

        public override void Interact(GameObject interactor)
        {
            if (!TryGetCarryController(interactor, out PlayerCarryController carryController))
            {
                return;
            }

            if (preparedItem != null)
            {
                TryServePreparedItem(carryController);
                return;
            }

            if (IsProcessing)
            {
                return;
            }

            IngredientInstance heldItem = PeekHeldItem(carryController);
            if (!IsIngredient(heldItem, IngredientType.Vegetable, IngredientProcessState.Raw))
            {
                return;
            }

            preparedItem = TakeHeldItem(carryController);
            processingPlayer = carryController;
            processingRoutine = StartCoroutine(ProcessVegetableRoutine());
        }

        private IEnumerator ProcessVegetableRoutine()
        {
            yield return new WaitForSeconds(PrepareDurationSeconds);

            if (preparedItem != null)
            {
                preparedItem.SetState(IngredientProcessState.Prepared);
                TryServePreparedItem(processingPlayer);
            }

            processingPlayer = null;
            processingRoutine = null;
        }

        private void TryServePreparedItem(PlayerCarryController carryController)
        {
            if (preparedItem == null || carryController == null)
            {
                return;
            }

            if (!TryGiveItem(carryController, preparedItem))
            {
                return;
            }

            preparedItem = null;
        }
    }
}
