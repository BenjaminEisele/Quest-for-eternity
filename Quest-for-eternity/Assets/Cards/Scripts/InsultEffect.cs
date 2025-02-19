using UnityEngine;


[CreateAssetMenu(fileName = "InsultEffect", menuName = "Effect/InsultEffect")]
[System.Serializable]
public class InsultEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            Debug.Log("lead effect active");
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SendCardsOver(new GameObject().transform, 32);
        }
    }
}