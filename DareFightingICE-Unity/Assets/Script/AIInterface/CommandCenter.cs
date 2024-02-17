using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCenter
{
    private LinkedList<Key> skillKey;
    private FrameData frameData;
    private bool playerNumber;

    public CommandCenter()
    {
        this.skillKey = new LinkedList<Key>();
        this.frameData = new FrameData();
        this.playerNumber = true;
    }

    public void CommandCall(string str)
    {
		if (this.skillKey.Count == 0) {
			actionToCommand(str);
		}
	}

    private void actionToCommand(string str)
    {
		switch (str) {
            case "FORWARD_WALK":
                createKeys("6");
                break;
            case "DASH":
                createKeys("6 5 6");
                break;
            case "BACK_STEP":
                createKeys("4 5 4");
                break;
            case "CROUCH":
                createKeys("2");
                break;
            case "JUMP":
                createKeys("8");
                break;
            case "FOR_JUMP":
                createKeys("9");
                break;
            case "BACK_JUMP":
                createKeys("7");
                break;
            case "STAND_GUARD":
                createKeys("4");
                break;
            case "CROUCH_GUARD":
                createKeys("1");
                break;
            case "AIR_GUARD":
                createKeys("7");
                break;
            case "THROW_A":
                createKeys("4 _ A");
                break;
            case "THROW_B":
                createKeys("4 _ B");
                break;
            case "STAND_A":
                createKeys("A");
                break;
            case "STAND_B":
                createKeys("B");
                break;
            case "CROUCH_A":
                createKeys("2 _ A");
                break;
            case "CROUCH_B":
                createKeys("2 _ B");
                break;
            case "AIR_A":
                createKeys("A");
                break;
            case "AIR_B":
                createKeys("B");
                break;
            case "AIR_DA":
                createKeys("2 _ A");
                break;
            case "AIR_DB":
                createKeys("2 _ B");
                break;
            case "STAND_FA":
                createKeys("6 _ A");
                break;
            case "STAND_FB":
                createKeys("6 _ B");
                break;
            case "CROUCH_FA":
                createKeys("3 _ A");
                break;
            case "CROUCH_FB":
                createKeys("3 _ B");
                break;
            case "AIR_FA":
                createKeys("9 _ A");
                break;
            case "AIR_FB":
                createKeys("9 _ B");
                break;
            case "AIR_UA":
                createKeys("8 _ A");
                break;
            case "AIR_UB":
                createKeys("8 _ B");
                break;
            case "STAND_D_DF_FA":
                createKeys("2 3 6 _ A");
                break;
            case "STAND_D_DF_FB":
                createKeys("2 3 6 _ B");
                break;
            case "STAND_F_D_DFA":
                createKeys("6 2 3 _ A");
                break;
            case "STAND_F_D_DFB":
                createKeys("6 2 3 _ B");
                break;
            case "STAND_D_DB_BA":
                createKeys("2 1 4 _ A");
                break;
            case "STAND_D_DB_BB":
                createKeys("2 1 4 _ B");
                break;
            case "AIR_D_DF_FA":
                createKeys("2 3 6 _ A");
                break;
            case "AIR_D_DF_FB":
                createKeys("2 3 6 _ B");
                break;
            case "AIR_F_D_DFA":
                createKeys("6 2 3 _ A");
                break;
            case "AIR_F_D_DFB":
                createKeys("6 2 3 _ B");
                break;
            case "AIR_D_DB_BA":
                createKeys("2 1 4 _ A");
                break;
            case "AIR_D_DB_BB":
                createKeys("2 1 4 _ B");
                break;
            case "STAND_D_DF_FC":
                createKeys("2 3 6 _ C");
                break;
            default:
                createKeys(str);
                break;
        }
    }

    private void createKeys(string str)
    {
        Key buf;
        string[] commands = str.Split(' ');
        if (!this.frameData.IsFront(playerNumber)){
            commands = reverseKey(commands);
        }

        int index = 0;
        while (index < commands.Length)
        {
            buf = new Key();
            if (commands[index].Equals("L") || commands[index].Equals("4"))
            {
                buf.L = true;
            }
            else if (commands[index].Equals("R") || commands[index].Equals("6"))
            {
                buf.R = true;
            }
            else if (commands[index].Equals("D") || commands[index].Equals("2"))
            {
                buf.D = true;
            }
            else if (commands[index].Equals("U") || commands[index].Equals("8"))
            {
                buf.U = true;
            }
            else if (commands[index].Equals("LD") || commands[index].Equals("1"))
            {
                buf.L = true;
                buf.D = true;
            }
            else if (commands[index].Equals("LU") || commands[index].Equals("7"))
            {
                buf.L = true;
                buf.U = true;
            }
            else if (commands[index].Equals("RD") || commands[index].Equals("3"))
            {
                buf.R = true;
                buf.D = true;
            }
            else if (commands[index].Equals("RU") || commands[index].Equals("9"))
            {
                buf.R = true;
                buf.U = true;
            }

            if (index + 2 < commands.Length && commands[index + 1].Equals("_"))
            {
                index += 2;
            }
            if (commands[index].Equals("A"))
            {
                buf.A = true;
            }
            else if (commands[index].Equals("B"))
            {
                buf.B = true;
            }
            else if (commands[index].Equals("C"))
            {
                buf.C = true;
            }
            skillKey.AddLast(buf);
            index++;
        }
    }

    private string[] reverseKey(string[] commands)
    {
        string[] buffer = new string[commands.Length];
        for (int i = 0; i < commands.Length; i++)
        {
            if (commands[i].Equals("L") || commands[i].Equals("4"))
            {
                buffer[i] = "6";
            }
            else if (commands[i].Equals("R") || commands[i].Equals("6"))
            {
                buffer[i] = "4";
            }
            else if (commands[i].Equals("LD") || commands[i].Equals("1"))
            {
                buffer[i] = "3";
            }
            else if (commands[i].Equals("LU") || commands[i].Equals("7"))
            {
                buffer[i] = "9";
            }
            else if (commands[i].Equals("RD") || commands[i].Equals("3"))
            {
                buffer[i] = "1";
            }
            else if (commands[i].Equals("RU") || commands[i].Equals("9"))
            {
                buffer[i] = "7";
            }
            else
            {
                buffer[i] = commands[i];
            }
        }
        return buffer;
    }

    public void SetFrameData(FrameData frameData, bool playerNumber)
    {
        this.frameData = frameData;
        this.playerNumber = playerNumber;
    }

    public bool GetSkillFlag()
    {
        return this.skillKey.Count > 0;
    }

    public Key GetSkillKey()
    {
        if (this.skillKey.Count > 0)
        {
            Key key = this.skillKey.First.Value;
            this.skillKey.RemoveFirst();
            return key;
        }
        else
        {
            return new Key();
        }
    }

    public LinkedList<Key> GetSkillKeys()
    {
        return new LinkedList<Key>(this.skillKey);
    }

    public void SkillCancel()
    {
        this.skillKey.Clear();
    }

    public bool GetPlayerNumber()
    {
        return this.playerNumber;
    }

}
