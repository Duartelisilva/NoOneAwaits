using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveSetMessage : Objective
    {
        [Tooltip("Set this to the objective text that should appear")]
        public string ObjectiveMessage = "Find the exit";

        [Tooltip("Mark objective as complete after delay")]
        public bool AutoComplete = false;

        [Tooltip("Seconds to wait before completing the objective (if AutoComplete is true)")]
        public float AutoCompleteDelay = 3f;

        protected override void Start()
        {
            base.Start();

            if (string.IsNullOrEmpty(Title))
                Title = ObjectiveMessage;

            if (string.IsNullOrEmpty(Description))
                Description = "";

            UpdateObjective(Title, Description, "New objective: " + ObjectiveMessage);

            if (AutoComplete)
                Invoke(nameof(AutoCompleteObjective), AutoCompleteDelay);
        }

        public void SetMessage(string newMessage, bool autoComplete = false, float delay = 3f)
        {
            ObjectiveMessage = newMessage;
            Title = newMessage;
            UpdateObjective(Title, Description, "Objective: " + newMessage);

            if (autoComplete)
            {
                AutoComplete = true;
                AutoCompleteDelay = delay;
                CancelInvoke();
                Invoke(nameof(AutoCompleteObjective), AutoCompleteDelay);
            }
        }

        void AutoCompleteObjective()
        {
            CompleteObjective(Title, Description, "Objective complete: " + ObjectiveMessage);
        }
    }
}
