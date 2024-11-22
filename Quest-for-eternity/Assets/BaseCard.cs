using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class BaseCard : ScriptableObject
{
    public string cardName;
    public int effectValue;
    public Color cardColor;
    public Sprite cardSprite;
}



