using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TriggerCondition))]
public class TriggerConditionPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var typeProperty = property.FindPropertyRelative("Type");
        var conditionType = (TriggerCondition.EType)typeProperty.enumValueIndex;

        // draw the foldout
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                property.isExpanded, conditionType.ToString());
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // is the property expanded?
        if (property.isExpanded)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, typeProperty);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // draw the float property if appropriate
            if (conditionType == TriggerCondition.EType.Stay)
            {
                var floatProperty = property.FindPropertyRelative("FloatValue");
                EditorGUI.PropertyField(position, floatProperty, new GUIContent("Duration (s)"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            var allTags = UnityEditorInternal.InternalEditorUtility.tags;
            var tagsSelected = new bool[allTags.Length];
            var tagsProperty = property.FindPropertyRelative("Tags");

            // determine which tags are selected
            for(int selectedIndex = 0; selectedIndex < tagsProperty.arraySize; ++selectedIndex)
            {
                var tag = tagsProperty.GetArrayElementAtIndex(selectedIndex).stringValue;

                // check if the tag is selected
                for (int tagIndex = 0; tagIndex < allTags.Length; ++tagIndex)
                {
                    if (allTags[tagIndex] == tag)
                    {
                        tagsSelected[tagIndex] = true;
                        break;
                    }
                }
            }

            // draw the list of tags
            EditorGUI.LabelField(position, "Tags to check", EditorStyles.boldLabel);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            bool tagsModified = false;
            int numTagsSelected = 0;
            for(int tagIndex = 0; tagIndex < allTags.Length; ++tagIndex)
            {
                var wasSelected = tagsSelected[tagIndex];

                // draw the toggle for the tag and update the state
                tagsSelected[tagIndex] = EditorGUI.Toggle(position, new GUIContent(allTags[tagIndex]), tagsSelected[tagIndex]);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // check if the tag was modified
                tagsModified |= wasSelected != tagsSelected[tagIndex];

                // track how many tags are selected
                if (tagsSelected[tagIndex])
                    numTagsSelected++;
            }

            // rebuild the tags array
            if (tagsModified)
            {
                tagsProperty.arraySize = numTagsSelected;

                // update the tags list
                int writeIndex = 0;
                for (int tagIndex = 0; tagIndex < allTags.Length; ++tagIndex)
                {
                    if (tagsSelected[tagIndex])
                    {
                        tagsProperty.GetArrayElementAtIndex(writeIndex).stringValue = allTags[tagIndex];
                        ++writeIndex;
                    }
                }
            }

            var onTriggerProperty = property.FindPropertyRelative("OnTrigger");
            EditorGUI.PropertyField(position, onTriggerProperty);
        }

        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var typeProperty = property.FindPropertyRelative("Type");

            // stay property has the time field
            if ((TriggerCondition.EType)typeProperty.enumValueIndex == TriggerCondition.EType.Stay)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            height += UnityEditorInternal.InternalEditorUtility.tags.Length * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            var onTriggerProperty = property.FindPropertyRelative("OnTrigger");
            height += EditorGUI.GetPropertyHeight(onTriggerProperty, true);
        }

        return height;
    }
}
