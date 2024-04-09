using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
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

        rawDataAsBytes = Convert2DFloatArrayToByteArray(rawData);

        fft.process(rawDataClone[0]);
        fftData[0].RealData = fft.getReal();
        fftData[0].ImagData = fft.getImag();
        fftData[0].RealDataAsBytes = Convert1DFloatArrayToByteArray(fftData[0].RealData);
        fftData[0].ImagDataAsBytes = Convert1DFloatArrayToByteArray(fftData[0].ImagData);

        fft.process(rawDataClone[1]);
        fftData[1].RealData = fft.getReal();
        fftData[1].ImagData = fft.getImag();
        fftData[1].RealDataAsBytes = Convert1DFloatArrayToByteArray(fftData[1].RealData);
        fftData[1].ImagDataAsBytes = Convert1DFloatArrayToByteArray(fftData[1].ImagData);

        spectrogramData[0] = mfcc.melSpectrogram(rawData[0]);
        spectrogramData[1] = mfcc.melSpectrogram(rawData[1]);
        spectrogramDataAsBytes = Convert3DFloatArrayToByteArray(spectrogramData);
    }

    byte[] Convert1DFloatArrayToByteArray(float[] samples)
    {
        int sampleCount = samples.Length;

        // Create a byte array to hold the converted data
        byte[] byteArray = new byte[sampleCount * sizeof(float)];

        // Copy the float array to the byte array
        for (int i = 0; i < sampleCount; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(samples[i]), 0, byteArray, i * sizeof(float), sizeof(float));
        }

        return byteArray;
    }

    byte[] Convert2DFloatArrayToByteArray(float[][] samples)
    {
        int channels = samples.Length;
        int sampleCount = samples[0].Length;

        // Create a byte array to hold the converted data
        byte[] byteArray = new byte[channels * sampleCount * sizeof(float)];
        // Copy the float array to the byte array, interleaving channels
        for (int i = 0; i < sampleCount; i++)
        {
            for (int channel = 0; channel < channels; channel++)
            {
                int index = sampleCount * channel + i;
                byte[] temp = BitConverter.GetBytes(samples[channel][i]);
                Buffer.BlockCopy(temp, 0, byteArray, index * sizeof(float),
                    sizeof(float));
            }
        }
        
        return byteArray;
    }

    byte[] Convert3DFloatArrayToByteArray(float[][][] samples)
    {
        int channels = samples.Length;
        int frameCount = samples[0].Length;
        int sampleCount = samples[0][0].Length;

        // Create a byte array to hold the converted data
        byte[] byteArray = new byte[channels * frameCount * sampleCount * sizeof(float)];

        // Copy the float array to the byte array, interleaving channels and frames
        for (int i = 0; i < channels; i++)
        {
            for (int j = 0; j < frameCount; j++)
            {
                for (int k = 0; k < sampleCount; k++)
                {

                    int index = (i * channels * frameCount) + (j * sampleCount) + k;
                    byte [] temp = BitConverter.GetBytes(samples[i][j][k]);
                    Buffer.BlockCopy(temp, 0, byteArray,
                        index * sizeof(float), sizeof(float));

                }
            }
        }

        return byteArray;
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