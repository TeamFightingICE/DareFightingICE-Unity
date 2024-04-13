public class ServiceUtils
{
    public static IServer GetServerInstance() {
        if (FlagSetting.Instance.useSocket)
        {
            return SocketServer.Instance;
        }
        else if (FlagSetting.Instance.useGrpc)
        {
            return GrpcServer.Instance;
        }
        return null;
    }

    public static bool IsServerOpen() {
        IServer serverInstance = GetServerInstance();
        if (serverInstance != null)
        {
            return serverInstance.IsOpen;
        }
        return false;
    }

    public static IAIInterface GetPlayerInstance(bool isPlayerOne)
    {
        IServer serverInstance = GetServerInstance();
        if (serverInstance != null)
        {
            return serverInstance.GetPlayer(isPlayerOne);
        }
        return new Sandbox();
    }
}
