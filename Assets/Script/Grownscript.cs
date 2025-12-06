using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class FoodItem
{
    public string Name;
    public int LoveValue = 0;
    public int GrownValue = 0;
    public Sprite FoodSprite;
}

[System.Serializable]
public class CharacterItem
{
    public string Name;
    public Sprite CharacterSprite;
    public int RequiredGrown;
}

public class Grownscript : MonoBehaviour
{
    public bool isfinalstage = false;

    [Header("Character System")]
    [SerializeField]
    public List<CharacterItem> CharacterList;

    [SerializeField]
    public Image CharacterDisplayImage;

    private int currentCharacterIndex = 0;

    [Header("Stats")]
    [SerializeField]
    public int Love = 0;
    public int SumLove = 0;

    [SerializeField]
    public int Grown = 0;
    public int SumGrown = 0;

    [Header("Timer System")]
    public float timeremain = 60f;
    [SerializeField]
    public TextMeshProUGUI timerText;
    public bool timerRunning = false;

    [Header("Food System")]
    [SerializeField]
    public List<FoodItem> AllAvailableFoods;

    private FoodItem[] CurrentRandomFoods = new FoodItem[3];

    [SerializeField]
    public Image[] FoodButtonImages = new Image[3];

    [Header("Effect System")]
    [SerializeField]
    private Image targetImage;
    [SerializeField] private float effectDuration = 1.6f;
    [SerializeField] private float scaleFactor = 2f;
    [SerializeField] private Color effectColor = Color.black;
    [SerializeField] private Color finalColor = Color.white;

    private Color originalColor;
    private Vector3 originalScale;

    void Start()
    {
        timerRunning = true;

        SetupFoodButtons();
        UpdateCharacterDisplay(currentCharacterIndex);

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null)
        {
            originalColor = targetImage.color;
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
                timerText.text = Mathf.Round(timeremain).ToString();
            }
            else
            {
                timeremain = 0;
                timerRunning = false;
                timerText.text = "0";
            }
        }
    }

    public void OnFoodButton1Click(int buttonIndex)
    {
        if (isfinalstage) return;

        if (buttonIndex < 0 || buttonIndex >= CurrentRandomFoods.Length)
        {
            Debug.LogError("Invalid button index: " + buttonIndex);
            return;
        }

        FoodItem selectedFood = CurrentRandomFoods[buttonIndex];

        // *** แก้ไข 1: ส่ง selectedFood.Name เข้าไปด้วย ***
        FeedPet(selectedFood.LoveValue, selectedFood.GrownValue, selectedFood.Name);

        SetupFoodButtons();
    }
    public void OnFoodButton2Click(int buttonIndex)
    {
        if (isfinalstage) return;

        if (buttonIndex < 0 || buttonIndex >= CurrentRandomFoods.Length)
        {
            Debug.LogError("Invalid button index: " + buttonIndex);
            return;
        }

        FoodItem selectedFood = CurrentRandomFoods[buttonIndex];

        // *** แก้ไข 1: ส่ง selectedFood.Name เข้าไปด้วย ***
        FeedPet(selectedFood.LoveValue, selectedFood.GrownValue, selectedFood.Name);

        SetupFoodButtons();
    }
    public void OnFoodButton3Click(int buttonIndex)
    {
        if (isfinalstage) return;

        if (buttonIndex < 0 || buttonIndex >= CurrentRandomFoods.Length)
        {
            Debug.LogError("Invalid button index: " + buttonIndex);
            return;
        }

        FoodItem selectedFood = CurrentRandomFoods[buttonIndex];

        // *** แก้ไข 1: ส่ง selectedFood.Name เข้าไปด้วย ***
        FeedPet(selectedFood.LoveValue, selectedFood.GrownValue, selectedFood.Name);

        SetupFoodButtons();
    }


    // *** แก้ไข 2: เพิ่ม parameter 'string currentFoodName' เพื่อรับชื่ออาหาร ***
    public void FeedPet(int loveAmount, int grownAmount, string currentFoodName)
    {
        Love += loveAmount;
        SumLove += loveAmount;
        Grown += grownAmount;
        SumGrown += grownAmount;

        // *** แก้ไข 3: แสดงชื่ออาหารใน Debug Log ***
        Debug.Log($"Pet Fed: Food={currentFoodName}, Love={Love} (Added {loveAmount}), Grown={Grown} (Added {grownAmount})");

        if (Grown >= CharacterList[currentCharacterIndex].RequiredGrown)
        {
            CheckEvolution();
            PlayPopEffect();
        }
    }

    public void SetupFoodButtons()
    {
        if (AllAvailableFoods.Count < 3 || FoodButtonImages.Length < 3)
        {
            Debug.LogError("Need at least 3 foods and 3 button images!");
            return;
        }

        FoodItem[] randomSelection = AllAvailableFoods
            .OrderBy(x => Random.value)
            .Take(3)
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
            case 0:
                if (Love >= 0) nextIndex = 2;
                else nextIndex = 1;
                break;

            case 1:
                if (Love >= 0) nextIndex = 4;
                else nextIndex = 3;
                break;

            case 2:
                if (Love >= 0) nextIndex = 6;
                else nextIndex = 5;
                break;

            case 3:
                if (Love >= 0) nextIndex = 8;
                else nextIndex = 7;
                break;

            case 4:
                if (Love >= 0) nextIndex = 10;
                else nextIndex = 9;
                break;

            case 5:
                if (Love >= 0) nextIndex = 12;
                else nextIndex = 11;
                break;

            case 6:
                if (Love >= 0) nextIndex = 14;
                else nextIndex = 13;
                break;

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

    public void PlayPopEffect()
    {
        if (targetImage == null)
        {
            Debug.LogError("Target Image is not set!");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(PopEffectRoutine());
    }

    private IEnumerator PopEffectRoutine()
    {
        float timer = 0f;

        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float t = timer / effectDuration;

            targetImage.color = Color.Lerp(effectColor, originalColor, t);

            float scaleT = Mathf.PingPong(t * 2.0f, 1.0f);
            transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleFactor, scaleT);

            yield return null;
        }

        targetImage.color = originalColor;
        transform.localScale = originalScale;
    }
}