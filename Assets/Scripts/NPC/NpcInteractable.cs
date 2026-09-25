using UnityEngine;

namespace AIEmotionWorld.NPC
{
    public sealed class NpcInteractable : MonoBehaviour
    {
        [SerializeField] private string npcName = "Courtyard Visitor";
        [SerializeField] private float interactionRange = 2.5f;

        public string NpcName => npcName;
        public float InteractionRange => interactionRange;

        public void Interact(GameObject interactor)
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

            Debug.Log($"{npcName}: Hello, traveler.", this);
        }
    }
}
