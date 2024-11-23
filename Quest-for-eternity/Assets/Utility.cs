using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UtilityCard", menuName = "Cards/UtilityCard")]
[System.Serializable]
public class Utility : BaseCard
{
    // public List<EffectTemplate> effectList;
    public List<EffectUnit> effectUnitList;
    public bool isDisplayable;
}
