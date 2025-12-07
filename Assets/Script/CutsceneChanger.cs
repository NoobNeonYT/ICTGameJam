using System.Collections; // ต้องมีบรรทัดนี้เพื่อใช้ Coroutine และ WaitForSeconds
using UnityEngine;

public class DelayExample : MonoBehaviour
{
    public float delayTime = 2.0f; // กำหนดเวลาที่ต้องการรอ (2 วินาที)

    void Start()
    {
        // ต้องเรียกใช้ Coroutine ด้วย StartCoroutine()
        StartCoroutine(StartDelayedAction());
    }
    private void Awake()
    {
        AkSoundEngine.LoadBank("CutscenceSoundBank", out uint cutscencemusicbankID);
        AkSoundEngine.LoadBank("MusicMenuBank", out uint musicbankID);


        AkSoundEngine.PostEvent("CutscenceSound", gameObject);
        AkSoundEngine.PostEvent("MusicMenu", gameObject);
    }

    // ฟังก์ชันที่ใช้ในการรอ ต้องเป็น IEnumerator
    private IEnumerator StartDelayedAction()
    {
       

       
        yield return new WaitForSeconds(delayTime);



        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");



    }
}