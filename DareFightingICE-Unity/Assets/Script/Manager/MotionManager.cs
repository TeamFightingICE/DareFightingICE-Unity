using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionManager : Singleton<MotionManager>
{
    public TextAsset zenMotion;
    public TextAsset garnetMotion;
    public TextAsset ludMotion;

    public bool isLoad = false;
    private Dictionary<string, Dictionary<string, object>> _zenMotionData = new Dictionary<string, Dictionary<string, object>>();
    private Dictionary<string, Dictionary<string, object>> _garnetMotionData = new Dictionary<string, Dictionary<string, object>>();
    private Dictionary<string, Dictionary<string, object>> _ludMotionData = new Dictionary<string, Dictionary<string, object>>();

    public void LoadMotion(TextAsset zen, TextAsset garnet, TextAsset lud)
    {
        zenMotion = zen;
        garnetMotion = garnet;
        ludMotion = lud;
    }

    public void LoadMotionData()
    {
        TextAsset[] motionFiles = { zenMotion, garnetMotion, ludMotion };
        Dictionary<string, Dictionary<string, object>>[] motionDataArrays = { _zenMotionData, _garnetMotionData, _ludMotionData };

        for (int j = 0; j < motionFiles.Length; j++)
        {
            TextAsset csvFile = motionFiles[j];
            Dictionary<string, Dictionary<string, object>> motionData = motionDataArrays[j];

            string[] lines = csvFile.text.Split('\n');
            if (lines.Length > 0)
            {
                string[] headers = lines[0].Split(',');

                for (int i = 1; i < lines.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        string[] fields = lines[i].Split(',');
                        string motionName = fields[0].Trim();
                        Dictionary<string, object> properties = new Dictionary<string, object>();

                        for (int k = 1; k < headers.Length; k++)
                        {
                            string value = fields[k].Trim();
                            if (int.TryParse(value, out int intValue))
                            {
                                properties[headers[k].Trim()] = intValue;
                            }
                            else if (bool.TryParse(value, out bool boolValue))
                            {
                                properties[headers[k].Trim()] = boolValue;
                            }
                            else
                            {
                                properties[headers[k].Trim()] = value;
                            }
                        }
                        motionData[motionName] = properties;
                    }
                }
            }
        }
    }

    public object GetMotionAttribute(string character, string motionName, string attributeName)
    {
        Dictionary<string, Dictionary<string, object>> motionData = null;

        switch (character.ToLower())
        {
            case "zen":
                motionData = _zenMotionData;
                break;
            case "garnet":
                motionData = _garnetMotionData;
                break;
            case "lud":
                motionData = _ludMotionData;
                break;
            default:
                return null;
        }

        if (motionData != null && motionData.TryGetValue(motionName, out Dictionary<string, object> attributes))
        {
            if (attributes.TryGetValue(attributeName, out object value))
            {
                return value;
            }
        }

        return null;
    }
    
    public int GetStartGiveEnergyForMotion(string character, string motionName)
    {
        object attributeValue = GetMotionAttribute(character, motionName, "attack.StartAddEnergy");

        if (attributeValue is int energy)
        {
            return energy;
        }
        else
        {
            return 1; // Default or error value
        }
    }
    
    public MotionAttribute GetMotionAttributes(string character, string motionName) {
        Dictionary<string, Dictionary<string, object>> motionData = null;

        switch (character.ToLower()) {
            case "zen":
                motionData = _zenMotionData;
                break;
            case "garnet":
                motionData = _garnetMotionData;
                break;
            case "lud":
                motionData = _ludMotionData;
                break;
            default:
                return null;
        }

        if (motionData != null && motionData.TryGetValue(motionName, out var attributes)) {
            MotionAttribute motionAttributes = new()
            {
                hitDamage = GetAttributeValue<int>(attributes, "attack.HitDamage"),
                guardDamage = GetAttributeValue<int>(attributes, "attack.GuardDamage"),
                startAddEnergy = GetAttributeValue<int>(attributes, "attack.StartAddEnergy"),
                hitAddEnergy = GetAttributeValue<int>(attributes, "attack.HitAddEnergy"),
                guardAddEnergy = GetAttributeValue<int>(attributes, "attack.GuardAddEnergy"),
                giveEnergy = GetAttributeValue<int>(attributes, "attack.GiveEnergy"),
                impactX = GetAttributeValue<int>(attributes, "attack.ImpactX"),
                impactY = GetAttributeValue<int>(attributes, "attack.ImpactY"),
                attackType = GetAttackType(GetAttributeValue<int>(attributes, "attack.AttackType")),
                isDown = GetAttributeValue<bool>(attributes, "attack.DownProp"),
                activeTime = GetAttributeValue<int>(attributes, "attack.Active")
            };
            return motionAttributes;
        }
        return null;
    }

    private AttackType GetAttackType(int type)
    {
        AttackType attackType = AttackType.MOVE;
        switch (type)
        {
            case 0:
                attackType = AttackType.MOVE;
                break;
            case 1:
                attackType = AttackType.HIGH;
                break;
            case 2:
                attackType = AttackType.MIDDLE;
                break;
            case 3:
                attackType = AttackType.LOW;
                break;
            case 4:
                attackType = AttackType.THROW;
                break;
        }
        return attackType;
    }
    private T GetAttributeValue<T>(Dictionary<string, object> attributes, string key, T defaultValue = default(T)) {
        if (attributes.TryGetValue(key, out var value) && value is T typedValue) {
            return typedValue;
        }

        return defaultValue; // Return default value for T if not found or not the expected type
    }
}