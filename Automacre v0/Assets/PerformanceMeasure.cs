using UnityEngine;

public class PerformanceMeasure : MonoBehaviour
{
    float deltaTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;


    }
    void OnGUI()
    {
        int fps = (int)(1.0f / deltaTime);
        GUI.Label(new Rect(10, 10, 200, 20), "FPS: " + fps);
    }
}
