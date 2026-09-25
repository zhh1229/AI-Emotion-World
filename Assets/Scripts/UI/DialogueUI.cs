using AIEmotionWorld.AI;
using AIEmotionWorld.NPC;
using AIEmotionWorld.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AIEmotionWorld.UI
{
    public sealed class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text responseText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private DeepSeekChatClient chatClient;

        private NpcInteractable currentNpc;
        private GameObject currentInteractor;

        public bool IsOpen => dialoguePanel != null && dialoguePanel.activeSelf;

        private void Awake()
        {
            if (dialoguePanel == null ||
                speakerText == null ||
                responseText == null ||
                inputField == null ||
                sendButton == null ||
                chatClient == null)
            {
                Debug.LogError("Dialogue UI references are incomplete.", this);
                enabled = false;
                return;
            }

            sendButton.onClick.AddListener(SubmitMessage);
            inputField.onSubmit.AddListener(_ => SubmitMessage());
            dialoguePanel.SetActive(false);
        }

        private void Update()
        {
            if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        public void Open(NpcInteractable npc, GameObject interactor)
        {
            if (npc == null)
            {
                return;
            }

            currentNpc = npc;
            currentInteractor = interactor;
            currentNpc.FaceInteractor(currentInteractor);

            speakerText.text = currentNpc.NpcName;
            responseText.text = currentNpc.GetGreeting();
            inputField.text = string.Empty;
            dialoguePanel.SetActive(true);
            playerController?.SetMovementEnabled(false);

            EventSystem.current?.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();
        }

        public void Close()
        {
            dialoguePanel.SetActive(false);
            currentNpc = null;
            currentInteractor = null;
            playerController?.SetMovementEnabled(true);
            EventSystem.current?.SetSelectedGameObject(null);
        }

        private void SubmitMessage()
        {
            if (!IsOpen || currentNpc == null)
            {
                return;
            }

            string playerMessage = inputField.text.Trim();

            if (string.IsNullOrEmpty(playerMessage))
            {
                return;
            }

            string npcName = currentNpc.NpcName;
            inputField.text = string.Empty;
            responseText.text = $"{npcName}: Thinking...";
            SetInputEnabled(false);

            chatClient.SendChat(
                playerMessage,
                result => HandleReply(npcName, result.Reply),
                error => HandleRequestError(error));
        }

        private void HandleReply(string npcName, string reply)
        {
            responseText.text = $"{npcName}: {reply}";
            SetInputEnabled(true);
            EventSystem.current?.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();
        }

        private void HandleRequestError(string error)
        {
            responseText.text = error;
            SetInputEnabled(true);
            EventSystem.current?.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();
        }

        private void SetInputEnabled(bool enabled)
        {
            inputField.interactable = enabled;
            sendButton.interactable = enabled;
        }
    }
}
