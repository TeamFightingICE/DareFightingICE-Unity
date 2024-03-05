using System;
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

    public float[][][] fftData;

    public byte[] fftDataAsByte;

    public byte[] rawDataAsBytes;

    public float[][][] spectrogramData;

    public byte[] spectrogramDataAsBytes;

    public static FFT fft = new FFT();

    public static MFCC mfcc = new MFCC();

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
        // fftData = new float[2][2][]
        // {
        //     new float[1024],
        //     new float[1024],
        // };
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
        fftData = new float[2][][]
        {
            new float[2][]
            {
                new float[1024],
                new float[1024],
            },
            new float[2][]
            {
                new float[1024],
                new float[1024],
            },
        };

        fft.process(rawDataClone[0]);
        fftData[0][0] = fft.getReal();
        fftData[0][1] = fft.getImag();
        fft.process(rawDataClone[1]);
        fftData[1][0] = fft.getReal();
        fftData[1][1] = fft.getImag();

        spectrogramData[0] = mfcc.melSpectrogram(rawData[0]);
        spectrogramData[1] = mfcc.melSpectrogram(rawData[1]);
        rawDataAsBytes = ConvertToByteArray1(rawData);
        spectrogramDataAsBytes = ConvertToByteArray2(spectrogramData);
        fftDataAsByte = ConvertToByteArray2(fftData);
    }

    byte[] ConvertToByteArray1(float[][] samples)
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
                int index = i * channels + channel;
                Buffer.BlockCopy(BitConverter.GetBytes(samples[channel][i]), 0, byteArray, index * sizeof(float),
                    sizeof(float));
            }
        }

        return byteArray;
    }

    byte[] ConvertToByteArray2(float[][][] samples)
    {
        int channels = samples.Length;
        int frameCount = samples[0].Length;
        int sampleCount = samples[0][0].Length;

        // Create a byte array to hold the converted data
        byte[] byteArray = new byte[channels * frameCount * sampleCount * sizeof(float)];

        // Copy the float array to the byte array, interleaving channels and frames
        for (int frame = 0; frame < frameCount; frame++)
        {
            for (int channel = 0; channel < channels; channel++)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    int index = (frame * channels * sampleCount) + (channel * sampleCount) + sample;
                    Buffer.BlockCopy(BitConverter.GetBytes(samples[channel][frame][sample]), 0, byteArray,
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