using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class FaceCalculationTest : MonoBehaviour
{
    public Transform Mean;
    Vector3 avgPos = Vector3.zero;

    public List<Transform> Points;
    public List<Transform> OrderedPoints;
    public List<float> angles;
    public List<KeyValuePair<Transform, float>> pp = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // Mean = GameObject.Find("Mean").transform;

        foreach(Transform t in transform)
        {
            avgPos += t.position;
        }
        avgPos /= transform.childCount;

        Mean.position = avgPos;

        Vector2 meanpos = Mean.position;

        Vector3 dir1 = Mean.position + Vector3.up;


        foreach(Transform t2 in transform)
        {
            Vector2 t2a = t2.position;

            //Vector3 dir2 = -t2.position + Mean.position;
           // Vector3 dir2 = (t2.position - Mean.position).normalized;
            Vector2 dir2 = (t2a - meanpos).normalized;
           

            //float quaternion = Vector3.SignedAngle(Mean.position, t2.position, Vector3.up);
            float quaternion = Vector2.SignedAngle(Vector2.up, dir2);

            angles.Add(quaternion);
            pp.Add(new KeyValuePair<Transform, float>(t2, quaternion));

            Debug.Log(t2.gameObject.name + " " +  quaternion);
        }

        pp.Sort((a, b) => a.Value.CompareTo(b.Value));
        //pp.Sort(SortOnValueX);
        //pp = pp.OrderBy(x => x.Value).ToList();

        foreach(KeyValuePair<Transform, float> k in pp)

        {
            OrderedPoints.Add(k.Key);
        }

        
    }
    private static int SortOnValueX(KeyValuePair<Transform, float> a, KeyValuePair<Transform, float> b)
    {
        return a.Value.CompareTo(b.Value);
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < pp.Count; i++)
        {
            Debug.DrawLine(pp[i].Key.position, pp[(i+1) % pp.Count].Key.position);
        }

        Vector3 dir2 = (transform.GetChild(0).position - Mean.position).normalized;
        Debug.DrawLine(Mean.transform.position, Mean.transform.position + dir2);
        Debug.DrawLine(Mean.transform.position, Mean.transform.position + Vector3.up);
    }
}
