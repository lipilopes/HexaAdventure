using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffect : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSumindo;

    [SerializeField]
    AudioSource audioReaparecendo;

    public GameObject target;

    [SerializeField]
    bool gira;

    [SerializeField]
    float velocidade;

    WaitForSeconds waitUpdate = new WaitForSeconds(0.5f);

    List<ParticleSystem> particleTarget = new List<ParticleSystem>();


    IEnumerator UpdateCoroutine()
    {
        yield return waitUpdate;

        while (gameObject.activeInHierarchy)
        {
            yield return null;

            if (target == null)
                    gameObject.SetActive(false);

            if (gira)
                transform.Rotate(0, Time.deltaTime * velocidade, 0);            

            transform.position = target.transform.position;
        }
    }
    private void Start()
    {
        audioReaparecendo.loop = false;
        audioSumindo.loop = false;
    }


    private void OnEnable()
    {
        particleTarget.Clear();       

        if (target != null)
        {
            transform.position = target.transform.position;

            StartCoroutine(UpdateCoroutine());
        }
        else
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopCoroutine(UpdateCoroutine());
    }

    public void PlayAudioSumindo()
    {
        if (audioSumindo != null && !audioSumindo.isPlaying)
        {
            audioSumindo.Play();
            audioSumindo.loop = true;
        }

        if (target != null)
        {
            if (target.GetComponent<MeshRenderer>())
                if (target.GetComponent<MeshRenderer>().enabled)
                    target.GetComponent<MeshRenderer>().enabled = false;

            if (target.GetComponent<ParticleSystem>())
                if (target.GetComponent<ParticleSystem>().isPlaying)
                {
                    target.GetComponent<ParticleSystem>().Stop();
                    particleTarget.Add(target.GetComponent<ParticleSystem>());
                }

            foreach (var p in target.GetComponentsInChildren<ParticleSystem>())
            {
                if (p.isPlaying)
                {
                    p.Stop();
                    particleTarget.Add(p);
                }
            }
        }
    }

    public void PlayAudioReaparecendo()
    {
        if (audioReaparecendo != null && !audioReaparecendo.isPlaying)
        {
            audioReaparecendo.Play();
            audioReaparecendo.loop = false;

            StartCoroutine(ReaparecendoCoroutine());
        }

        if (target != null)
        {
            if (target.GetComponent<MeshRenderer>())
                if (!target.GetComponent<MeshRenderer>().enabled)
                    target.GetComponent<MeshRenderer>().enabled = true;

            foreach (var p in particleTarget)
            {
                if (p != null && !p.isPlaying)
                    p.Play();
            }

            particleTarget.Clear();
        }
    }

    IEnumerator ReaparecendoCoroutine()
    {
        while (audioReaparecendo.isPlaying)
        {
            yield return null;
        }

        target = null;
        gameObject.SetActive(false);
    }

    public bool ActiveTeleport(GameObject _target)
    {
        if (/*target != null || */gameObject.activeInHierarchy)
            return false;
        
        target = _target;            
        PlayAudioSumindo();
        gameObject.SetActive(true);

        return true;
    }

    public void ActiveTeleportVoid(GameObject _target)
    {
        if (/*target != null || */gameObject.activeInHierarchy)
            return;

        target = _target;              
        gameObject.SetActive(true);
        PlayAudioSumindo();
    }

    public bool DesactiveTeleport(GameObject _target)
    {
        if (/*target != null || */!gameObject.activeInHierarchy)
        return false;

        PlayAudioReaparecendo();

        if (audioReaparecendo == null)
        {
            target = null;
            gameObject.SetActive(false);
        }
       
        return true;
    }

    public void DesactiveTeleportVoid(GameObject _target)
    {
        if (/*target != null || */!gameObject.activeInHierarchy)
            return;

        PlayAudioReaparecendo();

        if (audioReaparecendo == null)
        {
            target = null;
            gameObject.SetActive(false);
        }
    }
}
