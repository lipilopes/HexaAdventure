using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEffect : MobSkillAreaDamage
{
    protected override void Start()
    {
        base.Start();

        _totalTime++;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (TurnSystem.Instance)
        {
            TurnSystem.DelegateTurnEnd += TimerDesactive;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (TurnSystem.Instance)
        {
            TurnSystem.DelegateTurnEnd -= TimerDesactive;
        }
    }

    protected override IEnumerator UpdateCoroutine()
    {
        yield return null;
    }

    protected override void SkillRespawned(GameObject user, GameObject target, HexManager _hex)
    {
        Debug.LogError(name + " foi criado no " + _hex.name + " para atacar o " + target);

        base.SkillRespawned(user, target, _hex);

        EffectManager.Instance.PopUpDamageEffect(_timeOn + "/"+_totalTime, gameObject);

        transform.position = _hex.transform.position;
        transform.rotation = _hex.transform.rotation;

        if (_hex.currentMob != null && _hex.currentMob.GetComponent<MobHealth>())
            WalkInDamage(_hex.currentMob.GetComponent<MobHealth>());
    }

    public override void Damage(MobHealth target)
    {
        if (!CanAttack(target.gameObject))
            return;

        Debug.LogError(name + " attacou " + target.name);

        CameraOrbit.Instance.ChangeTarget(target.gameObject);

        Debug.LogError(name + " atacou " + target.name + " e deu " + _damage);

        float _crit = _damageCritical ? _chanceCrit : 0;

        if (_realDamage)
            target.RealDamage(_user, _damage, _crit);
        else
            target.Damage(_user, _damage, _crit);

        if (_chanceHit != 0 && _maxHit != 0)
        {
            float _critChanceHit = _maxHitDamageCritical ? _chanceCrit : 0;

            int currentDamage = 0;

            for (int i = _maxHitAreaDivideDamage; i < _maxHit + _maxHitAreaDivideDamage; i++)
            {
                if (CheckChance(_chanceHit))
                {
                    if (_maxHitRealDamage)
                    {
                        if (target.RealDamage(_user, (_damage * _porcenMaxHitDamage / 100) / i, _critChanceHit))
                        {
                            currentDamage++;

                            if (_maxHitDbuff)
                                Hit(target.gameObject, false);
                        }
                    }
                    else
                    {
                        if (target.Damage(_user, (_damage * _porcenMaxHitDamage / 100) / i, _critChanceHit))
                        {
                            currentDamage++;

                            if (_maxHitDbuff)
                                Hit(target.gameObject, false);
                        }
                    }
                }
            }

            EffectManager.Instance.PopUpDamageEffect(currentDamage + "/" + _maxHit, User);
        }

        Hit(target.gameObject, false);
    }

    protected override void WalkInDamage(MobHealth target)
    {
        if (!CanAttack(target.gameObject))
            return;

        Debug.LogError(name + " attacou " + target.name);

        if (_damageWalkIn > 0)
        {
            float _crit = _walkDamageCritical ? _chanceCrit : 0;

            if (_walkRealDamage)
                target.RealDamage(_user, _damageWalkIn, _crit);
            else
                target.Damage(_user, _damageWalkIn, _crit);
        }

        Hit(target.gameObject, true);
    }

    public override void TimerDesactive()
    {
        Attack(false);

        _timeOn--;

        if (GetComponent<ToolTipType>() !=null)
        {
            int Timer = _timeOn;
            GetComponent<ToolTipType>().AttDescrição(0,
                GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(139), "" + Timer++),//<b>Duração: {0} turno(s)</b>
                GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(139), "" + _timeOn));//<b>Duração: {0} turno(s)</b>
        }

        if (_timeOn == 0)
        {
            Desactive();
            return;
        }

        EffectManager.Instance.PopUpDamageEffect( _timeOn + "/"+_totalTime, gameObject);
    }

    public override void Desactive()
    {
        base.Desactive();
    }
}
