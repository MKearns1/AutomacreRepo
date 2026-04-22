using System;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;

public class ComponentOptionDetails_Walker : ComponentOptionDetails_LimbType
{
    [Header("Walker Settings")]
    public float FootSize;
    public Vector2 minMaxFootSize;
    public int MoveGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void ChangeLength(float amount)
    {
        BotComponent_Walker Limb = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        float nextAmount = Limb.LimbCreator.Length + amount;

        if (nextAmount < minMaxLength.x || nextAmount > minMaxLength.y) return;

        float min = Vector3.Distance(Limb.FootPlacementPosition().point, Limb.transform.position) * 1.2f;

        nextAmount = Mathf.Clamp(nextAmount, MathF.Round(min), 8);

        Limb.LimbCreator.Length = nextAmount;
        Limb.LimbCreator.CreateJoints();
        Limb.LimbCreator.CreateJoints();
        Limb.transform.GetComponent<SelectionHighlight>().UpdateVisual();

        // transform.Find("Length").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();
        Length = nextAmount;
        UpdateUI();

    }

    public void ChangeFootSize(int amount)
    {
        BotComponent_Walker walker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        float nextAmount = MathF.Round((walker.Foot.localScale.y + (float)amount*.1f)*10)/10;

        if (nextAmount < 0 || nextAmount > 1.2f) return;

        walker.Foot.localScale = new Vector3(nextAmount, nextAmount, nextAmount);
        //walker.GetComponentInChildren<LimbCreator>().CreateJoints();
        // walker.GetComponentInChildren<LimbCreator>().CreateJoints();

        //transform.Find("FootSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();

        FootSize = nextAmount;
        UpdateUI();

    }

    public void ChangeMoveGroup(int amount)
    {
        BotComponent_Walker walker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        int nextAmount = MoveGroup+amount;

        MoveGroup = nextAmount;

        walker.MovementGroup = MoveGroup;

        UpdateUI();

    }

    /*    public void ChangeJointSize(int amount)
        {
            BotComponent_Walker walker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

            float nextAmount = MathF.Round((walker.GetComponentInChildren<LimbCreator>().JointSize + (float)amount * .1f) * 10) / 10;

            if (nextAmount < 0.1 || nextAmount > 0.5f) return;

            walker.GetComponentInChildren<LimbCreator>().JointSize = nextAmount;
            walker.GetComponentInChildren<LimbCreator>().CreateJoints();
            walker.GetComponentInChildren<LimbCreator>().CreateJoints();

            //transform.Find("FootSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();

            JointSize = nextAmount;
            UpdateUI();

        }*/

    public override void SetOptionVariables(BotComponent CopyComponent = null, ComponentOptionDetails CopyOptions = null)
    {
        base.SetOptionVariables(CopyComponent, CopyOptions);

        if (!isValidSet(CopyComponent, CopyOptions))
        {
            Debug.LogWarning("Can't set optionvariables - type mismatch. TYPE: " + this.GetType().ToString());
            return;
        }

        var WalkerOptions = CopyOptions as ComponentOptionDetails_Walker;
        var WalkerComp = CopyComponent as BotComponent_Walker;

        if (WalkerOptions != null)
        {
            FootSize = WalkerOptions.FootSize;
            MoveGroup = WalkerOptions.MoveGroup;
        }
        else
        {
            FootSize = WalkerComp.Foot.localScale.x;
            MoveGroup = WalkerComp.MovementGroup;
        }

        UpdateUI();
        return;

/*        int newJoints;
        float newLength;
        float newFootSize;

        if (comp == null && Options == null)
        {
            Debug.LogWarning("Set CompOptionsDetails Failed - No Data");
            return;
        }

        if (Options != null)
        {
            Joints = Options.Joints;
            Length = Options.Length;
            FootSize = Options.FootSize;
            JointSize = Options.JointSize;
        }
        else
        {
            Joints = comp.GetComponentInChildren<LimbCreator>().NumberOfJoints;
            Length = comp.GetComponentInChildren<LimbCreator>().Length;
            FootSize = comp.Foot.localScale.y;
            JointSize = comp.GetComponentInChildren<LimbCreator>().JointSize;
        }

        UpdateUI();*/
       
    }

    public override void SetComponentValues(BotComponent CopyComponent = null, ComponentOptionDetails CopyOptions = null)
    {
        base.SetComponentValues(CopyComponent, CopyOptions);
        var WalkerCopyOptions = CopyOptions as ComponentOptionDetails_Walker;
        var WalkerCopyComponent = CopyComponent as BotComponent_Walker;

        var SelectedGrabber = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        float newFootSize = FootSize;
        int newMoveGroup = MoveGroup;

        if (WalkerCopyComponent == null && WalkerCopyOptions == null)
        {
            Debug.LogWarning("Set ComponentValues Failed - No Data");
            return;
        }

        if (WalkerCopyOptions != null)
        {
            newFootSize = WalkerCopyOptions.FootSize;
            newMoveGroup = WalkerCopyOptions.MoveGroup;
        }
        else
        {
            newFootSize = WalkerCopyComponent.Foot.localScale.y;
            newMoveGroup = WalkerCopyComponent.MovementGroup;
        }

        FootSize = newFootSize;
        MoveGroup = newMoveGroup;

        SelectedGrabber.Foot.localScale = new Vector3(newFootSize, newFootSize, newFootSize);
        SelectedGrabber.MovementGroup = MoveGroup;

        UpdateUI();
        return;


        /*BotComponent_Walker SelectedWalker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        int newJoints = Joints;
        float newLength = Length;
        float newFootSize = FootSize;
        float newJointSize = JointSize;

        if (CopyComponent == null && CopyOptions == null)
        {
            Debug.LogWarning("Set ComponentValues Failed - No Data");
            return;
        }

        if (CopyOptions != null)
        {
            newJoints = CopyOptions.Joints;
            newLength = CopyOptions.Length;
            newFootSize = CopyOptions.FootSize;
            newJointSize = CopyOptions.JointSize;
        }
        else
        {
            newJoints = CopyComponent.GetComponentInChildren<LimbCreator>().NumberOfJoints;
            newLength = CopyComponent.GetComponentInChildren<LimbCreator>().Length;
            newFootSize = CopyComponent.Foot.localScale.y;
            newJointSize = CopyComponent.GetComponentInChildren<LimbCreator>().JointSize;
        }

        Joints = newJoints;
        Length = newLength;
        FootSize = newFootSize;
        JointSize = newJointSize;

        SelectedWalker.GetComponentInChildren<LimbCreator>().NumberOfJoints = newJoints;
        SelectedWalker.GetComponentInChildren<LimbCreator>().Length = newLength;
        SelectedWalker.Foot.localScale = new Vector3(newFootSize, newFootSize, newFootSize);
        SelectedWalker.GetComponentInChildren<LimbCreator>().JointSize = newJointSize;
        SelectedWalker.GetComponentInChildren<LimbCreator>().CreateJoints();
        SelectedWalker.GetComponentInChildren<LimbCreator>().CreateJoints();

        UpdateUI();
        return;*/
    }

    public override void CopyDetailsButton()
    {
        WorkshopGeneral.instance.CurrentCopiedOptions = Clone();
    }

    public override void PasteDetails()
    {
        base.PasteDetails();

        var WalkerOptions = WorkshopGeneral.instance.CurrentCopiedOptions as ComponentOptionDetails_Walker;

        SetComponentValues(null, WalkerOptions);

    }

    public override ComponentOptionDetails Clone()
    {
        var options = base.Clone() as ComponentOptionDetails_Walker;
        options.FootSize = this.FootSize;
        options.MoveGroup = this.MoveGroup;
        return options;
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        transform.Find("FootSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = FootSize.ToString();
        transform.Find("MoveGroup").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = MoveGroup.ToString();

    }
}
[Serializable]
public class DetailChange
{
    public string name;
    float change;
}
