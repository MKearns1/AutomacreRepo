using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ComponentOptionDetails_LimbType : ComponentOptionDetails
{
    [Header("Limb Settings")]
    public int Joints;
    public float Length;
    public float JointSize;

    [SerializeField]
    public Vector2 minMaxNumJoints, minMaxLength, minMaxJointSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*    public void ChangeDetail(DetailChange change)
    {
        switch (change.name)
        {
            case "Joints":

                BotComponent_Walker walker = WorkshopGeneral.instance.CurrentSelectedComponentToPlace as BotComponent_Walker;

                //Joints += amount;
                walker.GetComponent<LimbCreator>().NumberOfJoints = Joints;
                walker.GetComponent<LimbCreator>().CreateJoints();
                walker.GetComponent<LimbCreator>().CreateJoints();

                break;

        }
    }*/


    public virtual void ChangeJoints(int amount)
    {
        BotComponent_LimbType Limb = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_LimbType;

        int nextAmount = Limb.LimbCreator.NumberOfJoints + amount;

        if(nextAmount < minMaxNumJoints.x ||  nextAmount > minMaxNumJoints.y) return;

        Limb.LimbCreator.NumberOfJoints = nextAmount;
        Limb.LimbCreator.CreateJoints();
        Limb.LimbCreator.CreateJoints();

        // transform.Find("Joints").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();
        Joints = nextAmount;
        UpdateUI();
    }

    public virtual void ChangeLength(float amount)
    {
        BotComponent_LimbType Limb = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_LimbType;

        float nextAmount = Limb.LimbCreator.Length + amount;

        if (nextAmount < minMaxLength.x || nextAmount > minMaxLength.y) return;

        Limb.LimbCreator.Length = nextAmount;
        Limb.LimbCreator.CreateJoints();
        Limb.LimbCreator.CreateJoints();

        // transform.Find("Length").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();
        Length = nextAmount;
        UpdateUI();

    }

    public virtual void ChangeJointSize(int amount)
    {
        BotComponent_LimbType Limb = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_LimbType;

        float nextAmount = MathF.Round((Limb.LimbCreator.JointSize + (float)amount * .1f) * 10) / 10;

        if (nextAmount < minMaxJointSize.x || nextAmount > minMaxJointSize.y) return;

        Limb.LimbCreator.JointSize = nextAmount;
        Limb.LimbCreator.CreateJoints();
        Limb.LimbCreator.CreateJoints();

        //transform.Find("FootSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();

        JointSize = nextAmount;
        UpdateUI();

    }

    public override void SetOptionVariables(BotComponent CopyComponent = null, ComponentOptionDetails CopyOptions = null)
    {
        base.SetOptionVariables(CopyComponent, CopyOptions);

        if (!isValidSet(CopyComponent, CopyOptions))
        {
            Debug.LogWarning("Can't set optionvariables - type mismatch. TYPE: " + this.GetType().ToString());
            return;
        }

        ComponentOptionDetails_LimbType LimbCopyOptions = CopyOptions as ComponentOptionDetails_LimbType;
        BotComponent_LimbType LimbCopy = CopyComponent as BotComponent_LimbType;

        if (LimbCopyOptions)
        {
            this.Joints = LimbCopyOptions.Joints;
            this.Length = LimbCopyOptions.Length;
            this.JointSize = LimbCopyOptions.JointSize;
        }
        else
        {
            this.Joints = LimbCopy.LimbCreator.NumberOfJoints;
            this.Length = LimbCopy.LimbCreator.Length;
            this.JointSize = LimbCopy.LimbCreator.JointSize;
        }

        UpdateUI();       
    }

    public override void SetComponentValues(BotComponent CopyComponent = null, ComponentOptionDetails CopyOptions = null)
    {
        base.SetComponentValues(CopyComponent, CopyOptions);

        ComponentOptionDetails_LimbType LimbCopyOptions = CopyOptions as ComponentOptionDetails_LimbType;
        BotComponent_LimbType LimbCopy = CopyComponent as BotComponent_LimbType;

        BotComponent_LimbType SelectedLimbComp = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_LimbType;
        
        int newJoints = Joints;
        float newLength = Length;
        float newJointSize = JointSize;

        if (LimbCopy == null && LimbCopyOptions == null)
        {
            Debug.LogWarning("Set ComponentValues Failed - No Data");
            return;
        }

        if (LimbCopyOptions != null)
        {
            newJoints = LimbCopyOptions.Joints;
            newLength = LimbCopyOptions.Length;
            newJointSize = LimbCopyOptions.JointSize;
        }
        else
        {
            newJoints = LimbCopy.LimbCreator.NumberOfJoints;
            newLength = LimbCopy.LimbCreator.Length;
            newJointSize = LimbCopy.LimbCreator.JointSize;
        }

        Joints = newJoints;
        Length = newLength;
        JointSize = newJointSize;

        SelectedLimbComp.LimbCreator.NumberOfJoints = newJoints;
        SelectedLimbComp.LimbCreator.Length = newLength;
        SelectedLimbComp.LimbCreator.JointSize = newJointSize;
        SelectedLimbComp.LimbCreator.CreateJoints();
        SelectedLimbComp.LimbCreator.CreateJoints();

        UpdateUI();
        return;
    }

    public virtual void CopyDetailsButton()
    {
        WorkshopGeneral.instance.CurrentCopiedOptions = Clone();
    }

    public override void PasteDetails()
    {
        base.PasteDetails();

        ComponentOptionDetails_LimbType LimbOptionsComp = WorkshopGeneral.instance.CurrentCopiedOptions as ComponentOptionDetails_LimbType;

        SetComponentValues(null,LimbOptionsComp);
    }


    public override ComponentOptionDetails Clone() 
    {
           if (WorkshopGeneral.instance.CurrentCopiedOptions != null)
            Destroy(WorkshopGeneral.instance.CurrentCopiedOptions.gameObject);

        ComponentOptionDetails_LimbType options = Instantiate(this) as ComponentOptionDetails_LimbType;
        options.Joints = this.Joints;
        options.Length = this.Length;
        options.JointSize = this.JointSize;

        return options;
    }

    public override void UpdateUI()
    {
        transform.Find("Joints").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = Joints.ToString();
        transform.Find("Length").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = Length.ToString();
        transform.Find("JointSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = JointSize.ToString();

    }
}

