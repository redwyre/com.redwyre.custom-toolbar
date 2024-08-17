using System;
using System.Collections.Generic;
using UnityEngine;

namespace Redwyre.CustomToolbar.Editor
{
    [Serializable]
    public class ToolbarSection
    {
        [SerializeField]
        ToolbarSide toolbarSide;

        public ToolbarSide ToolbarSide => toolbarSide;

        public List<ToolbarItem> Items = new();

        public ToolbarSection(ToolbarSide toolbarSide)
        {
            this.toolbarSide = toolbarSide;
        }
    }
}