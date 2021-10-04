using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassivaArmaduraEspelhada : PassiveManager
{
    [Space]
    [Header("Armadura Espelhada")]
    [SerializeField, Tooltip("Retorna dano Normal como dano real $P0")]
    protected bool _returnDamageInReal = true;
    [SerializeField, Tooltip("% do dano Normal retornado $P1"), Range(0, 100)]
    protected int _porcentReturnDamage = 30;
    [Space]
    [SerializeField, Tooltip("Retorna dano real em forma de Dano real $P2")]
    protected bool _returnRealDamageInReal = true;
    [SerializeField, Tooltip("% do dano Real retornado $P3"), Range(0, 100)]
    protected int  _porcentReturnRealDamage = 60;
    //[Space]
    //[Header("FX")]
    //[SerializeField]
    //protected Effects _prefabArmaduraEspelhadaFx;
    //protected Effects _armaduraEspelhadaFx;
    //[SerializeField]
    //protected Effects _prefabArmaduraEspelhadaAtivaFx;
    //protected Effects _armaduraEspelhadaAtivaFx;

    protected override void Start()
    {   
        base.Start();
    }

    protected override void LoadEffects()
    {
        //if (_prefabInLoadingFx != null)
        //   _prefabInLoadingFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_Active_Fx", typeof(Effects));

        if (_prefabActiveFx == null)
            _prefabActiveFx = (Effects)Resources.Load("Passivas/Armadura_Espelhada/Armadura Espelhada Ative FX -Prefab", typeof(Effects));

        if (_prefabCanActiveFx == null)
            _prefabCanActiveFx = (Effects)Resources.Load("Passivas/Armadura_Espelhada/Armadura Espelhada FX -Prefab", typeof(Effects));

        base.LoadEffects();       
    }

    protected override void DescriptionPost()
    {
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)
            return;

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P0", "<b>" + (_returnDamageInReal ?
             "<color=yellow>" + XmlMenuInicial.Instance.Get(69) + "</color>"/*<color=yellow>Dano Real</color>*/ :
             "<color=red>" + XmlMenuInicial.Instance.Get(70) + "</color>"/*<color=red>Dano</color>*/) + "</b>");

        AttDescription("$P1", "<b>" + _porcentReturnDamage + "%</b>");

        AttDescription("$P2", "<b>" + (_returnRealDamageInReal ?
            "<color=yellow>" + XmlMenuInicial.Instance.Get(69) + "</color>"/*Dano Real*/ :
            "<color=red>" + XmlMenuInicial.Instance.Get(70) + "</color>"/*Dano*/) + "</b>");

        AttDescription("$P3", "<b>" + _porcentReturnRealDamage + "%</b>");

        _Description = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

        if (_mobSkillManager && !_AttDescriptonOnTooltip)
        {
            _mobSkillManager.Description += _Description;
            descriptionPost = true;
        }
    }

    public override string DescriptionToolType
    {
        get
        {
            string _r = "";

            if (User == null || !_AttDescriptonOnTooltip)
                return _r;

            _Description = XmlMobPassive.Instance.GetDescription(_XmlID);

            AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

            AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

            AttDescription("$P0", "<b>" + (_returnDamageInReal ?
            "<color=yellow>" + XmlMenuInicial.Instance.Get(69) + "</color>"/*<color=yellow>Dano Real</color>*/ :
            "<color=red>" + XmlMenuInicial.Instance.Get(70) + "</color>"/*<color=red>Dano</color>*/) + "</b>");

            AttDescription("$P1", "<b>" + _porcentReturnDamage + "%</b>");

            AttDescription("$P2", "<b>" + (_returnRealDamageInReal ?
                "<color=yellow>" + XmlMenuInicial.Instance.Get(69) + "</color>"/*Dano Real*/ :
                "<color=red>" + XmlMenuInicial.Instance.Get(70) + "</color>"/*Dano*/) + "</b>");

            AttDescription("$P3", "<b>" + _porcentReturnRealDamage + "%</b>");

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    public override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + (_returnDamageInReal ? 
            "<color=yellow>"+XmlMenuInicial.Instance.Get(69)+"</color>"/*<color=yellow>Dano Real</color>*/ : 
            "<color=red>"+   XmlMenuInicial.Instance.Get(70)+"</color>"/*<color=red>Dano</color>*/) + "</b>");

        AttDescription("$P1", "<b>" + _porcentReturnDamage + "%</b>");

        AttDescription("$P2", "<b>" + (_returnRealDamageInReal ?
            "<color=yellow>"+XmlMenuInicial.Instance.Get(69)+"</color>"/*Dano Real*/ : 
            "<color=red>"+   XmlMenuInicial.Instance.Get(70)+ "</color>"/*Dano*/) + "</b>");

        AttDescription("$P3", "<b>" + _porcentReturnRealDamage + "%</b>");
    }

    public override void ActivePassive(GameObject target, UnityEvent _event)
    {
        base.ActivePassive(target, _event);

        //AttDescription("$P0", "<b>" + (_returnDamageInReal ?
        //    "<color=yellow>" + XmlMenuInicial.Instance.Get(69) + "</color>"/*<color=yellow>Dano Real</color>*/ :
        //    "<color=red>" + XmlMenuInicial.Instance.Get(70) + "</color>"/*<color=red>Dano</color>*/) + "</b>");

        //AttDescription("$P1", "<b>" + _porcentReturnDamage + "%</b>");

        //AttDescription("$P2", "<b>" + (_returnRealDamageInReal ?
        //    "<color=yellow>" + XmlMenuInicial.Instance.Get(69) + "</color>"/*Dano Real*/ :
        //    "<color=red>" + XmlMenuInicial.Instance.Get(70) + "</color>"/*Dano*/) + "</b>");

        //AttDescription("$P3", "<b>" + _porcentReturnRealDamage + "%</b>");
    }

    public override void StartPassive(GameObject target, float value, params Passive[] passive)
    {
        Debug.LogError("StartPassive");

        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return;
        }

        if (_Passive.Length == 0)
        {
            foreach (var i in passive)
            {                
                if (i == Passive.GetDamage && ChanceActivePassive())
                {
                    ReturnDamage(target, value);
                }

                if (i == Passive.GetRealDamage && ChanceActivePassive())
                {
                    ReturnDamage(target, value, true);
                }

                if (i == Passive.DefenseDamage && ChanceActivePassive())
                {
                    ReturnDamage(target, value);
                }
            }
        }
        else
            foreach (var y in _Passive)
            {
                foreach (var i in passive)
                {
                    if (y._startPassive == i && ChanceActivePassive())
                    {
                        ActivePassive(target, y._eventPassiveEffect);
                    }
                }              
            }
    }

    void ReturnDamage(GameObject who, float damageValue, bool realDamage = false)
    {
        if (who == null ||
            who != null && who.GetComponent<MobHealth>() == null)
            return;

        ActiveDesativeEffect(true, false);

        CooldownReset();      

        int porcent = _porcentReturnDamage;
        bool isReal = _returnDamageInReal;

        if (realDamage)
        {
            Debug.LogError("Return Real Damage for -> "+who.name);
            porcent = _porcentReturnRealDamage;
            isReal = _returnRealDamageInReal;
        }

        float damage = (damageValue * porcent) / 100;

        if (damage > 0)
        {
            if (isReal)
                who.GetComponent<MobHealth>().RealDamage(User, damage, activePassive: false);
            else
                who.GetComponent<MobHealth>().Damage(User, damage, activePassive: false);
        }
    }
}

