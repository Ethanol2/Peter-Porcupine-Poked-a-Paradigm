using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class menuScript : MonoBehaviour
{
    public GameObject creditScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void startButton()
    {
        SceneManager.LoadScene("_Level One");
    }

    public static void exitButton()
    {
        Application.Quit();
    }

    public void creditsPopup()
    {
        creditScreen.SetActive(true);
    }
    public void creditsDropdown()
    {
        creditScreen.SetActive(false);
    }
    
    public void backToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
