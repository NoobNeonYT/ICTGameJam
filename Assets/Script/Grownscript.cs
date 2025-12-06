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
    public Image CharacterDisplayImage; // รูปหลักใน Scene

    // *** ส่วนที่แก้ไข: เพิ่มรายการรูปสำหรับสุ่มร่างแรก ***
    [Header("First Stage Random Pool (ใส่รูป 3 แบบของร่างแรกที่นี่)")]
    [SerializeField] public List<Sprite> FirstStageVariants;

    // ตัวแปรเก็บรูปร่างแรกที่สุ่มได้ (เพื่อไม่ให้มันเปลี่ยนไปเปลี่ยนมาระหว่างเล่น)
    private Sprite selectedFirstStageSprite;

    public static int currentCharacterIndex = 0;

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
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float scaleFactor = 2f;
    [SerializeField] private float scalePopFactor = 1.2f;
    [SerializeField] private Color effectColor = Color.black;
    [SerializeField] private Color finalColor = Color.white;

    private Color originalColor;
    private Vector3 originalScale;
    private Vector3 originalanimationscale;

    void Awake()
    {
        AkSoundEngine.LoadBank("MusicBank", out uint bankID);
        AkSoundEngine.LoadBank("UISoundBank", out uint uiBankID);
        AkSoundEngine.LoadBank("MusicMenuBank", out uint musicbankID);
        AkSoundEngine.LoadBank("CutscenceSoundBank", out uint scencebankID);
        AkSoundEngine.LoadBank("AllEatingSoundBank", out uint eatbankID);

        AkSoundEngine.PostEvent("MusicIngame", gameObject);
    }

    void Start()
    {
        timerRunning = true;

        // *** สุ่มรูปร่างแรก ตั้งแต่เริ่มเกมเพียงครั้งเดียว ***
        if (FirstStageVariants != null && FirstStageVariants.Count > 0)
        {
            int randomIndex = Random.Range(0, FirstStageVariants.Count);
            selectedFirstStageSprite = FirstStageVariants[randomIndex];
            Debug.Log("Random First Stage Selected Index: " + randomIndex);
        }

        SetupFoodButtons();

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        // อัปเดตรูปตัวละครเริ่มต้น
        UpdateCharacterDisplay(currentCharacterIndex);

        if (targetImage != null)
        {
            originalColor = targetImage.color;
            targetImage.color = effectColor;
        }
        originalScale = transform.localScale;
        originalanimationscale = targetImage.transform.localScale;

        PlayPopEffect();

        AkSoundEngine.LoadBank("UISoundBank", out uint bankID);
        AkSoundEngine.LoadBank("MusicMenuBank", out uint musicbankID);
        AkSoundEngine.LoadBank("CutscenceSoundBank", out uint scencebankID);
        AkSoundEngine.LoadBank("AllEatingSoundBank", out uint eatbankID);
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

    public void UpdateCharacterDisplay(int index)
    {
        if (index >= 0 && index < CharacterList.Count)
        {
            if (CharacterDisplayImage != null)
            {
                // *** ส่วนที่แก้ไข: ลอจิกการแสดงรูป ***

                if (index == 0) // ถ้าร่างแรก (Index 0)
                {
                    // ให้ใช้รูปที่สุ่มได้ (ถ้ามี)
                    if (selectedFirstStageSprite != null)
                    {
                        CharacterDisplayImage.sprite = selectedFirstStageSprite;
                    }
                    else
                    {
                        // ถ้าไม่ได้ใส่รูปสุ่มมา ให้ใช้รูป default ใน list เดิม
                        CharacterDisplayImage.sprite = CharacterList[index].CharacterSprite;
                    }
                }
                else // ถ้าร่างอื่นๆ (Index > 0)
                {
                    // ใช้รูปตามลำดับวิวัฒนาการปกติ
                    CharacterDisplayImage.sprite = CharacterList[index].CharacterSprite;
                }

                Debug.Log("Displayed character: " + CharacterList[index].Name);

                AkSoundEngine.PostEvent("Evolution", gameObject);

                if (index == 8 || index == 9 || index == 10 || index == 11)
                {
                    AkSoundEngine.ExecuteActionOnEvent(
                      "MusicIngame",
                        AkActionOnEventType.AkActionOnEventType_Stop,
                      gameObject,
                      0,
                        AkCurveInterpolation.AkCurveInterpolation_Linear
                     );

                    AkSoundEngine.PostEvent("BadEnding", gameObject);
                }
            }

            currentCharacterIndex = index;
            PlayCharacterVoice(currentCharacterIndex);
        }
        else
        {
            Debug.LogError("Character Index out of bounds: " + index);
        }
    }

    // ... (ส่วนที่เหลือ PlayCharacterVoice, OnFoodButtonClick, FeedPet, SetupFoodButtons, CheckEvolution, Effects เหมือนเดิม ไม่ต้องแก้) ...

    public void PlayCharacterVoice(int charIndex)
    {
        string newEventName = "Char_Voice_" + charIndex.ToString();
        AkSoundEngine.PostEvent(newEventName, gameObject);
        Debug.Log($"Wwise: Posting voice event for state: {newEventName}");
    }

    public void OnFoodButton1Click(int buttonIndex)
    {
        PlayScalePop();
        PlayCharacterVoice(currentCharacterIndex);
        if (isfinalstage) return;
        if (buttonIndex < 0 || buttonIndex >= CurrentRandomFoods.Length) return;
        FoodItem selectedFood = CurrentRandomFoods[buttonIndex];
        FeedPet(selectedFood.LoveValue, selectedFood.GrownValue, selectedFood.Name);
        SetupFoodButtons();
    }
    public void OnFoodButton2Click(int buttonIndex)
    {
        PlayScalePop();
        PlayCharacterVoice(currentCharacterIndex);
        if (isfinalstage) return;
        if (buttonIndex < 0 || buttonIndex >= CurrentRandomFoods.Length) return;
        FoodItem selectedFood = CurrentRandomFoods[buttonIndex];
        FeedPet(selectedFood.LoveValue, selectedFood.GrownValue, selectedFood.Name);
        SetupFoodButtons();
    }
    public void OnFoodButton3Click(int buttonIndex)
    {
        PlayScalePop();
        PlayCharacterVoice(currentCharacterIndex);
        if (isfinalstage) return;
        if (buttonIndex < 0 || buttonIndex >= CurrentRandomFoods.Length) return;
        FoodItem selectedFood = CurrentRandomFoods[buttonIndex];
        FeedPet(selectedFood.LoveValue, selectedFood.GrownValue, selectedFood.Name);
        SetupFoodButtons();
    }

    public void FeedPet(int loveAmount, int grownAmount, string currentFoodName)
    {
        Love += loveAmount;
        SumLove += loveAmount;
        Grown += grownAmount;
        SumGrown += grownAmount;
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
        FoodItem[] randomSelection = AllAvailableFoods.OrderBy(x => Random.value).Take(3).ToArray();
        for (int i = 0; i < 3; i++)
        {
            CurrentRandomFoods[i] = randomSelection[i];
            if (FoodButtonImages[i] != null) FoodButtonImages[i].sprite = randomSelection[i].FoodSprite;
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
            case 0: nextIndex = (Love >= 0) ? 2 : 1; break;
            case 1: nextIndex = (Love >= 0) ? 4 : 3; break;
            case 2: nextIndex = (Love >= 0) ? 6 : 5; break;
            case 3: nextIndex = (Love >= 0) ? 8 : 7; break;
            case 4: nextIndex = (Love >= 0) ? 10 : 9; break;
            case 5: nextIndex = (Love >= 0) ? 12 : 11; break;
            case 6: nextIndex = (Love >= 0) ? 14 : 13; break;
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
            default: Debug.LogWarning("Index นี้ไม่อยู่ในเงื่อนไขการวิวัฒนาการ"); break;
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
        if (targetImage == null) return;
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

    public void PlayScalePop()
    {
        StopAllCoroutines();
        StartCoroutine(ScalePopRoutine(animationDuration, scalePopFactor));
        AkSoundEngine.PostEvent("UISound", gameObject);
    }

    private IEnumerator ScalePopRoutine(float duration, float targetScaleFactor)
    {
        Vector3 startScale = originalanimationscale;
        Vector3 peakScale = originalanimationscale * targetScaleFactor;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            float pingPongValue = Mathf.PingPong(progress * 2.0f, 1.0f);
            targetImage.transform.localScale = Vector3.Lerp(startScale, peakScale, pingPongValue);
            yield return null;
        }
        targetImage.transform.localScale = originalanimationscale;
    }
}