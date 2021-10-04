using System.Collections.Generic;
using UnityEngine;

public class PassivaEspadaMagica : PassivaDBuff
{
    /*
      *Ideia #0: Rouba uma % do escudo do inimigo, ataca e fica com uma % do escudo dele depois devolve.
      *Ideia #1: Pega uma % do Escudo do inimigo por X turno(s) depois devolve.
      *Ideia #2: Se Vincula com o escudo do inimigo.[-> Inimigo e esse mob compartilham do msm escudo]

      +Ideia #0,#1,#2:Caso o inimigo não tenha escudo ganha Buff de Ataque [ +5 por 2 turnos ]

    */
    [Space]
    [Header("Espada Magica")]
    [SerializeField, Tooltip("% de escudo Absorvido do inimigo $P4"),Range(0,1f)]
    protected float _porcentDefenseAbsorvido = 0.5f;
    [SerializeField, Tooltip("Maximo de Escudo Absorvido do inimigo $P5")]
    protected int  _maxDefenseAbsorvido = 10;
    [Space]
    [SerializeField, Tooltip("% Roubo do Escudo $P6"), Range(0, 1)]
    protected float _porcentGetDefenseAbsorvido = 0.15f;
    [SerializeField, Tooltip("Minimo Roubo de Escudo Absorvido $P7")]
    protected int _minGetDefenseAbsorvido = 1;
    [SerializeField, Tooltip("Duracao em turnos do Escudo Absorvido $P8")]
    protected int _duracaoDefense = 3;

    [SerializeField, Tooltip("Alvo do roubo")]
    protected MobHealth _TargetPassive;

    /// <summary>
    /// Contador de turnos
    /// </summary>
    int Counter = 0;
    bool Active = false;
    int defenseValue=0;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (TurnSystem.Instance)
        {
            TurnSystem.DelegateTurnEnd += CountEffect;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (TurnSystem.Instance)
        {
            TurnSystem.DelegateTurnEnd -= CountEffect;
        }
    }

    protected override void DescriptionPost()
    {
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)
            return;        

        Debug.LogError("<color=red>DescriptionPost</color>");

        DbuffReDescription();

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P4", "<b>" + (
           "<color=red>" + 100*_porcentDefenseAbsorvido+ "</color>" ) + "%</b>");

        AttDescription("$P5", "<b>" + _maxDefenseAbsorvido + "</b>");

        AttDescription("$P6", "<b>" + (
           "<color=blue>" + 100 * _porcentGetDefenseAbsorvido + "</color>")  + "%</b>");

        AttDescription("$P7", "<b>" + _minGetDefenseAbsorvido + "</b>");

        AttDescription("$P8", "<b>" + _duracaoDefense + "</b>");

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
            Debug.LogError("<color=red>DescriptionToolType</color>");

            string _r = "";

            if (User == null || !_AttDescriptonOnTooltip)
                return _r;

            _Description = XmlMobPassive.Instance.GetDescription(_XmlID);

            DbuffReDescription();

            AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

            AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

            AttDescription("$P4", "<b>" + (
               "<color=red>" + 100 * _porcentDefenseAbsorvido + "</color>") + "%</b>");

            AttDescription("$P5", "<b>" + _maxDefenseAbsorvido + "</b>");

            AttDescription("$P6", "<b>" + (
               "<color=blue>" + 100 * _porcentGetDefenseAbsorvido + "</color>") + "%</b>");

            AttDescription("$P7", "<b>" + _minGetDefenseAbsorvido + "</b>");

            AttDescription("$P8", "<b>" + _duracaoDefense + "</b>");

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    public override void AttDescription()
    {
        base.AttDescription();

        DbuffReDescription();

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P4", "<b>" + (
           "<color=red>" + 100 * _porcentDefenseAbsorvido + "</color>") + "%</b>");

        AttDescription("$P5", "<b>" + _maxDefenseAbsorvido + "</b>");

        AttDescription("$P6", "<b>" + (
           "<color=blue>" + 100 * _porcentGetDefenseAbsorvido + "</color>") + "%</b>");

        AttDescription("$P7", "<b>" + _minGetDefenseAbsorvido + "</b>");

        AttDescription("$P8", "<b>" + _duracaoDefense + "</b>");
    }

    public override void StartPassive(GameObject target, float value, params Passive[] passive)
    {
        Debug.LogError("StartPassive");

        if (Active && defenseValue > 0)
        {
            foreach (var i in passive)
            {
                if (i == Passive.DefenseDamage)
                {
                    defenseValue -= (int)value;

                    if (defenseValue< 0)
                        defenseValue = 0;
                }
            }
        }

        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return;
        }

        foreach (var i in passive)
        {
            foreach (var y in _Passive)
            {
                if (y._startPassive == i)
                {
                    if (!Active && target != null && target.GetComponent<MobHealth>() && target.GetComponent<MobHealth>().Alive && ChanceActivePassive())
                    {
                        //EffectManager.Instance.PopUpDamageEffect(_Nome + " Active", User);
                        _TargetPassive = target.GetComponent<MobHealth>();
                        ActiveEffect();
                    }
                }
            }
        }
    }

    void ActiveEffect()
    {
        if (Active)
            return;

        DevolveDefense();

        ActiveDesativeEffect(true, false);

        Active = true;

        defenseValue = (int)_TargetPassive.defense;

        if (defenseValue > 0)
        {
            _TargetPassive.defense -= defenseValue;

            User.GetComponent<MobHealth>().Defense(defenseValue, _TargetPassive.gameObject);
        }
        else
            ActiveDbuffList(0, false);

        Counter = _duracaoDefense;
    }

    void CountEffect()
    {
        if (Active)
        {
            Counter--;

            if (Counter > 0)
            {

            }
            else
            {
                ActiveDesativeEffect(false, false);

                if (cooldownCurrent == -2)
                {
                    return;
                }

                DevolveDefense();
            }
        }
    }

    public override void Cooldown()
    {
        if (!Active && cooldownCurrent > 0)
        {
            base.Cooldown();

            if (cooldownCurrent<=0 && cooldownCurrent !=-2)
                ActiveDesativeEffect(false, true);
        }
    }

    void DevolveDefense()
    {
        Active = false;

        if (defenseValue > _minGetDefenseAbsorvido && _TargetPassive != null && _TargetPassive.Alive)
        {
            float Pm = (defenseValue * (1 - _porcentGetDefenseAbsorvido));

            if (Pm > _minGetDefenseAbsorvido)
            {
                User.GetComponent<MobHealth>().defense -= Pm;

                _TargetPassive.GetComponent<MobHealth>().Defense(Pm, User.gameObject);
            }
        }
    }
}
