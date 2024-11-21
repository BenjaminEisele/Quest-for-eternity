using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public abstract class BaseCard : ScriptableObject
{
    public string cardName;
    public string description;
    public Color cardColor;
    public Sprite cardSprite;
}

[CreateAssetMenu(fileName = "UtilityCard", menuName = "Cards/UtilityCard")]
public class Utility : BaseCard
{
    public List<EffectTemplate> effectList;
}

[CreateAssetMenu(fileName = "ActionCard", menuName = "Cards/ActionCard")]
public class Action : BaseCard
{
    public int cardDamage;
}