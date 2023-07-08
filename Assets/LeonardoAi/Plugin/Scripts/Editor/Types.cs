using System.Collections.Generic;
using UnityEngine;

namespace LeonardoAi
{
    public struct SelectionData
    {
        public string FileName;
        public string FileExtension;
        public string AssetPath;
        public GameObject GameObject;
        public ModelData[] ModelDatas;

        public static SelectionData Default => new SelectionData
        {
            FileName = string.Empty,
            FileExtension = string.Empty,
            AssetPath = string.Empty,
            GameObject = null,
            ModelDatas = null
        };
    }

    [System.Serializable]
    public class TrackingData
    {
        public List<ModelData> Models = new List<ModelData>();
    }

    [System.Serializable]
    public class ModelData
    {
        public string Path;
        public string LeoModelId;
        public long LastWriteTimeUTC;

        [System.NonSerialized]
        public string LastWriteTime;

        public List<TextureJobData> TextureJobs = new List<TextureJobData>();
    }

    [System.Serializable]
    public class TextureJobData
    {
        public string LeoJobId;
        public string Prompt;
        public string NegativePrompt;
        public string Status;
        public string DownloadPath;
        public int Seed;
        public float FrontRotationOffset;
        public bool IsPreview;
        public GenerationTextureData[] GenerationTextureDatas;
    }

    public static class TextureJobStatus
    {
        public const string PENDING = "PENDING";
        public const string COMPLETE = "COMPLETE";
        public const string FAILED = "FAILED";

        private const string PENDING_COLOR_AS_HEX = "#FFFF00";
        private const string COMPLETE_COLOR_AS_HEX = "#00FF00";
        private const string FAILED_COLOR_AS_HEX = "#FF0000";

        private const string FALLBACK_COLOR_AS_HEX = "#FFFFFF";

        public static string GetStatusAsColorHex(string status)
        {
            switch(status)
            {
                case PENDING: return PENDING_COLOR_AS_HEX;
                case COMPLETE: return COMPLETE_COLOR_AS_HEX;
                case FAILED: return FAILED_COLOR_AS_HEX;
            }
            return FALLBACK_COLOR_AS_HEX;
        }
    }
}
