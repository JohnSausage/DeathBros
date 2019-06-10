using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : _MB
{
    [SerializeField]
    private SoundsSO generalSoundsSO;

    [Space]
    [SerializeField]
    private List<Sound> sounds;

    public static AudioManager Instance { get; protected set; }


    public override void Init()
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

        base.Init();


        //foreach (Sound s in sounds)
        //{
        //    InitSound(s);
        //}
        //
        //SoundContainer[] soundContainers = (SoundContainer[])Resources.FindObjectsOfTypeAll(typeof(SoundContainer));
        //foreach (var sc in soundContainers)
        //{
        //    foreach (var s in sc.sounds)
        //    {
        //        AddSound(s);
        //    }
        //}

    }

    public override void LateInit()
    {
        base.LateInit();


        //foreach (Sound s in sounds)
        //{
        //    InitSound(s);
        //}

        generalSoundsSO.LoadSounds();
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

        if (s == null)
        {
            Debug.Log(name + " sound not found!");
        }
        else
        {
            s.Source.Play();
        }
    }

    public static void AddSound(Sound sound)
    {
        Instance.InitSound(sound);
        Instance.sounds.Add(sound);
    }
}
