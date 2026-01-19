using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

public class ComponentOptionsPopUp : MonoBehaviour
{
    [SerializeField]
    public List<Entry> OptionEntries;

    public GameObject Options_Walker;
    public GameObject CurrentOption;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOptionDetails(BotComponent Component)
    {
        switch (Component.ComponentDefaultData.Type)
        {

            case ComponentType.Walker:

                ComponentOptionDetails_Walker walkerOptionDetails = CurrentOption.GetComponent<ComponentOptionDetails>() as ComponentOptionDetails_Walker;
                walkerOptionDetails.SetOptionDetails(Component as BotComponent_Walker);

                /*int numJoints = Component.GetComponentInChildren<LimbCreator>().NumberOfJoints;
                float Length = Component.GetComponentInChildren<LimbCreator>().Length;

                ComponentOptionDetails_Walker walkerOptionDetails = CurrentOption.GetComponent<ComponentOptionDetails>() as ComponentOptionDetails_Walker;

                walkerOptionDetails.Joints = numJoints;
                walkerOptionDetails.Length = Length;

                CurrentOption.GetComponent<RectTransform>().Find("Joints").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = numJoints.ToString();
                CurrentOption.GetComponent<RectTransform>().Find("Length").GetChild(0).Find("ValueText").GetComponent<TextMeshProUGUI>().text = Length.ToString();
*/
                break;


        }
    }

    public void OnChangedComponentOptions()
    {
        switch(WorkshopGeneral.instance.SelectedComponentOnBot.ComponentDefaultData.Type)
        {
            case ComponentType.Walker:

                

                break;
        }
    }

    public void RemoveButtonPressed()
    {
        WorkshopGeneral.instance.RemoveComponentFromBot(WorkshopGeneral.instance.SelectedComponentOnBot);
    }

    public void LeaveOptions()
    {
        Destroy(CurrentOption);
    }
}

[Serializable]
public class Entry
{
    public string name;
    public EntryType type;
}

public enum EntryType
{
    Button,
}