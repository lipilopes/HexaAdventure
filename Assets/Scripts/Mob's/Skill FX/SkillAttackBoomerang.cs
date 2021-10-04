using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackBoomerang : SkillAttackGoTo
{

    [Space]
    [Header("Boomerang")]
    [SerializeField, Tooltip("Apos acertar o alvo fica parado para voltar")]
    protected float _delayBoomerangToBack = 0.5f;

    protected override void Start()
    {
        base.Start();

        if (MaxHit < 2)
        {
            _maxHit = 2;
        }

        velocidade = 0;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Fx.transform.rotation = new Quaternion(0, 0, 0, 0);

        iTween.Stop(Fx);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        iTween.RotateTo(Fx,
            iTween.Hash(
                "name","rotateFx"+name,
                "x",(360*2),
                "time",0.5f,
                "easetype", iTween.EaseType.linear,
                "looptype", iTween.LoopType.loop
                ));
    }

    public void HitTeste()
    {
        iTween.StopByName("rotateFx" + name);

        iTween.MoveTo(gameObject,
            iTween.Hash(
                "delay", _delayBoomerangToBack,
                "time", 0,
                "oncomplete", "BackToWho")
                );       
    }

    protected override void Hit()
    {
        Debug.LogError(name + " Hit()");

        if (target.GetComponent<MobManager>())
            target.transform.LookAt(who.transform);

        if (Skill != null)
            Skill.Hit(false,target);

        if (!_hit)
        {
            _hit = true;

            hitMainTargetEvent.Invoke();

            iTween.Stop(Fx);

            Invoke("BackToWho", _delayBoomerangToBack);
        }      
    }

    protected virtual void BackToWho()
    {
        iTween.Stop(Fx, "rotate");

        Debug.LogError("Voltando");

        iTween.RotateTo(Fx,
           iTween.Hash(
               "name", "rotateFx" + name,
               "x", (360 * 2) / -1,
               "time", 0.5f,
               "easetype", iTween.EaseType.linear,
               "looptype", iTween.LoopType.loop
                ));

        Follow = who.GetComponent<MoveController>();

        CheckPosition();
    }

    protected override void CheckPosition()
    {
        hexagonX = Solo.x;
        hexagonY = Solo.y;

        //Debug.LogError("CheckPosition");

        if (Solo.currentMob == who && _hit)
        {
            StartCoroutine(DesactiveCouroutine());

            return;
        }

        if (!_hit)
            EnemyWalk(Follow, !followTarget);
        else
            EnemyWalk(Follow, false);
    }
}
