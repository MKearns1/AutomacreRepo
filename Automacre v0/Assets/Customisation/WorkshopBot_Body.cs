using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class WorkshopBot_Body : MonoBehaviour
{
    public string BodyType;
    public Transform LowestPoint { get { return transform.Find("LowestPoint"); } }

    private void Start()
    {
        transform.AddComponent<AreaConstraint>();
        GetComponent<Transformable>().Constraint = GetComponent<AreaConstraint>();
    }
}
