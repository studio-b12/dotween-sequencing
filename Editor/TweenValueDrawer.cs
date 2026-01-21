using UnityEditor;
using UnityEngine;

namespace Rehawk.DOTweenSequencing.Editor
{
    [CustomPropertyDrawer(typeof(TweenValue<>), true)]
    public class TweenValueDrawer : PropertyDrawer
    {
        private const float Spacing = 4f;
        private const float BadgeHeight = 18f;
        private const float BadgePaddingX = 8f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            SerializedProperty useFromProperty = property.FindPropertyRelative("UseFrom");
            SerializedProperty fromProperty = property.FindPropertyRelative("From");
            SerializedProperty toProperty = property.FindPropertyRelative("To");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float y = position.y;

            string header = GetHeaderText(label);
            Rect headerLine = new Rect(position.x, y, position.width, lineHeight);

            bool useFrom = useFromProperty.boolValue;
            string badgeText = useFrom ? "From > To" : "To";

            float badgeWidth = EditorStyles.miniButton.CalcSize(new GUIContent(badgeText)).x + BadgePaddingX;
            Rect badgeRect = new Rect(
                headerLine.xMax - badgeWidth,
                headerLine.y + (lineHeight - BadgeHeight) * 0.5f,
                badgeWidth,
                BadgeHeight
            );

            Rect headerLabelRect = new Rect(
                headerLine.x,
                headerLine.y,
                Mathf.Max(0f, badgeRect.xMin - headerLine.x - Spacing),
                headerLine.height
            );

            EditorGUI.LabelField(headerLabelRect, header);

            if (GUI.Button(badgeRect, badgeText, EditorStyles.miniButton))
            {
                useFromProperty.boolValue = !useFromProperty.boolValue;
            }

            y += lineHeight + Spacing;

            Rect contentRect = new Rect(position.x, y, position.width, position.height - (lineHeight + Spacing));
            Rect indented = IndentRect(contentRect, EditorGUI.indentLevel + 1);

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
                contentHeight += Spacing + fromLine;
            }

            return headerHeight + Spacing + contentHeight;
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
                    fromLabelRect.xMax + Spacing,
                    fromLine.y,
                    fromLine.xMax - (fromLabelRect.xMax + Spacing),
                    fromHeight
                );

                GUI.Label(fromLabelRect, "From");
                EditorGUI.PropertyField(fromValueRect, fromProperty, GUIContent.none, true);

                y = fromLine.yMax + Spacing;
            }

            Rect toLine = new Rect(position.x, y, position.width, Mathf.Max(lineHeight, toHeight));
            Rect toLabelRect = new Rect(toLine.x, toLine.y, labelWidth, lineHeight);
            Rect toValueRect = new Rect(
                toLabelRect.xMax + Spacing,
                toLine.y,
                toLine.xMax - (toLabelRect.xMax + Spacing),
                toHeight
            );

            GUI.Label(toLabelRect, "To");
            EditorGUI.PropertyField(toValueRect, toProperty, GUIContent.none, true);
        }

        private string GetHeaderText(GUIContent defaultLabel)
        {
            if (attribute is TweenValueDrawerAttribute drawerAttribute && drawerAttribute.OverrideLabel)
                return drawerAttribute.Label;

            if (fieldInfo == null) 
                return defaultLabel.text;
            
            object[] attrs = fieldInfo.GetCustomAttributes(typeof(TweenValueDrawerAttribute), true);
            if (attrs.Length <= 0) 
                return defaultLabel.text;
            
            drawerAttribute = (TweenValueDrawerAttribute) attrs[0];
            return drawerAttribute.OverrideLabel ? drawerAttribute.Label : defaultLabel.text;
        }
        
        private static Rect IndentRect(Rect rect, int indentSteps = 1)
        {
            float indent = 15f * indentSteps;
            rect.x += indent;
            rect.width = Mathf.Max(0f, rect.width - indent);
            return rect;
        }
    }
}