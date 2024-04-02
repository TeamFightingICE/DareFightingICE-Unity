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
            // Try not to do something like this from now and in the future
            // Refer to "Law of Demeter" for more information
            // MctsAi call this function frequently so it is not wise to use this
            // return GameObject.FindGameObjectWithTag("SimFighting").GetComponent<SimFightingController>().Simulate(frameData, playerNumber,myAct,oppAct, simulatorLimit);
            return new FrameData();
        }
    }
}
