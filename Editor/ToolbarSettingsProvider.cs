using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Redwyre.CustomToolbar.Editor
{
    public class ToolbarSettingsProvider : SettingsProvider
    {
        public const string defaultPath = "Preferences/CustomToolbar";
        public const SettingsScope defaultScope = SettingsScope.User;
        static string[] defaultKeywords = new[] { "toolbar", "custom" };

        SerializedObject settings;

        public ToolbarSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
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
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomToolbarSettingProvider()
        {
            if (IsSettingsAvailable())
            {
                return new ToolbarSettingsProvider(defaultPath, defaultScope, defaultKeywords);
            }

            return null;
        }
    }
}