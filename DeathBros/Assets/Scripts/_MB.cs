using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _MB : MonoBehaviour
{
    protected bool initialized;

    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
            initialized = true;
    }
}
