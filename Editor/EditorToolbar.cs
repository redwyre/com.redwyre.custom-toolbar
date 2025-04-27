// Copyright (c) BovineLabs. All rights reserved.
// With modifications
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    public static class EditorToolbar
    {
        private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject? currentToolbar = null;
        private static VisualElement? Root = null;

        static EditorToolbar()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;

            LeftParent = Utils.CreateElement("ToolbarCustomParentLeft");
            RightParent = Utils.CreateElement("ToolbarCustomParentRight");

            LeftParent.Add(LeftLeftParent = Utils.CreateElement("ToolbarCustomLeftAlignLeft"));
            LeftParent.Add(LeftCenterParent = Utils.CreateElement("ToolbarCustomLeftAlignCenter", Justify.Center));
            LeftParent.Add(LeftRightParent = Utils.CreateElement("ToolbarCustomLeftAlignRight", Justify.FlexEnd));

            RightParent.Add(RightLeftParent = Utils.CreateElement("ToolbarCustomRightAlightLeft"));
            RightParent.Add(RightCenterParent = Utils.CreateElement("ToolbarCustomRightAlightCenter", Justify.Center));
            RightParent.Add(RightRightParent = Utils.CreateElement("ToolbarCustomRightAlightRight", Justify.FlexEnd));
        }

        public static VisualElement LeftParent { get; }

        public static VisualElement LeftLeftParent { get; }

        public static VisualElement LeftCenterParent { get; }

        public static VisualElement LeftRightParent { get; }

        public static VisualElement RightParent { get; }

        public static VisualElement RightLeftParent { get; }

        public static VisualElement RightCenterParent { get; }

        public static VisualElement RightRightParent { get; }

        private static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (currentToolbar == null)
            {
                CreateToolbar();
            }
        }

        private static void CreateToolbar()
        {
            var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            if (toolbars.Length == 0)
            {
                return;
            }

            currentToolbar = (ScriptableObject)toolbars[0];
            var root = currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            Root = (VisualElement)root!.GetValue(currentToolbar);
            
            Root.Q("ToolbarZoneLeftAlign").Add(LeftParent);
            Root.Q("ToolbarZoneRightAlign").Add(RightParent);
        }

        public static VisualElement GetSectionParent(ToolbarSide side)
        {
            return side switch
            {
                ToolbarSide.LeftAlignLeft => LeftLeftParent,
                ToolbarSide.LeftAlignCenter => LeftCenterParent,
                ToolbarSide.LeftAlignRight => LeftRightParent,
                ToolbarSide.RightAlignLeft => RightLeftParent,
                ToolbarSide.RightAlignCenter => RightCenterParent,
                ToolbarSide.RightAlignRight => RightRightParent,
                _ => throw new InvalidOperationException("Invalid side"),
            };
        }

        public static void AddStyleSheet(StyleSheet styleSheet)
        {
            if (Root != null)
            {
                Root.styleSheets.Add(styleSheet);
            }
        }
    }
}
