using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [InitializeOnLoad]
    public static class ToolbarIcons
    {
        private const string BasePath = "Packages/com.redwyre.custom-toolbar/FontAwesome/icons/";
        static readonly Dictionary<string, Sprite> namedIcons = new();
        static readonly Dictionary<string, Sprite> unitySprites = new();

        static ToolbarIcons()
        {
            Load();
        }

        public static void Load()
        {
            var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { BasePath });
            foreach (var spriteGuid in spriteGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(spriteGuid);
                var asset = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                var relativePath = path.Replace(BasePath, string.Empty).Replace(".png", string.Empty);
                namedIcons.Add(relativePath, asset);
            }
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

        public static Sprite? GetIcon(string name)
        {
            if (unitySprites.TryGetValue(name, out var sprite))
            {
                return sprite;
            }

            var faIconName = name.Contains('/') ? name : $"solid/{name}";

            if (namedIcons.TryGetValue(faIconName, out sprite))
            {
                return sprite;
            }

            var texture = LoadBuiltInTexture(name);

        if (texture != null)
        {
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            sprite.name = texture.name;
            unitySprites[name] = sprite;

            Debug.Log($"Created sprite {sprite.name}");
            return sprite;
        }

            Debug.LogWarning($"Unable to find icon {name}");
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