using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field,AllowMultiple = false)]
public class EnableIfEnumAttribute : PropertyAttribute
{
    public string enumFiledName; //�����𔻒肷��t�B�[���h��
    public int[] enumValues;   //�L�����Ώۂ�enum�̒l
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