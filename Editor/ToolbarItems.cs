using System;
using System.Collections;
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

    [InitializeOnLoad]
    public static class ToolbarItems
    {
        static ToolbarItemConfig[] itemConfigs;
        static ToolbarItem[] savedItems = Array.Empty<ToolbarItem>();
        static List<VisualElement> activeElements = new List<VisualElement>();

        public static ToolbarItemConfig[] Items => itemConfigs;

        static ToolbarItems()
        {
            itemConfigs = GetItems();

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
                    var c = new ToolbarItemConfig(method.Name);
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
            foreach (var e in activeElements)
            {
                e.RemoveFromHierarchy();
            }
            activeElements.Clear();

            if (!ToolbarSettings.instance.Enabled)
            {
                return;
            }

            var configLookup = itemConfigs.ToDictionary(ic => ic.TypeName);

            var toolbarItems = ToolbarSettings.instance.items;

            foreach (var item in toolbarItems)
            {
                if (configLookup.TryGetValue(item.TypeName, out var config))
                {
                    if (!item.Enabled)
                    {
                        continue;
                    }

                    var b = CreateToolbarButton(item, config);
                    var side = item.Side;
                    var ve = GetParent(side);

                    ve.Add(b);
                    activeElements.Add(b);
                }
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

        public static VisualElement CreateToolbarButton(ToolbarItem item, ToolbarItemConfig config)
        {
            var button = new ToolbarButton(config.Action);
            button.tooltip = config.Tooltip;
            button.AddToClassList("unity-editor-toolbar-element");

            if (config.Label != null)
            {
                button.text = config.Label;
            }

            if (item.Icon != null)
            {
                var icon = new Image();
                icon.AddToClassList("unity-editor-toolbar-element__icon");
                icon.style.backgroundImage = Background.FromTexture2D(item.Icon);
                icon.style.height = 16;
                icon.style.width = 16;
                icon.style.alignSelf = Align.Center;
                button.Add(icon);
            }

            return button;
        }
    }
}
