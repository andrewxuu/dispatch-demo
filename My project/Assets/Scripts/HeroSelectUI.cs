using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Required for Lists

public class HeroSelectUI : MonoBehaviour
{
    [Header("UI Containers")]
    public GameObject uiStateObject;
    public GameObject gameStateObject;
    public Transform buttonContainer; // The Roster Bar
    
    // NEW: The container where slots go (under the radar)
    public Transform slotsContainer; 

    [Header("Prefabs")]
    public GameObject buttonCardPrefab;
    public GameObject slotPrefab; // Drag 'TeamSlotTemplate' here

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
        uiStateObject.SetActive(true);
        gameStateObject.SetActive(true);
        
        dispatchButton.interactable = false;
        dispatchButton.onClick.AddListener(OnDispatchClicked);

        // 1. Build the Roster
        RefreshRoster();

        // 2. Build the Slots based on Mission Data
        GenerateMissionSlots();
    }

void GenerateMissionSlots()
    {
        // A. Clear existing slots
        foreach (Transform child in slotsContainer) Destroy(child.gameObject);
        spawnedSlots.Clear();

        // B. Spawn new slots based on Mission Data
        int count = currentMission.teamSize; // <-- GETTING NUMBER FROM DATA

        for (int i = 0; i < count; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotsContainer);
            Image slotImg = newSlot.GetComponent<Image>();
            
            // Set default empty state
            slotImg.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Dark Grey
            
            spawnedSlots.Add(slotImg);
        }
    }

    void RefreshRoster()
    {
        foreach (Transform child in buttonContainer) Destroy(child.gameObject);

        foreach (HeroStats hero in availableHeroes)
        {
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
        
        // 3. Fill the FIRST slot with the selected hero
        // (Later, you can add logic to fill Slot 2, Slot 3, etc.)
        if (spawnedSlots.Count > 0)
        {
            spawnedSlots[0].sprite = hero.heroPortrait;
            spawnedSlots[0].color = Color.white;
        }

        // Highlight Roster Card
        if (currentSelectedCard != null) 
             currentSelectedCard.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f); 
        
        currentSelectedCard = cardObject;
        currentSelectedCard.GetComponent<Image>().color = new Color(0f, 0.8f, 0.8f);

        dispatchButton.interactable = true;
    }
    void OnDispatchClicked()
    {
        uiStateObject.SetActive(false);
        gameManager.ConfirmAndStart();
    }
}