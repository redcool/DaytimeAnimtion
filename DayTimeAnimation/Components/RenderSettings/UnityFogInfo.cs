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

        [Tooltip("Linear Fog start")]
        public float fogStartDistance = 10;
        [Tooltip("Linear Fog end")]
        public float fogEndDistance = 100;

        [Tooltip("Exp Fog use")]
        public float fogDensity = 0.01f;

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