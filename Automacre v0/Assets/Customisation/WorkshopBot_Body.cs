using UnityEngine;

public class WorkshopBot_Body : MonoBehaviour
{
    public string BodyType;
    public Transform LowestPoint { get { return transform.Find("LowestPoint"); } }
}
