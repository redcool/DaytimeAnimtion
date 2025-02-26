#if UNITY_EDITOR
namespace PowerUtilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// DaytimeAnimation editor
    /// </summary>
    public class DayTimeAnimationMenu
    {
        public const string ROOT_PATH = "PowerUtilities/动画昼夜/";
        public const string CORE_PATH = ROOT_PATH + "核心组件/";
        public const string MATERIAL_PATH = ROOT_PATH + "材质组件/";
        public const string FOG_PROP_PATH = ROOT_PATH;

        [MenuItem(CORE_PATH + "添加 DayTimeAnimationDriver", priority = 2)]
        static void AddDriver()
        {
            var driver = Object.FindObjectOfType<DayTimeAnimationDriver>(true);
            if (!driver)
            {
                var go = new GameObject("DayTimeAnimationDriver");
                driver = go.AddComponent<DayTimeAnimationDriver>();
            }
            EditorGUIUtility.PingObject(driver);
        }

        /// <summary>
        /// 新建 item
        /// </summary>
        [MenuItem(CORE_PATH + "新建 DayTimeAnimationItem")]
        static void NewItem()
        {
            var items = Object.FindObjectsOfType<DayTimeAnimationItem>(true);

            var go = new GameObject("DayTimeAnimationItem_" + items.Length, new[] { typeof(DayTimeAnimationItem) });
            go.transform.SetParent(Selection.activeTransform, false);
        }


        /// <summary>
        /// 附加 item组件
        /// </summary>
        [MenuItem(CORE_PATH + "附加 DayTimeAnimationItem 到选择节点")]
        static void AttachItemComponentToSelection()
        {
            AttachComponent<DayTimeAnimationItem>(Selection.activeGameObject);
        }

        [MenuItem(MATERIAL_PATH + "附加 Material Color到选择节点")]
        static void AttachMaterialColor()
        {
            AttachComponent<DaytimeMaterialUpdate>(Selection.activeGameObject);
        }


        [MenuItem(FOG_PROP_PATH + "环境(Fog,Ambient)")]
        static void AttachForwardParams()
        {
            AttachComponent<DaytimeRenderSettings>(Selection.activeGameObject);
        }


        public static void AttachComponent<T>(GameObject go) where T : Component
        {
            if (!go)
                return;

            var item = go.GetComponent<T>();
            if (!item)
                item = go.AddComponent<T>();
            EditorGUIUtility.PingObject(item);
        }
    }
}
#endif