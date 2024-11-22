using UnityEngine;

[System.Serializable]
public abstract class EffectTemplate : ScriptableObject
{
    public virtual void UseEffect<T>(int targetId, T value)
    {
        Debug.Log("I dun it!");
    }
}


