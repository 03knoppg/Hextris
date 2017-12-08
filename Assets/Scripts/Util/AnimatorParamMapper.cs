using System;
using UnityEngine;

public abstract class AnimatorParamMapper<Type, AnimationType>
{
    protected Type _Obj;

    public Type Obj
    {
        get
        {
            return _Obj;
        }
        set
        {
            _Obj = value;
            UpdateAnimator();
        }
    }


    public AnimationType Value
    {
        get
        {
            if (GetValue == null)
                throw new Exception("GetValue Function undefined");

            return GetValue.Invoke(_Obj);
        }

        set
        {
            if (GetValue == null)
                throw new Exception("SetValue Function undefined");

            Obj = SetValue.Invoke(value);
            UpdateAnimator();
        }
    }

    protected Animator animator;
    protected string ParamName;
    protected Func<Type, AnimationType> GetValue;
    protected Func<AnimationType, Type> SetValue;

    public AnimatorParamMapper(Type Obj, Animator animator, string ParamName, Func<Type, AnimationType> GetValue, Func<AnimationType, Type> SetValue)
    {
        _Obj = Obj;
        this.animator = animator;
        this.ParamName = ParamName;
        this.GetValue = GetValue;
        this.SetValue = SetValue;

        UpdateAnimator();
    }
    protected void Do(Action<Type> aExpression)
    {
        aExpression(_Obj);
    }

    protected ReturnType Do<ReturnType>(Func<ReturnType> aExpression)
    {
        return aExpression();
    }

    public void Call(Action<Type> aExpression)
    {
        Do(aExpression);

        UpdateAnimator();
    }

    public ReturnType Call<ReturnType>(Func<ReturnType> aExpression)
    {
        ReturnType r = Do(aExpression);

        UpdateAnimator();

        return r;
    }

    protected abstract void UpdateAnimator();
}

public class AnimatorIntWrapper<T> : AnimatorParamMapper<T, int>
{
    public AnimatorIntWrapper(T obj, Animator animator, string ParamName, Func<T, int> GetIntValue, Func<int, T> SetIntValue = null) : 
        base(obj, animator, ParamName, GetIntValue, SetIntValue)
    {
    }

    protected override void UpdateAnimator()
    {
        if (GetValue == null)
            throw new Exception("GetValue Function undefined");

        animator.SetInteger(ParamName, GetValue.Invoke(_Obj));
    }
}


public class AnimatorFloatWrapper<T> : AnimatorParamMapper<T, float>
{
    public AnimatorFloatWrapper(T obj, Animator animator, string ParamName, Func<T, float> GetFloatValue, Func<float, T> SetFloatValue = null) :
        base(obj, animator, ParamName, GetFloatValue, SetFloatValue)
    {
    }

    protected override void UpdateAnimator()
    {
        if (GetValue == null)
            throw new Exception("GetValue Function undefined");

        animator.SetFloat(ParamName, GetValue.Invoke(_Obj));
    }
}


public class AnimatorBoolWrapper<T> : AnimatorParamMapper<T, bool>
{
    public AnimatorBoolWrapper(T obj, Animator animator, string ParamName, Func<T, bool> GetBoolValue, Func<bool, T> SetBoolValue = null) :
        base(obj, animator, ParamName, GetBoolValue, SetBoolValue)
    {
    }

    protected override void UpdateAnimator()
    {
        if (GetValue == null)
            throw new Exception("GetValue Function undefined");

        animator.SetBool(ParamName, GetValue.Invoke(_Obj));
    }
}


public class AnimatorTriggerWrapper<T> : AnimatorParamMapper<T, bool>
{
    public AnimatorTriggerWrapper(T obj, Animator animator, string ParamName, Func<T, bool> GetBoolValue, Func<bool, T> SetBoolValue = null) :
        base(obj, animator, ParamName, GetBoolValue, SetBoolValue)
    {
    }

    protected override void UpdateAnimator()
    {
        if (GetValue == null)
            throw new Exception("GetValue Function undefined");

        if (GetValue.Invoke(_Obj))
            animator.SetTrigger(ParamName);
        else
            animator.ResetTrigger(ParamName);
    }
}