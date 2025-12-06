using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("Scene System")]
    [SerializeField]
    public string nextSceneName = "SummaryScene";

    [Header("Character System")]
    [SerializeField]
    public List<CharacterItem> CharacterList;

    [SerializeField]
    public Image CharacterDisplayImage;

    [Header("First Stage Random Pool")]
    [SerializeField] public List<Sprite> FirstStageVariants;

    private Sprite selectedFirstStageSprite;
    private int selectedVariantIndex = 0;

    public static int currentCharacterIndex = 0;

    [Header("Stats")]
    [SerializeField]
    public int Love = 0;
    public int SumLove = 0;

    [SerializeField]
    public int Grown = 0;
    public int SumGrown = 0;

    [Header("Timer System")]
    // *** แก้ไขจุดที่ 1: เพิ่มตัวแปรสำหรับตั้งค่าเวลาเริ่มต้น ***
    [Tooltip("ใส่เวลาที่ต้องการตรงนี้ (วินาที)")]
    [SerializeField] public float StartTime = 60f;

    public float timeremain; // ไม่ต้องกำหนดค่าตรงนี้ เดี๋ยวไปรับค่าจาก StartTime เอง

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
        // =========================================================
        // *** RESET STATS ***
        // =========================================================
        currentCharacterIndex = 0;
        Love = 0;
        SumLove = 0;
        Grown = 0;
        SumGrown = 0;

        // *** แก้ไขจุดที่ 2: ให้เวลารีเซ็ตเท่ากับค่าที่คุณตั้งใน Inspector ***
        timeremain = StartTime;

        isfinalstage = false;

        PlayerPrefs.DeleteKey("SavedCharacterIndex");
        PlayerPrefs.DeleteKey("SavedVariantIndex");
        PlayerPrefs.DeleteKey("SavedLove");
        PlayerPrefs.DeleteKey("SavedGrown");
        // =========================================================

        timerRunning = true;

        if (FirstStageVariants != null && FirstStageVariants.Count > 0)
        {
            selectedVariantIndex = Random.Range(0, FirstStageVariants.Count);
            selectedFirstStageSprite = FirstStageVariants[selectedVariantIndex];
            Debug.Log("Game Started/Reset. Random Variant Selected: " + selectedVariantIndex);
        }

        SetupFoodButtons();

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

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

                SaveAndChangeScene();
            }
        }
    }

    void SaveAndChangeScene()
    {
        Debug.Log("Time's up! Saving data...");

        PlayerPrefs.SetInt("SavedCharacterIndex", currentCharacterIndex);
        PlayerPrefs.SetInt("SavedVariantIndex", selectedVariantIndex);
        PlayerPrefs.SetInt("SavedLove", Love);
        PlayerPrefs.SetInt("SavedGrown", Grown);

        PlayerPrefs.Save();

        Debug.Log($"Saved Character Index: {currentCharacterIndex}, Variant: {selectedVariantIndex}");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("ยังไม่ได้ใส่ชื่อ Scene ต่อไปใน Inspector!");
        }
    }

    public void UpdateCharacterDisplay(int index)
    {
        if (index >= 0 && index < CharacterList.Count)
        {
            if (CharacterDisplayImage != null)
            {
                if (index == 0)
                {
                    if (selectedFirstStageSprite != null)
                        CharacterDisplayImage.sprite = selectedFirstStageSprite;
                    else
                        CharacterDisplayImage.sprite = CharacterList[index].CharacterSprite;
                }
                else
                {
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

    public void PlayCharacterVoice(int charIndex)
    {
        string newEventName = "Char_Voice_" + charIndex.ToString();
        AkSoundEngine.PostEvent(newEventName, gameObject);
    }
    public void OnFoodButton1Click(int buttonIndex) { HandleFoodClick(buttonIndex); }
    public void OnFoodButton2Click(int buttonIndex) { HandleFoodClick(buttonIndex); }
    public void OnFoodButton3Click(int buttonIndex) { HandleFoodClick(buttonIndex); }

    void HandleFoodClick(int buttonIndex)
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
        if (Grown >= CharacterList[currentCharacterIndex].RequiredGrown)
        {
            CheckEvolution();
            PlayPopEffect();
        }
    }

    public void SetupFoodButtons()
    {
        if (AllAvailableFoods.Count < 3 || FoodButtonImages.Length < 3) return;
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