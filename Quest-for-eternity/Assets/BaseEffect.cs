using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public interface IEffect
{
   public void ExecuteEffect<T>(T input);
}
[CreateAssetMenu(fileName = "BaseEffectFileName", menuName = "BaseEffectScriptable")]
public class BaseEffect : ScriptableObject, IEffect
{
    public IEffect myEffectInterface;
    public int damage;
    public string lalala;
    public int lolololol;
    
    public virtual void ExecuteEffect<T>(T input)
    {
        Debug.Log("effect executed!");
    }

    public virtual void TryAddEffect<T>(T input)
    {
        Debug.Log("test!");
        myEffectInterface.ExecuteEffect(222);
    }
}
