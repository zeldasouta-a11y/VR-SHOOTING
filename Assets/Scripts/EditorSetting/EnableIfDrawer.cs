using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

//EnableAttribute���ݒ肳�ꂽ�v���p�e�B��\������Ƃ��ɌĂ΂��
[CustomPropertyDrawer(typeof(EnableIfAttribute))]
public class EnableIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnableIfAttribute enAttr = (EnableIfAttribute)attribute;
        
        bool enable = EvaluateConditions(property, enAttr);

        //�B���ݒ�
        if (!enable && enAttr.hideWhenFalse)
        {
            //�������Ȃ�
            //��\���ɂ���
            return;
        }
        //�L�� or �O���[�A�E�g���ĕ\��������
        bool prev = GUI.enabled;
        GUI.enabled = enable; //true�Ȃ�ҏW�\,false�Ȃ�ҏW�ł��Ȃ�
        //EditorGUI�́AGUI�̐ݒ�����ɁA�C���X�y�N�^�[���X�V����B
        EditorGUI.PropertyField (position, property, label,true);
        //����GUI���e�����󂯂Ȃ��悤�ɖ߂�(���̃I�[�o�[���C�h�́AEnableIfAttirbute���Ă΂ꂽ���ɋN����)
        GUI.enabled = prev;
    }

    //�����ɂ���ꍇ�A�e�L�X�g�t�B�[���h�̍�����0,�܂��\���ɂ���B
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        EnableIfAttribute enAttr = (EnableIfAttribute)attribute;
        bool enable = EvaluateConditions(property, enAttr);

        //��\���Ȃ�A����0�ɂ���
        if (!enable && enAttr.hideWhenFalse)
        {
            return 0f;
        }
            
        return EditorGUI.GetPropertyHeight(property, label,true);
    }
    private bool EvaluateConditions(SerializedProperty property, EnableIfAttribute enAttr)
    {
        if (enAttr.conditionFieldNames == null || enAttr.conditionFieldNames.Length == 0)
            return true;

        bool[] results = new bool[enAttr.conditionFieldNames.Length];

        for (int i = 0; i < enAttr.conditionFieldNames.Length; i++)
        {
            string rawName = enAttr.conditionFieldNames[i];
            bool negate = false;

            // �ے�v���t�B�b�N�X '!' �ɑΉ�
            if (rawName.StartsWith("!"))
            {
                negate = true;
                rawName = rawName.Substring(1); // �擪��!���폜
            }
            //�����K�w(����t�@�C��)�ɂ��閼�O����
            string conditionPath = property.propertyPath.Replace(property.name, rawName);
            //���O�̑O�ɂ��鑮��(int,float, bool...)���擾
            SerializedProperty conditionProp = property.serializedObject.FindProperty(conditionPath);

            bool value = false;
            if (conditionProp != null && conditionProp.propertyType == SerializedPropertyType.Boolean)
                value = conditionProp.boolValue;

            // �ے艉�Z��K�p
            results[i] = negate ? !value : value;
        }

        // ---- �_�����Z�܂Ƃ� ----
        switch (enAttr.logic)
        {
            case ConditionLogic.AND:
                return Array.TrueForAll(results, r => r);

            case ConditionLogic.OR:
                return Array.Exists(results, r => r);

            case ConditionLogic.NOT:
                return !results[0];

            case ConditionLogic.NAND:
                return !Array.TrueForAll(results, r => r);

            case ConditionLogic.NOR:
                return !Array.Exists(results, r => r);

            case ConditionLogic.XOR:
                int count = 0;
                foreach (bool r in results)
                    if (r) count++;
                return (count % 2 == 1);

            default:
                return true;
        }
    }
}
