using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DareFightingICE.Grpc.Proto;
using Google.Protobuf;
using UnityEngine;

public class AudioData
{
    public float[][] RawData { get; set; }
    
    public byte[] RawDataAsBytes { get; set; }

    public FFTData[] FftData { get; set; }
   
    public float[][][] SpectrogramData { get; set; }
   
    public byte[] SpectrogramDataAsBytes { get; set; }

    public AudioData() {
        this.RawData = new float[0][];
        this.RawDataAsBytes = new byte[0];
        this.FftData = new FFTData[2] { new(), new()};
        this.SpectrogramData = new float[0][][];
        this.SpectrogramDataAsBytes = new byte[0];
    }

    public AudioData(AudioData other) {
        this.RawData = other.RawData;
        this.RawDataAsBytes = other.RawDataAsBytes;
        this.FftData = other.FftData;
        this.SpectrogramData = other.SpectrogramData;
        this.SpectrogramDataAsBytes = other.SpectrogramDataAsBytes;
    }

    public GrpcAudioData ToProto()
    {
        return new GrpcAudioData {
            RawDataAsBytes = ByteString.CopyFrom(RawDataAsBytes),
            FftData = { FftData[0].ToProto(), FftData[1].ToProto() },
            SpectrogramDataAsBytes = ByteString.CopyFrom(SpectrogramDataAsBytes)
        };
    }
}
