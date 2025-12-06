using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class EndingData
{
    public string Name;
    public Sprite CharacterSprite;

    [TextArea(3, 10)]
    public string Dialogue;
}

public class EndingResultScript : MonoBehaviour
{
    [Header("UI Components")]
    public Image DisplayImage;
    public TextMeshProUGUI DialogueText;

    [Header("First Stage Random Config")]
    [Tooltip("ใส่รูป 3 แบบของร่างแรกที่นี่ (ให้เหมือนกับใน Grownscript)")]
    public List<Sprite> FirstStageVariants; // *** เพิ่มตรงนี้: ใส่รูป 3 แบบของร่างแรก

    [Header("Ending Configuration")]
    public List<EndingData> EndingList;

    void Start()
    {
        ShowEndingResult();
    }

    void ShowEndingResult()
    {
        // 1. รับค่า Index หลัก
        int finalIndex = PlayerPrefs.GetInt("SavedCharacterIndex", 0);

        // 2. รับค่า Variant Index (ตัวเลขที่สุ่มได้)
        int variantIndex = PlayerPrefs.GetInt("SavedVariantIndex", 0);

        Debug.Log($"Game Ended. Index: {finalIndex}, Variant: {variantIndex}");

        if (EndingList != null && finalIndex >= 0 && finalIndex < EndingList.Count)
        {
            EndingData data = EndingList[finalIndex];

            // --- ส่วนจัดการรูปภาพ ---
            if (DisplayImage != null)
            {
                // *** เช็คพิเศษ: ถ้าเป็นร่างแรก (Index 0) ***
                if (finalIndex == 0 && FirstStageVariants != null && FirstStageVariants.Count > 0)
                {
                    // ให้ใช้รูปจาก List ที่เราใส่ไว้ ตามเลขที่สุ่มได้ (variantIndex)
                    if (variantIndex < FirstStageVariants.Count)
                    {
                        DisplayImage.sprite = FirstStageVariants[variantIndex];
                    }
                    else
                    {
                        DisplayImage.sprite = data.CharacterSprite; // กันพลาด
                    }
                }
                else
                {
                    // ถ้าร่างอื่น ใช้รูปตามปกติ
                    DisplayImage.sprite = data.CharacterSprite;
                }

                DisplayImage.preserveAspect = true;
            }

            // --- แสดงบทพูด ---
            if (DialogueText != null)
            {
                DialogueText.text = data.Dialogue;
            }
        }
        else
        {
            Debug.LogError($"หาข้อมูลของ Index {finalIndex} ไม่เจอ!");
            if (EndingList.Count > 0)
            {
                DisplayImage.sprite = EndingList[0].CharacterSprite;
                DialogueText.text = EndingList[0].Dialogue;
            }
        }
    }

    public void RestartGame()
    {
        PlayerPrefs.DeleteKey("SavedCharacterIndex");
        PlayerPrefs.DeleteKey("SavedVariantIndex"); // ลบค่าสุ่มด้วย
        PlayerPrefs.DeleteKey("SavedLove");
        PlayerPrefs.DeleteKey("SavedGrown");

        SceneManager.LoadScene("SampleScene");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}