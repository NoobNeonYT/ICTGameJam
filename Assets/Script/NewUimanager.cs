using UnityEngine;


public class SceneChanger : MonoBehaviour
{
    // ฟังก์ชันสำหรับเปลี่ยน Scene
    public void LoadNewScene(string sceneName)
    {
        // โหลด Scene ใหม่ด้วย 'ชื่อ' ที่กำหนด
        AkSoundEngine.StopAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void QuitApplication()
    {
        // ออกจากแอปพลิเคชัน
        Application.Quit();
    }

}