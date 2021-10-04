using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassivaReflexo : PassivaConjuracao 
{
    [Space]
    [Header("Reflexo")]
    [SerializeField, Tooltip("Dodge bonus $P6")]
    protected float _chanceDodge = 0;

    protected float _resetChanceDodge;
    [SerializeField, Tooltip("$P7")]
    protected Passive _resetPassive = Passive.GetDodge;

    protected MobHealth _mobHealth;



    public override string DescriptionToolType
    {
        get
        {
            string _r = "";


            if (User == null || !_AttDescriptonOnTooltip)
                return _r;

            _Description = XmlMobPassive.Instance.GetDescription(_XmlID);

            DbuffReDescription();

            AttDescription("$P6", "<b>" + _chanceDodge.ToString("F0") + "</b>% (" + _resetChanceDodge.ToString("F0") + "%)");

            AttDescription("$P7", "<b>" + XmlMenuInicial.Instance.PassiveTranslate(_resetPassive) + "</b>");

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
                        Dur += "</b> e <b>";
                }

                Dur += "</b>";

                if (Dur.Contains("_"))
                    AttDescription("_", " ");

                AttDescription("$P2", Dur);
            }

            AttDescription("$P4", "<b>" + _useForAtiveTimer + "</b>");

            AttDescription("$P5", "<b>" + XmlMenuInicial.Instance.PassiveTranslate(_activePassive) + "</b>");

            AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

            AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

            AttDescription("$P0", "<b>" + (_chance * 100) + "%</b>");

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

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    protected override void DescriptionPost()
    {
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)
            return;

        DbuffReDescription();

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P0", "<b>" + (_chance * 100) + "%</b>");
        AttDescription("$P3", _otherTargets ?
            XmlMenuInicial.Instance.Get(73)/*<color=blue>Chance Dbuff em Inimigos não Alvos</color>*/ :
            XmlMenuInicial.Instance.Get(74)/*<color=blue>Chance Dbuff Apenas no Alvo</color>*/);

        AttDescription("$P5", "<b>" + XmlMenuInicial.Instance.PassiveTranslate(_activePassive) + "</b>");

        AttDescription("$P6", "<b>" + _chanceDodge.ToString("F0") + "</b>% (" + _resetChanceDodge.ToString("F0") + "%)");

        AttDescription("$P7", "<b>" + XmlMenuInicial.Instance.PassiveTranslate(_resetPassive) + "</b>");

        AttDescription("$P4", "<b>" + _useForAtiveTimer + "</b>");

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
                    Dur += "</b> e <b>";
            }
        }

        _Description = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

        if (_mobSkillManager && !_AttDescriptonOnTooltip)
        {
            _mobSkillManager.Description += _Description;
            descriptionPost = true;
        }
    }

    public override void AttDescription()
    {
        if (XmlMobPassive.Instance != null)
        {
            PassiveXml p = XmlMobPassive.Instance.GetPassive(_XmlID);

            if (p._nameX != null)
                _Nome = p._nameX;

            if (p._description != null)
                _Description = p._description;
        }

        DbuffReDescription();

        AttDescription("$P0", "<b>" + (_chance * 100) + "%</b>");
        AttDescription("$P3", _otherTargets ?
            XmlMenuInicial.Instance.Get(73)/*<color=blue>Chance Dbuff em Inimigos não Alvos</color>*/ :
            XmlMenuInicial.Instance.Get(74)/*<color=blue>Chance Dbuff Apenas no Alvo</color>*/);

        AttDescription("$P4", "<b>" + _useForAtiveTimer + "</b>");

        AttDescription("$P5", "<b>" + XmlMenuInicial.Instance.PassiveTranslate(_activePassive) + "</b>");

        AttDescription("$P6", "<b>" + _chanceDodge.ToString("F0") + "</b>% (" + _resetChanceDodge.ToString("F0") + "%)");

        AttDescription("$P7", "<b>" + XmlMenuInicial.Instance.PassiveTranslate(_resetPassive) + "</b>");

        if (_Description.Contains("$P1"))
        {
            string Dur = XmlMenuInicial.Instance.Get(80);//Duração

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
                    Dur += "</b> e <b>";
            }

            Dur += "</b>";

            if (Dur.Contains("_"))
                AttDescription("_", " ");

            AttDescription("$P2", Dur);
        }

    }

    protected override void LoadEffects()
    {
        //if (_prefabInLoadingFx != null)
        //   _prefabInLoadingFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_Active_Fx", typeof(Effects));

        if (_prefabActiveFx == null)
            _prefabActiveFx = (Effects)Resources.Load("Passivas/Conjuraçao_Magica/ConjuracaoMagica_Active_Fx", typeof(Effects));

        if (_prefabCanActiveFx == null)
            _prefabCanActiveFx = (Effects)Resources.Load("Passivas/Conjuraçao_Magica/ConjuracaoMagica_CanUse_Fx", typeof(Effects));

        base.LoadEffects();

        if (_canActiveFx != null)
            _canActiveFx.ChangeTimeActive = 2.5f;
    }

    protected override void Start()
    {
        _mobHealth = _mobManager.GetComponent<MobHealth>();

        _chanceDodge = (100 - _mobManager.chanceCritical) / _useForAtive;

        base.Start();
    }

    public override void StartPassive(GameObject gO, params Passive[] passive)
    {
        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));

            ActiveDesativeEffect(false, false);
            return;
        }

        GameObject target = User.GetComponent<EnemyAttack>().target;

        foreach (var i in passive)
        {
            if (i == _resetPassive)
            {
                _mobHealth.dodge -= _resetChanceDodge;

                _resetChanceDodge = 0;

                EffectManager.Instance.PopUpDamageEffect(
                      "<color=white> Reset" +/*_chanceDodge.ToString("F0") + */"</color>"
                      , User, 2.5f);

                ActiveDesativeEffect(false, false);
            }

            if (i == _activePassive)
            {
                if (_mobHealth.dodge < 100)
                {
                    _mobHealth.dodge += _chanceDodge;

                    _resetChanceDodge += _chanceDodge;

                    EffectManager.Instance.PopUpDamageEffect(
                        "<color=white>+" + _chanceDodge.ToString("F0") + "%</color>"
                        , User, 2.5f);

                    if (_mobHealth.dodge >= 100)
                    {
                        ActiveDesativeEffect(true, false);
                    }
                    else
                        ActiveDesativeEffect(false, true);
                }
            }

            foreach (var y in _Passive)
            {
                if (y._startPassive == i)
                {
                    ActivePassive(gO, y._eventPassiveEffect);
                }
            }

            int count = _dbuffList.Count;
            for (int index = 0; index < count; index++)
            {

                if (_dbuffList[index]._startPassive == i)
                {
                    Debug.LogError("StartPassive - StartSkill (" + _dbuffList[index]._buff + ") [" + index + "]");


                    if (GameManagerScenes._gms.Dbuff
             (User,
              User.GetComponent<MobManager>().damage,
               target,
             _dbuffList[index]._buff,
             _dbuffList[index]._forMe,
             _dbuffList[index]._dbuffChance,
             _dbuffList[index]._dbuffDuracaoMin,
             _dbuffList[index]._dbuffDuracaoMax,
             _dbuffList[index]._acumule,
             _dbuffList[index]._acumuleMax,
             idAcumule: this.GetInstanceID()))
                    {
                        EffectManager.Instance.PopUpDamageEffect("" + XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[index]._buff, true), User, 2.5f);
                    }
                }               
            }
        }
    }
}
