using System;
using UnityEditor.UIElements;
using UnityEngine;

public class ComponentOptionDetails_Walker : ComponentOptionDetails
{
    public int Joints;
    public float Length;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeDetail(DetailChange change)
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
    }


    public void ChangeJoints(int amount)
    {
        BotComponent_Walker walker = WorkshopGeneral.instance.CurrentSelectedComponentToPlace as BotComponent_Walker;

        Joints += amount;
        walker.GetComponent<LimbCreator>().NumberOfJoints = Joints;
        walker.GetComponent<LimbCreator>().CreateJoints();
        walker.GetComponent<LimbCreator>().CreateJoints();

        Debug.Log(amount);
    }


    public void zz()
    {

    }
}
[Serializable]
public class DetailChange
{
    public string name;
    float change;
}
