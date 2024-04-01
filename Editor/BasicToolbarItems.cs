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
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
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
    }
}
