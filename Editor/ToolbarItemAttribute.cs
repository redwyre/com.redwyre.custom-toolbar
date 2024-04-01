using System;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    sealed class ToolbarItemAttribute : Attribute
    {
        public string? ToolTip { get; set; }

        public string? Icon { get; set; }

        public string? Label { get; set; }
    }
}