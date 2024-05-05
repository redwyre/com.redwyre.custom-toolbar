using System;
using UnityEngine;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public string TypeName;
        public Sprite?[] Icons = Array.Empty<Sprite?>();
        public string? Settings;

        public Sprite? Icon;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}
