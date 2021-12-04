using RoR2EditorKit.Settings;
using UnityEditor;
using System.Linq;
using UnityEngine;

namespace RoR2EditorKit.Core.Inspectors
{
    public abstract class ExtendedInspector : Editor
    {
        public static RoR2EditorKitSettings Settings { get => RoR2EditorKitSettings.GetOrCreateSettings<RoR2EditorKitSettings>(); }

        public EnabledAndDisabledInspectorsSettings.InspectorSetting InspectorSetting
        {
            get
            {
                if(_inspectorSetting == null)
                {
                    var setting = Settings.InspectorSettings.GetOrCreateInspectorSetting(GetType());
                    _inspectorSetting = setting;
                }
                return _inspectorSetting;
            }
            set
            {
                if(_inspectorSetting != value)
                {
                    var index = Settings.InspectorSettings.EnabledInspectors.IndexOf(_inspectorSetting);
                    Settings.InspectorSettings.EnabledInspectors[index] = value;
                }
                _inspectorSetting = value;
            }
        }

        private EnabledAndDisabledInspectorsSettings.InspectorSetting _inspectorSetting;

        public bool InspectorEnabled { get => InspectorSetting.isEnabled; set => InspectorSetting.isEnabled = value; }

        private void OnEnable()
        {
            InspectorEnabled = InspectorSetting.isEnabled;
            finishedDefaultHeaderGUI += DrawEnableToggle;
        }
        private void OnDisable() => finishedDefaultHeaderGUI -= DrawEnableToggle;

        private void DrawEnableToggle(Editor obj)
        {
            if(obj is ExtendedInspector extendedInspector)
            {
                InspectorEnabled = EditorGUILayout.ToggleLeft($"Enable {ObjectNames.NicifyVariableName(extendedInspector.target.GetType().Name)} Inspector", InspectorEnabled);
            }
        }

        public override void OnInspectorGUI()
        {
            if (!InspectorEnabled)
            {
                DrawDefaultInspector();
            }
        }
    }
}
