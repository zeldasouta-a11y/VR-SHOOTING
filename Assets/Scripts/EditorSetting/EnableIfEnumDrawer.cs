using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnableIfEnumAttribute))]
public class EnableIfEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnableIfEnumAttribute condition = (EnableIfEnumAttribute)attribute;

        //���݂̃v���p�e�B��
        string conditionPath = property.propertyPath.Replace(property.name, condition.enumFiledName);
        SerializedProperty enumProp = property.serializedObject.FindProperty(conditionPath);

        //�Ȃ��ꍇ�f�t�H���g�\��
        if (enumProp == null)
        {
            //�\���֐�,bool�́A�q�����\�����邩�ǂ���
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        bool enabled = false;
        if (enumProp.propertyType == SerializedPropertyType.Enum)
        {
            int currentValue = enumProp.enumValueIndex;//enum�̌��ݒn���擾
            foreach(int valid in condition.enumValues){
                if (currentValue == valid)
                {
                    enabled = true;//���ݒn�Ǝw��n���������̂݁A�^�ɍX�V
                    break;
                }
            }
        }
        //�B���ݒ�
        if (!enabled && condition.hideWhenFalse)
        {
            //�����\�����Ȃ�
            return;
        }

        //�����ɍ���Ȃ��ꍇ�̓O���[
        bool prev = GUI.enabled;
        GUI.enabled = enabled;
        EditorGUI.PropertyField (position, property, label, true);
        GUI.enabled = prev;
    }
    //�����ɂ���ꍇ�A�e�L�X�g�t�B�[���h�̍�����0,�܂��\���ɂ���B
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        EnableIfEnumAttribute condition = (EnableIfEnumAttribute)attribute;
        string conditionPath = property.propertyPath.Replace(property.name, condition.enumFiledName);
        SerializedProperty enumProp = property.serializedObject.FindProperty(conditionPath);

        //��\���Ȃ�A����0�ɂ���
        if (enumProp != null && enumProp.propertyType == SerializedPropertyType.Enum)
        {
            bool enabled = false;
            int currentValue = enumProp.enumValueIndex;//enum�̌��ݒn���擾
            foreach (int valid in condition.enumValues)
            {
                if (currentValue == valid)
                {
                    enabled = true;//���ݒn�Ǝw��n���������̂݁A�^�ɍX�V
                    break;
                }
            }
            if (!enabled && condition.hideWhenFalse)
            {
                return 0f;
            }

        }
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
