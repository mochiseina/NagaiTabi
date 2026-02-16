using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Naninovel
{
    public static class GUIContents
    {
        public static readonly GUIContent HelpIcon;
        public static readonly GUIContent EditIcon;
        public static readonly GUIContent NaninovelIcon;
        public static readonly GUIContent ScriptAssetIcon;

        static GUIContents ()
        {
            var contentsType = typeof(EditorGUI).GetNestedType("GUIContents", BindingFlags.NonPublic);
            var px = EditorGUIUtility.pixelsPerPoint >= 2 ? "@2x" : "@1x";

            HelpIcon = contentsType.GetProperty("helpIcon",
                BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as GUIContent;
            NaninovelIcon = new(Engine.LoadInternalResource<Texture2D>($"NaninovelIcon{px}"));
            EditIcon = new(Engine.LoadInternalResource<Texture2D>("EditMetaIcon"), "Edit actor metadata.");
            ScriptAssetIcon = new(Engine.LoadInternalResource<Texture2D>("ScriptAssetIcon"));
        }
    }
}
