using UnityEngine;
using System.Collections;
using UnityEngine.UI; // ต้องมีบรรทัดนี้เพื่อใช้ Image Component


using System.Collections.Generic;

[System.Serializable]
public class ImageGroup
{
    // 1. ช่องแสดงผล (Image Component) สำหรับกลุ่มนี้
    public Image targetImage;

    // 2. รายการ Sprite สำหรับช่องนี้โดยเฉพาะ (จะไม่ปนกับช่องอื่น)
    public List<Sprite> availableSprites;
}

public class Uianimation : MonoBehaviour
{
    // *** เปลี่ยน Array/List เดิม เป็น List ของ ImageGroup ***
    [SerializeField]
    private List<ImageGroup> imageGroups; // List ที่เก็บทั้งช่อง UI และ Sprite ของช่องนั้น
    [SerializeField]
    private Image targetImage; // ตัวแปรสำหรับเก็บ Image Component ที่จะทำการขยาย

    // ตัวแปรสำหรับกำหนดช่วงเวลาการสุ่ม (โค้ดเดิม)

    private float randomizeInterval = 0.25f;
    [Header("Character shadow System")]
    [SerializeField]
    public List<CharacterItem> CharacterList; // ลิสต์เก็บข้อมูลตัวละครทุกร่าง

    [SerializeField]
    public Image ShadowDisplayImage; // ที่แสดงรูปตัวละคร

    // Index ของตัวละครที่กำลังแสดงผล (เริ่มต้นที่ 0 = ร่างแรก)
    private int currentshadow = 0;

    void Start()
    {
        // ตรวจสอบว่ามีการกำหนดค่าหรือไม่
        if (imageGroups != null && imageGroups.Count > 0)
        {
            StartCoroutine(RandomizeImageRoutine());
        }
        else
        {
            Debug.LogError("UIManager: Image Groups ยังไม่ได้กำหนดค่า!");
        }
    }


    private IEnumerator RandomizeImageRoutine()
    {
        while (true)
        {
            // 1. วนลูปทำงานกับทุกกลุ่ม (Group) ที่กำหนดไว้
            foreach (ImageGroup group in imageGroups)
            {
                // 2. ตรวจสอบว่ากลุ่มนี้มีช่อง UI และ Sprite ให้สุ่มหรือไม่
                if (group.targetImage != null && group.availableSprites != null && group.availableSprites.Count > 0)
                {
                    // 3. สุ่ม Index จาก Sprite List ของกลุ่มนี้โดยเฉพาะ
                    int randomIndex = Random.Range(0, group.availableSprites.Count);

                    // 4. เปลี่ยน Sprite ในช่องแสดงผลของกลุ่มนี้
                    group.targetImage.sprite = group.availableSprites[randomIndex];
                }
            }

            // 5. รอตามช่วงเวลาที่กำหนด (0.3 วินาที) ก่อนที่จะวนซ้ำ
            yield return new WaitForSeconds(randomizeInterval);
        }
    }
}