using UnityEditor;
using UnityEngine;

namespace Rehawk.DOTweenSequencing.Editor
{
    [CustomPropertyDrawer(typeof(TweenValue<>), true)]
    public class TweenValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            SerializedProperty useFromProperty = property.FindPropertyRelative("UseFrom");
            SerializedProperty fromProperty = property.FindPropertyRelative("From");
            SerializedProperty toProperty = property.FindPropertyRelative("To");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float y = position.y;

            string header = TweenValueDrawerUtils.GetHeaderText(this, label);

            Rect headerLine = new Rect(position.x, y, position.width, lineHeight);

            bool useFrom = useFromProperty.boolValue;
            string badgeText = useFrom ? "From > To" : "To";

            TweenValueDrawerUtils.DrawHeaderLine(headerLine, header, badgeText, useFrom,
                                                 out Rect badgeRect, out Rect headerLabelRect);

            if (GUI.Button(badgeRect, badgeText, EditorStyles.miniButton))
                useFromProperty.boolValue = !useFromProperty.boolValue;

            y += lineHeight + TweenValueDrawerUtils.Spacing;

            Rect contentRect = new Rect(position.x, y, position.width, position.height - (lineHeight + TweenValueDrawerUtils.Spacing));
            Rect indented = TweenValueDrawerUtils.IndentRect(contentRect, EditorGUI.indentLevel + 1);

            DrawToFromRows(indented, useFromProperty, fromProperty, toProperty);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty useFromProperty = property.FindPropertyRelative("UseFrom");
            SerializedProperty fromProperty = property.FindPropertyRelative("From");
            SerializedProperty toProperty = property.FindPropertyRelative("To");

            float lineHeight = EditorGUIUtility.singleLineHeight;

            float headerHeight = lineHeight;

            float toHeight = EditorGUI.GetPropertyHeight(toProperty, true);
            float toLine = Mathf.Max(lineHeight, toHeight);

            float contentHeight = toLine;

            if (useFromProperty.boolValue)
            {
                float fromHeight = EditorGUI.GetPropertyHeight(fromProperty, true);
                float fromLine = Mathf.Max(lineHeight, fromHeight);
                contentHeight += TweenValueDrawerUtils.Spacing + fromLine;
            }

            return headerHeight + TweenValueDrawerUtils.Spacing + contentHeight;
        }
        
        private void DrawToFromRows(Rect position, SerializedProperty useFromProperty, SerializedProperty fromProperty,
                                    SerializedProperty toProperty)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            bool useFrom = useFromProperty.boolValue;

            float fromHeight = useFrom ? EditorGUI.GetPropertyHeight(fromProperty, true) : 0f;
            float toHeight = EditorGUI.GetPropertyHeight(toProperty, true);

            float fromLabelWidth = EditorStyles.label.CalcSize(new GUIContent("From")).x;
            float toLabelWidth = EditorStyles.label.CalcSize(new GUIContent("To")).x;
            float labelWidth = Mathf.Clamp(Mathf.Max(fromLabelWidth, toLabelWidth) + 10f, 28f, Mathf.Min(80f, position.width * 0.35f));

            float y = position.y;

            if (useFrom)
            {
                Rect fromLine = new Rect(position.x, y, position.width, Mathf.Max(lineHeight, fromHeight));
                Rect fromLabelRect = new Rect(fromLine.x, fromLine.y, labelWidth, lineHeight);
                Rect fromValueRect = new Rect(
                    fromLabelRect.xMax + TweenValueDrawerUtils.Spacing,
                    fromLine.y,
                    fromLine.xMax - (fromLabelRect.xMax + TweenValueDrawerUtils.Spacing),
                    fromHeight
                );

                GUI.Label(fromLabelRect, "From");
                EditorGUI.PropertyField(fromValueRect, fromProperty, GUIContent.none, true);

                y = fromLine.yMax + TweenValueDrawerUtils.Spacing;
            }

            Rect toLine = new Rect(position.x, y, position.width, Mathf.Max(lineHeight, toHeight));
            Rect toLabelRect = new Rect(toLine.x, toLine.y, labelWidth, lineHeight);
            Rect toValueRect = new Rect(
                toLabelRect.xMax + TweenValueDrawerUtils.Spacing,
                toLine.y,
                toLine.xMax - (toLabelRect.xMax + TweenValueDrawerUtils.Spacing),
                toHeight
            );

            GUI.Label(toLabelRect, "To");
            EditorGUI.PropertyField(toValueRect, toProperty, GUIContent.none, true);
        }
    }
}