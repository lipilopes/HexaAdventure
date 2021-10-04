using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    protected int _timeStart;
    public UnityEvent _Start;
    protected WaitForSeconds _waitTimeStart;

    [Space]
    [SerializeField]
    protected int            _timeOnEnable;
    public UnityEvent        _OnEnable;
    protected WaitForSeconds _waitTimeOnEnable;


    [Space]
    [SerializeField]
    protected int            _timeOnDisable;
    public UnityEvent        _OnDisable;
    protected WaitForSeconds _waitTimeOnDisable;

    protected void Awake()
    {
        _waitTimeStart     = new WaitForSeconds(_timeStart);
        _waitTimeOnEnable  = new WaitForSeconds(_timeOnEnable);
        _waitTimeOnDisable = new WaitForSeconds(_timeOnDisable);
    }

    protected void Start()
    {
        StartCoroutine(StartCoutine());
    }
    protected IEnumerator StartCoutine()
    {
        yield return _waitTimeStart;

        _Start.Invoke();
    }

    protected void OnEnable()
    {
        StartCoroutine(OnEnableCoutine());
        OnDisable();
    }
    protected IEnumerator OnEnableCoutine()
    {
        while (!gameObject.activeInHierarchy)
        {

        }

        yield return _waitTimeOnEnable;

        _OnEnable.Invoke();
    }

    protected void OnDisable()
    {
        StartCoroutine(OnDisableCoutine());
    }
    protected IEnumerator OnDisableCoutine()
    {
        while (gameObject.activeInHierarchy)
        {

        }

        yield return _waitTimeOnDisable;

        _OnDisable.Invoke();
    }
}
