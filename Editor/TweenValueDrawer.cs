using UnityEditor;
using UnityEngine;

namespace Rehawk.DOTweenSequencing.Editor
{
    [CustomPropertyDrawer(typeof(TweenValue<>), true)]
    public class TweenValueDrawer : PropertyDrawer
    {
        private const float ToggleWidth = 18f;
        private const float LabelWidth = 32f;
        private const float Spacing = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            SerializedProperty useFromProperty = property.FindPropertyRelative("UseFrom");
            SerializedProperty fromProperty = property.FindPropertyRelative("From");
            SerializedProperty toProperty = property.FindPropertyRelative("To");

            float lineHeight = EditorGUIUtility.singleLineHeight;

            float fromHeight = EditorGUI.GetPropertyHeight(fromProperty, true);
            float toHeight = EditorGUI.GetPropertyHeight(toProperty, true);

            float y = position.y;

            // ---- Line 1: From label + Toggle + From value ----
            Rect fromLine = new Rect(position.x, y, position.width, Mathf.Max(lineHeight, fromHeight));

            Rect fromLabelRect = new Rect(
                fromLine.x,
                fromLine.y,
                LabelWidth,
                lineHeight
            );

            Rect toggleRect = new Rect(
                fromLabelRect.xMax + Spacing,
                fromLine.y,
                ToggleWidth,
                lineHeight
            );

            float valueX = toggleRect.xMax + Spacing; // shared column start for values
            Rect fromValueRect = new Rect(
                valueX,
                fromLine.y,
                fromLine.xMax - valueX,
                fromHeight
            );

            EditorGUI.LabelField(fromLabelRect, "From");
            useFromProperty.boolValue = EditorGUI.Toggle(toggleRect, useFromProperty.boolValue);

            using (new EditorGUI.DisabledScope(!useFromProperty.boolValue))
            {
                EditorGUI.PropertyField(fromValueRect, fromProperty, GUIContent.none, true);
            }

            // ---- Line 2: To label + To value (aligned to valueX) ----
            y = fromLine.yMax + Spacing;

            Rect toLine = new Rect(position.x, y, position.width, Mathf.Max(lineHeight, toHeight));

            Rect toLabelRect = new Rect(
                toLine.x,
                toLine.y,
                LabelWidth,
                lineHeight
            );

            Rect toValueRect = new Rect(
                valueX,             // same start as From value
                toLine.y,
                toLine.xMax - valueX,
                toHeight
            );

            EditorGUI.LabelField(toLabelRect, "To");
            EditorGUI.PropertyField(toValueRect, toProperty, GUIContent.none, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty fromProperty = property.FindPropertyRelative("From");
            SerializedProperty toProperty = property.FindPropertyRelative("To");

            float lineHeight = EditorGUIUtility.singleLineHeight;

            float fromHeight = EditorGUI.GetPropertyHeight(fromProperty, true);
            float toHeight = EditorGUI.GetPropertyHeight(toProperty, true);

            float fromLine = Mathf.Max(lineHeight, fromHeight);
            float toLine = Mathf.Max(lineHeight, toHeight);

            return fromLine + Spacing + toLine;
        }
    }
}