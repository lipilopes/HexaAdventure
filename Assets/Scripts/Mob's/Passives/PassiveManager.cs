
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Passive
{
    /// <summary>
    /// Assim que Lança a skill
    /// </summary>
    [Tooltip("Assim que Lança a skill")]
    ShootSkill,
    /// <summary>
    /// Assim que a skill acaba
    /// </summary>
    [Tooltip("Assim que a skill acaba")]
    EndSkill,
    /// <summary>
    /// Assim que chama o UseSkill
    /// </summary>
    [Tooltip("Assim que chama o UseSkill")]
    StartSkill,
    /// <summary>
    /// Assim que skill da hit
    /// </summary>
    [Tooltip("Assim que skill da hit")]
    HitSkill,

    /// <summary>
    /// Quando Recupera vida
    /// </summary>
    [Tooltip("Quando Recupera vida")]
    RestoreHp,
    /// <summary>
    /// Quando Ganha Escudo
    /// </summary>
    [Tooltip("Quando Ganha Escudo")]
    GetDefense,
    /// <summary>
    /// Assim que leva dano
    /// </summary>
    [Tooltip("Assim que leva dano")]
    GetDamage,
    /// <summary>
    /// Assim que leva dano critico
    [Tooltip("Assim que leva dano critico")]
    GetCriticalDamage,
    /// <summary>
    /// Quando Defende o dano
    /// </summary>
    [Tooltip("Quando Defende o dano")]
    DefenseDamage,

    /// <summary>
    /// Assim Que mata um inimigo
    /// </summary>
    [Tooltip("Assim Que mata um inimigo")]
    KillEnemy,
    /// <summary>
    /// Dano que deu no Inimigo
    /// </summary>
    [Tooltip("Dano que deu no Inimigo")]
    SetDamageInEnemy,
    /// <summary>
    /// Quando Dono do script morre
    /// </summary>
    [Tooltip("Dono do script morre")]
    Kill,
    /// <summary>
    /// Quando Dono Acaba o turno
    /// </summary>
    [Tooltip("Quando Dono Acaba o turno")]
    EndTurn,

    /// <summary>
    /// Quando Inimigo acerta skill em você
    /// </summary>
    [Tooltip("Quando Inimigo acerta skill em você")]
    EnimyHitSkill,

    /// <summary>
    /// Quando Area Criada Ataca
    /// </summary>
    [Tooltip("Quando Area Criada Ataca")]
    AreaAttack,

    /// <summary>
    /// Assim que skill da hit no alvo principal
    /// </summary>
    [Tooltip("Assim que skill da hit no alvo principal")]
    TargetHitSkill,

    /// <summary>
    /// Assim que leva dano Real
    /// </summary>
    [Tooltip("Assim que leva dano Real")]
    GetRealDamage,

    /// <summary>
    /// Assim que Andar
    /// </summary>
    [Tooltip("Assim que andar")]
    Walk,

    /// <summary>
    /// Assim que Andar por um Dbuff
    /// </summary>
    [Tooltip("Assim que andar por um Dbuff")]
    WalkDbuff,

    /// <summary>
    /// Antes de Levar Dano
    /// </summary>
    [Tooltip("Antes de Levar Dano")]
    BeforeGetDamage,

    /// <summary>
    /// Quando Ganha Armadura
    /// </summary>
    [Tooltip("Quando Ganha Armadura")]
    GetArmor,

    /// <summary>
    /// Quando Mob é Criado
    /// </summary>
    [Tooltip("Quando Mob é Criado")]
    Criation,

    /// <summary>
    /// Acertou Dano Critico
    /// </summary>
    [Tooltip("Acertou Dano Critico")]
    SetCriticalDamage,

    /// <summary>
    /// Desviou do atk
    /// </summary>
    [Tooltip("Desviou do atk")]
    GetDodge,

    /// <summary>
    /// Inimigo Desviou
    /// </summary>
    [Tooltip("Inimigo Desviou")]
    EnemyDodge,

    /// <summary>
    ///  Desvio Falhou
    /// </summary>
    [Tooltip("Desvio Falhou")]
    DodgeFail,

    /// <summary>
    /// levou dano do inimigo
    /// </summary>
    [Tooltip("levou dano do inimigo")]
    EnemyHit,

    /// <summary>
    /// Nada
    /// </summary>
    [Tooltip("Nada")]
    Empty
}

[System.Serializable]
public class PassiveList
{
    [Tooltip("Como Passiva é Ativada")]
    public Passive _startPassive;

    [Tooltip("O que a Passiva Faz")]
    public UnityEvent _eventPassiveEffect;
}

//[RequireComponent(typeof(MobSkillManager))]
public class PassiveManager : MonoBehaviour
{
    protected MobSkillManager _mobSkillManager;

    [SerializeField]
    protected int _XmlID = -1;    
    [Space,SerializeField]
    protected Game_Mode _balanceMode = Game_Mode.History;
    [SerializeField,Tooltip("In Mode Battle Can Desactive This??")]
    private bool _truePassive = true;
    [HideInInspector]
    public bool CheckCompativelMode = false;
    [Space]
    [SerializeField, Tooltip("Passiva Silenciada, não pode ser usada")]
    protected bool _silence = false;
    public bool SilencePassive { get { return _silence; } }
    //Tempo de duração
    protected int _silenceTime = 0;
    public int SilenceTime { get { return _silenceTime; } }

    /// <summary>
    /// Silencia a skill
    /// </summary>
    /// <param name="_time">tempo</param>
    public void Silence(int _time)
    {
        _silence = true;

        _silenceTime = _time;

        ActiveDesativeEffect(_time<=0);

        Debug.LogError("AtiveDbuffSilencePassive(time:" + _time + ") " + _Nome + " esta silenciada");
    }

    /// <summary>
    /// Cooldown do Silence
    /// </summary>
    /// <param name="_reset">Desativar caso esta ativo</param>
    public void SilenceCooldown(bool _reset = false)
    {
        _silenceTime--;

        if (_silenceTime == 0 || _reset)
        {
            _silence = false;

            _silenceTime = 0;

            ActiveDesativeEffect(true);
        }
    }

    [Space]
    [SerializeField]
    protected MobManager _mobManager;
    [SerializeField, Range(0, 100), Tooltip("Para alterar Valores Coloque como -99")]
    protected float _baseAtkPorcent0;
    /// <summary>
    /// -99
    /// </summary>
    protected int BaseAtkPorcent0 { get { return _mobManager!=null ? (int)(_mobManager.damage * _baseAtkPorcent0) / 100 : 0 ; } }

    [SerializeField, Range(0, 100), Tooltip("Para alterar Valores Coloque como -100")]
    protected float _baseAtkPorcent1;
    /// <summary>
    /// -100
    /// </summary>
    protected int BaseAtkPorcent1 { get { return _mobManager != null ? (int)(_mobManager.damage * _baseAtkPorcent1) / 100 : 0; } }

    [Space(2.5f)]
    [SerializeField, Range(0, 100), Tooltip("Para alterar Valores Coloque como -999")]
    protected float _baseHpPorcent0;
    /// <summary>
    /// -999
    /// </summary>
    protected int BaseHpPorcent0 { get { return _mobManager != null ? (int)(_mobManager.health * _baseHpPorcent0) / 100 : 0; } }

    [SerializeField, Range(0, 100), Tooltip("Para alterar Valores Coloque como -1000")]
    protected float _baseHpPorcent1;
    /// <summary>
    /// -1000
    /// </summary>
    protected int BaseHpPorcent1 { get { return _mobManager != null ? (int)(_mobManager.health * _baseHpPorcent1) / 100 : 0; } }


    [Space]
    [Header("Main inf's")]
    [SerializeField, Tooltip("Nome da Passiva")]
    protected string _Nome       = "";
    public string Nome { get { return _Nome; } }
    [SerializeField, Tooltip("Descrição vai para o tooltip,/ else descrição da skill")]
    protected bool _AttDescriptonOnTooltip = true;
    [SerializeField, Tooltip("descrição da Passiva"),TextArea()]
    protected string _Description = "";
    [Space]    
    [SerializeField, Range(0.1f, 100), Tooltip("Chance de ativar Passiva $%")]
    protected float _chanceActivePassive=100;
    [Space]
    [SerializeField, Tooltip("Como Passiva é Ativada")]
    protected PassiveList[] _Passive;
    [Space]
    [SerializeField,Tooltip("tempo de espera para ativar passiva -1 para ativação unica $P3")]
    protected int cooldownMax;
    public int CooldownMax { get { return cooldownMax; } set { cooldownMax = value; } }
    /// <summary>
    /// -2 ativação unica ja efetuada.
    /// </summary>
    protected int cooldownCurrent=0;
    /// <summary>
    /// 
    /// </summary>
    public int CooldownCurrent { get { return cooldownCurrent; } set { cooldownCurrent = value; ActiveDesativeEffect(value == 0); } }
    protected virtual void CooldownReset()
    {
        if (CooldownMax == -1)
            CooldownCurrent = -2;
        else
            CooldownCurrent = CooldownMax;
    }

    /// <summary>
    /// --Cooldown
    /// </summary>
    public virtual void Cooldown()
    {
        if (CooldownCurrent >= 1 && CooldownMax != -1)
        {
            CooldownCurrent--;

            Debug.LogError(Nome + " Cooldown[" + CooldownCurrent + "]");
        }
    }
    [Space]
    [Header("FX")]
    [SerializeField, Tooltip("Fx Quando efeito esta Carregando")]
    protected Effects _prefabInLoadingFx;//Carregando
    [SerializeField, Tooltip("Fx Quando efeito pode ser Ativo")]
    protected Effects _prefabCanActiveFx;//Pode Ativar
    [SerializeField, Tooltip("Fx Quando efeito esta Ativo")]
    protected Effects _prefabActiveFx;//Passiva Ativa
    /// <summary>
    /// Efeito de quando Passiva ainda Esta Em CC
    /// </summary>
    protected Effects _inloadingFx;//Pode ativar
    /// <summary>
    /// Efeito de quando Passiva Pode Ser Ativada
    /// </summary>
    protected Effects _canActiveFx;//Pode ativar
    /// <summary>
    /// Efeito da Passiva Ativa
    /// </summary>
    protected Effects _activeFx;//Ativa


    protected GameObject user;

    public    GameObject User { get { return user; } set { user = value; Debug.LogError("User("+value+") do "+ _Nome); } }

    protected virtual void Awake()
    {
        if (GameManagerScenes.BattleMode)
        {
            if (!GameManagerScenes._gms.BattleModeOptionPassiveMobActive && _truePassive)
            {
                Debug.LogWarning(name + " Opção Battle Mode Passive Active Ativada");
                Destroy(this);
                return;
            }
        }    

        if (!enabled)
            return;

        if (!CheckCompativelMode)
        {
            int count = 0;

            foreach (var item in GetComponents<PassiveManager>())
                if (item.name == name)
                    count++;

            if (GameManagerScenes._gms.GameMode != _balanceMode
                && count >= 2)
            {
                enabled = false;
                Debug.LogWarning(name + " Incompativel com o modo!!!");
                Destroy(this);
                return;
            }

            CheckCompativelMode = true;

            Debug.Log(name + " total de Passivas iguais " + count);
        }

        GetUser();

        LoadEffects();

        if (CooldownMax >= 0)
        CooldownCurrent = 0;

        _silence     = false;
        _silenceTime = 0;       
    }

    public virtual void AttDescription()
    {
        if (XmlMobPassive.Instance != null)
        {
            PassiveXml p = XmlMobPassive.Instance.GetPassive(_XmlID);

            if (p._nameX != null)
                _Nome = p._nameX;

            if (p._description != null)
                _Description = p._description;
        }
    }

    protected virtual void LoadEffects()
    {
        if (_prefabInLoadingFx != null && _inloadingFx == null)
        {
            _inloadingFx = Instantiate(_prefabInLoadingFx, transform.position, transform.rotation);

            if (_inloadingFx != null)
            {
                _inloadingFx.name = _Nome + " (InLoading) - " + user.name;

                _inloadingFx.target = User;
            }
        }

        if (_prefabActiveFx != null && _activeFx == null)
        {
            _activeFx = Instantiate(_prefabActiveFx, transform.position, transform.rotation);

            if (_activeFx!=null)
            {
                _activeFx.name = _Nome+" (Active) - " + user.name;

                _activeFx.target = User;
            }
        }

        if (_prefabCanActiveFx != null && _canActiveFx == null)
        {
            _canActiveFx = Instantiate(_prefabCanActiveFx, transform.position, transform.rotation);

            if (_canActiveFx!=null)
            {
                _canActiveFx.name = _Nome+ " (Can Active) -  " + user.name;
                ActiveDesativeEffect();

                _canActiveFx.target = user;
            }
        }

        ActiveDesativeEffect(false, false);
        ActiveDesativeEffect(cooldownCurrent <= 0);
    }

    protected virtual void Start()
    {
        //GetUser();                   
        AttDescription();
        DescriptionPost();
    }

    protected virtual void OnEnable()
    {
        GetUser();

     //   if(_AttDescriptonOnEnable)
     //   DescriptionPost();
    }

    protected void GetUser()
    {
        if (User!=null)
            return;

        if (GetComponent<MobManager>())
            _mobManager = GetComponent<MobManager>();

        _mobSkillManager = GetComponent<MobSkillManager>();

        if (User == null)
        {
            if (_mobSkillManager != null)
            {
                User = _mobSkillManager.User;
                print("GetUserStatus -> _mobSkillManager -> " + User);
            }
            else
            if (GetComponent<MobSkillAreaDamage>())
            {
                User = GetComponent<MobSkillAreaDamage>().User;
                print("GetUserStatus -> MobSkillAreaDamage -> " + User);
            }
            else
                if (GetComponent<MobManager>())
            {
                User = gameObject;
                print("GetUserStatus -> MobManager -> " + User);
            }
            else
                if (GetComponent<SkillAttack>())
            {
                if (GetComponent<SkillAttack>().who != null)
                {
                    User = GetComponent<SkillAttack>().who;
                    print("GetUserStatus -> SkillAttack  Who -> " + User);
                }
                
                  if (GetComponent<SkillAttack>().SkillManager != null)
                {
                    _mobSkillManager = GetComponent<SkillAttack>().SkillManager;

                    if (User==null)
                    {
                        User = _mobSkillManager.User;

                        print("GetUserStatus -> SkillAttack Skill Manager-> " + User);
                    }                
                }
            }
            else
            {
                print("<color=red>GetUserStatus Null</color>");
            }
        }

        if (_mobSkillManager == null && user != null && user.GetComponent<MobSkillManager>())
        {
            _mobSkillManager = user.GetComponent<MobSkillManager>();
        }

        if (user !=null && user.GetComponent<SkillManager>()!=null)
        user.GetComponent<SkillManager>().Passives.Add(this);


        if (user != null && _mobManager== null)
        {
            _mobManager = user.GetComponent<MobManager>();
        }

        if ((int)GameManagerScenes._gms.Dificuldade() <= 1 && _truePassive && !_mobManager.isPlayer && !GameManagerScenes.BattleMode)
        {
            Debug.LogWarning(name + " Esta Passiva não pode ser ativada nessa dificuldade");
            //Destroy(this);
            enabled = false;
            _XmlID = -1;
            _Nome = "";
            _Description = "";
            _AttDescriptonOnTooltip = false;
            ActiveDesativeEffect(false,false);
            _silence = true;
            Destroy(this);
            return;
        }
    }

    protected bool descriptionPost = false;
    protected virtual void DescriptionPost()
    {        
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)        
            return;             
        
        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        _Description +=  cooldownMax == -1 
            ? "\n"+XmlMenuInicial.Instance.Get(68)/*cooldown*/+ "<b>: " + (XmlMenuInicial.Instance.Get(72)/*Ativação Única*/)
            : "";

        _Description = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

        if (_mobSkillManager && !_AttDescriptonOnTooltip)
        {
            _mobSkillManager.Description += _Description;
            descriptionPost = true;
        }
    }

    public virtual string DescriptionToolType
    {
        get
        {
            string _r = "";

            if (User == null || !_AttDescriptonOnTooltip)
                return _r;

            _Description = XmlMobPassive.Instance.GetDescription(_XmlID);

            AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

            AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

            _Description += cooldownMax == -1
            ? "\n" + XmlMenuInicial.Instance.Get(68)/*cooldown*/+ "<b>: " + (XmlMenuInicial.Instance.Get(72)/*Ativação Única*/)
            : "";
            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>"            
              +"\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())           
                GetComponent<ToolTipType>()._passiveDesc = _r;           

            return _r;
        }
    }

    public virtual void StartPassive(GameObject target,params Passive[] passive)
    {
        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return;
        }

        Debug.LogError("Passiva[" + _Nome + "] foi ativa");

        foreach (var i in passive)
        {
            foreach (var y in _Passive)
            {
                if (y._startPassive == i && ChanceActivePassive() && _mobSkillManager!=null && _mobSkillManager.useSkill)
                {
                  //  if (_mobSkillManager != null && _mobSkillManager.useSkill || _mobSkillManager == null)
                        ActivePassive(target, y._eventPassiveEffect);
                }
            }
        }
    }
    public virtual void StartPassive(GameObject target,  float value, params Passive[] passive)
    {
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
                if (y._startPassive == i && ChanceActivePassive())
                {
                  //  if (_mobSkillManager != null && _mobSkillManager.useSkill || _mobSkillManager == null)
                        ActivePassive(target, y._eventPassiveEffect);
                }
            }
        }
    }
    public virtual int StartPassiveReturn(GameObject target, float value, params Passive[] passive)
    {
        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return -1;
        }

        foreach (var i in passive)
        {
            foreach (var y in _Passive)
            {
                if (y._startPassive == i && ChanceActivePassive())
                {
                    //  if (_mobSkillManager != null && _mobSkillManager.useSkill || _mobSkillManager == null)
                    ActivePassive(target, y._eventPassiveEffect);
                }
            }
        }

        return 0;
    }

    public virtual void ActivePassive(GameObject target, UnityEvent _event)
    {
        Debug.LogError("Passiva["+_Nome+"] foi ativa");

        CooldownReset();

        _event.Invoke();
    }

    protected void AttDescription(string key, string _new)
    {
       // if (_AttDescriptonOnTooltip)
        {
            GameManagerScenes gms = GameManagerScenes._gms;

            if (gms != null)
                _Description = gms.AttDescrição(_Description, key, _new, _Description);
            else
                Debug.LogError("Gms Não encontrado!!!");
        }
    }

    protected virtual void OnDestroy()
    {
        GetUser();

   /*     if (_AttDescriptonOnTooltip && User!=null)
        {
            ToolTipType tt = User.GetComponent<ToolTipType>();

            if (tt != null)
            {
                tt.extraInInfo = GameManagerScenes._gms.AttDescrição(tt.extraInInfo, _Description, "", tt.extraInInfo);
            }
        }*/
    }

    protected virtual void OnDisable()
    {
        GetUser();

        /*
                if (_AttDescriptonOnTooltip && User!=null)
                {
                    ToolTipType tt = User.GetComponent<ToolTipType>();

                    if (tt != null)
                    {
                        tt.extraInInfo = GameManagerScenes._gms.AttDescrição(tt.extraInInfo, _Description, "", tt.extraInInfo);
                    }
                }*/
    }

    protected virtual bool ChanceActivePassive(float extra=0)
    {
        if (_silence)
        {
            Debug.LogError("Passiva[" + _Nome + "] Está Silenciada por "+_silenceTime+" turnos");
            return false;
        }

        if (CooldownCurrent > 0 || CooldownCurrent == -2)
            return false;

        if (_chanceActivePassive+extra >= 100)
            return true;
        else
            if (_chanceActivePassive + extra <= 0)
            return false;

        return (Random.value * 100) <= (_chanceActivePassive + extra);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chance">0 - 100</param>
    /// <param name="extra">sum chance</param>
    /// <returns></returns>
    protected virtual bool CheckChance(float chance,float extra = 0)
    {
        if (chance + extra >= 100)
            return true;
        else
            if (chance + extra <= 0)
            return false;

        return (Random.value * 100) <= (chance + extra);
    }

    protected virtual void ActiveDesativeEffect(bool active = true, bool canActive = true)
    {
        Debug.LogError("ActiveDesativeEffect => active -> " + active +" / CanActive? -> "+canActive);

        if (canActive)
        {
            if (_canActiveFx != null)                
                if (active && !SilencePassive && CooldownCurrent == 0 || !active)
                {
                    _canActiveFx.gameObject.SetActive(active);

                    Debug.LogError("CanActive => "+active+" Silence("+SilencePassive+" / "+SilenceTime+"t) - CC("+cooldownCurrent+")");
                }
        }

        if (!canActive)
        {
            if (_activeFx != null)
                if (active || _activeFx.TimeActive <= 0 && !active)             
            { 
                _activeFx.gameObject.SetActive(active);

                Debug.LogError("Active => " + active);
            }

            if (_inloadingFx!=null)
                if(CooldownCurrent > 0)
                {
                    _inloadingFx.gameObject.SetActive(!active);

                    Debug.LogError("InLoading => " + active);
                }
        }
    }

    public void TesteStartPassive()
    {
        print("TesteStartPassive()");

        StartPassive(gameObject, _Passive[0]._startPassive);
    }
}
