using UnityEngine;

public class Transformable : MonoBehaviour
{
    public TransformGizmo Gizmo;
    public Transform TargetTransform;
    public bool hasX = true;
    public bool hasY = true;    
    public bool hasZ = true;    
    //{ get { return transform.parent.GetComponentInChildren<LimbCreator>().Pole; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        Destroy(WorkshopGeneral.instance.CurTransformGizmo);
        GameObject newGizmo = Instantiate(WorkshopGeneral.instance.TransformGizmoPrefab, transform.position, Quaternion.Euler(0,0,0));
        Gizmo = newGizmo.GetComponent<TransformGizmo>();
        WorkshopGeneral.instance.CurTransformGizmo = Gizmo.gameObject;
        Gizmo.Initialise(TargetTransform, this);
    }
}
