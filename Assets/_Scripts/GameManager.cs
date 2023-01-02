using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SpriteRenderer[] characterClothRenderer;
    public List<ClothParts> clothesList;
    private int _clothesIndex;
    
    public GameObject outfitSelectionPanel;
    public bool isOutfitPanelActive;
    
    //Cinemachine Cameras.
    public CinemachineVirtualCamera cameraPlayer, cameraOutfit;

    private PlayerController _playerController;

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
        for (int i = 0; i < characterClothRenderer.Length; ++i)
        {
            characterClothRenderer[i].sprite = clothesList[outfitIndex].clothSprites[i];
        }
    }

    public void ButtonPressed()
    {
        Debug.Log(GreenConsole("UI Button Pressed!"));
    }
    
    //Turn Debug Message to green. Wanna see this on success stuff.
    public static string GreenConsole(string text) => $"<b><color=green>{text}</color></b>";
}
