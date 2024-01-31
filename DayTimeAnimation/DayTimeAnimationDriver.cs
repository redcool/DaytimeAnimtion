namespace PowerUtilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(DayTimeAnimationDriver))]
    public class DayTimeAnimationDriverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var inst = (DayTimeAnimationDriver)target;


            ShowExistItems(inst);

        }

        void ShowExistItems(DayTimeAnimationDriver inst)
        {
            inst.isShowDaytimeItems = EditorGUILayout.BeginFoldoutHeaderGroup(inst.isShowDaytimeItems, "DayTime Items");

            if (inst.isShowDaytimeItems)
            {
                if (!EditorApplication.isPlaying)
                {
                    EditorGUILayout.LabelField("Daytime Items only show when Playing");
                }

                foreach (var item in DayTimeAnimationDriver.AnimList)
                {
                    EditorGUILayout.ObjectField(item, typeof(GameObject), true);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
#endif

    /// <summary>
    /// 控制动画按时间(24h)进行播放
    /// </summary>
    public class DayTimeAnimationDriver : MonoBehaviour
    {
        public enum UpdateMode
        {
            Frame1 = 1, Frame2 = 2, Frame4 = 4, Frame8 = 8, Frame16 = 16
        }


        [Header("2.0.1")]
        [Header("(游戏)一天是(现实中)多少秒?")]
        [Min(1)]
        public float secondsADay = 30;

        public bool autoDaytime;
        bool lastAutoDaytime;

        [Header("昼夜时间比例(0:夜,1:昼)")]
        [Range(0, 1)] public float timeRate;
        float lastRate;

        [Header("更新间隔")]
        public UpdateMode updateMode = UpdateMode.Frame2;

        float elapsedSecs;
        [Header("Debug Info")]
        public float hour;

        [HideInInspector]
        public bool isShowDaytimeItems;

        public static DayTimeAnimationDriver Instance;

        public static event Action OnAnimationUpdate;

        int currentFrame;

        static List<DayTimeAnimationItem> animList = new List<DayTimeAnimationItem>();
        public static List<DayTimeAnimationItem> AnimList => animList;
        public static void Add(DayTimeAnimationItem item)
        {
            if (!animList.Contains(item))
            {
                animList.Add(item);
            }
        }
        public static void Remove(DayTimeAnimationItem item)
        {
            if (animList.Contains(item))
            {
                animList.Remove(item);
            }
        }

        void Awake()
        {
            Instance = this;
        }
        void OnDestroy()
        {
            Instance = null;
            OnAnimationUpdate = null;
        }
        // Update is called once per frame
        void Update()
        {
            SyncElapsedSecs();

            if (autoDaytime)
                UpdateTimeRate();

            timeRate -= Mathf.Floor(timeRate);

            hour = timeRate * 24;

            if (CanUpdate())
            {
                UpdateAnimations();

                currentFrame = 0;
                lastRate = timeRate;
            }
        }

        private void SyncElapsedSecs()
        {
            if (lastAutoDaytime != autoDaytime)
            {
                elapsedSecs = timeRate * secondsADay;
                lastAutoDaytime = autoDaytime;
            }
        }

        bool CanUpdate()
        {
            var remain = ++currentFrame % (int)updateMode;
            return remain == 0 && timeRate != lastRate;
        }

        private void UpdateAnimations()
        {
            foreach (var item in animList)
            {
                if (item.isActiveAndEnabled)
                    item.UpdateAnimation(timeRate);
            }

            if (OnAnimationUpdate != null)
            {
                OnAnimationUpdate();
            }
        }

        void UpdateTimeRate()
        {
            elapsedSecs += Time.deltaTime;
            elapsedSecs %= secondsADay;

            timeRate = elapsedSecs / secondsADay;
        }
    }
}