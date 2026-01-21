using System;
using TMPro;
using UnityEngine;

public class ComponentOptionDetails_Grabber : ComponentOptionDetails_LimbType
{
    [Header("Grabber Settings")]
    public float HandSize;
    public Vector2 minMaxHandSize;

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


    public void ChangeJoints(int amount)
    {
        base.ChangeJoints(amount);return;

        BotComponent_Walker walker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        int nextAmount = walker.GetComponentInChildren<LimbCreator>().NumberOfJoints + amount;

        if(nextAmount == 0 ||  nextAmount == 20) return;

        walker.GetComponentInChildren<LimbCreator>().NumberOfJoints = nextAmount;
        walker.GetComponentInChildren<LimbCreator>().CreateJoints();
        walker.GetComponentInChildren<LimbCreator>().CreateJoints();

        // transform.Find("Joints").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();
        Joints = nextAmount;
        UpdateUI();
    }

    public void ChangeLength(float amount)
    {
        base.ChangeLength(amount);return;

        BotComponent_Walker walker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        float nextAmount = walker.GetComponentInChildren<LimbCreator>().Length + amount;

        if (nextAmount < 2 || nextAmount > 8) return;

        walker.GetComponentInChildren<LimbCreator>().Length = nextAmount;
        walker.GetComponentInChildren<LimbCreator>().CreateJoints();
        walker.GetComponentInChildren<LimbCreator>().CreateJoints();

        // transform.Find("Length").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();
        Length = nextAmount;
        UpdateUI();

    }

    public void ChangeHandSize(int amount)
    {
        BotComponent_Grabber grabber = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Grabber;

        float nextAmount = MathF.Round((grabber.Hand.localScale.y + (float)amount*.1f)*10)/10;

        if (nextAmount < minMaxHandSize.x || nextAmount > minMaxHandSize.y) return;

        grabber.Hand.localScale = new Vector3(nextAmount, nextAmount, nextAmount);
        //walker.GetComponentInChildren<LimbCreator>().CreateJoints();
        // walker.GetComponentInChildren<LimbCreator>().CreateJoints();

        //transform.Find("FootSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();

        HandSize = nextAmount;
        UpdateUI();

    }

    public override void ChangeJointSize(int amount)
    {
        base.ChangeJointSize(amount);return;

        BotComponent_Walker walker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        float nextAmount = MathF.Round((walker.GetComponentInChildren<LimbCreator>().JointSize + (float)amount * .1f) * 10) / 10;

        if (nextAmount < 0.1 || nextAmount > 0.5f) return;

        walker.GetComponentInChildren<LimbCreator>().JointSize = nextAmount;
        walker.GetComponentInChildren<LimbCreator>().CreateJoints();
        walker.GetComponentInChildren<LimbCreator>().CreateJoints();

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

        var GrabberOptions = CopyOptions as ComponentOptionDetails_Grabber;
        var GrabberComp = CopyComponent as BotComponent_Grabber;

        if (GrabberOptions != null)
        {
            HandSize = GrabberOptions.HandSize;
        }
        else
        {
            HandSize = GrabberComp.Hand.localScale.x;
        }

        UpdateUI();
        return;

        /*int newJoints;
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

        UpdateUI();
       */
    }

    public override void SetComponentValues(BotComponent CopyComponent = null, ComponentOptionDetails CopyOptions = null)
    {
        base.SetComponentValues(CopyComponent, CopyOptions);
        ComponentOptionDetails_Grabber GrabberCopyOptions = CopyOptions as ComponentOptionDetails_Grabber;
        BotComponent_Grabber GrabberCopyComponent = CopyComponent as BotComponent_Grabber;

        BotComponent_Grabber SelectedGrabber = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Grabber;

        float newHandSize = HandSize;

        if (GrabberCopyComponent == null && GrabberCopyOptions == null)
        {
            Debug.LogWarning("Set ComponentValues Failed - No Data");
            return;
        }

        if (GrabberCopyOptions != null)
        {
            newHandSize = GrabberCopyOptions.HandSize;
        }
        else
        {
            newHandSize = GrabberCopyComponent.Hand.localScale.y;
        }

        HandSize = newHandSize;

        SelectedGrabber.Hand.localScale = new Vector3(newHandSize, newHandSize, newHandSize);

        UpdateUI();
        return;
    }

    public override void CopyDetailsButton()
    {
        WorkshopGeneral.instance.CurrentCopiedOptions = Clone();
    }

    public override void PasteDetails()
    {
        base.PasteDetails();

        ComponentOptionDetails_Grabber GrabberOptions = WorkshopGeneral.instance.CurrentCopiedOptions as ComponentOptionDetails_Grabber;

        SetComponentValues(null, GrabberOptions);

    }


    public override ComponentOptionDetails Clone() 
    {
        ComponentOptionDetails_Grabber options = base.Clone() as ComponentOptionDetails_Grabber;
        options.HandSize = this.HandSize;
        return options;
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        transform.Find("HandSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = HandSize.ToString();

    }
}

