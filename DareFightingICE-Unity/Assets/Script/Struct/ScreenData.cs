using DareFightingICE.Grpc.Proto;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenData
{
    public byte[] DisplayBytes { get; set; }

    public ScreenData()
    {
        this.DisplayBytes = new byte[96 * 64];
    }

    public ScreenData(ScreenData other)
    {
        this.DisplayBytes = other.DisplayBytes;
    }

    public GrpcScreenData ToProto()
    {
        return new GrpcScreenData
        {
            DisplayBytes = ByteString.CopyFrom(GrpcUtil.CompressBytes(DisplayBytes)),
        };
    }
}
