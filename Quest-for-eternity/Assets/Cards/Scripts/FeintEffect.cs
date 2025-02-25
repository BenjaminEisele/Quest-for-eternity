using UnityEngine;


[CreateAssetMenu(fileName = "FeintEffect", menuName = "Effect/FeintEffect")]
[System.Serializable]
public class FeintEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SendCardsOver(null, inputGameobject.GetComponent<SceneObjectDatabase>().databasePlayerAccess.cardList.Count - 7);
        }
    }
}