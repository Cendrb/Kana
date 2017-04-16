using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Util
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
            {
                Debug.logger.Log(tag, text, context);
            }
            else
            {
                Debug.logger.Log(tag, text);
            }
        }

        public static void Error(string tag, string text)
        {
            Error(tag, text, null);
        }

        public static void Error(string tag, string text, Object context)
        {
            if (context != null)
            {
                Debug.logger.LogError(tag, text, context);
            }
            else
            {
                Debug.logger.LogError(tag, text);
            }
        }

        public static void Warning(string tag, string text)
        {
            Warning(tag, text, null);
        }

        public static void Warning(string tag, string text, Object context)
        {
            if (context != null)
            {
                Debug.logger.LogWarning(tag, text, context);
            }
            else
            {
                Debug.logger.LogWarning(tag, text);
            }
        }

        public static void Exception(string tag, string reason, Exception e)
        {
            Error(tag, reason + "\nException logged bellow");
            Debug.LogException(e);
        }
    }
}
