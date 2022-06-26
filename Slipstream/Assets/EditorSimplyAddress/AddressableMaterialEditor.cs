using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.EditorGUI;
using UnityObject = UnityEngine.Object;
/**
 * MIT License
 * 
 * Copyright (c) 2022 PassivePicasso
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace PassivePicasso.SimplyAddress
{
    [CustomEditor(typeof(AddressableMaterial), true)]
    public class AddressableMaterialEditor : Editor
    {
        Editor materialEditor;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var children = serializedObject.GetIterator().GetVisibleChildren();
            var addressProperty = serializedObject.FindProperty(nameof(SimpleAddress.Address));
            var targetComponentsProperty = serializedObject.FindProperty(nameof(AddressableMaterial.TargetComponents));
            var addMat = target as AddressableMaterial;

            var rList = new ReorderableList(serializedObject, targetComponentsProperty, true, true, true, true);
            rList.drawElementCallback += OnDrawElement;
            void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                var element = rList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element);
            }

            targetComponentsProperty.isExpanded = EditorGUILayout.Foldout(targetComponentsProperty.isExpanded, "Target Components");
            if (targetComponentsProperty.isExpanded)
            {
                rList.DoLayoutList();
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            EditorGUILayout.PropertyField(addressProperty);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            if (!addMat.TargetComponents.Any(tc => tc.gameObject == addMat.gameObject))
                using (new DisabledScope(true))
                {
                    CustomDrawFoldoutInspector(addMat.AssetInstance, ref materialEditor);
                }

            serializedObject.ApplyModifiedProperties();
        }

        public void CustomDrawFoldoutInspector(UnityObject target, ref Editor editor)
        {
            if (editor != null && (editor.target != target || target == null))
            {
                UnityObject.DestroyImmediate(editor);
                editor = null;
            }

            if (editor == null && target != null)
            {
                editor = CreateEditor(target);
            }

            if (!(editor == null))
            {
                editor.DrawHeader();
                editor.OnInspectorGUI();
            }
        }
    }
}