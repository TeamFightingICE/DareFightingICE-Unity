using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Script.AIInterface;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class MctsAi23i : IAIInterface
{
    private bool isPlayerOne;
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;
    private Key key;
    private CommandCenter commandCenter;
    private GameData gameData;
    private Simulator simulator;

    private FrameData simulatorAheadFrameData;
    private LinkedList<Action> myActions;
    private LinkedList<Action> oppActions;
    private CharacterData myCharacter, oppCharacter;

    private static readonly int FRAME_AHEAD = 14;
    private List<MotionAttribute> myMotion;
    private List<MotionAttribute> oppMotion;

    private Action[] actionAir, actionGround;
    private Action spSkill;
    private Node rootNode;

    public static readonly bool DEBUG_MODE = false;

    public bool IsBlind()
    {
        return false;
    }

    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        this.isPlayerOne = isPlayerOne;
        this.gameData = gameData;
        this.key = new Key();
        this.frameData = new FrameData();
        this.commandCenter = new CommandCenter();
        actionAir = new Action[] { Action.AIR_GUARD, Action.AIR_A, Action.AIR_B, Action.AIR_DA, Action.AIR_DB,
                Action.AIR_FA, Action.AIR_FB, Action.AIR_UA, Action.AIR_UB, Action.AIR_D_DF_FA, Action.AIR_D_DF_FB,
                Action.AIR_F_D_DFA, Action.AIR_F_D_DFB, Action.AIR_D_DB_BA, Action.AIR_D_DB_BB };
        actionGround = new Action[] { Action.STAND_D_DB_BA, Action.BACK_STEP, Action.FORWARD_WALK, Action.DASH,
                Action.JUMP, Action.FOR_JUMP, Action.BACK_JUMP, Action.STAND_GUARD, Action.CROUCH_GUARD, Action.THROW_A,
                Action.THROW_B, Action.STAND_A, Action.STAND_B, Action.CROUCH_A, Action.CROUCH_B, Action.STAND_FA,
                Action.STAND_FB, Action.CROUCH_FA, Action.CROUCH_FB, Action.STAND_D_DF_FA, Action.STAND_D_DF_FB,
                Action.STAND_F_D_DFA, Action.STAND_F_D_DFB, Action.STAND_D_DB_BB };
        spSkill = Action.STAND_D_DF_FC;

        this.myMotion = this.gameData.GetMotionData(this.isPlayerOne);
        this.oppMotion = this.gameData.GetMotionData(!this.isPlayerOne);
    }

    public void GetNonDelayFrameData(FrameData frameData)
    {

    }

    public void GetInformation(FrameData frameData, bool isControl)
    {
        this.frameData = frameData;
        this.commandCenter.SetFrameData(frameData, this.isPlayerOne);
        this.myCharacter = frameData.getCharacter(this.isPlayerOne);
        this.oppCharacter = frameData.getCharacter(!this.isPlayerOne);
    }

    public void GetAudioData(AudioData audioData)
    {
        this.audioData = audioData;
    }

    public void GetScreenData(ScreenData screenData)
    {
        this.screenData = screenData;
    }

    public void Processing()
    {
        if (frameData.EmptyFlag || frameData.RemainingFrameNumber <= 0)
        {
            return;
        }

        // implement Mcts logic here
        if (commandCenter.GetSkillFlag())
        {
            key = commandCenter.GetSkillKey();
        }
        else
        {
            key.Empty();
            commandCenter.SkillCancel();
            mctsPrepare();
            this.rootNode = new Node(simulatorAheadFrameData, null, myActions, oppActions, gameData, isPlayerOne, commandCenter);

            rootNode.CreateNode();
            Action bestAction = rootNode.mcts();
            commandCenter.CommandCall(nameof(bestAction));
            
        }
    }

    public Key Input()
    {
        return key;
    }

    public void RoundEnd(RoundResult result)
    {

    }

    public void Close()
    {

    }

    private void mctsPrepare()
    {
        simulatorAheadFrameData = simulator.simulate(frameData, this.isPlayerOne, null, null, FRAME_AHEAD);

        myCharacter = simulatorAheadFrameData.getCharacter(this.isPlayerOne);
        oppCharacter = simulatorAheadFrameData.getCharacter(!this.isPlayerOne);

        setMyAction();
        setOppAction();
    }

    private void setMyAction()
    {
        myActions.Clear();
        int energy = myCharacter.Energy;
        if (myCharacter.State == State.Air)
        {
            for(int i = 0; i < actionAir.Length; i++)
            {
                
                if (Math.Abs(myMotion.ElementAt((int)Enum.Parse(typeof(Action), actionAir[i].ToString())).startAddEnergy) <= energy)
                {
                    myActions.AddLast(actionAir[i]);
                }
               
            }
        }
        else
        {
            if (Math.Abs(myMotion.ElementAt((int)Enum.Parse(typeof(Action), spSkill.ToString())).startAddEnergy) <= energy)
            {
                myActions.AddLast(spSkill);
            }
            for (int i = 0; i < actionGround.Length; i++)
            {
                if (Math.Abs(myMotion.ElementAt((int)Enum.Parse(typeof(Action), actionGround[i].ToString())).startAddEnergy) <= energy)
                {
                    myActions.AddLast(actionGround[i]);
                }
            }
        }
    }

    private void setOppAction()
    {
        oppActions.Clear();
        int energy = oppCharacter.Energy;
        if (myCharacter.State == State.Air)
        {
            for (int i = 0; i < actionAir.Length; i++)
            {

                if (Math.Abs(oppMotion.ElementAt((int)Enum.Parse(typeof(Action), actionAir[i].ToString())).startAddEnergy) <= energy)
                {
                    oppActions.AddLast(actionAir[i]);
                }

            }
        }
        else
        {
            if (Math.Abs(oppMotion.ElementAt((int)Enum.Parse(typeof(Action), spSkill.ToString())).startAddEnergy) <= energy)
            {
                oppActions.AddLast(spSkill);
            }
            for (int i = 0; i < actionGround.Length; i++)
            {
                if (Math.Abs(oppMotion.ElementAt((int)Enum.Parse(typeof(Action), actionGround[i].ToString())).startAddEnergy) <= energy)
                {
                    oppActions.AddLast(actionGround[i]);
                }
            }
        }
    }

    private class Node
    {
        public static readonly int UCT_TIME = 165 * 10000;
        public static readonly int ITERATION_LIMIT = 23;
        public static readonly double UCB_C = 3;
        public static readonly int UCT_TREE_DEPTH = 2;
        public static readonly int UCT_CREATE_NODE_THRESHOLD = 10;
        public static readonly int SIMULATION_TIME = 60;
        private System.Random rnd;
        private Node parent;
        private Node[] children;
        private int depth;
        private int games;
        private double ucb;
        private double score;
        private LinkedList<Action> myActions, oppActions, selectedMyActions;
        private Simulator simulator;
        private int oppOriginalHP, myOriginalHP;
        private FrameData frameData;
        private Boolean playerNumber;
        private CommandCenter commandCenter;
        private GameData gameData;
        private Boolean isCreateNode;
        private LinkedList<Action> mAction, oppAction;
        public Node(FrameData frameData, Node parent, LinkedList<Action> myActions,
            LinkedList<Action> oppActions, GameData gameData, bool playerNumber,
            CommandCenter commandCenter)
        {
            this.frameData = frameData;
            this.parent = parent;
            this.myActions = myActions;
            this.oppActions = oppActions;
            this.gameData = gameData;
            this.simulator = new Simulator(gameData);
            this.playerNumber = playerNumber;
            this.commandCenter = commandCenter;

            this.selectedMyActions = new LinkedList<Action>();

            this.rnd = new System.Random();
            this.mAction = new LinkedList<Action>();
            this.oppAction = new LinkedList<Action>();

            CharacterData myCharacter = frameData.getCharacter(playerNumber);
            CharacterData oppCharacter = frameData.getCharacter(!playerNumber);
            this.myOriginalHP = myCharacter.Hp;
            this.oppOriginalHP = oppCharacter.Hp;

            if (this.parent != null)
            {
                this.depth = this.parent.depth + 1;
            }
            else
            {
                this.depth = 0;
            }
        }
        public Node(FrameData frameData, Node parent, LinkedList<Action> myActions, LinkedList<Action> oppActions, GameData gameData,
            Boolean playerNumber, CommandCenter commandCenter, LinkedList<Action> selectedMyActions) : this(frameData, parent, myActions, oppActions, gameData, playerNumber, commandCenter)
        {

            this.selectedMyActions = selectedMyActions;

        }

        public Action mcts()
        {
            long start = nanoTime();
            for (int i = 0; nanoTime() - start <= UCT_TIME && i < ITERATION_LIMIT; i++)
            {
                uct();
            }
            return getBestVisitAction();
        }

        public double playout()
        {
            this.mAction.Clear();
            this.oppAction.Clear();

            for (int i = 0; i < this.selectedMyActions.Count; i++) { 
                this.mAction.AddLast(this.selectedMyActions.ElementAt(i));
            }

            for (int i = 0; i < 5 - this.selectedMyActions.Count; i++)
            {
                this.mAction.AddLast(this.myActions.ElementAt(this.rnd.Next(0, this.myActions.Count)));
            }

            for (int i = 0; i < 5; i++)
            {
                this.oppAction.AddLast(this.oppAction.ElementAt(this.rnd.Next(0, this.oppActions.Count)));
            }

            FrameData nFrameData = simulator.simulate(frameData, playerNumber, this.mAction, this.oppAction, SIMULATION_TIME);

            return getScore(nFrameData);
        }

        public double uct()
        {
            Node selectedNode = null;
            double bestUcb = -99999;
            foreach (Node child in this.children) 
            { 
                if (child.games == 0)
                {
                    child.ucb = 9999 + this.rnd.Next(0, 50);
                }
                else{
                    child.ucb = getUcb(child.score / child.games, games, child.games);
                }
                
                if (bestUcb < child.ucb)
                {
                    selectedNode = child;
                    bestUcb = child.ucb;
                }
            }

            double score = 0;
            if (selectedNode.games == 0)
            {
                score = selectedNode.playout();
            }
            else
            {
                if (selectedNode.children == null)
                {
                    if (selectedNode.depth < UCT_TREE_DEPTH)
                    {
                        if (UCT_CREATE_NODE_THRESHOLD <= selectedNode.games)
                        {
                            selectedNode.CreateNode();
                            selectedNode.isCreateNode = true;
                            score = selectedNode.uct();
                        }
                        else
                        {
                            score = selectedNode.playout();
                        }
                    }
                    else
                    {
                        score = selectedNode.playout();
                    }
                }
                else
                {
                    if (selectedNode.depth < UCT_TREE_DEPTH)
                    {
                        score = selectedNode.uct();
                    }
                    else
                    {
                        selectedNode.playout();
                    }
                }
            }
            selectedNode.games++;
            selectedNode.score += score;
            if (depth == 0)
            {
                games++;
            }
            return score;
        }

        public void CreateNode()
        {
            this.children = new Node[this.myActions.Count];
            for (int i = 0; i < this.children.Length; i++)
            {
                LinkedList<Action> my = new LinkedList<Action> ();
                foreach(Action act in selectedMyActions)
                {
                    my.AddLast(act);
                }
                my.AddLast(this.myActions.ElementAt(i));
                this.children[i] = 
                    new Node(frameData, this, this.myActions, this.oppActions, this.gameData, this.playerNumber, this.commandCenter, my);
            }
        }

        public Action getBestVisitAction()
        {
            int selected = -1;
            double bestGames = -9999;
            for (int i = 0; i < children.Length; i++)
            {

                
                if (bestGames < children[i].games)
                {
                    bestGames = children[i].games;
                    selected = i;
                }
            }

            return this.myActions.ElementAt(selected);

        }


        public Action getBestScoreAction()
        {
            int selected = -1;
            double bestScore = -9999;
            for (int i = 0; i < children.Length; i++)
            {

                double meanScore = children[i].score / children[i].games;
                if (bestScore < meanScore)
                {
                    bestScore = meanScore;
                    selected = i;
                }
            }

            return this.myActions.ElementAt(selected);

        }

        public int getScore(FrameData fd)
        {
            return (fd.getCharacter(playerNumber).Hp - myOriginalHP) - (fd.getCharacter(!playerNumber).Hp - oppOriginalHP);
        }

        public double getUcb(double score, int n, int ni)
        {
            return score + UCB_C * Math.Sqrt((2 * Math.Log(n)) / ni);
        }


        private static long nanoTime()
        {
            long nano = 10000L * Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }
    }
}
