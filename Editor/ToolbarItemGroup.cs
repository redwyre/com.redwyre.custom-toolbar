using Redwyre.CustomToolbar.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

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
