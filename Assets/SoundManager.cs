using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundGroup
    {
        private AudioSource _audioSource;
        public AudioClip audioClip;
        public string soundName;
        public void SetAudioSource(AudioSource audioSource) => _audioSource = audioSource;
        public AudioSource GetAudioSource() => _audioSource;
    }

    public static SoundManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    public class SoundName
    {
        public static string Delete = "Delete";
        public static string LeveUp = "LeveUp";
        public static string Move = "Move";
        public static string Rotate = "Rotate";
        public static string Ground = "Ground";
    }

    public List<SoundGroup> soundList = new List<SoundGroup>();

    private void Start()
    {
        foreach (var soundGroup in soundList)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = soundGroup.audioClip;
            soundGroup.SetAudioSource(audioSource);
        }
    }

    public static void PlaySoundByName(string soundName)
    {
        for (var i = 0; i < _instance.soundList.Count; i++)
        {
            if (_instance.soundList[i].soundName.Equals(soundName))
                _instance.soundList[i].GetAudioSource().Play();
        }
    }
}