using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    public static AmbientSoundManager init;

    private void Awake()
    {
        if (init == null)
            init = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }
}
