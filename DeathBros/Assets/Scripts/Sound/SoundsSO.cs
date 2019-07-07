using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundContainer")]
public class SoundsSO : ScriptableObject
{
    public string loadSoundsFromFolder = "Sounds/";

    public List<Sound> sounds;

    //private bool loaded = false;

    //private void OnEnable()
    //{
    //    loaded = false;
    //}

    //public void LoadSounds()
    //{
    //    if (loaded == false)
    //    {
    //        for (int i = 0; i < sounds.Count; i++)
    //        {
    //            if (AudioManager.Instance != null)
    //            {
    //                AudioManager.AddSound(sounds[i]);
    //            }
    //        }

    //        loaded = true;
    //    }
    //}
}
