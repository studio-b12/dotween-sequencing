using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Rehawk.DOTweenSequencing.Editor
{
    [CustomEditor(typeof(DOTweenSequencer))]
    public class DOTweenSequencerEditor : UnityEditor.Editor
    {
        private const float HEADER_HEIGHT = 28f;
        private const float HEADER_PADDING = 6f;
        private const float BODY_PADDING = 6f;
        private const float TYPE_LABEL_HEIGHT = 18f;
        private const float TYPE_LABEL_SPACING = 4f;
        private const float PROGRESS_BAR_HEIGHT = 8f;
        private const float PROGRESS_BAR_SPACING = 4f;
        
        private SerializedProperty playOnEnableProperty;
        private SerializedProperty restartOnEnableProperty;

        private SerializedProperty autoKillProperty;
        private SerializedProperty recyclableProperty;
        private SerializedProperty loopsProperty;
        private SerializedProperty loopTypeProperty;
        private SerializedProperty updateTypeProperty;
        private SerializedProperty ignoreTimeScaleProperty;
        private SerializedProperty durationMultiplierProperty;
        
        private SerializedProperty stepsProperty;
        
        private SerializedProperty onStartProperty;
        private SerializedProperty onPauseProperty;
        private SerializedProperty onCompleteProperty;
        private SerializedProperty onStepCompleteProperty;
        private SerializedProperty onRewindProperty;
        
        private ReorderableList stepsList;
        private AdvancedDropdownState addDropdownState;

        private int selectedIndex = -1;
        private bool lastShowBars;
            
        private readonly List<StepTiming> stepTimings = new();
        
        private void OnEnable()
        {
            playOnEnableProperty = serializedObject.FindProperty("playOnEnable");
            restartOnEnableProperty = serializedObject.FindProperty("restartOnEnable");

            autoKillProperty = serializedObject.FindProperty("autoKill");
            recyclableProperty = serializedObject.FindProperty("recyclable");
            loopsProperty = serializedObject.FindProperty("loops");
            loopTypeProperty = serializedObject.FindProperty("loopType");
            updateTypeProperty = serializedObject.FindProperty("updateType");
            ignoreTimeScaleProperty = serializedObject.FindProperty("ignoreTimeScale");
            durationMultiplierProperty = serializedObject.FindProperty("durationMultiplier");

            stepsProperty = serializedObject.FindProperty("steps");
            
            onStartProperty = serializedObject.FindProperty("onStarted");
            onPauseProperty = serializedObject.FindProperty("onPaused");
            onCompleteProperty = serializedObject.FindProperty("onCompleted");
            onStepCompleteProperty = serializedObject.FindProperty("onStepCompleted");
            onRewindProperty = serializedObject.FindProperty("onRewound");
            
            addDropdownState ??= new AdvancedDropdownState();
            
            lastShowBars = ShouldShowProgressBars();
            BuildStepsList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            IsPlaybackOpen = DrawFoldoutBlock("Playback", IsPlaybackOpen, () =>
            {
                EditorGUILayout.PropertyField(playOnEnableProperty);
                EditorGUILayout.PropertyField(restartOnEnableProperty);
            });

            IsSequenceOpen = DrawFoldoutBlock("Sequence", IsSequenceOpen, () =>
            {
                EditorGUILayout.PropertyField(autoKillProperty);
                EditorGUILayout.PropertyField(recyclableProperty);

                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(loopsProperty);
                if (loopsProperty.intValue != 0)
                    EditorGUILayout.PropertyField(loopTypeProperty);

                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(updateTypeProperty);
                EditorGUILayout.PropertyField(ignoreTimeScaleProperty);
                EditorGUILayout.PropertyField(durationMultiplierProperty);
            });

            IsStepsOpen = DrawFoldoutBlock("Steps", IsStepsOpen, () =>
            {
                RebuildStepTimings();
                
                float baseTotal = EstimateTotalSequenceLengthSeconds(stepsProperty);
                float multiplier = Mathf.Max(0f, durationMultiplierProperty.floatValue);

                float effectiveTotal = multiplier <= 0f ? 0f : baseTotal * multiplier;

                EditorGUILayout.LabelField($"Estimated Duration: {effectiveTotal:0.###}s  (base {baseTotal:0.###}s × {multiplier:0.###})",
                                           EditorStyles.miniBoldLabel);
                
                bool showBars = ShouldShowProgressBars();
                if (showBars != lastShowBars)
                {
                    lastShowBars = showBars;
                    BuildStepsList();
                    Repaint();
                }
                
                stepsList.DoLayoutList();
                
                using (new EditorGUI.DisabledScope(clipboard == null))
                {
                    if (GUILayout.Button("Add From Clipboard")) PasteAddStep();
                }
                
                // Repaint while playing so bars animate
                if (Application.isPlaying)
                    Repaint();
            });

            IsEventsOpen = DrawFoldoutBlock("Events", IsEventsOpen, () =>
            {
                EditorGUILayout.PropertyField(onStartProperty);
                EditorGUILayout.PropertyField(onPauseProperty);
                EditorGUILayout.PropertyField(onCompleteProperty);
                EditorGUILayout.PropertyField(onStepCompleteProperty);
                EditorGUILayout.PropertyField(onRewindProperty);
            });

            EditorGUILayout.Space(10);

            DrawBlock("Runtime Controls", DrawRuntimeControls);

            serializedObject.ApplyModifiedProperties();
        }

        private void BuildStepsList()
        {
            stepsList = new ReorderableList(serializedObject, stepsProperty, true, false, true, true)
                {
                    elementHeightCallback = i =>
                    {
                        SerializedProperty elementProperty = stepsProperty.GetArrayElementAtIndex(i);

                        bool showBars = ShouldShowProgressBars();

                        float height = HEADER_HEIGHT;

                        if (showBars)
                        {
                            height += 4f + PROGRESS_BAR_HEIGHT + PROGRESS_BAR_SPACING;
                        }
                        else
                        {
                            height += 6f;
                        }

                        if (!elementProperty.isExpanded)
                            return height;

                        float body = GetManagedRefChildrenHeight(elementProperty, stepExcludeProperties);

                        bool hasAnyPriority = stepPriorityProperties.Any(n => elementProperty.FindPropertyRelative(n) != null);
                        bool hasRemaining = HasAnyDrawableChildren(elementProperty, CombineExcludes(stepExcludeProperties, stepPriorityProperties));

                        if (hasAnyPriority && hasRemaining)
                            body += 6f;

                        body += TYPE_LABEL_HEIGHT + TYPE_LABEL_SPACING;

                        height += body + BODY_PADDING;
                        return height;
                    },
                    drawElementCallback = (r, i, a, f) =>
                    {
                        SerializedProperty elementProperty = stepsProperty.GetArrayElementAtIndex(i);
                        DrawStepBlock(r, i, elementProperty);
                    },
                    onAddDropdownCallback = (buttonRect, list) =>
                    {
                        var dropdown = new TweenStepAdvancedDropdown(addDropdownState, GetConcreteStepTypes(), AddStep);
                        dropdown.Show(buttonRect);
                    },
                    onRemoveCallback = list =>
                    {
                        if (EditorUtility.DisplayDialog("Remove step?", "Remove the selected step?", "Remove", "Cancel"))
                            ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    }
                };
        }

        private void DrawStepBlock(Rect rect, int index, SerializedProperty elementProperty)
        {
            rect.y += 2f;
            rect.height -= 4f;

            var headerRect = new Rect(rect.x, rect.y, rect.width, HEADER_HEIGHT);

            ComputeHeaderLayout(headerRect, out Rect foldRect, out Rect enabledRect,
                                out Rect titleRect, out Rect durationRect, out Rect placementRect);

            HandleHeaderContextClick(headerRect, enabledRect, titleRect, placementRect, index);
            HandleHeaderMouse(headerRect, foldRect, enabledRect, titleRect, durationRect, placementRect, index, elementProperty);

            DrawHeaderBackground(headerRect, index == selectedIndex);
            DrawHeaderTooltip(headerRect, elementProperty);
            DrawHeaderControls(elementProperty, foldRect, enabledRect, titleRect, durationRect, placementRect);

            bool showBars = ShouldShowProgressBars();

            Rect barRect = default;
            if (showBars)
            {
                barRect = GetProgressBarSlotRect(rect);
                DrawProgressBar(barRect, index);
            }

            if (!elementProperty.isExpanded)
                return;

            float bodyTop = showBars ? (barRect.yMax + PROGRESS_BAR_SPACING) : (rect.y + HEADER_HEIGHT + 4f);
            var bodyRect = new Rect(rect.x, bodyTop, rect.width, rect.height - (bodyTop - rect.y));

            bodyRect.x += BODY_PADDING;
            bodyRect.width -= BODY_PADDING * 2f;
            bodyRect.y += 2f;

            string typeName = ShortTypeName(elementProperty.managedReferenceFullTypename);
            string niceTypeName = ObjectNames.NicifyVariableName(StripStepSuffix(typeName));
            string tooltip = elementProperty.managedReferenceFullTypename ?? "";

            var typeRect = new Rect(bodyRect.x, bodyRect.y, bodyRect.width, TYPE_LABEL_HEIGHT);
            EditorGUI.LabelField(typeRect, new GUIContent($"Type: {niceTypeName}", tooltip), EditorStyles.miniBoldLabel);

            float nextY = typeRect.yMax + TYPE_LABEL_SPACING;
            float deltaY = nextY - bodyRect.y;

            bodyRect.y = nextY;
            bodyRect.height = Mathf.Max(0f, bodyRect.height - deltaY);

            DrawManagedRefChildrenPrioritized(bodyRect, elementProperty, stepPriorityProperties, stepExcludeProperties);
        }

        private void HandleHeaderMouse(Rect headerRect, Rect foldRect, Rect enabledRect,
                                       Rect titleRect, Rect durationRect, Rect placementRect,
                                       int index, SerializedProperty element)
        {
            Event e = Event.current;
            
            if (e.type != EventType.MouseDown)
                return;
            
            if (!headerRect.Contains(e.mousePosition)) 
                return;

            selectedIndex = index;

            if (e.button == 0 && foldRect.Contains(e.mousePosition))
            {
                element.isExpanded = !element.isExpanded;
                e.Use();
                return;
            }

            bool onControl = enabledRect.Contains(e.mousePosition) ||
                             titleRect.Contains(e.mousePosition) ||
                             durationRect.Contains(e.mousePosition) ||
                             placementRect.Contains(e.mousePosition);

            if (onControl)
                return;

            if (e.button != 0)
                return;

            element.isExpanded = !element.isExpanded;
            e.Use();
        }
        
        private void HandleHeaderContextClick(Rect headerRect, Rect enabledRect, Rect titleRect, 
                                              Rect placementRect, int index)
        {
            Event e = Event.current;

            if (e.type != EventType.ContextClick)
                return;

            if (!headerRect.Contains(e.mousePosition))
                return;

            // Do NOT open the step menu when hovering controls
            bool onControl = enabledRect.Contains(e.mousePosition) ||
                             titleRect.Contains(e.mousePosition) ||
                             placementRect.Contains(e.mousePosition);

            if (onControl)
                return;

            selectedIndex = index;
            ShowStepContextMenu(index);
            e.Use();
        }

        private void DrawHeaderControls(SerializedProperty element, Rect foldRect, Rect enabledRect,
                                        Rect titleRect, Rect durationRect, Rect placementRect)
        {
            SerializedProperty enabledProperty = element.FindPropertyRelative("enabled");
            SerializedProperty titleProperty = element.FindPropertyRelative("title");
            SerializedProperty placementProperty = element.FindPropertyRelative("placement");

            GUIContent foldContent = element.isExpanded
                ? EditorGUIUtility.IconContent("IN Foldout on")
                : EditorGUIUtility.IconContent("IN Foldout");

            if (foldContent == null || foldContent.image == null)
                foldContent = new GUIContent(element.isExpanded ? "▼" : "▶");

            GUI.Label(foldRect, foldContent, GUIStyle.none);
            
            Event e = Event.current;
            if (e.type == EventType.MouseDown && foldRect.Contains(e.mousePosition))
            {
                if (e.button == 0)
                {
                    element.isExpanded = !element.isExpanded;
                    GUI.changed = true;
                    e.Use();
                }
            }

            if (enabledProperty != null)
                enabledProperty.boolValue = EditorGUI.Toggle(enabledRect, enabledProperty.boolValue);

            string typeName = ShortTypeName(element.managedReferenceFullTypename);
            string defaultName = ObjectNames.NicifyVariableName(StripStepSuffix(typeName));

            string current = titleProperty != null ? titleProperty.stringValue : string.Empty;
            bool hasCustom = !string.IsNullOrWhiteSpace(current);

            if (titleProperty != null)
            {
                GUI.SetNextControlName($"step_title_{element.propertyPath}");
                EditorGUI.BeginChangeCheck();
                string shown = hasCustom ? current : defaultName;
                string newTitle = EditorGUI.TextField(titleRect, shown);
                if (EditorGUI.EndChangeCheck())
                    titleProperty.stringValue = (newTitle == defaultName) ? string.Empty : newTitle;

                if (!hasCustom)
                {
                    Color color = GUI.color;
                    GUI.color = new Color(0f, 0f, 0f, 0.35f);
                    GUI.Label(new Rect(titleRect.x + 3f, titleRect.y + 1f, titleRect.width, titleRect.height), defaultName);
                    GUI.color = color;
                }
            }
            else
            {
                EditorGUI.LabelField(titleRect, defaultName, EditorStyles.boldLabel);
            }

            var durStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleRight };
            
            float stepLen = GetStepLengthSeconds(element);
            float mult = durationMultiplierProperty?.floatValue ?? 1f;
            float eff = mult <= 0f ? 0f : stepLen * mult;

            EditorGUI.LabelField(durationRect, $"[{eff:0.###}s]", durStyle);

            if (placementProperty != null)
            {
                int newIndex = EditorGUI.Popup(placementRect, placementProperty.enumValueIndex, placementProperty.enumDisplayNames);
                placementProperty.enumValueIndex = newIndex;
            }
        }
        
        private bool ShouldShowProgressBars()
        {
            if (!Application.isPlaying)
                return false;

            var sequencer = (DOTweenSequencer)target;
            return sequencer != null && sequencer.HasSequence;
        }
        
        private void DrawProgressBar(Rect barRect, int index)
        {
            if (!Application.isPlaying)
                return;

            var sequencer = (DOTweenSequencer)target;
            if (sequencer == null || !sequencer.HasSequence)
                return;

            if (index < 0 || index >= stepTimings.Count)
                return;

            StepTiming timing = stepTimings[index];
            float time = sequencer.EditorElapsed;

            // Colors
            Color backgroundColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.10f) : new Color(0f, 0f, 0f, 0.10f);
            Color fillerColor     = EditorGUIUtility.isProSkin ? new Color(0.30f, 0.65f, 1.00f, 0.85f) : new Color(0.20f, 0.45f, 0.90f, 0.85f);

            Color delayBackground = EditorGUIUtility.isProSkin ? new Color(1f, 0.85f, 0.20f, 0.18f) : new Color(1f, 0.75f, 0.10f, 0.22f);
            Color delayFiller     = EditorGUIUtility.isProSkin ? new Color(1f, 0.70f, 0.10f, 0.75f) : new Color(0.95f, 0.60f, 0.05f, 0.80f);

            // Always draw base background
            EditorGUI.DrawRect(barRect, backgroundColor);

            float totalLength = Mathf.Max(0f, timing.End - timing.ScheduledStart);
            float delayLength = Mathf.Max(0f, timing.DelayEnd - timing.ScheduledStart);
            float durationLength = Mathf.Max(0f, timing.End - timing.DelayEnd);

            // Special case: zero-length step -> show "instant" completion as a tiny fill once reached
            if (totalLength <= 0f)
            {
                // If we've reached/passed the scheduled start, show it as complete
                if (time >= timing.ScheduledStart)
                {
                    // Fill the whole bar, or just a tiny tick if you prefer
                    EditorGUI.DrawRect(barRect, fillerColor);
                }

                DrawProgressBarBorder(barRect);
                return;
            }

            // Delay region background tint
            if (delayLength > 0f)
            {
                float delayWidth = barRect.width * Mathf.Clamp01(delayLength / totalLength);
                var delayRect = new Rect(barRect.x, barRect.y, delayWidth, barRect.height);
                EditorGUI.DrawRect(delayRect, delayBackground);
            }

            // Delay fill
            if (delayLength > 0f)
            {
                float delayFill = Mathf.Clamp01((time - timing.ScheduledStart) / delayLength);
                float delaySegmentW = barRect.width * Mathf.Clamp01(delayLength / totalLength);
                float w = delaySegmentW * delayFill;

                if (w > 0f)
                    EditorGUI.DrawRect(new Rect(barRect.x, barRect.y, w, barRect.height), delayFiller);
            }

            // Duration fill
            {
                float delaySegmentW = barRect.width * Mathf.Clamp01(delayLength / totalLength);
                float durSegmentW = barRect.width - delaySegmentW;

                float durFill = (durationLength <= 0f)
                    ? (time >= timing.DelayEnd ? 1f : 0f)
                    : Mathf.Clamp01((time - timing.DelayEnd) / durationLength);

                float w = durSegmentW * durFill;
                if (w > 0f)
                    EditorGUI.DrawRect(new Rect(barRect.x + delaySegmentW, barRect.y, w, barRect.height), fillerColor);
            }

            DrawProgressBarBorder(barRect);
        }
        
        private void ShowStepContextMenu(int index)
        {
            var menu = new GenericMenu();

            bool hasStep = index >= 0 && index < stepsProperty.arraySize;
            bool canPaste = clipboard != null && !string.IsNullOrEmpty(clipboard.AssemblyQualifiedTypeName);

            if (!hasStep)
            {
                menu.AddDisabledItem(new GUIContent("No Step"));
                menu.ShowAsContext();
                return;
            }
            
            menu.AddItem(new GUIContent("Duplicate"), false, () => DuplicateStep(index));
            menu.AddSeparator("");
            
            menu.AddItem(new GUIContent("Copy"), false, () => CopyStep(index));
            
            if (canPaste)
            {
                menu.AddItem(new GUIContent("Paste (Replace)"), false, () => PasteStepReplace(index));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste (Replace)"));
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Delete"), false, () => DeleteStep(index));

            menu.ShowAsContext();
        }

        private void RebuildStepTimings()
        {
            stepTimings.Clear();

            if (stepsProperty == null || !stepsProperty.isArray)
                return;

            float cursor = 0f;
            float lastAppendStart = 0f;

            for (int i = 0; i < stepsProperty.arraySize; i++)
            {
                SerializedProperty stepProperty = stepsProperty.GetArrayElementAtIndex(i);

                SerializedProperty placementProperty = stepProperty.FindPropertyRelative("placement");
                TweenPlacement placement = (TweenPlacement)(placementProperty?.enumValueIndex ?? 0);

                float delay = (stepProperty.managedReferenceValue == null) ? 0f : GetStepDelaySeconds(stepProperty);
                float duration = (stepProperty.managedReferenceValue == null) ? 0f : GetStepDurationSeconds(stepProperty);

                float scheduledStart = placement == TweenPlacement.Join ? lastAppendStart : cursor;
                float occupiedLength = delay + duration;

                if (placement == TweenPlacement.Join)
                {
                    cursor = Mathf.Max(cursor, scheduledStart + occupiedLength);
                }
                else
                {
                    lastAppendStart = scheduledStart;
                    cursor = scheduledStart + occupiedLength;
                }

                float delayEnd = scheduledStart + delay;
                float end = delayEnd + duration;

                stepTimings.Add(new StepTiming
                {
                    ScheduledStart = scheduledStart,
                    DelayEnd = delayEnd,
                    End = end
                });
            }
        }

        private void DuplicateStep(int index)
        {
            serializedObject.Update();

            SerializedProperty sourceProperty = stepsProperty.GetArrayElementAtIndex(index);

            List<ObjectRefEntry> objectRefs = ExtractObjectRefs(sourceProperty);

            object copy = CreateDeepCopyFromProperty(sourceProperty);
            if (copy == null)
            {
                Debug.LogWarning("Duplicate failed: couldn't copy step.");
                return;
            }

            int insertIndex = index + 1;

            stepsProperty.InsertArrayElementAtIndex(insertIndex);

            SerializedProperty destinationProperty = stepsProperty.GetArrayElementAtIndex(insertIndex);
            destinationProperty.managedReferenceValue = null;
            destinationProperty.managedReferenceValue = copy;
            destinationProperty.isExpanded = true;

            ApplyObjectRefs(destinationProperty, objectRefs);

            selectedIndex = insertIndex;

            serializedObject.ApplyModifiedProperties();
        }

        private void CopyStep(int index)
        {
            serializedObject.Update();

            SerializedProperty source = stepsProperty.GetArrayElementAtIndex(index);
            object obj = source.managedReferenceValue;
            if (obj == null)
                return;

            var data = new TweenStepClipboardData
            {
                AssemblyQualifiedTypeName = obj.GetType().AssemblyQualifiedName,
                Json = EditorJsonUtility.ToJson(obj),
                ObjectRefs = ExtractObjectRefs(source)
            };

            clipboard = data;
        }

        private void PasteAddStep()
        {
            if (clipboard == null || string.IsNullOrEmpty(clipboard.AssemblyQualifiedTypeName))
                return;

            Type type = Type.GetType(clipboard.AssemblyQualifiedTypeName);
            if (type == null)
            {
                Debug.LogWarning($"Paste failed: type not found '{clipboard.AssemblyQualifiedTypeName}'.");
                return;
            }

            object instance = Activator.CreateInstance(type);
            EditorJsonUtility.FromJsonOverwrite(clipboard.Json, instance);

            serializedObject.Update();

            int index = stepsProperty.arraySize;
            stepsProperty.arraySize++; // safer for SerializeReference than InsertArrayElementAtIndex

            SerializedProperty elementProperty = stepsProperty.GetArrayElementAtIndex(index);
            bool wasExpanded = elementProperty.isExpanded;
            elementProperty.managedReferenceValue = instance;
            elementProperty.isExpanded = wasExpanded;

            ApplyObjectRefs(elementProperty, clipboard.ObjectRefs);

            selectedIndex = index;

            serializedObject.ApplyModifiedProperties();
        }
        
        private void PasteStepReplace(int index)
        {
            if (clipboard == null)
                return;
            
            if (index < 0 || index >= stepsProperty.arraySize)
                return;

            Type type = Type.GetType(clipboard.AssemblyQualifiedTypeName);
            if (type == null)
            {
                Debug.LogWarning($"Paste failed: type not found '{clipboard.AssemblyQualifiedTypeName}'.");
                return;
            }

            object instance = Activator.CreateInstance(type);
            EditorJsonUtility.FromJsonOverwrite(clipboard.Json, instance);

            serializedObject.Update();

            SerializedProperty elementProperty = stepsProperty.GetArrayElementAtIndex(index);
            bool wasExpanded = elementProperty.isExpanded;
            elementProperty.managedReferenceValue = instance;
            elementProperty.isExpanded = wasExpanded;

            ApplyObjectRefs(elementProperty, clipboard.ObjectRefs);
            
            selectedIndex = index;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRuntimeControls()
        {
            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Runtime controls are available in Play Mode.", MessageType.None);

            var sequencer = (DOTweenSequencer)target;
            
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Play Forward")) sequencer.Play();
                    if (GUILayout.Button("Play Backwards")) sequencer.PlayBackwards();
                    if (GUILayout.Button("Pause")) sequencer.Pause();
                    if (GUILayout.Button("Resume")) sequencer.Resume();
                }
            }
        }

        private void AddStep(Type type)
        {
            serializedObject.Update();

            int index = stepsProperty.arraySize;
            stepsProperty.InsertArrayElementAtIndex(index);
            SerializedProperty elementProperty = stepsProperty.GetArrayElementAtIndex(index);
            elementProperty.managedReferenceValue = Activator.CreateInstance(type);

            elementProperty.isExpanded = true;
            selectedIndex = index;

            serializedObject.ApplyModifiedProperties();
        }

        private void DeleteStep(int index)
        {
            serializedObject.Update();

            // Important for managedReference arrays:
            // must set managedReferenceValue = null before deleting, otherwise Unity can keep stale refs.
            SerializedProperty elementProperty = stepsProperty.GetArrayElementAtIndex(index);
            elementProperty.managedReferenceValue = null;

            stepsProperty.DeleteArrayElementAtIndex(index);

            selectedIndex = Mathf.Clamp(selectedIndex, -1, stepsProperty.arraySize - 1);

            serializedObject.ApplyModifiedProperties();
        }
        
        private static TweenStepClipboardData clipboard;
        private static Type[] stepTypesCache;
        
        private static readonly string[] stepExcludeProperties =
        {
            "enabled",
            "title",
            "placement"
        };

        private static readonly string[] stepPriorityProperties =
        {
            "target",
            "delay",
            "duration"
        };
        
        private static bool IsPlaybackOpen
        {
            get => EditorPrefs.GetBool("DOTweenSequencerEditor_IsPlaybackOpen", true);
            set => EditorPrefs.SetBool("DOTweenSequencerEditor_IsPlaybackOpen", value);
        }
        
        private static bool IsSequenceOpen 
        {
            get => EditorPrefs.GetBool("DOTweenSequencerEditor_IsSequenceOpen", true);
            set => EditorPrefs.SetBool("DOTweenSequencerEditor_IsSequenceOpen", value);
        }
        
        private static bool IsStepsOpen
        {
            get => EditorPrefs.GetBool("DOTweenSequencerEditor_IsStepsOpen", true);
            set => EditorPrefs.SetBool("DOTweenSequencerEditor_IsStepsOpen", value);
        }
        
        private static bool IsEventsOpen
        {
            get => EditorPrefs.GetBool("DOTweenSequencerEditor_IsEventsOpen", false);
            set => EditorPrefs.SetBool("DOTweenSequencerEditor_IsEventsOpen", value);
        }

        private static void DrawBlock(string title, Action drawContents)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var headerRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel, 
                                                      GUILayout.Height(EditorGUIUtility.singleLineHeight));

            const float leftPadding = 0f;
            const float rightPadding = 6f;
            
            headerRect.x += leftPadding;
            headerRect.width -= leftPadding + rightPadding;

            EditorGUI.LabelField(headerRect, title, EditorStyles.boldLabel);

            EditorGUILayout.Space(4);
            drawContents?.Invoke();
            EditorGUILayout.Space(2);

            EditorGUILayout.EndVertical();
        }
        
        private static bool DrawFoldoutBlock(string title, bool state, Action drawContents)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var headerRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel, 
                                                      GUILayout.Height(EditorGUIUtility.singleLineHeight));

            const float leftPadding = 12f;
            const float rightPadding = 6f;
            
            headerRect.x += leftPadding;
            headerRect.width -= leftPadding + rightPadding;

            state = EditorGUI.Foldout(headerRect, state, title, true, EditorStyles.boldLabel);

            if (state)
            {
                EditorGUILayout.Space(4);
                drawContents?.Invoke();
                EditorGUILayout.Space(2);
            }

            if (headerRect.Contains(Event.current.mousePosition))
            {
                EditorGUI.DrawRect(new Rect(headerRect.x - leftPadding, headerRect.y, headerRect.width + leftPadding, headerRect.height),
                                   new Color(1f, 1f, 1f, 0.035f));
            }
            
            EditorGUILayout.EndVertical();

            return state;
        }
        
        private static void ComputeHeaderLayout(Rect headerRect, out Rect foldRect, out Rect enabledRect,
                                                out Rect titleRect, out Rect durationRect, out Rect placementRect)
        {
            float x = headerRect.x + HEADER_PADDING;
            float y = headerRect.y + 5f;

            foldRect = new Rect(x, y + 1f, 18f, 18f);
            x += 22f;

            enabledRect = new Rect(x, y + 1f, 18f, 18f);
            x += 24f;

            float rightPadding = 8f;
            float durationWidth = 60f;
            float placementWidth = 110f;

            durationRect = new Rect(headerRect.xMax - rightPadding - durationWidth, y, durationWidth, 18f);

            placementRect = new Rect(durationRect.x - 6f - placementWidth, y, placementWidth, 18f);

            float titleRight = placementRect.x - 8f;
            titleRect = new Rect(x, y, Mathf.Max(60f, titleRight - x), 18f);
        }
        
        private static Rect GetProgressBarSlotRect(Rect elementRect)
        {
            float x = elementRect.x + BODY_PADDING;
            float w = elementRect.width - BODY_PADDING * 2f;

            float y = elementRect.y + HEADER_HEIGHT + 4f;
            return new Rect(x, y, w, PROGRESS_BAR_HEIGHT);
        }
        
        private static void DrawHeaderBackground(Rect rect, bool selected)
        {
            Color previousColor = GUI.color;
            GUI.color = selected ? new Color(0.82f, 0.88f, 1f, 1f) : new Color(1f, 1f, 1f, 1f);
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            GUI.color = previousColor;
        }
        
        private static void DrawHeaderTooltip(Rect headerRect, SerializedProperty elementProperty)
        {
            string full = elementProperty.managedReferenceFullTypename ?? "";
            string shortName = ShortTypeName(full);
            string nice = ObjectNames.NicifyVariableName(StripStepSuffix(shortName));

            string tooltip = $"{nice}\n\n({full})";

            GUI.Label(headerRect, new GUIContent(string.Empty, tooltip));
        }
        
        private static void DrawProgressBarBorder(Rect barRect)
        {
            var border = EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.35f) : new Color(0f, 0f, 0f, 0.25f);
            EditorGUI.DrawRect(new Rect(barRect.x, barRect.y, barRect.width, 1f), border);
            EditorGUI.DrawRect(new Rect(barRect.x, barRect.yMax - 1f, barRect.width, 1f), border);
            EditorGUI.DrawRect(new Rect(barRect.x, barRect.y, 1f, barRect.height), border);
            EditorGUI.DrawRect(new Rect(barRect.xMax - 1f, barRect.y, 1f, barRect.height), border);
        }
        
        private static void DrawManagedRefChildrenPrioritized(Rect rect, SerializedProperty managedRefProperty, string[] priorityNames,
                                                              string[] excludeNames)
        {
            float y = rect.y;

            bool drewAnyPriority = false;

            // 1. Draw priority fields in the desired order
            if (priorityNames != null)
            {
                foreach (string name in priorityNames)
                {
                    if (string.IsNullOrEmpty(name))
                        continue;
                    
                    if (excludeNames != null && excludeNames.Contains(name)) 
                        continue;

                    SerializedProperty property = managedRefProperty.FindPropertyRelative(name);
                    
                    if (property == null) 
                        continue;

                    float h = EditorGUI.GetPropertyHeight(property, true);
                    EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, h), property, true);
                    y += h + 2f;
                    drewAnyPriority = true;
                }
            }

            // 2. Draw the rest (excluding priority + excludeNames)
            string[] combinedExclude = CombineExcludes(excludeNames, priorityNames);

            // Before drawing the remaining properties, check if any remain;
            // if yes and we drew priority fields, add the requested space.
            bool hasRemaining = HasAnyDrawableChildren(managedRefProperty, combinedExclude);

            if (drewAnyPriority && hasRemaining)
                y += 6f;

            DrawManagedRefChildren(new Rect(rect.x, y, rect.width, rect.height - (y - rect.y)),
                                   managedRefProperty, combinedExclude);
        }
        
        private static void DrawManagedRefChildren(Rect rect, SerializedProperty managedRefProperty, string[] excludeNames)
        {
            SerializedProperty property = managedRefProperty.Copy();
            SerializedProperty endProperty = property.GetEndProperty();

            bool enterChildren = true;
            float y = rect.y;

            while (property.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property, endProperty))
            {
                enterChildren = false;

                if (property.depth <= managedRefProperty.depth)
                    continue;

                if (excludeNames != null && excludeNames.Contains(property.name))
                    continue;

                float h = EditorGUI.GetPropertyHeight(property, true);
                var r = new Rect(rect.x, y, rect.width, h);
                EditorGUI.PropertyField(r, property, true);
                y += h + 2f;
            }
        }

        private static string[] CombineExcludes(string[] a, string[] b)
        {
            if (a == null || a.Length == 0) 
                return b ?? Array.Empty<string>();
            
            if (b == null || b.Length == 0) 
                return a;

            return a.Concat(b).Distinct().ToArray();
        }

        private static bool HasAnyDrawableChildren(SerializedProperty managedRefProperty, string[] excludeNames)
        {
            SerializedProperty property = managedRefProperty.Copy();
            SerializedProperty endProperty = property.GetEndProperty();

            bool enterChildren = true;

            while (property.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property, endProperty))
            {
                enterChildren = false;

                if (property.depth <= managedRefProperty.depth)
                    continue;

                if (excludeNames != null && excludeNames.Contains(property.name))
                    continue;

                return true;
            }

            return false;
        }
        
        private static float GetManagedRefChildrenHeight(SerializedProperty managedRefProperty, string[] excludeNames)
        {
            SerializedProperty property = managedRefProperty.Copy();
            SerializedProperty endProperty = property.GetEndProperty();

            bool enterChildren = true;
            float total = 0f;

            while (property.NextVisible(enterChildren) && !SerializedProperty.EqualContents(property, endProperty))
            {
                enterChildren = false;

                if (property.depth <= managedRefProperty.depth)
                    continue;

                if (excludeNames != null && excludeNames.Contains(property.name))
                    continue;

                total += EditorGUI.GetPropertyHeight(property, true) + 2f;
            }

            return total;
        }
        
        private static object CreateDeepCopyFromProperty(SerializedProperty sourceProperty)
        {
            object obj = sourceProperty.managedReferenceValue;
            
            if (obj == null) 
                return null;

            Type type = obj.GetType();
            object instance = Activator.CreateInstance(type);
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(obj), instance);
            return instance;
        }

        private static List<ObjectRefEntry> ExtractObjectRefs(SerializedProperty root)
        {
            var result = new List<ObjectRefEntry>();

            SerializedProperty it = root.Copy();
            SerializedProperty end = it.GetEndProperty();

            bool enterChildren = true;
            while (it.NextVisible(enterChildren) && !SerializedProperty.EqualContents(it, end))
            {
                enterChildren = false;

                if (it.propertyType == SerializedPropertyType.ObjectReference)
                {
                    // Relative path from the step root, so we can FindPropertyRelative on paste
                    string rel = GetRelativePath(root.propertyPath, it.propertyPath);
                    if (!string.IsNullOrEmpty(rel))
                    {
                        result.Add(new ObjectRefEntry
                        {
                            RelativePath = rel,
                            Value = it.objectReferenceValue
                        });
                    }
                }
            }

            return result;
        }
        
        private static void ApplyObjectRefs(SerializedProperty root, List<ObjectRefEntry> references)
        {
            if (references == null || references.Count == 0)
                return;

            foreach (ObjectRefEntry reference in references)
            {
                if (string.IsNullOrEmpty(reference.RelativePath))
                    continue;

                SerializedProperty property = root.FindPropertyRelative(reference.RelativePath);
                if (property == null)
                    continue;

                if (property.propertyType != SerializedPropertyType.ObjectReference)
                    continue;

                property.objectReferenceValue = reference.Value;
            }
        }

        private static string GetRelativePath(string rootPath, string childPath)
        {
            // childPath starts with rootPath + "."
            if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(childPath))
                return null;

            if (childPath == rootPath)
                return ""; // root itself

            if (!childPath.StartsWith(rootPath, StringComparison.Ordinal))
                return null;

            int start = rootPath.Length;
            if (childPath.Length > start && childPath[start] == '.')
                start++;

            return childPath.Substring(start);
        }

        private static float GetStepDelaySeconds(SerializedProperty stepProperty)
        {
            return Mathf.Max(0f, GetFloatOrInt(stepProperty.FindPropertyRelative("delay")));
        }

        private static float GetStepDurationSeconds(SerializedProperty stepProperty)
        {
            return Mathf.Max(0f, GetFloatOrInt(stepProperty.FindPropertyRelative("duration")));
        }

        private static float GetStepLengthSeconds(SerializedProperty stepProperty)
        {
            // Convention: delay + duration
            float delay = GetStepDelaySeconds(stepProperty);
            float duration = GetStepDurationSeconds(stepProperty);

            // Some steps might not have "delay" or "duration" -> treat missing as 0
            float len = Mathf.Max(0f, delay) + Mathf.Max(0f, duration);
            return len;
        }

        private static float GetFloatOrInt(SerializedProperty property)
        {
            if (property == null) 
                return 0f;
            
            return property.propertyType switch
            {
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.Integer => property.intValue,
                _ => 0f
            };
        }

        private static float EstimateTotalSequenceLengthSeconds(SerializedProperty stepsArrayProperty)
        {
            if (stepsArrayProperty == null || !stepsArrayProperty.isArray) 
                return 0f;

            float cursor = 0f;
            float lastAppendStart = 0f;

            for (int i = 0; i < stepsArrayProperty.arraySize; i++)
            {
                SerializedProperty step = stepsArrayProperty.GetArrayElementAtIndex(i);
                if (step.managedReferenceValue == null)
                    continue;

                float len = GetStepLengthSeconds(step);
                if (len <= 0f)
                    continue;

                SerializedProperty placementProperty = step.FindPropertyRelative("placement");
                TweenPlacement placement = (TweenPlacement) (placementProperty?.enumValueIndex ?? 0);

                switch (placement)
                {
                    case TweenPlacement.Join:
                    {
                        float start = lastAppendStart;
                        cursor = Mathf.Max(cursor, start + len);
                        break;
                    }
                    case TweenPlacement.Append:
                    default:
                    {
                        float start = cursor;
                        lastAppendStart = start;
                        cursor = start + len;
                        break;
                    }
                }
            }

            return Mathf.Max(0f, cursor);
        }

        private static Type[] GetConcreteStepTypes()
        {
            if (stepTypesCache != null)
                return stepTypesCache;

            Type stepInterface = typeof(ITweenStep);
            stepTypesCache = TypeCache.GetTypesDerivedFrom(stepInterface)
                .Where(t => !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null)
                .ToArray();

            return stepTypesCache;
        }

        private static string ShortTypeName(string managedReferenceFullTypename)
        {
            if (string.IsNullOrEmpty(managedReferenceFullTypename)) 
                return "Step";
            
            string[] parts = managedReferenceFullTypename.Split(' ');
            if (parts.Length != 2) 
                return "Step";
            
            string typeName = parts[1];
            int lastDotIndex = typeName.LastIndexOf('.');
            return lastDotIndex >= 0 ? typeName.Substring(lastDotIndex + 1) : typeName;
        }

        private static string StripStepSuffix(string name)
        {
            if (string.IsNullOrEmpty(name)) 
                return name;
            
            return name.EndsWith("Step", StringComparison.Ordinal) ? name.Substring(0, name.Length - 4) : name;
        }
        
        private struct StepTiming
        {
            public float ScheduledStart;
            public float DelayEnd;
            public float End;
        }
    }
}