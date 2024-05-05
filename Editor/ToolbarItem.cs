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
        public Sprite?[] Icons = Array.Empty<Sprite?>();
        public string[] IconNames = Array.Empty<string>();
        public string? Settings;
        public string? SettingsIconName;
        public Sprite? SettingsIcon;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}
