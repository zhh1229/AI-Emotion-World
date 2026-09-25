using AIEmotionWorld.NPC;
using AIEmotionWorld.UI;
using UnityEngine;

namespace AIEmotionWorld.Player
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private KeyCode interactionKey = KeyCode.E;
        [SerializeField] private float searchRadius = 6f;
        [SerializeField] private DialogueUI dialogueUI;

        public NpcInteractable CurrentInteractable { get; private set; }

        private void Awake()
        {
            if (dialogueUI == null)
            {
                dialogueUI = FindFirstObjectByType<DialogueUI>();
            }
        }

        private void Update()
        {
            CurrentInteractable = FindNearestInteractable();

            if (CurrentInteractable != null &&
                Input.GetKeyDown(interactionKey) &&
                dialogueUI != null &&
                !dialogueUI.IsOpen)
            {
                dialogueUI.Open(CurrentInteractable, gameObject);
            }
        }

        private NpcInteractable FindNearestInteractable()
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(
                transform.position,
                searchRadius,
                ~0,
                QueryTriggerInteraction.Collide);

            NpcInteractable nearest = null;
            float nearestDistanceSquared = float.PositiveInfinity;

            foreach (Collider nearbyCollider in nearbyColliders)
            {
                NpcInteractable candidate = nearbyCollider.GetComponentInParent<NpcInteractable>();

                if (candidate == null)
                {
                    continue;
                }

                Vector3 offset = candidate.transform.position - transform.position;
                offset.y = 0f;
                float distanceSquared = offset.sqrMagnitude;
                float rangeSquared = candidate.InteractionRange * candidate.InteractionRange;

                if (distanceSquared > rangeSquared || distanceSquared >= nearestDistanceSquared)
                {
                    continue;
                }

                nearest = candidate;
                nearestDistanceSquared = distanceSquared;
            }

            return nearest;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, searchRadius);
        }
    }
}
