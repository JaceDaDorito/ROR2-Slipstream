using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [InitializeOnLoad]
    [CustomEditor(typeof(AddressableSkybox), true)]
    public class AddressableSkyboxEditor : Editor
    {
        [InitializeOnLoadMethod]
        static void InitializeSkyboxSystem()
        {
            EditorSceneManager.sceneSaving += SceneSaving;
            EditorSceneManager.sceneSaved += SceneSaved;
        }

        static void SceneSaved(Scene scene)
        {
            var addressableSkybox = scene.GetRootGameObjects().SelectMany(rgo => rgo.GetComponentsInChildren<AddressableSkybox>()).FirstOrDefault();
            if (!addressableSkybox) return;
            RenderSettings.skybox = addressableSkybox.material;
        }

        static void SceneSaving(Scene scene, string path)
        {
            var addressableSkybox = scene.GetRootGameObjects().SelectMany(rgo => rgo.GetComponentsInChildren<AddressableSkybox>()).FirstOrDefault();
            if (!addressableSkybox) return;
            RenderSettings.skybox = null;
        }

        Editor materialEditor;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var children = serializedObject.GetIterator().GetVisibleChildren();
            var addressProperty = serializedObject.FindProperty(nameof(SimpleAddress.Address));
            var addMat = target as AddressableSkybox;

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            EditorGUILayout.PropertyField(addressProperty);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            using (new DisabledScope(true))
            {
                CustomDrawFoldoutInspector(addMat.material, ref materialEditor);
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