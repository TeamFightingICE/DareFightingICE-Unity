using System;
using System.Collections;
using System.Collections.Generic;
using DareFightingICE.Grpc.Proto;
using Google.Protobuf;
using UnityEngine;

public class FFTData
{
    public float[] RealData { get; set; }
    public float[] ImagData { get; set; }
    public byte[] RealDataAsBytes { get; set; }
    public byte[] ImagDataAsBytes { get; set; }

    public FFTData() {
        this.RealData = new float[0];
        this.ImagData = new float[0];
        this.RealDataAsBytes = new byte[0];
        this.ImagDataAsBytes = new byte[0];
    }

    public GrpcFftData ToProto()
    {
        return new GrpcFftData {
            RealDataAsBytes = ByteString.CopyFrom(RealDataAsBytes),
            ImaginaryDataAsBytes = ByteString.CopyFrom(ImagDataAsBytes)
        };
    }
}
