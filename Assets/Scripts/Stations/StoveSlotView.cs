using UnityEngine;
using YesChef.Ingredients;
using YesChef.UI;

namespace YesChef.Stations
{
    /// <summary>
    /// Handles world visuals and timer rendering for one stove slot.
    /// </summary>
    public sealed class StoveSlotView : MonoBehaviour
    {
        [SerializeField] private Transform itemAnchor;
        [SerializeField] private WorldTimerUI worldTimerUI;

        private GameObject spawnedVisual;
        private GameObject currentVisualPrefab;

        public void Refresh(StoveSlot slot)
        {
            if (slot == null || slot.IsEmpty())
            {
                ClearVisual();
                SetTimerVisible(false);
                return;
            }

            RefreshVisual(slot.Item);

            if (slot.IsCooking)
            {
                SetTimerVisible(true);
                worldTimerUI?.SetTime(slot.RemainingCookTime);
            }
            else
            {
                SetTimerVisible(false);
            }
        }

        private void RefreshVisual(IngredientInstance item)
        {
            GameObject targetPrefab = item?.Data?.GetPrefabForState(item.State);
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
            spawnedVisual = Object.Instantiate(targetPrefab, anchor);
            spawnedVisual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            spawnedVisual.transform.localScale = Vector3.one;
            currentVisualPrefab = targetPrefab;
        }

        private void SetTimerVisible(bool isVisible)
        {
            if (worldTimerUI != null)
            {
                worldTimerUI.SetVisible(isVisible);
            }
        }

        private void ClearVisual()
        {
            if (spawnedVisual != null)
            {
                Object.Destroy(spawnedVisual);
                spawnedVisual = null;
            }

            currentVisualPrefab = null;
        }
    }
}
