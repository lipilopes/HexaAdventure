using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class Effects : MonoBehaviour
{
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    protected AudioSource _audio;

    public bool FXAnimatorStart
    {
        set
        {
            if (anim!=null)
            anim.SetBool("Start",value);
        }
    }

    public GameObject target;

    [SerializeField,Tooltip("Desativa obj quando estiver sem um target")]
    public bool desativaSemTarget = true;
    [SerializeField,Tooltip("Fica na position da camera")]
    protected bool camPosition = false;
    [Tooltip("obj fica girando")]
    public bool gira = false;
    public void Gira(bool _new)
    {
        gira = _new;
    }
    [SerializeField, Tooltip("Atualiza posição com o target")]
    public bool attPosition=true;
    [SerializeField, Tooltip("Toca a Animação assim q fica ativo")]
    protected bool startAnimOnEnable = false;
    [Tooltip("Velocidade em que o obj gira")]
    public float velocidade;

    [SerializeField, Tooltip("Tempo que efeito fica ativo")]
    protected float _timeActive = -1;
    public float TimeActive {get { return _timeActive; } }
    public float ChangeTimeActive { set { _timeActive = value; } }

    protected WaitForSeconds waitTimeActive;

    protected Vector3 targetPos;

    public  Vector3 MyPosition { set { targetPos = value; } }

    protected  virtual void Start()
    {
        if (GetComponent<AudioSource>()!=null && _audio==null)
        _audio = GetComponent<AudioSource>();

        if (GetComponent<Animator>() != null && anim == null)
            anim = GetComponent<Animator>();

        if (_timeActive > 0)
        {
            waitTimeActive = new WaitForSeconds(_timeActive);
        }
    }

    protected  virtual void OnEnable()
    {
        PlayAudio();
        AttPosition();

        FXAnimatorStart = startAnimOnEnable;

        if (_timeActive > 0)
        {
            StopCoroutine(TimerCoroutine());
            StartCoroutine(TimerCoroutine());
        }
    }

    protected virtual void OnDisable()
    {

    }

    public     virtual void ChangeTimerActive(float _value)
    {
        _timeActive = _value;
        waitTimeActive = new WaitForSeconds(_timeActive);
    }

    protected  virtual void Update()
    {
        if (gira)
            transform.Rotate(0, Time.deltaTime * velocidade, 0);

        if (target == null && desativaSemTarget || target!=null && !target.activeInHierarchy && desativaSemTarget || target != null && !target.activeSelf && desativaSemTarget)
        {
            if (gameObject.activeSelf)
            {
                TargetDesactive();
            }               

            return;
        }

        if (/*attPosition && */transform.position != AttPosition())
        {
            return;
        }
    }

    protected virtual void TargetDesactive()
    {
        gameObject.SetActive(false);
        target = null;
    }

    public     virtual Vector3 AttPosition()
    {
        if (target == null || !attPosition)
            return targetPos;

        if (!camPosition)
        {
            targetPos          = target.transform.position;
            transform.position = targetPos;
        }
        else
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);
            targetPos           = screenPoint;
            transform.position  = targetPos;
        }

        return targetPos;
    }

    protected  virtual void PlayAudio()
    {
        if (_audio != null && _audio.clip!=null)
        {
            _audio.Play();
            //audio.loop = true;
        }       
    }

    protected  virtual IEnumerator TimerCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return waitTimeActive;

            gameObject.SetActive(false);
        }
    }
}
