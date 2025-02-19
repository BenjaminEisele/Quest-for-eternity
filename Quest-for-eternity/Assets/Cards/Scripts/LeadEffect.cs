using UnityEngine;


[CreateAssetMenu(fileName = "LeadEffect", menuName = "Effect/LeadEffect")]
[System.Serializable]
public class LeadEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            Debug.Log("lead effect active");
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SendCardsOver(null, 31);
        }
    }
}