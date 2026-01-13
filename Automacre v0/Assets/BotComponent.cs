using System;
using UnityEngine;

public class BotComponent : MonoBehaviour
{
    public string CompName;
   // [SerializeField] public ComponentType Type;
    [SerializeField] public BotBodyBase body;
   // [SerializeField] public GameObject DefaultComponentPrefab;
    [SerializeField] public ComponentDefinition ComponentDefaultData;
    public ComponentDesignInfo DesignInfo;


    public virtual void Awake()
    {
        DesignInfo = GetDesignInfo();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DesignInfo = GetDesignInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Initialise(ComponentDesignInfo DesignInformation, BotController BC)
    {
        Debug.LogWarning("THIS IS THE BASE BotComponent Script");
    }
    public virtual void OnAttached()
    {
        Debug.LogWarning("THIS IS THE BASE BotComponent Script");
    }
    public virtual ComponentDesignInfo GetDesignInfo()
    {
        return null;
    }
}
[Serializable]
public abstract class ComponentDesignInfo
{
    public int temp;
}

public enum ComponentType
{
    None,
    Walker,
    Grabber,
    Wheel
}
