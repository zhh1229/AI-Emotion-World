using System;
using UnityEngine;

namespace AIEmotionWorld.AI
{
    [RequireComponent(typeof(DeepSeekChatClient))]
    public sealed class EmotionConversationService : MonoBehaviour
    {
        [SerializeField] private DeepSeekChatClient chatClient;

        public bool IsRequestInProgress =>
            chatClient != null && chatClient.IsRequestInProgress;

        public bool HasLastResult { get; private set; }
        public EmotionResult LastResult { get; private set; }
        public string LastError { get; private set; }

        private void Awake()
        {
            if (chatClient == null)
            {
                chatClient = GetComponent<DeepSeekChatClient>();
            }
        }

        public void RequestEmotion(
            string playerMessage,
            Action<EmotionResult> onSuccess,
            Action<string> onError)
        {
            if (chatClient == null)
            {
                ReportFailure("DeepSeek chat client is not configured.", onError);
                return;
            }

            HasLastResult = false;
            LastError = null;

            chatClient.SendChat(
                playerMessage,
                content => HandleContent(content, onSuccess, onError),
                error => ReportFailure(error, onError));
        }

#if UNITY_EDITOR
        public void ConfigureForTesting(string apiUrl, string apiKey, string model)
        {
            chatClient?.ConfigureForTesting(apiUrl, apiKey, model);
        }

        public void ClearRuntimeConfiguration()
        {
            chatClient?.ClearRuntimeConfiguration();
        }
#endif

        private void HandleContent(
            string content,
            Action<EmotionResult> onSuccess,
            Action<string> onError)
        {
            if (!DeepSeekEmotionResponseParser.TryParse(
                    content,
                    out EmotionResult result,
                    out string parseError))
            {
                ReportFailure(parseError, onError);
                return;
            }

            HasLastResult = true;
            LastResult = result;
            LastError = null;
            onSuccess?.Invoke(result);
        }

        private void ReportFailure(string error, Action<string> onError)
        {
            HasLastResult = false;
            LastResult = default;
            LastError = error;
            onError?.Invoke(error);
        }
    }
}
