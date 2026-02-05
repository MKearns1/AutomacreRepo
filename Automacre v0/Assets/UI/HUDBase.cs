using TMPro;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UI;

public class HUDBase : MonoBehaviour
{
    public GameObject Task;
    TextMeshProUGUI HoverText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // transform.Find("TaskManager").Find("ScrollArea").Find("Content").GetComponent<HorizontalLayoutGroup>()
       HoverText = transform.Find("HoverObjectTip").Find("TipText").GetComponent<TextMeshProUGUI>();
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*    public void AddNewTask(Direction direction)
    {
        GameObject newTask = GameObject.Instantiate(Task);
        newTask.transform.SetParent(
        transform.Find("TaskManager").Find("ScrollArea").Find("Content"), false);
    }*/

    public void Hover(IClickable ClickableObj)
    {
        if (ClickableObj != null)
        {
            string Text = ClickableObj.GetHoverText();
            HoverText.text = Text;
        }
        else
        {
            HoverText.text = "";
        }

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            Input.mousePosition,
            null,  // pass null if it's Screen Space - Overlay
            out pos
        );

        transform.Find("HoverObjectTip").GetComponent<RectTransform>().anchoredPosition = pos;

    }
}
