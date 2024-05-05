using System;
using UnityEngine;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public string TypeName;
        public Sprite? Icon;
        public string? Settings;

        public ToolbarItem(string typeName)
        {
            TypeName = typeName;
        }
    }
}
