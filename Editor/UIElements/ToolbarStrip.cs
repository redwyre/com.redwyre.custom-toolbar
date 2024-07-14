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
                ResetToggles(index);
                groupAction.Invoke(index);

                selected = index;
                previouslySelected = selected;
            }
            else
            {
                // toggle with previously selected
                ResetToggles();
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

        void ResetToggles(int leaveIndex = -1)
        {
            foreach (var (index, toggle) in toggles)
            {
                if (index == leaveIndex)
                    continue;

                toggle.SetValueWithoutNotify(false);
            }
        }

        public void Init()
        {
            EditorToolbarUtility.SetupChildrenAsButtonStrip(this);
            ResetToggles();
            selected = groupAction.Invoke(null);
            toggles[selected].SetValueWithoutNotify(true);
        }
    }
}