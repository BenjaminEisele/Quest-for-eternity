using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ActionCard", menuName = "Cards/ActionCard")]
[System.Serializable]
public class Action : BaseCard
{
    public int cardDamage;
    public float cardHitRate;
    public List<EffectUnit> actionEffectUnitList;
}
