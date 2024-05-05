using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Redwyre.CustomToolbar.Editor
{
    public static class Utils
    {
        //public static Texture2D? GetTextureFromIcon(string icon)
        //{
        //    var content = EditorGUIUtility.IconContent(icon);

        //    return (content != null) ? (content.image as Texture2D) : null;
        //}

        public static Sprite? GeSpriteFromIcon(string? icon)
        {
            var sprite = icon != null ? ToolbarIcons.GetIcon(icon) : null;
            return sprite;
        }

        public static VisualElement CreateElement(string name, Justify justify = Justify.FlexStart)
        {
            return new VisualElement
            {
                name = name,
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                    justifyContent = justify
                },
            };
        }

        static public bool SameSignature<T>(MethodInfo methodInfo)
        {
            var delegateType = typeof(T).GetMethod("Invoke");

            if (delegateType.ReturnType != methodInfo.ReturnType)
                return false;

            var delegateParams = delegateType.GetParameters().Select(p => p.ParameterType);
            var methodParams = methodInfo.GetParameters().Select(p => p.ParameterType);
            return delegateParams.SequenceEqual(methodParams);
        }
    }
}