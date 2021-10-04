using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabinhoSkillMultiplasLancas : MobSkillShootable
{
    [Space]
    [Header("Multiplas Lancas")]
    [SerializeField,Tooltip("100% de Chance de Atacar  $P0")]
    protected int _shootDefault                     = 3;


    [SerializeField, Range(0,1), Tooltip("% De chance de atacar novamente $P1")]
    protected float _chanceShootMore     = 0.65f;

    [SerializeField, Range(0, 1), Tooltip("perde % De chance de atacar novamente $P2")]
    protected float _loseChanceShootMore = 0.05f;

    protected int contador = 0;

    protected float chance = 0;

    WaitForSeconds wait = new WaitForSeconds(1.5f);

    protected override void AttDescription()
    {
        base.AttDescription();

        int dp1 = (int)((_chanceShootMore) * 100);
        int dp2 = (int)((_loseChanceShootMore) * 100);

        AttDescription("$P0", "<b>" + _shootDefault.ToString("F0") + "</b>");
        AttDescription("$P1", "<b>" + dp1.ToString("F0") + "</b>");
        AttDescription("$P2", "-<b>" + dp2.ToString("F0") + "</b>");
    }

    public override void UseSkill()
    {
        if (!CheckPlayerUseSkill()|| !CheckUseSkill())
            return;

        useSkill = true;

        mobManager.ActivePassive(Passive.StartSkill, target);

        skillAttack.endTurn = false;

        mobManager.currentTimeAttack--;

        objectSelectTouch = null;     

        target = enemyAttack.target;

        StopAllCoroutines();

        contador = 0;
        chance   = _chanceShootMore;

        if (_shootDefault != 0)
            StartCoroutine(DefaultAttack());
        else
        {
            chance += _loseChanceShootMore;
            StartCoroutine(PassiveAttack());
        }
    }

    protected IEnumerator DefaultAttack()
    {
        if (!target.activeInHierarchy)
        {
            FailEndSkill();
        }

        yield return wait;

        while (skillAttack.gameObject.activeSelf)
        {
            yield return null;
        }

        
        contador++;

        EffectManager.Instance.PopUpDamageEffect(contador + "/" + _shootDefault, User);

        ShootSkill();

        Debug.LogError(User.name + " jogou <b>" + contador + "/" + _shootDefault + "</b> lanças.");
    }

    protected IEnumerator PassiveAttack()
    {
        if (chance > 0 && enemyAttack.target.activeInHierarchy)
        {
            yield return wait;

            float V = Random.value;

            if (V <= chance)
            {
                while (skillAttack.gameObject.activeSelf)
                {
                    yield return null;
                }

                chance -= _loseChanceShootMore;

                ShootSkill();

                contador++;

                EffectManager.Instance.PopUpDamageEffect((chance*100).ToString("F0")+"%", User);

                Debug.LogError(User.name + " jogou <b>+1</b> lança Extra.");
            }
            else
            {
                FailEndSkill();
            }
        }
        else
        {
            FailEndSkill();
        }
    }

    protected override void ShootSkill()
    {
        if (target.activeInHierarchy)
        {
            base.ShootSkill();
        }
        else
        {
            FailEndSkill();
        }

    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        base.Hit(endTurn, targetDbuff);

        if (_shootDefault == 0 || contador >= _shootDefault)
        {
            StartCoroutine(PassiveAttack());
        }
        else
        {
            StartCoroutine(DefaultAttack());
        }
    }

    protected virtual void FailEndSkill()
    {
        if (mobManager.myTurn)
        {
            if (skillAttack.gameObject.activeSelf)
            {
                Invoke("FailEndSkill", 0.5f);
                return;
            }

            useSkill = false;

            EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(132), User);//Acabou

            enemyAttack.timeAttack--;

            //ResetCoolDownManager();

            Debug.LogError(User.name + " jogou " + (contador - _shootDefault) + " lanças Extras." + chance + "%");

            StopAllCoroutines();
            //mobManager.EndTurn();

            EndSkill();
        }
    }
}
