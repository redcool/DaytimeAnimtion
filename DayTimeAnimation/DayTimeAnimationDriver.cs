namespace PowerUtilities
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    [CustomEditor(typeof(DayTimeAnimationDriver))]
    public class DayTimeAnimationDriverEditor : Editor
    {
        public bool isShowDaytimeItems;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var inst = (DayTimeAnimationDriver)target;


            ShowExistItems(inst);

        }

        void ShowExistItems(DayTimeAnimationDriver inst)
        {
            isShowDaytimeItems = EditorGUILayout.BeginFoldoutHeaderGroup(isShowDaytimeItems, "DayTime Items");

            if (isShowDaytimeItems)
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

        [HelpBox]
        public string helpBox = "Drive all DayTimeAnimationItem";

        [Header("LifeTime")]
        [Tooltip("keep this driver across scenes")]
        public bool isDontDestroyOnLoad;

        [Tooltip("When autoDaytime is true, stop run daytime when scene unload, continue run when new scene loaded")]
        public bool isStopDaytimeWhenSceneUnloaded = true;

        [Header("(游戏)一天是(现实中)多少秒?")]
        [Min(1)]
        public float secondsADay = 30;

        [Tooltip("auto run daytime")]
        public bool autoDaytime;
        bool lastAutoDaytime;

        [Header("昼夜时间比例(0:夜,1:昼)")]
        [Range(0, 1)] public float timeRate;
        float lastRate;

        [Header("更新间隔")]
        public UpdateMode updateMode = UpdateMode.Frame2;

        float elapsedSecs;

        [Header("Runtime Info")]
        [Tooltip("current hour,24h format")]
        [EditorDisableGroup]
        public float hour; //[0,24]
        // hour integer
        [EditorDisableGroup]
        public int hourId; //[0,24]
        int lastHourId=-1;

        [EditorDisableGroup]
        [Tooltip("Total hours from start")]
        public int totalHours;

        int currentFrame;

        /// <summary>
        /// static,events
        /// </summary>
        public static DayTimeAnimationDriver Instance;

        /// <summary>
        /// When animation update, call this event
        /// </summary>
        public static event Action OnAnimationUpdate;
        /// <summary>
        /// When hour changed, call this event
        /// </summary>
        public static event Action<int> OnHourChanged;

        /// <summary>
        ///instance list 
        /// </summary>
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

            if (isDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnEnable()
        {
            if (autoDaytime)
            {
                SceneManagerTools.AddSceneUnloaded(OnSceneUnload);
                SceneManagerTools.AddSceneLoaded(OnSceneLoaded);
            }
        }

        private void OnDisable()
        {
            SceneManagerTools.RemoveSceneUnloaded(OnSceneUnload);
            SceneManagerTools.RemoveSceneLoaded(OnSceneLoaded);
        }

        void OnDestroy()
        {
            Instance = null;
            OnAnimationUpdate = null;
            OnHourChanged = null;
        }
        // Update is called once per frame
        void Update()
        {
            SyncElapsedSecs();

            if (autoDaytime)
                UpdateTimeRate();

            timeRate -= Mathf.Floor(timeRate);

            UpdateHour();

            /// ------------ drive all items
            if (CanUpdate())
            {
                UpdateAnimations();

                currentFrame = 0;
                lastRate = timeRate;
            }
        }
        void OnSceneUnload(Scene scene)
        {
            if (isStopDaytimeWhenSceneUnloaded)
            {
                autoDaytime = false;
            }
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (isStopDaytimeWhenSceneUnloaded)
            {
                autoDaytime = true;
            }
        }
        private void UpdateHour()
        {
            hour = timeRate * 24;
            hourId = (int)hour;
            // check int hour 
            if (CompareTools.CompareAndSet(ref lastHourId, hourId))
            {
                totalHours += 1;
                OnHourChanged?.Invoke(hourId);
            }
        }

        private void SyncElapsedSecs()
        {
            if (CompareTools.CompareAndSet(ref lastAutoDaytime, autoDaytime))
            {
                elapsedSecs = timeRate * secondsADay;
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

            OnAnimationUpdate?.Invoke();
        }

        void UpdateTimeRate()
        {
            elapsedSecs += Time.deltaTime;
            elapsedSecs %= secondsADay;

            timeRate = elapsedSecs / secondsADay;
        }
    }
}