using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject inGameCanvas;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private string volumeParameter = "MasterVolume";

    private void Start()
    {
        ShowMenu();

        // โหลดค่า Volume จาก Mixer → ตั้งค่า Slider
        if (audioMixer.GetFloat(volumeParameter, out float value))
        {
            volumeSlider.value = Mathf.Pow(10, value / 20);  // Convert dB → linear 0–1
        }

        // ฟัง event เวลาเลื่อน Slider
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // -------------------------
    //     UI Page Switching
    // -------------------------
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

    // -------------------------
    //     START GAME
    // -------------------------
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // -------------------------
    //     QUIT GAME
    // -------------------------
    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }

    // -------------------------
    //     VOLUME CONTROL
    // -------------------------
    private void SetVolume(float value)
    {
        // Linear 0–1 → dB
        float dB = Mathf.Log10(value) * 20;
        audioMixer.SetFloat(volumeParameter, dB);
    }
}
