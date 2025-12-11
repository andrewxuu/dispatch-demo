using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; 

public class HeroSelectUI : MonoBehaviour
{
    [Header("UI Containers")]
    public Transform buttonContainer; 
    public Transform slotsContainer; 

    [Header("Text References")]
    // NEW: Drag your "Civilian Quote" text object here
    public TextMeshProUGUI missionDescriptionText; 
    
    // Optional: If you want to show the Title too
    public TextMeshProUGUI missionTitleText;

    [Header("Prefabs")]
    public GameObject buttonCardPrefab;
    public GameObject slotPrefab; 

    [Header("Buttons")]
    public Button dispatchButton;

    [Header("Data")]
    public MissionTemplate currentMission; 
    public HeroStats[] availableHeroes;    
    public MinigameManager gameManager;  

    private List<Image> spawnedSlots = new List<Image>();
    private GameObject currentSelectedCard;

    void Start()
    {
        dispatchButton.interactable = false;
        dispatchButton.onClick.AddListener(OnDispatchClicked);

        // --- NEW: Update the Mission Text ---
        if (currentMission != null)
        {
            if (missionDescriptionText != null) 
                missionDescriptionText.text = currentMission.missionDescription;

            if (missionTitleText != null)
                missionTitleText.text = currentMission.missionName.ToUpper();
        }
        // ------------------------------------

        RefreshRoster();
        GenerateMissionSlots();
    }
    
void GenerateMissionSlots()
    {
       foreach (Transform child in slotsContainer) Destroy(child.gameObject);
        spawnedSlots.Clear();
        for (int i = 0; i < currentMission.teamSize; i++) {
            GameObject newSlot = Instantiate(slotPrefab, slotsContainer);
            spawnedSlots.Add(newSlot.GetComponent<Image>());
        }
    }

    void RefreshRoster() 
    { 
         foreach (Transform child in buttonContainer) Destroy(child.gameObject);
         foreach (HeroStats hero in availableHeroes) {
            GameObject newCard = Instantiate(buttonCardPrefab, buttonContainer);
            TMP_Text nameText = newCard.transform.Find("NameText").GetComponent<TMP_Text>();
            Image portraitImg = newCard.transform.Find("PortraitImage").GetComponent<Image>();
            if (nameText != null) nameText.text = hero.heroName.ToUpper();
            if (portraitImg != null) portraitImg.sprite = hero.heroPortrait;
            newCard.GetComponent<Button>().onClick.AddListener(() => OnHeroPreview(hero, newCard));
         }
    }

    void OnHeroPreview(HeroStats hero, GameObject cardObject)
    {
        gameManager.SetupPreview(hero, currentMission);
        
        if (spawnedSlots.Count > 0)
        {
            spawnedSlots[0].sprite = hero.heroPortrait;
            spawnedSlots[0].color = Color.white;
        }
        if (currentSelectedCard != null) 
             currentSelectedCard.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f); 
        
        currentSelectedCard = cardObject;
        currentSelectedCard.GetComponent<Image>().color = new Color(0f, 0.8f, 0.8f);

        dispatchButton.interactable = true;
    }

    void OnDispatchClicked()
    {
        SetInteractivity(false); 
        gameManager.ConfirmAndStart();
    }

    public void SetInteractivity(bool isInteractable)
    {
        dispatchButton.interactable = isInteractable;

        Button[] rosterButtons = buttonContainer.GetComponentsInChildren<Button>();
        foreach (Button btn in rosterButtons)
        {
            btn.interactable = isInteractable;
        }
    }
}