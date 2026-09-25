using System;

namespace AIEmotionWorld.AI
{
    [Serializable]
    public sealed class ChatMessage
    {
        public string role;
        public string content;

        public ChatMessage()
        {
        }

        public ChatMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }

    [Serializable]
    internal sealed class ChatCompletionRequest
    {
        public string model;
        public ChatMessage[] messages;
        public bool stream;
    }

    [Serializable]
    internal sealed class ChatCompletionResponse
    {
        public ChatCompletionChoice[] choices;
    }

    [Serializable]
    internal sealed class ChatCompletionChoice
    {
        public ChatMessage message;
    }

    [Serializable]
    internal sealed class ApiErrorResponse
    {
        public ApiError error;
    }

    [Serializable]
    internal sealed class ApiError
    {
        public string message;
    }
}
