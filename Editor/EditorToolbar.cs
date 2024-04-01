// Copyright (c) BovineLabs. All rights reserved.
// With modifications

namespace Redwyre.CustomToolbar.Editor
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class EditorToolbar
    {
        private static readonly Type ToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject currentToolbar;
        private static bool isInitialized;

        static EditorToolbar()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;

            currentToolbar = ScriptableObject.CreateInstance<ScriptableObject>();

            LeftParent = CreateParentElement();
            RightParent = CreateParentElement();

            LeftParent.Add(LeftLeftParent = CreateSectionElement());
            LeftParent.Add(LeftCenterParent = CreateSectionElement(Justify.Center));
            LeftParent.Add(LeftRightParent = CreateSectionElement(Justify.FlexEnd));

            RightParent.Add(RightLeftParent = CreateSectionElement());
            RightParent.Add(RightCenterParent = CreateSectionElement(Justify.Center));
            RightParent.Add(RightRightParent = CreateSectionElement(Justify.FlexEnd));
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
            if (currentToolbar == null || !isInitialized)
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
            var mRoot = (VisualElement)root!.GetValue(currentToolbar);

            mRoot.Q("ToolbarZoneLeftAlign").Add(LeftParent);
            mRoot.Q("ToolbarZoneRightAlign").Add(RightParent);
            isInitialized = true;
        }

        private static VisualElement CreateParentElement()
        {
            return new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                },
            };
        }

        private static VisualElement CreateSectionElement(Justify justify = Justify.FlexStart)
        {
            return new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                    justifyContent = justify
                },
            };
        }
    }
}
