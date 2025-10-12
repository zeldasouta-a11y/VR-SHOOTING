using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field,Inherited = true,AllowMultiple = false)]
public class EnableIfAttribute : PropertyAttribute
{
    public string conditionFieldName;
    public bool hideWhenFalse;

    //‚±‚±‚É–¼‘O‚ð“ü‚ê‚é
    public EnableIfAttribute(string conditionFieldName, bool hideWhenFalse = false)
    {
        this.conditionFieldName = conditionFieldName;
        this.hideWhenFalse = hideWhenFalse;
    }
}
