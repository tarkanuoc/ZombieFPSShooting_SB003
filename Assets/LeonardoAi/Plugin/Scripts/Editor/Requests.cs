using Newtonsoft.Json;

namespace LeonardoAi
{
    [System.Serializable]
    public class UploadPresignedPostRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name;

        [JsonProperty(PropertyName = "modelExtension")]
        public string ModelExtension;
    }

    [System.Serializable]
    public class TexturizeJobPayload
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("negative_prompt")]
        public string NegativePrompt { get; set; }

        [JsonProperty("sd_version")]
        public string SDVersion { get; set; }

        [JsonProperty("modelAssetId")]
        public string ModelAssetId { get; set; }

        [JsonProperty("seed")]
        public int Seed { get; set; }

        [JsonProperty("front_rotation_offset")]
        public float FrontRotationoffset { get; set; }

        [JsonProperty("preview")]
        public bool Preview { get; set; }
    }
}