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
                .Select(m => (methodInfo: m, attribute: m.GetCustomAttribute<ToolbarItemBaseAttribute>()))
                .Where(x => x.attribute != null)
                .ToArray();

            foreach (var (method, attr) in methods)
            {
                try
                {
                    var c = new ToolbarItemConfig(attr, method.Name);

                    switch (attr)
                    {
                        case ToolbarItemAttribute itemAttr:
                            {
                                if (!Utils.SameSignature<Action>(method))
                                {
                                    Debug.LogError($"Method {method.Name} does not match expected signature for {nameof(ToolbarItemAttribute)}");
                                    continue;
                                }

                                c.Action = (Action)Delegate.CreateDelegate(typeof(Action), method);

                                if (itemAttr.Icon == null)
                                {
                                    Debug.LogError($"No icon specified for {method.Name} in {nameof(ToolbarItemAttribute)}");
                                    continue;
                                }
                            }
                            break;
                        case ToolbarItemGroupAttribute groupAttr:
                            {
                                if (!Utils.SameSignature<GroupAction>(method))
                                {
                                    Debug.LogError($"Method {method.Name} does not match expected signature for {nameof(ToolbarItemGroupAttribute)}");
                                    continue;
                                }

                                c.GroupAction = (GroupAction)Delegate.CreateDelegate(typeof(GroupAction), method);

                                if (groupAttr.Icons.Length == 0)
                                {
                                    Debug.LogError($"No icons specified for {method.Name} in {nameof(ToolbarItemGroupAttribute)}");
                                    continue;
                                }
                            }
                            break;
                        default:
                            {
                                Debug.LogError($"Unknown attribute for {method.Name}");
                                continue;
                            }
                    }

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


            // update textures in place
            foreach (var x in ToolbarSettings.instance.Sections)
            {
                foreach (var y in x.Items)
                {
                    y.Icons = y.IconNames.Select(icon => Utils.GeSpriteFromIcon(icon)).ToArray();
                    y.SettingsIcon = string.IsNullOrEmpty(y.SettingsIconName) ? null : Utils.GeSpriteFromIcon(y.SettingsIconName!);
                }
            }


            var configLookup = itemConfigs.ToDictionary(ic => ic.TypeName);

            var sections = ToolbarSettings.instance.Sections;

            foreach (var group in sections)
            {
                var sectionParent = EditorToolbar.GetSectionParent(group.ToolbarSide);

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
                            sectionParent.Add(strip);
                            activeElements.Add(strip);
                        }
                        else if (config.Action != null)
                        {
                            var b = CreateToolbarButton(item, config);

                            sectionParent.Add(b);
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
                icon.style.backgroundImage = Utils.BackgroundFromObject(item.Icons[index]);
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

            if (item.Icons.FirstOrDefault() != null)
            {
                var icon = new Image();
                icon.AddToClassList("unity-editor-toolbar-element__icon");

                //Utils.SetImageFromObject(icon, item.Icons.FirstOrDefault());
                var b = Utils.BackgroundFromObject(item.Icons.FirstOrDefault());
                icon.style.backgroundImage = b;
                icon.style.height = 16;
                icon.style.width = 16;
                icon.style.alignSelf = Align.Center;

                if (config.Attribute.Tintable)
                {
                    icon.AddToClassList("unity-editor-toolbar-element__icon-tintable");
                    icon.style.unityBackgroundImageTintColor = new Color(0.7f, 0.7f, 0.7f);
                }

                button.Add(icon);
            }

            return button;
        }
    }
}
