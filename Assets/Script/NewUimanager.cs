using UnityEngine;


public class SceneChanger : MonoBehaviour
{
    // ฟังก์ชันสำหรับเปลี่ยน Scene
   
    
    public void QuitApplication()
    {
        // ออกจากแอปพลิเคชัน
        Application.Quit();
    }


    public static bool isCutscenePlayed = false;

    public void Nextscene ()
    {
        if (!isCutscenePlayed)
        {
            AkSoundEngine.StopAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene("cutscene");
            isCutscenePlayed = true;
        }
        else
        {
            AkSoundEngine.StopAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }
}