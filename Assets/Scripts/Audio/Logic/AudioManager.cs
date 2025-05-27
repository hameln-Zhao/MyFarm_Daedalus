using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [Header("音乐数据库")] public SoundDetailsList_SO soundDetailsData;
    public SceneSoundList_SO sceneSoundData;
    [Header("audio source")] 
    public AudioSource ambientSource;
    public AudioSource gameSource;

    private Coroutine soundRoutine;
    
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    
    [Header("Snapshots")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot ambientSnapshot;
    public AudioMixerSnapshot muteSnapshot;
    private float musicTransitionSecond = 8f;
    
    
    public float MusicStartSecond => Random.Range(5f, 15f);

    private void OnEnable()
    {
        EventHandler.AfterSceneEvent += OnAfterSceneEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
    }

    private void OnAfterSceneEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        
        if(sceneSound == null) return;

        SoundDetails ambient = soundDetailsData.GetSoundDetails(sceneSound.ambient);
        SoundDetails music = soundDetailsData.GetSoundDetails(sceneSound.music);
        
        /*PlayMusicClip(music);
        PlayAmbientClip(ambient);*/
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }

        soundRoutine = StartCoroutine(PlaySoundRoutine(music, ambient));
    }

    private IEnumerator PlaySoundRoutine(SoundDetails music, SoundDetails ambient)
    {
        if (music != null && ambient != null)
        {
            PlayAmbientClip(ambient,1f);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayMusicClip(music, musicTransitionSecond);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolume));
        gameSource.clip = soundDetails.soundClip;
        if(gameSource.isActiveAndEnabled)
            gameSource.Play();
        
        normalSnapshot.TransitionTo(transitionTime);
    }
    /// <summary>
    /// 播放环境音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolume));
        ambientSource.clip = soundDetails.soundClip;
        if(ambientSource.isActiveAndEnabled)
            ambientSource.Play();
        ambientSnapshot.TransitionTo(transitionTime);
    }
    
    private float ConvertSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }
}




