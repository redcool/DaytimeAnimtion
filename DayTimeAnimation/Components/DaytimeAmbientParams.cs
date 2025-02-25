namespace PowerUtilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    /// DaytimeAnimation 间接控制的unity属性
    /// 比如 RenderSettings的属性
    /// </summary>
    [ExecuteInEditMode]
    public class DaytimeAmbientParams : MonoBehaviour
    {
        [Header("Upadte Frequency")]
        [Tooltip("how many frames update once")]
        [Min(1)] public int updateFrameCount = 3;
        int frameCount;

        /// <summary>
        /// Ambient Settings
        /// </summary>
        [Header("--- Ambient Settings")]
        public bool isUpdateAmbient;
        [Tooltip("Tri colors")]
        [ColorUsage(false, true)] public Color ambientSkyColor;
        [ColorUsage(false, true)] public Color ambientEquatorColor, ambientGroundColor;

        [Tooltip("Mode")]
        public AmbientMode ambientMode = AmbientMode.Skybox;

        [Tooltip("Ambient")]
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
        /// Fog info
        /// </summary>
        [Header("--- Unity Fog")]
        public bool isUpdateFog;
        [Header("Fog")]
        public bool fogEnabled;
        [ColorUsage(false,true)]public Color fogColor = Color.gray;
        public FogMode fogMode = FogMode.Linear;

        [Tooltip("Linear Fog")]
        public float fogStartDistance = 10;
        public float fogEndDistance = 100;

        [Tooltip("Exp Fog")]
        public float fogDentisy = 0.01f;

        /// <summary>
        /// current ambient info
        /// </summary>
        AmbientInfo ambientInfo = new ();
        /// <summary>
        /// current fog info
        /// </summary>
        UnityFogInfo fogInfo = new();

        /// <summary>
        /// current RenderSettings params
        /// </summary>
        AmbientInfo sceneAmbientInfo = new() { isUpdateAmbient = true };
        UnityFogInfo sceneFogInfo = new() { isUpdateFog = true };

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
            RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;

            // keep last probe
            lastAmbientProbe = RenderSettings.ambientProbe;
            SaveSceneInfos();

            SceneManagerTools.AddActiveSceneChanged(OnSceneChanged);
        }

        private void OnSceneChanged(Scene a, Scene b)
        {
            SaveSceneInfos();
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
            SceneManagerTools.RemoveActiveSceneChanged(OnSceneChanged);
        }

        public void SaveSceneInfos()
        {
            sceneAmbientInfo.SaveAmbient();
            sceneFogInfo.SaveFog();
        }

        private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera cam)
        {
            //if (cam.CompareTag("MainCamera"))
            {
                ambientInfo.ApplyAmbientProbe();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //frameCount++;
            //if (frameCount < updateFrameCount)
            //    return;
            //frameCount = 0;

            // update params
            ambientInfo.Update(this);
            fogInfo.Update(this);

            // apply to RenderSettings params
            if (ambientInfo.isUpdateAmbient)
                ambientInfo.ApplyAmbient();

            if (fogInfo.isUpdateFog)
                fogInfo.ApplyFog();
        }


    }
}