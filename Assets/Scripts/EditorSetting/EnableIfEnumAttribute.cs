using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field,AllowMultiple = false)]
public class EnableIfEnumAttribute : PropertyAttribute
{
    public string enumFiledName; //条件を判定するフィールド名
    public int[] enumValues;   //有効か対象のenumの値
    public bool hideWhenFalse;

    public EnableIfEnumAttribute(string enumFiledName, bool hideWhenFalse = false , params object[] enumValues)
    {
        this.enumFiledName = enumFiledName;
        this.enumValues = new int[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++)
        {
            this.enumValues[i] = (int)enumValues[i];
        }

        this.hideWhenFalse = hideWhenFalse;
    }
    //public EnableIfEnumAttribute(string enumFiledName, params object[] enumValues)
    //{
    //    this.enumFiledName = enumFiledName;
    //    this.enumValues = new int[enumValues.Length];
    //    for (int i = 0; i < enumValues.Length; i++)
    //    {
    //        this.enumValues[i] = (int)enumValues[i];
    //    }
    //    this.hideWhenFalse = false;
    //}
}