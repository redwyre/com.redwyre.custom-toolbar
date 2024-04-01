using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    public class ToolbarItemConfig
    {
        public Action? Action;
        public string? Tooltip;
        public string? Icon;
        public string? Label;
    }

    [InitializeOnLoad]
    public static class ToolbarItems
    {
        static ToolbarItemConfig[] items;

        public static IReadOnlyList<ToolbarItemConfig> Items => items;

        static ToolbarItems()
        {
            items = GetItems();

            RebuildToolbar();
        }

        public static ToolbarItemConfig[] GetItems()
        {
            var l = new List<ToolbarItemConfig>();
            var methods = Assembly.GetCallingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .Select(m => (methodInfo: m, attribute: m.GetCustomAttribute<ToolbarItemAttribute>()))
                .Where(x => x.attribute != null)
                .ToArray();

            foreach (var (method, attr) in methods)
            {
                try
                {
                    var c = new ToolbarItemConfig();
                    c.Tooltip = attr.ToolTip;
                    c.Action = (Action)Delegate.CreateDelegate(typeof(Action), method);
                    c.Icon = attr.Icon;
                    c.Label = attr.Label;

                    l.Add(c);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error trying to set up toolbar item for {method.Name}: {e.Message}");
                }
            }

            return l.ToArray();
        }

        public static void RebuildToolbar()
        {
            foreach (var item in items)
            {
                var b = CreateButton(item);
                var side = ToolbarSide.LeftAlignCenter;
                var ve = GetParent(side);

                ve.Add(b);
            }
        }

        public static VisualElement GetParent(ToolbarSide side)
        {
            return side switch
            {
                ToolbarSide.LeftAlignLeft => EditorToolbar.LeftLeftParent,
                ToolbarSide.LeftAlignCenter => EditorToolbar.LeftCenterParent,
                ToolbarSide.LeftAlignRight => EditorToolbar.LeftRightParent,
                ToolbarSide.RightAlignLeft => EditorToolbar.RightLeftParent,
                ToolbarSide.RightAlignCenter => EditorToolbar.RightCenterParent,
                ToolbarSide.RightAlignRight => EditorToolbar.RightRightParent,
                _ => throw new InvalidOperationException("Invalid side"),
            };
        }

        public static VisualElement CreateButton(ToolbarItemConfig config)
        {
            var button = new ToolbarButton(config.Action);
            button.tooltip = config.Tooltip;
            button.AddToClassList("unity-editor-toolbar-element");

            if (config.Label != null)
            {
                button.text = config.Label;
            }

            if (config.Icon != null)
            {
                var icon = new Image();
                icon.AddToClassList("unity-editor-toolbar-element__icon");
                icon.style.backgroundImage = Background.FromTexture2D((Texture2D)EditorGUIUtility.IconContent(config.Icon).image);
                icon.style.height = 16;
                icon.style.width = 16;
                icon.style.alignSelf = Align.Center;
                button.Add(icon);
            }

            return button;
        }
    }
}
