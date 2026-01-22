using UnityEditor;
using UnityEngine;

namespace Rehawk.DOTweenSequencing.Editor
{
    [CustomPropertyDrawer(typeof(TweenVector2))]
    public sealed class TweenVector2Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            SerializedProperty useFromProperty = property.FindPropertyRelative("UseFrom");
            SerializedProperty fromProperty = property.FindPropertyRelative("From");
            SerializedProperty toProperty = property.FindPropertyRelative("To");
            SerializedProperty axesProperty = property.FindPropertyRelative("Axes");

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

            Rect contentRect = new Rect(position.x, y, position.width,
                position.height - (lineHeight + TweenValueDrawerUtils.Spacing));
            Rect indented = TweenValueDrawerUtils.IndentRect(contentRect, EditorGUI.indentLevel + 1);

            DrawAxesAndRows(indented, useFromProperty, fromProperty, toProperty, axesProperty);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty useFromProperty = property.FindPropertyRelative("UseFrom");
            SerializedProperty fromProperty = property.FindPropertyRelative("From");
            SerializedProperty toProperty = property.FindPropertyRelative("To");
            SerializedProperty axesProperty = property.FindPropertyRelative("Axes");

            float lineHeight = EditorGUIUtility.singleLineHeight;

            float totalHeight = lineHeight;
            totalHeight += TweenValueDrawerUtils.Spacing;

            float axesHeight = EditorGUI.GetPropertyHeight(axesProperty, true);
            totalHeight += Mathf.Max(lineHeight, axesHeight);
            totalHeight += TweenValueDrawerUtils.Spacing;

            float toHeight = EditorGUI.GetPropertyHeight(toProperty, true);
            totalHeight += Mathf.Max(lineHeight, toHeight);

            if (useFromProperty.boolValue)
            {
                totalHeight += TweenValueDrawerUtils.Spacing;
                float fromHeight = EditorGUI.GetPropertyHeight(fromProperty, true);
                totalHeight += Mathf.Max(lineHeight, fromHeight);
            }

            return totalHeight;
        }

        private static void DrawAxesAndRows(Rect position, SerializedProperty useFromProperty, SerializedProperty fromProperty,
                                            SerializedProperty toProperty, SerializedProperty axesProperty)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float y = position.y;

            float maxNeededWidth = Mathf.Max(EditorStyles.label.CalcSize(new GUIContent("Axes")).x,
                                             EditorStyles.label.CalcSize(new GUIContent("From")).x,
                                             EditorStyles.label.CalcSize(new GUIContent("To")).x);
            float labelWidth = Mathf.Clamp(maxNeededWidth + 10f, 28f, Mathf.Min(80f, position.width * 0.35f));

            float axesHeight = EditorGUI.GetPropertyHeight(axesProperty, true);
            Rect axesLine = new Rect(position.x, y, position.width, Mathf.Max(lineHeight, axesHeight));

            DrawLabeledPropertyRow(axesLine, "Axes", axesProperty, labelWidth);

            y = axesLine.yMax + TweenValueDrawerUtils.Spacing;

            bool useFrom = useFromProperty.boolValue;
            Vector2Axes axes = (Vector2Axes)axesProperty.intValue;

            if (useFrom)
            {
                float fromHeight = EditorGUI.GetPropertyHeight(fromProperty, true);
                Rect fromLine = new Rect(position.x, y, position.width, Mathf.Max(lineHeight, fromHeight));

                DrawVector2FilteredRow(fromLine, "From", fromProperty, axes, labelWidth);
                y = fromLine.yMax + TweenValueDrawerUtils.Spacing;
            }

            float toHeight = EditorGUI.GetPropertyHeight(toProperty, true);
            Rect toLine = new Rect(position.x, y, position.width, Mathf.Max(lineHeight, toHeight));

            DrawVector2FilteredRow(toLine, "To", toProperty, axes, labelWidth);
        }

        private static void DrawLabeledPropertyRow(Rect lineRect, string label, SerializedProperty property,
                                                   float labelWidth)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;

            Rect labelRect = new Rect(lineRect.x, lineRect.y, labelWidth, lineHeight);
            Rect valueRect = new Rect(labelRect.xMax + TweenValueDrawerUtils.Spacing,
                                      lineRect.y,
                                      lineRect.xMax - (labelRect.xMax + TweenValueDrawerUtils.Spacing),
                                      lineRect.height);

            GUI.Label(labelRect, label);
            EditorGUI.PropertyField(valueRect, property, GUIContent.none, true);
        }
        
        private static void DrawVector2FilteredRow(Rect lineRect, string label, SerializedProperty vectorProperty,
                                                   Vector2Axes axes, float labelWidth)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;

            Rect labelRect = new Rect(lineRect.x, lineRect.y, labelWidth, lineHeight);
            Rect valueRect = new Rect(labelRect.xMax + TweenValueDrawerUtils.Spacing,
                                      lineRect.y,
                                      lineRect.xMax - (labelRect.xMax + TweenValueDrawerUtils.Spacing),
                                      lineRect.height);

            GUI.Label(labelRect, label);

            bool xOn = (axes & Vector2Axes.X) != 0;
            bool yOn = (axes & Vector2Axes.Y) != 0;

            Vector2 v = vectorProperty.vector2Value;

            DrawVector2AxisFields(valueRect, ref v, xOn, yOn);

            vectorProperty.vector2Value = v;
        }

        private static void DrawVector2AxisFields(Rect rect, ref Vector2 vector2, bool xEnabled, bool yEnabled)
        {
            float width = rect.width;
            float height = EditorGUIUtility.singleLineHeight;

            // Layout similar-ish to Unity's Vector2Field: label+field for each component
            const float axisLabelWidth = 14f;
            const float innerSpacing = 4f;

            float fieldWidth = (width - (axisLabelWidth * 3f) - (innerSpacing * 5f)) / 2f;
            fieldWidth = Mathf.Max(10f, fieldWidth);

            float x0 = rect.x;

            // X
            Rect lx = new Rect(x0, rect.y, axisLabelWidth, height);
            Rect fx = new Rect(lx.xMax + innerSpacing, rect.y, fieldWidth, height);
            x0 = fx.xMax + innerSpacing;

            // Y
            Rect ly = new Rect(x0, rect.y, axisLabelWidth, height);
            Rect fy = new Rect(ly.xMax + innerSpacing, rect.y, fieldWidth, height);

            EditorGUI.LabelField(lx, "X");
            using (new EditorGUI.DisabledScope(!xEnabled))
                vector2.x = EditorGUI.FloatField(fx, vector2.x);

            EditorGUI.LabelField(ly, "Y");
            using (new EditorGUI.DisabledScope(!yEnabled))
                vector2.y = EditorGUI.FloatField(fy, vector2.y);
        }
    }
}