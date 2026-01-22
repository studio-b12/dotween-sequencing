using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Rehawk.DOTweenSequencing.Editor
{
    internal static class TweenValueDrawerUtils
    {
        public const float Spacing = 4f;
        public const float BadgeHeight = 18f;
        public const float BadgePaddingX = 8f;

        public static string GetHeaderText(PropertyDrawer drawer, GUIContent defaultLabel)
        {
            if (drawer.attribute is TweenValueDrawerAttribute drawerAttribute && drawerAttribute.OverrideLabel)
                return drawerAttribute.Label;

            FieldInfo fieldInfo = drawer.fieldInfo;
            if (fieldInfo == null)
                return defaultLabel.text;

            object[] attributes = fieldInfo.GetCustomAttributes(typeof(TweenValueDrawerAttribute), true);
            if (attributes.Length == 0)
                return defaultLabel.text;

            drawerAttribute = (TweenValueDrawerAttribute)attributes[0];
            return drawerAttribute.OverrideLabel ? drawerAttribute.Label : defaultLabel.text;
        }

        public static Rect IndentRect(Rect rect, int indentSteps)
        {
            float indent = 15f * indentSteps;
            rect.x += indent;
            rect.width = Mathf.Max(0f, rect.width - indent);
            return rect;
        }

        public static float CalcBadgeWidth(string badgeText)
        {
            return EditorStyles.miniButton.CalcSize(new GUIContent(badgeText)).x + BadgePaddingX;
        }

        public static void DrawBadgeToggle(Rect badgeRect, ref bool value, string badgeText)
        {
            if (GUI.Button(badgeRect, badgeText, EditorStyles.miniButton))
                value = !value;
        }

        public static void DrawHeaderLine(Rect headerLine, string headerText, string badgeText, bool toggleValue,
                                          out Rect badgeRect, out Rect headerLabelRect)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float badgeWidth = CalcBadgeWidth(badgeText);

            badgeRect = new Rect(
                headerLine.xMax - badgeWidth,
                headerLine.y + (lineHeight - BadgeHeight) * 0.5f,
                badgeWidth,
                BadgeHeight
            );

            headerLabelRect = new Rect(
                headerLine.x,
                headerLine.y,
                Mathf.Max(0f, badgeRect.xMin - headerLine.x - Spacing),
                headerLine.height
            );

            EditorGUI.LabelField(headerLabelRect, headerText);
        }
    }
}
