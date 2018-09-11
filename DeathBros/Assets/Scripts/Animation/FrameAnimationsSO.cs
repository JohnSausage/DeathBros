using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FrameAnimation/FrameAnimationsSO")]
[System.Serializable]
public class FrameAnimationsSO : ScriptableObject
{
    public List<FrameAnimation> frameAnimations;
}
