using System;
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

        [ToolbarItem(Icon = "folder-open", ToolTip = "Open Folder")]
        public static void OpenFolder()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
        }

        [Serializable]
        public class OpenTerminalOptions
        {
            public string terminal = "wt";
        }

        [ToolbarItem(Icon = "terminal", ToolTip = "Open Terminal")]
        public static void OpenTerminal()
        {
            // first use configured terminal if any
            var term = Environment.GetEnvironmentVariable("TERM");
            if (term != null && Process.Start(term) != null)
            {
                return;
            }

            // use standard platform terminals
            string[] terminals = Application.platform switch
            {
                RuntimePlatform.WindowsEditor => new[] { "wt", "pwsh", "powershell", "cmd" },
                RuntimePlatform.LinuxEditor => new[] { "/usr/bin/gnome-terminal", "/usr/bin/xterm", "/bin/bash" },
                RuntimePlatform.OSXEditor => new[] { "Terminal" },
                _ => throw new NotImplementedException()
            };

            foreach (var terminal in terminals)
            {
                if (Process.Start(terminal) != null)
                {
                    break;
                }
            }
        }

        [ToolbarItem(Icon = "cs Script Icon", ToolTip = "Script Recompile")]
        public static void ScriptRecompile()
        {
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
        }

        [ToolbarItem(Icon = "recycle", ToolTip = "Domain Reload")]
        public static void DomainReload()
        {
            EditorUtility.RequestScriptReload();
        }

        [ToolbarItem(Icon = "user-xmark", ToolTip = "Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            if (EditorUtility.DisplayDialog("Clear PlayerPrefs", "Are you sure you want to clear PlayerPrefs?", "Clear", "Cancel"))
            {
                PlayerPrefs.DeleteAll();
            }
        }

        [ToolbarItem(ToolTip = "Memory Leak Detection", Icons = new[] {
            "Packages/com.redwyre.custom-toolbar/Editor Default Resources/Icons/MemoryLeakDetection_0.png",
            "Packages/com.redwyre.custom-toolbar/Editor Default Resources/Icons/MemoryLeakDetection_1.png",
            "Packages/com.redwyre.custom-toolbar/Editor Default Resources/Icons/MemoryLeakDetection_2.png" })]
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
