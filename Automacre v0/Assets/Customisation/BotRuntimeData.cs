using System.Collections.Generic;
using UnityEngine;

public class BotRuntimeData
{
    BotBodyBase body;
    List<Transform> Components = new List<Transform>();
    public Dictionary<string, AttatchPoint> AttachPoints = new Dictionary<string, AttatchPoint>();

}
