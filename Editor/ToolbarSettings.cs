using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [FilePath("Redwyre/CustomToolbarSettings.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class ToolbarSettings : ScriptableSingleton<ToolbarSettings>
    {
        public List<ToolbarItem> items = new();

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(instance);
        }

        public void Save()
        {
            Save(true);
        }    
    }
}