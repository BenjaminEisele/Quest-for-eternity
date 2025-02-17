using UnityEngine;


[CreateAssetMenu(fileName = "FullImmunityEffect", menuName = "Effect/FullImmunityEffect")]
[System.Serializable]
public class FullImmunityEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            for(int i = 0; i < 2; i++)
            {
                inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.immunityIdList.Add(i);
            }
            //inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.immunityIdList.Add((int)effectValue);
            //inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.immunityCount += 2;
            inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.immunityCount += RefereeScript.instance.enemyList.Count;
        }
    }
}