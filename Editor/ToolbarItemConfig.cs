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
        public ToolbarItemBaseAttribute Attribute;

        public string? Tooltip => Attribute.ToolTip;
        public string? SettingsIcon => Attribute.SettingsIcon;
        public string? Label => Attribute.Label;

        public ToolbarItemConfig(ToolbarItemBaseAttribute attribute, string typeName)
        {
            Attribute = attribute;
            TypeName = typeName;
        }
    }
}
