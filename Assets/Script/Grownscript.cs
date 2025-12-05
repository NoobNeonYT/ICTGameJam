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



public class Grownscript : MonoBehaviour
{

    [SerializeField]
    public int Love = 0;    //ค่ารัก
    public int SumLove = 0;             //รวมค่ารัก

    [SerializeField]
    public int Grown = 0;           //ค่าการเจริญเติบโต
    public int SumGrown = 0;                    //รวมค่าการเจริญเติบโต

    public float timeremain = 60f;    //ตัวแปรเวลาที่เหลือ

    [SerializeField]
    public TextMeshProUGUI timerText; //ข้อความแสดงเวลา

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

}