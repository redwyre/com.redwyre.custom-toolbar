using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Redwyre.CustomToolbar.Editor
{
    public static class BasicToolbarItems
    {
        [ToolbarItem]
        public static void ShowProjectSettings()
        {
            EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
        }
    }
}
