using System.Diagnostics;
using Unity.Collections;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    public static class BasicToolbarItems
    {
        [ToolbarItem(Icon = "Settings", ToolTip = "Project Settings")]
        public static void OpenProjectSettings()
        {
            EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
        }

        [ToolbarItem(Icon = "Package Manager", ToolTip = "Package Manager")]
        public static void OpenPackageManager()
        {
            UnityEditor.PackageManager.UI.Window.Open("");
        }

        [ToolbarItem(Icon = "Folder Icon", ToolTip = "Open Folder")]
        public static void OpenFolder()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
        }

        [ToolbarItem(Icon = "d_winbtn_win_max", ToolTip = "Open Terminal")]
        public static void OpenTerminal()
        {
            Process.Start("wt");
        }

        [ToolbarItem(Icon = "cs Script Icon", ToolTip = "Script Recompile")]
        public static void ScriptRecompile()
        {
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
        }

        [ToolbarItem(Icon = "P4_Updating", ToolTip = "Domain Reload")]
        public static void DomainReload()
        {
            EditorUtility.RequestScriptReload();
        }

        [ToolbarItem(Icon = "Cancel", ToolTip = "Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [ToolbarItem(ToolTip = "Memory Leak Detection", Icons = new[] { "0", "1", "2" })]
        public static int MemoryLeakDetection(int? newValue)
        {
            if (newValue.HasValue)
            {
                NativeLeakDetection.Mode = newValue.Value switch
                {
                    1 => NativeLeakDetectionMode.Enabled,
                    2 => NativeLeakDetectionMode.EnabledWithStackTrace,
                    _ => NativeLeakDetectionMode.Disabled,
                };

                Debug.Log($"NativeLeakDetection.Mode set to {NativeLeakDetection.Mode}");
            }

            return NativeLeakDetection.Mode switch
            {
                NativeLeakDetectionMode.Enabled => 1,
                NativeLeakDetectionMode.EnabledWithStackTrace => 2,
                _ => 0
            };
        }
    }
}
