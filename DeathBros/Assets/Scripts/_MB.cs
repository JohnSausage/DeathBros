using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _MB : MonoBehaviour
{
    protected bool initialized;

    void Awake()
    {
        if (!initialized)
            Init();
    }

    public virtual void Init()
    {
        initialized = true;
    }
}
