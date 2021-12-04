using RoR2EditorKit.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace RoR2EditorKit.Core.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(EnabledAndDisabledInspectorsSettings.InspectorSetting))]
    public class InspectorSettingPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var isEnabled = property.FindPropertyRelative("isEnabled");
            var displayName = property.FindPropertyRelative("inspectorName");

            EditorGUI.PropertyField(position, isEnabled, new GUIContent(ObjectNames.NicifyVariableName(displayName.stringValue)));
            
            EditorGUI.EndProperty();
        }
    }
}
