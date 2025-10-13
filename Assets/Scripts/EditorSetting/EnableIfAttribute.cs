using System;
using UnityEngine;

public enum ConditionLogic
{
    AND,
    OR,
    NOT,
    NAND,
    NOR,
    XOR
}

[AttributeUsage(AttributeTargets.Field,Inherited = true,AllowMultiple = false)]
public class EnableIfAttribute : PropertyAttribute
{
    /// <summary>
    /// ���O�̐擪��!�������ꍇ�ے�ɂȂ�
    /// </summary>
    public string[] conditionFieldNames;
    public bool hideWhenFalse;
    public ConditionLogic logic;

    //�����ɖ��O������
    public EnableIfAttribute(string conditionFieldName, bool hideWhenFalse = false)
    {
        this.conditionFieldNames = new[] { conditionFieldName };
        this.hideWhenFalse = hideWhenFalse;
        this.logic = ConditionLogic.AND;
    }

    // ���������p�R���X�g���N�^
    public EnableIfAttribute(string[] conditionFieldNames, ConditionLogic logic = ConditionLogic.AND, bool hideWhenFalse = false)
    {
        this.conditionFieldNames = conditionFieldNames;
        this.hideWhenFalse = hideWhenFalse;
        this.logic = logic;
    }
}
