using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // *** ต้องเพิ่มบรรทัดนี้ เพื่อให้รู้จักปุ่ม (Button) ***

public class EndingButtons : MonoBehaviour
{
    [Header("Drag Buttons Here (ลากปุ่มมาใส่ตรงนี้)")]
    public Button MainMenuButton; // ช่องใส่ปุ่มกลับเมนู
    public Button RestartButton;  // ช่องใส่ปุ่มเล่นใหม่

    void Start()
    {
        // สั่งให้โค้ดทำงานเมื่อกดปุ่ม (AddListener)
        if (MainMenuButton != null)
        {
            MainMenuButton.onClick.AddListener(OnMainMenuClick);
        }

        if (RestartButton != null)
        {
            RestartButton.onClick.AddListener(OnRestartClick);
        }
    }

    // ฟังก์ชันกลับเมนู
    void OnMainMenuClick()
    {
        PlayerPrefs.Save(); // บันทึกข้อมูล
        Debug.Log("Go to Main Menu");
        SceneManager.LoadScene("MainMenu"); // อย่าลืมเปลี่ยนชื่อ Scene ให้ตรง
    }

    // ฟังก์ชันเล่นใหม่
    void OnRestartClick()
    {
        // ล้างค่าเก่า
        PlayerPrefs.DeleteKey("SavedCharacterIndex");
        PlayerPrefs.DeleteKey("SavedVariantIndex");
        PlayerPrefs.DeleteKey("SavedLove");
        PlayerPrefs.DeleteKey("SavedGrown");

        Debug.Log("Restart Game");
        SceneManager.LoadScene("SampleScene"); // อย่าลืมเปลี่ยนชื่อ Scene ให้ตรง
    }
}