using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCharacter : MonoBehaviour
{
    [Header("UI Component")]
    [Tooltip("ถ้าไม่ลากใส่ เดี๋ยวโค้ดจะหา Image ชื่อ 'LastCharacterImage' ให้เอง")]
    [SerializeField] private Image displayImage;

    [Header("Configuration")]
    [Tooltip("รูปทุกร่างเรียงตาม Index (0-14)")]
    [SerializeField] private List<Sprite> AllCharacterSprites;

    [Tooltip("รูป 3 แบบของร่างแรก (ถ้ามี)")]
    [SerializeField] private List<Sprite> FirstStageVariants;

    void Start()
    {
        // 1. ค้นหา Image อัตโนมัติ (เผื่อลืมลากใส่)
        if (displayImage == null)
        {
            GameObject imgObj = GameObject.Find("LastCharacterImage");
            if (imgObj != null) displayImage = imgObj.GetComponent<Image>();
        }

        // ถ้ายังหาไม่เจออีก ก็จบข่าว
        if (displayImage == null)
        {
            Debug.LogError("หา Image ไม่เจอ! สร้าง Image แล้วตั้งชื่อว่า 'LastCharacterImage' หรือลากใส่ใน Inspector");
            return;
        }

        ShowCharacter();
    }

    void ShowCharacter()
    {
        // 2. ดึงข้อมูล (ถ้าไม่มีเซฟ จะได้ค่า 0 คือร่างแรกสุดโดยอัตโนมัติ)
        int showIndex = PlayerPrefs.GetInt("SavedCharacterIndex", 0);
        int showVariant = PlayerPrefs.GetInt("SavedVariantIndex", 0); // ถ้าไม่มีเซฟ จะได้แบบที่ 0

        Debug.Log($"MainMenu Display: Index {showIndex}, Variant {showVariant}");

        // 3. บังคับเปิดการแสดงผล (แก้ปัญหาจอดำ/รูปหาย)
        displayImage.gameObject.SetActive(true);

        // แก้ค่า Scale ให้เป็น 1 (บางทีมันหดเหลือ 0)
        displayImage.rectTransform.localScale = Vector3.one;

        // แก้ค่าสีให้ชัดเจน (บางที Alpha เป็น 0)
        Color c = displayImage.color;
        c.a = 1f;
        displayImage.color = c;

        // 4. เลือกรูปมาใส่
        if (showIndex == 0)
        {
            // === กรณีร่างแรก (หรือเริ่มเกมใหม่) ===
            // ให้เช็คว่ามีรูป Variant ให้เลือกไหม
            if (FirstStageVariants != null && FirstStageVariants.Count > 0)
            {
                // ถ้าค่า Variant ที่เซฟมาถูกต้อง ก็ใช้รูปนั้น
                if (showVariant < FirstStageVariants.Count)
                {
                    displayImage.sprite = FirstStageVariants[showVariant];
                }
                else
                {
                    // กันพลาด: ใช้ตัวแรกสุด
                    displayImage.sprite = FirstStageVariants[0];
                }
            }
            else
            {
                // ถ้าไม่มี Variant ให้ไปใช้รูปรวมช่องที่ 0 แทน
                if (AllCharacterSprites.Count > 0)
                    displayImage.sprite = AllCharacterSprites[0];
            }
        }
        else
        {
            // === กรณีร่างอื่นๆ (Index 1 ขึ้นไป) ===
            if (AllCharacterSprites != null && showIndex < AllCharacterSprites.Count)
            {
                displayImage.sprite = AllCharacterSprites[showIndex];
            }
            else
            {
                // ถ้าหาไม่เจอจริงๆ ให้กลับไปโชว์ร่างแรกกันบั๊ก
                if (AllCharacterSprites.Count > 0)
                    displayImage.sprite = AllCharacterSprites[0];
            }
        }

        // จัดสัดส่วนภาพให้สวยงาม
        displayImage.preserveAspect = true;
    }

    // ฟังก์ชันสำหรับปุ่ม Reset (เผื่อใช้ test)
    public void ResetToDefault()
    {
        PlayerPrefs.DeleteAll();
        // โหลดหน้าใหม่เพื่อให้รูปเปลี่ยนกลับเป็นตัวแรก
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}