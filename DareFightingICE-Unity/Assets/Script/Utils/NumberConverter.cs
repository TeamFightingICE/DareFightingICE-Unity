using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NumberConverter
{
    public static byte[] Convert1DFloatArrayToByteArray(float[] samples)
    {
        int sampleCount = samples.Length;

        // Create a byte array to hold the converted data
        byte[] byteArray = new byte[sampleCount * sizeof(float)];

        // Copy the float array to the byte array
        for (int i = 0; i < sampleCount; i++)
        {
            byte[] temp = BitConverter.GetBytes(samples[i]);
            Buffer.BlockCopy(temp, 0, byteArray, i * sizeof(float), sizeof(float));
        }

        return byteArray;
    }

    public static byte[] Convert2DFloatArrayToByteArray(float[][] samples)
    {
        int channels = samples.Length;
        int sampleCount = samples[0].Length;

        // Create a byte array to hold the converted data
        byte[] byteArray = new byte[channels * sampleCount * sizeof(float)];

        // Copy the float array to the byte array, interleaving channels
        for (int i = 0; i < channels; i++)
        {
            for (int j = 0; j < sampleCount; j++)
            {
                int index = i * sampleCount + j;
                byte[] temp = BitConverter.GetBytes(samples[i][j]);
                Buffer.BlockCopy(temp, 0, byteArray, index * sizeof(float), sizeof(float));
            }
        }
        
        return byteArray;
    }

    public static byte[] Convert3DFloatArrayToByteArray(float[][][] samples)
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
                    int index = (i * frameCount * sampleCount) + (j * sampleCount) + k;
                    byte[] temp = BitConverter.GetBytes(samples[i][j][k]);
                    Buffer.BlockCopy(temp, 0, byteArray, index * sizeof(float), sizeof(float));
                }
            }
        }

        return byteArray;
    }
}