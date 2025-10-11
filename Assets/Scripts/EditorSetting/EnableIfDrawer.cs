using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

//EnableAttributeが設定されたプロパティを表示するときに呼ばれる
[CustomPropertyDrawer(typeof(EnableIfAttribute))]
public class EnableIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnableIfAttribute enAttr = (EnableIfAttribute)attribute;
        //同じ階層(同一ファイル)にある名前検索
        string conditionPath = property.propertyPath.Replace(property.name, enAttr.conditionFieldName);
        //名前の前にある属性(int,float, bool...)を取得
        SerializedProperty conditionProp =  property.serializedObject.FindProperty(conditionPath);

        //プロパティが見つからない場合(名前がないばあい)通常表示
        if (conditionProp == null)
        {
            //表示関数,boolは、子供も表示するかどうか
            EditorGUI.PropertyField(position, conditionProp, label,true);
            return;
        }

        bool enable = false;
        //conditonProbがboolの時、その値をenableに代入,enableでは無かったらfalseにする。
        if (conditionProp.propertyType == SerializedPropertyType.Boolean) 
        {
            enable = conditionProp.boolValue;
        }
        else 
        {
            enable = false;
        }
        //隠す設定
        if (!enable && enAttr.hideWhenFalse)
        {
            //何もしない
            //非表示にする
            return;
        }
        //有効 or グレーアウトして表示させる
        bool prev = GUI.enabled;
        GUI.enabled = enable; //trueなら編集可能,falseなら編集できない
        //EditorGUIは、GUIの設定を元に、インスペクターを更新する。
        EditorGUI.PropertyField (position, property, label,true);
        //他のGUIが影響を受けないように戻す(このオーバーライドは、EnableIfAttirbuteが呼ばれた時に起こる)
        GUI.enabled = prev;
    }

    //無効にする場合、テキストフィールドの高さを0,つまり非表示にする。
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        EnableIfAttribute enAttr = (EnableIfAttribute)attribute;
        string conditionPath = property.propertyPath.Replace(property.name, enAttr.conditionFieldName);
        SerializedProperty conditionProp = property.serializedObject.FindProperty(conditionPath);

        //非表示なら、高さ0にする
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
