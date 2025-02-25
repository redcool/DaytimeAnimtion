namespace PowerUtilities
{
    using System;
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

        [Header("Env")]
        public Material skyboxMat;
        public Light sunSource;

        /// <summary>
        /// Apply DaytimeForwardParams's params
        /// </summary>
        /// <param name="p"></param>
        public void Update(DaytimeAmbientParams p)
        {
            //isUpdateAmbient = p.isUpdateAmbient;
            //ambientSkyColor = p.ambientSkyColor;
            //ambientEquatorColor = p.ambientEquatorColor;
            //ambientGroundColor = p.ambientGroundColor;

            ReflectionTools.CopyFieldInfoValues(p, this);
        }

        /// <summary>
        /// Save RenderSettings params
        /// </summary>
        public void SaveAmbient()
        {
            ambientSkyColor = RenderSettings.ambientSkyColor;
            ambientEquatorColor = RenderSettings.ambientEquatorColor;
            ambientGroundColor = RenderSettings.ambientGroundColor;
            ambientIntensity = RenderSettings.ambientIntensity;
            ambientLight = RenderSettings.ambientLight;
            ambientMode = RenderSettings.ambientMode;

            lastAmbientProbe = RenderSettings.ambientProbe;

            sunSource = RenderSettings.sun;
            skyboxMat = RenderSettings.skybox;
        }

        public void ApplyAmbient()
        {
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
            RenderSettings.ambientGroundColor = ambientGroundColor;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.ambientLight = ambientLight;
            RenderSettings.ambientMode = ambientMode;
        }

        public void ApplyAmbientProbe()
        {
            var probe = lastAmbientProbe;
            probe.AddAmbientLight(shAmbientLight);
            if (shLight)
                probe.AddLight(shLight, Vector3.zero);

            RenderSettings.ambientProbe = probe;
        }
    }
}