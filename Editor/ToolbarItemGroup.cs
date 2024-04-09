using System;
using System.Collections.Generic;
using UnityEngine;

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarItemGroup
    {
        [SerializeField]
        ToolbarSide toolbarSide;

        public ToolbarSide ToolbarSide => toolbarSide;

        public List<ToolbarItem> Items = new();

        public ToolbarItemGroup(ToolbarSide toolbarSide)
        {
            this.toolbarSide = toolbarSide;
        }
    }
}