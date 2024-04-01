using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Redwyre.CustomToolbar.Editor
{
delegate void VoidDelegate();

public class ToolbarItemConfig
{
    public Delegate d;
    public string tooltip;
}
    [InitializeOnLoad]
    public static class ToolbarItems
    {
        static ToolbarItemConfig[] items;

        static ToolbarItems()
        {
            items = GetItems();
        }

        public static ToolbarItemConfig[] GetItems()
        {
            var l = new List<ToolbarItemConfig>();
            var methods = Assembly.GetCallingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .Select(m => (methodInfo: m, attribute: m.GetCustomAttribute<ToolbarItemAttribute>()))
                .Where(x => x.attribute != null)
                .ToArray();

            foreach (var (method, attr) in methods)
            {
                try
                {
                    var c = new ToolbarItemConfig();
                    c.tooltip = attr.ToolTip;
                    c.d = Delegate.CreateDelegate(typeof(VoidDelegate), method);

                    l.Add(c);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error trying to set up toolbar item for {method.Name}: {e.Message}");
                }
            }

            return l.ToArray();
        }
    }
}
