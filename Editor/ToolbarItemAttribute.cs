using System;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;

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
        public virtual bool Tintable { get; set; } = true;


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