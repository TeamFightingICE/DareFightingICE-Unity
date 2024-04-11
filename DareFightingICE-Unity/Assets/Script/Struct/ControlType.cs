public enum ControlType
{
    KEYBOARD,
    LOCAL_AI,
    GRPC
}

public class ControlTypeUtil
{
    public static string GetString(ControlType controlType)
    {
        return controlType switch
        {
            ControlType.KEYBOARD => "Keyboard",
            ControlType.LOCAL_AI => "AI",
            ControlType.GRPC => FlagSetting.Instance.grpc ? "gRPC" : FlagSetting.Instance.socket ? "Socket" : "Unknown",
            _ => "Unknown",
        };
    }
}