using AIEmotionWorld.AI;
using UnityEngine;

namespace AIEmotionWorld.NPC
{
    [RequireComponent(typeof(NpcEmotionState))]
    public sealed class NpcInteractable : MonoBehaviour
    {
        [SerializeField] private string npcName = "Courtyard Visitor";
        [SerializeField] private float interactionRange = 2.5f;
        [SerializeField] private NpcEmotionState emotionState;

        public string NpcName => npcName;
        public float InteractionRange => interactionRange;
        public NpcEmotionState EmotionState => emotionState;

        private void Awake()
        {
            if (emotionState == null)
            {
                emotionState = GetComponent<NpcEmotionState>();
            }
        }

        public string GetGreeting()
        {
            return "Hello, traveler.";
        }

        public void ApplyEmotion(EmotionResult result)
        {
            emotionState?.ApplyEmotion(result);
        }

        public void FaceInteractor(GameObject interactor)
        {
            if (interactor == null)
            {
                return;
            }

            Vector3 direction = interactor.transform.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }
    }
}
