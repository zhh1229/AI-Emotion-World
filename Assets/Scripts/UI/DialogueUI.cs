using AIEmotionWorld.AI;
using AIEmotionWorld.NPC;
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
        [SerializeField] private EmotionConversationService conversationService;
        [SerializeField] private ChineseFontAssetProvider fontProvider;

        private NpcInteractable currentNpc;
        private GameObject currentInteractor;
        private TMP_FontAsset activeFont;

        public bool IsOpen => dialoguePanel != null && dialoguePanel.activeSelf;

        private void Awake()
        {
            if (dialoguePanel == null ||
                speakerText == null ||
                responseText == null ||
                inputField == null ||
                sendButton == null ||
                conversationService == null ||
                fontProvider == null)
            {
                Debug.LogError("Dialogue UI references are incomplete.", this);
                enabled = false;
                return;
            }

            ApplyChineseFont(fontProvider.GetFontAsset());
            inputField.onValueChanged.AddListener(EnsureFontCharacters);
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

            EventSystem.current?.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();
        }

        public void Close()
        {
            dialoguePanel.SetActive(false);
            currentNpc = null;
            currentInteractor = null;
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

            EnsureFontCharacters(playerMessage);
            string npcName = currentNpc.NpcName;
            inputField.text = string.Empty;
            responseText.text = $"{npcName}: Thinking...";
            SetInputEnabled(false);
            NpcInteractable targetNpc = currentNpc;

            conversationService.RequestEmotion(
                playerMessage,
                result => HandleReply(npcName, targetNpc, result),
                error => HandleRequestError(error));
        }

        private void HandleReply(
            string npcName,
            NpcInteractable targetNpc,
            EmotionResult result)
        {
            targetNpc?.ApplyEmotion(result);
            EnsureFontCharacters(result.Reply);
            responseText.text = $"{npcName}: {result.Reply}";
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

        private void ApplyChineseFont(TMP_FontAsset font)
        {
            if (font == null)
            {
                return;
            }

            activeFont = font;
            EnsureFontCharacters("中文测试你好庭院情绪开心悲伤愤怒平静中性，。！？");
            speakerText.font = font;
            responseText.font = font;
            inputField.textComponent.font = font;

            if (inputField.placeholder is TMP_Text placeholderText)
            {
                placeholderText.font = font;
            }

            TMP_Text buttonLabel = sendButton.GetComponentInChildren<TMP_Text>(true);

            if (buttonLabel != null)
            {
                buttonLabel.font = font;
            }
        }

        private void EnsureFontCharacters(string text)
        {
            if (activeFont == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            activeFont.TryAddCharacters(text, out _, true);
        }
    }
}
