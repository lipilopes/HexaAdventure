using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillMachadosRunicos : DiabinhoSkillMultiplasLancas
{
    [Space]
    [Header("Machados Runicos")]
    [SerializeField,Tooltip("Change $Buff for $OrdBuff ex: $Dur == $OrdDur, Ordem is $Ord + num")]
    protected List<DbuffBuff> _ordemAttackDbuffList = new List<DbuffBuff>();

    protected override void AttDescription()
    {
        base.AttDescription();

        int count = _ordemAttackDbuffList.Count;

        for (int i = 0; i < count; i++)
        {
            if (_ordemAttackDbuffList[i]._dbuffDuracaoMin == -1)
                _ordemAttackDbuffList[i]._dbuffDuracaoMin = currentdamage;

            if (_ordemAttackDbuffList[i]._dbuffDuracaoMax == -1)
                _ordemAttackDbuffList[i]._dbuffDuracaoMax = currentdamage;


            string ord = ("$Ord" + i);
            if (Description.Contains(ord))
            {
                AttDescription(ord, "<b>" +(i+1)+ "</b>");
            }

            string buff = ("$OrdBuff" + i);
            if (Description.Contains(buff))
            {
                AttDescription(buff, "<b>" + XmlMenuInicial.Instance.DbuffTranslate(_ordemAttackDbuffList[i]._buff) + "</b>");

                if (Description.Contains("_"))
                    AttDescription("_", " $OrdDur" + (i+1) + " de ");
            }

            string chance = ("$Ord%" + i);
            if (Description.Contains(chance))
                AttDescription(chance, "<b>" + (_ordemAttackDbuffList[i]._dbuffChance * 100).ToString() + "%</b>");

            string min = ("$OrdMin" + i);
            if (Description.Contains(min))
                AttDescription(min, "<b>" + _ordemAttackDbuffList[i]._dbuffDuracaoMin + "</b>");

            string max = ("$OrdMax" + i);
            if (Description.Contains(max))
                AttDescription(max, "<b>" + _ordemAttackDbuffList[i]._dbuffDuracaoMax + "</b>");

            string dur = ("$OrdDur" + i);
            if (Description.Contains(dur))
            {
                string Dur = "<b>" + _ordemAttackDbuffList[i]._dbuffDuracaoMin + " - " + _ordemAttackDbuffList[i]._dbuffDuracaoMax + "</b>";

                if (_ordemAttackDbuffList[i]._dbuffDuracaoMin == _ordemAttackDbuffList[i]._dbuffDuracaoMax)
                    Dur = "<b>" + _ordemAttackDbuffList[i]._dbuffDuracaoMax + "</b>";

                AttDescription(dur, Dur);
            }
        }
    }

    protected override void ShootSkill()
    {
        if (target.activeInHierarchy)
        {
            base.ShootSkill();

            //EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.DbuffTranslate(_ordemAttackDbuffList[0]._buff,true), Target, 0.5f);

            ActiveOrdemDbuffEffect(contador-1);
        }
        else
        {
            FailEndSkill();
        }

    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        base.Hit(endTurn, targetDbuff);

        if (targetDbuff == Target)
        {
            ActiveOrdemDbuff(contador - 1);

            ActiveOrdemDbuffEffect(contador - 1, active: false);
        }

        if (_shootDefault == 0 || contador >= _shootDefault)
        {
            StartCoroutine(PassiveAttack());
        }
        else
        {
            StartCoroutine(DefaultAttack());
        }
    }

    protected virtual void ActiveOrdemDbuff(int _currentAttack, GameObject _target=null)
    {
        if (_currentAttack < 0)
            _currentAttack = 0;

        if (_currentAttack < -1 && _currentAttack >= _ordemAttackDbuffList.Count)
        {
            Debug.LogError("ActiveOrdemDbuff() -> _currentAttack < -1 e >" + _ordemAttackDbuffList.Count);
            return;
        }

        if (_target == null)
                _target = Target;

            CreateDbuff(_target, _ordemAttackDbuffList[_currentAttack]._buff, _ordemAttackDbuffList[_currentAttack]._forMe, _ordemAttackDbuffList[_currentAttack]._dbuffChance, _ordemAttackDbuffList[_currentAttack]._dbuffDuracaoMin, _ordemAttackDbuffList[_currentAttack]._dbuffDuracaoMax);
    }

    protected virtual void ActiveOrdemDbuffEffect(int _currentAttack, GameObject _target = null,bool active=true)
    {
        if (_currentAttack < -1 && _currentAttack > _ordemAttackDbuffList.Count)
        {
            Debug.LogError("ActiveOrdemDbuffEffect() -> _currentAttack < -1 e >" + _ordemAttackDbuffList.Count);
            return; 
        }


            if (_target == null && skillAttack!=null)
                _target = skillAttack.gameObject;


        if (active)
            EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.DbuffTranslate(_ordemAttackDbuffList[_currentAttack]._buff, true), _target, 0.5f);

            switch (_ordemAttackDbuffList[_currentAttack]._buff)
            {
                case Dbuff.Fire:
                    if (active)
                        EffectManager.Instance.FireArmaEffect(_target);
                    else
                        EffectManager.Instance.FireArmaReset(_target);
                    break;

                case Dbuff.Envenenar:
                    if (active)
                        EffectManager.Instance.PoisonArmaEffect(_target);
                    else
                        EffectManager.Instance.PoisonArmaReset(_target);
                    break;

                case Dbuff.Petrificar:
                    if (active)
                        EffectManager.Instance.PetrifyArmaEffect(_target);
                    else
                        EffectManager.Instance.PetrifyArmaReset(_target);
                    break;

                case Dbuff.Stun:
                    if (active)
                        EffectManager.Instance.StunArmaEffect(_target);
                    else
                        EffectManager.Instance.StunArmaReset(_target);
                    break;

                case Dbuff.Bleed:
                    if (active)
                        EffectManager.Instance.BleedArmaEffect(_target);
                    else
                        EffectManager.Instance.BleedArmaReset(_target);
                    break;

                case Dbuff.Recuar:
                if (active)
                {
                    //EffectManager.Instance.BleedArmaEffect(_target);
                }
                //else
                    //EffectManager.Instance.BleedArmaReset(_target);
                break;
                case Dbuff.Chamar:
                if (active)
                {
                    //EffectManager.Instance.BleedArmaEffect(_target);
                }
                //else
                //EffectManager.Instance.BleedArmaReset(_target);
                break;
                case Dbuff.Cooldown:
                if (active)
                {
                    //EffectManager.Instance.BleedArmaEffect(_target);
                }
                //else
                //EffectManager.Instance.BleedArmaReset(_target);
                break;
                case Dbuff.Recupera_HP:
                if (active)
                {
                    //EffectManager.Instance.BleedArmaEffect(_target);
                }
                //else
                //EffectManager.Instance.BleedArmaReset(_target);
                break;
                case Dbuff.Escudo:
                if (active)
                {
                    //EffectManager.Instance.BleedArmaEffect(_target);
                }
                //else
                //EffectManager.Instance.BleedArmaReset(_target);
                break;
            }
        }    
}
