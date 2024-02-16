public enum ControlType
{
    KEYBOARD,
    AI,
    GRPC
}

public class ControlTypeUtil
{
    public static string GetString(ControlType controlType)
    {
        switch (controlType)
        {
            case ControlType.KEYBOARD:
                return "Keyboard";
            case ControlType.AI:
                return "AI";
            case ControlType.GRPC:
                return "gRPC";
        }
        return "Unknown";
    }
}