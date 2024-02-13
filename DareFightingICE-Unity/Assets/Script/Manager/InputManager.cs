﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InputManager : Singleton<InputManager>
{
    private Key p1Key;
    private Key p2Key;
    public InputManager()
    {         
        this.p1Key = new Key();
        this.p2Key = new Key();
    }
    public void SetInput(bool playerNumber, Key key)
    {
        if (playerNumber)
        {
            this.p1Key = key;
        }
        else
        {
            this.p2Key = key; 
        }
    }
    public Key GetInput(bool playerNumber)
    {
        return (playerNumber) ? p1Key : p2Key;
    }
}
