using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager init;

    [SerializeField]private AudioSource aSorce;
    [SerializeField] private AudioClip[] hitSFX;
    [SerializeField] private AudioClip[] missSFX;

    private void Awake()
    {
        if (init == null)
            init = this;
        else
            Destroy(gameObject);
    }

    public void playHitSfx()
    {
        int x = Random.Range(0, hitSFX.Length);
        aSorce.Stop();
        aSorce.clip = hitSFX[x];

        aSorce.pitch = Random.Range(.5f, 2);
        aSorce.volume = Random.Range(.5f, 2);

        aSorce.Play();
    }

    public void playMissSfx()
    {
        int x = Random.Range(0, missSFX.Length);
        aSorce.Stop();
        aSorce.clip = missSFX[x];

        aSorce.pitch = Random.Range(.5f, 2);
        aSorce.volume = Random.Range(.5f, 2);

        aSorce.Play();
    }

}
