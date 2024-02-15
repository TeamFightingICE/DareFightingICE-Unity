using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MFCC
{
    private static  int n_mfcc = 20;

    private static  float fMin = 0.0F;

    private static  int n_fft = 1024;

    private static  int hop_Length = 480;

    private static  int n_mels = 80;

    private static  float sampleRate = 48000.0F;

    private static  float fMax = 24000.0F;

    FFT fft = new FFT();

    public float[] process(float[] floatInputBuffer)
    {
        float[][] mfccResult = dctMfcc(floatInputBuffer);
        return finalshape(mfccResult);
    }

    private float[] finalshape(float[][] mfccSpecTro)
    {
        float[] finalMfcc = new float[(mfccSpecTro[0]).Length * mfccSpecTro.Length];
        int k = 0;
        for (int i = 0; i < (mfccSpecTro[0]).Length; i++)
        {
            for (int j = 0; j < mfccSpecTro.Length; j++)
            {
                finalMfcc[k] = mfccSpecTro[j][i];
                k++;
            }
        }
        return finalMfcc;
    }

    private float[][] dctMfcc(float[] y)
    {
        float[][] specTroGram = powerToDb(melSpectrogram(y));
        float[][] dctBasis = dctFilter(20, 80);
        float[][] mfccSpecTro = new float[20][];
        for (int i = 0; i < 20; i++)
        {
            mfccSpecTro[i] = new float[specTroGram[0].Length];
        }
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < (specTroGram[0]).Length; j++)
            {
                for (int k = 0; k < specTroGram.Length; k++)
                    mfccSpecTro[i][j] = mfccSpecTro[i][j] + dctBasis[i][k] * specTroGram[k][j];
            }
        }
        return mfccSpecTro;
    }

    public float[][] melSpectrogram(float[] y)
    {
        float[][] melBasis = melFilter();
        float[][] spectro = stftMagSpec(y);
        float[][] melS = new float[melBasis.Length][];
        for (int i = 0; i < melBasis.Length; i++)
        {
            melS[i] =new float[(spectro[0]).Length];
        }
        for (int i = 0; i < melBasis.Length; i++)
        {
            for (int j = 0; j < (spectro[0]).Length; j++)
            {
                for (int k = 0; k < (melBasis[0]).Length; k++)
                    melS[i][j] = melS[i][j] + melBasis[i][k] * spectro[k][j];
            }
        }
        return melS;
    }

    private float[][] stftMagSpec(float[] y)
    {
        float[] fftwin = getWindow();
        float[] ypad = new float[1024 + y.Length];
        for (int i = 0; i < 512; i++)
        {
            ypad[512 - i - 1] = y[i + 1];
            ypad[512 + y.Length + i] = y[y.Length - 2 - i];
        }
        for (int j = 0; j < y.Length; j++)
            ypad[512 + j] = y[j];
        float[][] frame = yFrame(ypad);
        float[][] fftmagSpec = new float[513][];
        for(int i = 0;i < fftmagSpec.Length;i++)
        {
            fftmagSpec[i] = new float[(frame[0]).Length];
        }
        
        float[] fftFrame = new float[1024];
        for (int k = 0; k < (frame[0]).Length; k++)
        {
            for (int l = 0; l < 1024; l++)
                fftFrame[l] = fftwin[l] * frame[l][k];
            float[] magSpec = magSpectrogram(fftFrame);
            for (int m = 0; m < 513; m++)
                fftmagSpec[m][k] = magSpec[m];
        }
        return fftmagSpec;
    }

    private float[] magSpectrogram(float[] frame)
    {
        float[] magSpec = new float[frame.Length];
        this.fft.process(frame);
        for (int m = 0; m < frame.Length; m++)
            magSpec[m] = this.fft.real[m] * this.fft.real[m] + this.fft.imag[m] * this.fft.imag[m];
        return magSpec;
    }

    private float[] getWindow()
    {
        float[] win = new float[1024];
        for (int i = 0; i < 1024; i++)
            win[i] = (float)(0.5D - 0.5D * Math.Cos(6.283185307179586D * i / 1024.0D));
        return win;
    }

    private float[][] yFrame(float[] ypad)
    {
        int n_frames = 1 + (ypad.Length - 1024) / 480;
        float[][] winFrames = new float[1024][];
        for(int i = 0; i < 1024; i++)
        {
            winFrames[i] = new float[n_frames];
        }
        
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < n_frames; j++)
                winFrames[i][j] = ypad[j * 480 + i];
        }
        return winFrames;
    }

    private float[][] powerToDb(float[][] melS)
    {
        float[][] log_spec = new float[melS.Length][];
        for(int a = 0; a < melS.Length; a++)
        {
            log_spec[a]= new float[(melS[0]).Length];
        }
        float maxValue = -100.0F;
        int i;
        for (i = 0; i < melS.Length; i++)
        {
            for (int j = 0; j < (melS[0]).Length; j++)
            {
                float magnitude = Math.Abs(melS[i][j]);
                if (magnitude > 1.0E-10D)
                {
                    log_spec[i][j] = (float)(10.0D * log10(magnitude));
                }
                else
                {
                    log_spec[i][j] = -100.0F;
                }
                if (log_spec[i][j] > maxValue)
                    maxValue = log_spec[i][j];
            }
        }
        for (i = 0; i < melS.Length; i++)
        {
            for (int j = 0; j < (melS[0]).Length; j++)
            {
                if (log_spec[i][j] < maxValue - 80.0D)
                    log_spec[i][j] = (float)(maxValue - 80.0D);
            }
        }
        return log_spec;
    }

    private float[][] dctFilter(int n_filters, int n_input)
    {
        float[][] basis = new float[n_filters][];
        for(int i = 0;i < n_filters; i++)
        {
            basis[i] = new float[n_input];
        }
        float[] samples = new float[n_input];
        for (int k = 0; k < n_input; k++)
            samples[k] = (float)((1 + 2 * k) * Math.PI / 2.0D * n_input);
        for (int j = 0; j < n_input; j++)
            basis[0][j] = (float)(1.0D / Math.Sqrt(n_input));
        for (int i = 1; i < n_filters; i++)
        {
            for (int m = 0; m < n_input; m++)
                basis[i][m] = (float)(Math.Cos((i * samples[m])) * Math.Sqrt(2.0D / n_input));
        }
        return basis;
    }

    private float[][] melFilter()
    {
        float[] fftFreqs = fftFreq();
        float[] melF = melFreq(82);
        float[] fdiff = new float[melF.Length - 1];
        for (int i = 0; i < melF.Length - 1; i++)
            fdiff[i] = melF[i + 1] - melF[i];
        float[][] ramps = new float[melF.Length][];
        for(int i = 0;i < melF.Length; i++)
        {
            ramps[i] = new float[fftFreqs.Length];
        }
        
        for (int j = 0; j < melF.Length; j++)
        {
            for (int n = 0; n < fftFreqs.Length; n++)
                ramps[j][n] = melF[j] - fftFreqs[n];
        }
        float[][] weights = new float[80][];
        for(int i = 0; i < 80; i++)
        {
            weights[i] = new float[513];
        }
        
        for (int k = 0; k < 80; k++)
        {
            for (int n = 0; n < fftFreqs.Length; n++)
            {
                float lowerF = -ramps[k][n] / fdiff[k];
                float upperF = ramps[k + 2][n] / fdiff[k + 1];
                if (lowerF > upperF && upperF > 0.0F)
                {
                    weights[k][n] = upperF;
                }
                else if (lowerF > upperF && upperF < 0.0F)
                {
                    weights[k][n] = 0.0F;
                }
                else if (lowerF < upperF && lowerF > 0.0F)
                {
                    weights[k][n] = lowerF;
                }
                else if (lowerF < upperF && lowerF < 0.0F)
                {
                    weights[k][n] = 0.0F;
                }
            }
        }
        float[] enorm = new float[80];
        for (int m = 0; m < 80; m++)
        {
            enorm[m] = (float)(2.0D / (melF[m + 2] - melF[m]));
            for (int n = 0; n < fftFreqs.Length; n++)
                weights[m][n] = weights[m][n] * enorm[m];
        }
        return weights;
    }

    private float[] fftFreq()
    {
        float[] freqs = new float[513];
        for (int i = 0; i < 513; i++)
            freqs[i] = 0.0F + 46.875F * i;
        return freqs;
    }

    private float[] melFreq(int numMels)
    {
        float[] LowFFreq = new float[1];
        float[] HighFFreq = new float[1];
        LowFFreq[0] = 0.0F;
        HighFFreq[0] = 24000.0F;
        float[] melFLow = freqToMel(LowFFreq);
        float[] melFHigh = freqToMel(HighFFreq);
        float[] mels = new float[numMels];
        for (int i = 0; i < numMels; i++)
            mels[i] = melFLow[0] + (melFHigh[0] - melFLow[0]) / (numMels - 1) * i;
        return melToFreq(mels);
    }

    private float[] melToFreqS(float[] mels)
    {
        float[] freqs = new float[mels.Length];
        for (int i = 0; i < mels.Length; i++)
            freqs[i] = (float)(700.0D * (Math.Pow(10.0D, mels[i] / 2595.0D) - 1.0D));
        return freqs;
    }

    protected float[] freqToMelS(float[] freqs)
    {
        float[] mels = new float[freqs.Length];
        for (int i = 0; i < freqs.Length; i++)
            mels[i] = (float)(2595.0D * log10((float)(1.0D + freqs[i] / 700.0D)));
        return mels;
    }

    private float[] melToFreq(float[] mels)
    {
        float f_min = 0.0F;
        float f_sp = 66.666664F;
        float[] freqs = new float[mels.Length];
        float min_log_hz = 1000.0F;
        float min_log_mel = 15.000001F;
        float logstep = (float)(Math.Log(6.4D) / 27.0D);
        for (int i = 0; i < mels.Length; i++)
        {
            if (mels[i] < 15.000001F)
            {
                freqs[i] = 0.0F + 66.666664F * mels[i];
            }
            else
            {
                freqs[i] = (float)(1000.0D * Math.Exp((logstep * (mels[i] - 15.000001F))));
            }
        }
        return freqs;
    }

    protected float[] freqToMel(float[] freqs)
    {
        float f_min = 0.0F;
        float f_sp = 66.666664F;
        float[] mels = new float[freqs.Length];
        float min_log_hz = 1000.0F;
        float min_log_mel = 15.000001F;
        float logstep = (float)(Math.Log(6.4D) / 27.0D);
        for (int i = 0; i < freqs.Length; i++)
        {
            if (freqs[i] < 1000.0F)
            {
                mels[i] = (freqs[i] - 0.0F) / 66.666664F;
            }
            else
            {
                mels[i] = (float)(15.000000953674316D + Math.Log((freqs[i] / 1000.0F)) / logstep);
            }
        }
        return mels;
    }

    private float log10(float value)
    {
        return (float)(Math.Log(value) / Math.Log(10.0D));
    }
}
