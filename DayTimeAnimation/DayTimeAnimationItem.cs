namespace PowerUtilities
{
    using System;

    using UnityEngine;
    using UnityEngine.Playables;

#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(DayTimeAnimationItem))]
    public class DayTimeAnimationItemEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var inst = target as DayTimeAnimationItem;

            inst.isShowDaytimeDriver = EditorGUILayout.BeginFoldoutHeaderGroup(inst.isShowDaytimeDriver, "Daytime Driver");

            if (inst.isShowDaytimeDriver)
            {
                if (!EditorApplication.isPlaying)
                {
                    EditorGUILayout.LabelField("Daytime Driver only show when Playing");
                }
                else
                {

                    EditorGUILayout.ObjectField(DayTimeAnimationDriver.Instance, typeof(GameObject), true);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
#endif

    /// <summary>
    /// 24h动画播放控制项
    /// 2021.03.17 重构
    ///     1 Animation,Animator use MainClip
    ///     2 PlayableDirector call Evaluate
    /// </summary>
    public class DayTimeAnimationItem : MonoBehaviour
    {
        public AnimationClip mainClip;

        PlayableDirector director;
        Animation anim;
        Animator animator;


        [HideInInspector]
        public bool isShowDaytimeDriver;

        private void Awake()
        {
            InitPlayableDirector();
            InitAnimation();
            InitAnimator();

            InitMainClip();


            // disable this item if mainClip is null
            enabled = mainClip || director;
        }

        void InitAnimator()
        {
            animator = GetComponent<Animator>();
            if (!animator)
                return;
            animator.enabled = false;
        }

        void InitAnimation()
        {
            anim = GetComponent<Animation>();
            if (!anim)
                return;

            // stop animation's play
            anim.playAutomatically = false;
            anim.Stop();
            anim.enabled = false;
        }

        private void InitPlayableDirector()
        {
            director = GetComponent<PlayableDirector>();
            if (director)
            {
                director.playOnAwake = false;
                director.Stop();
            }
        }

        private void InitMainClip()
        {
            // find mainClip from animation
            if (!mainClip)
                mainClip = FindMainClipFromAnimation(anim);

            // find mainClip from animator
            if (!mainClip)
                mainClip = FindMainClipFromAnimator(animator);
        }


        AnimationClip FindMainClipFromAnimator(Animator anim)
        {
            if (!anim || !anim.runtimeAnimatorController)
                return null;

            var clips = anim.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i])
                    return clips[i];
            }
            return null;
        }

        AnimationClip FindMainClipFromAnimation(Animation anim)
        {
            if (!anim)
                return null;

            if (anim.clip)
                return anim.clip;

            // find clip from states
            foreach (AnimationState state in anim)
            {
                if (!state.clip)
                    continue;

                return state.clip;
            }

            return null;
        }


        private void OnEnable()
        {
            DayTimeAnimationDriver.Add(this);
        }

        private void OnDisable()
        {
            DayTimeAnimationDriver.Remove(this);
        }

        public void UpdateAnimation(float timeRate)
        {
            SampleMainClip(timeRate);
            EvaluatePlayable(timeRate);
        }

        void EvaluatePlayable(float timeRate)
        {
            if (!director)
                return;

            director.time = timeRate * director.duration;
            director.Evaluate();
        }

        void SampleMainClip(float timeRate)
        {
            if (!mainClip)
                return;

            mainClip.SampleAnimation(gameObject, timeRate * mainClip.length);
        }
    }
}