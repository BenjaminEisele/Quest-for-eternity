using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DataUnit //: MonoBehaviour
{
    public int damage;
    public Vector2 hitFraction;
    public Color cardColor;
    public int cardType;
    public string cardName;
    public Sprite cardSprite;
    public List<BaseEffect> cardEffect;
    //public List<EffectDataUnit> cardEffect;
    
}
