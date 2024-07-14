using System;
using UnityEngine;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public string TypeName;
        public Texture2D?[] Icons = Array.Empty<Texture2D?>();
        public string? Settings;

        public Texture2D? Icon;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}
