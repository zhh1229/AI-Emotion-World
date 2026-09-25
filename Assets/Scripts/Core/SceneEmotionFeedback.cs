using AIEmotionWorld.AI;
using AIEmotionWorld.NPC;
using UnityEngine;

namespace AIEmotionWorld.Core
{
    [RequireComponent(typeof(NpcEmotionState))]
    public sealed class SceneEmotionFeedback : MonoBehaviour
    {
        [SerializeField] private NpcEmotionState emotionState;
        [SerializeField] private Light directionalLight;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Renderer groundRenderer;
        [SerializeField] private float transitionSpeed = 4f;

        private Material groundMaterial;
        private Color originalLightColor;
        private float originalLightIntensity;
        private Color originalAmbientColor;
        private Color originalCameraColor;
        private CameraClearFlags originalClearFlags;
        private Color originalGroundColor;
        private Color originalFogColor;
        private float originalFogDensity;
        private bool originalFogEnabled;

        private Color currentLightColor;
        private float currentLightIntensity;
        private Color currentAmbientColor;
        private Color currentCameraColor;
        private Color currentGroundColor;
        private Color currentFogColor;
        private float currentFogDensity;
        private float currentEffectBlend;

        private void Awake()
        {
            if (emotionState == null)
            {
                emotionState = GetComponent<NpcEmotionState>();
            }

            if (directionalLight == null)
            {
                directionalLight = RenderSettings.sun;
            }

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (groundRenderer != null)
            {
                groundMaterial = groundRenderer.material;
            }

            CacheOriginalSettings();
            ResetCurrentSettings();
            emotionState.EmotionChanged += HandleEmotionChanged;
            AnimateFeedback(1f);
        }

        private void OnDestroy()
        {
            if (emotionState != null)
            {
                emotionState.EmotionChanged -= HandleEmotionChanged;
            }
        }

        private void Update()
        {
            if (emotionState == null)
            {
                return;
            }

            float transition = 1f - Mathf.Exp(-transitionSpeed * Time.deltaTime);
            AnimateFeedback(transition);
        }

        private void HandleEmotionChanged(EmotionResult result)
        {
            AnimateFeedback(1f);
        }

        private void AnimateFeedback(float transition)
        {
            GetTargetSettings(
                emotionState.CurrentEmotion,
                emotionState.CurrentIntensity,
                out Color targetLightColor,
                out float targetLightIntensity,
                out Color targetAmbientColor,
                out Color targetCameraColor,
                out Color targetGroundColor,
                out Color targetFogColor,
                out float targetFogDensity,
                out float targetBlend);

            currentLightColor = Color.Lerp(currentLightColor, targetLightColor, transition);
            currentLightIntensity = Mathf.Lerp(
                currentLightIntensity,
                targetLightIntensity,
                transition);
            currentAmbientColor = Color.Lerp(currentAmbientColor, targetAmbientColor, transition);
            currentCameraColor = Color.Lerp(currentCameraColor, targetCameraColor, transition);
            currentGroundColor = Color.Lerp(currentGroundColor, targetGroundColor, transition);
            currentFogColor = Color.Lerp(currentFogColor, targetFogColor, transition);
            currentFogDensity = Mathf.Lerp(currentFogDensity, targetFogDensity, transition);
            currentEffectBlend = Mathf.Lerp(currentEffectBlend, targetBlend, transition);

            ApplyCurrentSettings();
        }

        private void CacheOriginalSettings()
        {
            originalLightColor = directionalLight != null
                ? directionalLight.color
                : Color.white;
            originalLightIntensity = directionalLight != null
                ? directionalLight.intensity
                : 1f;
            originalAmbientColor = RenderSettings.ambientLight;
            originalCameraColor = mainCamera != null ? mainCamera.backgroundColor : Color.black;
            originalClearFlags = mainCamera != null
                ? mainCamera.clearFlags
                : CameraClearFlags.Skybox;
            originalGroundColor = groundMaterial != null ? groundMaterial.color : Color.white;
            originalFogColor = RenderSettings.fogColor;
            originalFogDensity = RenderSettings.fogDensity;
            originalFogEnabled = RenderSettings.fog;
        }

        private void ResetCurrentSettings()
        {
            currentLightColor = originalLightColor;
            currentLightIntensity = originalLightIntensity;
            currentAmbientColor = originalAmbientColor;
            currentCameraColor = originalCameraColor;
            currentGroundColor = originalGroundColor;
            currentFogColor = originalFogColor;
            currentFogDensity = originalFogDensity;
            currentEffectBlend = 0f;
        }

        private void GetTargetSettings(
            EmotionType emotion,
            float intensity,
            out Color lightColor,
            out float lightIntensity,
            out Color ambientColor,
            out Color cameraColor,
            out Color groundColor,
            out Color fogColor,
            out float fogDensity,
            out float blend)
        {
            lightColor = originalLightColor;
            lightIntensity = originalLightIntensity;
            ambientColor = originalAmbientColor;
            cameraColor = originalCameraColor;
            groundColor = originalGroundColor;
            fogColor = originalFogColor;
            fogDensity = 0f;
            blend = 0f;

            float effectStrength = Mathf.Lerp(0.55f, 1f, Mathf.Clamp01(intensity));

            switch (emotion)
            {
                case EmotionType.Happy:
                    lightColor = new Color(1f, 0.72f, 0.32f, 1f);
                    lightIntensity = 1.65f;
                    ambientColor = new Color(0.52f, 0.34f, 0.16f, 1f);
                    cameraColor = new Color(0.38f, 0.72f, 0.95f, 1f);
                    groundColor = new Color(0.62f, 0.72f, 0.26f, 1f);
                    fogColor = cameraColor;
                    fogDensity = 0f;
                    blend = effectStrength;
                    break;
                case EmotionType.Sad:
                    lightColor = new Color(0.43f, 0.60f, 0.82f, 1f);
                    lightIntensity = 0.45f;
                    ambientColor = new Color(0.12f, 0.18f, 0.28f, 1f);
                    cameraColor = new Color(0.10f, 0.16f, 0.25f, 1f);
                    groundColor = new Color(0.26f, 0.39f, 0.35f, 1f);
                    fogColor = cameraColor;
                    fogDensity = 0.016f;
                    blend = effectStrength;
                    break;
                case EmotionType.Angry:
                    lightColor = new Color(1f, 0.25f, 0.18f, 1f);
                    lightIntensity = 1.85f;
                    ambientColor = new Color(0.34f, 0.08f, 0.06f, 1f);
                    cameraColor = new Color(0.28f, 0.06f, 0.05f, 1f);
                    groundColor = new Color(0.55f, 0.30f, 0.19f, 1f);
                    fogColor = cameraColor;
                    fogDensity = 0.010f;
                    blend = effectStrength;
                    break;
                case EmotionType.Calm:
                    lightColor = new Color(0.48f, 0.84f, 0.70f, 1f);
                    lightIntensity = 1.05f;
                    ambientColor = new Color(0.18f, 0.34f, 0.27f, 1f);
                    cameraColor = new Color(0.34f, 0.64f, 0.72f, 1f);
                    groundColor = new Color(0.36f, 0.62f, 0.30f, 1f);
                    fogColor = cameraColor;
                    fogDensity = 0f;
                    blend = effectStrength;
                    break;
            }
        }

        private void ApplyCurrentSettings()
        {
            if (directionalLight != null)
            {
                directionalLight.color = currentLightColor;
                directionalLight.intensity = currentLightIntensity;
            }

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = currentAmbientColor;
            RenderSettings.fog = currentEffectBlend >= 0.5f;
            RenderSettings.fogColor = currentFogColor;
            RenderSettings.fogDensity = currentFogDensity;

            if (mainCamera != null)
            {
                mainCamera.clearFlags = currentEffectBlend < 0.01f
                    ? originalClearFlags
                    : CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = currentCameraColor;
            }

            if (groundMaterial != null)
            {
                groundMaterial.color = currentGroundColor;
            }
        }
    }
}
