using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{
    //因为是单例模式，所以不能同时设置两个不同的audioSource
    public class AudioManager : SingleWithMon<AudioManager>
    {
        //TODO：需要增加账号唯一ID作为同一设备不同账号的设置区别；
        private const string audioSourceKey = "PlayerAudioSource_";

        private AudioSource clickAudioSource;
        private AudioSource backGroudAudioSource;
        private AudioSource otherAudioSource;

        public AudioSource ClickAudioSource
        {
            get
            {
                if (clickAudioSource == null)
                {
                    GameObject go = new GameObject("ClickAudioSource");
                    go.transform.SetParent(this.transform);
                    clickAudioSource = go.AddComponent<AudioSource>();
                }
                return clickAudioSource;
            }
        }

        public AudioSource BackGroundAudioSource
        {
            get
            {
                if (backGroudAudioSource == null)
                {
                    GameObject go = new GameObject("BackGroundAudioSource");
                    go.transform.SetParent(this.transform);
                    backGroudAudioSource = go.AddComponent<AudioSource>();
                }
                return backGroudAudioSource;
            }
        }

        public AudioSource OtherAudioSource
        {
            get
            {
                if (otherAudioSource == null)
                {
                    GameObject go = new GameObject("OtherAudioSource");
                    go.transform.SetParent(this.transform);
                    otherAudioSource = go.AddComponent<AudioSource>();
                }
                return otherAudioSource;
            }
        }

        private AudioListener onlyAudioLister;

        private void Awake()
        {
            InitAudioSource();
        }
        private void InitAudioSource()
        {
            if (onlyAudioLister == null)
                onlyAudioLister = gameObject.AddComponent<AudioListener>();
            
        }

        //本地序列化保存声音设置相关信息；
    }
}
