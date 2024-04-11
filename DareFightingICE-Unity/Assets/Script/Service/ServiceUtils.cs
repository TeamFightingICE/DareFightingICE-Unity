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
    public static bool IsGrpcOrSocketOpen() {
        return FlagSetting.Instance.grpc && GrpcServer.Instance.IsOpen || FlagSetting.Instance.socket && SocketServer.Instance.IsOpen;
    }
}
