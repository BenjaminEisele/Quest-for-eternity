using UnityEngine;


[CreateAssetMenu(fileName = "UtilityCountAttackEffect", menuName = "Effect/UtilityCountAttackEffect")]
[System.Serializable]
public class UtilityCountAttackEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {

        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            FieldScript.damagePoints += inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.utlCardsPlayedForOtherPlayer;
        }
        
    }
}
