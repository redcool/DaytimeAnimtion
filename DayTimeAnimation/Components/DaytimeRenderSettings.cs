namespace PowerUtilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// DaytimeAnimation 间接控制的unity属性
    /// 比如 RenderSettings的属性
    /// </summary>
    [ExecuteInEditMode]
    public class DaytimeRenderSettings : MonoBehaviour
    {
        [HelpBox]
        [SerializeField]
        string helpBox = "check(IsUpdateAmbient,IsUpdateFog,IsUpdateAmbientProbe) will start update RenderSettings, uncheck will use SceneAmbientInfo";

        [Header("Upadte Frequency")]
        [Tooltip("how many frames update once")]
        [Min(1)] public int updateFrameCount = 3;
        int frameCount;

        [EditorButton(onClickCall = "SaveSceneInfos",text = "SaveSceneRenderSettings")]
        [Tooltip("save current scene Amibent and fog")]
        public bool isSaveSceneRenderSettings;
            
        bool lastIsUpdateAmbient, lastIsUpdateFog,lastIsUpdateAmbientProbe;
        /// <summary>
        /// Ambient Settings
        /// </summary>
        [Header("--- RenderSettings/Envronment")]
        [Tooltip("uncheck use sceneAmbientInfo")]
        public bool isUpdateAmbient;
        [Tooltip("Mode")]
        public AmbientMode ambientMode = AmbientMode.Skybox;
        [Tooltip("Tri colors")]
        [ColorUsage(false, true)] public Color ambientSkyColor;
        [ColorUsage(false, true)] public Color ambientEquatorColor, ambientGroundColor;


        [Tooltip("Ambient")]
        public float ambientIntensity = 1;

        [ColorUsage(false, true)]
        public Color ambientLight;

        public Material skyboxMat;
        public Light sunSource;

        public Color subtractiveShadowColor;
        public float haloStrength;
        public float flareStrength;
        public int reflectionBounces;
        public float reflectionIntensity;
        public Texture customReflectionTexture;
        public DefaultReflectionMode defaultReflectionMode;
        public int defaultReflectionResolution;

        [Header("AmbientProbe")]
        public bool isUpdateAmbientProbe;
        [HideInInspector]
        public SphericalHarmonicsL2 lastAmbientProbe;
        public Color shAmbientLight;
        public Light[] shLights;


        /// <summary>
        /// Fog info
        /// </summary>
        [Header("--- RenderSettings/Fog")]
        [Tooltip("uncheck,use sceneFogInfo")]
        public bool isUpdateFog;
        [Header("Fog Settings")]
        public bool fogEnabled;
        [ColorUsage(false,true)]public Color fogColor = Color.gray;
        public FogMode fogMode = FogMode.Linear;

        [Tooltip("Linear Fog start")]
        public float fogStartDistance = 10;
        [Tooltip("Linear Fog end")]
        public float fogEndDistance = 100;

        [Tooltip("Exp Fog use")]
        public float fogDensity = 0.01f;

        /// <summary>
        /// current ambient info
        /// </summary>
        AmbientInfo ambientInfo = new ();
        /// <summary>
        /// current fog info
        /// </summary>
        UnityFogInfo fogInfo = new();

        [Header("Current SceneR enderSettings")]
        /// <summary>
        /// current RenderSettings params
        /// </summary>
        public AmbientInfo sceneAmbientInfo = new() { isUpdateAmbient = true };
        public UnityFogInfo sceneFogInfo = new() { isUpdateFog = true };

        private void Awake()
        {

            SaveSceneInfos();
        }

        private void OnEnable()
        {
            SceneManagerTools.AddActiveSceneChanged(OnSceneChanged);
        }

        private void OnSceneChanged(Scene a, Scene b)
        {
            SaveSceneInfos();
        }

        private void OnDisable()
        {
            SceneManagerTools.RemoveActiveSceneChanged(OnSceneChanged);
        }

        public void SaveSceneInfos()
        {
            // keep last probe
            lastAmbientProbe = RenderSettings.ambientProbe;

            sceneAmbientInfo.SaveAmbient();
            sceneFogInfo.SaveFog();
        }

        // Update is called once per frame
        void Update()
        {
            frameCount++;
            if (frameCount < updateFrameCount)
                return;
            frameCount = 0;

            UpdateAmbients();
        }

        private void LateUpdate()
        {
            if (CompareTools.CompareAndSet(ref lastIsUpdateAmbient, ref isUpdateAmbient) && !lastIsUpdateAmbient)
            {
                sceneAmbientInfo.ApplyAmbient();
            }

            if(CompareTools.CompareAndSet(ref lastIsUpdateAmbientProbe,ref isUpdateAmbientProbe) && !lastIsUpdateAmbientProbe)
            {
                RenderSettings.ambientProbe = lastAmbientProbe;
            }

            if(CompareTools.CompareAndSet(ref lastIsUpdateFog,ref isUpdateFog) && !lastIsUpdateFog)
            {
                sceneFogInfo.ApplyFog();
            }
        }

        private void UpdateAmbients()
        {
            // sync params
            ambientInfo.Sync(this);
            fogInfo.Sync(this);

            // apply to RenderSettings params
            if (ambientInfo.isUpdateAmbient)
                ambientInfo.ApplyAmbient();
            if (ambientInfo.isUpdateAmbientProbe)
                ambientInfo.ApplyAmbientProbe();

            if (fogInfo.isUpdateFog)
                fogInfo.ApplyFog();
        }
    }
}