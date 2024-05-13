using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声音管理器
/// </summary>
public class SoundManager : MonoSingleTon<SoundManager>
{
    private string path = "Sound/{0:s}";

    private List<AudioSource> asList = new List<AudioSource>();
    private AudioSource bgmAS
    {
        get
        {
            if (!_bgmAS)
            {
                _bgmAS = gameObject.AddComponent<AudioSource>();
                _bgmAS.reverbZoneMix = 0;
            }

            return _bgmAS;
        }
        set
        {
            _bgmAS = value;
        }
    }
    private AudioSource _bgmAS;

    /// <summary>
    /// 音乐音量
    /// </summary>
    /// <value></value>
    public float bgmVolume
    {
        get => _bgmVolume;
        set
        {
            bgmAS.volume = value;
            _bgmVolume = value;
        }
    }
    private float _bgmVolume;
    /// <summary>
    /// 音效音量
    /// </summary>
    /// <value></value>
    public float soundVolume
    {
        get => _soundVolume;
        set
        {
            foreach (var item in asList)
            {
                item.volume = value;
            }
            _soundVolume = value;
        }
    }
    private float _soundVolume;

    private AudioClip GetSound(string clipName)
    {
        AudioClip clip = FYTools.Resource.Load<AudioClip>(string.Format(path, clipName));

        return clip;
    }

    /// <summary>
    /// 播放背景音
    /// </summary>
    /// <param name="clipName">声音文件名</param>
    public void PlayMusic(string clipName)
    {
        var clip = GetSound(clipName);
        bgmAS.clip = clip;
        bgmAS.Play();
    }

    /// <summary>
    /// 暂停背景音
    /// </summary>
    public void StopMusic()
    {
        bgmAS.Pause();
    }
    /// <summary>
    /// 激活背景音
    /// </summary>
    public void ResumeMusic()
    {
        bgmAS.UnPause();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clipName">声音文件名</param>
    public void PlaySound(string clipName, float volume = 1)
    {
        if (string.IsNullOrEmpty(clipName)) return;
        
        var clip = GetSound(clipName);
        AudioSource audioS = null;
        audioS = asList.Find(t => !t.isPlaying);
        //如果没找到就创建一个
        if (audioS == null && asList.Count < 60)
        {
            audioS = gameObject.AddComponent<AudioSource>();
            audioS.reverbZoneMix = 0;

            asList.Add(audioS);
        }

        if (audioS == null) return;

        audioS.Stop();
        audioS.clip = clip;
        audioS.volume = volume;
        audioS.Play();
    }
}