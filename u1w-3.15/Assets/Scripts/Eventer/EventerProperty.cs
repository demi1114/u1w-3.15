using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Ev), true)]
public class EventerProperty : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string full = property.managedReferenceFullTypename;

        if (!string.IsNullOrEmpty(full))
        {
            // "Assembly-CSharp 名前空間.クラス名"
            string[] splitSpace = full.Split(' ');
            if (splitSpace.Length > 1)
            {
                string classPath = splitSpace[1];
                string[] splitDot = classPath.Split('.');
                string className = splitDot[splitDot.Length - 1]; // クラス名取得

                if (EvDict.nameDict.TryGetValue(className, out var jp)) //Eventer.cs内の名前リストから取得
                {
                    label.text = jp;
                }
                else
                {
                    label.text = className;
                }
            }
        }

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}