using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FFT : MonoBehaviour
{
    public float[] real;

    public float[] imag;

    public void process(float[] signal)
    {
        int numPoints = signal.Length;
        this.real = signal;
        this.imag = new float[numPoints];
        double pi = Math.PI;
        int numStages = (int)(Math.Log(numPoints) / Math.Log(2.0D));
        int halfNumPoints = numPoints >> 1;
        int j = halfNumPoints;
        for (int i = 1; i < numPoints - 2; i++)
        {
            if (i < j)
            {
                float tempReal = this.real[j];
                float tempImag = this.imag[j];
                this.real[j] = this.real[i];
                this.imag[j] = this.imag[i];
                this.real[i] = tempReal;
                this.imag[i] = tempImag;
            }
            int k = halfNumPoints;
            while (k <= j)
            {
                j -= k;
                k >>= 1;
            }
            j += k;
        }
        for (int stage = 1; stage <= numStages; stage++)
        {
            int LE = 1;
            for (int k = 0; k < stage; k++)
                LE <<= 1;
            int LE2 = LE >> 1;
            float UR = 1.0F;
            float UI = 0.0F;
            float SR = (float)Math.Cos(Math.PI / LE2);
            float SI = (float)-Math.Sin(Math.PI / LE2);
            for (int subDFT = 1; subDFT <= LE2; subDFT++)
            {
                for (int butterfly = subDFT - 1; butterfly <= numPoints - 1; butterfly += LE)
                {
                    int ip = butterfly + LE2;
                    float tempReal = this.real[ip] * UR - this.imag[ip] * UI;
                    float tempImag = this.real[ip] * UI + this.imag[ip] * UR;
                    this.real[ip] = this.real[butterfly] - tempReal;
                    this.imag[ip] = this.imag[butterfly] - tempImag;
                    this.real[butterfly] = this.real[butterfly] + tempReal;
                    this.imag[butterfly] = this.imag[butterfly] + tempImag;
                }
                float tempUR = UR;
                UR = tempUR * SR - UI * SI;
                UI = tempUR * SI + UI * SR;
            }
        }
    }

    public float[] getReal()
    {
        return this.real;
    }

    public float[] getImag()
    {
        return this.imag;
    }
}
