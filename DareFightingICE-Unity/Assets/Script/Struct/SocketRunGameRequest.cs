using DareFightingICE.Grpc.Proto;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketRunGameRequest
{
    public string Character_1 { get; set; }
    public string Character_2 { get; set; }
    public string Player_1 { get; set; }
    public string Player_2 { get; set; }
    public int GameNumber { get; set; }
}
