using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class AudioDataManager : Singleton<AudioDataManager>
{
    public float[][] rawData;

    private float[][] rawDataClone;

    public FFTData[] fftData;

    public byte[] rawDataAsBytes;

    public float[][][] spectrogramData;

    public byte[] spectrogramDataAsBytes;

    public static FFT fft = new();

    public static MFCC mfcc = new();

    public void Initialize()
    {
        rawData = new float[2][]
        {
            new float[1024],
            new float[1024],
        };
        rawDataClone = new float[2][]
        {
            new float[1024],
            new float[1024],
        };
        fftData = new FFTData[2]
        {
            new(),
            new(),
        };
        spectrogramData = new float[2][][];
    }

    public void ProcessAudioData()
    {
        TransformAudio();
    }

    private void TransformAudio()
    {
        AudioListener.GetOutputData(rawData[0], 0);
        AudioListener.GetOutputData(rawData[1], 1);
        Array.Copy(rawData[0], rawDataClone[0], 1024);
        Array.Copy(rawData[1], rawDataClone[1], 1024);

        rawDataAsBytes = NumberConverter.Convert2DFloatArrayToByteArray(rawData);

        fft.Process(rawDataClone[0]);
        fftData[0].RealData = fft.getReal();
        fftData[0].ImagData = fft.getImag();
        fftData[0].RealDataAsBytes = NumberConverter.Convert1DFloatArrayToByteArray(fftData[0].RealData);
        fftData[0].ImagDataAsBytes = NumberConverter.Convert1DFloatArrayToByteArray(fftData[0].ImagData);

        fft.Process(rawDataClone[1]);
        fftData[1].RealData = fft.getReal();
        fftData[1].ImagData = fft.getImag();
        fftData[1].RealDataAsBytes = NumberConverter.Convert1DFloatArrayToByteArray(fftData[1].RealData);
        fftData[1].ImagDataAsBytes = NumberConverter.Convert1DFloatArrayToByteArray(fftData[1].ImagData);

        spectrogramData[0] = mfcc.melSpectrogram(rawData[0]);
        spectrogramData[1] = mfcc.melSpectrogram(rawData[1]);
        spectrogramDataAsBytes = NumberConverter.Convert3DFloatArrayToByteArray(spectrogramData);
    }

    public AudioData GetAudioData()
    {
        AudioData data = new AudioData
        {
            RawData = rawData,
            FftData = fftData,
            RawDataAsBytes = rawDataAsBytes,
            SpectrogramData = spectrogramData,
            SpectrogramDataAsBytes = spectrogramDataAsBytes
        };
        return data;
    }
}