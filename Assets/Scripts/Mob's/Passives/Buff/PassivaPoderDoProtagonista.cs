using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassivaPoderDoProtagonista : PassiveManager
{
    [Space]
    [Header("Poder Do Protagonista")]
    //[SerializeField,Range(0,100), Tooltip("Recupera de Hp $P0")]
   // protected float _recHp        = 15;
    [SerializeField, Range(1, 100), Tooltip("Em quantos turnos rec Hp $P1")]
    protected int _countTurn    = 3;
    //[SerializeField, Range(0, 100), Tooltip("Necessario de hp para ativar $P2")]
   // protected float _needForAtive = 15;
   // [SerializeField, Tooltip("tempo de espera para ativar passiva -1 para ativação unica $P3")]
    //protected int  cooldownMax     = 15;
    [SerializeField, Tooltip("Ganha quando turno for finalizado")]
    protected bool _endTurn = true;
    //[Space]
    //[Header("FX")]
    //[SerializeField,Tooltip("Fx Quando efeito esta Ativo")]
    //protected Effects _prefabPoderDoProtagonistaFx;
    //[SerializeField, Tooltip("Fx Quando efeito pode ser Ativo")]
    //protected Effects _prefabPoderDoProtagonistaCanBeAtiveFx;

    MobHealth userHealth;

    [SerializeField,Tooltip("[Precisa Alterar -> _baseHpPorcent0]Hp que Recupera Por Turno  $P0"),Range(0,0)]
    int HpRec         = 0;
    [SerializeField, Tooltip("[Precisa Alterar -> _baseHpPorcent1]Hp Necessario Para Ativar $P1"), Range(0, 0)]
    int HpForAtive    = 0;
    //int cooldownCurrent = 0;
    int Counter       = 0;
    bool  Active        = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (TurnSystem.Instance)
        {
            if (_endTurn)
                TurnSystem.DelegateTurnEnd     += Effect;
            else
                TurnSystem.DelegateTurnCurrent += Effect;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (TurnSystem.Instance)
        {
            if (_endTurn)
                TurnSystem.DelegateTurnEnd     -= Effect;
            else
                TurnSystem.DelegateTurnCurrent -= Effect;
        }
    }

    protected override void Start()
    {
        //if (_AttDescriptonOnEnable)
        //    _AttDescriptonOnEnable = false;

        Invoke("GetUserStatus", 5);
    }

    protected override void DescriptionPost()
    {
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)
            return;

        if (_mobManager!=null)
        {
            userHealth = _mobManager.GetComponent<MobHealth>();

            HpRec = (int)((userHealth.MaxHealth * _baseHpPorcent0) / 100) / _countTurn;

            HpForAtive = (int)(userHealth.MaxHealth * _baseHpPorcent1) / 100;
        }

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P0", "<b>" + ("<color=blue>" + HpRec + "</color>") + "</b>");

        AttDescription("$P1", "<b>" + ("<color=blue>" + _countTurn + "</color>") + "</b>");

        AttDescription("$P2", "<b>" + ("<color=blue>" + HpForAtive + "</color>") + "</b>");

        //AttDescription("$P3", XmlMenuInicial.Instance.Get(68)/*cooldown*/+ "\n<b>" +
        //    (cooldownMax == -1 ? XmlMenuInicial.Instance.Get(72)/*<color=yellow>Ativação Única</color>*/
        //   : ("<color=blue>" + cooldownMax + "</color>")) + "</b>");

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

            if (_mobManager)
            {
                userHealth = _mobManager.GetComponent<MobHealth>();

                HpRec = (int)((userHealth.MaxHealth * _baseHpPorcent0) / 100) / _countTurn;

                HpForAtive = (int)(userHealth.MaxHealth * _baseHpPorcent1) / 100;
            }

            AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

            AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

            AttDescription("$P0", "<b>" + ("<color=blue>" + HpRec + "</color>") + "</b>");

            AttDescription("$P1", "<b>" + ("<color=blue>" + _countTurn + "</color>") + "</b>");

            AttDescription("$P2", "<b>" + ("<color=blue>" + HpForAtive + "</color>") + "</b>");

            //AttDescription("$P3", XmlMenuInicial.Instance.Get(68)/*cooldown*/+ "\n<b>" +
            //    (cooldownMax == -1 ? XmlMenuInicial.Instance.Get(72)/*<color=yellow>Ativação Única</color>*/
            //   : ("<color=blue>" + cooldownMax + "</color>")) + "</b>");

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    public override void AttDescription()
    {
        base.AttDescription();

        if (_mobManager)
        {
            userHealth = _mobManager.GetComponent<MobHealth>();

            HpRec      = (int)((userHealth.MaxHealth * _baseHpPorcent0) / 100) / _countTurn;

            HpForAtive = (int)(userHealth.MaxHealth * _baseHpPorcent1) / 100;
        }
        else
        {
            this.enabled = false;
            return;
        }

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        AttDescription("$P0", "<b>" + ("<color=blue>" + HpRec      + "</color>") + "</b>");

        AttDescription("$P1", "<b>" + ("<color=blue>" + _countTurn + "</color>") + "</b>");

        AttDescription("$P2", "<b>" + ("<color=blue>" + HpForAtive + "</color>") + "</b>");

        AttDescription("$P3", XmlMenuInicial.Instance.Get(68)/*cooldown*/+"\n<b>" + 
            (cooldownMax ==-1 ? XmlMenuInicial.Instance.Get(72)/*<color=yellow>Ativação Única</color>*/ 
           :("<color=blue>" + cooldownMax  + "</color>")) + "</b>");
    }

    protected override void LoadEffects()
    {
        //if (_prefabInLoadingFx != null)
        //   _prefabInLoadingFx = (Effects)Resources.Load("Passivas/Vantagem_Runica/VantagemRunica_Active_Fx", typeof(Effects));

        if (_prefabActiveFx == null)
            _prefabActiveFx = (Effects)Resources.Load("Passivas/Poder_Do_Protagonista/Poder Do Protagonista FX -Prefab", typeof(Effects));
        
        if (_prefabCanActiveFx == null)
            _prefabCanActiveFx = (Effects)Resources.Load("Passivas/Poder_Do_Protagonista/Poder Do Protagonista Can Be Active FX -Prefab", typeof(Effects));

        base.LoadEffects();
    }

    protected virtual bool GetUserStatus()
    {        
        print("GetUserStatus");

        GetUser();

        if (User == null)
            return false;

        //if (_AttDescriptonOnEnable)
        //    _AttDescriptonOnEnable = false;        
     
        if (_mobManager)
        {
            userHealth = _mobManager.GetComponent<MobHealth>();

            HpRec      = (int)((userHealth.MaxHealth * _baseHpPorcent0) / 100) / _countTurn;

            HpForAtive = (int)(userHealth.MaxHealth * _baseHpPorcent1) / 100;         
        }
        else
        {
            this.enabled = false;
            return true;
        }

        //AttDescription("$P0", "<b>" + ("<color=blue>" + HpRec      + "</color>") + "</b>");

        //AttDescription("$P1", "<b>" + ("<color=blue>" + _countTurn + "</color>") + "</b>");

        //AttDescription("$P2", "<b>" + ("<color=blue>" + HpForAtive + "</color>") + "</b>");

        //AttDescription("$P3", XmlMenuInicial.Instance.Get(68)/*cooldown*/+"\n<b>" + 
        //    (cooldownMax ==-1 ? XmlMenuInicial.Instance.Get(72)/*<color=yellow>Ativação Única</color>*/ 
        //   :("<color=blue>" + cooldownMax  + "</color>")) + "</b>");

        //LoadEffects();      

        base.Start();

        return true;
    }

    public override void StartPassive(GameObject target, float value, params Passive[] passive)
    {
        Debug.LogError("StartPassive");

        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome+" Esta Silenciada por "+SilenceTime+" turnos"
                : (cooldownCurrent >0 ? "Em Espera" :"Passiva já foi usada"));
            return;
        }

        if (_Passive.Length == 0)
        {
            foreach (var i in passive)
            {
                if (i == Passive.DodgeFail && ChanceActivePassive())
                {
                    PassiveAtive();
                }
                //if (i == Passive.GetDamage && ChanceActivePassive())
                //{
                //    PassiveAtive();
                //}

                //if (i == Passive.GetRealDamage && ChanceActivePassive())
                //{
                //    PassiveAtive();
                //}
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

    void PassiveAtive()
    {
        if (cooldownCurrent > 0 || Active)
            return;

        if (User       == null ||
            userHealth == null)
            return;

        CooldownReset();

        if (_activeFx == null || _canActiveFx == null)
            LoadEffects();

        if (userHealth.Health <= HpForAtive)
        {
            Active = true;

            Counter = _countTurn;
        }

        ActiveDesativeEffect(cooldownCurrent < 0 && !Active /*&& userHealth.Health <= HpForAtive*/);
    }

    void Effect()
    {
        if (!enabled)
        {
            return;
        }

        if (_activeFx == null || _canActiveFx == null)
            LoadEffects();      

        if (!Active)
        {
            if(cooldownCurrent > 0)
            cooldownCurrent--;

            ActiveDesativeEffect(cooldownCurrent <= 0 /*&& userHealth.Health <= HpForAtive*/);
            return;
        }

        if (Counter > 0)
        {
            Counter--;

            if (userHealth != null)
            {
                userHealth.RecHp(User, HpRec);

                ActiveDesativeEffect(true,false);
            }

            /*if (CooldownMax==-1)
                cooldownCurrent = -2;*/
        }
        else
        {
            ActiveDesativeEffect(false,false);

            if (cooldownCurrent == -2)
            {
                return;
            }

            Active = false;

            //cooldownCurrent = cooldownMax;

            Counter = 0;
        }

        if (userHealth != null)
            ActiveDesativeEffect(cooldownCurrent < 0 && !Active && userHealth.Health <= HpForAtive);
    }
}
