using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea
{
    public int Left { get; set; }

    public int Right{ get; set; }

    public int Top { get; set; }
    
    public int Bottom { get; set; }

    public HitArea() {
        this.Left = 0;
        this.Right = 0;
        this.Top = 0;
        this.Bottom = 0;
    }
}
