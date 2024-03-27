using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SimInputManager : Singleton<SimInputManager>
{
    private readonly LinkedList<Key> p1Keys = new();
    private readonly LinkedList<Key> p2Keys = new();
    public SimInputManager()
    {
        ClearInput();
    }
    private LinkedList<Key> GetKeys(bool player)
    {
        return player ? p1Keys : p2Keys;
    }
    public void SetInput(bool player, Key key)
    {
        var keys = this.GetKeys(player);
        keys.AddLast(key);

        while (keys.Count > 2)
        {
            keys.RemoveFirst();
        }
    }
    public Key GetInput(bool player)
    {
        return this.GetKeys(player).Last.Value;
    }
    public void ClearInput()
    {
        p1Keys.Clear();
        p2Keys.Clear();

        for (int i = 0; i < 2; i++)
        {
            p1Keys.AddLast(new Key());
            p2Keys.AddLast(new Key());
        }
    }
    public bool IsKeyPressed(bool player, string key)
    {
        var keys = this.GetKeys(player);
        var current = keys.Last.Value;
        var previous = keys.First.Value;
        return key switch
        {
            "R" => current.R && !previous.R,
            "L" => current.L && !previous.L,
            "U" => current.U && !previous.U,
            "D" => current.D && !previous.D,
            "A" => current.A && !previous.A,
            "B" => current.B && !previous.B,
            "C" => current.C && !previous.C,
            _ => false,
        };
    }
    public bool IsKeyHeld(bool player, string key)
    {
        var keys = this.GetKeys(player);
        var current = keys.Last.Value;
        var previous = keys.First.Value;
        return key switch
        {
            "R" => current.R && previous.R,
            "L" => current.L && previous.L,
            "U" => current.U && previous.U,
            "D" => current.D && previous.D,
            "A" => current.A && !previous.A,
            "B" => current.B && !previous.B,
            "C" => current.C && !previous.C,
            _ => false,
        };
    }
    public bool IsKeyRelease(bool player, string key)
    {
        var keys = this.GetKeys(player);
        var current = keys.Last.Value;
        var previous = keys.First.Value;
        return key switch
        {
            "R" => !current.R && previous.R,
            "L" => !current.L && previous.L,
            "U" => !current.U && previous.U,
            "D" => !current.D && previous.D,
            "A" => current.A && !previous.A,
            "B" => current.B && !previous.B,
            "C" => current.C && !previous.C,
            _ => false,
        };
    }
}
