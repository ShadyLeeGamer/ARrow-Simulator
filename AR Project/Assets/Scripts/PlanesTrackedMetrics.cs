public static class PlanesTrackedMetrics
{
    static float bedrockPosY = float.MaxValue;
    public static float BedrockPosY => bedrockPosY;

    public static void SetBedrockPosY(float y)
    {
        bedrockPosY = y;
    }
}