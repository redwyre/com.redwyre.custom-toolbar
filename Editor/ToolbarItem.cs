using System;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public string TypeName;
        [NonSerialized]
        public Object?[] Icons = Array.Empty<Object?>();
        public string[] IconNames = Array.Empty<string>();
        public string? Settings;
        public string? SettingsIconName;
        public Object? SettingsIcon;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}
