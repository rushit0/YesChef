using UnityEngine;
using YesChef.Core.Interfaces;

namespace YesChef.Stations
{
    /// <summary>
    /// Base station implementation for counters, pans, ovens, and similar processors.
    /// Derived stations only need to describe their rules, while this class handles shared interaction flow.
    /// </summary>
    public abstract class ProcessStationBase : MonoBehaviour, IInteractable, IProcessStation
    {
        [SerializeField, Min(0.1f)] private float processDurationSeconds = 2f;
        [SerializeField] private Transform outputAnchor;

        private float elapsedProcessTime;
        private IHoldable currentHoldable;

        public bool IsProcessing { get; private set; }
        public float ProgressNormalized => processDurationSeconds <= 0f ? 0f : Mathf.Clamp01(elapsedProcessTime / processDurationSeconds);

        protected IHoldable CurrentHoldable => currentHoldable;
        protected Transform OutputAnchor => outputAnchor != null ? outputAnchor : transform;

        protected virtual void Update()
        {
            if (!IsProcessing)
            {
                return;
            }

            elapsedProcessTime += Time.deltaTime;
            if (elapsedProcessTime < processDurationSeconds)
            {
                return;
            }

            FinishProcessing(currentHoldable);
            CancelProcessing();
        }

        public virtual bool CanInteract(GameObject interactor)
        {
            return interactor != null;
        }

        public virtual void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor))
            {
                return;
            }

            Player.PlayerInteractor playerInteractor = interactor.GetComponent<Player.PlayerInteractor>();
            if (playerInteractor == null || !playerInteractor.TryGetHeldItem(out IHoldable holdable))
            {
                return;
            }

            if (!CanProcess(holdable))
            {
                return;
            }

            playerInteractor.ReleaseHeldItem(OutputAnchor.position);
            BeginProcessing(holdable);
        }

        public virtual bool CanProcess(IHoldable holdable)
        {
            return holdable != null && !IsProcessing;
        }

        public virtual void BeginProcessing(IHoldable holdable)
        {
            currentHoldable = holdable;
            elapsedProcessTime = 0f;
            IsProcessing = currentHoldable != null;
        }

        public virtual void CancelProcessing()
        {
            IsProcessing = false;
            elapsedProcessTime = 0f;
            currentHoldable = null;
        }

        protected abstract void FinishProcessing(IHoldable holdable);
    }
}
