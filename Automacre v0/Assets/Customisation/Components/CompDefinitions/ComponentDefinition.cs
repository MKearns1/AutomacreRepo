using UnityEngine;

[CreateAssetMenu(fileName = "ComponentDefinition", menuName = "Scriptable Objects/ComponentDefinition")]
public class ComponentDefinition : ScriptableObject
{
    public GameObject DefaultPrefab;
    public ComponentType Type;
    public bool ProvidesSupport;
}
