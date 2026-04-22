using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float FadeAmount = 0;
    public float FadeLength = 1;
    public int Direction = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        FadeAmount += (Time.deltaTime / FadeLength) * (float)(Direction);
        FadeAmount = Mathf.Clamp01(FadeAmount);
        Color color = GetComponent<Image>().color;
        color.a = FadeAmount;
        GetComponent<Image>().color = color;
    }

    public void SetDirection(int direction, Color color)
    {
        this.Direction = direction;
        GetComponent<Image>().color= color;

        if (direction < 0)FadeAmount = 1;
        if (direction > 0)FadeAmount = 0;
    }
}
