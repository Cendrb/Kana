using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    class Log
    {
        public static void Info(string tag, string text)
        {
            Info(tag, text, null);
        }

        public static void Info(string tag, string text, Object context)
        {
            if (context != null)
                Debug.Log(tagify(text, tag), context);
            else
                Debug.Log(tagify(text, tag));
        }

        public static void Error(string tag, string text)
        {
            Error(tag, text, null);
        }

        public static void Error(string tag, string text, Object context)
        {
            if (context != null)
                Debug.LogError(tagify(text, tag), context);
            else
                Debug.LogError(tagify(text, tag));
        }

        public static void Warning(string tag, string text)
        {
            Warning(tag, text, null);
        }

        public static void Warning(string tag, string text, Object context)
        {
            if (context != null)
                Debug.LogWarning(tagify(text, tag), context);
            else
                Debug.LogWarning(tagify(text, tag));
        }

        public static void Exception(string tag, string reason, Exception e)
        {
            Error(tag, reason + "\nException logged bellow");
            Debug.LogException(e);
        }

        private static string tagify(string text, string tag)
        {
            if (!String.IsNullOrEmpty(tag))
                return "[" + tag + "] " + text;
            else
                return text;
        }
    }
}
