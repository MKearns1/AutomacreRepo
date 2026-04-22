using UnityEngine;
using System.Collections.Generic;

public class BotBodyBase : MonoBehaviour
{
    public float moveSpeed;

    public LayerMask GroundLayer;
    public float DesiredOffsetFromGround;
    public float MaxOffsetFromGround;
    public List<ProceduralPart> ProceduralComponents;
    float heightVelocity = 0;
    public float DistanceFromGround;
    public float PredictedDistanceFromGround;
    public Vector3 PredictedBodyPos;
    public float PredictionLookAheadTime = .5f;
    Vector3 smoothedUp = Vector3.zero;
    public List<BotComponent> CurComponents;
    public Transform LowestPoint;
    public LayerMask IgnoreLayers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetAllProceduralComponents();
    }

    // Update is called once per frame
    void Update()
    { 

        List<Vector3> SupportPositions = new List<Vector3>();

        float CompAvgHeight = 0;

        //if(ProceduralComponents.Count == 0) return;

        foreach (var comp in ProceduralComponents)
        {
            string type = comp.GetType().ToString();
            if (type != "ProceduralWalker" && type != "ProceduralWheel") continue;

            float y = 0;
            y = comp.EndPoint.position.y;

            if (comp.GetType() == typeof(ProceduralWalker))
            {
                ProceduralWalker walkcomp = (ProceduralWalker)comp;
                
                if (walkcomp.IsMoving)
                {
                    y = Mathf.Lerp(walkcomp.EndPoint.position.y, walkcomp.NextStepPos.y,.5f);
                }
            }

            CompAvgHeight += y;

            Vector3 pos = comp.EndPoint.position;
            pos.y = y;

            SupportPositions.Add(pos);
        }

        CompAvgHeight /= ProceduralComponents.Count;
        CompAvgHeight += DesiredOffsetFromGround;

        Vector3 upNormal = Vector3.up;

        if(SupportPositions.Count >= 3)
        {
            upNormal = GetNormal(SupportPositions).normalized;
        }
        else if (SupportPositions.Count == 2)
        {
            upNormal = GetNormal2D(SupportPositions[0], SupportPositions[1]);
        }

        for (int i = 0; i < SupportPositions.Count; i++)
        {
            Debug.DrawLine(
                SupportPositions[i],
                SupportPositions[(i + 1) % SupportPositions.Count],
                Color.green
            );
        }


        float smoothedHeight;
        smoothedHeight = transform.position.y;

        smoothedHeight = Mathf.SmoothDamp(smoothedHeight, CompAvgHeight, ref heightVelocity, .25f);


        PredictedBodyPos = transform.position + transform.parent.GetComponentInChildren<BotAI>().NavAgent.velocity * PredictionLookAheadTime;

        Vector3 BotPosition = GetComponentInParent<BotController_Procedural>().Ai.transform.position;

        float PredictedGroundY = 0;
        float currentGroundY = 0;

        RaycastHit PredictedgroundHit;

        if (Physics.Raycast(PredictedBodyPos, Vector3.down, out PredictedgroundHit, float.PositiveInfinity, ~IgnoreLayers))//LayerMask.GetMask("Ground")
        {
            PredictedBodyPos.y = PredictedgroundHit.point.y + DesiredOffsetFromGround;
            PredictedDistanceFromGround = PredictedgroundHit.distance;
            PredictedGroundY = PredictedgroundHit.point.y;

            //Debug.LogWarning(transform.parent.name+PredictedgroundHit.collider.gameObject);
        }

        RaycastHit groundHit;
        if (Physics.Raycast(LowestPoint.position, Vector3.down, out groundHit, float.PositiveInfinity, ~IgnoreLayers))//LayerMask.GetMask("Ground")
        {
            DistanceFromGround = groundHit.distance;
            currentGroundY = groundHit.point.y;
           // Debug.LogWarning(transform.parent.name + groundHit.collider.gameObject);

        }

        if (PredictedGroundY > currentGroundY)
        {
            BotPosition.y = PredictedgroundHit.point.y + DesiredOffsetFromGround;
        }
        else
        {
            BotPosition.y = groundHit.point.y + DesiredOffsetFromGround;
        }


        
        if (Vector3.Dot(upNormal, Vector3.up) < 0f)
        {
            upNormal = -upNormal;
        }

        transform.position = Vector3.Lerp(transform.position, BotPosition, Time.deltaTime * 10);

        Vector3 forward = Vector3.ProjectOnPlane(GetComponentInParent<BotController_Procedural>().Ai.transform.forward, upNormal).normalized;
        if(forward.magnitude < 0.1f)
        {
            forward = GetComponentInParent<BotController_Procedural>().Ai.transform.forward.normalized;
        }


        Quaternion targetRot = Quaternion.LookRotation(forward, upNormal);

        smoothedUp = Vector3.Slerp(smoothedUp, upNormal, Time.deltaTime * 5f).normalized;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
    }




/*    public ProceduralWalker GetMostBehindComponent()
    {
        ProceduralWalker furthestComponent = null;
        float furthestdist = -Mathf.Infinity;

        foreach (var comp in ProceduralComponents)
        {
            float dist = Vector3.Distance(comp.EndPoint.position, comp.NextStepPos);

            if(dist > furthestdist)
            {
                furthestdist = dist;
                furthestComponent = comp;
            }

        }

        return furthestComponent;
    }*/

    public void GetAllProceduralComponents()
    {
        ProceduralComponents.Clear();
        foreach (Transform Attachpoint in transform)
        {
            if(Attachpoint.gameObject.GetComponent<AttatchPoint>() == null)continue;

            if(Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent == null) continue;

            BotComponent comp = Attachpoint.gameObject.GetComponent<AttatchPoint>().botComponent;
            CurComponents.Add(comp);

            if (comp.GetComponentInChildren<ProceduralPart>(false) == null)continue;

            ProceduralComponents.Add(comp.GetComponentInChildren<ProceduralPart>(false));
        }
    }

    public ProceduralWalker GetShortestSupportComponent()
    {
        if (ProceduralComponents == null || ProceduralComponents.Count == 0)
            return null;

        ProceduralWalker shortest = null;
        float shortestLength = float.MaxValue;

        foreach (var part in ProceduralComponents)
        {
            var walker = part as ProceduralWalker;
            if (walker == null)
                continue;


            if (walker.maxLimbLength < shortestLength)
            {
                shortestLength = walker.maxLimbLength;
                shortest = walker;
            }

        }
        return shortest;
    }


    public Vector3 GetNormal(List<Vector3> Points)
    {
        Points = OrderPoints(Points);

        Vector3 avgPos = Vector3.zero;
        foreach (Vector3 foot in Points)
        {
            avgPos += foot;
        }
        avgPos /= Points.Count;

        float minStance = 0.2f;
        float maxStance = 10;

        float stanceSize = 0f;

        Vector3 normal = Vector3.zero;

        for (int i = 0; i < Points.Count; i++)
        {
            Vector3 current = Points[i];
            Vector3 next = Points[(i + 1) % Points.Count];

            normal.x += (current.y - next.y) * (current.z + next.z);
            normal.y += (current.z - next.z) * (current.x + next.x);
            normal.z += (current.x - next.x) * (current.y + next.y);

            stanceSize += Vector2.Distance(new Vector2(Points[i].x, Points[i].z), new Vector2(avgPos.x, avgPos.z));
        }

        float tiltWeight = Mathf.InverseLerp(minStance, maxStance, stanceSize);

        //Debug.Log(stanceSize);
        Vector3 adjustedNormal = Vector3.Slerp(Vector3.up, normal, tiltWeight);


        return adjustedNormal.normalized;

/*
        List<Vector3> OrderedPoints = new List<Vector3>();
        List<float> angles = new List<float>();
        List<KeyValuePair<Vector3, float>> pp = new();

        Vector3 avgPos = Vector3.zero;

        foreach (Vector3 foot in Points)
        {
            avgPos += foot;
        }
        avgPos /= Points.Count;
        Vector2 avgPos2D = new Vector2(avgPos.x, avgPos.z);

        Vector3 dir1 = avgPos + Vector3.forward;


        foreach (Vector3 foot in Points)
        {
            Vector2 t2a = new Vector2(foot.x,foot.z);

            Vector2 dir2 = (t2a - avgPos2D).normalized;

            float quaternion = Vector2.SignedAngle(Vector2.left, dir2);

            angles.Add(quaternion);
            pp.Add(new KeyValuePair<Vector3, float>(foot, quaternion));

            //Debug.Log(t2.gameObject.name + " " + quaternion);
        }

        pp.Sort((a, b) => a.Value.CompareTo(b.Value));

        foreach (KeyValuePair<Vector3, float> k in pp)
        {
            OrderedPoints.Add(k.Key);
        }


        Vector3 averageNormal = Vector3.zero;

        for (int i = 0; i < OrderedPoints.Count; i++)
        {
            Vector3 current = OrderedPoints[i];
            Vector3 next = OrderedPoints[(i + 1) % OrderedPoints.Count];

            Vector3 directionFromCenterToCurrent = current - avgPos;
            Vector3 directionFromCenterToNext = next - avgPos;

            Vector3 planeNormal = Vector3.Cross(
                directionFromCenterToCurrent,
                directionFromCenterToNext
            );

            if (planeNormal != Vector3.zero)
            {
                averageNormal += planeNormal.normalized;
            }
        }
        return averageNormal;
*/



/*        Vector3 center = Vector3.zero;

        foreach (Vector3 foot in Points)
        {
            center.x += foot.x;
            center.y += foot.y;
            center.z += foot.z;
        }

        center.x /= Points.Count;
        center.y /= Points.Count;
        center.z /= Points.Count;

        Vector3 averageNormal = Vector3.zero;

        for (int i = 0; i < Points.Count; i++)
        {
            Vector3 current = Points[i];
            Vector3 next = Points[(i + 1) % Points.Count];

            Vector3 directionFromCenterToCurrent = current - center;
            Vector3 directionFromCenterToNext = next - center;

            Vector3 planeNormal = Vector3.Cross(
                directionFromCenterToCurrent,
                directionFromCenterToNext
            );

            if (planeNormal != Vector3.zero)
            {
                averageNormal += planeNormal.normalized;
            }
        }
        return averageNormal;*/
    }

    public Vector3 GetNormal2D(Vector3 PointA, Vector3 PointB)
    {
        Vector3 Direction = PointB - PointA;

        Vector3 side = Vector3.Cross(Vector3.up, Direction);
        side.Normalize();
        Vector3 normal = Vector3.Cross(Direction,side);
        normal.Normalize();
        return normal;
    }
    List<Vector3> OrderPoints(List<Vector3> points)
    {
        Vector3 center = Vector3.zero;

        foreach (var p in points)
            center += p;

        center /= points.Count;

        points.Sort((b, a) =>
        {
            Vector2 da = new Vector2(a.x - center.x, a.z - center.z);
            Vector2 db = new Vector2(b.x - center.x, b.z - center.z);

            float angleA = Mathf.Atan2(da.y, da.x);
            float angleB = Mathf.Atan2(db.y, db.x);

            return angleA.CompareTo(angleB);
        });

        return points;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        //if (!ShowDebug) return;
        if (!this.enabled) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(PredictedBodyPos, Vector3.one * .6f);
    }
}
