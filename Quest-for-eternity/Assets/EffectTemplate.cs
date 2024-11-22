using UnityEngine;

[System.Serializable]
public abstract class EffectTemplate : ScriptableObject
{
    
    public virtual void UseEffect<T>(int targetId,int effectValue ,T value)
    {
        Debug.Log("I dun it!");
    }
}


