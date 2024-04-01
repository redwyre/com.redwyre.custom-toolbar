using UnityEngine;
using UnityEditor;
using System;

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItem
    {
        public string TypeName;
        public ToolbarSide Side;
        public string Settings;
    }
}