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

[CreateAssetMenu(fileName = "UtilityCard", menuName = "Cards/UtilityCard")]
public class Utility : BaseCard
{
   // public List<EffectTemplate> effectList;
    public List<EffectUnit> effectUnitList;
}

[CreateAssetMenu(fileName = "ActionCard", menuName = "Cards/ActionCard")]
[System.Serializable]
public class Action : BaseCard
{
    public int cardDamage;
}