using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassivaVantagemRunica : PassivaDBuff
{
    [Space]
    [Header("Vantagem Runica")]

    /// <summary>
    /// Dbuff List
    /// </summary>
    protected int        index;
    protected GameObject _arma;

    protected bool _passiveActive = false; 

    protected override void Start()
    {          
        base.Start();

        if (_mobSkillManager!=null && _mobSkillManager.SkillAttackFx!=null)
        _arma = _mobSkillManager.SkillAttackFx;
    }

    protected override void LoadEffects()
    {
        //if (_prefabInLoadingFx != null)
        //   _prefabInLoadingFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_Active_Fx", typeof(Effects));

        if (_prefabActiveFx == null)
            _prefabActiveFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_Active_Fx", typeof(Effects));

        if (_prefabCanActiveFx == null)
            _prefabCanActiveFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_CanActive_Fx", typeof(Effects));

        base.LoadEffects();       
    }

    protected override void DescriptionPost()
    {
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)
            return;

        DbuffReDescription();

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P0", "<b>" + (_chance*100) + "%</b>");
        AttDescription("$P3", _otherTargets ? 
            XmlMenuInicial.Instance.Get(73)/*<color=blue>Chance Dbuff em Inimigos não Alvos</color>*/ :
            XmlMenuInicial.Instance.Get(74)/*<color=blue>Chance Dbuff Apenas no Alvo</color>*/);

        if (_Description.Contains("$P1"))
        {
            string Dur = XmlMenuInicial.Instance.Get(80)/*Duração*/;

            if (_duracaoMin == _duracaoMax)
                Dur += ": <b>" + _duracaoMax + "</b>";
            else
                Dur += ": <b>" + _duracaoMin + " - " + _duracaoMax + "</b>";

            AttDescription("$P1", Dur);
        }

        if (_Description.Contains("$P2"))
        {
            string Dur = "<b>";

            for (int i = 0; i < _dbuffList.Count; i++)
            {
                if (i >= 1 && _dbuffList[i]._startPassive != _dbuffList[1 - i]._startPassive && _tooltipStartPassiva)
                {
                    Dur += XmlMenuInicial.Instance.PassiveTranslate(_dbuffList[i]._startPassive) + ": ";
                }

                if (_tooltipDbuffDetal)
                    Dur += XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[i]._buff, _dbuffList[i]._dbuffChance, _dbuffList[i]._dbuffDuracaoMin, _dbuffList[i]._dbuffDuracaoMax, _dbuffList[i]._acumuleMax);
                else
                    Dur += XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[i]._buff);

                if (i != _dbuffList.Count - 1)
                {
                    Dur += ", ";
                }
                
                    if (i == _dbuffList.Count - 2)
                    Dur += "</b> ou <b>";
            }

            Dur += "</b>";

            if (Dur.Contains("_"))
                AttDescription("_", " ");

            AttDescription("$P2", Dur);
        }

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

            DbuffReDescription();

            AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

            AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

             AttDescription("$P0", "<b>" + (_chance*100) + "%</b>");
        AttDescription("$P3", _otherTargets ? 
            XmlMenuInicial.Instance.Get(73)/*<color=blue>Chance Dbuff em Inimigos não Alvos</color>*/ :
            XmlMenuInicial.Instance.Get(74)/*<color=blue>Chance Dbuff Apenas no Alvo</color>*/);

        if (_Description.Contains("$P1"))
        {
            string Dur = XmlMenuInicial.Instance.Get(80) + ": <b>" + _duracaoMin + " - " + _duracaoMax + "</b>";//Duração

            if (_duracaoMin == _duracaoMax)
                Dur = "<b>" + _duracaoMax + "</b>";

            AttDescription("$P1", Dur);
        }

        if (_Description.Contains("$P2"))
        {
            string Dur = "<b>";

            for (int i = 0; i < _dbuffList.Count; i++)
            {
                    if (i >= 1 && _dbuffList[i]._startPassive != _dbuffList[1 - i]._startPassive && _tooltipStartPassiva)
                    {
                        Dur += XmlMenuInicial.Instance.PassiveTranslate(_dbuffList[i]._startPassive) + ": ";
                    }

                    if (_tooltipDbuffDetal)
                        Dur += XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[i]._buff, _dbuffList[i]._dbuffChance, _dbuffList[i]._dbuffDuracaoMin, _dbuffList[i]._dbuffDuracaoMax, _dbuffList[i]._acumuleMax);
                    else
                        Dur += XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[i]._buff);

                    if (i != _dbuffList.Count - 1)
                {
                    Dur += ", ";
                }
                
                    if (i == _dbuffList.Count - 2)
                    Dur += "</b> ou <b>";
            }

            Dur += "</b>";

            if (Dur.Contains("_"))
                AttDescription("_", " ");

            AttDescription("$P2", Dur);
        }

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    public override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + (_chance*100) + "%</b>");
        AttDescription("$P3", _otherTargets ? 
            XmlMenuInicial.Instance.Get(73)/*<color=blue>Chance Dbuff em Inimigos não Alvos</color>*/ :
            XmlMenuInicial.Instance.Get(74)/*<color=blue>Chance Dbuff Apenas no Alvo</color>*/);

        DbuffReDescription();

        if (_Description.Contains("$P1"))
        {
            string Dur = XmlMenuInicial.Instance.Get(80) + ": <b>" + _duracaoMin + " - " + _duracaoMax + "</b>";//Duração

            if (_duracaoMin == _duracaoMax)
                Dur = "<b>" + _duracaoMax + "</b>";

            AttDescription("$P1", Dur);
        }

        if (_Description.Contains("$P2"))
        {
            string Dur = "<b>";

            for (int i = 0; i < _dbuffList.Count; i++)
            {
                if (i >= 1 && _dbuffList[i]._startPassive != _dbuffList[1 - i]._startPassive && _tooltipStartPassiva)
                {
                    Dur += XmlMenuInicial.Instance.PassiveTranslate(_dbuffList[i]._startPassive) + ": ";
                }

                if (_tooltipDbuffDetal)
                    Dur += XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[i]._buff, _dbuffList[i]._dbuffChance, _dbuffList[i]._dbuffDuracaoMin, _dbuffList[i]._dbuffDuracaoMax, _dbuffList[i]._acumuleMax);
                else
                    Dur += XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[i]._buff);

                if (i != _dbuffList.Count - 1)
                {
                    Dur += ", ";
                }
                
                    if (i == _dbuffList.Count - 2)
                    Dur += "</b> ou <b>";
            }

            Dur += "</b>";

            if (Dur.Contains("_"))
                AttDescription("_", " ");

            AttDescription("$P2", Dur);
        }
    }

    public override void StartPassive(GameObject gO, params Passive[] passive)
    {
        //Debug.LogError("StartPassive");

        foreach (var i in passive)
            if (i == Passive.HitSkill && _passiveActive)
            {
                Debug.LogError("StartPassive - HitSkill");
                DamageDbuff(gO);
            }

            if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return;
        }

        foreach (var i in passive)
        {
            if (i == Passive.StartSkill && ChanceActivePassive())
            {
                Debug.LogError("StartPassive - StartSkill");

                index = Random.Range(0, _dbuffList.Count);

                if (CheckChance(_dbuffList[index]._dbuffChance*100))
                {
                    ActiveDesativeEffect(true, false);

                    EffectManager.Instance.PopUpDamageEffect("" + XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[index]._buff, true), User, 2.5f);                  

                    _passiveActive = true;

                    MobSkillManager g = null;

                    if (User.GetComponent<SkillManager>())
                    {
                        g = User.GetComponent<SkillManager>().SkillInUseWho();

                        _mobSkillManager = g;
                    }

                    Debug.LogError("Vantagem Runica: arma " + g +" Dbuff: "+ _dbuffList[index]._buff);

                    if (g != null)
                        _arma = g.SkillAttack.Fx;
                   
                    ActiveDesativeEffect(false, true);
                }                                               
            }

            if (i == Passive.ShootSkill && _passiveActive)
            {
                Debug.LogError("StartPassive - ShootSkill");
                ActiveOrdemDbuffEffect(-1, _arma, true);
            }

            foreach (var y in _Passive)
            {             
                if (y._startPassive == i && ChanceActivePassive())
                {
                    ActivePassive(gO, y._eventPassiveEffect);
                }
            }
        }
    }

    public virtual void DamageDbuff(GameObject _target)
    {
        if (_mobSkillManager != null)
            if (_target != null && _otherTargets ||
                _target == _mobSkillManager.Target && !_otherTargets)
            {
                _mobSkillManager.CreateDbuff
                 (_target,
                 _dbuffList[index]._buff,
                 _dbuffList[index]._forMe,
                 1,
                 _dbuffList[index]._dbuffDuracaoMin,
                 _dbuffList[index]._dbuffDuracaoMax);

                if (_target == _mobSkillManager.Target)
                {
                    //ActiveDesativeEffect(false, false); tem timer

                    CooldownReset();

                    _passiveActive = false;

                    ActiveOrdemDbuffEffect(index, _arma, false);
                }
            }     
    }

    protected virtual void ActiveOrdemDbuffEffect(int _index, GameObject _target, bool active)
    {      
        if (GameManagerScenes._gms.Adm)
            InfoTable.Instance.NewInfo(name+" ActiveOrdemDbuffEffect("+index+","+active+")", 15);

        if (_index==-1)
        _index = index;

        if (_index < _dbuffList.Count)
        {           
            index = _index;

            if (_dbuffList[_index]._dbuffChance <= 0)
                return;

            if (active)
            {              
                Debug.LogError("StartPassive(" + _Nome + ") - "+ _dbuffList[_index]._buff);
               
                // EffectManager.Instance.PopUpDamageEffect(""+XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[_index]._buff,true), User, 2.5f);
            }

            switch (_dbuffList[_index]._buff)
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
                    break;

                case Dbuff.Chamar:
                    break;
                case Dbuff.Cooldown:
                    break;
                case Dbuff.Recupera_HP:
                    break;
                case Dbuff.Escudo:
                    break;
            }
        }
        else
            index = -2;
    }

}
