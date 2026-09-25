using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIEmotionWorld.AI
{
    public static class DeepSeekEmotionResponseParser
    {
        public static bool TryParse(
            string content,
            out EmotionResult result,
            out string error)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                result = default;
                error = "AI response was empty.";
                return false;
            }

            JObject root;

            try
            {
                root = JObject.Parse(content);
            }
            catch (JsonException)
            {
                result = default;
                error = "AI response was not valid JSON.";
                return false;
            }

            JToken emotionToken = root["emotion"];

            if (emotionToken == null || emotionToken.Type != JTokenType.String)
            {
                result = default;
                error = "AI response is missing a valid emotion.";
                return false;
            }

            if (!TryParseEmotion(emotionToken.Value<string>(), out EmotionType emotion))
            {
                result = default;
                error = "AI response contains an unknown emotion.";
                return false;
            }

            JToken intensityToken = root["intensity"];

            if (intensityToken == null ||
                (intensityToken.Type != JTokenType.Integer &&
                 intensityToken.Type != JTokenType.Float))
            {
                result = default;
                error = "AI response is missing a numeric intensity.";
                return false;
            }

            float intensity = intensityToken.Value<float>();

            if (float.IsNaN(intensity) ||
                float.IsInfinity(intensity) ||
                intensity < 0f ||
                intensity > 1f)
            {
                result = default;
                error = "AI intensity must be between 0 and 1.";
                return false;
            }

            JToken replyToken = root["reply"];

            if (replyToken == null ||
                replyToken.Type != JTokenType.String ||
                string.IsNullOrWhiteSpace(replyToken.Value<string>()))
            {
                result = default;
                error = "AI response is missing a valid reply.";
                return false;
            }

            result = new EmotionResult(emotion, intensity, replyToken.Value<string>().Trim());
            error = null;
            return true;
        }

        private static bool TryParseEmotion(string value, out EmotionType emotion)
        {
            switch (value?.Trim().ToLowerInvariant())
            {
                case "happy":
                    emotion = EmotionType.Happy;
                    return true;
                case "sad":
                    emotion = EmotionType.Sad;
                    return true;
                case "angry":
                    emotion = EmotionType.Angry;
                    return true;
                case "calm":
                    emotion = EmotionType.Calm;
                    return true;
                case "neutral":
                    emotion = EmotionType.Neutral;
                    return true;
                default:
                    emotion = default;
                    return false;
            }
        }
    }
}
