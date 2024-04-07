using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Redwyre.CustomToolbar.Editor
{
    public static class SerializedObjectUtils
    {
        public static string FormatSerializedObjectNames(SerializedObject serializedObject)
        {
            var sb = new StringBuilder();

            var prop = serializedObject.GetIterator();
            do
            {
                sb.AppendLine($"{prop.propertyPath}: {prop.type}");
            } while (prop.NextVisible(true));

            return sb.ToString();
        }
    }
}