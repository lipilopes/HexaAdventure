using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffMobDbuff
{
    public int        _value;
    public int        _time;
    public int        _id;
    public GameObject _whoActive;//who active
    [HideInInspector]
    public int       _acumule;//Acumulos
}

public class MobDbuff : MonoBehaviour
{
    ToolTipType tooltip;
    InfoTable infoTable;
    EffectManager effectManager;
    MobManager mobManager;
    MobHealth mobhealth;
    TurnSystem turnSystem;
    ButtonManager buttonManager;
    SkillManager skillManager;

    bool activeDelegate = false;

    [Header("Fire")]
    [SerializeField] bool _fire;
    [SerializeField] int _timeFire, _turnoFire, _turnoFireUpdate;//tempo do dbuff / em qual rodada pegou o buff
    float _fireDamageResistence;
    GameObject whoFire;//who active dbuff
    public bool Fire { get { return _fire; } }
    public int FireTurn { get { return _timeFire; } }
    public static int PorcentDamageFire = 5;
    float damageFire;
    public string InfoFire
    {
        get
        {
            return GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(95)//"Efeito <b>{0}</b>,\n{1} Em cada iniciar de turno(Qualquer um),\n leva Dano: {2}.\n efeito vai durar: {3} Turno(s)."
                     , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Fire)
                     , "<b>" + tooltip._name + "</b>"
                     , "<color=red>" + damageFire.ToString("F0") + "</color>"
                     , "<b>" + _timeFire + "</b>");
        }
    }

    [Header("Poison")]
    [SerializeField] bool _poison;
    [SerializeField] int _timePoison, _turnoPoison, _turnoPoisonUpdate;
    float _poisonDamageResistence;
    GameObject whoPoison;//who active dbuff
    public bool Poison { get { return _poison; } }
    public int PoisonTurn { get { return _timePoison; } }
    public static int PorcentDamagePoison = 10;
    float damagePoison;
    public string InfoPoison
    {
        get
        {
            return GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(96)//"Efeito <b>{0}</b>,\nDano: {1} para cada vez que o(a) {2} andar.\n Efeito vai durar: \n {3} Turno(s)."
                , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Envenenar)
                , "<color=red>" + damagePoison.ToString("F0") + "</color>"
                , "<b>" + tooltip._name + "</b>"
                , "<b>" + _timePoison + "</b>");
        }
    }

    [Header("Petrify")]
    [SerializeField] bool _petrify;
    [SerializeField] int _timePetrify;
    [SerializeField] Material originalMaterial;
    float _petrifyDamageResistence;
    GameObject whoPetrify;//who active dbuff
    public bool Petrify { get { return _petrify; } }
    public int PetrifyTurn { get { return _timePetrify; } }
    public static int PorcentDamagePetrify = 25;
    float damagePetrify;
    public string InfoPetrify
    {
        get
        {
            return GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(97)//"Efeito Dbuff <b>{0}</b>,\n {1} leva Dano: {2} no começo de seu turno\n Perde {3} Turno(s)."
                , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Petrificar)
                , "<b>" + tooltip._name + "</b>"
                , "<color=red>" + damagePetrify.ToString("F0") + "</color>"
                , "<b>" + _timePetrify + "</b>");
        }
    }

    [Header("Stun")]
    [SerializeField] bool _stun;
    [SerializeField] int _timeStun;
    public bool Stun { get { return _stun; } }
    public int StunTurn { get { return _timeStun; } }
    public string InfoStun
    {
        get
        {
            return GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(98)//"Efeito Dbuff <b>{0}</b>, \n {1} perde {2} Turno(s)."
                , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Stun)
                , "<b>" + tooltip._name + "</b>"
                , "<b>" + StunTurn + "</b>");
        }
    }

    [Header("Bleed")]
    [SerializeField] bool _bleed;
    [SerializeField] int _timeBleed, _turnoBleed, _turnoBleedUpdate;//tempo do dbuff / em qual rodada pegou o buff
    float _bleedDamageResistence;
    GameObject whoBleed;//who active dbuff
    public bool Bleed { get { return _bleed; } }
    public int BleedTurn { get { return _timeBleed; } }
    public static int PorcentDamageBleed = 5;
    float damageBleed;
    public string InfoBleed
    {
        get
        {
            return GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(99)//"Efeito <b>{0}</b>, {1} leva dano: {2} em todo inicio de turno(qualquer um) \n e ao andar\nPerde o Escudo caso tenha.\n efeito vai durar: \n{3} Turno(s)."
                   , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Bleed)
                   , "<b>" + tooltip._name + "</b>"
                   , "<color=red>" + damageBleed.ToString("F0") + "</color>"
                   , "<b>" + _timeBleed + "</b>");
        }
    }

    [Header("Silence Skill")]
    [SerializeField, Tooltip("Skill Silenciada, não pode ser usada")]
    protected bool _silence = false;
    public bool Silence { get { return _silence; } }
    [SerializeField]
    List<BuffMobDbuff> _silenceList = new List<BuffMobDbuff>();
    public string InfoSilenceSkill
    {
        get
        {
            return GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(99)//"Efeito <b>{0}</b>, {1} tem sua(s) skill's Desabilidadas: {2} por {3} Turno(s)."
                   , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Silence)
                   , "<b>" + tooltip._name + "</b>"
                   , "<color=red>" + 0/*damageBleed.ToString("F0")*/ + "</color>"
                   , "<b>" + 0/*_timeBleed*/+ "</b>");
        }
    }

    [Header("Buff Damage")]
    [SerializeField] bool _buffDamage;
    [SerializeField]
    List<BuffMobDbuff> _buffDamageList = new List<BuffMobDbuff>();

    [Header("DBuff Damage")]
    [SerializeField] bool _dbuffDamage;
    [SerializeField]
    List<BuffMobDbuff> _dbuffDamageList = new List<BuffMobDbuff>();

    [Header("Silence Passive")]
    [SerializeField, Tooltip("Skill Silenciada, não pode ser usada")]
    protected bool _silencePassive = false;
    public bool SilencePassive { get { return _silencePassive; } }
    [SerializeField]
    List<BuffMobDbuff> _silencePassiveList = new List<BuffMobDbuff>();

    [Header("Buff Armor")]
    [SerializeField]
    bool _buffArmor;
    [SerializeField]
    List<BuffMobDbuff> _buffArmorList = new List<BuffMobDbuff>();

    void Start()
    {
        effectManager = EffectManager.Instance;
        mobManager = GetComponent<MobManager>();
        mobhealth = GetComponent<MobHealth>();
        turnSystem = (TurnSystem)FindObjectOfType(typeof(TurnSystem));
        tooltip = GetComponent<ToolTipType>();
        if (turnSystem != null)
            infoTable = turnSystem.GetComponent<InfoTable>();
        buttonManager = (ButtonManager)FindObjectOfType(typeof(ButtonManager));
        skillManager = GetComponent<SkillManager>();

        originalMaterial = GetComponent<MeshRenderer>().materials[0];
    }

    void Update()
    {
        if (_fire != mobManager.fire || _poison != mobManager.poison || _petrify != mobManager.petrify || _stun != mobManager.stun)
            CheckDbuff();

        if (mobManager.DamageResistenceFire != _fireDamageResistence || mobManager.DamageResistencePoison != _poisonDamageResistence ||
            mobManager.DamageResistencePetrify != _petrifyDamageResistence || mobManager.DamageResistenceBleed != _bleedDamageResistence)
            CheckResistence();
    }

    void OnDisable()
    {
        DesativeAllDelegate(true);
    }

    void OnEnable()
    {
        StartCoroutine(AtiveAllDelegate());
    }

    IEnumerator AtiveAllDelegate()
    {
        while (TurnSystem.Instance == null)
            yield return null;

        TurnSystem.DelegateTurnEnd += CheckTimeBuffArmorDelegate;

        TurnSystem.DelegateTurnEnd += CheckTimeBuffDamageDelegate;

        TurnSystem.DelegateTurnEnd += CheckTimeDBuffDamageDelegate;

        TurnSystem.DelegateTurnEnd += CheckTimeSilenceSkillDelegate;

        TurnSystem.DelegateTurnEnd += CheckTimeSilencePassiveDelegate;//Passiva

        activeDelegate = true;

        Debug.LogError("AtiveAllDelegate Active");
    }

    void DesativeAllDelegate(bool removeDelegate = false)
    {

        if (removeDelegate && TurnSystem.Instance)
        {
            TurnSystem.DelegateTurnEnd -= CheckTimeBuffArmorDelegate;

            TurnSystem.DelegateTurnEnd -= CheckTimeBuffDamageDelegate;

            TurnSystem.DelegateTurnEnd -= CheckTimeSilenceSkillDelegate;

            TurnSystem.DelegateTurnEnd -= CheckTimeDBuffDamageDelegate;

            TurnSystem.DelegateTurnEnd -= CheckTimeSilencePassiveDelegate;//Passiva
        }

        #region Silence
        if (_silence)
            CooldownSilenceSkill(true);
        #endregion

        #region Silence Passive
        if (_silencePassive)
            CooldownSilencePassive(true);
        #endregion

        #region Buff Damage
        if (_buffDamage)
        {
            foreach (var b in _buffDamageList)
                b._time = 0;

            CheckTimeBuffDamageDelegate();
        }
        #endregion

        #region DBuff Damage
        if (_dbuffDamage)
        {
            foreach (var b in _dbuffDamageList)
                b._time = 0;

            CheckTimeDBuffDamageDelegate();
        }
        #endregion

        #region Buff Armor
        if (_buffArmor)
        {
            foreach (var b in _buffArmorList)
                b._time = 0;

            CheckTimeBuffArmorDelegate();
        }
        #endregion
    }

    void CheckDbuff()
    {
        mobManager.fire = _fire;
        mobManager.poison = _poison;
        mobManager.petrify = _petrify;
        mobManager.stun = _stun;
        mobManager.bleed = _bleed;
    }

    /// <summary>
    /// -1:ALL 0:Fire, 1:Poison, 2:Petrify, 3:Stun, 4:Bleed
    /// </summary>
    /// <param name="idIcon"></param>
    public void CheckIconsDbuff(int idIcon)
    {
        if (ButtonManager.Instance.player == gameObject)
        {
            #region Fire 0
            if (idIcon == 0 || idIcon == -1)
            {
                buttonManager.FindIcon(0, Fire, FireTurn.ToString("F0")).GetComponent<ToolTipType>()._descricao = InfoFire;
            }
            #endregion

            #region Poison 1
            if (idIcon == 1 || idIcon == -1)
            {
                buttonManager.FindIcon(1, Poison, PoisonTurn.ToString("F0")).GetComponent<ToolTipType>()._descricao = InfoPoison;
            }
            #endregion

            #region Petrify 2
            if (idIcon == 2 || idIcon == -1)
            {
                buttonManager.FindIcon(2, Petrify, PetrifyTurn.ToString("F0")).GetComponent<ToolTipType>()._descricao = InfoPetrify;
            }
            #endregion

            #region Stun 3
            if (idIcon == 3 || idIcon == -1)
            {
                buttonManager.FindIcon(3, Stun, StunTurn.ToString("F0")).GetComponent<ToolTipType>()._descricao = InfoStun;
            }
            #endregion

            #region Bleed 4
            if (idIcon == 4 || idIcon == -1)
            {
                buttonManager.FindIcon(4, Bleed, BleedTurn.ToString("F0")).GetComponent<ToolTipType>()._descricao = InfoBleed;
            }
            #endregion
        }
    }

    void CheckResistence()
    {
        _fireDamageResistence = mobManager.DamageResistenceFire;

        _poisonDamageResistence = mobManager.DamageResistencePoison;

        _petrifyDamageResistence = mobManager.DamageResistencePetrify;

        _bleedDamageResistence = mobManager.DamageResistenceBleed;
    }

    public void ClearDbuff(bool clearBuff = true)
    {
        if (_fire)
            AtiveDbuffFire(null, false);
        if (_poison)
            AtiveDbuffPoison(null, false);
        if (_petrify)
            AtiveDbuffPetrify(null, false);
        if (_stun)
            AtiveDbuffStun(false);
        if (_bleed)
            AtiveDbuffBleed(null, false);

        if (clearBuff)
            DesativeAllDelegate();
    }

    #region FIRE
    public bool AtiveDbuffFire(GameObject who, bool ative = true, int duration = 1, float _chance = 50)
    {
        if (ative)
        {
            if (_chance > 1)
                _chance = _chance / 100;

            if (_chance == 0 || Random.value > _chance)
                return false;

            if (_fireDamageResistence >= 100)
            {
                effectManager.PopUpDamageEffect("<color=blue>" + XmlMenuInicial.Instance.Get(100)/*Imune*/+ "</color>", gameObject);
                AtiveDbuffFire(who, false);
                return false;
            }

            whoFire = who;

            effectManager.FireEffect(gameObject);

            _fire = ative;

            if (_timeFire < duration)
            {
                _timeFire = duration;
                _turnoFire = turnSystem.totalZeroTurn;
                _turnoFireUpdate = turnSystem.isTurn;
            }

            if (whoFire != null)
                damageFire = whoFire.GetComponent<MobManager>().damage;

            if (_fireDamageResistence > 0)
                damageFire = damageFire - (damageFire * _fireDamageResistence / 100);

            if (GameManagerScenes.BattleMode)
                PorcentDamageFire = GameManagerScenes._gms.BattleModeOptionDbuffFirePorcentDamage;
            else
            {
                if (RespawMob.Instance.Player == gameObject)
                    GameManagerScenes._gms.TotalBurn();
                else
                if (!mobManager.isPlayer)
                    GameManagerScenes._gms.TotalBurn(false);
            }

            damageFire = damageFire * PorcentDamageFire / 100;

            //Debug.LogWarning(name + " Esta com o dbuff Fire ativo na rodadaZero " + _turnoFire + ",e vai durar " + duration + " Rodada(s).");

            if (mobManager.MesmoTime(RespawMob.Instance.PlayerTime) && gameObject != RespawMob.Instance.Player)
            {
                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(101)//"{0} esta com o Dbuff <b>{1}</b>, \n efeito vai durar: \n {2} Turno(s)/Rodada(s)."
                    , "<b>" + tooltip._name + "</b>"
                    , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Fire)
                    , "<b>" + _timeFire + "</b>"
                    ), 7);
                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(102)//"Efeito <b>{0}</b>, \n Em cada iniciar de turno(Qualquer um),\n{1} vai levar um pequeno Dano extra."
                    , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Fire)
                    , "<b>" + tooltip._name + "</b>"),
                    7);
            }
        }
        else
        {
            if (ButtonManager.Instance.player == gameObject)
                buttonManager.FindIcon(0, false);

            damageFire = 0;
            whoFire = null;
            effectManager.FireReset(gameObject);
            _fire = ative;
            _timeFire = 0;
            _turnoFire = -1;
            //Debug.LogWarning(name + " Sobreviveu ao Dbuff Fire");
        }

        CheckIconsDbuff(0);
        CheckDbuff();
        return true;
    }
    public void CooldownDbuffFire()
    {
        if (_fire && _turnoFire != turnSystem.totalZeroTurn)
        {
            _timeFire--;
            _turnoFireUpdate--;

            if (_timeFire <= 0 & _turnoFireUpdate <= 0)
            {
                AtiveDbuffFire(null, false);//desativa
            }
        }
    }
    public void DamageDbuffFire(float Damage = -1, float PorcDamage = 0, float ChanceCritital = 0)
    {
        if (!_fire)
            return;

        if (Damage != -1 && whoFire != null)
        {
            Damage = whoFire.GetComponent<MobManager>().damage;

            if (_fireDamageResistence > 0)
                Damage = Damage - (Damage * _fireDamageResistence / 100);

            Damage = Damage * (PorcentDamageFire + PorcDamage) / 100;
        }
        else
            Damage = damageFire;
        //Debug.Log(name + " Levou dano de " + Damage + " do debuff Fire.");

        if (Damage <= 0)
        {
            //AtiveDbuffFire(null, false);

            if (_fireDamageResistence >= 100)
                effectManager.PopUpDamageEffect("<color=blue>" + XmlMenuInicial.Instance.Get(100) +/*Imune*/"</color>", gameObject);
        }
        else
            mobhealth.GetDamage(whoFire, Damage, ChanceCritital, false,false);
    }
    #endregion

    #region POISON
    public bool AtiveDbuffPoison(GameObject who, bool ative = true, int duration = 2, float _chance = 50)
    {
        if (ative)
        {
            if (_chance > 1)
                _chance = _chance / 100;

            if (_chance == 0 || Random.value > _chance)
                return false;

            if (_poisonDamageResistence >= 100)
            {
                effectManager.PopUpDamageEffect("<color=blue>" + XmlMenuInicial.Instance.Get(100) +/*Imune*/"</color>", gameObject);
                AtiveDbuffPoison(who, false);
                return false;
            }

            whoPoison = who;

            effectManager.PoisonEffect(gameObject);
            _poison = ative;

            if (_timePoison < duration)
            {
                _timePoison = duration;
                _turnoPoison = turnSystem.totalZeroTurn;
                _turnoPoisonUpdate = turnSystem.isTurn;
            }

            if (whoPoison != null)
                damagePoison = whoPoison.GetComponent<MobManager>().damage;

            if (_poisonDamageResistence > 0)
                damagePoison = damagePoison - (damagePoison * _poisonDamageResistence / 100);

            if (GameManagerScenes.BattleMode)
                PorcentDamagePoison = GameManagerScenes._gms.BattleModeOptionDbuffPoisonPorcentDamage;
            else
            {
                if (RespawMob.Instance.Player == gameObject)
                    GameManagerScenes._gms.TotalPoison();
                else
                                if (!mobManager.isPlayer)
                    GameManagerScenes._gms.TotalPoison(false);
            }

            damagePoison = damagePoison * PorcentDamagePoison / 100;

            //Debug.LogWarning(name + " Esta com o dbuff Poison ativo por " + duration + " Rodada(s)");

            if (mobManager.MesmoTime(RespawMob.Instance.PlayerTime) && gameObject != RespawMob.Instance.Player)
            {
                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(103)//"{0} esta com o Dbuff <b>{1}</b>, \n efeito vai durar: \n {2} Turno(s)/Rodada(s)."
                    , "<b>" + tooltip._name + "</b>"
                    , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Envenenar)
                    , "<b>" + _timePoison + "</b>"), 7);


                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                   XmlMenuInicial.Instance.Get(104)//"Efeito <b>{0}</b>, \n Vai dar um pequeno dano extra cada vez que o(a) {1} andar."
                   , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Envenenar)
                   , "<b>" + tooltip._name + "</b>"), 7);
            }
        }
        else
        {
            if (ButtonManager.Instance.player == gameObject)
                buttonManager.FindIcon(1, false);

            effectManager.PoisonReset(gameObject);

            damagePoison = 0;
            whoPoison = null;
            _poison = ative;
            _timePoison = 0;
            _turnoPoison = -1;
            _turnoPoisonUpdate = -1;
            //Debug.LogWarning(name + " Sobreviveu ao Dbuff Poison.");
        }
        CheckIconsDbuff(1);
        CheckDbuff();
        return true;
    }
    public void CooldownDbuffPoison()
    {
        if (_poison && _turnoPoison != turnSystem.totalZeroTurn)
        {
            _timePoison--;
            _turnoPoison -= turnSystem.totalZeroTurn;
            _turnoPoisonUpdate -= turnSystem.isTurn;

            if (_timePoison <= 0 & _turnoPoison <= 0/* && _turnoPoisonUpdate<=0*/)
            {
                AtiveDbuffPoison(null, false);//desativa
            }
        }
    }
    public void DamageDbuffPoison(float Damage = -1, float PorcDamage = 0, float ChanceCritital = 0)
    {
        if (!_poison)
            return;

        if (Damage != -1 && whoPoison != null)
        {
            Damage = whoPoison.GetComponent<MobManager>().damage;

            if (_poisonDamageResistence > 0)
                Damage = Damage - (Damage * _poisonDamageResistence / 100);

            Damage = Damage * (PorcentDamagePoison + PorcDamage) / 100;
        }
        else
            Damage = damagePoison;

        //Debug.Log(name + " Levou dano de " + Damage + " do debuff Poison.");

        if (Damage <= 0)
        {
            //AtiveDbuffPoison(null, false);

            if (_poisonDamageResistence >= 100)
                effectManager.PopUpDamageEffect("<color=blue>" + XmlMenuInicial.Instance.Get(100) +/*Imune*/"</color>", gameObject);
        }
        else
            mobhealth.GetDamage(whoPoison, Damage, ChanceCritital, false,false);
    }
    #endregion

    #region PETRIFY
    public bool AtiveDbuffPetrify(GameObject who, bool ative = true, int duration = 1, float _chance = 25)
    {
        if (ative)
        {
            if (_chance > 1)
                _chance = _chance / 100;

            if (_chance == 0 || Random.value > _chance)
                return false;

            if (_petrifyDamageResistence >= 100)
            {
                effectManager.PopUpDamageEffect("<color=blue>" + XmlMenuInicial.Instance.Get(100) +/*Imune*/"</color>", gameObject);
                AtiveDbuffPetrify(who, false);
                return false;
            }

            whoPetrify = who;

            GetComponent<MeshRenderer>().materials[0] = effectManager.petrifyMaterial;

            effectManager.PetrifyEffect(gameObject);

            _petrify = ative;

            if (_timePetrify < duration)
                _timePetrify = duration;

            if (whoPetrify != null)
                damagePetrify = whoPetrify.GetComponent<MobManager>().damage;

            if (_petrifyDamageResistence > 0)
                damagePetrify = damagePetrify - (damagePetrify * _petrifyDamageResistence / 100);

            if (GameManagerScenes.BattleMode)
                PorcentDamagePetrify = GameManagerScenes._gms.BattleModeOptionDbuffPetrifyPorcentDamage;
            else
            {
                if (RespawMob.Instance.Player == gameObject)
                    GameManagerScenes._gms.TotalPetrify();
                else
                if (!mobManager.isPlayer)
                    GameManagerScenes._gms.TotalPetrify(false);
            }

            damagePetrify = damagePetrify * PorcentDamagePetrify / 100;

            //Debug.LogWarning(name + " Esta com o dbuff Petrify ativo por " + _timePetrify + " Rodada(s). com duração de ");
            if (mobManager.MesmoTime(RespawMob.Instance.PlayerTime) && gameObject != RespawMob.Instance.Player)
            {
                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                   XmlMenuInicial.Instance.Get(105)//"{0} esta com o Dbuff {1}, \n efeito vai durar: \n{2} Turno(s)/Rodada(s)."
                   , "<b>" + tooltip._name + "</b>"
                   , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Petrificar)
                   , "<b>" + _timePetrify + "</b>"), 7);


                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                   XmlMenuInicial.Instance.Get(106)//"Efeito {0}, \n {1} leva um dano extra no começo de seu turno \nPerde o Turno."
                    , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Petrificar)
                   , "<b>" + tooltip._name + "</b>"
                    ), 7);
            }
        }
        else
        {
            if (ButtonManager.Instance.player == gameObject)
                buttonManager.FindIcon(2, false);

            GetComponent<MeshRenderer>().material = originalMaterial;

            damagePetrify = 0;

            whoPetrify = null;

            _petrify = ative;

            _timePetrify = 0;

            effectManager.PetrifyReset(gameObject);

            //Debug.LogWarning(name + " Sobreviveu ao Dbuff Petrify.");
        }
        CheckIconsDbuff(2);
        CheckDbuff();
        return true;
    }
    public void CooldownDbuffPetrify()
    {
        if (_petrify)
        {
            effectManager.PetrifyEffect(gameObject);

            _timePetrify--;

            Debug.LogWarning(name + " esta petrificado e perdeu a vez.");

            //GetComponent<MobManager>().EndTurn();

            if (_timePetrify <= 0)
            {
                AtiveDbuffPetrify(null, false);//desativa
            }
        }
    }
    public void DamageDbuffPetrify(float Damage = -1, float PorcDamage = 0, float ChanceCritital = 0)
    {
        if (!_petrify)
            return;

        if (Damage != -1 && whoPetrify != null)
        {
            Damage = whoPetrify.GetComponent<MobManager>().damage;

            if (_petrifyDamageResistence > 0)
                Damage = Damage - (Damage * _petrifyDamageResistence / 100);

            Damage = Damage * (PorcentDamagePetrify + PorcDamage) / 100;
        }
        else
            Damage = damagePetrify;

        Debug.Log(name + " Levou dano de " + Damage + " do debuff Petrify.");

        if (Damage <= 0)
        {
            //AtiveDbuffPetrify(null, false);              
            effectManager.PopUpDamageEffect("<color=blue>" + XmlMenuInicial.Instance.Get(100) +/*Imune*/"</color>", gameObject);
        }
        else
            mobhealth.GetDamage(whoPetrify, Damage, ChanceCritital, false,false);
    }
    #endregion

    #region STUN
    public bool AtiveDbuffStun(bool ative = true, int duration = 1, float _chance = 25)
    {
        if (ative)
        {
            if (_chance > 1)
                _chance = _chance / 100;

            if (_chance == 0 || Random.value > _chance)
                return false;

            effectManager.StunEffect(gameObject);
            _stun = ative;

            if (_timeStun < duration)
                _timeStun = duration;


            if (RespawMob.Instance.Player == gameObject)
                GameManagerScenes._gms.TotalStun();
            else
            if (!mobManager.isPlayer)
                GameManagerScenes._gms.TotalStun(false);

            if (mobManager.MesmoTime(RespawMob.Instance.PlayerTime) && gameObject != RespawMob.Instance.Player)
            {
                //Debug.LogWarning(name + " Esta com o dbuff Stun ativo por " + _timeStun + " Rodada(s).");
                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                   XmlMenuInicial.Instance.Get(107)//"{0} esta com o Dbuff {1}, \n efeito vai durar: \n {2} Turno(s)/Rodada(s)."
                   , "<b>" + tooltip._name + "</b>"
                   , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Stun)
                   , "<b>" + _timeStun + "</b>"), 7);


                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(108)//"Efeito {0}, \n {1} perde \n {2} Turno(s)/Rodada(s)."
                    , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Stun)
                    , "<b>" + tooltip._name + "</b>"
                    , "<b>" + _timeStun + "</b>"), 7);
            }
        }
        else
        {
            if (ButtonManager.Instance.player == gameObject)
                buttonManager.FindIcon(3, false);

            _stun = ative;
            _timeStun = 0;

            effectManager.StunReset(gameObject);
            //Debug.LogWarning(name + " Sobreviveu ao Dbuff Stun.");
        }
        CheckIconsDbuff(3);
        CheckDbuff();
        return true;
    }
    public void CooldownDbuffStun()
    {
        if (!_stun)
            effectManager.StunReset(gameObject);

        if (_stun)
        {
            _timeStun--;

            //Debug.LogWarning(name+" esta stunado e perdeu a vez.");

            //GetComponent<MobManager>().EndTurn();

            if (_timeStun <= 0)
            {
                AtiveDbuffStun(false);//desativa
            }
        }
    }
    #endregion

    #region BLEED
    public bool AtiveDbuffBleed(GameObject who, bool ative = true, int duration = 2, float _chance = 25)
    {
        if (ative)
        {
            if (_chance > 1)
                _chance /= 100;

            if (_chance == 0 || Random.value > _chance)
                return false;

            if (_bleedDamageResistence >= 100)
            {
                effectManager.PopUpDamageEffect("<color=blue>" + XmlMenuInicial.Instance.Get(100) +/*Imune*/"</color>", gameObject);
                AtiveDbuffBleed(who, false);
                return false;
            }

            whoBleed = who;

            //effectManager.BleedEffect(gameObject);
            _turnoBleed = turnSystem.totalZeroTurn;

            _turnoBleedUpdate = turnSystem.isTurn;

            _bleed = ative;

            if (_timeBleed < duration)
                _timeBleed = duration;

            if (whoBleed != null)
                damageBleed = whoBleed.GetComponent<MobManager>().damage;

            if (_bleedDamageResistence > 0)
                damageBleed = damageBleed - (damageBleed * _bleedDamageResistence / 100);


            if (GameManagerScenes.BattleMode)
                PorcentDamageBleed = GameManagerScenes._gms.BattleModeOptionDbuffBleedPorcentDamage;
            else
            {
                if (RespawMob.Instance.Player == gameObject)
                    GameManagerScenes._gms.TotalBleed();
                else
             if (!mobManager.isPlayer)
                    GameManagerScenes._gms.TotalBleed(false);
            }

            damageBleed = damageBleed * PorcentDamageBleed / 100;

            if (mobManager.MesmoTime(RespawMob.Instance.PlayerTime) && gameObject != RespawMob.Instance.Player)
            {
                //Debug.LogWarning(name + " Esta com o dbuff Bleed ativo na rodadaZero " + _turnoBleed + ",e vai durar " + _timeBleed + " Rodada(s).");
                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(109)//"{0} esta com o Dbuff {1}, \n efeito vai durar: \n {2} Turno(s)/Rodada(s)."
                    , "<b>" + tooltip._name + "</b>"
                    , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Bleed)
                    , "<b>" + _timeBleed + "</b>"), 7);


                infoTable.NewInfo(GameManagerScenes._gms.AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(110)//"Efeito <b>{0}</b>, \n {1} leva um pequeno dano extra em todo inicio de turno(qualquer um) \n {2} leva um pequeno dano extra ao andar\nPerde o Escudo caso tenha."
                    , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Bleed)
                    , "<b>" + tooltip._name + "</b>"
                    , "<b>" + tooltip._name + "</b>"), 7);
            }
        }
        else
        {
            if (ButtonManager.Instance.player == gameObject)
                buttonManager.FindIcon(4, false);
            //effectManager.BleedReset(gameObject);
            damageBleed = 0;
            whoBleed = null;
            _bleed = ative;
            _timeBleed = 0;
            _turnoBleed = -1;
            //Debug.LogWarning(name + " Sobreviveu ao Dbuff Bleed");
        }
        CheckIconsDbuff(4);
        CheckDbuff();
        return true;
    }
    public void CooldownDbuffBleed()
    {
        if (_bleed && _turnoBleed != turnSystem.totalZeroTurn)
        {
            _timeBleed--;
            _turnoBleedUpdate--;
            if (_timeBleed <= 0 & _turnoBleedUpdate <= 0)
            {
                AtiveDbuffBleed(null, false);//desativa
            }
        }
    }
    public void DamageDbuffBleed(float Damage = -1, float PorcDamage = 0, float ChanceCritital = 0)
    {
        if (!_bleed)
            return;

        mobhealth.defense = 0;

        if (Damage != -1 && whoBleed != null)
        {
            Damage = whoBleed.GetComponent<MobManager>().damage;

            effectManager.DefeseReset(gameObject);

            if (_bleedDamageResistence > 0)
                Damage = Damage - (Damage * _bleedDamageResistence / 100);

            Damage = Damage * (PorcentDamageBleed + PorcDamage) / 100;
        }
        else
            Damage = damageBleed;
        //Debug.Log(name + " Levou dano de " + Damage + " do debuff Bleed.");

        if (Damage <= 0)
        {
            //AtiveDbuffBleed(null, false);
            if (_bleedDamageResistence >= 100)
                effectManager.PopUpDamageEffect("<color=blue>" + XmlMenuInicial.Instance.Get(100) +/*Imune*/"</color>", gameObject);
        }
        else
            mobhealth.GetDamage(whoBleed, Damage, ChanceCritital, false,false);
    }
    #endregion

    #region Silence Skill
    /// <summary>
    /// Desabilita a skill
    /// </summary>
    /// <param name="time">duração do dbuff</param>
    /// <param name="indexSkill">index da skill [-2: Todas, -3 Menor cooldown, -4 Todas as skills disponiveis]</param>
    /// <param name="who">Quem ativou</param>
    /// <param name="instanceId">Id do buff para atualizar valor</param>
    /// <param name="acumule">Pode Acumular o Tempo do SilenceSkill</param>
    /// <param name="maxAcumule">Pode Acumular no maximo X vezes o Dbuff SilenceSkill</param>
    public void AtiveDbuffSilenceSkill(int time, int indexSkill, GameObject who, int instanceId = -1, bool acumule = false, int maxAcumule = -1)
    {
        //para contar o tempo certo
        time++;

        bool temNaLista = false;

        effectManager.SilenceSkillEffect(gameObject);

        #region Index Da Skill Atualizar Dbuff
        if (instanceId != -1 && indexSkill >= 0)//Atualizar Dbuff
        {
            foreach (var b in _silenceList)
            {
                if (b._id == instanceId && b._value == indexSkill)//Procura a id
                {
                    if (acumule && maxAcumule > 0  && b._acumule >= maxAcumule)
                {
                    Debug.LogError(name + " Esta no seu Acumulo maximo do Silenciar.");
                    return;
                }

                
                    Debug.LogError("Buff Silenciar do " + name + " Atualizado");

                    int newTime = 0;

                    if (acumule)
                    {
                        newTime = b._time + time;

                        b._acumule++;
                    }
                    else
                        newTime = time > b._value ? time : b._value;

                    b._time = newTime;
                    b._value = indexSkill;
                    b._whoActive = who;

                    //Soma novo
                    skillManager.Skills[indexSkill].Silence(newTime);

                    temNaLista = true;
                    break;
                }
            }
        }
        #endregion

        if (!temNaLista)//Criar Dbuff na lista
        {
            int menorCooldown = 99999,
            indexMenorCooldown = -1;

            #region Criar Dbuff na Lista
            for (int i = 0; i < skillManager.Skills.Count; i++)//Procura na lista de skills
                if (skillManager.Skills[i] != null)//Skill existe                
                    if (i == indexSkill || indexSkill == -2)//index iguais ou todos
                    {
                        Debug.LogError(skillManager.Skills[i].Nome + "(" + indexSkill + ") Skill[" + i + "] silenciada por " + time + " turno(s).");

                        skillManager.Skills[i].Silence(time);

                        BuffMobDbuff n = new BuffMobDbuff();
                        n._value = i;
                        n._time = time;
                        n._id = instanceId;
                        n._whoActive = who;

                        _silenceList.Add(n);
                    }
            #endregion


            if (indexSkill == -3)  //Menor Cooldown
            {
                for (int i = 0; i < skillManager.Skills.Count; i++)//Procura menor cooldown
                    if (skillManager.Skills[i].CooldownCurrent <= menorCooldown)
                    {
                        Debug.LogError(skillManager.Skills[i].Nome + " Skill[" + i + "] Esta com o menor cooldown");
                        menorCooldown = skillManager.Skills[i].CooldownCurrent;
                        indexMenorCooldown = i;
                    }

                skillManager.Skills[indexMenorCooldown].Silence(time);

                BuffMobDbuff n = new BuffMobDbuff();
                n._value = indexMenorCooldown;
                n._time = time;
                n._id = instanceId;
                n._whoActive = who;

                _silenceList.Add(n);
            }

            if (indexSkill == -4)  //Skill Ativa
            {
                for (int i = 0; i < skillManager.Skills.Count; i++)//Procura Skill Ativa
                    if (skillManager.Skills[i].CooldownCurrent == 0)
                    {
                        skillManager.Skills[i].Silence(time);

                        BuffMobDbuff n = new BuffMobDbuff();
                        n._value = i;
                        n._time = time;
                        n._id = instanceId;
                        n._whoActive = who;

                        _silenceList.Add(n);
                    }
            }
        }
        //if (skillManager.SkillsReserva != null && indexSkill==-1)
        //    skillManager.SkillsReserva.AtiveDbuffSilenceSkill(time);

        _silence = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reset">Desativar Todas</param>
    public void CooldownSilenceSkill(bool reset = false)
    {
        //Tamanho da lista for maior que 1
        if (_silence)
        {
            if (reset)//Remove todos da lista
            {
                _silenceList.Clear();

                for (int i = 0; i < skillManager.Skills.Count; i++)
                    skillManager.Skills[i].SilenceCooldown(reset);

                    effectManager.SilenceSkillReset(gameObject);
            }
            else//Reset é false
            {
                bool remove = false;

                //Procura na lista Alguem a ser removido
                for (int i = 0; i < _silenceList.Count; i++)
                {
                    _silenceList[i]._time--;                   

                    Debug.LogError(name + " SilenceSkill [" + _silenceList[i]._value + "] faltam: " + _silenceList[i]._time + " ->" + _silenceList[i]._id);

                    if (_silenceList[i]._time <= 0)
                    {
                        skillManager.Skills[_silenceList[i]._value].SilenceCooldown(true);

                        remove = true;

                        Debug.LogError("SilenceSkill da skill["+ _silenceList[i]._value+"] acabou");
                    }
                    else
                        skillManager.Skills[_silenceList[i]._value].SilenceCooldown();
                }

                //Remove Da lista
                if (remove)
                {
                    for (int i = 0; i < _silenceList.Count; i++)
                        if (_silenceList[i]._time <= 0)                                                     
                            _silenceList.Remove(_silenceList[i]);
                }    
            }
        }

        //Atualiza SilenceSkill
        _silence = _silenceList.Count >= 1;

        if (!_silence)
        {
            //Remove Effect
            effectManager.SilenceSkillReset(gameObject);
        }
    }  

    /// <summary>
    /// Delegate Only
    /// </summary>
    public void CheckTimeSilenceSkillDelegate()
    {
        CooldownSilenceSkill(false);
    }
    #endregion

    #region Buff Atk       
    /// <summary>
    /// Aumenta Dano do mob
    /// </summary>
    /// <param name="value">Valor a ser aumentado</param>
    /// <param name="time">Tempo de duração</param>
    /// <param name="who">Quem Ativou</param>
    /// <param name="instanceId">Id do buff para acumular valor</param>
    /// <param name="acumule">Pode Acumular o Valor do Buff Damage</param>
    /// <param name="maxAcumule">Pode Acumular no maximo X vezes o Buff Damage</param>
    public void AtiveBuffDamage(int value, int time,GameObject who, int instanceId = -1,bool acumule=false,int maxAcumule=-1)
    {
        if (skillManager==null)
        {
            Debug.LogError(name + " Não contém skillManager");
            return;
        }

        //Debug.LogError(name+" AtiveBuffDamage("+value+","+time+","+who+","+instanceId+")");

        //para contar o tempo certo
        if (time > 0)
            time++;

        effectManager.BuffDamageEffect(gameObject);

        bool temNalista = false;
        if (instanceId != -1 && acumule)//Atualizar Buff
        {
            foreach (var b in _buffDamageList)//Busca buff
            {
                if (b._id == instanceId)//Procurar id
                {
                    if (acumule && maxAcumule > 0 &&b._acumule >= maxAcumule)
                {
                    Debug.LogError(name + " Esta no seu Acumulo maximo do buff Atk.");
                    return;
                }

               
                    //Debug.LogError("Dano Bonus do " + name + " Atualizado");

                    int newValue = value;

                    if (acumule)
                    {
                        newValue = b._value + value;

                        b._acumule++;
                    }
                    else
                        mobManager.damage -= b._value;

                    b._time      = time;
                    b._value     = newValue;
                    b._whoActive = who;

                    //Soma novo
                    mobManager.damage += b._value;

                    temNalista = true;

                    break;
                }
            }
        }

        if(!temNalista)//Criar Buff na lista
        {
            //Debug.LogError("Dano Bonus do " + name + " Novo");

            BuffMobDbuff n = new BuffMobDbuff();

            n._value = value;
            n._time = time;
            n._id = instanceId;
            n._whoActive = who;

            _buffDamageList.Add(n);

            mobManager.damage += value;//Somar valor
        }

        if (skillManager != null)//Atualizar valores
            skillManager.AttStatusSkills();

        _buffDamage = true;
    }

    /// <summary>
    /// Delegate Only
    /// </summary>
    public void CheckTimeBuffDamageDelegate()
    {
        // Debug.LogError("CheckTimeBuffDamageDelegate()");

        if (_buffDamage)//Buff esta Ativo
        {
            if (_buffDamageList.Count >= 1)//A alguem na lista
            {
                bool remove = false;

                foreach (var b in _buffDamageList)//Reduzir tempo
                {
                    if (b._time != -2)
                    {
                        b._time--;

                        Debug.LogError(name + "Dano Bonus [" + b._value + "] faltam: " + b._time + " ->" + b._id);

                        if (b._time == 0)
                        {
                            b._time = 0;

                            remove = true;

                            mobManager.damage -= b._value;

                            Debug.LogError("Dano Bonus do " + name + " Acabou ->" + b._id);
                        }
                    }
                }


                    if (remove)//Remover da lista
                    {
                        for (int i = 0; i < _buffDamageList.Count; i++)
                            if (_buffDamageList[i]._time == 0)
                                _buffDamageList.Remove(_buffDamageList[i]);


                        if (skillManager != null)
                            skillManager.AttStatusSkills();
                    }
                
            }

            //Atualizar valor do buff
            _buffDamage = _buffDamageList.Count >= 1;

            if (!_buffDamage)
            {
                //Remove Effect
                effectManager.BuffDamageReset(gameObject);
            }
        }
    }
    #endregion

    #region DBuff Atk       
    /// <summary>
    /// Diminui Dano do mob
    /// </summary>
    /// <param name="value">Valor a ser aumentado</param>
    /// <param name="time">Tempo de duração</param>
    /// <param name="who">Quem Ativou</param>
    /// <param name="instanceId">Id do buff para acumular valor</param>
    /// <param name="acumule">Pode Acumular o Valor do Dbuff Damage</param>
    /// <param name="maxAcumule">Pode Acumular no maximo X vezes o Dbuff Damage</param>
    public void AtiveDBuffDamage(int value, int time, GameObject who, int instanceId = -1, bool acumule = false, int maxAcumule = -1)
    {
        if (skillManager == null)
        {
            Debug.LogError(name + " Não contém skillManager");
            return;
        }

        //Debug.LogError(name+" AtiveBuffDamage("+value+","+time+","+who+","+instanceId+")");

        //para contar o tempo certo
        if (time != -2)
            time++;           

        bool temNalista = false;
        if (instanceId != -1 && acumule)//Atualizar Buff
        {          
            foreach (var b in _dbuffDamageList)//Busca buff
            {
                if (b._id == instanceId)//Procurar id
                {
                    if (acumule && maxAcumule > 1 && b._acumule >= maxAcumule)
                {
                    Debug.LogError(name + " Esta no seu Acumulo maximo do Dbuff Atk.");
                    return;
                }

                
                    //Debug.LogError("Dano Bonus do " + name + " Atualizado");

                    int newValue = value;

                    if (acumule)
                    {
                        newValue = b._value + value;

                        b._acumule++;
                    }
                    else
                    {
                        mobManager.damage += b._value;
                    }

                    b._time      = time;
                    b._value     = newValue;
                    b._whoActive = who;

                    //tira novo
                    mobManager.damage -= b._value;

                    if (mobManager.damage<0)                    
                        mobManager.damage = 0;                   

                    temNalista = true;

                    break;
                }
            }
        }

        if (!temNalista)//Criar Buff na lista
        {
            //Debug.LogError("Dano Bonus do " + name + " Novo");

            BuffMobDbuff n = new BuffMobDbuff();

            n._value = value;
            n._time = time;
            n._id = instanceId;
            n._whoActive = who;

            _dbuffDamageList.Add(n);

            mobManager.damage -= value;//Tira valor

            if (mobManager.damage < 0)
                mobManager.damage = 0;
        }

        if (skillManager != null)//Atualizar valores
            skillManager.AttStatusSkills();

        _dbuffDamage = true;

        effectManager.DbuffDamageEffect(gameObject);
    }

    /// <summary>
    /// Delegate Only
    /// </summary>
    public void CheckTimeDBuffDamageDelegate()
    {
        // Debug.LogError("CheckTimeBuffDamageDelegate()");

        if (_dbuffDamage)//Buff esta Ativo
        {
            if (_dbuffDamageList.Count >= 1)//A alguem na lista
            {
                bool remove = false;

                foreach (var b in _dbuffDamageList)//Reduzir tempo
                {
                    if (b._time != -2)
                    {

                        b._time--;

                        Debug.LogError(name + " Dano Dbuff [" + b._value + "] faltam: " + b._time + " ->" + b._id);

                        if (b._time == 0)
                        {
                            b._time=0;

                            remove = true;

                            mobManager.damage += b._value;

                            Debug.LogError("Dano Dbuff do " + name + " Acabou ->" + b._id);
                        }
                    }
                }
                if (remove)//Remover da lista
                {
                    for (int i = 0; i < _dbuffDamageList.Count; i++)
                        if (_dbuffDamageList[i]._time == 0)
                            _dbuffDamageList.Remove(_dbuffDamageList[i]);


                    if (skillManager != null)
                        skillManager.AttStatusSkills();
                }
            }
        }

        //Atualizar valor do buff
        _dbuffDamage = _dbuffDamageList.Count >= 1;

        if (!_dbuffDamage)
        {
            //Remove Effect
            effectManager.DbuffDamageReset(gameObject);
        }
    }
    #endregion

    #region Silence Passive
    /// <summary>
    /// Desabilita a passiva
    /// </summary>
    /// <param name="time">duração do dbuff</param>
    /// <param name="indexSkill">index da passiva [-2: Todas, -3 Menor cooldown, -4 Todas as Passivas disponiveis]</param>
    /// <param name="who">Quem ativou</param>
    /// <param name="instanceId">Id do buff para atualizar valor</param>
    /// <param name="acumule">Pode Acumular o Tempo do SilenceSkill</param>
    /// <param name="maxAcumule">Pode Acumular no maximo X vezes o Dbuff SilenceSkill</param>
    public void AtiveDbuffSilencePassive(int time, int index, GameObject who, int instanceId = -1, bool acumule = false, int maxAcumule = -1)
    {
        //para contar o tempo certo
        time++;

        effectManager.SilencePassiveEffect(gameObject);

        bool temNaLista = false;

        #region Index Da Passiva Atualizar Dbuff
        if (instanceId != -1 && index >= 0)//Atualizar Dbuff
        {
            foreach (var b in _silencePassiveList)
            {
                if (b._id == instanceId && b._value == index)//Procura a id
                { 
                    if (acumule && maxAcumule > 0 && b._acumule >= maxAcumule)
                {
                    Debug.LogError(name + " Esta no seu Acumulo maximo do Silenciar.");
                    return;
                }

                    Debug.LogError("Buff Silenciar do " + name + " Atualizado");

                    int newTime = 0;

                    if (acumule)
                    {
                        newTime = b._time + time;

                        b._acumule++;
                    }
                    else
                        newTime = time > b._time ? time : b._time;

                    b._time = newTime;
                    b._value = index;
                    b._whoActive = who;

                    //Soma novo
                    skillManager.Passives[index].Silence(newTime);

                    temNaLista = true;
                    break;
                }
            }
        }
        #endregion

        if (!temNaLista)//Criar Dbuff na lista
        {
            int menorCooldown = 99999,
            indexMenorCooldown = -1;

            #region Criar Dbuff na Lista
            for (int i = 0; i < skillManager.Passives.Count; i++)//Procura na lista de skills
                if (skillManager.Passives[i] != null)//Passive existe                
                    if (i == index || index == -2)//index iguais ou todos
                    {
                        Debug.LogError(skillManager.Passives[i].Nome + "(" + index + ") Passiva[" + i + "] silenciada por " + time + " turno(s).");

                        skillManager.Passives[i].Silence(time);

                        BuffMobDbuff n = new BuffMobDbuff();
                        n._value = i;
                        n._time = time;
                        n._id = instanceId;
                        n._whoActive = who;

                        _silencePassiveList.Add(n);
                    }
            #endregion

            if (index == -3)  //Menor Cooldown
            {
                for (int i = 0; i < skillManager.Passives.Count; i++)//Procura menor cooldown
                    if (skillManager.Passives[i].CooldownCurrent <= menorCooldown)
                    {
                        Debug.LogError(skillManager.Passives[i].Nome + " Passiva[" + i + "] Esta com o menor cooldown");
                        menorCooldown = skillManager.Passives[i].CooldownCurrent;
                        indexMenorCooldown = i;
                    }

                skillManager.Passives[indexMenorCooldown].Silence(time);

                BuffMobDbuff n = new BuffMobDbuff();
                n._value = indexMenorCooldown;
                n._time = time;
                n._id = instanceId;
                n._whoActive = who;

                _silenceList.Add(n);
            }

            if (index == -4)  //Skill Ativa
            {
                for (int i = 0; i < skillManager.Passives.Count; i++)//Procura Skill Ativa
                    if (skillManager.Passives[i].CooldownCurrent == 0)
                    {
                        skillManager.Passives[i].Silence(time);

                        BuffMobDbuff n = new BuffMobDbuff();
                        n._value = i;
                        n._time = time;
                        n._id = instanceId;
                        n._whoActive = who;

                        _silencePassiveList.Add(n);
                    }
            }
        }

        _silencePassive = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reset">Desativar Todas</param>
    public void CooldownSilencePassive(bool reset = false)
    {
        //Tamanho da lista for maior que 1
        if (_silencePassive)
        {
            if (reset)//Remove todos da lista
            {
                _silencePassiveList.Clear();

                for (int i = 0; i < skillManager.Passives.Count; i++)
                    skillManager.Passives[i].SilenceCooldown(reset);
            }
            else//Reset é false
            {
                bool remove = false;

                //Procura na lista Alguem a ser removido
                for (int i = 0; i < _silencePassiveList.Count; i++)
                {
                    _silencePassiveList[i]._time--;

                    Debug.LogError(name + " SilencePassiva [" + _silencePassiveList[i]._value + "] faltam: " + _silencePassiveList[i]._time + " ->" + _silencePassiveList[i]._id);

                    if (_silencePassiveList[i]._time <= 0)
                    {
                        skillManager.Passives[_silencePassiveList[i]._value].SilenceCooldown(true);

                        remove = true;

                        Debug.LogError("SilenceSkill da Passiva[" + _silencePassiveList[i]._value + "] acabou");
                    }
                    else
                        skillManager.Passives[_silencePassiveList[i]._value].SilenceCooldown();
                }

                //Remove Da lista
                if (remove)
                {
                    for (int i = 0; i < _silencePassiveList.Count; i++)
                        if (_silencePassiveList[i]._time <= 0)
                            _silencePassiveList.Remove(_silencePassiveList[i]);
                }
            }
        }

        //Atualiza SilenceSkill
        _silencePassive = _silencePassiveList.Count >= 1;

        if (!_silencePassive)
        {
            //Remove Effect
            effectManager.SilencePassiveReset(gameObject);
        }
    }

    /// <summary>
    /// Delegate Only
    /// </summary>
    public void CheckTimeSilencePassiveDelegate()
    {
        CooldownSilencePassive(false);
    }
    #endregion

    #region Buff Armor       
    /// <summary>
    /// Dá Armadura para o mob
    /// </summary>
    /// <param name="value">Valor a ser aumentado</param>
    /// <param name="time">Tempo de duração</param>
    /// <param name="who">Quem Ativou</param>
    /// <param name="instanceId">Id do buff para acumular valor</param>
    /// <param name="acumule">Pode Acumular o Valor do Buff Armor</param>
    /// <param name="maxAcumule">Pode Acumular no maximo X vezes o Buff Armor</param>
    public void AtiveBuffArmor(int value, int time, GameObject who, int instanceId = -1, bool acumule = false, int maxAcumule = -1)
    {
        if (skillManager == null)
        {
            Debug.LogError(name + " Não contém skillManager");
            return;
        }

        //Debug.LogError(name+" AtiveBuffDamage("+value+","+time+","+who+","+instanceId+")");

        //para contar o tempo certo
        if (time != -2)
            time++;

        bool temNalista = false;
        if (instanceId != -2 && acumule)//Atualizar Buff
        {
            foreach (var b in _buffArmorList)//Busca buff
            {
                if(b._id == instanceId)//Procurar id
                { 
                    if (acumule && maxAcumule > 1  && b._acumule >= maxAcumule)
                {
                    Debug.LogError(name + " Esta no seu Acumulo maximo do Buff Armor.");
                    return;
                }

                    int newValue =  value;

                    if (acumule)
                    {
                        newValue += b._value;

                        b._acumule++;
                    }
                    else
                        newValue = value > b._value ? value : b._value;

                    b._time = time;
                    b._value = newValue;
                    b._whoActive = who;

                    mobhealth.Armor(newValue,who);

                    temNalista = true;

                    break;
                }
            }
        }

        if (!temNalista)//Criar Buff na lista
        {
            //Debug.LogError("Dano Bonus do " + name + " Novo");

            BuffMobDbuff n = new BuffMobDbuff();

            n._value = value;
            n._time = time;
            n._id = instanceId;
            n._whoActive = who;

            _buffArmorList.Add(n);

            mobhealth.Armor(value, who);
        }

        _buffArmor = true;

        //
        effectManager.DbuffDamageEffect(gameObject);
    }

    /// <summary>
    /// Delegate Only
    /// </summary>
    public void CheckTimeBuffArmorDelegate()
    {
        // Debug.LogError("CheckTimeBuffDamageDelegate()");

        if (_buffArmor)//Buff esta Ativo
        {
            if (_buffArmorList.Count >= 1)//A alguem na lista
            {
                bool remove = false;

                foreach (var b in _buffArmorList)//Reduzir tempo
                {
                    if (b._time != -2)
                    {
                        b._time--;

                        Debug.LogError(name + " Armor Buff [" + b._value + "] faltam: " + b._time + " ->" + b._id);

                        if (b._time == 0)
                        {
                            b._time=0;

                            remove = true;

                            mobhealth.armor -= b._value;

                            int removeArmor = b._value;
                            //mobhealth.Armor(removeArmor, b._whoActive);

                            Debug.LogError("Armor Buff do " + name + " Acabou [removido ("+ removeArmor + ")] ->" + b._id);
                        }
                    }
                }
                if (remove)//Remover da lista
                {
                    for (int i = 0; i < _buffArmorList.Count; i++)
                        if (_buffArmorList[i]._time == 0)
                            _buffArmorList.Remove(_buffArmorList[i]);
                }
            }
        }

        //Atualizar valor do buff
        _buffArmor = _buffArmorList.Count >= 1;

        if (!_buffArmor)
        {
            //Remove Effect
            effectManager.DbuffDamageReset(gameObject);
        }
    }
    #endregion
}
