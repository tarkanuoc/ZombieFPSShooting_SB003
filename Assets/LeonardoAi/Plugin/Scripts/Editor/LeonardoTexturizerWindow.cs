using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace LeonardoAi
{
    public class LeonardoTexturizerWindow : EditorWindow, IHasCustomMenu
    {
        private const string DOWNLOAD_PATH = "Assets/LeonardoAi/Texturizer/";
        private const string TRACKING_FILE = "downloadTrackingData.json";
        private const string FULL_TRACKING_FILE_PATH = DOWNLOAD_PATH + TRACKING_FILE;
        private const string API_KEY_ID = "leonardo.ai_api_key";
        private const string LEONARDO_AI_API_URL = "https://cloud.leonardo.ai/api/rest/v1/";
        private const string DEFAULT_SD_VERSION = "v2";
        private const string API_URL = "https://app.leonardo.ai/settings";

        private const float DEFAULT_MATERIAL_PARALLAX = 0.02f;
        private const int PROMPT_CHAR_LIMIT = 1000;
        private const int NEGATIVE_PROMPT_CHAR_LIMIT = 1000;
        private const int NOTIFICATION_MESSAGE_TIME = 3;

        private static readonly string[] FACING_DIRECTIONS = new string[] { "X", "-X", "Z", "-Z" };
        private static readonly float[] FACING_DIRECTION_VALUES = new float[] { 90f, -90f, 180f, 0f };

        private TrackingData m_trackingData;
        private GameObject m_lastSelectedGameObject;
        private UserDetails m_userDetails = null;
        private List<List<Editor>> m_modelMaterialViewer;
        private SelectionData m_selectionData;
        private Vector2 m_windowScrollPos;
        private Vector2 m_modelScrollPos;

        private bool[] m_modelDataInfoFoldout;

        private bool m_isUpdatingDetails = false;
        private bool m_showUserDetails = true;
        private bool m_showTexturizingDetails = true;

        private string m_apiKey = string.Empty;
        private string m_prompt = string.Empty;
        private string m_negativePrompt = string.Empty;

        // Selection criteria
        private int m_selectedModelVersion = 0;
        private int m_selectedFacingDirection = 0;
        private int m_seed = 0;
        private bool m_isPreview;

        // validation
        private bool m_invalidAPIKey = false;
        private bool m_isUploading3DModel = false;
        private bool m_isInitTextureJob = false;


        [MenuItem("Window/Leonardo.ai/Texturizer")]
        public static void ShowWindow()
        {
            LeonardoTexturizerWindow window = EditorWindow.GetWindow(typeof(LeonardoTexturizerWindow), false, "Leonardo.ai Texturizer") as LeonardoTexturizerWindow;
            
            // Doesn't work docked
            window.minSize = new Vector2(350, 600);
            window.Show();
        }

        private void OnEnable()
        {
            ImportTrackingData();
            m_apiKey = EditorPrefs.GetString(API_KEY_ID);
            UpdateUserDetails();
        }

        /// <summary>
        /// Loads in the tracking data json file for previous uploaded models and jobs
        /// </summary>
        private void ImportTrackingData()
        {
            if (!Directory.Exists(DOWNLOAD_PATH))
                Directory.CreateDirectory(DOWNLOAD_PATH);

            if (!File.Exists(FULL_TRACKING_FILE_PATH))
            {
                m_trackingData = new TrackingData();
                SaveTrackingData();
            }
            else
            {
                m_trackingData = JsonConvert.DeserializeObject<TrackingData>(File.ReadAllText(FULL_TRACKING_FILE_PATH));
                if (m_trackingData == null) 
                    m_trackingData= new TrackingData();
            }

            Repaint();
        }

        /// <summary>
        /// Updates the window based on the 3D model selected
        /// </summary>
        private void Update()
        {
            if (m_lastSelectedGameObject == Selection.activeGameObject)
                return;

            if (Selection.activeGameObject == null)
            {
                m_lastSelectedGameObject = null;
                m_selectionData = SelectionData.Default;
                Repaint();
                return;
            }
        
            m_lastSelectedGameObject = Selection.activeGameObject;

            string filePath = GetSelectedFilePath(Selection.activeGameObject);
            if (string.IsNullOrEmpty(filePath))
            {
                m_selectionData = SelectionData.Default;
                Repaint();
                return;
            }

            RefreshSelectionData(filePath);
            Repaint();
        }


        /// <summary>
        /// Get the filepath of the mesh
        /// </summary>
        /// <param name="gameObject">GameObject with mesh attached</param>
        /// <returns></returns>
        private string GetSelectedFilePath(GameObject gameObject)
        {
            string filePath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
            if (!string.IsNullOrEmpty(filePath))
                return filePath;

            MeshFilter mesh = gameObject.GetComponentInChildren<MeshFilter>();
            if (mesh == null || (mesh != null && mesh.sharedMesh == null))
                return string.Empty;

            return AssetDatabase.GetAssetPath(mesh.sharedMesh);
        }

        /// <summary>
        /// Updates the editor window with the newly selected 3D object
        /// Selects the relevant fields from the Tracking Data json and 
        /// sets up the material viewer
        /// </summary>
        /// <param name="assetPath">The file path of the selected asset</param>
        private void RefreshSelectionData(string assetPath)
        {
            // Don't allow primitives
            if (string.IsNullOrEmpty(assetPath) || assetPath == "Library/unity default resources")
                return;

            if (m_trackingData == null || m_trackingData.Models == null)
                return;

            List<ModelData> fetchedModelDatas = m_trackingData.Models.Where(model => assetPath == model.Path).ToList();

            int modelCount = fetchedModelDatas.Count();
            if (modelCount > 0)
            {
                fetchedModelDatas = fetchedModelDatas.OrderByDescending(x => x.LastWriteTimeUTC).ToList();
                m_modelDataInfoFoldout = new bool[modelCount];
                m_modelDataInfoFoldout[0] = true;

                // Add material viewer on load
                m_modelMaterialViewer = new List<List<Editor>>(modelCount);
                for (int i = 0; i < fetchedModelDatas.Count; i++)
                {
                    m_modelMaterialViewer.Add(new List<Editor>(fetchedModelDatas[i].TextureJobs.Count));
                    for (int j = 0; j < fetchedModelDatas[i].TextureJobs.Count; j++)
                    {
                        Material mat = (Material)AssetDatabase.LoadAssetAtPath(fetchedModelDatas[i].TextureJobs[j].DownloadPath + "material.mat", typeof(Material));
                        if (mat != null)
                            m_modelMaterialViewer[i].Add(Editor.CreateEditor(mat));
                        else
                            m_modelMaterialViewer[i].Add(null);
                    }
                }
            }

            ModelData[] modelDatas = fetchedModelDatas.ToArray();
            m_selectionData = new SelectionData
            {
                FileName = Path.GetFileName(assetPath),
                FileExtension = Path.GetExtension(assetPath).Substring(1),
                AssetPath = assetPath,
                GameObject = Selection.activeGameObject,
                ModelDatas = modelDatas
            };

            m_selectedModelVersion = 0;

            // update last write time
            foreach (ModelData data in modelDatas)
            {
                DateTimeOffset localTimeOffset = new DateTimeOffset(data.LastWriteTimeUTC, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow));
                data.LastWriteTime = localTimeOffset.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss tt zzz");
            }
        }

        private void OnGUI()
        {
            m_windowScrollPos = EditorGUILayout.BeginScrollView(m_windowScrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawGUI();
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws the editor window fields
        /// </summary>
        private void DrawGUI()
        {
            // API Key
            EditorGUILayout.LabelField("API Key");
            EditorGUILayout.BeginHorizontal();
            m_apiKey = EditorGUILayout.PasswordField(m_apiKey);
            if (GUILayout.Button("Submit", EditorStyles.miniButton))
            {
                m_invalidAPIKey = false;
                EditorPrefs.SetString(API_KEY_ID, m_apiKey);
                UpdateUserDetails();
            }
            EditorGUILayout.EndHorizontal();

            // Invalid API key error
            if (m_invalidAPIKey)
            {
                EditorGUILayout.HelpBox("The API key is invalid", MessageType.Error);
                if (GUILayout.Button("Get API key"))
                {
                    Application.OpenURL(API_URL);
                }
                return;
            }

            // API Key Validation
            if (string.IsNullOrEmpty(m_apiKey))
            {
                EditorGUILayout.HelpBox("An API Key is required", MessageType.Warning);
                if (GUILayout.Button("Get API key"))
                {
                    Application.OpenURL(API_URL);
                }

                return;
            }

            // User details
            if (m_userDetails == null)
                return;

            m_showUserDetails = EditorGUILayout.Foldout(m_showUserDetails, "User Details", true);
            if (m_showUserDetails)
            {
                EditorGUILayout.Space();
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Username: ", m_userDetails.User.Username);

                EditorGUI.BeginDisabledGroup(m_isUpdatingDetails);
                if (GUILayout.Button("Refresh", EditorStyles.miniButton))
                {
                    UpdateUserDetails();
                }

                EditorGUI.EndDisabledGroup();

                GUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Tokens", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Subscription: ", m_userDetails.SubscriptionTokens.ToString());
                EditorGUILayout.LabelField("Subscription GPT: ", m_userDetails.SubscriptionGPTTokens.ToString());
                EditorGUILayout.LabelField("Subscription Model: ", m_userDetails.SubscriptionModelTokens.ToString());
                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (m_selectionData.GameObject == null)
            {
                EditorGUILayout.HelpBox("A GameObject needs to be selected", MessageType.Info);
                return;
            }

            if (!string.Equals(m_selectionData.FileExtension, "obj"))
            {
                EditorGUILayout.HelpBox("File needs to be of type .obj", MessageType.Warning);
                return;
            }

            m_showTexturizingDetails = EditorGUILayout.Foldout(m_showTexturizingDetails, "Texturizing", true);
            if (m_showTexturizingDetails)
            {
                // Upload 3D model
                EditorGUI.BeginDisabledGroup(m_isUploading3DModel);
                if (GUILayout.Button("Upload 3D model"))
                {
                    Upload3DModel(m_selectionData);
                }
                EditorGUI.EndDisabledGroup();

                // No previously uploaded models
                if (m_selectionData.ModelDatas.Length == 0)
                {
                    EditorGUILayout.HelpBox("Click upload to upload this model before texturizing", MessageType.Info);
                    EditorGUILayout.HelpBox("Ensure the uploaded model has correctly unwrapped UVs", MessageType.Warning);
                    return;
                }

                EditorGUILayout.Space();

                // Texturizer criteria
                // Model version
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Model Version", "The selected model version of the 3D model to texturize"));
                m_selectedModelVersion = EditorGUILayout.Popup(m_selectedModelVersion, m_selectionData.ModelDatas.Select(x => x.LastWriteTime).ToArray());
                EditorGUILayout.EndHorizontal();

                // Seed
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Seed", "The seed used along with the prompt to generate this variation"));
                m_seed = EditorGUILayout.IntField( m_seed);
                EditorGUILayout.EndHorizontal();

                // Facing direction
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Facing Direction", "The models facing direction with no rotation"));
                m_selectedFacingDirection = EditorGUILayout.Popup(m_selectedFacingDirection, FACING_DIRECTIONS);
                EditorGUILayout.EndHorizontal();

                // Is Preview
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Is Preview", "Preview just the facing direction for quicker iterations"));
                m_isPreview = EditorGUILayout.Toggle(m_isPreview);
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.Space();

                // Prompt
                EditorGUILayout.LabelField(new GUIContent("Prompt", "Text that specifies the desired elements in the texture generation"), EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                m_prompt = EditorGUILayout.TextArea(m_prompt, GUILayout.Height(50f));
                if (EditorGUI.EndChangeCheck())
                {
                    m_prompt = m_prompt.Substring(0, Mathf.Min(m_prompt.Length, PROMPT_CHAR_LIMIT));
                }

                // Negative Prompt
                EditorGUILayout.LabelField(new GUIContent("Negative Prompt", "Text that specifies desired excluded elements in the texture generation"), EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                m_negativePrompt = EditorGUILayout.TextArea(m_negativePrompt, GUILayout.Height(50f));
                if (EditorGUI.EndChangeCheck())
                {
                    m_negativePrompt = m_negativePrompt.Substring(0, Mathf.Min(m_negativePrompt.Length, NEGATIVE_PROMPT_CHAR_LIMIT));
                }

                // Initialise texturize job
                EditorGUI.BeginDisabledGroup(m_isInitTextureJob);
                if (GUILayout.Button("Texturize"))
                {
                    TexturizeJobPayload payload = new TexturizeJobPayload
                    {
                        Prompt = m_prompt,
                        NegativePrompt = m_negativePrompt,
                        FrontRotationoffset = FACING_DIRECTION_VALUES[m_selectedFacingDirection],
                        SDVersion = DEFAULT_SD_VERSION,
                        ModelAssetId = m_selectionData.ModelDatas[m_selectedModelVersion].LeoModelId,
                        Seed = m_seed,
                        Preview = m_isPreview
                    };

                    InitialiseTexturizeJob(m_selectionData.ModelDatas[m_selectedModelVersion], payload, m_selectedModelVersion);
                }
                EditorGUI.EndDisabledGroup();
            }
            
            // Model and Texture information
            if (m_selectionData.ModelDatas != null)
            {
                EditorGUILayout.LabelField("Uploaded Models", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("The model dates are the last write time of the files uploaded", MessageType.Info);

                m_modelScrollPos = EditorGUILayout.BeginScrollView(m_modelScrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                for (int modelIdx = 0; modelIdx < m_selectionData.ModelDatas.Length; ++modelIdx)
                {
                    ModelData modelData = m_selectionData.ModelDatas[modelIdx];

                    m_modelDataInfoFoldout[modelIdx] = EditorGUILayout.Foldout(m_modelDataInfoFoldout[modelIdx], modelData.LastWriteTime);
                    if (!m_modelDataInfoFoldout[modelIdx])
                        continue;

                    // Model Id
                    EditorGUILayout.LabelField("Model Id", modelData.LeoModelId);
                    EditorGUILayout.LabelField("Model Path", modelData.Path);
                    EditorGUILayout.Space();


                    for (int jobIdx = 0; jobIdx < modelData.TextureJobs.Count; jobIdx++)
                    {
                        //The rectangle is drawn in the Editor (when MyScript is attached) with the width depending on the value of the Slider
                        TextureJobData textureJob = modelData.TextureJobs[jobIdx];
                        GUILayout.BeginVertical(EditorStyles.helpBox);

                        if (textureJob.IsPreview)
                        {
                            EditorGUILayout.LabelField($"[PREVIEW]");
                        }

                        EditorGUILayout.LabelField("Prompt", textureJob.Prompt, EditorStyles.wordWrappedLabel);
                        EditorGUILayout.LabelField("Negative Prompt", textureJob.NegativePrompt, EditorStyles.wordWrappedLabel);
                        EditorGUILayout.LabelField("Seed", textureJob.Seed.ToString(), EditorStyles.wordWrappedLabel);

                        // Material preview
                        if (m_modelMaterialViewer[modelIdx][jobIdx] != null)
                            m_modelMaterialViewer[modelIdx][jobIdx].OnPreviewGUI(GUILayoutUtility.GetRect(50, 100), EditorStyles.whiteLabel);

                        if (textureJob.Status == TextureJobStatus.PENDING
                            && GUILayout.Button("Refresh"))
                        {
                            CheckTextureJobStatus(textureJob);
                        }

                        if (textureJob.Status == TextureJobStatus.COMPLETE
                            && GUILayout.Button("Download & Apply Textures"))
                        {
                            DownloadTexturesSync(textureJob, modelIdx, jobIdx);
                        }

                        GUILayout.BeginHorizontal();

                        if (textureJob.Status == TextureJobStatus.COMPLETE && GUILayout.Button("Select Material"))
                        {
                            string materialPath = textureJob.DownloadPath + "material.mat";
                            EditorUtility.FocusProjectWindow();
                            
                            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(materialPath);
                            Selection.activeObject = obj;
                        }

                        GUILayout.EndHorizontal();

                        // Failed texturize jobs
                        if (textureJob.Status == TextureJobStatus.FAILED
                            && GUILayout.Button("Retry"))
                         {
                            TexturizeJobPayload payload = new TexturizeJobPayload
                            {
                                Prompt = textureJob.Prompt,
                                NegativePrompt = textureJob.NegativePrompt,
                                FrontRotationoffset = textureJob.FrontRotationOffset,
                                SDVersion = "v2",
                                ModelAssetId = m_selectionData.ModelDatas[m_selectedModelVersion].LeoModelId
                            };

                            InitialiseTexturizeJob(modelData, payload, modelIdx);

                            m_modelMaterialViewer[modelIdx].RemoveAt(jobIdx);
                            modelData.TextureJobs.RemoveAt(jobIdx);

                            SaveTrackingData();
                        }

                        GUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// Fetches and updates user API Details by polling the Leonardo.ai API
        /// </summary>
        private async void UpdateUserDetails()
        {
            if (string.IsNullOrEmpty(m_apiKey))
                return;

            m_isUpdatingDetails = true;


            using HttpClient leonardoClient = new HttpClient();
            leonardoClient.BaseAddress = new System.Uri(LEONARDO_AI_API_URL);
            leonardoClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            leonardoClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {m_apiKey}");

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "me");
            HttpResponseMessage response = await leonardoClient.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                m_invalidAPIKey = true;
                return;
            }

            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            GetUserInformationResponse userDetailsResponse = JsonConvert.DeserializeObject<GetUserInformationResponse>(responseContent);
            m_userDetails = userDetailsResponse.UserDetails.FirstOrDefault();

            // Send VS Attribution
            VSAttribution.SendAttributionEvent("fetchUserDetails", "Leonardo.ai", m_userDetails.User.Id);

            m_isUpdatingDetails = false;
            Repaint();
        }

        /// <summary>
        /// Polls the Leonardo.ai API on the progress of the texture job
        /// </summary>
        /// <param name="textureJobData">The texture job data of the job we want to check</param>
        private async void CheckTextureJobStatus(TextureJobData textureJobData)
        {
            using HttpClient leonardoClient = new HttpClient();
            leonardoClient.BaseAddress = new System.Uri(LEONARDO_AI_API_URL);
            leonardoClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            leonardoClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {m_apiKey}");

            string endpoint = "generations-texture/" + textureJobData.LeoJobId;
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, endpoint);
            HttpResponseMessage response = await leonardoClient.SendAsync(message);
            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                EditorUtility.DisplayDialog("Check Texture Job Status", "An error occurred checking the texture job status, the response has been logged in the Unity console", "Ok");
                Utils.LogError(responseContent);
                return;
            }


            TextureJobStatusResponse data = JsonConvert.DeserializeObject<TextureJobStatusResponse>(responseContent);
            textureJobData.Status = data.GenerationData.Status;

            ShowNotification(new GUIContent($"JOB {textureJobData.Status}"), NOTIFICATION_MESSAGE_TIME);

            if (textureJobData.Status == TextureJobStatus.COMPLETE)
            {
                textureJobData.Prompt = data.GenerationData.Prompt;
                textureJobData.NegativePrompt = data.GenerationData.NegativePrompt;
                textureJobData.Seed = data.GenerationData.Seed;
                textureJobData.GenerationTextureDatas = data.GenerationData.GenerationTextureDatas;

                SaveTrackingData();
                UpdateUserDetails();
            }
        }

        /// <summary>
        /// Downloads the textures and assigns them to the material
        /// </summary>
        /// <param name="textureJobData">The texture job data being downloaded</param>
        /// <param name="modelIdx">The index of the model in the loaded model set</param>
        /// <param name="jobIdx">The index of the job in the loaded job set</param>
        private void DownloadTexturesSync(TextureJobData textureJobData, int modelIdx, int jobIdx)
        {
            // Get or add renderer to selected model
            Renderer rend = Selection.activeGameObject.GetComponentInChildren<MeshRenderer>();
            if (!rend)
            {
                MeshFilter mf = Selection.activeGameObject.GetComponent<MeshFilter>();
                rend = mf.gameObject.AddComponent<MeshRenderer>();
            }

            if (!Directory.Exists(textureJobData.DownloadPath))
                Directory.CreateDirectory(textureJobData.DownloadPath);

            // Download textures
            using System.Net.WebClient webClient = new System.Net.WebClient();
            foreach (GenerationTextureData textureData in textureJobData.GenerationTextureDatas)
            {
                string texturePath = Path.Combine(textureJobData.DownloadPath, textureData.Type + ".jpg");

                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
                if (tex == null)
                {
                    webClient.DownloadFile(new System.Uri(textureData.Url), texturePath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Create material
            string materialPath = textureJobData.DownloadPath + "material.mat";
            Material mat =  (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
            if (mat == null)
            {
                // is using pipeline
                if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
                    mat = new Material(UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.defaultMaterial);
                else
                    mat = new Material(Shader.Find("Standard"));

                AssetDatabase.CreateAsset(mat, textureJobData.DownloadPath + "material.mat");
            }

            foreach (GenerationTextureData textureData in textureJobData.GenerationTextureDatas)
            {
                string texturePath = Path.Combine(textureJobData.DownloadPath, textureData.Type + ".jpg");
                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
                if (tex == null)
                {
                    Debug.Log($"{texturePath} was not found, please download again");
                    continue;
                }

                switch (textureData.Type)
                {
                    case "ALBEDO": mat.mainTexture = tex; break;
                    case "ROUGHNESS": mat.SetTexture("_MetallicGlossMap", tex); break;
                    case "DISPLACEMENT": mat.SetTexture("_ParallaxMap", tex); break;
                    case "NORMAL":
                        AssetDatabase.SaveAssets();
                        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
                        importer.textureType = TextureImporterType.NormalMap;
                        mat.SetTexture("_BumpMap", tex);
                        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
                        break;
                }
            }

            mat.SetFloat("_Parallax", DEFAULT_MATERIAL_PARALLAX);
            rend.material = mat;

            m_modelMaterialViewer[modelIdx][jobIdx] = Editor.CreateEditor(mat);

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Starts a texturizer job on the Leonardo.ai API
        /// </summary>
        /// <param name="modelData">The data about the selected 3D model</param>
        /// <param name="payload">The data required for the Leonardo.api texturize job</param>
        /// <param name="modelIdx">The index of the model in the loaded model set</param>
        private async void InitialiseTexturizeJob(ModelData modelData, TexturizeJobPayload payload, int modelIdx)
        {
            m_isInitTextureJob = true;

            using HttpClient leonardoClient = new HttpClient();
            leonardoClient.BaseAddress = new System.Uri(LEONARDO_AI_API_URL);
            leonardoClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            leonardoClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {m_apiKey}");

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "generations-texture");
            message.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await leonardoClient.SendAsync(message);
            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            InitTextureJobResponse data = JsonConvert.DeserializeObject<InitTextureJobResponse>(responseContent);

            if (data.TextureGenerationJob == null)
            {
                EditorUtility.DisplayDialog("Initialise Texture Job", "The texture job could not be initialised, the response has been logged in the Unity console", "Ok");
                Utils.LogError(responseContent.ToString());
                m_isInitTextureJob = false;
                return;
            }

            TextureJobData textureJobData = new TextureJobData
            {
                LeoJobId = data.TextureGenerationJob.Id,
                Prompt = payload.Prompt,
                NegativePrompt = payload.NegativePrompt,
                Status = TextureJobStatus.PENDING,
                DownloadPath = string.Format($"{DOWNLOAD_PATH}{modelData.LeoModelId }/{data.TextureGenerationJob.Id}/"),
                Seed = payload.Seed,
                FrontRotationOffset = payload.FrontRotationoffset,
                IsPreview = m_isPreview
            };

            m_isInitTextureJob = false;
            ShowNotification(new GUIContent("Texture job initialised"), NOTIFICATION_MESSAGE_TIME);

            m_modelMaterialViewer[modelIdx].Insert(0, null);
            modelData.TextureJobs.Insert(0, textureJobData);
            SaveTrackingData();
            Repaint();
        }

        /// <summary>
        /// Uploads the 3D model to the Leonardo.ai API
        /// </summary>
        /// <param name="selectionData">Information about the 3D model selected in the scene</param>
        private async void Upload3DModel(SelectionData selectionData)
        {
            m_isUploading3DModel = true;

            long lastWriteTime = File.GetLastWriteTimeUtc(selectionData.AssetPath).Ticks;

            // Check if the model is considered uploaded locally
            if (selectionData.ModelDatas != null)
            {
                ModelData existingModelData = selectionData.ModelDatas.FirstOrDefault(model => model.Path == selectionData.AssetPath);
                if (existingModelData != null)
                {
                    double existingModelSeconds = TimeSpan.FromTicks(existingModelData.LastWriteTimeUTC).TotalSeconds;
                    double newModelSeconds = TimeSpan.FromTicks(lastWriteTime).TotalSeconds;
                    double difference = existingModelSeconds - newModelSeconds;

                    if (Math.Abs(difference) <= 1.0)
                    {
                        EditorUtility.DisplayDialog("3D Model Upload", "The model has not been changed since the previous upload, it will not be uploaded", "Ok");
                        m_isUploading3DModel = false;
                        return;
                    }
                }
            }

            using HttpClient leonardoClient = new HttpClient();
            leonardoClient.BaseAddress = new System.Uri(LEONARDO_AI_API_URL);
            leonardoClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            leonardoClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {m_apiKey}");

            // Get presigned post request from Leonardo.ai API
            UploadPresignedPostRequest presignedRequest = new UploadPresignedPostRequest
            {
                Name = selectionData.FileName,
                ModelExtension = selectionData.FileExtension
            };
            string payload = JsonConvert.SerializeObject(presignedRequest);

            HttpResponseMessage responseMessage = await leonardoClient.PostAsync("models-3d/upload", new StringContent(payload, Encoding.UTF8, "application/json"));
            string presignedResponse = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!responseMessage.IsSuccessStatusCode)
            {
                m_isUploading3DModel = false;
                EditorUtility.DisplayDialog("3D Model Upload", "The 3D model failed to upload, the response has been logged in the Unity console", "Ok");
                Utils.LogError(presignedResponse);
                return;
            }

            UploadPresignedPostResponse presignedResponseData = JsonConvert.DeserializeObject<UploadPresignedPostResponse>(presignedResponse);
            Dictionary<string, string> fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(presignedResponseData.UploadModelAsset.ModelFields);

            // Upload to storage
            HttpRequestMessage uploadRequest = new HttpRequestMessage(HttpMethod.Post, presignedResponseData.UploadModelAsset.ModelUrl);
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                foreach (KeyValuePair<string, string> field in fields)
                {
                    content.Add(new StringContent(field.Value), field.Key);
                }

                content.Add(new StreamContent(File.OpenRead(selectionData.AssetPath)), "file", selectionData.FileName);
                uploadRequest.Content = content;
                using HttpClient awsClient = new HttpClient();
                HttpResponseMessage uploadResponse = await awsClient.SendAsync(uploadRequest);
                string uploadResponseContent = uploadResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                m_isUploading3DModel = false;

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    EditorUtility.DisplayDialog("3D Model Upload", "The 3D model failed to upload, the response has been logged in the Unity console", "Ok");
                    Utils.LogError(uploadResponseContent);
                    return;
                }

                ModelData model = new ModelData
                {
                    Path = selectionData.AssetPath,
                    LeoModelId = presignedResponseData.UploadModelAsset.ModelId,
                    LastWriteTimeUTC = lastWriteTime
                };

                m_trackingData.Models.Add(model);

                ShowNotification(new GUIContent("Upload Successful"), NOTIFICATION_MESSAGE_TIME);

                SaveTrackingData();

                string assetPath = GetSelectedFilePath(Selection.activeGameObject);
                if (assetPath == m_selectionData.AssetPath)
                {
                    RefreshSelectionData(assetPath);
                    Repaint();
                }
            }
        }

        /// <summary>
        /// Serializes tracking data back to file
        /// </summary>
        private void SaveTrackingData()
        {
            File.WriteAllText(FULL_TRACKING_FILE_PATH, JsonConvert.SerializeObject(m_trackingData, Formatting.Indented));
        }

        /// <summary>
        /// Reload tracking data functionality in window context menu
        /// </summary>
        /// <param name="menu"></param>
        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reload tracking data"), false, ImportTrackingData);
        }
    }
   
}