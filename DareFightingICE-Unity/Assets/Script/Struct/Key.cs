using UnityEngine;

public class Key
{
    public bool A { get; set; }
    public bool B { get; set; }
    public bool C { get; set; }
    public bool U { get; set; }
    public bool R { get; set; }
    public bool D { get; set; }
    public bool L { get; set; }

    public Key()
    {
        Empty();
    }

    public Key(Key key)
    {
        if (key != null)
        {
            A = key.A;
            B = key.B;
            C = key.C;
            U = key.U;
            R = key.R;
            D = key.D;
            L = key.L;
        }
        else
        {
            Empty();
        }
    }

    public void Empty()
    {
        A = B = C = U = R = D = L = false;
    }

    public bool IsEmpty()
    {
        return !(A || B || C || U || R || D || L);
    }

    public int GetLever(bool isFront)
    {
        int lever = 5;

        if (U) lever += 3;
        if (D) lever -= 3;
        if (L) lever += isFront ? -1 : 1;
        if (R) lever += isFront ? 1 : -1;

        return lever;
    }
    
    public bool previousR = false;
    public bool previousL = false;
    public bool previousU = false;
    public bool previousD = false;
    public bool previousA = false;
    public bool previousB = false;
    public bool previousC = false;
    // Add other keys as needed

    // Call this method at the end of each frame to update the previous state
    public void UpdatePreviousState()
    {
        previousR = R;
        previousL = L;
        previousU = U;
        previousD = D;
        previousA = A;
        previousB = B;
        previousC = C;
        // Update other keys as needed
    }

    // Method to check if a key was just pressed (not held)
    public bool IsKeyPressed(string key)
    {
        switch (key)
        {
            case "R": return R && !previousR;
            case "L": return L && !previousL;
            case "U": return U && !previousU;
            case "D": return D && !previousD;
            case "A": return A && !previousA;
            case "B": return B && !previousB;
            case "C": return C && !previousC;
            // Add other keys as needed
            default: return false;
        }
    }

    // Method to check if a key is being held
    public bool IsKeyHeld(string key)
    {
        switch (key)
        {
            case "R": return R && previousR;
            case "L": return L && previousL;
            case "U": return U && previousU;
            case "D": return D && previousD;
            case "A": return A && !previousA;
            case "B": return B && !previousB;
            case "C": return C && !previousC;
            // Add other keys as needed
            default: return false;
        }
    }

    public bool IsKeyRelease(string key)
    {
        switch (key)
        {
            case "R": return !R && previousR;
            case "L": return !L && previousL;
            case "U": return !U && previousU;
            case "D": return !D && previousD;
            case "A": return A && !previousA;
            case "B": return B && !previousB;
            case "C": return C && !previousC;
            // Add other keys as needed
            default: return false;
        }
    }
}