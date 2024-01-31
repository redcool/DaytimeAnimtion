namespace PowerUtilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    /// RenderSettings.ambientXXX
    /// </summary>
    [Serializable]
    public class AmbientInfo
    {
        public bool isUpdateAmbient;
        [Header("Tri colors")]
        public Color ambientSkyColor;
        public Color ambientEquatorColor,ambientGroundColor;

        [Header("Mode")]
        public AmbientMode ambientMode = AmbientMode.Skybox;

        [Header("Ambient")]
        public float ambientIntensity = 1;
        public Color ambientLight;

        //[Header("AmbientProbe")]
        //public bool isUpdateAmbientProbe;
        //public SphericalHarmonicsL2 ambientProbe;
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


        // Update is called once per frame
        void Update()
        {
            frameCount++;
            if (frameCount < updateFrameCount)
                return;
            frameCount = 0;

            if (ambientInfo!=null && ambientInfo.isUpdateAmbient)
                UpdateAmbient(ambientInfo);

            if(fogInfo != null && fogInfo.isUpdateFog)
                UpdateFog(fogInfo);
        }

        private void UpdateAmbient(AmbientInfo info)
        {
            RenderSettings.ambientSkyColor = info.ambientSkyColor;
            RenderSettings.ambientEquatorColor = info.ambientEquatorColor;
            RenderSettings.ambientGroundColor = info.ambientGroundColor;
            RenderSettings.ambientIntensity = info.ambientIntensity;
            RenderSettings.ambientLight = info.ambientLight;
            RenderSettings.ambientMode = info.ambientMode;

            //if(info.isUpdateAmbientProbe)
            //    RenderSettings.ambientProbe = info.ambientProbe;
        }

        void EnableFog(bool isEnalbed)
        {
            RenderSettings.fog = isEnalbed;
            //Shader.SetGlobalInt(WeatherShader.IS_FOG_ON, fogEnabled ? 1 : 0);
        }

        private void UpdateFog(UnityFogInfo fogInfo)
        {
            RenderSettings.fogEndDistance = fogInfo.fogEndDistance;
            RenderSettings.fogStartDistance = fogInfo.fogStartDistance;
            EnableFog(fogInfo.fogEnabled);

            RenderSettings.fogColor = fogInfo.fogColor;
            RenderSettings.fogMode = fogInfo.fogMode;
            RenderSettings.fogDensity = fogInfo.fogDentisy;
        }

    }
}