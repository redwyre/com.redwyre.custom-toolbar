using System;
using UnityEngine;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public string TypeName;
        [NonSerialized]
        public Texture2D?[] Icons = Array.Empty<Texture2D?>();
        public string[] IconNames = Array.Empty<string>();
        public string? Settings;
        public string? SettingsIconName;
        public Texture2D? SettingsIcon;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}
