using UnityEngine;

public class childtest : parenttest
{
    public override void FF(parenttest parent)
    {
        base.FF(parent);

        Debug.Log(parent is parenttest);
    }
}
