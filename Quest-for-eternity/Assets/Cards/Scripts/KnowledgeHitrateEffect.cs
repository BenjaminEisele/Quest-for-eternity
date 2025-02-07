using UnityEngine;

[CreateAssetMenu(fileName = "KnowledgeHitrateEffect", menuName = "Effect/KnowledgeHitrateEffect")]
[System.Serializable]
public class KnowledgeHitrateEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.knowledgeIdList.Add((int)effectValue);
    }
}
