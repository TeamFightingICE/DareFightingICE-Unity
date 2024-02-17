using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAIUtil
{
    public static bool IsAIExist(string aiName)
    {
        return aiName switch
        {
            "MctsAi23i" => true,
            "SampleAI" => true,
            "Sandbox" => true,
            _ => false,
        };
    }

    public static IAIInterface GetAIInterface(string aiName)
    {
        return aiName switch
        {
            "MctsAi23i" => new MctsAi23i(),
            "SampleAI" => new SampleAI(),
            "Sandbox" => new Sandbox(),
            _ => null,
        };
    }
}
