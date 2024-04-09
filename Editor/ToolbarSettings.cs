using System;
using System.Linq;
using UnityEditor;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    [FilePath("Redwyre/CustomToolbarSettings.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class ToolbarSettings : ScriptableSingleton<ToolbarSettings>
    {
        public bool Enabled = true;
        public ToolbarItemGroup[] Groups = new ToolbarItemGroup[6];

        public ToolbarSettings()
        {
            foreach (var side in Enum.GetValues(typeof(ToolbarSide)).Cast<ToolbarSide>())
            {
                Groups[(int)side] = new ToolbarItemGroup(side);
            }
        }

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