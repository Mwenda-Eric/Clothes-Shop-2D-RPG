using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private ClothParts[] purchasedOutfits;
    private int[] purchasedIndexes;
    private int _clothesIndex;
    private int _playerCoins;
    
    //Outfit Panel UI Elements.
    public GameObject outfitSelectionPanel, attributesPanel, buyButton;
    public TextMeshProUGUI outfitNameText, outfitPriceText, outfitHealthText, outfitSwordDamageText;
    public TextMeshProUGUI outfitPanelInfoDisplay, coinsDisplay;
    private int _outfitSelectedIndex;
    public Image outfitImageDisplay;
    public bool isOutfitPanelActive;
    
    //Cinemachine Cameras.
    public CinemachineVirtualCamera cameraPlayer, cameraOutfit;

    private PlayerController _playerController;
    private ShopkeeperScript _shopkeeper;

    private string _folderPath;

    //Use the Singleton pattern for this GameManager.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else if(Instance != null) Destroy(this);
        
        DontDestroyOnLoad(this);
        //PlayerPrefs.SetInt("PlayerCoins" , 1000000);
    }

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _shopkeeper = FindObjectOfType<ShopkeeperScript>();
        DisplayCoins();
        
        //Path to be saving the purchased outfit classes.
        string folderName = "Outfits";
        char directorySeparator = Path.DirectorySeparatorChar;
        _folderPath = Application.persistentDataPath + directorySeparator + folderName + directorySeparator;
        purchasedOutfits = LoadAllOutfits();
    }
    
    public void AddPlayerCoins(int numberOfCoins,bool isIncrease)
    {
        _playerCoins = PlayerPrefs.GetInt("PlayerCoins");
        if (isIncrease) _playerCoins += numberOfCoins; else _playerCoins -= numberOfCoins;
        PlayerPrefs.SetInt("PlayerCoins", _playerCoins);
        
        DisplayCoins();
        Debug.Log(GreenConsole("Coins Added! Total = " + _playerCoins));
    }

    private void DisplayCoins()
    {
        _playerCoins = PlayerPrefs.GetInt("PlayerCoins");
        coinsDisplay.text = "Coins\n" + _playerCoins;
    }

    public void EnableOutfitSelectionPanel()
    {
        isOutfitPanelActive = true;
        outfitSelectionPanel.SetActive(isOutfitPanelActive);
        
        //Set the zoom camera.
        cameraPlayer.Priority = cameraOutfit.Priority - 1;
        _playerController.LockMovement();
    }

    public void DisableOutfitSelectionPanel()
    {
        isOutfitPanelActive = false;
        outfitSelectionPanel.SetActive(isOutfitPanelActive);
        
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

        if (IsOutfitPurchased(outfitIndex))
        {
            Debug.Log("Outfit is Already Purchased.");
            outfitPanelInfoDisplay.color = Color.green;
            outfitPanelInfoDisplay.text = "Already Bought " + clothesList[outfitIndex].clothName;
            buyButton.SetActive(false);
        }
        else
        {
            outfitPanelInfoDisplay.text = "";
            buyButton.SetActive(true);
        }
    }

    private bool IsOutfitPurchased(int outfitIndex)
    {
        purchasedOutfits = LoadAllOutfits();
        return purchasedIndexes.Contains(outfitIndex);
    }

    public void BuyTheOutfit()
    {
        if (_playerCoins < clothesList[_outfitSelectedIndex].clothPrice)
        {
            outfitPanelInfoDisplay.color = Color.red;
            outfitPanelInfoDisplay.text = "Not Enough Coins!";
            return;
        }
        
        outfitPanelInfoDisplay.color = Color.green;
        outfitPanelInfoDisplay.text = "Successfully Bought " + clothesList[_outfitSelectedIndex].clothName; 
        buyButton.SetActive(false);
        
        AddPlayerCoins((int)clothesList[_outfitSelectedIndex].clothPrice, false);
        DressCharacterSelectedOutfit();
        
        //Save the purchased outfit to local storage.
        SavePurchasedOutfit(clothesList[_outfitSelectedIndex]);
    }

    private void SavePurchasedOutfit(ClothParts outfit)
    {
        string fileName = outfit.clothName;
        string fileExtension = ".json";
        
        if (!Directory.Exists(_folderPath))
            Directory.CreateDirectory(_folderPath);
        
        var filePath = _folderPath + fileName + fileExtension;

        var json = JsonUtility.ToJson(outfit);
        File.WriteAllText(filePath, json);
        //Debug.Log(GreenConsole("Saved Successfully to : " + filePath));
    }

    private ClothParts[] LoadAllOutfits()
    {
        if (!Directory.Exists(_folderPath))
            Directory.CreateDirectory(_folderPath);
        
        var files = Directory.GetFiles(_folderPath);
        var data = new ClothParts[files.Length];

        for (int i = 0; i < files.Length; ++i)
        {
            var json = File.ReadAllText(files[i]);
            data[i] = JsonUtility.FromJson<ClothParts>(json);
        }
        
        purchasedIndexes = new int[data.Length];
        for (int i = 0; i < data.Length; ++i)
        {
            purchasedIndexes[i] = data[i].clothIndex;
        }
        return data;
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
