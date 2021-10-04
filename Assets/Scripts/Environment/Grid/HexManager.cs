using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HexManager : MonoBehaviour
{

    public bool free
                ,puxeItem;

    public int floorType;

    public int x, y;

    public GameObject currentMob,currentItem;

    [SerializeField] AudioClip[] walkAudio;
    AudioSource _audioSource;

     void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void WalkInHere()
    {
        if (currentItem!=null)
        {
            if (currentItem.GetComponent<PortalManager>()!=null)
                currentItem.GetComponent<PortalManager>().CheckInHere();

            if (currentItem.GetComponent<ItemRecHp>() != null)
                currentItem.GetComponent<ItemRecHp>().CheckInHere();
        }

        if (walkAudio.Length <= 0)
            return;

        _audioSource.clip = walkAudio[Random.Range(0, walkAudio.Length)];

        _audioSource.Play();
    }

    public void CurrentMobDead()
    {
        free = true;
        currentMob = null;
    }
}
