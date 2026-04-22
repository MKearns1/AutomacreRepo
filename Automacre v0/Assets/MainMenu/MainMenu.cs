using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public RectTransform ControlsPanel;
    public RectTransform CreditsPanel;
    public GameObject Fade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Workshop1");
    }

    public void LoadScene(string sceneName)
    {
        Instantiate(Fade, transform).GetComponent<Fade>().SetDirection(1,Color.black);
        Invoke("LoadGame", 2.5f);
    }

    public void quit()
    {
       Application.Quit();  
    }

    public void SetPanel(string Name)
    {
        switch (Name)
        {
            case "Controls":
                ControlsPanel.gameObject.SetActive(!ControlsPanel.gameObject.activeInHierarchy);
                CreditsPanel.gameObject.SetActive(false);
                break;
            case "Credits":
                CreditsPanel.gameObject.SetActive(!CreditsPanel.gameObject.activeInHierarchy);
                ControlsPanel.gameObject.SetActive(false);
                break;
        }
    }
}
