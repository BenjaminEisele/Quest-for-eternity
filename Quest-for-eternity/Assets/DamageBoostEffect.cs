using UnityEngine;


[CreateAssetMenu(fileName = "DamageBoostEffect", menuName = "Effect/DamageBoostEffect")]
[System.Serializable]
public class DamageBoostEffect : EffectTemplate
{
    
    public override void UseEffect<T>(int targetId, int effectValue, T value)
    {
        Debug.Log("damage boost activated");
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            Debug.Log(FieldScript.damagePoints);
            FieldScript.damagePoints += effectValue;
            Debug.Log(FieldScript.damagePoints);
            //UiScript.UpdateFieldDamageText(boostedDamage.ToString(), true);
        }
        //Debug.Log($"I boosted the damage by {targetId}! The value was: {value}");
    }
}
