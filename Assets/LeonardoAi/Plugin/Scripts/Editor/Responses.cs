using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LeonardoAi
{
    // Presigned post response
    [System.Serializable]
    public class UploadPresignedPostResponse
    {
        [JsonProperty("uploadModelAsset")]
        public UploadModelAsset UploadModelAsset;
    }

    [System.Serializable]
    public class UploadModelAsset
    {
        [JsonProperty("modelUrl")]
        public string ModelUrl;

        [JsonProperty("modelFields")]
        public string ModelFields;

        [JsonProperty("modelId")]
        public string ModelId;
    }

    // Texture generation running
    [System.Serializable]
    public class InitTextureJobResponse
    {
        [JsonProperty("textureGenerationJob")]
        public TextureGenerationJob TextureGenerationJob;
    }

    [System.Serializable]
    public class TextureGenerationJob
    {
        [JsonProperty("id")]
        public string Id;
    }


    // Texture generation running complete
    [System.Serializable]
    public class TextureJobStatusResponse
    {
        [JsonProperty("model_asset_texture_generations_by_pk")]
        public GenerationData GenerationData;
    }

    [System.Serializable]
    public class GenerationData
    {
        [JsonProperty("id")]    
        public string Id;

        [JsonProperty("createdAt")]
        public string CreatedAt;

        [JsonProperty("prompt")]
        public string Prompt;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("negativeprompt")]
        public string NegativePrompt;

        [JsonProperty("seed")]
        public int Seed;

        [JsonProperty("model_asset_texture_images")]
        public GenerationTextureData[] GenerationTextureDatas;
    }

    [System.Serializable]
    public class GenerationTextureData
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("url")]
        public string Url;
    }

    // Get user information
    [System.Serializable]
    public class GetUserInformationResponse
    {
        [JsonProperty("user_details")]
        public List<UserDetails> UserDetails;
    }

    [System.Serializable]
    public class UserDetails
    {
        [JsonProperty("user")]
        public User User;

        [JsonProperty("subscriptionTokens")]
        public int SubscriptionTokens;

        [JsonProperty("subscriptionGptTokens")]
        public int SubscriptionGPTTokens;

        [JsonProperty("subscriptionModelTokens")]
        public int SubscriptionModelTokens;
    }

    [System.Serializable]
    public class User
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("username")]
        public string Username;
    }

    // Get Genetations By User Id
    [System.Serializable]
    public class GetGenerationsByUserIdResponse
    {
        [JsonProperty("generations")]
        public List<Generations> Generations;
    }

    [System.Serializable]
    public class Generations
    {
        [JsonProperty("prompt")]
        public string Prompt;

        [JsonProperty("negativePrompt")]
        public string NegativePrompt;

        [JsonProperty("sdVersion")]
        public string SDVersion;

        [JsonProperty("presetStyle")]
        public string PresetStyle;

        [JsonProperty("createdAt")]
        public string CreatedAt;

        [JsonProperty("guidanceScale")]
        public int GuidanceScale;

        [JsonProperty("generated_images")]
        public List<GeneratedImages> GeneratedImages;
    }

    public class GeneratedImages
    {
        [JsonProperty("url")]
        public string Url;

        [JsonProperty("nsfw")]
        public bool Nsfw;

        [JsonProperty("id")]
        public string Id;

        public List<GeneratedImageVariation> GeneratedImageVariations;
    }

    public class GeneratedImageVariation
    {
        [JsonProperty("url")]
        public string Url;

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("status")]
        public string Status; // COMPLETE

        [JsonProperty("transformType")]
        public string TransformType; // UPSCALE, NOBG
    }
}
