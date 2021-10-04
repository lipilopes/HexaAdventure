using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DbuffBuffPassive
{
    [Tooltip("Como Passiva é Ativada $Start + ElementNumber")]
    public Passive _startPassive;

    [Tooltip("$Buff + ElementNumber /SilenceSkill: [-2 Todas, -3 Menor cooldown, -4 Todas as skills disponiveis]")]
    public Dbuff _buff;

    [Space]
    public bool _forMe = false;

    [Range(0, 1), Tooltip("$% + ElementNumber")]
    public float _dbuffChance = 0;

    [Space]
    [Tooltip("Valor minimo, em alguns casos esse pode ser o index ,$Min + ElementNumber ou $Dur + ElementNumber para aparecer o min - max")]
    public int _dbuffDuracaoMin = 0;
    [Tooltip("Valor maximo,em alguns casos esse pode ser o valor do Tempo,$Max + ElementNumber ou $Dur + ElementNumber para aparecer o min - max")]
    public int _dbuffDuracaoMax = 1;
    [Space]
    [Tooltip("Apenas Bonus e Escudo,Acumulativo $Ac + ElementNumber caso tenho maximo e isso seja verdadeiro ~print->(Acumula - x2)")]
    public bool _acumule = false;
    [Tooltip("Apenas Bonus e Escudo,Maximo que pode acumular $AcMax + ElementNumber")]
    public int _acumuleMax = 1;
}

public class PassivaDBuff : PassiveManager
{
    [Space, Header("Passiva DBuff")]
    [SerializeField]
    protected bool _tooltipDbuffDetal = false;
    [SerializeField,Tooltip("Detalhe adcional do $P2")]
    protected bool _tooltipStartPassiva = false;
    [SerializeField, Range(-1, 1), Tooltip("Altera a chance de todos os Dbuff List que estao com valor '0', -1 para nao alterar de todos $P0")]
    protected float _chance;
    [SerializeField, Tooltip("Altera a duração minima de todos os Dbuff List que estao com valor '-1', -1 para nao alterar de todos $P1")]
    protected int _duracaoMin;
    [SerializeField, Tooltip("Altera a duração maxima de todos osDbuff List que estao com valor '-1't, -1 para nao alterar de todos $P1")]
    protected int _duracaoMax;
    [SerializeField, Tooltip("Buff Pega nos Não alvos $P3")]
    protected bool _otherTargets;
    [Space]
    [SerializeField, Tooltip("Nome do buff $P2")]
    protected List<DbuffBuffPassive> _dbuffList = new List<DbuffBuffPassive>();

    protected override void Awake()
    {
        base.Awake();

        if (_duracaoMin == -99)
            _duracaoMin = BaseAtkPorcent0;
        if (_duracaoMin == -100)
            _duracaoMin = BaseAtkPorcent1;
//
        if (_duracaoMin == -999)
            _duracaoMin = BaseHpPorcent0;
        if (_duracaoMin == -1000)
            _duracaoMin = BaseHpPorcent1;
/****/
        if (_duracaoMax == -99)
            _duracaoMax = BaseAtkPorcent0;
        if (_duracaoMax == -100)
            _duracaoMax = BaseAtkPorcent1;
//
        if (_duracaoMax == -999)
            _duracaoMax = BaseHpPorcent0;
        if (_duracaoMax == -1000)
            _duracaoMax = BaseHpPorcent1;

        for (int i = 0; i < _dbuffList.Count; i++)
        {
            Debug.LogWarning("----ATT Dbuff["+i+"]("+name+")");

            #region Atk/Hp Porcent
            if (_dbuffList[i]._dbuffDuracaoMin == -99)
                _dbuffList[i]._dbuffDuracaoMin = BaseAtkPorcent0;
            if (_dbuffList[i]._dbuffDuracaoMin == -100)
                _dbuffList[i]._dbuffDuracaoMin = BaseAtkPorcent1;
        //
            if (_dbuffList[i]._dbuffDuracaoMin == -999)
                _dbuffList[i]._dbuffDuracaoMin = BaseHpPorcent0;
            if (_dbuffList[i]._dbuffDuracaoMin == -1000)
                _dbuffList[i]._dbuffDuracaoMin = BaseHpPorcent1;
//****//
            if (_dbuffList[i]._dbuffDuracaoMax == -99)
                _dbuffList[i]._dbuffDuracaoMax = BaseAtkPorcent0;
            if (_dbuffList[i]._dbuffDuracaoMax == -100)
                _dbuffList[i]._dbuffDuracaoMax = BaseAtkPorcent1;
            //
            if (_dbuffList[i]._dbuffDuracaoMax == -999)
                _dbuffList[i]._dbuffDuracaoMax = BaseHpPorcent0;
            if (_dbuffList[i]._dbuffDuracaoMax == -1000)
                _dbuffList[i]._dbuffDuracaoMax = BaseHpPorcent1;
            #endregion


            if (_chance != -1 && _dbuffList[i]._dbuffChance == 0)
            {
                _dbuffList[i]._dbuffChance = _chance;
                Debug.LogWarning("*Dbuff[" + i + "] :"+_chance);
            }

            if (_duracaoMin >= 0 && _dbuffList[i]._dbuffDuracaoMin == -1) { 
                _dbuffList[i]._dbuffDuracaoMin = _duracaoMin;
                Debug.LogWarning("*Dbuff[" + i + "] :" + _duracaoMin);
            }

            if (_duracaoMax >= 0 && _dbuffList[i]._dbuffDuracaoMax == -1) { 
                _dbuffList[i]._dbuffDuracaoMax = _duracaoMax;
            Debug.LogWarning("*Dbuff[" + i + "] :" + _duracaoMax);
        }

        Debug.LogWarning("----ATT Dbuff[" + i + "](" + name + ")");
        }
    }

    protected override void LoadEffects()
    {        //if (_prefabInLoadingFx != null)
        //   _prefabInLoadingFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_Active_Fx", typeof(Effects));

        if (_prefabActiveFx == null)
            _prefabActiveFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_Active_Fx", typeof(Effects));

        if (_prefabCanActiveFx == null)
            _prefabCanActiveFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_CanActive_Fx", typeof(Effects));

        base.LoadEffects();
    }

    protected virtual void DbuffReDescription()
    {
        if (_dbuffList.Count >= 1)
        {
            for (int i = 0; i < _dbuffList.Count; i++)
            {

                if (_Description.Contains("$Start" + i))
                    AttDescription("$Start" + i, XmlMenuInicial.Instance.PassiveTranslate(_dbuffList[i]._startPassive));

                if (_Description.Contains("$Buff" + i))
                    AttDescription("$Buff" + i, XmlMenuInicial.Instance.DbuffTranslate(_dbuffList[i]._buff));

                if (_Description.Contains("$Min" + i))
                {
                    if (_dbuffList[i]._buff == Dbuff.Silence ||
                        _dbuffList[i]._buff == Dbuff.SilencePassive)
                    {
                        switch (_dbuffList[i]._dbuffDuracaoMin)
                        {


                            case -2:
                                AttDescription("$Min" + i, "<b>Todas as " + (_dbuffList[i]._buff == Dbuff.Silence ? XmlMenuInicial.Instance.Get(214)/*Skills*/+ "(s)" : XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "(s)") + " </b>");
                                break;

                            case -3:
                                AttDescription("$Min" + i, "<b>" + (_dbuffList[i]._buff == Dbuff.Silence ? XmlMenuInicial.Instance.Get(214)/*Skills*/+ "(s)" : XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "(s)") + " com menor Recarga</b>");
                                break;

                            case -4:
                                AttDescription("$Min" + i, "<b>" + (_dbuffList[i]._buff == Dbuff.Silence ? XmlMenuInicial.Instance.Get(214)/*Skills*/+ "(s)" : XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "(s)") + " Disponiveis</b>");
                                break;
                        }

                    }
                    else
                    {
                        AttDescription("$Min" + i, "<b>" + _dbuffList[i]._dbuffDuracaoMin + "</b>");
                    }
                }


                string max = ("$Max" + i);
                if (_Description.Contains(max))
                {
                    if (_dbuffList[i]._dbuffDuracaoMax == -2)
                    {
                        AttDescription(max, "<b>" + XmlMenuInicial.Instance.Get(222) + "</b>");
                    }
                    else
                        AttDescription(max, "<b>" + _dbuffList[i]._dbuffDuracaoMax + "</b>");
                }
                string dur = ("$Dur" + i);
                if (_Description.Contains(dur))
                {
                    string Dur =
                        (_dbuffList[i]._dbuffDuracaoMin == _dbuffList[i]._dbuffDuracaoMax)
                       ? "<b>" + _dbuffList[i]._dbuffDuracaoMax + "</b>"
                       : "<b>" + _dbuffList[i]._dbuffDuracaoMin + " - " + _dbuffList[i]._dbuffDuracaoMax + "</b>";

                    AttDescription(dur, Dur);
                }

                string acumule = ("$Ac" + i);
                if (_Description.Contains(acumule))
                    AttDescription(acumule, _dbuffList[i]._acumule
                        ? "(<color=blue><b>" + XmlMenuInicial.Instance.Get(193) + "</b></color>" +//Acumula
                            (_dbuffList[i]._acumuleMax > 1 ? " - x" + _dbuffList[i]._acumuleMax + ")" : "")
                        : ""//"<color=red><b>"  + XmlMenuInicial.Instance.Get(194) + "</b></color>"//Não acumula
                        );
            }
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

            AttDescription("$P0", "<b>" + (_chance * 100) + "%</b>");
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
                        Dur += "</b> e <b>";
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

        DbuffReDescription();

        AttDescription("$P0", "<b>" + (_chance * 100) + "%</b>");
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
                if (i != 0 && _tooltipStartPassiva && _dbuffList[i]._startPassive != _dbuffList[1 - i]._startPassive)
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
                else
                    Dur += "</b> e <b>";
            }

            Dur += "</b>";

            if (Dur.Contains("_"))
                AttDescription("_", " ");

            AttDescription("$P2", Dur);
        }
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


        bool Active = ChanceActivePassive();

        if (!Active)
        {
            ActiveDesativeEffect(false, false);
            return;
        }

        GameObject target = User.GetComponent<EnemyAttack>().target;

        foreach (var i in passive)
        {         
            foreach (var y in _Passive)
            {
                if (y._startPassive == i)
                {
                    ActivePassive(gO, y._eventPassiveEffect);
                }
            }
        }

        foreach (var d in _dbuffList)
        {
            foreach (var p in passive)
            {
                if (d._startPassive == p)
                {
                    if (GameManagerScenes._gms.Dbuff
             (User,
              User.GetComponent<MobManager>().damage,
               target,
              d._buff,
             d._forMe,
             d._dbuffChance,
             d._dbuffDuracaoMin,
             d._dbuffDuracaoMax,
             d._acumule,
             d._acumuleMax,            
             idAcumule: this.GetInstanceID()))
                    {
                        EffectManager.Instance.PopUpDamageEffect("" + XmlMenuInicial.Instance.DbuffTranslate(d._buff, true), User, 2.5f);

                        CooldownReset();
                    }
                }
            }
        }
    }

    public void DbuffList(int index)
    {
        ActiveDbuffList(index);
    }

    protected virtual void ActiveDbuffList(int index,bool cooldownReset=true)
    {
        GameObject target = User.GetComponent<EnemyAttack>().target;

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

            if (cooldownReset)
            CooldownReset();
        }
    }
}
