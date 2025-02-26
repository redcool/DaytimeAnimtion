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
        [Header("Mode")]
        public AmbientMode ambientMode = AmbientMode.Skybox;
        [Header("Tri colors")]
        [ColorUsage(false,true)] public Color ambientSkyColor;
        [ColorUsage(false, true)] public Color ambientEquatorColor,ambientGroundColor;


        [Header("Ambient")]
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
        public Light shLight;

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

            subtractiveShadowColor = RenderSettings.subtractiveShadowColor;
            haloStrength = RenderSettings.haloStrength;
            flareStrength = RenderSettings.flareStrength;
            // reflection
            reflectionBounces = RenderSettings.reflectionBounces;
            reflectionIntensity= RenderSettings.reflectionIntensity;
            customReflectionTexture = RenderSettings.customReflectionTexture;
            defaultReflectionMode = RenderSettings.defaultReflectionMode;
            defaultReflectionResolution = RenderSettings.defaultReflectionResolution;

        }

        public void ApplyAmbient()
        {
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
            RenderSettings.ambientGroundColor = ambientGroundColor;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.ambientLight = ambientLight;
            RenderSettings.ambientMode = ambientMode;

            RenderSettings.sun = sunSource;
            RenderSettings.skybox = skyboxMat;

            RenderSettings.subtractiveShadowColor = subtractiveShadowColor;
            RenderSettings.haloStrength = haloStrength;
            RenderSettings.flareStrength = flareStrength;
            // reflection
            RenderSettings.reflectionBounces = reflectionBounces;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            RenderSettings.customReflectionTexture = customReflectionTexture;
            RenderSettings.defaultReflectionMode = defaultReflectionMode;
            RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
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