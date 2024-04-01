using System;

namespace Redwyre.CustomToolbar.Editor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    sealed class ToolbarItemAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236


        public string ToolTip { get; set; }
    }
}