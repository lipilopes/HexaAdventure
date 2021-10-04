using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassivaMuralha : PassiveManager
{
    [Space]
    [Header("Passiva Muralha")]
    [SerializeField, Tooltip("Absorve todo o dano $P0")]
    protected bool _totalDamage = true;
    [Space]
    [SerializeField, Tooltip("% do dano Absorvido $P1"), Range(0, 100)]
    protected int _porcentDamage = 45;
    [Space]
    [SerializeField, Tooltip("")]
    protected LayerMask _skillTypeBlocked;

    protected override void LoadEffects()
    {
        //if (_prefabInLoadingFx != null)
        //   _prefabInLoadingFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_Active_Fx", typeof(Effects));

        if (_prefabCanActiveFx == null)
            _prefabCanActiveFx = (Effects)Resources.Load("Passivas/Muralha/Muralha FX -Prefab", typeof(Effects));

        if (_prefabActiveFx == null)
            _prefabActiveFx = (Effects)Resources.Load("Passivas/Muralha/Muralha Ative FX -Prefab", typeof(Effects));

        base.LoadEffects();
    }

    protected override void DescriptionPost()
    {
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)
            return;

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P0", "<b>" + XmlMenuInicial.Instance.Get(71) + "</b>");//Recebe qualquer skill como se fosse o alvo.

        AttDescription("$P1", "<b>" + _porcentDamage + "%</b>");

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

            AttDescription("$P0", "<b>" + XmlMenuInicial.Instance.Get(71) + "</b>");//Recebe qualquer skill como se fosse o alvo.

            AttDescription("$P1", "<b>" + _porcentDamage + "%</b>");

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    public override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + XmlMenuInicial.Instance.Get(71) + "</b>");//Recebe qualquer skill como se fosse o alvo.

        AttDescription("$P1", "<b>" + _porcentDamage + "%</b>");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Effect(collision.gameObject.GetComponent<SkillAttack>());
    }

    void Effect(SkillAttack skill)
    {
        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return;
        }

        if (skill != null &&
            skill.gameObject.layer == _skillTypeBlocked && 
            ChanceActivePassive())
        {
            if (GetComponent<MobManager>().MesmoTime(skill.who))
                return;

            CooldownReset();

            ActiveDesativeEffect(true,false);

            Debug.LogError("StartPassive");

            if (_totalDamage)
            {
                if (skill.target != gameObject)
                {
                    skill.target = gameObject;

                    skill.who.GetComponent<SkillManager>().Skills[skill.skill].Target = gameObject;

                    skill.Desactive();

                    EffectManager.Instance.PopUpDamageEffect("<color=yellow>"+_Nome+"</color>", gameObject);
                }               
            }
            else
            {
                int damage = (skill.damage * _porcentDamage) / 100;

                EffectManager.Instance.PopUpDamageEffect
                    (
                    skill.who.GetComponent<SkillManager>().Skills[skill.skill].Nome+" -"+damage,
                    skill.gameObject
                    );             

                skill.damage -= damage;
            }
        }
    }
}
