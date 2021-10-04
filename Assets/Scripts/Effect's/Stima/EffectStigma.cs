using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStigma : Effects
{
    [Space]
    [Header("Stigma")]
    [SerializeField]
    protected int timer;
    [SerializeField]
    protected int duration;
    [SerializeField]
    protected bool currentTurn;
    [Tooltip("Apos User Morrer,GO sera Desativada")]
    public bool desactiveThis = false;

    public int durationEffect = -1;

    protected GameObject targetStigma;
    protected GameObject user;

    /// <summary>
    /// Game Obj que tem a passiva Stigma
    /// </summary>
    protected PassivaEstigma gameObjScript;        

    protected override void OnEnable()
    {
        base.OnEnable();

        if (TurnSystem.Instance)
        {
            if (currentTurn)
                TurnSystem.DelegateTurnCurrent += TimerStigma;
            else
                TurnSystem.DelegateTurnEnd     += TimerStigma;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (TurnSystem.Instance)
        {
            if (currentTurn)
                TurnSystem.DelegateTurnCurrent -= TimerStigma;
            else
                TurnSystem.DelegateTurnEnd     -= TimerStigma;
        }
    }

    public virtual void StartStigma(PassivaEstigma gOScript,GameObject _user,GameObject _target,int _timeTurn,bool _currentTurn,int _durationEffect=-1)
    {
        gameObjScript = gOScript;

        user         = _user;

        targetStigma = _target;
        target       = targetStigma;

        duration    = _timeTurn;
        timer       = _timeTurn;

        currentTurn = _currentTurn;

        durationEffect = _durationEffect;

        Effect();
    }

    //public virtual void StartStigma(int _timeTurn)
    //{
    //    if (!CurrentStigma())
    //        return;

    //    duration = _timeTurn;
    //    timer    = _timeTurn;

    //    //currentTurn = _currentTurn;

    //    Effect();
    //}

    public virtual void TimerStigma()
    {
        if (gameObject == null 
             || !enabled
             || !gameObject.activeInHierarchy)       
            return;        

        if (desactiveThis && user != null &&
            user.GetComponent<MobManager>() && !user.GetComponent<MobManager>().Alive ||
            desactiveThis && user == null)
        {
            EndStigma();

            return;
        }

        timer--;

        if (EffectManager.Instance != null && targetStigma != null)
            EffectManager.Instance.PopUpDamageEffect((duration - timer) + "/" + duration, targetStigma);

        if (timer <= 0 && targetStigma != null)
            EndStigma();
    }

    protected virtual void Effect()
    {
        if (gameObjScript!=null)        
        gameObjScript.StigmaTargets.Add(targetStigma);
    }

    protected override void TargetDesactive()
    {
        EndStigma();

        base.TargetDesactive();
    }

    protected virtual void EndStigma()
    {
        if (!gameObject.activeInHierarchy)
            return;

        timer = 0;

        if (gameObject.GetComponent<MobHealth>())
        {
            if (EffectManager.Instance)
                EffectManager.Instance.PopUpDamageEffect("PUFF", gameObject);

            gameObject.GetComponent<MobHealth>().HitKill(null, false);
        }
        else
            gameObject.SetActive(false);
    }
}
