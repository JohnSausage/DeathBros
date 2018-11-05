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

        StartCoroutine(LateUpdateRoutine());
    }

    public virtual void LateInit()
    {

    }

    protected IEnumerator LateUpdateRoutine()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        LateInit();
    }
}
