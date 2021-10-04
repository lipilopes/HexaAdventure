using UnityEngine;

public class PassiveAtaqueVampiresco : PassiveManager
{

    [Space]
    [Header("Ataque Vampiresco")]
    [SerializeField, Tooltip("Ganha em Vida ou Escudo $P0")]
    protected bool _health = true;
    [Space]
    [SerializeField, Tooltip("% de Roubo de vida $P1"), Range(0, 100)]
    protected int _porcentGetHealth = 30;

    protected override void Start()
    {        
        base.Start();
    }

    protected override void DescriptionPost()
    {
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)
            return;     

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P0", "<b>" + (_health ?
           "<color=green>" + XmlMenuInicial.Instance.Get(75)/*Vida*/+ "</color>" :
           "<color=blue>" + XmlMenuInicial.Instance.Get(76)/*Escudo*/+ "</color>") + "</b>");

        AttDescription("$P1", "<b>" + _porcentGetHealth + "%</b>");

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

            AttDescription("$P0", "<b>" + (_health ?
           "<color=green>" + XmlMenuInicial.Instance.Get(75)/*Vida*/+ "</color>" :
           "<color=blue>" + XmlMenuInicial.Instance.Get(76)/*Escudo*/+ "</color>") + "</b>");

            AttDescription("$P1", "<b>" + _porcentGetHealth + "%</b>");

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    public override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + (_health ?
            "<color=green>" + XmlMenuInicial.Instance.Get(75)/*Vida*/+ "</color>" :
            "<color=blue>" + XmlMenuInicial.Instance.Get(76)/*Escudo*/+ "</color>") + "</b>");

        AttDescription("$P1", "<b>" + _porcentGetHealth + "%</b>");
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

        foreach (var i in passive)
        {
            foreach (var y in _Passive)
            {
                if (y._startPassive == i)
                {
                    if (User != null && User.GetComponent<MobHealth>() && ChanceActivePassive())
                    {
                        //EffectManager.Instance.PopUpDamageEffect(_Nome + " Active", User);
                        RecHpDamage(User, value);
                    }
                }
            }
        }
    }

    void RecHpDamage(GameObject who, float damageValue)
    {
        CooldownReset();

        float damage = (damageValue * _porcentGetHealth) / 100;

        if (_health)
            User.GetComponent<MobHealth>().RecHp(User, damage);
        else
            User.GetComponent<MobHealth>().Defense(damage,User);
    }
}
