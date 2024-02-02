namespace PowerUtilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.UI;

    /// <summary>
    /// RenderSettings.ambientXXX
    /// </summary>
    [Serializable]
    public class AmbientInfo
    {
        public bool isUpdateAmbient;
        [Header("Tri colors")]
        [ColorUsage(false,true)] public Color ambientSkyColor;
        [ColorUsage(false, true)] public Color ambientEquatorColor,ambientGroundColor;

        [Header("Mode")]
        public AmbientMode ambientMode = AmbientMode.Skybox;

        [Header("Ambient")]
        public float ambientIntensity = 1;

        [ColorUsage(false, true)]
        public Color ambientLight;

        [Header("AmbientProbe")]
        public bool isUpdateAmbientProbe; 
        [HideInInspector]
        public SphericalHarmonicsL2 lastAmbientProbe;
        public Color shAmbientLight;
        public Light shLight;
    }

    [Serializable]
    public class UnityFogInfo
    {
        public bool isUpdateFog;
        [Header("Fog")]
        public bool fogEnabled;
        public Color fogColor = Color.gray;
        public FogMode fogMode = FogMode.Linear;

        [Header("Linear Fog")]
        public float fogStartDistance = 10;
        public float fogEndDistance = 100;

        [Header("Exp Fog")]
        public float fogDentisy = 0.01f;
    }
    /// <summary>
    /// DaytimeAnimation 间接控制的unity属性
    /// 比如 RenderSettings的属性
    /// </summary>
    [ExecuteInEditMode]
    public class DaytimeForwardParams : MonoBehaviour
    {
        [Header("Upadte Frequency")]
        [Tooltip("how many frames update once")]
        [Min(1)] public int updateFrameCount = 3;
        int frameCount;

        [Header("--- Ambient Settings")]
        public AmbientInfo ambientInfo = new AmbientInfo();


        [Header("--- Unity Fog")]
        public UnityFogInfo fogInfo = new UnityFogInfo();

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
            RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;

            // keep last probe
            ambientInfo.lastAmbientProbe = RenderSettings.ambientProbe;
        }
        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
        }

        private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera cam)
        {
            //if (cam.CompareTag("MainCamera"))
            {
                UpdateAmbientProbe(ambientInfo);
            }
        }

        // Update is called once per frame
        void Update()
        {
            frameCount++;
            if (frameCount < updateFrameCount)
                return;
            frameCount = 0;

            UpdateAmbient(ambientInfo);
            UpdateFog(fogInfo);
        }

        private void UpdateAmbient(AmbientInfo info)
        {
            if (info == null || !info.isUpdateAmbient)
                return;

            RenderSettings.ambientSkyColor = info.ambientSkyColor;
            RenderSettings.ambientEquatorColor = info.ambientEquatorColor;
            RenderSettings.ambientGroundColor = info.ambientGroundColor;
            RenderSettings.ambientIntensity = info.ambientIntensity;
            RenderSettings.ambientLight = info.ambientLight;
            RenderSettings.ambientMode = info.ambientMode;
        }

        private static void UpdateAmbientProbe(AmbientInfo info)
        {
            if (info == null || !info.isUpdateAmbientProbe)
                return;

            var probe = info.lastAmbientProbe;
            probe.AddAmbientLight(info.shAmbientLight);
            if (info.shLight)
                probe.AddLight(info.shLight, Vector3.zero);

            RenderSettings.ambientProbe = probe;
        }

        private void UpdateFog(UnityFogInfo fogInfo)
        {
            if(fogInfo == null || !fogInfo.isUpdateFog)
                return;

            RenderSettings.fog = fogInfo.fogEnabled;

            RenderSettings.fogEndDistance = fogInfo.fogEndDistance;
            RenderSettings.fogStartDistance = fogInfo.fogStartDistance;

            RenderSettings.fogColor = fogInfo.fogColor;
            RenderSettings.fogMode = fogInfo.fogMode;
            RenderSettings.fogDensity = fogInfo.fogDentisy;
        }

    }
}