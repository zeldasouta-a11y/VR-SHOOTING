using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class OnInspectorButtonAttribute : PropertyAttribute
{
    public string label;
    public bool showOnlyInPlayMode;

    /// <param name="label">�{�^���̃��x���inull�Ȃ烁�\�b�h���j</param>
    /// <param name="showOnlyInPlayMode">���s���̂ݕ\�����邩</param>
    public OnInspectorButtonAttribute(string label = null, bool showOnlyInPlayMode = false)
    {
        this.label = label;
        this.showOnlyInPlayMode = showOnlyInPlayMode;
    }
}
