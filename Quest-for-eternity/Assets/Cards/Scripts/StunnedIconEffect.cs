using UnityEngine;


[CreateAssetMenu(fileName = "StunnedIconEffect", menuName = "Effect/StunnedIconEffect")]
[System.Serializable]
public class StunnedIconEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SendCardsOver(null, inputGameobject.GetComponent<SceneObjectDatabase>().databasePlayerAccess.cardList.Count - 9);
        }
    }
}