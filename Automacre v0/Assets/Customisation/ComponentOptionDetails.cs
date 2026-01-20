using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ComponentOptionDetails : MonoBehaviour
{
    public ComponentType componentType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public virtual ComponentOptionDetails Clone() { return null; }
    public abstract ComponentOptionDetails Clone();

    public virtual void UpdateUI() { }

    public virtual void SetComponentValues(BotComponent CopyComponent = null, ComponentOptionDetails CopyOptions = null)
    {
        if (!isValidSet(CopyComponent, CopyOptions))
        {
            Debug.LogWarning("Can't set Component Values - type mismatch. TYPE: " + this.GetType().ToString());
        }
        UpdateUI();
        return;
    }
    public virtual void SetOptionVariables(BotComponent CopyComponent = null, ComponentOptionDetails CopyOptions = null)
    {
        if (!isValidSet(CopyComponent, CopyOptions))
        {
            Debug.LogWarning("Can't set optionvariables - type mismatch. TYPE: " + this.GetType().ToString());
            return;
        }

    }

    public virtual void PasteDetails()
    {
        if (WorkshopGeneral.instance.CurrentCopiedOptions == null)
        {
            Debug.LogWarning("Cannot Paste component details - Nothing copied");
            return;
        }
        if (WorkshopGeneral.instance.CurrentCopiedOptions as ComponentOptionDetails_LimbType == null)
        {
            Debug.LogWarning("Cannot Paste component details - incompatible type");
            return;
        }

    }

    public virtual bool isValidSet(BotComponent CopyComponent = null, ComponentOptionDetails CopyOptions = null)
    {
        bool validComp = CopyComponent.ComponentDefaultData.Type == this.componentType;
        bool validOptions = CopyOptions.GetType() == this.GetType() && CopyOptions != null;

        return validComp || validOptions;
    }
}

public class OptionsData
{

}
