using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public Sound[] sounds;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        float defaultPitch = s.source.pitch;
        if (s == null)
        {
            Debug.Log("der Sound" + name + "existiert nicht");
            return;
        }
        if (s.source.isPlaying == false)
            s.source.Play();
    }

    IEnumerator ResetPitch(Sound _sound, float _defaultPitch, float _delay)
    {
        yield return new WaitForSeconds(_delay);

        _sound.source.pitch = _defaultPitch;
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("der Sound" + name + "existiert nicht");
            return;
        }
        s.source.Stop();
    }
}
