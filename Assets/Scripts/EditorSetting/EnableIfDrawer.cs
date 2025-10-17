#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.Rendering.VolumeComponent;

[CustomPropertyDrawer(typeof(EnableIfAttribute))]
public class EnableIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // ���C���X���b�h�ň��S�ɕ`��ł��邩�m�F
        if (!IsMainThreadSafe())
        {
            // ���C���X���b�h�ɖ߂����Ƃ��ɍĕ`�悷��悤�o�^���Ă���
            EditorApplication.delayCall += () => SafeRepaint(property);
            return;
        }

        EnableIfAttribute enableIf = (EnableIfAttribute)attribute;
        bool enabled = EvaluateConditionsRecursive(property, enableIf);

        bool prevGUIEnabled = GUI.enabled;
        GUI.enabled = enabled;

        if (!enableIf.hideWhenFalse || enabled)
        {
            try
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            catch (UnityException e)
            {
                // �t�H���g�������C���X���b�h�ˑ��Ŏ��s����P�[�X�����S�Ɉ���Ԃ�
                Debug.LogWarning($"[EnableIfDrawer] PropertyField skipped due to UnityException: {e.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EnableIfDrawer] Unexpected error drawing property: {ex}");
            }
        }

        GUI.enabled = prevGUIEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!IsMainThreadSafe())
            return 0f;

        EnableIfAttribute enableIf = (EnableIfAttribute)attribute;
        bool enabled = EvaluateConditionsRecursive(property, enableIf);
        if (enableIf.hideWhenFalse && !enabled)
            return 0f;

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    // --- �ȉ��͊����̃l�X�g�Ή����W�b�N�i���̂܂܁j ---
    private bool EvaluateConditionsRecursive(SerializedProperty property, EnableIfAttribute attribute)
    {
        object targetObject = GetParentTarget(property) ?? property.serializedObject.targetObject;
        if (targetObject == null)
            return true;

        bool[] results = attribute.conditionFieldNames.Select(name =>
        {
            bool negate = name.StartsWith("!");
            string fieldName = negate ? name.Substring(1) : name;

            FieldInfo field = GetFieldRecursiveFromRoot(targetObject, fieldName);
            if (field == null)
                return false;

            object value = field.GetValue(GetObjectContainingField(property, targetObject));
            bool result = value switch
            {
                bool b => b,
                Enum e => Convert.ToInt32(e) != 0,
                _ => value != null
            };

            return negate ? !result : result;
        }).ToArray();

        return attribute.logic switch
        {
            ConditionLogic.AND => results.All(r => r),
            ConditionLogic.OR => results.Any(r => r),
            ConditionLogic.NOT => !results.FirstOrDefault(),
            ConditionLogic.NAND => !results.All(r => r),
            ConditionLogic.NOR => !results.Any(r => r),
            ConditionLogic.XOR => results.Count(r => r) == 1,
            _ => true,
        };
    }

    // �ċA�I�Ɍ^��񂩂� FieldInfo ��T���i�p���`�F�[���Ή��j
    private FieldInfo GetFieldRecursiveFromRoot(object obj, string fieldName)
    {
        if (obj == null) return null;
        Type type = obj.GetType();
        while (type != null)
        {
            var f = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null) return f;
            type = type.BaseType;
        }
        return null;
    }

    // property.propertyPath �����ǂ��āu���̃t�B�[���h���܂ރI�u�W�F�N�g�v��Ԃ�
    private object GetParentTarget(SerializedProperty property)
    {
        string path = property.propertyPath.Replace(".Array.data[", "[");
        object obj = property.serializedObject.targetObject;
        var elements = path.Split('.');
        for (int i = 0; i < elements.Length - 1; i++)
        {
            string element = elements[i];
            if (element.Contains("["))
            {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = int.Parse(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValueFromObject(obj, elementName, index);
            }
            else
            {
                obj = GetValueFromObject(obj, element);
            }
            if (obj == null) return null;
        }
        
        return obj;
    }


    private object GetValueFromObject(object source, string name, int index = -1)
    {
    if (source == null) return null;
    FieldInfo f = GetFieldRecursiveFromRoot(source, name);
    if (f == null) return null;
    object v = f.GetValue(source);
    if (index >= 0 && v is System.Collections.IEnumerable enumerable)
    {
        var en = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!en.MoveNext()) return null;
            }

            return en.Current;
        }
        return v;
    }

    // ���̃v���p�e�B��������u���̃I�u�W�F�N�g���́v�i�t�B�[���h�����C���X�^���X�j��Ԃ��⏕
    private object GetObjectContainingField(SerializedProperty property, object root)
    {
        // GetParentTarget �œ������̂� root �̂͂��Ȃ̂ł��̂܂ܕԂ��i�璷���������̊g���p�j
        return GetParentTarget(property) ?? root;
    }

// --- ���C���X���b�h����ƈ��S�ȍĕ`�� ---
    private bool IsMainThreadSafe()
    {
        try
        {
            // main-thread-only APIs throw if called from loading thread; Screen.width is safe to probe
            var _ = UnityEngine.Screen.width;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void SafeRepaint(SerializedProperty property)
    {
        try
        {
            if (property == null || property.serializedObject == null) return;
            // Repaint inspector of the target object
            var editors = UnityEditor.Editor.CreateEditor(property.serializedObject.targetObject);
            if (editors != null) editors.Repaint();
        }
        catch { /* swallow */ }
    }
}
#endif
