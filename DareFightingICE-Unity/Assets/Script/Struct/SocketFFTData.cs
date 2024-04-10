using System.Collections;
using System.Collections.Generic;
using DareFightingICE.Grpc.Proto;
using Google.Protobuf;
using UnityEngine;

public class SocketFFTData
{
    public string RealDataBytestring { get; set; }
    public string ImaginaryDataBytestring { get; set; }
}
