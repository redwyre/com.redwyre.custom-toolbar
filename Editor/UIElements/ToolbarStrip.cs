using System;
using System.Collections.Generic;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Redwyre.CustomToolbar.Editor.UIElements
{
    public class ToolbarStrip : VisualElement
    {
        public GroupAction groupAction;
        Dictionary<int, ToolbarToggle> toggles = new Dictionary<int, ToolbarToggle>();
        int selected = -1;
        int previouslySelected = -1;

        public ToolbarStrip()
        {
            AddToClassList("unity-editor-toolbar-element");
            AddToClassList("unity-editor-toolbar__button-strip");
        }

        public void OnChange(ChangeEvent<bool> evt, int index)
        {
            var toggle = evt.target as ToolbarToggle;

            if (toggle.value)
            {
                UpdateToggles(index);
                groupAction.Invoke(index);

                selected = index;
                previouslySelected = selected;
            }
            else
            {
                // toggle with previously selected
                UpdateToggles();
                toggles[previouslySelected].SetValueWithoutNotify(true);
                groupAction?.Invoke(previouslySelected);
                (selected, previouslySelected) = (previouslySelected, selected);
            }
        }

        public void AddButton(ToolbarToggle toggle, int index)
        {

            toggle.AddToClassList("unity-editor-toolbar__button-strip-element");
            toggle.AddToClassList("unity-editor-toolbar__button-strip-element--left");

            toggle.RegisterCallback<ChangeEvent<bool>, int>(OnChange, index);
            toggles.Add(index, toggle);
            Add(toggle);
        }

        void UpdateToggles(int selectedIndex = -1)
        {
            foreach (var (index, toggle) in toggles)
            {
                if (index == selectedIndex)
                    continue;

                toggle.SetValueWithoutNotify(false);
            }

            if (selectedIndex != -1 && toggles.TryGetValue(selectedIndex, out var selectedToggle))
            {
                selectedToggle.SetValueWithoutNotify(true);
            }
        }

        public void Init()
        {
            EditorToolbarUtility.SetupChildrenAsButtonStrip(this);
            selected = groupAction.Invoke(null);
            UpdateToggles(selected);
        }
    }
}