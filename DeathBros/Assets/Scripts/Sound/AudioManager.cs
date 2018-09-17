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
        if (Instance == null)
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
        }

        SoundContainer[] soundContainers = (SoundContainer[])Resources.FindObjectsOfTypeAll(typeof(SoundContainer));
        foreach (var sc in soundContainers)
        {
            foreach (var s in sc.sounds)
            {
                AddSound(s);
            }
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
        Instance.InitSound(sound);
        Instance.sounds.Add(sound);
    }
}
