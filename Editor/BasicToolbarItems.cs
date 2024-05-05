using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

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
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
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
    }
}
