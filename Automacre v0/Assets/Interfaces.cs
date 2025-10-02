using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Interfaces
{
    
}

public interface IClickable
{
    void OnClick(Vector3 hitPoint, List<BotScript> AffectedBots);
}
