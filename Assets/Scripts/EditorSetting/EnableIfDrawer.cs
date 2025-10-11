using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

//EnableAttribute���ݒ肳�ꂽ�v���p�e�B��\������Ƃ��ɌĂ΂��
[CustomPropertyDrawer(typeof(EnableIfAttribute))]
public class EnableIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnableIfAttribute enAttr = (EnableIfAttribute)attribute;
        //�����K�w(����t�@�C��)�ɂ��閼�O����
        string conditionPath = property.propertyPath.Replace(property.name, enAttr.conditionFieldName);
        //���O�̑O�ɂ��鑮��(int,float, bool...)���擾
        SerializedProperty conditionProp =  property.serializedObject.FindProperty(conditionPath);

        //�v���p�e�B��������Ȃ��ꍇ(���O���Ȃ��΂���)�ʏ�\��
        if (conditionProp == null)
        {
            //�\���֐�,bool�́A�q�����\�����邩�ǂ���
            EditorGUI.PropertyField(position, conditionProp, label,true);
            return;
        }

        bool enable = false;
        //conditonProb��bool�̎��A���̒l��enable�ɑ��,enable�ł͖���������false�ɂ���B
        if (conditionProp.propertyType == SerializedPropertyType.Boolean) 
        {
            enable = conditionProp.boolValue;
        }
        else 
        {
            enable = false;
        }
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
        string conditionPath = property.propertyPath.Replace(property.name, enAttr.conditionFieldName);
        SerializedProperty conditionProp = property.serializedObject.FindProperty(conditionPath);

        //��\���Ȃ�A����0�ɂ���
        if (conditionProp != null && conditionProp.propertyType == SerializedPropertyType.Boolean)
        {
            bool enable = conditionProp.boolValue;
            if (!enable && enAttr.hideWhenFalse)
            {
                return 0f;
            }
            
        }
        return EditorGUI.GetPropertyHeight(property, label,true);
    }
}
