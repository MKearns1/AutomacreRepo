using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    RectTransform cursor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cursor = transform.Find("Cursor").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").transform as RectTransform, Input.mousePosition, null, out pos);

        cursor.anchoredPosition = pos;
    }
}
