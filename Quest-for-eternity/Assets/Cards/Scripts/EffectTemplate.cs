using UnityEngine;

[System.Serializable]
public abstract class EffectTemplate : ScriptableObject
{
    
    public virtual void UseEffect<T>(int targetId,float effectValue ,T value)
    {
        Debug.Log("I dun it!");
    }
}


