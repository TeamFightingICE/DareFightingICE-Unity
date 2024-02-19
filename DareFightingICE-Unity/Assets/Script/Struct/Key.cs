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
}