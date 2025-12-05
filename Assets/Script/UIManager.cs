using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject inGameCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowMenu();
    }
    public void ShowMenu() 
    { 
        menuCanvas.SetActive(true); 
        inGameCanvas.SetActive(false); 
    }
    public void InGame() 
    { 
        menuCanvas.SetActive(false); 
        inGameCanvas.SetActive(true); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
