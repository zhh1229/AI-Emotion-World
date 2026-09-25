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
        public string LastReply { get; private set; }
        public string LastError { get; private set; }

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

        public void SendChat(string playerMessage, Action<string> onSuccess, Action<string> onError)
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
                        "Reply in the same language as the player. Keep answers under two short sentences."),
                    new ChatMessage("user", playerMessage)
                }
            };

            IsRequestInProgress = true;
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

                if (!TryParseReply(request.downloadHandler.text, out string reply, out string parseError))
                {
                    ReportFailure(parseError, onError);
                    yield break;
                }

                IsRequestInProgress = false;
                LastReply = reply;
                LastError = null;
                onSuccess?.Invoke(reply);
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

        private static bool TryParseReply(string responseJson, out string reply, out string error)
        {
            if (string.IsNullOrWhiteSpace(responseJson))
            {
                reply = null;
                error = "AI response was empty.";
                return false;
            }

            try
            {
                ChatCompletionResponse response =
                    JsonUtility.FromJson<ChatCompletionResponse>(responseJson);
                string content = response?.choices is { Length: > 0 }
                    ? response.choices[0]?.message?.content
                    : null;

                if (string.IsNullOrWhiteSpace(content))
                {
                    reply = null;
                    error = "AI response did not contain a message.";
                    return false;
                }

                reply = content.Trim();
                error = null;
                return true;
            }
            catch (ArgumentException)
            {
                reply = null;
                error = "AI response was not valid JSON.";
                return false;
            }
        }

        private void ReportFailure(string error, Action<string> onError)
        {
            IsRequestInProgress = false;
            LastReply = null;
            LastError = error;
            Debug.LogWarning(error, this);
            onError?.Invoke(error);
        }
    }
}
