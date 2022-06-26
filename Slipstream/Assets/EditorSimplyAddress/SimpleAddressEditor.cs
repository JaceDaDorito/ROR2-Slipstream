using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
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
    [CustomEditor(typeof(SimpleAddress), true)]
    public class SimpleAddressEditor : Editor
    {
        protected virtual bool CanUseAddress() => true;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            var iterator = serializedObject.GetIterator().Copy();
            var children = iterator.GetVisibleChildren().Where(child => child.name != "m_Script").ToArray();
            foreach (var child in children)
            {
                if (child.isArray && child.arrayElementType != "char")
                {
                    var rList = new ReorderableList(child.serializedObject, child, true, true, true, true);

                    void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) => EditorGUI.PropertyField(rect, rList.serializedProperty.GetArrayElementAtIndex(index));
                    void OnHeader(Rect rect) => GUI.Label(rect, child.displayName);

                    rList.drawHeaderCallback += OnHeader;
                    rList.drawElementCallback += OnDrawElement;

                    var imguiContainer = new IMGUIContainer(() =>
                    {
                        serializedObject.Update();
                        rList.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                    });
                    root.Add(imguiContainer);
                }
                else
                    root.Add(new PropertyField(child));
            }
            return root;
        }
    }
}