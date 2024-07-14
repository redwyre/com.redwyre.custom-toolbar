using System;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    sealed class ToolbarItemAttribute : Attribute
    {
        string[] icons = Array.Empty<string>();

        public string? ToolTip { get; set; }

        public string? Icon
        {
            get => icons.Length > 0 ? icons[0] : null;
            set => icons = (value != null) ? new[] { value } : Array.Empty<string>();
        }

        public string? Label { get; set; }

        public string[] Icons { get => icons; set => icons = value; }
    }
}