using AIEmotionWorld.AI;
using UnityEngine;

namespace AIEmotionWorld.NPC
{
    [RequireComponent(typeof(NpcEmotionState))]
    public sealed class NpcEmotionReaction : MonoBehaviour
    {
        private const int TorsoIndex = 0;
        private const int HeadIndex = 1;
        private const int LeftArmIndex = 2;
        private const int RightArmIndex = 3;

        [SerializeField] private NpcEmotionState emotionState;
        [SerializeField] private float transitionSpeed = 6f;

        private static readonly Color HappyColor = new Color(0.91f, 0.68f, 0.20f, 1f);
        private static readonly Color SadColor = new Color(0.25f, 0.40f, 0.56f, 1f);
        private static readonly Color AngryColor = new Color(0.68f, 0.16f, 0.14f, 1f);
        private static readonly Color CalmColor = new Color(0.28f, 0.52f, 0.42f, 1f);

        private Transform[] poseTargets;
        private Vector3[] originalPositions;
        private Quaternion[] originalRotations;
        private Renderer torsoRenderer;
        private Material torsoMaterial;
        private Color originalTorsoColor;
        private Transform visualRoot;
        private Vector3 originalVisualPosition;
        private Vector3 originalVisualScale;

        private EmotionType currentEmotion = EmotionType.Neutral;
        private float currentIntensity;

        public EmotionType CurrentEmotion => currentEmotion;
        public float CurrentIntensity => currentIntensity;
        public Color CurrentTorsoColor =>
            torsoMaterial != null ? torsoMaterial.color : Color.white;

        private void Awake()
        {
            if (emotionState == null)
            {
                emotionState = GetComponent<NpcEmotionState>();
            }

            ResolvePoseTargets();
            ApplyEmotion(emotionState.CurrentEmotion, emotionState.CurrentIntensity);
            emotionState.EmotionChanged += HandleEmotionChanged;
            AnimatePose(1f);
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
            float transition = 1f - Mathf.Exp(-transitionSpeed * Time.deltaTime);
            AnimatePose(transition);
        }

        private void HandleEmotionChanged(EmotionResult result)
        {
            ApplyEmotion(result.Emotion, result.Intensity);
            AnimatePose(1f);
        }

        private void ApplyEmotion(EmotionType emotion, float intensity)
        {
            currentEmotion = emotion;
            currentIntensity = Mathf.Clamp01(intensity);
        }

        private void ResolvePoseTargets()
        {
            visualRoot = transform.Find("Visual");

            if (visualRoot == null)
            {
                Debug.LogError("NPC Visual root was not found.", this);
                enabled = false;
                return;
            }

            poseTargets = new[]
            {
                visualRoot.Find("Torso"),
                visualRoot.Find("Head"),
                visualRoot.Find("UpperArm_L"),
                visualRoot.Find("UpperArm_R")
            };

            foreach (Transform poseTarget in poseTargets)
            {
                if (poseTarget == null)
                {
                    Debug.LogError("NPC emotion pose target was not found.", this);
                    enabled = false;
                    return;
                }
            }

            originalPositions = new Vector3[poseTargets.Length];
            originalRotations = new Quaternion[poseTargets.Length];

            for (int index = 0; index < poseTargets.Length; index++)
            {
                originalPositions[index] = poseTargets[index].localPosition;
                originalRotations[index] = poseTargets[index].localRotation;
            }

            torsoRenderer = poseTargets[TorsoIndex].GetComponent<Renderer>();
            torsoMaterial = torsoRenderer != null ? torsoRenderer.material : null;
            originalTorsoColor = torsoMaterial != null ? torsoMaterial.color : Color.white;
            originalVisualPosition = visualRoot.localPosition;
            originalVisualScale = visualRoot.localScale;
        }

        private void AnimatePose(float transition)
        {
            if (poseTargets == null || poseTargets.Length == 0)
            {
                return;
            }

            float intensity = currentIntensity;
            float time = Time.time;
            float breathing = Mathf.Sin(time * 2f) * 0.012f;

            Vector3 torsoPositionOffset = Vector3.up * breathing;
            Vector3 torsoRotationOffset = Vector3.zero;
            Vector3 headRotationOffset = Vector3.zero;
            Vector3 leftArmRotationOffset = Vector3.zero;
            Vector3 rightArmRotationOffset = Vector3.zero;
            Color targetTorsoColor = originalTorsoColor;
            Vector3 visualPositionOffset = Vector3.zero;
            float targetVisualScale = 1f;

            switch (currentEmotion)
            {
                case EmotionType.Happy:
                    torsoPositionOffset += Vector3.up * (0.12f * intensity);
                    torsoRotationOffset.z = Mathf.Sin(time * 5f) * 6f * intensity;
                    headRotationOffset.x = -22f * intensity;
                    leftArmRotationOffset.z = -55f * intensity;
                    rightArmRotationOffset.z = 55f * intensity;
                    visualPositionOffset.y = Mathf.Abs(Mathf.Sin(time * 4f)) * 0.05f * intensity;
                    targetVisualScale = 1f + 0.08f * intensity;
                    targetTorsoColor = Color.Lerp(originalTorsoColor, HappyColor, intensity);
                    break;
                case EmotionType.Sad:
                    torsoPositionOffset += Vector3.down * (0.13f * intensity);
                    torsoRotationOffset.x = 16f * intensity;
                    headRotationOffset.x = 38f * intensity;
                    leftArmRotationOffset.z = -18f * intensity;
                    rightArmRotationOffset.z = 18f * intensity;
                    targetVisualScale = 1f - 0.07f * intensity;
                    targetTorsoColor = Color.Lerp(originalTorsoColor, SadColor, intensity);
                    break;
                case EmotionType.Angry:
                    torsoPositionOffset += new Vector3(
                        Mathf.Sin(time * 20f) * 0.035f * intensity,
                        0f,
                        0f);
                    torsoRotationOffset.x = 10f * intensity;
                    headRotationOffset.x = -10f * intensity;
                    leftArmRotationOffset.z = -70f * intensity;
                    rightArmRotationOffset.z = 70f * intensity;
                    visualPositionOffset.x = Mathf.Sin(time * 22f) * 0.025f * intensity;
                    targetVisualScale = 1f + 0.11f * intensity;
                    targetTorsoColor = Color.Lerp(originalTorsoColor, AngryColor, intensity);
                    break;
                case EmotionType.Calm:
                    torsoPositionOffset += Vector3.up * (breathing * 0.8f);
                    torsoRotationOffset.z = Mathf.Sin(time * 1.4f) * 1.5f * intensity;
                    headRotationOffset.x = 5f * intensity;
                    targetVisualScale = 1f + 0.02f * intensity;
                    targetTorsoColor = Color.Lerp(originalTorsoColor, CalmColor, intensity);
                    break;
                default:
                    targetTorsoColor = originalTorsoColor;
                    break;
            }

            ApplyLocalTransform(TorsoIndex, torsoPositionOffset, torsoRotationOffset, transition);
            ApplyLocalTransform(HeadIndex, Vector3.zero, headRotationOffset, transition);
            ApplyLocalTransform(LeftArmIndex, Vector3.zero, leftArmRotationOffset, transition);
            ApplyLocalTransform(RightArmIndex, Vector3.zero, rightArmRotationOffset, transition);

            if (visualRoot != null)
            {
                visualRoot.localPosition = Vector3.Lerp(
                    visualRoot.localPosition,
                    originalVisualPosition + visualPositionOffset,
                    transition);
                visualRoot.localScale = Vector3.Lerp(
                    visualRoot.localScale,
                    originalVisualScale * targetVisualScale,
                    transition);
            }

            if (torsoMaterial != null)
            {
                torsoMaterial.color = Color.Lerp(
                    torsoMaterial.color,
                    targetTorsoColor,
                    transition);
            }
        }

        private void ApplyLocalTransform(
            int index,
            Vector3 positionOffset,
            Vector3 rotationOffset,
            float transition)
        {
            Transform target = poseTargets[index];
            target.localPosition = Vector3.Lerp(
                target.localPosition,
                originalPositions[index] + positionOffset,
                transition);
            target.localRotation = Quaternion.Slerp(
                target.localRotation,
                originalRotations[index] * Quaternion.Euler(rotationOffset),
                transition);
        }
    }
}
