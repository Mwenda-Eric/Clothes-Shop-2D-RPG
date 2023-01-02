using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SpriteRenderer[] characterClothRenderer;
    public List<ClothParts> clothesList;
    private int _clothesIndex;
    
    //Outfit Panel UI Elements.
    public GameObject outfitSelectionPanel, attributesPanel;
    public TextMeshProUGUI outfitNameText, outfitPriceText, outfitHealthText, outfitSwordDamageText;
    public TextMeshProUGUI outfitPanelInfoDisplay;
    private int _outfitSelectedIndex;
    public Image outfitImageDisplay;
    public bool isOutfitPanelActive;
    
    
    
    //Cinemachine Cameras.
    public CinemachineVirtualCamera cameraPlayer, cameraOutfit;

    private PlayerController _playerController;
    private ShopkeeperScript _shopkeeper;

    //Use the Singleton pattern for this GameManager.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else if(Instance != null) Destroy(this);
        
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _shopkeeper = FindObjectOfType<ShopkeeperScript>();
    }

    public void EnableOutfitSelectionPanel()
    {
        isOutfitPanelActive = true;
        outfitSelectionPanel.SetActive(isOutfitPanelActive);
        //_shopkeeper.gameObject.SetActive(false);
        //Set the zoom camera.
        cameraPlayer.Priority = cameraOutfit.Priority - 1;
        _playerController.LockMovement();
    }

    public void DisableOutfitSelectionPanel()
    {
        isOutfitPanelActive = false;
        outfitSelectionPanel.SetActive(isOutfitPanelActive);
        //_shopkeeper.gameObject.SetActive(true);
        cameraPlayer.Priority = cameraOutfit.Priority + 1;
        _playerController.UnlockMovement();
    }
    public void ChangeNextOutfit()
    {
        for (int i = 0; i < characterClothRenderer.Length; ++i)
        {
            characterClothRenderer[i].sprite = clothesList[_clothesIndex].clothSprites[i];
        }
        
        ++_clothesIndex;
        if (_clothesIndex == clothesList.Count) _clothesIndex = 0;
    }

    public void DressOutfitAtIndex(int outfitIndex)
    {
        //Set respective texts to the attributes display for this selected outfit.
        outfitNameText.text = clothesList[outfitIndex].clothName;
        outfitImageDisplay.sprite = clothesList[outfitIndex].clothImage;
        outfitPriceText.text = clothesList[outfitIndex].clothPrice.ToString();
        outfitHealthText.text = "TOTAL HEALTH\n" + clothesList[outfitIndex].clothPlayerHealth;
        outfitSwordDamageText.text = "SWORD DAMAGE\n" + clothesList[outfitIndex].clothSwordDamage;
        _outfitSelectedIndex = outfitIndex;
        DressCharacterSelectedOutfit();
    }

    public void BuyTheOutfit()
    {
        Debug.Log("Check if we have enough coins though.");
        outfitPanelInfoDisplay.color = Color.green;
        outfitPanelInfoDisplay.text = "Successfully Bought " + clothesList[_outfitSelectedIndex].clothName; 
        DressCharacterSelectedOutfit();
    }

    private void DressCharacterSelectedOutfit()
    {
        //This will Dress the character with cloth sprites at selected Index.
        for (int i = 0; i < characterClothRenderer.Length; ++i)
        {
            characterClothRenderer[i].sprite = clothesList[_outfitSelectedIndex].clothSprites[i];
        }
    }

    public void ButtonPressed()
    {
        Debug.Log(GreenConsole("UI Button Pressed!"));
    }
    
    //Turn Debug Message to green. Wanna see this on success stuff.
    public static string GreenConsole(string text) => $"<b><color=green>{text}</color></b>";
    public static string RedConsole(string text) => $"<b><color=red>{text}</color></b>";
}
