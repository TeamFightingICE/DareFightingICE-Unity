using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.AIInterface
{
    internal class Simulator
    {
        public Simulator(GameData gameData)
        {

        }

        public FrameData simulate(FrameData frameData, bool playerNumber, LinkedList<Action> myAct, LinkedList<Action> oppAct, int simulatorLimit)
        {
            return GameObject.FindGameObjectWithTag("SimFighting").GetComponent<SimFightingController>().Simulate(frameData, playerNumber,myAct,oppAct, simulatorLimit);
        }
    }
}
