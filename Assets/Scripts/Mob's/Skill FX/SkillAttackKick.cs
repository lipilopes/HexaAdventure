using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackKick : SkillAttackGoTo
{
    [Space]
    [Header("Kick Attack")]
    [SerializeField,Tooltip("Maximos de kicks que pode fazer")]
    protected int _maxKick         = 3;
    [SerializeField,Range(1,6), Tooltip("Range para o kick")]
    protected int _rangeKick       = 1;
    [SerializeField, Range(0, 100),Tooltip("Reduz dano em inimigo q ja levou kick")]
    protected int _reduzKickDamage = 10;

    protected override void Start()
    {
        _maxHit = _maxKick;

        base.Start();
    }

    protected override void Hit()
    {
        if (_currentHit <= MaxHit)
        {
            hitMainTargetEvent.Invoke();

            Debug.LogError(name + " Hit()");

            if (target.GetComponent<MobManager>())
                target.transform.LookAt(who.transform);

            if (Skill != null)
                //who.GetComponent<SkillManager>().HitSkill(target);
                Skill.Hit(false, target);

            if (_currentHit == MaxHit)
                _hit = true;

            if (_hit)
                StartCoroutine(DesactiveCouroutine());
            else
                CheckKickAgain();
        }
    }

    protected virtual void CheckKickAgain() { }
}
