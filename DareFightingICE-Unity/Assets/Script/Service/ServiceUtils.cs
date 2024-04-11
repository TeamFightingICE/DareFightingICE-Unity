using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UnityEngine;

public class ServiceUtils
{
    public static bool IsServerOpen() {
        if (FlagSetting.Instance.grpc)
        {
            return GrpcServer.Instance.IsOpen;
        }
        else if (FlagSetting.Instance.socket)
        {
            return SocketServer.Instance.IsOpen;
        }
        else
        {
            return false;
        }
    }

    public static IServer GetServerInstance() {
        if (FlagSetting.Instance.grpc)
        {
            return GrpcServer.Instance;
        }
        else if (FlagSetting.Instance.socket)
        {
            return SocketServer.Instance;
        }
        else
        {
            return null;
        }
    }
}
