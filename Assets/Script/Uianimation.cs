using UnityEngine;
using System.Collections;
using UnityEngine.UI; // ต้องมีบรรทัดนี้เพื่อใช้ Image Component


using System.Collections.Generic;

[System.Serializable]
public class ImageGroup
{
    // 1. ช่องแสดงผล (Image Component) สำหรับกลุ่มนี้
    public Image targetImage;

    // 2. รายการ Sprite สำหรับช่องนี้โดยเฉพาะ
    public List<Sprite> availableSprites;
}

public class Uianimation : MonoBehaviour
{
    // List ที่เก็บทั้งช่อง UI และ Sprite ของช่องนั้น (ต้องกำหนดค่า Size ใน Inspector)
    [SerializeField]
    private List<ImageGroup> imageGroups = new List<ImageGroup>();

    // ตัวแปรสำหรับกำหนดช่วงเวลาการสุ่ม
    private float randomizeInterval = 0.25f;

    // *** ตัวแปรที่ไม่เกี่ยวข้องกับการสุ่มภาพ UI ถูกลบออกไปทั้งหมดแล้ว ***
    // (targetImage, CharacterList, ShadowDisplayImage, currentshadow ถูกลบออกจาก Script นี้)

    void Start()
    {
        // ตรวจสอบว่ามีการกำหนดค่าหรือไม่ และมีข้อมูลอย่างน้อยหนึ่งกลุ่ม
        if (imageGroups != null && imageGroups.Count > 0)
        {
            StartCoroutine(RandomizeImageRoutine());
        }
        else
        {
            // เปลี่ยนเป็น LogWarning เพื่อให้เกมรันต่อได้ (แต่แจ้งให้ทราบว่าไม่ได้ตั้งค่า)
            Debug.LogWarning("Uianimation: Image Groups ว่างเปล่า! ไม่มีการสุ่มภาพ UI");
        }
    }


    private IEnumerator RandomizeImageRoutine()
    {
        while (true)
        {
            // วนลูปทำงานกับทุกกลุ่มที่กำหนดไว้
            foreach (ImageGroup group in imageGroups)
            {
                // ตรวจสอบเงื่อนไขอย่างเข้มงวด: Target Image ต้องมี, Sprite List ต้องมีรูปอย่างน้อย 1 รูป
                if (group.targetImage != null &&
                    group.availableSprites != null &&
                    group.availableSprites.Count > 0)
                {
                    // 1. สุ่ม Index จาก Sprite List ของกลุ่มนี้
                    int randomIndex = Random.Range(0, group.availableSprites.Count);

                    // 2. เปลี่ยน Sprite
                    group.targetImage.sprite = group.availableSprites[randomIndex];
                }
            }

            // 3. รอตามช่วงเวลาที่กำหนด
            yield return new WaitForSeconds(randomizeInterval);
        }
    }
}