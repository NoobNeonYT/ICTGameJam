using UnityEngine;

public class MainmenuUi : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuUi;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Samplescene");
    }
}
