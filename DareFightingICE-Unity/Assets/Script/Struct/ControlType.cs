public enum ControlType
{
    KEYBOARD,
    LOCAL_AI,
    EXTERNAL_AI
}

public class ControlTypeUtil
{
    public static string GetString(ControlType controlType)
    {
        return controlType switch
        {
            ControlType.KEYBOARD => "Keyboard",
            ControlType.LOCAL_AI => "AI",
            ControlType.EXTERNAL_AI => FlagSetting.Instance.useGrpc ? "gRPC" : FlagSetting.Instance.useSocket ? "Socket" : "Unknown",
            _ => "Unknown",
        };
    }
}