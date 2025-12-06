using System.Collections.Generic; // สำหรับการใช้ List
using System.Linq; // สำหรับการสุ่มที่ไม่ซ้ำ (.OrderBy().Take())
using TMPro;
using UnityEngine;
using UnityEngine.UI; // ต้องเพิ่มเพื่อใช้ Image Component
using System.Collections;

[System.Serializable]
public class FoodItem
{
    public string Name;
    public int LoveValue = 0;   // อาหารดี Love เป็น +, อาหารแย่ Love เป็น -
    public int GrownValue = 0;
    public Sprite FoodSprite; // รูปภาพของอาหาร
}

[System.Serializable]
public class CharacterItem
{
    public string Name; // ชื่อตัวละคร
    public Sprite CharacterSprite; // รูปภาพของตัวละครขั้นนี้
    public int RequiredGrown; // ค่า Grown ที่ต้องมีเพื่อเปลี่ยนร่าง
}

public class Grownscript : MonoBehaviour
{
    public bool isfinalstage = false;

    [Header("Character System")]
    [SerializeField]
    public List<CharacterItem> CharacterList; // ลิสต์เก็บข้อมูลตัวละครทุกร่าง

    [SerializeField]
    public Image CharacterDisplayImage; // ที่แสดงรูปตัวละคร

    // Index ของตัวละครที่กำลังแสดงผล (เริ่มต้นที่ 0 = ร่างแรก)
    private int currentCharacterIndex = 0;

    [Header("Stats")]
    [SerializeField]
    public int Love = 0;                // ค่ารัก (ใช้กำหนดเส้นทางวิวัฒนาการ)
    public int SumLove = 0;             // รวมค่ารักทั้งหมด (เก็บสถิติ)

    [SerializeField]
    public int Grown = 0;               // ค่าการเจริญเติบโต (ใช้กำหนดเวลาเปลี่ยนร่าง)
    public int SumGrown = 0;            // รวมค่าการเจริญเติบโต

    [Header("Timer System")]
    public float timeremain = 60f;      // เวลาที่เหลือ
    [SerializeField]
    public TextMeshProUGUI timerText;   // ข้อความแสดงเวลา
    public bool timerRunning = false;

    [Header("Food System")]
    [SerializeField]
    public List<FoodItem> AllAvailableFoods;

    // เก็บอาหาร 3 อย่างที่สุ่มมาปัจจุบัน
    private FoodItem[] CurrentRandomFoods = new FoodItem[3];

    [SerializeField]
    public Image[] FoodButtonImages = new Image[3]; // ปุ่มกดอาหาร 3 ปุ่ม

    void Start()
    {
        timerRunning = true;

        // เริ่มต้นระบบ
        SetupFoodButtons();
        UpdateCharacterDisplay(currentCharacterIndex);

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null)
        {
            // *** 1. เก็บสีเดิม (สีขาว/สีที่ต้องการให้เป็นสุดท้าย) ***
            originalColor = targetImage.color;

            // *** 2. กำหนดให้รูปเป็นสีดำตั้งแต่เริ่มต้น (ตามที่ผู้ใช้ต้องการ) ***
            targetImage.color = effectColor;
        }
        originalScale = transform.localScale;

        PlayPopEffect();
    }


        void Update()
        {
            if (timerRunning)
            {
                if (timeremain > 0)
                {
                    timeremain -= Time.deltaTime;
                    timerText.text = "" + Mathf.Round(timeremain).ToString() + "";
                }
                else
                {
                    timeremain = 0;
                    timerRunning = false;
                    timerText.text = "0";
                }
            }

        }
    

    // ฟังก์ชันเมื่อกดปุ่มอาหาร (ผูกกับปุ่มใน Unity)
    public void OnFoodButtonClick(int buttonIndex)
    {
        // ถ้าร่างสุดท้ายแล้ว ไม่ต้องทำอะไร (หรือจะให้กินได้แต่ไม่เปลี่ยนร่างก็ได้)
        if (isfinalstage) return;

        if (buttonIndex < 0 || buttonIndex >= CurrentRandomFoods.Length)
        {
            Debug.LogError("Invalid button index: " + buttonIndex);
            return;
        }

        // ดึงข้อมูลอาหารจากปุ่มที่กด
        FoodItem selectedFood = CurrentRandomFoods[buttonIndex];

        // ให้อาหาร
        FeedPet(selectedFood.LoveValue, selectedFood.GrownValue);

        // สุ่มอาหารชุดใหม่
        SetupFoodButtons();
    }

    public void FeedPet(int loveAmount, int grownAmount)
    {
        Love += loveAmount;
        SumLove += loveAmount;
        Grown += grownAmount;
        SumGrown += grownAmount;

        Debug.Log($"Pet Fed: Love={Love}, Grown={Grown}");

        // เช็คว่าโตพอจะเปลี่ยนร่างหรือยัง
        if (Grown >= CharacterList[currentCharacterIndex].RequiredGrown)
        {
            CheckEvolution();
            
                PlayPopEffect();
           
        }
    }

    public void SetupFoodButtons()
    {
        // ตรวจสอบความพร้อม
        if (AllAvailableFoods.Count < 3 || FoodButtonImages.Length < 3)
        {
            Debug.LogError("Need at least 3 foods and 3 button images!");
            return;
        }

        // สุ่มอาหาร 3 อย่างจากลิสต์ทั้งหมด
        FoodItem[] randomSelection = AllAvailableFoods
            .OrderBy(x => Random.value) // สุ่มลำดับ
            .Take(3) // เอามา 3 อัน
            .ToArray();

        for (int i = 0; i < 3; i++)
        {
            CurrentRandomFoods[i] = randomSelection[i];

            if (FoodButtonImages[i] != null)
            {
                FoodButtonImages[i].sprite = randomSelection[i].FoodSprite;
            }
        }
    }

    public void UpdateCharacterDisplay(int index)
    {
        // ตรวจสอบว่า Index อยู่ในขอบเขต List
        if (index >= 0 && index < CharacterList.Count)
        {
            if (CharacterDisplayImage != null)
            {
                CharacterDisplayImage.sprite = CharacterList[index].CharacterSprite;
                Debug.Log("Displayed character: " + CharacterList[index].Name);
            }
        }
        else
        {
            Debug.LogError("Character Index out of bounds: " + index);
        }
    }

    private void ResetStats()
    {
        Love = 0;
        Grown = 0;
        Debug.Log("Stats Reset for next evolution stage.");

    }

    public void CheckEvolution()
    {
        int nextIndex = -1;

        switch (currentCharacterIndex)
        {
            // =========================================================
            // RAGE 1 (Start)
            // =========================================================
            case 0:
                if (Love >= 0) nextIndex = 2; // ไปสายดี
                else nextIndex = 1;           // ไปสายร้าย
                
                break;

            // =========================================================
            // RAGE 2 (Evolution 1)
            // =========================================================
            case 1: // มาจากสายร้าย
                if (Love >= 0) nextIndex = 4; // กลับใจ
                else nextIndex = 3;           // ร้ายต่อ
                break;

            case 2: // มาจากสายดี
                if (Love >= 0) nextIndex = 6; // ดีต่อ
                else nextIndex = 5;           // เริ่มเสีย
                break;

            // =========================================================
            // RAGE 3 (Evolution 2) -> ตอนนี้จะวิวัฒนาการต่อได้แล้ว
            // =========================================================
            case 3: // (ร้าย->ดี)
                if (Love >= 0) nextIndex = 8;  // (ร้าย->ดี)->ดี
                else nextIndex = 7;            // (ร้าย->ดี)->ร้าย
                break;

            case 4: // (ร้าย->ร้าย)
                if (Love >= 0) nextIndex = 10;  // (ร้าย->ร้าย)->ดี
                else nextIndex = 9;           // (ร้าย->ร้าย)->ร้าย
                break;

            case 5: // (ดี->ร้าย)
                if (Love >= 0) nextIndex = 12; // (ดี->ร้าย)->ดี
                else nextIndex = 11;           // (ดี->ร้าย)->ร้าย
                break;

            case 6: // (ดี->ดี)
                if (Love >= 0) nextIndex = 14; // (ดี->ดี)->ดี
                else nextIndex = 13;           // (ดี->ดี)->ร้าย
                break;

            // =========================================================
            // RAGE 4 (Final Stage) -> ร่างสุดท้ายจริงๆ อยู่ตรงนี้
            // =========================================================
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                isfinalstage = true;
                Debug.Log("Max Evolution Reached (Stage 4)!");
                break;

            default:
                Debug.LogWarning("Index นี้ไม่อยู่ในเงื่อนไขการวิวัฒนาการ");
                break;
        }

        if (nextIndex != -1 && !isfinalstage)
        {
            currentCharacterIndex = nextIndex;
            UpdateCharacterDisplay(currentCharacterIndex);
            ResetStats(); 
        }
    }

    [SerializeField]
    private Image targetImage; // ลาก Image Component มาใส่ใน Inspector

    // การตั้งค่า Effect
    [SerializeField] private float effectDuration = 1.6f; // ระยะเวลาทั้งหมด (เร็วๆ)
    [SerializeField] private float scaleFactor = 2f;    // ขนาดที่ใหญ่ขึ้น (200%)
    [SerializeField] private Color effectColor = Color.black; // สีเริ่มต้น (สีดำ)
    [SerializeField] private Color finalColor = Color.white; // สีสุดท้ายที่ต้องการให้เฟดไป (สีขาว)

    private Color originalColor;
    private Vector3 originalScale;
    public void PlayPopEffect()
    {
        if (targetImage == null)
        {
            Debug.LogError("Target Image is not set!");
            return;
        }

        StopAllCoroutines();
        // ไม่ต้องเปลี่ยนสีเป็นดำอีก เพราะ Start() ตั้งค่าไว้แล้ว
        StartCoroutine(PopEffectRoutine());
    }

    private IEnumerator PopEffectRoutine()
    {
        float timer = 0f;

        // วนลูปไปตลอดระยะเวลา Effect
        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            // t คือ Progress รวม 0 ถึง 1 ตลอด effectDuration
            float t = timer / effectDuration;

            // ==========================================================
            // 1. Color Fade: เฟดจาก effectColor (ดำ) ไป originalColor (ขาว/สีเดิม)
            // ==========================================================
            // Color.Lerp จะเปลี่ยนสีไปเรื่อยๆ ตั้งแต่ t=0 จนถึง t=1
            targetImage.color = Color.Lerp(effectColor, originalColor, t);

            // ==========================================================
            // 2. Scale Pop: ขยายและหดกลับในเวลาเดียวกัน
            // ==========================================================
            // Mathf.PingPong(t * 2f, 1f) จะสร้างค่า 0 -> 1 -> 0 ตลอดระยะเวลา t=0 ถึง t=1
            float scaleT = Mathf.PingPong(t * 2.0f, 1.0f);

            // Scale: จากขนาดเดิม (t=0) ไปขนาดใหญ่สุด (t=0.5) แล้วกลับมาขนาดเดิม (t=1)
            transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleFactor, scaleT);

            yield return null;
        }

        // 3. ตั้งค่าสุดท้ายให้แม่นยำ
        targetImage.color = originalColor;
        transform.localScale = originalScale;
    }
}