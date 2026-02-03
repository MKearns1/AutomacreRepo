using UnityEngine;

public class Eye : MonoBehaviour
{
    [Range(1, 3)] public float LookInterval = 1f;
    public float LookTimer;
    [Range(1, 3)] public float BlinkInterval = 1f;
    public float BlinkTimer;
    public float BlinkLength;
    public float BlinkProgress;
    public int numOfBlinks = 1;
    float BlinkSpeed = 1;

    Quaternion startLookRot;
    Quaternion targetLookRot;

    [SerializeField] float coneAngle;
    [SerializeField][Range(6, 14)] float lookSpeed;

    Vector3 startForward;

    public AnimationCurve curve;
    Vector3 DefaultEyeSize;

    void Start()
    {
        Vector3 dir = RandomDirectionInCone(transform.forward, coneAngle);
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        startLookRot = transform.localRotation;
        targetLookRot = transform.localRotation;
        startForward = transform.forward;

        LookTimer = 0f;
        LookInterval = Random.Range(1, 5f);
        lookSpeed = Random.Range(0.5f, 2f);
        DefaultEyeSize = transform.Find("Pupil").transform.localScale;
    }

    void Update()
    {
        LookTimer += Time.deltaTime;

        float t = Mathf.Clamp01(LookTimer / LookInterval);
        t = curve.Evaluate(t);
        transform.localRotation = Quaternion.Slerp(startLookRot, targetLookRot, t * lookSpeed);

        if (LookTimer >= LookInterval)
        {
            LookTimer = 0f;
            LookInterval = Random.Range(1,3f);
            lookSpeed = Random.Range(6f,14f);

            startLookRot = transform.localRotation;

            Vector3 newDir = RandomDirectionInCone(startForward, coneAngle);
            targetLookRot = Quaternion.LookRotation(newDir, Vector3.up);
        }
       // Debug.Log(Time.time);
        BlinkTimer += Time.deltaTime;

        if (BlinkTimer >= BlinkInterval || false)
        {
            BlinkProgress -= Time.deltaTime*BlinkSpeed;

            float pingpong = Mathf.PingPong(BlinkProgress, DefaultEyeSize.y);

            Vector3 EyeScale = DefaultEyeSize;

            EyeScale.y = pingpong;

            transform.Find("Pupil").transform.localScale = EyeScale;

            if (BlinkProgress <= -DefaultEyeSize.y * numOfBlinks)
            {
                BlinkTimer = 0;
                //BlinkInterval = Random.Range(1,4);
                BlinkInterval = 2;
                BlinkProgress = DefaultEyeSize.y;
                numOfBlinks = randomOddNum(1,5);
                //numOfBlinks = 3;
                //BlinkSpeed = 2/(float)numOfBlinks;
                BlinkSpeed = Random.Range(2,3);
                transform.Find("Pupil").transform.localScale = DefaultEyeSize;
                Debug.LogWarning("BLINK");

            }

        }

        
    }

    Vector3 RandomDirectionInCone(Vector3 forward, float angleDegrees)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;

        // Uniform spherical cap sampling
        float z = Mathf.Cos(angleRad) + UnityEngine.Random.value * (1 - Mathf.Cos(angleRad));
        float theta = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

        float x = Mathf.Sqrt(1 - z * z) * Mathf.Cos(theta);
        float y = Mathf.Sqrt(1 - z * z) * Mathf.Sin(theta);

        Vector3 localDir = new Vector3(x, y, z);

        return localDir.normalized;
        return Quaternion.LookRotation(forward) * localDir;
    }

    int randomOddNum(int min, int max)
    {
        int result = Random.Range(min, max+1);
        while (result % 2 == 0) { result = Random.Range(min, max + 1); }
        return result;
    }
}
