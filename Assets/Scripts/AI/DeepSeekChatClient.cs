using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AIEmotionWorld.AI
{
    public sealed class DeepSeekChatClient : MonoBehaviour
    {
        [SerializeField] private int timeoutSeconds = 30;

        private DeepSeekConnectionSettings runtimeSettings;
        private bool hasRuntimeSettings;

        public bool IsRequestInProgress { get; private set; }
        public string LastContent { get; private set; }
        public string LastError { get; private set; }

#if UNITY_EDITOR
        public void ConfigureForTesting(string apiUrl, string apiKey, string model)
        {
            runtimeSettings = new DeepSeekConnectionSettings(apiKey, apiUrl, model);
            hasRuntimeSettings = true;
        }

        public void ClearRuntimeConfiguration()
        {
            runtimeSettings = default;
            hasRuntimeSettings = false;
        }
#endif

        public void SendChat(
            string playerMessage,
            Action<string> onSuccess,
            Action<string> onError)
        {
            if (IsRequestInProgress)
            {
                onError?.Invoke("An AI request is already in progress.");
                return;
            }

            if (!TryGetSettings(out DeepSeekConnectionSettings settings, out string configError))
            {
                ReportFailure(configError, onError);
                return;
            }

            ChatCompletionRequest requestBody = new ChatCompletionRequest
            {
                model = settings.Model,
                stream = false,
                messages = new[]
                {
                    new ChatMessage(
                        "system",
                        "You are a friendly NPC in a small 3D courtyard. " +
                        "Return only one valid JSON object with exactly these fields: " +
                        "\"emotion\" (one of happy, sad, angry, calm, neutral), " +
                        "\"intensity\" (a number from 0 to 1), and " +
                        "\"reply\" (a short answer in the player's language). " +
                        "Do not use Markdown or code fences. Do not add any text outside the JSON object."),
                    new ChatMessage("user", playerMessage)
                }
            };

            IsRequestInProgress = true;
            LastContent = null;
            LastError = null;
            StartCoroutine(SendChatRequest(requestBody, settings, onSuccess, onError));
        }

        private IEnumerator SendChatRequest(
            ChatCompletionRequest requestBody,
            DeepSeekConnectionSettings settings,
            Action<string> onSuccess,
            Action<string> onError)
        {
            string json = JsonUtility.ToJson(requestBody);

            using (UnityWebRequest request = new UnityWebRequest(settings.ApiUrl, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = timeoutSeconds;
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {settings.ApiKey}");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string error = BuildHttpError(request);
                    ReportFailure(error, onError);
                    yield break;
                }

                if (!TryGetAssistantContent(
                        request.downloadHandler.text,
                        out string content,
                        out string parseError))
                {
                    ReportFailure(parseError, onError);
                    yield break;
                }

                IsRequestInProgress = false;
                LastContent = content;
                LastError = null;
                onSuccess?.Invoke(content);
            }
        }

        private bool TryGetSettings(
            out DeepSeekConnectionSettings settings,
            out string error)
        {
            if (hasRuntimeSettings)
            {
                settings = runtimeSettings;
                error = null;
                return true;
            }

            return DeepSeekApiSettings.TryLoad(out settings, out error);
        }

        private static string BuildHttpError(UnityWebRequest request)
        {
            string responseBody = request.downloadHandler?.text;

            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                try
                {
                    ApiErrorResponse apiError = JsonUtility.FromJson<ApiErrorResponse>(responseBody);

                    if (!string.IsNullOrWhiteSpace(apiError?.error?.message))
                    {
                        return $"AI request failed: {apiError.error.message}";
                    }
                }
                catch (ArgumentException)
                {
                    // Fall through to the transport error when the body is not JSON.
                }
            }

            string transportError = string.IsNullOrWhiteSpace(request.error)
                ? "Unknown transport error"
                : request.error;

            return $"AI request failed ({request.responseCode}): {transportError}";
        }

        private static bool TryGetAssistantContent(
            string responseJson,
            out string content,
            out string error)
        {
            if (string.IsNullOrWhiteSpace(responseJson))
            {
                content = null;
                error = "AI response was empty.";
                return false;
            }

            try
            {
                ChatCompletionResponse response =
                    JsonUtility.FromJson<ChatCompletionResponse>(responseJson);
                content = response?.choices is { Length: > 0 }
                    ? response.choices[0]?.message?.content
                    : null;

                if (string.IsNullOrWhiteSpace(content))
                {
                    error = "AI response did not contain a message.";
                    return false;
                }

                content = content.Trim();
                error = null;
                return true;
            }
            catch (ArgumentException)
            {
                content = null;
                error = "AI response was not valid JSON.";
                return false;
            }
        }

        private void ReportFailure(string error, Action<string> onError)
        {
            IsRequestInProgress = false;
            LastContent = null;
            LastError = error;
            Debug.LogWarning(error, this);
            onError?.Invoke(error);
        }
    }
}
