using UnityEngine;

public enum DirectType
{
    Move,
    Harvest
}

public class BotDirection
{
    public DirectType Type;
    public Vector3 Location;
    public GameObject TargetObject;

    public BotDirection(DirectType T, Vector3 L, GameObject G)
    {
        Type = T;
        Location = L;
        TargetObject = G;
    }
}

