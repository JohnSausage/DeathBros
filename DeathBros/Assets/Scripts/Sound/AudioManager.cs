using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private SoundsSO generalSoundsSO;

    [SerializeField]
    private SoundsSO backgroundMusicSO;

    [Space]
    [SerializeField]
    private List<Sound> sounds;

    public static AudioManager Instance { get; protected set; }

    public SoundContainer[] soundContainer;
    public List<SoundsSO> UniqueSoundSOs;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    protected void Start()
    {
        FindAndLoadSounds();
    }

    public void FindAndLoadSounds()
    {
        RemoveAllAudioSources();


        LoadSounds(generalSoundsSO);
        LoadSounds(backgroundMusicSO);

        soundContainer = FindObjectsOfType<SoundContainer>();

        UniqueSoundSOs = new List<SoundsSO>();

        foreach (SoundContainer sc in soundContainer)
        {
            if (UniqueSoundSOs.Contains(sc.soundsSO) == false)
            {
                UniqueSoundSOs.Add(sc.soundsSO);
            }
        }

        for (int i = 0; i < UniqueSoundSOs.Count; i++)
        {
            LoadSounds(UniqueSoundSOs[i]);
        }


    }

    protected void LoadSounds(SoundsSO soundsSO)
    {
        GameObject parent = new GameObject(soundsSO.name);
        parent.transform.SetParent(transform);

        for (int i = 0; i < soundsSO.sounds.Count; i++)
        {
            InitSound(soundsSO.sounds[i], parent);

            sounds.Add(soundsSO.sounds[i]);
        }
    }

    protected void RemoveAllAudioSources()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void InitSound(Sound sound, GameObject parent)
    {
        sound.Source = parent.AddComponent<AudioSource>();

        sound.Source.clip = sound.clip;
        sound.Source.volume = sound.volume;
        sound.Source.loop = sound.loop;
    }

    private void InitSound(Sound sound)
    {
        sound.Source = gameObject.AddComponent<AudioSource>();

        sound.Source.clip = sound.clip;
        sound.Source.volume = sound.volume;
        sound.Source.loop = sound.loop;
    }

    public static void PlaySound(string name, bool retry = false)
    {
        Sound s = Instance.sounds.Find(x => x.name == name);

        if (s == null)
        {
            if (retry)
            {
                Instance.StartCoroutine(Instance.PlaySoundRetry(name));
            }
            else
            {
                Debug.Log(name + " sound not found!");
            }
        }
        else
        {
            s.Source.Play();
        }
    }

    public static void StopSound(string name)
    {
        if(Instance.sounds == null)
        {
            return;
        }

        Sound s = Instance.sounds.Find(x => x.name == name);

        if(s == null)
        {
            return;
        }

        s.Source.Stop();
    }

    public static void PlaySound(string name, Vector2 position)
    {
        Sound s = Instance.sounds.Find(x => x.name == name);

        if (s == null)
        {
            Debug.Log(name + " sound not found!");
        }
        else
        {
            float maxdistance = 24f;
            float volume = (maxdistance - (position - CameraController.Position).magnitude) / maxdistance;
            if (volume < 0) volume = 0;


            float oldVolume = s.Source.volume;
            s.Source.volume = s.volume * volume;


            s.Source.Play();
        }
    }

    IEnumerator PlaySoundRetry(string name)
    {
        yield return new WaitForSeconds(1);
        PlaySound(name, false);
    }
}
