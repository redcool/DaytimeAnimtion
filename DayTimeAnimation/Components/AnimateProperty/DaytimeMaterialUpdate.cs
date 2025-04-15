namespace PowerUtilities
{
    using PowerUtilities.RenderFeatures;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 动画记录材质属性
    /// </summary>
    [ExecuteAlways]
    public class DaytimeMaterialUpdate : MonoBehaviour
    {
        [HelpBox]
        public string helpBox = "Update Material Properties by DayTimeAnimationDriver";

        public Vector4 v;
        public Matrix4x4 m;

        public Material mat;
        public bool isRunInUpdate;

        [Tooltip("binding global float variables")]
        [ListItemDraw("name:,name,value:,value", "50,100,50,")]
        public List<ShaderValue<float>> floatValues = new List<ShaderValue<float>>();

        [ListItemDraw("name:,name,value:,value", "50,100,50,")]
        public List<ShaderValue<Vector4>> vectorValues = new List<ShaderValue<Vector4>>();

        [ListItemDraw("name:,name,value:,value", "50,100,50,")]
        public List<ShaderValue<Color>> colorValues = new List<ShaderValue<Color>>();

        [ListItemDraw("name:,name,value:,value", "50,100,50,")]
        public List<ShaderValue<Texture>> textureValues = new List<ShaderValue<Texture>>();

        public void OnEnable()
        {
            if (mat)
            {
                DayTimeAnimationDriver.OnAnimationUpdate += UpdateMatProperties;
            }
        }

        public void OnDisable()
        {
            DayTimeAnimationDriver.OnAnimationUpdate -= UpdateMatProperties;
        }

        public void UpdateMatProperties()
        {
            if (!mat)
                return;

            foreach (var item in floatValues)
                if (item.IsValid) mat.SetFloat(item.name, item.value);

            foreach (var item in vectorValues)
                if (item.IsValid) mat.SetVector(item.name, item.value);

            foreach (var item in colorValues)
                if (item.IsValid) mat.SetColor(item.name, item.value);

            foreach (var item in textureValues)
                if (item.IsValid) mat.SetTexture(item.name, item.value);
        }

        void Update()
        {
            if (isRunInUpdate)
            {
                UpdateMatProperties();
            }
        }
    }
}