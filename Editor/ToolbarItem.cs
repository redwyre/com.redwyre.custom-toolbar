using UnityEngine;
using UnityEditor;
using System;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public string TypeName;
        public ToolbarSide Side;
        public string? Settings;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}