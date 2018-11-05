using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FrameAnimation/FrameAnimation")]
[System.Serializable]
public class FrameAnimation : ScriptableObject
{
    //public string animationName;

    public List<Frame> frames;

    //public int Length { get { return frames.Count; } }

    public List<Damage> damages;

    void Start()
    {
        //frames = new List<Frame>();
    }

    public int Length()
    {
        int length = 0;

        for (int i = 0; i < frames.Count; i++)
        {
            length += frames[i].duration;
        }

        return length;
    }
}