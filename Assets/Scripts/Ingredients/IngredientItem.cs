using UnityEngine;
using YesChef.Core.Interfaces;

namespace YesChef.Ingredients
{
    /// <summary>
    /// Runtime world representation of an ingredient.
    /// It implements the holdable contract so players and stations can use the same item flow.
    /// </summary>
    public sealed class IngredientItem : MonoBehaviour, IHoldable
    {
        [SerializeField] private IngredientDefinition definition;

        public IngredientDefinition Definition => definition;
        public Transform Transform => transform;
        public bool IsHeld { get; private set; }

        public void OnPickedUp(Transform holdPoint)
        {
            IsHeld = true;
            transform.SetParent(holdPoint);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void OnDropped(Vector3 worldPosition)
        {
            IsHeld = false;
            transform.SetParent(null);
            transform.position = worldPosition;
        }
    }
}
