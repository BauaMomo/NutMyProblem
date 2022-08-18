using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
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
            s.source.pitch = defaultPitch + UnityEngine.Random.Range(-0.1f, 0.1f);
            s.source.PlayOneShot(s.clip);
            StartCoroutine(ResetPitch(s, defaultPitch, s.source.clip.length/s.source.pitch));
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
