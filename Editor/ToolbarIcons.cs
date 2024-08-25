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

        static Dictionary<string, string> iconPacks = new()
        {
            { "fa", BasePath }
        };

        static ToolbarIcons()
        {

        }

        public static Object? GetIcon(string name)
        {
            if (name.StartsWith("Packages") || name.StartsWith("Assets"))
            {
                return LoadIconAsset(name);
            }

            if (FindIconPack(name, out var path))
            {
                return LoadIconAsset(path);
            }

            var texture = LoadBuiltInTexture(name);

            if (texture != null)
            {
                return texture;
            }

            Debug.LogWarning($"Unable to find icon {name}");
            return null;
        }

        private static bool FindIconPack(string name, out string path)
        {
            path = string.Empty;
            var parts = name.Split(':');
            if ( parts.Length != 2)
            {
                return false;
            }

            var pack = parts[0];
            var iconName = parts[1];

            if (iconPacks.TryGetValue(pack, out path))
            {
                path = Path.Combine(path, iconName + ".svg");
                return true;
            }

            return false;
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