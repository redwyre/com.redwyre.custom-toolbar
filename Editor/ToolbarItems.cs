using Redwyre.CustomToolbar.Editor.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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

                    if (SameSignature<GroupAction>(method))
                    {
                        c.GroupAction = (GroupAction)Delegate.CreateDelegate(typeof(GroupAction), method);
                    }
                    else if (SameSignature<Action>(method))
                    {
                        c.Action = (Action)Delegate.CreateDelegate(typeof(Action), method);
                    }
                    else
                    {
                        Debug.LogError($"Method {method.Name} does not match expected signature");
                        continue;
                    }

                    c.Icons = attr.Icons;
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

            var groups = ToolbarSettings.instance.Groups;

            foreach (var group in groups)
            {
                var groupParent = GetParent(group.ToolbarSide);

                foreach (var item in group.Items)
                {
                    if (configLookup.TryGetValue(item.TypeName, out var config))
                    {
                        if (config.GroupAction != null && item.Icons != null)
                        {
                            var groupItems = new List<ToolbarToggle>();

                            foreach (var icon in item.Icons)
                            {
                                var toggle = CreateToolbarToggle(item, config, groupItems.Count);
                                groupItems.Add(toggle);
                            }

                            var strip = CreateToolbarGroup(item, config, groupItems);
                            groupParent.Add(strip);
                            activeElements.Add(strip);
                        }
                        else if (config.Action != null)
                        {
                            var b = CreateToolbarButton(item, config);

                            groupParent.Add(b);
                            activeElements.Add(b);
                        }
                        else
                        {
                            Debug.LogError($"Error instantiating toolbar item for {item.TypeName}");
                        }
                    }
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

        public static VisualElement CreateToolbarGroup(ToolbarItem item, ToolbarItemConfig config, List<ToolbarToggle> groupItems)
        {
            var group = new ToolbarStrip();

            group.groupAction = config.GroupAction;

            // fixme 

            for (int i = 0; i < groupItems.Count; ++i)
            {
                var groupItem = groupItems[i];

                group.AddButton(groupItem, i);
            }

            group.Init();

            return group;
        }

        public static ToolbarToggle CreateToolbarToggle(ToolbarItem item, ToolbarItemConfig config, int index)
        {
            var toggle = new ToolbarToggle();
            toggle.tooltip = config.Tooltip;
            toggle.AddToClassList("unity-editor-toolbar-element");

            var checkmark = toggle.Q<VisualElement>("unity-checkmark");

            var visualElement = toggle.Q<VisualElement>("unity-checkmark").parent;

            if (config.Label != null)
            {
                toggle.text = config.Label;
            }

            if (item.Icons != null && item.Icons.Length >= index)
            {
                var icon = new Image();
                icon.AddToClassList("unity-editor-toolbar-element__icon");
                icon.style.backgroundImage = Background.FromTexture2D(item.Icons[index]);
                icon.style.height = 16;
                icon.style.width = 16;
                icon.style.alignSelf = Align.Center;
                visualElement.Add(icon);

                checkmark.style.display = DisplayStyle.None;
            }

            return toggle;
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

        static bool SameSignature<T>(MethodInfo methodInfo)
        {
            var delegateType = typeof(T).GetMethod("Invoke");

            if (delegateType.ReturnType != methodInfo.ReturnType)
                return false;

            var delegateParams = delegateType.GetParameters().Select(p => p.ParameterType);
            var methodParams = methodInfo.GetParameters().Select(p => p.ParameterType);
            return delegateParams.SequenceEqual(methodParams);
        }
    }
}
