using System;
using UnityEngine;

public abstract class AnimatorObjWrapper<T, R>
{
    public T Obj;

    protected Animator animator;
    protected string ParamName;
    protected Func<T, R> GetValue;

    public AnimatorObjWrapper(T Obj, Animator animator, string ParamName, Func<T, R> GetValue)
    {
        this.Obj = Obj;
        this.animator = animator;
        this.ParamName = ParamName;
        this.GetValue = GetValue;
    }
    protected void Do(Action<T> aExpression)
    {
        aExpression(Obj);
    }

    protected ReturnType Do<ReturnType>(Func<ReturnType> aExpression)
    {
        return aExpression();
    }
}

public class AnimatorIntWrapper<T> : AnimatorObjWrapper<T, int>
{
    public AnimatorIntWrapper(T obj, Animator animator, string ParamName, Func<T, int> GetIntValue) : 
        base(obj, animator, ParamName, GetIntValue)
    {
    }

    public void Call(Action<T> aExpression)
    {
        Do(aExpression);

        animator.SetInteger(ParamName, GetValue(Obj));
    }

    public ReturnType Call<ReturnType>(Func<ReturnType> aExpression)
    {
        ReturnType r = Do(aExpression);
        
        animator.SetInteger(ParamName, GetValue(Obj));

        return r;
    }
}
