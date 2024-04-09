using System;
using UnityEngine;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public string TypeName;
        public Texture2D? Icon;
        public string? Settings;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}
