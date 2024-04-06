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

        VisualTreeAsset list;
        VisualTreeAsset settingsPage;

        public ToolbarSettingsProvider(string path, SettingsScope scopes, IEnumerable<string>? keywords = null)
            : base(path, scopes, keywords)
        {
            list = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.redwyre.custom-toolbar/Editor/ToolbarItemSetting.uxml");
            settingsPage = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.redwyre.custom-toolbar/Editor/ToolbarSettings.uxml");
        }

        public static bool IsSettingsAvailable()
        {
            return ScriptableSingleton<ToolbarSettings>.instance != null;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = ToolbarSettings.GetSerializedSettings();

            //var title = new Label() { text = label };
            //title.AddToClassList("title");

            settingsPage.CloneTree(rootElement);
            //rootElement.Add(title);

            //rootElement.Add(new PropertyField(settings.FindProperty(nameof(ToolbarSettings.Enabled))));
            //rootElement.Add(new PropertyField(settings.FindProperty(nameof(ToolbarSettings.items))));

            //rootElement.Add(new ListView(ToolbarSettings.instance.items, makeItem: SettingsMakeItem) { bindingPath = "items" });
            //rootElement.Add(new ListView(ToolbarItems.Items, makeItem: MakeItem, bindItem: BindItem));

            rootElement.Q<Label>("Title").text = label;
            rootElement.Q<ListView>("Items").makeItem = SettingsMakeItem;
            rootElement.Q<ListView>("Adds").makeItem = MakeItem;
            rootElement.Q<ListView>("Adds").bindItem = BindItem;
            rootElement.Q<ListView>("Adds").itemsSource = ToolbarItems.Items;

            var dummy = new VisualElement();
            dummy.TrackSerializedObjectValue(settings, SettingsChanged);
            rootElement.Add(dummy);
            rootElement.Bind(settings);

        }

        private void SettingsChanged(SerializedObject settingsObject)
        {
            settingsObject.ApplyModifiedPropertiesWithoutUndo();
            ScriptableSingleton<ToolbarSettings>.instance.Save();
            ToolbarItems.RebuildToolbar();
        }

        private void SettingsBindItem(VisualElement element, int index)
        {
            if (element is TemplateContainer templateContainer)
            {
                templateContainer.bindingPath = $"items.Array.data[{index}]";
            }
        }

        private VisualElement SettingsMakeItem()
        {
            var fullElement = list.Instantiate();
            var trimmed = fullElement[0];
            return trimmed;
        }

        private void AddItem(ToolbarItemConfig config)
        {
            ScriptableSingleton<ToolbarSettings>.instance.items.Add(new ToolbarItem(config.TypeName) { Icon = GetTextureFromIcon(config) });
        }

        private static Texture2D? GetTextureFromIcon(ToolbarItemConfig config)
        {
            var content = EditorGUIUtility.IconContent(config.Icon);

            return (content != null) ? (content.image as Texture2D) : null;
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
                SettingsChanged(settings);
                settings = null;
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