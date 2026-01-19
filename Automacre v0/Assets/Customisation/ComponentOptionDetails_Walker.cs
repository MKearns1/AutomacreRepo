using System;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ComponentOptionDetails_Walker : ComponentOptionDetails
{
    public int Joints;
    public float Length;
    public float FootSize;
    public float JointSize;


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

    public void ChangeJointSize(int amount)
    {
        BotComponent_Walker walker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

        float nextAmount = MathF.Round((walker.Foot.localScale.y + (float)amount * .1f) * 10) / 10;

        if (nextAmount < 0 || nextAmount > 1.2f) return;

        walker.GetComponentInChildren<LimbCreator>().JointSize = nextAmount;
        walker.GetComponentInChildren<LimbCreator>().CreateJoints();
        walker.GetComponentInChildren<LimbCreator>().CreateJoints();

        //transform.Find("FootSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = nextAmount.ToString();

        JointSize = nextAmount;
        UpdateUI();

    }

    public void SetOptionDetails(BotComponent_Walker comp = null, ComponentOptionDetails_Walker Options = null)
    {
        int newJoints;
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

        }
        else
        {
            Joints = comp.GetComponentInChildren<LimbCreator>().NumberOfJoints;
            Length = comp.GetComponentInChildren<LimbCreator>().Length;
            FootSize = comp.Foot.localScale.y;
        }

        UpdateUI();
       
    }

    public void SetComponentValues(BotComponent_Walker CopyComponent = null, ComponentOptionDetails_Walker CopyOptions = null)
    {
        BotComponent_Walker SelectedWalker = WorkshopGeneral.instance.SelectedComponentOnBot as BotComponent_Walker;

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
        return;
    }

    public void CopyDetailsButton()
    {
        WorkshopGeneral.instance.CurrentCopiedOptions = Clone();
    }

    public void PasteDetails()
    {
        if (WorkshopGeneral.instance.CurrentCopiedOptions == null)
        {
            Debug.LogWarning("Cannot Paste component details - Nothing copied");
            return;
        }
        if (WorkshopGeneral.instance.CurrentCopiedOptions as ComponentOptionDetails_Walker == null)
        {
            Debug.LogWarning("Cannot Paste component details - incompatible type");
            return;
        }

        ComponentOptionDetails_Walker walkerOptionsComp = WorkshopGeneral.instance.CurrentCopiedOptions as ComponentOptionDetails_Walker;

        // Joints = walkerComp.Joints;
        // Length = walkerComp.Length;
        // FootSize = walkerComp.FootSize;

        SetComponentValues(null,walkerOptionsComp);
    }


    public override ComponentOptionDetails Clone() 
    {
        /*return new ComponentOptionDetails_Walker
        {
            Joints = this.Joints,
            Length = this.Length,
            FootSize = this.FootSize,
        };*/

        if(WorkshopGeneral.instance.CurrentCopiedOptions != null)
        Destroy(WorkshopGeneral.instance.CurrentCopiedOptions.gameObject);

        ComponentOptionDetails_Walker options = Instantiate(this) as ComponentOptionDetails_Walker;
        options.Joints = this.Joints;
        options.Length = this.Length;
        options.FootSize = this.FootSize;
        options.JointSize = this.JointSize;

        return options;
    }

    public void UpdateUI()
    {
        transform.Find("Joints").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = Joints.ToString();
        transform.Find("Length").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = Length.ToString();
        transform.Find("FootSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = FootSize.ToString();
        transform.Find("JointSize").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = FootSize.ToString();

    }
}
[Serializable]
public class DetailChange
{
    public string name;
    float change;
}
