using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AudioManager : SingleBase<AudioManager>
{
    private AudioSource bkMusic = null;

    
    private float bkvalue = 1;

    
    private float soundvalue = 1;

    
    private GameObject soundObj = null;
    
    private List<AudioSource> audiosList = new List<AudioSource>();

    public AudioManager()
    {
        MonoManager.Instance.AddUpdateListener(Updata);
    }

    private void Updata()
    {
        
        for (int i = audiosList.Count - 1; i >= 0; i--)
        {
            if (audiosList[i])
            {
                
                if (!audiosList[i].isPlaying)
            {
                
                    GameObject.Destroy(audiosList[i]);
                    audiosList.RemoveAt(i);
               

            }
            }
        }
    }

    
    public void PlayBackMusic(string name)
    {
        
        if (bkMusic == null)
        {
            GameObject obj = new GameObject("bkMusic");
            bkMusic = obj.AddComponent<AudioSource>();
        }
        
        ResManager.Instance.LoadResAsync<AudioClip>("Music/BK/" + name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkvalue;
            bkMusic.Play();
        });
    }

    
    public void PauseBackMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    
    public void SetSoundVaule(float value)
    {
        bkvalue = value;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkvalue;
    }

    
    public void StopBackMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    
    public void PlaySound(string name, bool isloop = false, UnityAction<AudioSource> callback = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject("Sound");
        }
        
        ResManager.Instance.LoadResAsync<AudioClip>("Music/Sound/" + name, (clip) =>
        {
            AudioSource audio = soundObj.AddComponent<AudioSource>();

            audio.clip = clip;
            audio.volume = soundvalue;
            audio.loop = isloop;
            audio.Play();
            audiosList.Add(audio);
            
            if (callback != null)
                callback(audio);
        });
    }

    public void ChangeSoundValue(float value)
    {
        soundvalue = value;
        for (int i = 0; i < audiosList.Count; i++)
        {
            audiosList[i].volume = soundvalue;
        }
    }

    
    public void StopSoun(AudioSource source)
    {
        if (audiosList.Contains(source))
        {
            audiosList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}
