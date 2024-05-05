using System;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    public class ToolbarItemConfig
    {
        public string TypeName;
        public Action? Action;
        public string? Tooltip;
        public string? Icon;
        public string? Label;

        public ToolbarItemConfig(string typeName)
        {
            TypeName = typeName;
        }
    }
}
