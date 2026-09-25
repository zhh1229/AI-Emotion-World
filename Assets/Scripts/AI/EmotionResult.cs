namespace AIEmotionWorld.AI
{
    public enum EmotionType
    {
        Happy,
        Sad,
        Angry,
        Calm,
        Neutral
    }

    public readonly struct EmotionResult
    {
        public EmotionType Emotion { get; }
        public float Intensity { get; }
        public string Reply { get; }

        public EmotionResult(EmotionType emotion, float intensity, string reply)
        {
            Emotion = emotion;
            Intensity = intensity;
            Reply = reply;
        }
    }
}
