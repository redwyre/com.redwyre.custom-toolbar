using UnityEngine;
using UnityEditor;
using System;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public bool Enabled;
        public string TypeName;
        public ToolbarSide Side;
        public Texture2D? Icon;
        public string? Settings;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}
