using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
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
    public static class AddressablePrefabPreview
    {

        private const string Library = "Library";
        private const string SimplyAddress = "SimplyAddress";
        private const string Previews = "Previews";

        private static string PreviewRoot => Path.Combine(Library, SimplyAddress, Previews);

        public static readonly Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();
        public static readonly Dictionary<string, Texture2D> PreviewCache = new Dictionary<string, Texture2D>();

        public static readonly string[] FindAllFolders = new[] { "Packages", "Assets" };

        [InitializeOnLoadMethod]
        public static void UpdatePreviews()
        {
            EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGUICallback;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUICallback;
        }
        static void ProjectWindowItemOnGUICallback(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!assetPath.EndsWith(".prefab")) return;

            // If we're a directory, ignore it.
            if (AssetDatabase.IsValidFolder(assetPath)) return;

            // If we're not a prefab, ignore it
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (go == null) return;

            // If we're not a variant, ignore it.
            //PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(go);
            //if (prefabAssetType != PrefabAssetType.Variant) return;
            var ap = go.GetComponent<SimpleAddress>();
            if (ap == null) return;
            var smallestSize = Mathf.Min(selectionRect.width, selectionRect.height);
            if (smallestSize == 16)
                return;

            Texture2D texture = null;
            if (PreviewCache.ContainsKey(ap.Address) && PreviewCache[ap.Address])
                texture = PreviewCache[ap.Address];
            else
                _ = RenderIcon(ap.Address);

            if (texture == null) return;

            GUIContent icon = new GUIContent(texture);

            Rect adjustedRect = new Rect(selectionRect.x, selectionRect.y, smallestSize, smallestSize);
            GUI.Label(adjustedRect, icon);
        }

        private static async Task RenderIcon(string address)
        {
            string previewCachePath = Path.Combine(PreviewRoot, $"{address}.png");
            if (File.Exists(previewCachePath))
            {
                var texture = new Texture2D(128, 128);
                texture.LoadImage(File.ReadAllBytes(previewCachePath));
                texture.Apply();
                PreviewCache[address] = texture;
            }

            if (PreviewCache.ContainsKey(address)) return;

            Object result = null;
            if (!PrefabCache.ContainsKey(address))
                try
                {
                    if (!address.EndsWith(".unity"))
                    {
                        var loadOperation = Addressables.LoadAssetAsync<Object>(address);
                        await loadOperation.Task;
                        result = loadOperation.Result;
                        PreviewCache[address] = UpdatePreview(result);
                    }
                }
                catch { }
            else
                result = PrefabCache[address];

            if (result)
                while (AssetPreview.IsLoadingAssetPreviews())
                {
                    await Task.Delay(500);
                    var texture = PreviewCache[address] = UpdatePreview(result);
                    EditorApplication.RepaintProjectWindow();
                    if (texture)
                    {
                        var png = texture.EncodeToPNG();
                        var fileName = $"{Path.GetFileName(address)}.png";
                        string addressFolder = Path.GetDirectoryName(address);
                        var finalFolder = Path.Combine(PreviewRoot, addressFolder);
                        Directory.CreateDirectory(finalFolder);
                        var filePath = Path.Combine(finalFolder, fileName);
                        File.WriteAllBytes(filePath, png);
                    }
                }
        }

        private static Texture2D UpdatePreview(Object result)
        {
            Texture2D preview;
            switch (result)
            {
                case GameObject gobj when gobj.GetComponentsInChildren<SkinnedMeshRenderer>().Any()
                                       || gobj.GetComponentsInChildren<SpriteRenderer>().Any()
                                       || gobj.GetComponentsInChildren<MeshRenderer>().Any()
                                       || gobj.GetComponentsInChildren<CanvasRenderer>().Any():
                case Material mat:
                    preview = AssetPreview.GetAssetPreview(result);
                    break;
                default:
                    preview = AssetPreview.GetMiniThumbnail(result);
                    break;
            }

            return preview;
        }
    }
}
