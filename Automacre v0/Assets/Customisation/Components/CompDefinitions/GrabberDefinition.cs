using UnityEngine;

[CreateAssetMenu(
    fileName = "GrabberDefinition",
    menuName = "Scriptable Objects/Components/LimbType/Grabber"
)]
public class GrabberDefinition : LimbTypeDefinition
{
    public GameObject DefaultClawPrefab;
    public Vector3 DefaultClawRestOffset;
}
