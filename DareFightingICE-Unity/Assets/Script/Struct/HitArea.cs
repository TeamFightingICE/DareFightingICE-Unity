using DareFightingICE.Grpc.Proto;
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
    
    public HitArea(HitArea other)
    {
        this.Left = other.Left;
        this.Right = other.Right;
        this.Top = other.Top;
        this.Bottom = other.Bottom;
    }

    public GrpcHitArea ToProto()
    {
        return new GrpcHitArea
        {
            Left = this.Left,
            Right = this.Right,
            Top = this.Top,
            Bottom = this.Bottom,
        };
    }
}
