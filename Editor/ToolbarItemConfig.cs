using System;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    public delegate int GroupAction(int? newValue);

    public class ToolbarItemConfig
    {
        public string TypeName;
        public Action? Action;
        public GroupAction? GroupAction;
        public string? Tooltip;
        public string[]? Icons;
        public string? Label;

        public ToolbarItemConfig(string typeName)
        {
            TypeName = typeName;
        }
    }
}
