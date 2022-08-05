using UnityEngine.Audio;
using System;
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
        if (s == null)
        {
            Debug.Log("der Sound" + name + "existiert nicht");
            return;
        }
        if (s.source.isPlaying == false)
            s.source.PlayOneShot(s.clip);
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
    private void Start()
    {

        FindObjectOfType<AudioManager>().Play("Fanfare");
    }
}
