using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Core;
using YesChef.Managers;

namespace YesChef.UI
{
    /// <summary>
    /// Start menu popup that explains controls and begins the round.
    /// </summary>
    public sealed class StartPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text controlsLabel;
        [SerializeField] private Button startButton;

        private void Awake()
        {
            if (controlsLabel != null)
            {
                controlsLabel.text = "Move: WASD / Arrow Keys\nInteract: E\nPause: Esc";
            }
        }

        private void OnEnable()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(HandleStartClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameFlowManager.StateChanged += HandleStateChanged;
                HandleStateChanged(GameManager.Instance.GameFlowManager.CurrentState);
            }
        }

        private void OnDisable()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(HandleStartClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameFlowManager.StateChanged -= HandleStateChanged;
            }
        }

        private void HandleStartClicked()
        {
            GameManager.Instance?.GameFlowManager.StartGame();
        }

        private void HandleStateChanged(GameFlowManager.GameState state)
        {
            gameObject.SetActive(state == GameFlowManager.GameState.StartMenu);
        }
    }
}
