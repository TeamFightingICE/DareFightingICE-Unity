using System.Collections.Generic;
using UnityEngine;

namespace Script.Struct
{
    [System.Serializable]
    public struct CharacterData
    {
        public bool playerNumber;
        public int hp;
        public int energy;
        public Vector2 position;
        public Rect bounds;
        public Vector2 speed;
        public State state;
        public bool front;
        public bool control;
        public int remainingFrame;
        public bool hitConfirm;
        public int hitCount;
        public int lastHitFrame;
        public List<string> inputCommands;
        public List<string> processedCommands;


        public CharacterData(CharacterData characterData)
        {
            playerNumber = characterData.playerNumber;
            hp = characterData.hp;
            energy = characterData.energy;
            position = characterData.position;
            bounds = characterData.bounds;
            speed = characterData.speed;
            state = characterData.state;
            front = characterData.front;
            control = characterData.control;
            remainingFrame = characterData.remainingFrame;
            hitConfirm = characterData.hitConfirm;
            hitCount = characterData.hitCount;
            lastHitFrame = characterData.lastHitFrame;
            inputCommands = new List<string>(characterData.inputCommands);
            processedCommands = new List<string>(characterData.processedCommands);

        }
    }
}