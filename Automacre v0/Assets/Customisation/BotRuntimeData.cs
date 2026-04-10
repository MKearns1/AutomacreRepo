using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BotRuntimeData
{
    BotBodyBase body;
    public List<Transform> Components = new();
    public Dictionary<string, AttatchPoint> AttachPoints = new Dictionary<string, AttatchPoint>();
    public float OffsetFromGround;

    public List<Transform> poo()
    {
        List<Transform> result = new List<Transform>();
        foreach(var ap in AttachPoints)
        {
            if (ap.Value == null) continue;
            if (ap.Value.botComponent == null) continue;

            result.Add(ap.Value.botComponent.transform);
        }
        return result;
    }
}
