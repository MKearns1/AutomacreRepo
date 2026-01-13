using UnityEngine;

[CreateAssetMenu(
    fileName = "WalkerDefinition",
    menuName = "Scriptable Objects/Components/Walker"
)]
public class WalkerDefinition : ComponentDefinition
{
    public GameObject DefaultFootPrefab;
}
