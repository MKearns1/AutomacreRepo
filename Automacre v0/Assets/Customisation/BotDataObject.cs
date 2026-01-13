using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BotDataObject", menuName = "Scriptable Objects/BotDataObject")]
public class BotDataObject : ScriptableObject
{
    BotBodyBase body;
    List<Transform> Components = new List<Transform>();
    public Dictionary<string, AttatchPoint> AttachPoints = new Dictionary<string, AttatchPoint>();

}
