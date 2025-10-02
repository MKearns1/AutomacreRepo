using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class HUDBase : MonoBehaviour
{
    public GameObject Task;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // transform.Find("TaskManager").Find("ScrollArea").Find("Content").GetComponent<HorizontalLayoutGroup>()
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewTask(Direction direction)
    {
        GameObject newTask = GameObject.Instantiate(Task);
        newTask.transform.SetParent(
        transform.Find("TaskManager").Find("ScrollArea").Find("Content"), false);
    }
}
