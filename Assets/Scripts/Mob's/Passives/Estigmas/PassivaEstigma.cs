using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassivaEstigma : PassiveManager
{
    [Space]
    [Header("Estigma")]
    [SerializeField]
    protected EffectStigma StigmaEffect;
    [Space]
    [SerializeField,Tooltip("Turnos para ativar estigma, $TFS")]
    protected int turnForStigma;
    [SerializeField, Tooltip("Duração do efeito do estigma, -1 para ilimitado, $DES")]
    protected int durationEffectStigma = -1;
    [SerializeField, Tooltip("Turno corrido ou quando é finalizado")]
    protected bool currentTurn = true;
    [Space]
    [SerializeField, Tooltip("Maximo de targets que podem ficar com o stigma, -1 para sem limite")]
    protected int maxTargetsStigma =-1;    

    protected virtual bool CurrentStigma()
    {
        if (maxTargetsStigma == -1)
            return true;

        return maxTargetsStigma > StigmaTargets.Count;
    }

    public virtual void CheckStigmaTargets()
    {
        if (StigmaTargets.Count != 0)
        {
            foreach (var t in StigmaTargets)
            {
                if (!t.gameObject.activeInHierarchy ||
                    t.GetComponent<MobManager>() && !t.GetComponent<MobManager>().Alive)
                {
                    StigmaTargets.Remove(t);
                }
            }
        }
    }

    [HideInInspector]
    public List<GameObject> StigmaTargets = new List<GameObject>();

    List<EffectStigma> stigmaEffectList   = new List<EffectStigma>();

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

        AttDescription("$TFS", "<b>" + turnForStigma + "</b>");

        AttDescription("$DES", "<b>" + durationEffectStigma + "</b>");

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

            AttDescription("$TFS", "<b>" + turnForStigma + "</b>");

            AttDescription("$DES", "<b>" + durationEffectStigma + "</b>");

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    public override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$TFS", "<b>" + turnForStigma + "</b>");

        AttDescription("$DES", "<b>" + durationEffectStigma + "</b>");
    }

    protected EffectStigma FindStigmaEffect()
    {
        EffectStigma fx = null;

        foreach (var f in stigmaEffectList)
        {
            if (!f.gameObject.activeInHierarchy)
            {
                fx = f;
            }
        }

        if (fx==null)
        {
            fx = Instantiate(StigmaEffect,new Vector3(-999,-999,-999),transform.rotation);

            fx.name = fx.name + " - " + (stigmaEffectList.Count + 2);

            stigmaEffectList.Add(fx);
        }

        Debug.LogError("FindStigma -> "+fx.name);

        return fx;
    }

    public override void ActivePassive(GameObject target, UnityEvent _event)
    {
        if(CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return;
        }

        if (!CurrentStigma())       
            return;

        ActivePassive(target);

        base.ActivePassive(target, _event);       
    }

    public virtual void ActivePassive(GameObject Target)
    {
        if (!CurrentStigma())
            return;

        CooldownReset();

        EffectStigma stigma = FindStigmaEffect();

        GameObject target = Target;

        stigma.gameObject.SetActive(true);

        if (_mobSkillManager != null)
            user = _mobSkillManager.User;
        else
        if (GetComponent<MobSkillManager>())
            user = GetComponent<MobSkillManager>().User;
        else
            if (GetComponent<SkillAttack>())
            user = GetComponent<SkillAttack>().who;    

        if (Target == null)
        {
            if (GetComponent<MobSkillManager>())
                target = GetComponent<MobSkillManager>().Target;
            else
                if (GetComponent<SkillAttack>())
                target = GetComponent<SkillAttack>().target;
        }

        stigma.StartStigma(this, User, target, turnForStigma, currentTurn,durationEffectStigma);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (TurnSystem.Instance)
        {
            TurnSystem.DelegateTurnCurrent += CheckStigmaTargets;
        }       
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (TurnSystem.Instance)
        {            
                TurnSystem.DelegateTurnCurrent -= CheckStigmaTargets;
        }
    }
}
