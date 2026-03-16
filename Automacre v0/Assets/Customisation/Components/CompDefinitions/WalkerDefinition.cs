using UnityEngine;

[CreateAssetMenu(
    fileName = "WalkerDefinition",
    menuName = "Scriptable Objects/Components/LimbType/Walker"
)]
public class WalkerDefinition : LimbTypeDefinition
{
    public GameObject DefaultFootPrefab;
    public Vector3 DefaultFootOffset;
}
