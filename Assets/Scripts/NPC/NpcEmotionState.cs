using System;
using AIEmotionWorld.AI;
using UnityEngine;

namespace AIEmotionWorld.NPC
{
    public sealed class NpcEmotionState : MonoBehaviour
    {
        [SerializeField] private EmotionType currentEmotion = EmotionType.Neutral;
        [SerializeField, Range(0f, 1f)] private float currentIntensity;

        public event Action<EmotionResult> EmotionChanged;

        public EmotionType CurrentEmotion => currentEmotion;
        public float CurrentIntensity => currentIntensity;

        public void ApplyEmotion(EmotionResult result)
        {
            currentEmotion = result.Emotion;
            currentIntensity = Mathf.Clamp01(result.Intensity);
            EmotionChanged?.Invoke(result);
        }

        public void ResetToNeutral()
        {
            EmotionResult neutralResult =
                new EmotionResult(EmotionType.Neutral, 0f, string.Empty);

            ApplyEmotion(neutralResult);
        }
    }
}
