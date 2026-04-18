using TMPro;
using UnityEngine;

namespace YesChef.UI
{
    /// <summary>
    /// Displays a world-space countdown for station processing.
    /// </summary>
    public sealed class WorldTimerUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text timerLabel;

        public void SetVisible(bool isVisible)
        {
            GameObject target = root != null ? root : gameObject;
            target.SetActive(isVisible);
        }

        public void SetTime(float remainingSeconds)
        {
            if (timerLabel == null)
            {
                return;
            }

            timerLabel.text = $"{Mathf.Max(0f, remainingSeconds):0.0}s";
        }
    }
}
