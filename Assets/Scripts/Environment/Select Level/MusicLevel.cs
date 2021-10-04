using System.Collections;
using UnityEngine;

public class MusicLevel : MonoBehaviour
{
   [SerializeField] AudioSource audioSource;
                    GameManagerScenes _gms;

    void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _gms = GameManagerScenes._gms;
    }

    public void StartMusic(bool start=true, AudioClip clip=null)
    {
        audioSource.clip = clip;

        if (start)
        {
            if (audioSource.clip!=null)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    public void StartMusic(bool start = true, int indexFase=0)
    {
        audioSource.clip = _gms.ClipFase(_gms.PlayerID-1,indexFase);

        if (start)
        {
            if (audioSource.clip != null)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
