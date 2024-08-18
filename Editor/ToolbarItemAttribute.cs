using System;
using System.Drawing;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.Profiling;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    public abstract class ToolbarItemBaseAttribute : Attribute
    {
        string? settingsIcon = null;
        protected string[] icons = Array.Empty<string>();

        public virtual string? ToolTip { get; set; } = null;
        public virtual string? SettingsIcon { get => settingsIcon ?? icons.FirstOrDefault(); set => settingsIcon = value; }
        public virtual string? Label { get; set; } = null;


        public string[] GetIcons() => icons;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    sealed class ToolbarItemAttribute : ToolbarItemBaseAttribute
    {
        public string? Icon { get => icons.SingleOrDefault(); set => icons = string.IsNullOrEmpty(value) ? Array.Empty<string>() : new string[] { value! }; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    sealed class ToolbarItemGroupAttribute : ToolbarItemBaseAttribute
    {
        public string[] Icons { get => icons; set => icons = value; }
    }
}