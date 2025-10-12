using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field,Inherited = true,AllowMultiple = false)]
public class EnableIfAttribute : PropertyAttribute
{
    public string conditionFieldName;
    public bool hideWhenFalse;

    //�����ɖ��O������
    public EnableIfAttribute(string conditionFieldName, bool hideWhenFalse = false)
    {
        this.conditionFieldName = conditionFieldName;
        this.hideWhenFalse = hideWhenFalse;
    }
}
