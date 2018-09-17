using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<Sound> sounds;

    public static AudioManager Instance { get; protected set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        foreach (Sound s in sounds)
        {
            InitSound(s);
            /*
            s.Source = gameObject.AddComponent<AudioSource>();

            s.Source.clip = s.clip;
            s.Source.volume = s.volume;
            s.Source.loop = s.loop;
            */
        }
    }

    private void InitSound(Sound sound)
    {
        sound.Source = gameObject.AddComponent<AudioSource>();

        sound.Source.clip = sound.clip;
        sound.Source.volume = sound.volume;
        sound.Source.loop = sound.loop;
    }

    public static void PlaySound(string name)
    {
        Sound s = Instance.sounds.Find(x => x.name == name);
        s.Source.Play();
    }

    public static void AddSound(Sound sound)
    {
        Instance.sounds.Add(sound);
        Instance.InitSound(sound);
    }
}
