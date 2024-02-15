using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData
{
    public float[][] rawData { get; set; }
    
    public byte[] rawDataAsBytes { get; set; }

    public float[][] fftData { get; set; }
   
    public float[][][] spectrogramData { get; set; }
   
    public byte[] spectrogramDataAsBytes { get; set; }
    
    public static FFT fft { get; set; }
    
    public static MFCC mfcc { get; set; }

}
