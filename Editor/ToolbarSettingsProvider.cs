using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    public class ToolbarSettingsProvider : SettingsProvider
    {
        public const string defaultPath = "Preferences/Custom Toolbar";
        public const SettingsScope defaultScope = SettingsScope.User;
        static string[] defaultKeywords = new[] { "toolbar", "custom" };

        SerializedObject? settings;

        public ToolbarSettingsProvider(string path, SettingsScope scopes, IEnumerable<string>? keywords = null)
            : base(path, scopes, keywords)
        {
        }

        public static bool IsSettingsAvailable()
        {
            return ScriptableSingleton<ToolbarSettings>.instance != null;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = ToolbarSettings.GetSerializedSettings();

            var title = new Label() { text = label };
            title.AddToClassList("title");
            rootElement.Add(title);

            rootElement.Add(new PropertyField(settings.FindProperty(nameof(ToolbarSettings.items))));

            rootElement.Add(new ListView(ToolbarItems.Items, makeItem: MakeItem, bindItem: BindItem));

            rootElement.Bind(settings);
        }

        private void AddItem(ToolbarItemConfig config)
        {
            ScriptableSingleton<ToolbarSettings>.instance.items.Add(new ToolbarItem(config.TypeName));
        }

        private void BindItem(VisualElement element, int index)
        {
            var i = ToolbarItems.Items[index];
            element.Q<Label>("Label").text = i.TypeName;
            element.Q<Button>("Button").clicked += () => { AddItem(i); };
        }

        private VisualElement MakeItem()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.height = 32;
            var label = new Label()
            {
                name = "Label"
            };
            var button = new Button()
            {
                name = "Button",
                text = "Add",
            };
            button.style.width = 32;
            root.Add(label);
            root.Add(button);
            return root;
        }

        public override void OnDeactivate()
        {
            if (settings != null)
            {
                settings.ApplyModifiedPropertiesWithoutUndo();
                ScriptableSingleton<ToolbarSettings>.instance.Save();
                settings = null;

                ToolbarItems.RebuildToolbar();
            }
        }

        [SettingsProvider]
        public static SettingsProvider? CreateCustomToolbarSettingProvider()
        {
            if (IsSettingsAvailable())
            {
                return new ToolbarSettingsProvider(defaultPath, defaultScope, defaultKeywords);
            }

            return null;
        }
    }
}