using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        const int MaxEnumItems = (int)ToolbarSide.RightAlignRight + 1;

        SerializedObject? settings;

        VisualTreeAsset list;
        VisualTreeAsset settingsPage;

        public ToolbarSettingsProvider(string path, SettingsScope scopes, IEnumerable<string>? keywords = null)
            : base(path, scopes, keywords)
        {
            list = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.redwyre.custom-toolbar/Editor/ToolbarItemSetting.uxml");
            settingsPage = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.redwyre.custom-toolbar/Editor/ToolbarSettings.uxml");

            Assert.IsNotNull(list);
            Assert.IsNotNull(settingsPage);
        }

        public static bool IsSettingsAvailable()
        {
            return ScriptableSingleton<ToolbarSettings>.instance != null;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = ToolbarSettings.GetSerializedSettings();

            //Debug.Log(SerializedObjectUtils.FormatSerializedObjectNames(settings));

            settingsPage.CloneTree(rootElement);

            var lists = rootElement.Q<ScrollView>("Lists");

            foreach (var side in Enum.GetValues(typeof(ToolbarSide)).Cast<ToolbarSide>())
            {
                lists.Add(new Label(side.ToString()));
                var itemList = new ListView()
                {
                    name = $"{side}List",
                    bindingPath = $"Groups.Array.data[{(int)side}].Items",
                    showBoundCollectionSize = false,
                    reorderable = true,
                    reorderMode = ListViewReorderMode.Animated,
                    showBorder = true,
                    virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                };

                itemList.makeItem = () =>
                {
                    var templateContainer = list.Instantiate();
                    var rootElement = templateContainer[0];
                    return rootElement;
                };
                itemList.bindItem = (element, index) =>
                {
                    var bindableElement = (BindableElement)element;

                    bindableElement.bindingPath = $"{itemList.bindingPath}.Array.data[{index}]";
                    var prop = settings.FindProperty(bindableElement.bindingPath);
                    bindableElement.BindProperty(prop);

                    var removeButton = element.Q<Button>("Remove");
                    var moveLeftButton = element.Q<Button>("MoveLeft");
                    var moveRightButton = element.Q<Button>("MoveRight");

                    moveLeftButton.SetEnabled(side != ToolbarSide.LeftAlignLeft);
                    moveRightButton.SetEnabled(side != ToolbarSide.RightAlignRight);

                    removeButton.clicked += () =>
                    {
                        var savedSide = (int)side;
                        ToolbarSettings.instance.Groups[savedSide].Items.RemoveAt(index);
                    };
                    moveLeftButton.clicked += () =>
                    {
                        var savedSide = (int)side;
                        var item = ToolbarSettings.instance.Groups[savedSide].Items[index];

                        ToolbarSettings.instance.Groups[savedSide].Items.RemoveAt(index);
                        ToolbarSettings.instance.Groups[--savedSide].Items.Add(item);
                    };
                    moveRightButton.clicked += () =>
                    {
                        var savedSide = (int)side;
                        var item = ToolbarSettings.instance.Groups[savedSide].Items[index];

                        ToolbarSettings.instance.Groups[savedSide].Items.RemoveAt(index);
                        ToolbarSettings.instance.Groups[++savedSide].Items.Add(item);
                    };
                };
                itemList.unbindItem = (element, index) =>
                {
                    var removeButton = element.Q<Button>("Remove");
                    var moveLeftButton = element.Q<Button>("MoveLeft");
                    var moveRightButton = element.Q<Button>("MoveRight");
                    removeButton.clickable = null;
                    moveLeftButton.clickable = null;
                    moveRightButton.clickable = null;
                };

                lists.Add(itemList);
            }

            rootElement.Q<Label>("Title").text = label;
            //rootElement.Q<ListView>("Items").makeItem = SettingsMakeItem;
            rootElement.Q<ListView>("Adds").makeItem = AddsMakeItem;
            rootElement.Q<ListView>("Adds").bindItem = AddsBindItem;
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
            element.Q<Button>("Remove").clicked += () => { };
        }

        private void SubListSettingsBindItem(VisualElement element, int index)
        {
            if (element is TemplateContainer templateContainer)
            {
                templateContainer.bindingPath = $"items.Array.data[{index}]";
            }
        }

        private void AddItem(ToolbarItemConfig config)
        {
            var icons = config.Icons.Select(icon => GetTextureFromIcon(icon)).ToArray();
            var item = new ToolbarItem(config.TypeName)
            {
                Icons = icons,
                Icon = icons.Length > 0 ? icons[0] : null,
            };

            ScriptableSingleton<ToolbarSettings>.instance.Groups[0].Items.Add(item);
            //settings!.Update();
        }

        private static Texture2D? GetTextureFromIcon(string icon)
        {
            var content = EditorGUIUtility.IconContent(icon);

            return (content != null) ? (content.image as Texture2D) : null;
        }

        private void AddsBindItem(VisualElement element, int index)
        {
            var i = ToolbarItems.Items[index];
            element.Q<Label>("Label").text = i.TypeName;
            element.Q<Button>("Button").clicked += () => { AddItem(i); };
        }



        private VisualElement AddsMakeItem()
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