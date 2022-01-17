using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
namespace TurbidCurrent
{
    public enum AudioSourEnum
    {
        None,
        Click,
        BackGround,
        Other,
    }
    //因为是单例模式，所以不能同时设置两个不同的audioSource
    public class AudioManager : SingleWithMon<AudioManager>
    {
        //TODO：需要增加账号唯一ID作为同一设备不同账号的设置区别；
        private const string audioSourceKey = "PlayerAudioSource_";

        private AudioSource clickAudioSource;
        private AudioSource backGroudAudioSource;
        private AudioSource otherAudioSource;
        private AudioListener onlyAudioLister;

        BundleNormal<AudioClip> bundleClick;
        BundleNormal<AudioClip> bundleBg;
        BundleNormal<AudioClip> bundleOther;

        private void Awake()
        {
            InitAudioSource();
        }
        private void InitAudioSource()
        {
            if (onlyAudioLister == null)
                onlyAudioLister = gameObject.AddComponent<AudioListener>();
            if (clickAudioSource == null)
            {
                GameObject go = new GameObject("ClickAudioSource");
                go.transform.SetParent(this.transform);
                clickAudioSource = go.AddComponent<AudioSource>();
            }
            if (backGroudAudioSource == null)
            {
                GameObject go = new GameObject("BackGroundAudioSource");
                go.transform.SetParent(this.transform);
                backGroudAudioSource = go.AddComponent<AudioSource>();
            }
            if (otherAudioSource == null)
            {
                GameObject go = new GameObject("OtherAudioSource");
                go.transform.SetParent(this.transform);
                otherAudioSource = go.AddComponent<AudioSource>();
            }
            LoadAudioClip(AudioSourEnum.BackGround, "DefaultBg");
            LoadAudioClip(AudioSourEnum.Click, "defaultclickclip");
        }

        public void LoadAudioClip(AudioSourEnum source, string audioClipName)
        {
            MDebug.Log(audioClipName);
            //点击音效
            BundleNormal<AudioClip> bundle = GetBundle(source, audioClipName);
            bundle.Load(() =>
            {
                GetAudioSorcre(source).clip = bundle.AssetObj;
                if (!backGroudAudioSource.isPlaying)
                    backGroudAudioSource.Play();
            });
        }

        public void SetAudioSourceParameters(AudioSourEnum source,bool isMute=false,bool playOnAwake=false,bool isLoop=false,float volume = 0.8f)
        {
            //TODO:设置参数，如果默认
            AudioSource audio = GetAudioSorcre(source);
            audio.mute = isMute;
            audio.playOnAwake = playOnAwake;
            audio.volume = volume;
            audio.loop = isLoop;      
        }
        private AudioSource GetAudioSorcre(AudioSourEnum sourEnum)
        {
            switch (sourEnum)
            {
                case AudioSourEnum.BackGround:                   
                    return backGroudAudioSource;
                  
                case AudioSourEnum.Click:
                    return clickAudioSource;
      
                case AudioSourEnum.Other:
                    return otherAudioSource;
                    
                case AudioSourEnum.None:                
                default:
                    return null;
            }
        }
        private BundleNormal<AudioClip> GetBundle(AudioSourEnum sourEnum,string audioClipName)
        {
            switch (sourEnum)
            {
                case AudioSourEnum.BackGround:
                    if (bundleBg == null)
                        bundleBg = new BundleNormal<AudioClip>(audioClipName);
                    return bundleBg;

                case AudioSourEnum.Click:
                    if (bundleClick == null)
                        bundleClick = new BundleNormal<AudioClip>(audioClipName);
                    return bundleClick;

                case AudioSourEnum.Other:
                    if (bundleOther == null)
                        bundleOther = new BundleNormal<AudioClip>(audioClipName);
                    return bundleOther;

                case AudioSourEnum.None:
                default:
                    return null;
            }
        }

        public void Clear()
        {
            if (bundleClick != null)
            {
                bundleClick.Release();
                bundleClick = null;
            }
        }
        //本地序列化保存声音设置相关信息；


        //public AudioSource ClickAudioSource
        //{
        //    get
        //    {
        //        if (clickAudioSource == null)
        //        {
        //            GameObject go = new GameObject("ClickAudioSource");
        //            go.transform.SetParent(this.transform);
        //            clickAudioSource = go.AddComponent<AudioSource>();
        //        }
        //        return clickAudioSource;
        //    }
        //}

        //public AudioSource BackGroundAudioSource
        //{
        //    get
        //    {
        //        if (backGroudAudioSource == null)
        //        {
        //            GameObject go = new GameObject("BackGroundAudioSource");
        //            go.transform.SetParent(this.transform);
        //            backGroudAudioSource = go.AddComponent<AudioSource>();
        //        }
        //        return backGroudAudioSource;
        //    }
        //}

        //public AudioSource OtherAudioSource
        //{
        //    get
        //    {
        //        if (otherAudioSource == null)
        //        {
        //            GameObject go = new GameObject("OtherAudioSource");
        //            go.transform.SetParent(this.transform);
        //            otherAudioSource = go.AddComponent<AudioSource>();
        //        }
        //        return otherAudioSource;
        //    }
        //}

        //public AudioListener AudioListenerOnly
        //{
        //    get
        //    {
        //        if(onlyAudioLister==null)
        //            onlyAudioLister = gameObject.AddComponent<AudioListener>();
        //        return onlyAudioLister;
        //    }
        //}
    }
}
