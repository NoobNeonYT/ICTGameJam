using System.Linq; // สำหรับการสุ่มที่ไม่ซ้ำ (.OrderBy().Take())
using System.Collections.Generic; // สำหรับการใช้ List<FoodItem>
using TMPro;
using UnityEngine;
using UnityEngine.UI; // ต้องเพิ่มเพื่อใช้ Image Component



[System.Serializable]
public class FoodItem
{
    public string Name;
    public int LoveValue;
    public int GrownValue;
    public Sprite FoodSprite; // รูปภาพของอาหาร
}
[System.Serializable]
public class CharacterItem
{
    public string Name; // ชื่อตัวละคร หรือ ขั้นพัฒนาการ
    public Sprite CharacterSprite; // รูปภาพของตัวละครขั้นนี้
    public int RequiredGrown; // ค่า Grown ที่ต้องมีเพื่อเปลี่ยนร่าง

   
}

public class Grownscript : MonoBehaviour
{

    public bool isfinalstage = false;

    [Header("Character System")]
    [SerializeField]
    public List<CharacterItem> CharacterList;

    [SerializeField]
    public Image CharacterDisplayImage;

    // *** ตัวแปรใหม่: Index ของตัวละครที่กำลังแสดงผล (เริ่มต้นที่ 1 ตามผัง) ***
    private int currentCharacterIndex = 0;

    [SerializeField]
    public int Love = 0;                //ค่ารัก
    public int SumLove = 0;             //รวมค่ารัก

    [SerializeField]
    public int Grown = 0;                //ค่าการเจริญเติบโต
    public int SumGrown = 0;                    //รวมค่าการเจริญเติบโต

    public float timeremain = 60f;         //ตัวแปรเวลาที่เหลือ

    [SerializeField]
    public TextMeshProUGUI timerText;      //ข้อความแสดงเวลา

    public bool timerRunning = false;


    
    [Header("Food System")]
    [SerializeField]
    
    public List<FoodItem> AllAvailableFoods;

  
    private FoodItem[] CurrentRandomFoods = new FoodItem[3];

    [SerializeField]
    
    public Image[] FoodButtonImages = new Image[3];


   


    void Start()
    {
        timerRunning = true;
       
        SetupFoodButtons();

        UpdateCharacterDisplay(currentCharacterIndex);
    }

    void Update()
    {
        if (timerRunning)
        {
            if (timeremain > 0)
            {
                timeremain -= Time.deltaTime;
                timerText.text = "Time Left: " + Mathf.Round(timeremain).ToString() + "s";
            }
            else
            {
                timeremain = 0;
                timerRunning = false;
                timerText.text = "Time's Up!";
            }
        }
    }


    public void OnFoodButtonClick(int buttonIndex)
    {
        
        if (buttonIndex < 0 || buttonIndex >= CurrentRandomFoods.Length)
        {
            Debug.LogError("Invalid button index: " + buttonIndex);
            return;
        }

       
        FoodItem selectedFood = CurrentRandomFoods[buttonIndex];

       
        FeedPet(selectedFood.LoveValue, selectedFood.GrownValue);

      
        SetupFoodButtons();
    }


    public void FeedPet(int loveAmount, int grownAmount)
    {
        Love += loveAmount;
        SumLove += loveAmount;
        Grown += grownAmount;
        SumGrown += grownAmount;

        Debug.Log($"Pet Fed with {Love} Love and {Grown} Grown.");

        if (Grown >= CharacterList[currentCharacterIndex].RequiredGrown)
        {
            CheckEvolution();
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

        
        FoodItem[] randomSelection = AllAvailableFoods
            .OrderBy(x => Random.value) // สุ่มลำดับรายการทั้งหมด
            .Take(3) // เลือกมาแค่ 3 อันดับแรก
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
        
        // ตรวจสอบว่า Index ที่ส่งมาอยู่ในขอบเขตของ CharacterList หรือไม่
        if (CharacterList.Count > index && index >= 0)
        {
            if (CharacterDisplayImage != null)
            {
                // นำ Sprite จาก List มาแสดงผลใน UI Image
                CharacterDisplayImage.sprite = CharacterList[index].CharacterSprite;
                Debug.Log("Displayed character: " + CharacterList[index].Name);
               
            }
        }
        else
        {
            Debug.LogError("Character Index " + index + " is out of bounds or list is empty.");
        }
    }
    private void ResetStats()
    {
        Love = 0;
        Grown = 0;
        // ไม่จำเป็นต้องรีเซ็ต SumLove/SumGrown ถ้าต้องการเก็บค่ารวม
        Debug.Log("Stats Reset: Love=0, Grown=0");
    }

    // *** ฟังก์ชันใหม่: ตรวจสอบการวิวัฒนาการตามผัง ***
    public void CheckEvolution()
    {
        if (currentCharacterIndex == 0)                                      //ร่าง1stage1
        {
            if (Love >= 0 )
                {
                currentCharacterIndex = 2;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
            }
            else if (Love < 0 )
            {
                currentCharacterIndex = 1;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
            }
        }
        else if (currentCharacterIndex == 1)                          //ร่าง2st-2
        {
            if (Love >= 0)
            {
                currentCharacterIndex = 4; 
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
            }
            else if (Love < 0)
            {
                currentCharacterIndex = 3;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
            }
        }
        else if (currentCharacterIndex == 2)                               //ร่าง3st-2
        {
            if (Love >= 0)
            {
                currentCharacterIndex = 6;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
            }
            else if (Love < 0)
            {
                currentCharacterIndex = 5;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
            }
        }
        else if (currentCharacterIndex == 3)                               //ร่าง4st-3
        {
            if (Love >= 0)
            {
                currentCharacterIndex = 8;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
                isfinalstage = true;
            }
            else if (Love < 0)
            {
                currentCharacterIndex = 7;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
                isfinalstage = true;
            }
        }

        else if (currentCharacterIndex == 4)                               //ร่าง5st-3
        {
            if (Love >= 0)
            {
                currentCharacterIndex = 10;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
                isfinalstage = true;
            }
            else if (Love < 0)
            {
                currentCharacterIndex = 9;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
                isfinalstage = true;
            }
        }
        else if (currentCharacterIndex == 5)                               //ร่าง6st-3
        {
            if (Love >= 0)
            {
                currentCharacterIndex = 12;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
                isfinalstage = true;
            }
            else if (Love < 0)
            {
                currentCharacterIndex = 11;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
                isfinalstage = true;
            }
        }
        else if (currentCharacterIndex == 6)                               //ร่าง6st-3
        {
            if (Love >= 0)
            {
                currentCharacterIndex = 14;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
                isfinalstage = true;
            }
            else if (Love < 0)
            {
                currentCharacterIndex = 13;
                UpdateCharacterDisplay(currentCharacterIndex);
                ResetStats();
                isfinalstage = true;
            }
        }

    }

}
    
