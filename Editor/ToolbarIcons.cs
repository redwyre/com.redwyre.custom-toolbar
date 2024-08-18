using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [InitializeOnLoad]
    public static class ToolbarIcons
    {
        private const string BasePath = "Packages/com.redwyre.custom-toolbar/Editor Default Resources/FontAwesome/";
        static readonly Dictionary<string, Sprite> namedIcons = new();
        static readonly Dictionary<string, Sprite> unitySprites = new();

        static ToolbarIcons()
        {
            Load();
        }

        public static void Load()
        {
            //var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { BasePath });
            //foreach (var spriteGuid in spriteGuids)
            //{
            //    var path = AssetDatabase.GUIDToAssetPath(spriteGuid);
            //    var asset = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            //    var relativePath = path.Replace(BasePath, string.Empty).Replace(".png", string.Empty);
            //    namedIcons.Add(relativePath, asset);
            //}
        }

        public static void Unload()
        {
            foreach (var s in unitySprites.Values)
            {
                Sprite.Destroy(s);
            }
            unitySprites.Clear();
            namedIcons.Clear();
        }

        public static Object? GetIcon(string name)
        {
            if (name.StartsWith("Packages") || name.StartsWith("Assets"))
            {
                return LoadIconAsset(name);
            }

            if (name.StartsWith("fa:", StringComparison.OrdinalIgnoreCase))
            {
                var path = Path.Combine(BasePath, name.Split(':')[1]);
                return LoadIconAsset($"{path}.svg");
            }

            var texture = LoadBuiltInTexture(name);

            if (texture != null)
            {
                return texture;
            }

            Debug.LogWarning($"Unable to find icon {name}");
            return null;
        }

        private static Object? LoadIconAsset(string name)
        {
            var asset = AssetDatabase.LoadAssetAtPath(name, typeof(Object));

            if (asset is Texture2D or Sprite or VectorImage)
            {
                return asset;
            }

            Debug.LogError($"Unable to load asset {name}");
            return null;
        }

        static Texture2D? LoadBuiltInTexture(string name)
        {
            if (EditorGUIUtility.isProSkin)
            {
                name = "d_" + name;
            }

            var texture = EditorGUIUtility.Load(EditorResources.generatedIconsPath + name + ".asset") as Texture2D;

            if (texture == null)
            {
                texture = EditorGUIUtility.Load(EditorResources.iconsPath + name + ".png") as Texture2D;
            }

            if (texture == null)
            {
                texture = EditorGUIUtility.Load(name) as Texture2D;
            }

            return texture;
        }
    }
}