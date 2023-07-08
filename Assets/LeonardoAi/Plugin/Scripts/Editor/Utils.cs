using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LeonardoAi
{
    public static class Utils
    {
        public static void LogInfo(string text)
        {
            Debug.Log("[Leonardo.ai] " + text);
        }

        public static void LogWarning(string text)
        {
            Debug.LogWarning("[Leonardo.ai] " + text);
        }

        public static void LogError(string text)
        {
            Debug.LogError("[Leonardo.ai] " + text);
        }
    }
}