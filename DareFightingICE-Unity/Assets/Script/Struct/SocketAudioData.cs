using System.Collections;
using System.Collections.Generic;
using DareFightingICE.Grpc.Proto;
using Google.Protobuf;
using UnityEngine;

public class SocketAudioData
{
    public string RawDataBytestring { get; set; }
    public List<SocketFFTData> FftData { get; set; }
    public string SpectrogramDataBytestring { get; set; }
}
