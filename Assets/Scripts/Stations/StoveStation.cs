using UnityEngine;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    /// <summary>
    /// Cooks up to two raw meat ingredients independently and lets the player collect completed items.
    /// </summary>
    public sealed class StoveStation : StationBase
    {
        private const float CookDurationSeconds = 6f;

        [SerializeField] private StoveSlot[] slots = { new(), new() };

        private void Reset()
        {
            if (slots == null || slots.Length != 2)
            {
                slots = new[] { new StoveSlot(), new StoveSlot() };
            }
        }

        private void Update()
        {
            if (slots == null)
            {
                return;
            }

            foreach (StoveSlot slot in slots)
            {
                if (slot == null || !slot.IsCooking)
                {
                    continue;
                }

                slot.Tick(Time.deltaTime);
                if (slot.IsReady)
                {
                    slot.Item.SetState(IngredientProcessState.Prepared);
                }
            }
        }

        public override void Interact(GameObject interactor)
        {
            if (!TryGetCarryController(interactor, out PlayerCarryController carryController))
            {
                return;
            }

            if (!carryController.HasItem())
            {
                TryCollectCookedItem(carryController);
                return;
            }

            IngredientInstance heldItem = PeekHeldItem(carryController);
            if (!IsIngredient(heldItem, IngredientType.Meat, IngredientProcessState.Raw))
            {
                return;
            }

            StoveSlot emptySlot = FindEmptySlot();
            if (emptySlot == null)
            {
                return;
            }

            emptySlot.Assign(TakeHeldItem(carryController), CookDurationSeconds);
        }

        private void TryCollectCookedItem(PlayerCarryController carryController)
        {
            StoveSlot readySlot = FindReadySlot();
            if (readySlot == null)
            {
                return;
            }

            IngredientInstance cookedItem = readySlot.Remove();
            if (!TryGiveItem(carryController, cookedItem))
            {
                readySlot.Assign(cookedItem, 0f);
            }
        }

        private StoveSlot FindEmptySlot()
        {
            foreach (StoveSlot slot in slots)
            {
                if (slot != null && slot.IsEmpty())
                {
                    return slot;
                }
            }

            return null;
        }

        private StoveSlot FindReadySlot()
        {
            foreach (StoveSlot slot in slots)
            {
                if (slot != null && slot.IsReady)
                {
                    return slot;
                }
            }

            return null;
        }
    }
}
