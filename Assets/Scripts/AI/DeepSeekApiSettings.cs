using System;
using System.IO;
using UnityEngine;

namespace AIEmotionWorld.AI
{
    [Serializable]
    internal sealed class DeepSeekConfigFile
    {
        public string apiKey;
        public string apiUrl;
        public string model;
    }

    public readonly struct DeepSeekConnectionSettings
    {
        public string ApiKey { get; }
        public string ApiUrl { get; }
        public string Model { get; }

        public DeepSeekConnectionSettings(string apiKey, string apiUrl, string model)
        {
            ApiKey = apiKey;
            ApiUrl = apiUrl;
            Model = model;
        }
    }

    public static class DeepSeekApiSettings
    {
        public const string ApiKeyEnvironmentVariable = "DEEPSEEK_API_KEY";
        public const string ApiUrlEnvironmentVariable = "DEEPSEEK_API_URL";
        public const string ModelEnvironmentVariable = "DEEPSEEK_MODEL";
        public const string DefaultApiUrl = "https://api.deepseek.com/chat/completions";
        public const string DefaultModel = "deepseek-v4-flash";

        public static bool TryLoad(out DeepSeekConnectionSettings settings, out string error)
        {
            string apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
            string apiUrl = Environment.GetEnvironmentVariable(ApiUrlEnvironmentVariable);
            string model = Environment.GetEnvironmentVariable(ModelEnvironmentVariable);

            string configPath = Path.Combine(
                Application.persistentDataPath,
                "deepseek-config.json");

            if (string.IsNullOrWhiteSpace(apiKey) && File.Exists(configPath))
            {
                try
                {
                    DeepSeekConfigFile config =
                        JsonUtility.FromJson<DeepSeekConfigFile>(File.ReadAllText(configPath));

                    if (config != null)
                    {
                        apiKey = string.IsNullOrWhiteSpace(apiKey) ? config.apiKey : apiKey;
                        apiUrl = string.IsNullOrWhiteSpace(apiUrl) ? config.apiUrl : apiUrl;
                        model = string.IsNullOrWhiteSpace(model) ? config.model : model;
                    }
                }
                catch (Exception exception)
                {
                    settings = default;
                    error = $"Could not read local DeepSeek config: {exception.Message}";
                    return false;
                }
            }

            apiUrl = string.IsNullOrWhiteSpace(apiUrl) ? DefaultApiUrl : apiUrl.Trim();
            model = string.IsNullOrWhiteSpace(model) ? DefaultModel : model.Trim();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                settings = default;
                error =
                    $"Missing {ApiKeyEnvironmentVariable}. Set the environment variable " +
                    $"or create {configPath} with an apiKey field.";
                return false;
            }

            settings = new DeepSeekConnectionSettings(apiKey.Trim(), apiUrl, model);
            error = null;
            return true;
        }
    }
}
