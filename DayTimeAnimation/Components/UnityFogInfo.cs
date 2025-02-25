namespace PowerUtilities
{
    using System;
    using UnityEngine;

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
        public float fogDensity = 0.01f;

        public void Update(DaytimeAmbientParams p)
        {
            ReflectionTools.CopyFieldInfoValues(p, this);
        }
        /// <summary>
        /// Save RenderSettings params
        /// </summary>
        public void SaveFog()
        {
            fogEnabled = RenderSettings.fog;
            fogEndDistance = RenderSettings.fogEndDistance;
            fogStartDistance = RenderSettings.fogStartDistance;
            fogColor = RenderSettings.fogColor;
            fogMode = RenderSettings.fogMode;
            fogDensity = RenderSettings.fogDensity;
        }

        public void ApplyFog()
        {
            RenderSettings.fog = fogEnabled;

            RenderSettings.fogEndDistance = fogEndDistance;
            RenderSettings.fogStartDistance = fogStartDistance;

            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogDensity = fogDensity;
        }
    }
}