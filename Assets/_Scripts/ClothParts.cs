using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ClothParts
{
    public string clothName;//The name of the outfit we are wearing.
    public int clothIndex;
    public bool isLastWorn;
    public float clothPlayerHealth;
    public float clothSwordDamage;
    public float clothPrice;
    public Sprite clothImage;
    public Sprite[] clothSprites;//All the different sprites making up the outfit.
}
