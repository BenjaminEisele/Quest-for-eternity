using UnityEngine;


[CreateAssetMenu(fileName = "VitalStrikeEffect", menuName = "Effect/VitalStrikeEffect")]
[System.Serializable]
public class VitalStrikeEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SendCardsOver(null, inputGameobject.GetComponent<SceneObjectDatabase>().databasePlayerAccess.cardList.Count - 5);
        }
    }
}