using System;

namespace YesChef.Ingredients {
    /// <summary>
    /// Plain runtime ingredient model carried by the player or owned by gameplay systems.
    /// It has no Unity scene dependency, which keeps business logic portable and easy to test.
    /// </summary>
    [Serializable]
    public class IngredientInstance {
        public IngredientInstance(IngredientData data, IngredientProcessState initialState = IngredientProcessState.Raw) {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            State = initialState;
        }

        public IngredientData Data { get; }
        public IngredientProcessState State { get; private set; }

        public event Action<IngredientProcessState> StateChanged;

        public void SetState(IngredientProcessState newState) {
            if (State == newState) {
                return;
            }

            State = newState;
            StateChanged?.Invoke(State);
        }
    }
}
