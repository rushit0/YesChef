using UnityEngine;
using YesChef.Core.Interfaces;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    /// <summary>
    /// Cooks up to two meats independently and exposes contextual place/pick actions.
    /// </summary>
    public sealed class StoveStation : BaseStationUIController
    {
        private const float CookDurationSeconds = 6f;

        private StoveSlot[] slots = { new(), new() };
        [SerializeField] private StoveSlotView[] slotViews = new StoveSlotView[2];

        public override string PopupTitle => "Stove";

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

                bool wasCooking = slot.IsCooking;
                slot.Tick(Time.deltaTime);
                if (wasCooking && slot.IsReady)
                {
                    slot.Item.SetState(IngredientProcessState.Prepared);
                    NotifyContextChanged();
                }
            }

            RefreshViews();
        }

        private void OnDisable()
        {
            RefreshViews();
        }

        public override void GetContextActions(PlayerCarryController playerCarryController, System.Collections.Generic.List<ContextActionData> actions)
        {
            if (playerCarryController == null)
            {
                return;
            }

            if (CanPlaceRawMeat(playerCarryController))
            {
                StoveSlot emptySlot = FindEmptySlot();
                if (emptySlot != null)
                {
                    actions.Add(new ContextActionData("Place Meat", true, () => PlaceRawMeat(playerCarryController, emptySlot)));
                }
            }

            if (CanPickupCookedMeat(playerCarryController))
            {
                actions.Add(new ContextActionData("Pick Cooked Meat", true, () => TryCollectCookedItem(playerCarryController)));
            }
        }

        private void PlaceRawMeat(PlayerCarryController carryController, StoveSlot targetSlot)
        {
            if (carryController == null || targetSlot == null || !targetSlot.IsEmpty())
            {
                return;
            }

            IngredientInstance heldItem = PeekHeldItem(carryController);
            if (!IsIngredient(heldItem, IngredientType.Meat, IngredientProcessState.Raw))
            {
                return;
            }

            targetSlot.Assign(TakeHeldItem(carryController), CookDurationSeconds);
            NotifyContextChanged();
            RefreshViews();
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
                return;
            }

            NotifyContextChanged();
            RefreshViews();
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

        private bool HasEmptySlot()
        {
            return FindEmptySlot() != null;
        }

        private bool HasReadySlot()
        {
            return FindReadySlot() != null;
        }

        private bool CanPlaceRawMeat(PlayerCarryController playerCarryController)
        {
            if (playerCarryController == null || !HasEmptySlot())
            {
                return false;
            }

            IngredientInstance heldItem = PeekHeldItem(playerCarryController);
            return IsIngredient(heldItem, IngredientType.Meat, IngredientProcessState.Raw);
        }

        private bool CanPickupCookedMeat(PlayerCarryController playerCarryController)
        {
            return playerCarryController != null && !playerCarryController.HasItem() && HasReadySlot();
        }

        private void RefreshViews()
        {
            if (slotViews == null)
            {
                return;
            }

            for (int index = 0; index < slotViews.Length; index++)
            {
                if (slotViews[index] == null)
                {
                    continue;
                }

                StoveSlot slot = index < slots.Length ? slots[index] : null;
                slotViews[index].Refresh(slot);
            }
        }
    }
}
