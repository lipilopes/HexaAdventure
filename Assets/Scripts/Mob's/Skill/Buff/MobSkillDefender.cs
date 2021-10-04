using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillDefender : MobSkillBasicDamage
{
    [Space]
    [Header("Defense")]
    [SerializeField,Tooltip("Aumenta a chance maxima de ganhar escudo")]
    protected int _bonusDefenseMin = 10;
    [SerializeField, Tooltip("Aumenta a chance maxima de ganhar escudo")]
    protected int _bonusDefenseMax = 10;
    [Space]
    [SerializeField, Range(0, 100), Tooltip("Chance de quem usou a skill Ganhar Escudo $P0")]
    protected int _chanceBonusMe;
    [SerializeField,Range(0,100), Tooltip("Porcentagem de bonus de escudo ganho caso nao for o alvo $P1")]
    protected int _porcentBonusMe;
    [SerializeField, Tooltip("Quem usou a skill Acumula escudo $Pac0")]
    protected bool _acumeleMe = false;

    protected int _minBuffMe;

    protected int _maxBuffMe;

    bool createDbuff = false;

    protected override void AttDescription()
    {
        base.AttDescription();
        if (!createDbuff)
        {
            CreateDbuff(Dbuff.Escudo, false, 1, currentdamage, currentdamage + (Random.Range(_bonusDefenseMin, _bonusDefenseMax)), _acumeleMe);
            createDbuff = true;
        }
        _maxBuffMe = currentdamage;

        _minBuffMe = _maxBuffMe * _porcentBonusMe / 100;
        //Description += "\n$Defense";
        AttDescription("$Defense",
            GameManagerScenes._gms.AttDescriçãoMult(
                XmlMobSkill.Instance.Description(-70)//"<color=blue><b>{0}</b>% de chance de {1} Ganhar <b>{2} - {3}</b> de {4} {5},<b>Caso não seja o alvo.</b></color>"
                , "" + _chanceBonusMe
                , User.GetComponent<ToolTipType>()._name
                , "" + _minBuffMe
                , "" + currentdamage
                , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Escudo)
                , _acumeleMe ? "<color=blue><b>" + XmlMenuInicial.Instance.Get(193)/*Acumula*/ + "</b> x∞.</color>" : ""));

       

    }


    public override void AttDamageAndDescription()
    {
        base.AttDamageAndDescription();

        _maxBuffMe = currentdamage;

        _minBuffMe = _maxBuffMe * _porcentBonusMe / 100;

        AttDescription("$Defense",
            GameManagerScenes._gms.AttDescriçãoMult(
                XmlMobSkill.Instance.Description(-70)//"<color=blue><b>{0}</b>% de chance de {1} Ganhar <b>{2} - {3}</b> de {4} {5},<b>Caso não seja o alvo.</b></color>"
                , "" + _chanceBonusMe
                , User.GetComponent<ToolTipType>()._name
                , "" + _minBuffMe
                , "" + currentdamage
                , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Escudo)
                , _acumeleMe ? "<color=blue><b>" + XmlMenuInicial.Instance.Get(193) + "</b> x<b>∞</b>.</color>" : ""));
    }

    public override void AttDamage()
    {       
        base.AttDamage();

        //Start();

        //AttDescription("$Defense",
        //   GameManagerScenes._gms.AttDescriçãoMult(
        //       XmlMobSkill.Instance.Description(-70)//"<color=blue><b>{0}</b>% de chance de {1} Ganhar <b>{2} - {3}</b> de {4} {5},<b>Caso não seja o alvo.</b></color>"
        //       , "" + _chanceBonusMe
        //       , User.GetComponent<ToolTipType>()._name
        //       , "" + _minBuffMe
        //       , "" + currentdamage
        //       , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Escudo)
        //       , _acumeleMe ? "<color=blue><b>" + XmlMenuInicial.Instance.Get(193) + "</b></color>" : ""));
    }

    protected override void ShootSkill()
    {
        if (target != null && User != target)
            User.transform.LookAt(target.transform);

        mobManager.ActivePassive(Passive.ShootSkill, target);

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

        if (GameManagerScenes._gms.Adm)
        {
            EffectManager.Instance.PopUpDamageEffect(mobManager.MesmoTime(RespawMob.Instance.PlayerTime) ? "<color=#055b05>" + Nome + "</color>" : "<color=#962209>" + Nome + "</color>", User);
        }



        if (skillAttack != null)
        {
            if (inMyPosition)
                skillAttack.transform.position = User.transform.position;
            else
                skillAttack.transform.position = target.transform.position;

            skillAttack.damage = 0;

            skillAttack.UseSkill(target, this);

        }
        else
        {      
            Hit(true, target);         
        }
                
        //Hit(true, target);
        //ResetCoolDownManager();
    }

    protected override void DbuffActive(GameObject targetDbuff, int index)
    {
        base.DbuffActive(targetDbuff, index);

        if (targetDbuff!=User && index>=0)
        {
            if (_DbuffBuff[index]._buff == Dbuff.Escudo)
            {
                BonusForMe();
            }
        }
    }

    protected override void DbuffFail(GameObject targetDbuff, int index)
    {
        base.DbuffFail(targetDbuff, index);

        if (targetDbuff != User && index>=0)
        {
            if (_DbuffBuff[index]._buff == Dbuff.Escudo)
            {
                BonusForMe();
            }
        }
    }

    void BonusForMe()
    {
        CreateDbuff(User, Dbuff.Escudo, true, _chanceBonusMe, _minBuffMe, _maxBuffMe,_acumeleMe);
    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        Debug.LogError(Nome+" Hit("+targetDbuff+")");

        if (User == target)
        {
            CreateDbuff(Target, Dbuff.Escudo, true, 1, currentdamage, currentdamage + (Random.Range(_bonusDefenseMin, _bonusDefenseMax)));
            _dbuffBuff[0]._forMe = true;
            DbuffActive(targetDbuff, 0);
            DbuffActive(targetDbuff,0);
            _dbuffBuff[0]._forMe = false;
        }

        base.Hit(endTurn, targetDbuff);
    }
}
