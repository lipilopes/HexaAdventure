using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Dbuff
{
    //Ao adicionar novo item na lista tem que cadastra-lo no XmlMenuInicial.Instance.DbuffTranslate
    Fire, Envenenar, Petrificar, Stun, Bleed, Recuar, Chamar,
    ///<summary>Min: Index/ Max: Valor</summary>
    Cooldown,
    Recupera_HP, Escudo,Buff_Atk,
    ///<summary>-2: Todas, -3 Menor cooldown, -4 Todas as skills disponiveis</summary>
    Silence, Dbuff_Atk,
    ///<summary>-2: Todas, -3 Menor cooldown, -4 Todas as Passivas disponiveis</summary>
    SilencePassive,
    Armadura
}

[System.Serializable]
public class DbuffBuff
{
    [Tooltip("$Buff + ElementNumber /SilenceSkill: [-2 Todas, -3 Menor cooldown, -4 Todas as skills disponiveis]")]
    public Dbuff _buff;

    [Space]
    public bool  _forMe = false;

    [Range(0,1), Tooltip("$% + ElementNumber")]
    public float _dbuffChance      = 0;

    [Space]
    [Tooltip("Valor minimo, em alguns casos esse pode ser o index ,$Min + ElementNumber ou $Dur + ElementNumber para aparecer o min - max")]
    public int   _dbuffDuracaoMin  = 0;
    [Tooltip("Valor maximo,em alguns casos esse pode ser o valor do Tempo,$Max + ElementNumber ou $Dur + ElementNumber para aparecer o min - max")]
    public int   _dbuffDuracaoMax  = 1;
    [Space]
    [Tooltip("Apenas Bonus e Escudo,Acumulativo $Ac + ElementNumber caso tenho maximo e isso seja verdadeiro ~print->(Acumula - x2)")]
    public bool _acumule = false;
    [Tooltip("Apenas Bonus e Escudo,Maximo que pode acumular $AcMax + ElementNumber")]
    public int _acumuleMax = 1;
}

public class MobSkillManager : MonoBehaviour
{
    protected MobManager     mobManager;
    protected MobCooldown    mobCooldown;
    protected MoveController moveController;
    protected EnemyAttack    enemyAttack;
    protected PassiveManager passive;   
    [SerializeField]
    protected float _XmlID=-1;
    [Space]
    [SerializeField]
    private Game_Mode _balanceMode = Game_Mode.History;
    public Game_Mode BalanceMode { get { return _balanceMode; } }
    [Space]
    [SerializeField, Tooltip("Skill Silenciada, não pode ser usada")]
    protected bool _silence = false;
    public bool SilenceSkill { get { return _silence; } }
    //Tempo de duração
    protected int _silenceTime = 0;
    public    int SilenceTime { get { return _silenceTime; } }

    /// <summary>
    /// Silencia a skill
    /// </summary>
    /// <param name="_time">tempo</param>
    public void Silence(int _time)
    {
        _silence = true;

        _silenceTime = _time;

        Debug.LogError("AtiveDbuffSilenceSkill(time:" + _time + ") " + Nome + " esta silenciada");
    }

    /// <summary>
    /// Cooldown do SilenceSkill
    /// </summary>
    /// <param name="_reset">Desativar caso esta ativo</param>
    public void SilenceCooldown(bool _reset = false)
    {
        _silenceTime--;

        if (_silenceTime == 0 || _reset)
        {
            _silence = false;

            _silenceTime = 0;
        }
    }

    [Space]
    [HideInInspector]
    public SkillManager skillManager;

    public GameObject User { set; get; }
    [Space]
    [SerializeField, Range(1, 3)]
    protected int _skill;
    public    int Skill { get { return _skill; } }

    [SerializeField]
    protected string nome;
    public    string Nome { get { return nome; } }
    [Space]
    [SerializeField]
    private Skill_Type _skillType;
    public  Skill_Type SkillType { get { return _skillType; } }
    [SerializeField]
    protected Sprite icon;
    public    Sprite Icon { get { return icon; } }
     
    [SerializeField, TextArea(5,15)]
    protected string description;
    public    string Description { get { return description; } set { description = value; } }

    [Space]
    [Header("Prefab Skill")]
    [SerializeField]
    protected GameObject  skillPrefab;
    protected SkillAttack skillAttack;
    public SkillAttack SkillAttack { get { return skillAttack; } }
    protected GameObject skillAttackOther;
    public    GameObject  SkillAttackFx
    {
        get
        {
            if (skillAttack != null)
                return skillAttack.Fx;
            else
                return User;
        }
    }
    [SerializeField]
    protected List<DbuffBuff> _dbuffBuff;
    public    List<DbuffBuff> _DbuffBuff { get { return _dbuffBuff; } }

    [Space]
    [Header("Setup")]
    [SerializeField, Range(0, 1),Tooltip("% de Chance de Skill Ser Usada")]
    protected float chanceUseSkill=1;
    /// <summary>
    /// Chance de Skill Sair
    /// </summary>
    public float ChanceUseSkill { get { return chanceUseSkill; } }
    protected bool UseSkillChecket = false;


    [SerializeField, Range(-1, 6)]
    protected int range;
    public    int Range { get { return range; } }

    [SerializeField, Range(0, 150)]
    protected int porcentDamage;
    public    int PorcentDamage { get { return porcentDamage; } }
    [SerializeField,Tooltip("Dano Base Somado a porcentagem")]
    protected int baseDamage;
    public    int BaseDamage { get { return baseDamage; } }
    [SerializeField, Tooltip("Divide o dano")]
    protected int dividedDamage = 1;
    public    int DividedDamage { get { return dividedDamage; } }
    [SerializeField, Tooltip("Skill da dano real")]
    protected bool realDamage = false;
    public    bool RealDamage { get { return realDamage; } }
    [Space]
    [SerializeField, Range(0, 100),Tooltip("Aumenta dano de acordo com o hp maximo")]
    protected float _maxHpPorcent = 0;
    public    int   MaxHpProcent{ get { return (int)User.GetComponent<MobHealth>().MaxHealthPorcent(_maxHpPorcent); } }
    public    float MaxHpProcentBase { get { return _maxHpPorcent; } }

    [Space]
    [SerializeField, Tooltip("Dano da skill ativa Passiva do inimigo")]
    protected bool _damageCountPassive = true;
    public    bool DamageCountPassive { get { return _damageCountPassive; } }
    [Space]
    [SerializeField, Tooltip("Skill Da Dano em Outros inimigos")]
    protected bool areaDamage;
    public    bool AreaDamage { get { return areaDamage; } }
    [SerializeField, Tooltip("Skill da dano real em area")]
    protected bool areaRealDamage = false;
    public    bool AreaRealDamage { get { return areaRealDamage; } }
    [SerializeField, Range(0, 150), Tooltip("% do dano da skill em outros inimigos")]
    protected int damageAreaDamage;
    public    int DamageAreaDamage { get { return damageAreaDamage; } }
    [SerializeField, Tooltip("Skill aplica dbuff da skill em Outros inimigos")]
    protected bool otherTargetDbuff;
    public    bool OtherTargetDbuff { get { return otherTargetDbuff; } }
    [Space]
    [SerializeField]
    protected int cooldownMax;
    public    int CooldownMax { get { return cooldownMax; } set { cooldownMax = value; } }
    //[SerializeField]
    protected int cooldownCurrent;
    public int CooldownCurrent { get { return cooldownCurrent; } set { cooldownCurrent = value; Debug.LogError("CurrentCooldown: "+value+" ["+Nome+"]"); } }
    [Space]
    [SerializeField]
    protected bool needTarget = true;
    public    bool NeedTarget { get { return needTarget; } }
    [SerializeField]
    protected bool targetFriend;
    public    bool TargetFriend { get { return targetFriend; } }
    [SerializeField]
    protected bool targetMe;
    public    bool TargetMe { get { return targetMe; } }
    [Space]
    //[SerializeField]
    protected int currentdamage;
    public    int CurrentDamage { get { return currentdamage; } }

    //[HideInInspector]
    public bool useSkill {
        set
        {
            _useSkill = value;
            enemyAttack.useSkill = value;                   
        }
        get
        {
            return _useSkill;
        }
    }

    public
    bool _useSkill = false;

    [Space]
    [SerializeField]
    protected bool autoCorrectCollider = false;

    //GetHexa
    protected List<HexManager> hexList = new List<HexManager>();
    public    List<HexManager> HexList
    {
        get
        {
            return hexList;
        }

        set
        {
            hexList = value;
        }
    }

    protected List<GameObject> alvosListSkill = new List<GameObject>();

    protected bool selectTouch = false;
    protected bool SelectTouch
    {
        get { return selectTouch; }
        set
        {
            selectTouch = value;

            if (value)
            {
                StopCoroutine(UpdateCoroutine());

                StartCoroutine(UpdateCoroutine());
            }
        }
    }

    protected GameObject objectSelectTouch;

    protected GameObject target;

    public    GameObject Target {get { return target; } set { target = value; } }

    [Space]
    [SerializeField,Tooltip("Range para o script SkillAttackRange, $SAR0")]
    protected int rangeSkillAttackRange = -1;

    public int RangeSkillAttackRange { get { return rangeSkillAttackRange; } }

    public bool CheckCompativelMode = false;

    #region SetUp
    protected virtual void Awake()
    {
        if (!CheckCompativelMode)//Deleta a skill que nao pertence ao modo
        {
            int count = 0;

            foreach (var item in GetComponents<MobSkillManager>())
                if (item.Nome == Nome)
                    count++;

            if (GameManagerScenes._gms.GameMode != _balanceMode
                && count > 1)
            {
                Debug.LogWarning(name + " Incompativel com o modo!!!");
                Destroy(this);
                return;
            }

            CheckCompativelMode = true;

            Debug.Log(name + " total de Skill's iguais " + count);            
        }

        StartCoroutine(TestAttDescription());
    }

    protected virtual void Start()
    {
        if (User != null)
        {          
            if (mobManager == null)
                mobManager = User.GetComponent<MobManager>();
            if (enemyAttack == null)
                enemyAttack = User.GetComponent<EnemyAttack>();
            if (mobCooldown == null)
                mobCooldown = User.GetComponent<MobCooldown>();
            if (moveController == null)
                moveController = User.GetComponent<MoveController>();               
        }

        if (passive == null)
            passive = GetComponent<PassiveManager>();
    }

    IEnumerator TestAttDescription()
    {
        yield return new WaitForSeconds(1);

        string test = ("$");
        if (Description.Contains(test))
            Debug.LogError("<color=red>" + _skill + " Skill [ID "+ _XmlID + "] (" + Nome + ") do " + mobManager.name + " Não atualizou Corretamente a Descrição da skill!!!</color>");
    }

    protected virtual bool CheckPlayerUseSkill()
    {
      //  enemyAttack.CheckDistance(Skill,false);

        if (!skillManager.SkillInUse /*|| !enemyAttack.ListTarget.Contains(target)*/)
        {
            Debug.LogError(nome+ " -(" + skillManager.SkillInUse + ") checkPlayerUseSkill -> return true");
            return true;
        }
        else
        {
            Debug.LogError(nome + " -("+skillManager.SkillInUse+") checkPlayerUseSkill -> return false");
            return false;
        }
    }

    protected virtual void AttDescription()
    {
        // GameManagerScenes gms = GameManagerScenes._gms;
        float id = _XmlID;
        if (mobManager != null)
            id += mobManager._SkinID;

        print(name+" XMLID:" + id);

        nome        = XmlMobSkill.Instance.Name(id);

        name = Nome + " - " + Skill + " [" + GameManagerScenes._gms.GameMode + "]";

        description = XmlMobSkill.Instance.Description(id);

        string user = ("$User");
        if (Description.Contains(user))
            AttDescription(user, User.GetComponent<ToolTipType>()._name);

        string sar = ("$SAR0");
        if (Description.Contains(sar))
            AttDescription(sar, "<b>"+ rangeSkillAttackRange+"</b>");

        int count = _DbuffBuff.Count;

        if (count <= 0)
            return;

        for (int i = 0; i < count; i++)
        {
            AttDescriptionDbuff(i);
        }
      }

    protected void AttDescriptionDbuff(int i)
    {
        if (i==-1)
            i = _dbuffBuff.Count - 1;

        if (i> _DbuffBuff.Count && i<-1)
            return;

        if (_DbuffBuff[i]._dbuffDuracaoMin == -1)
            _DbuffBuff[i]._dbuffDuracaoMin = currentdamage;

        if (_DbuffBuff[i]._dbuffDuracaoMax == -1)
            _DbuffBuff[i]._dbuffDuracaoMax = currentdamage;

        string buff = ("$Buff" + i);
        if (Description.Contains(buff))
        {
            AttDescription(buff, "<b>" + XmlMenuInicial.Instance.DbuffTranslate(_DbuffBuff[i]._buff) + "</b>");

            if (Description.Contains("_"))
                AttDescription("_", " $Dur" + i + " de ");
        }

        string chance = ("$%" + i);
        if (Description.Contains(chance))
            AttDescription(chance, "<b>" + (_DbuffBuff[i]._dbuffChance * 100).ToString() + "%</b>");

        string min = ("$Min" + i);
        if (Description.Contains(min))
        {
            if (_DbuffBuff[i]._buff == Dbuff.Silence ||
                _DbuffBuff[i]._buff == Dbuff.SilencePassive)
            {
                switch (_DbuffBuff[i]._dbuffDuracaoMin)
                {
                    case -2:
                        AttDescription(min, "<b>Todas as " + (_DbuffBuff[i]._buff == Dbuff.Silence ? XmlMenuInicial.Instance.Get(214)/*Skills*/+ "(s)" : XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "(s)") + " </b>");
                        break;

                    case -3:
                        AttDescription(min, "<b>" + (_DbuffBuff[i]._buff == Dbuff.Silence ? XmlMenuInicial.Instance.Get(214)/*Skills*/+ "(s)" : XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "(s)") + " com menor Recarga</b>");
                        break;

                    case -4:
                        AttDescription(min, "<b>" + (_DbuffBuff[i]._buff == Dbuff.Silence ? XmlMenuInicial.Instance.Get(214)/*Skills*/+ "(s)" : XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "(s)") + " Disponiveis</b>");
                        break;
                }

            }
            else
            {
                AttDescription(min, "<b>" + _DbuffBuff[i]._dbuffDuracaoMin + "</b>");
            }
        }

        string max = ("$Max" + i);
        if (Description.Contains(max))
            AttDescription(max, "<b>" + _DbuffBuff[i]._dbuffDuracaoMax + "</b>");

        string dur = ("$Dur" + i);
        if (Description.Contains(dur))
        {
            string Dur =
                (_DbuffBuff[i]._dbuffDuracaoMin == _DbuffBuff[i]._dbuffDuracaoMax)
               ? "<b>" + _DbuffBuff[i]._dbuffDuracaoMax + "</b>"
               : "<b>" + _DbuffBuff[i]._dbuffDuracaoMin + " - " + _DbuffBuff[i]._dbuffDuracaoMax + "</b>";

            AttDescription(dur, Dur);
        }

        string acumule = ("$Ac" + i);
        if (Description.Contains(acumule))
            AttDescription(acumule, _DbuffBuff[i]._acumule
                ? "(<color=blue><b>" + XmlMenuInicial.Instance.Get(193) + "</b></color>" +//Acumula
                    (_DbuffBuff[i]._acumuleMax > 1 ? " - x" + _DbuffBuff[i]._acumuleMax + ")" : "")
                : ""//"<color=red><b>"  + XmlMenuInicial.Instance.Get(194) + "</b></color>"//Não acumula
                );

    }

    protected void AttDescription(string key, string _new)
    {
        GameManagerScenes gms = GameManagerScenes._gms;

        description = gms.AttDescrição(Description, key, _new, Description);
    }

    protected virtual void AlvosNaListSkill(int _skill = -1, bool clear = true, bool removeTargetSkill = false)
    {
        if (clear)
            alvosListSkill.Clear();

        int _R = Range;

        if (_skill == -1)
            _skill = Skill;

        if (_skill >= 1 && _skill <= 3)
            _R = skillManager.Skills[_skill - 1].Range;
        else
        {
            _R = skillManager.CheckMoreDistanceSkill();

            _skill = Skill;
        }

        List<HexManager> hexAlvosNaListSkill = new List<HexManager>();

        hexAlvosNaListSkill = enemyAttack.RegisterOtherHex(moveController.hexagonX, moveController.hexagonY, _R,false,0);

        foreach (var i in hexAlvosNaListSkill)
        {
            GameObject I = i.currentMob;

            if (I != null && !alvosListSkill.Contains(I) && I.GetComponent<MobManager>())
            {
                MobManager M = I.GetComponent<MobManager>();

                if (M.Alive)
                {
                    if (M.MesmoTime(User)                        && skillManager.Skills[_skill - 1].targetFriend ||
                        !M.MesmoTime(User)                       && skillManager.Skills[_skill - 1].NeedTarget ||
                        skillManager.Skills[_skill - 1].TargetMe && I == User)
                    {
                        alvosListSkill.Add(I);
                    }
                }            
            }
        }

        /* 
         if (_skill == -1)
            _skill = Skill;
         if (_skill == 1)
             alvosListSkill = enemyAttack.ListTargetSkill1;
         if (_skill == 2)
             alvosListSkill = enemyAttack.ListTargetSkill2;
         if (_skill == 3)
             alvosListSkill = enemyAttack.ListTargetSkill3;
         if (_skill < -1 && _skill > 3)
             alvosListSkill = enemyAttack.ListTarget;
             */

         if (removeTargetSkill && alvosListSkill.Contains(enemyAttack.target))
             alvosListSkill.Remove(enemyAttack.target);

        if (GameManagerScenes._gms.Adm)
        {
            InfoTable.Instance.NewInfo(Nome + " tem alvos na lista " + alvosListSkill.Count, 3);
        }
    }

    public virtual void AttDamageAndDescription()
    {
        Debug.LogError("Dano da skill " + nome + " Atualizado(a)");

        currentdamage = skillManager.CalculePorcent(mobManager.damage, porcentDamage, dividedDamage, baseDamage, MaxHpProcent);

        AttDescription();

        if (skillAttack != null)
        {
            skillAttack.damage             = currentdamage;
            skillAttack.who                = User;
            skillAttack.AreaDamage         = AreaDamage;
            skillAttack.DamageAreaDamage   = DamageAreaDamage;
            skillAttack.OtherTargetDbuff   = OtherTargetDbuff;
            skillAttack.SkillManager       = this;
            skillAttack.skill              = Skill;
            skillAttack.TakeRealDamage     = RealDamage;
            skillAttack.AreaRealDamage     = AreaRealDamage;
            skillAttack.DamageCountPassive = DamageCountPassive;

            if (skillAttack.GetComponent<SkillAttackRange>())
                skillAttack.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRange;
        }

        if (GetComponent<ToolTipType>() != null)
            GetComponent<ToolTipType>().AttToltip();
    }
    #endregion   

    #region Create Skill
    public virtual void AttDamage()
    {
        if (mobManager == null)
            mobManager = User.GetComponent<MobManager>();
        if (enemyAttack == null)
            enemyAttack = User.GetComponent<EnemyAttack>();
        if (mobCooldown == null)
            mobCooldown = User.GetComponent<MobCooldown>();
        if (moveController == null)
            moveController = User.GetComponent<MoveController>();

        currentdamage = /*BaseDamage + */skillManager.CalculePorcent(mobManager.damage, porcentDamage, dividedDamage,baseDamage,MaxHpProcent);

        //Skin
        if (mobManager != null)
        {
            int _skillSkin = Skill - 1;

            //Debug.LogWarning("Tem mobManager " + _skillSkin + " < " + mobManager._PrefabSkillsSkin.Count);

            if (_skillSkin < mobManager._PrefabSkillsSkin.Count)
            {
               // Debug.LogWarning("Tem mobManager " + mobManager._PrefabSkillsSkin[_skillSkin].name);

                if (mobManager._PrefabSkillsSkin[_skillSkin] != null)
                {
                  //  Debug.LogWarning("Tem mobManager Feito ("+_skillSkin + " - " + mobManager._PrefabSkillsSkin[_skillSkin].name + ")");
                    skillPrefab = mobManager._PrefabSkillsSkin[_skillSkin];
                }
            }
        }

        CreateSkills();
    }

    protected virtual void CreateSkills()
    {
        AttDescription();

        if (skillAttack == null && skillPrefab != null)
        {
            GameObject _skill = Instantiate(skillPrefab, new Vector3(0, 0.057f, 0), new Quaternion());
            skillAttackOther = _skill;
            skillAttack = _skill.GetComponent<SkillAttack>();

            if (skillAttack != null)
            {                
                if (autoCorrectCollider)
                {
                    if (skillAttack.GetComponent<BoxCollider>())
                    {
                        float valueSizeCollider = (Range * Range) + Range;
                        Vector3 sizeCollider = new Vector3(valueSizeCollider, 2, valueSizeCollider);
                        skillAttack.GetComponent<BoxCollider>().size = sizeCollider;

                        Debug.LogError("Ajustado BoxCollider[" + skillPrefab.name + "] para " + sizeCollider);
                    }
                    else
                     if (skillAttack.GetComponent<CapsuleCollider>())
                    {
                        float valueSizeCollider = (Range / 10) + 0.05f;

                        skillAttack.GetComponent<CapsuleCollider>().radius = valueSizeCollider;

                        Debug.LogError("Ajustado CapsuleCollider[" + skillPrefab.name + "] para " + valueSizeCollider);
                    }
                    else
                     if (skillAttack.GetComponent<SphereCollider>())
                    {
                        float valueSizeCollider = (Range / 10) + 0.05f;

                        skillAttack.GetComponent<SphereCollider>().radius = valueSizeCollider;

                        Debug.LogError("Ajustado SphereCollider[" + skillPrefab.name + "] para " + valueSizeCollider);
                    }
                }

                skillAttack.damage = currentdamage;
                skillAttack.who = User;                
                skillAttack.AreaDamage = AreaDamage;               
                skillAttack.DamageAreaDamage = DamageAreaDamage;
                skillAttack.OtherTargetDbuff = OtherTargetDbuff;
                skillAttack.SkillManager = this;
                skillAttack.skill = Skill;
                skillAttack.TakeRealDamage = RealDamage;
                skillAttack.AreaRealDamage = AreaRealDamage;
                skillAttack.DamageCountPassive = DamageCountPassive;

                if (skillAttack.GetComponent<SkillAttackRange>())
                    skillAttack.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRange;

                skillAttack.gameObject.SetActive(false);
            }

            _skill.name = Nome + " - " + User.name;
            //_skill.transform.SetParent(transform);
            _skill.transform.position = User.transform.position;
            _skill.gameObject.SetActive(false);

            AttMobAttack();

            if (RespawMob.Instance)
                RespawMob.Instance.allRespaws.Add(_skill);
        }
        else
        {
            if (skillAttack != null)
            {
                skillAttack.damage = currentdamage;
                skillAttack.who = User;
                skillAttack.AreaDamage = AreaDamage;
                skillAttack.DamageAreaDamage = DamageAreaDamage;
                skillAttack.OtherTargetDbuff = OtherTargetDbuff;
                skillAttack.SkillManager = this;
                skillAttack.skill = Skill;
                skillAttack.TakeRealDamage = RealDamage;
                skillAttack.AreaRealDamage = AreaRealDamage;
                skillAttack.DamageCountPassive = DamageCountPassive;

                if (skillAttack.GetComponent<SkillAttackRange>())
                    skillAttack.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRange;
            }

            AttMobAttack();
        }

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

    }

    void AttMobAttack()
    {
      /*  MobAttack mobAttack = User.GetComponent<MobAttack>();

        if (Skill == 1)
        {
            mobAttack.nameSkill1 = Nome;
            mobAttack.distanceSkill1 = Range;
            mobAttack.damageSkill1 = PorcentDamage;
            mobAttack.MaxCooldownSkill1 = CooldownMax;

            mobAttack.Skill1NeedTarget = NeedTarget;
            mobAttack.Skill1TargetFriend = TargetFriend;
            mobAttack.Skill1TargetMe = TargetMe;
        }
        if (Skill == 2)
        {
            mobAttack.nameSkill2 = Nome;
            mobAttack.distanceSkill2 = Range;
            mobAttack.damageSkill2 = PorcentDamage;
            mobAttack.MaxCooldownSkill2 = CooldownMax;

            mobAttack.Skill2NeedTarget = NeedTarget;
            mobAttack.Skill2TargetFriend = TargetFriend;
            mobAttack.Skill2TargetMe = TargetMe;
        }
        if (Skill == 3)
        {
            mobAttack.nameSkill3 = Nome;
            mobAttack.distanceSkill3 = Range;
            mobAttack.damageSkill3 = PorcentDamage;
            mobAttack.MaxCooldownSkill3 = CooldownMax;

            mobAttack.Skill3NeedTarget = NeedTarget;
            mobAttack.Skill3TargetFriend = TargetFriend;
            mobAttack.Skill3TargetMe = TargetMe;
        }*/
    }

    #endregion   

    #region Main
    public virtual void UseSkill()
    {
        if (SilenceSkill)       
            return;

        if (!UseSkillChecket && !CheckUseSkill())
            return;

        this.gameObject.transform.position = new Vector3(User.transform.position.x, User.transform.position.y, User.transform.position.z);

        if(skillAttackOther!=null)
        skillAttackOther.transform.position = transform.position;

        target = enemyAttack.target;

        if (mobManager.isPlayer)
            if (!CheckPlayerUseSkill())
            {
                /* if (!mobManager.isPlayer)
                     enemyAttack.CheckInList();*/

                ButtonManager.Instance.SkillInUseCanceled();
                return;
            }       

        useSkill = true;

        mobManager.ActivePassive(Passive.StartSkill, target);

        alvosListSkill.Clear();

        HexList.Clear();

        mobManager.currentTimeAttack--;

        objectSelectTouch = null;    
       
        if (target == null)
            target = User;

        Debug.LogError(User + " usou a skill " + Nome+" no(a) "+target.name);

        if (skillAttack != null)
        {
            skillAttack.transform.position = Vector3.zero;

            Debug.LogError("Prefab -> " + skillAttack.name);
        }       
    }

    protected virtual void ShootSkill()
    {
        this.gameObject.transform.position = new Vector3(User.transform.position.x, User.transform.position.y, User.transform.position.z);
        if (skillAttackOther != null)
            skillAttackOther.transform.position = transform.position;

        Debug.LogError(Nome + " ShotSkill("+target+")");

        if (GameManagerScenes._gms.Adm)
        {
            EffectManager.Instance.PopUpDamageEffect(mobManager.MesmoTime(RespawMob.Instance.PlayerTime) ? "<color=#055b05>" + Nome + "</color>" : "<color=#962209>" + Nome + "</color>", User);
        }

        mobManager.ActivePassive(Passive.ShootSkill, target);

        if (target != null && User != target)
            User.transform.LookAt(target.transform);       

        if (skillAttack != null)
        {
            skillAttack.gameObject.transform.position = User.transform.position;
            skillAttack.gameObject.transform.rotation = User.transform.rotation;

            //if (target != null)
            //    skillAttack.gameObject.transform.LookAt(target.transform);

            skillAttack.UseSkill(target, this);
        }
        else
        {
            target.GetComponent<MobHealth>().Damage(User, CurrentDamage, mobManager.chanceCritical);
            Hit(true, Target);
        }      
    }

    /// <summary>
    /// Procura Target no raio da skill com menor vida
    /// </summary>
    public virtual void ShootSkillAndSeachTarget()
    {
        RegisterOtherHex(moveController.hexagonX, moveController.hexagonY, Range, false,_clearList: true);

        float menorHp = -1;
        GameObject _target = null,
                   _targetCheck;

        foreach (var H in HexList)
        {
            _targetCheck = H.currentMob;

            if (_targetCheck != null
                && _targetCheck.GetComponent<MobHealth>()
                && _targetCheck.GetComponent<MobManager>() && !_targetCheck.GetComponent<MobManager>().MesmoTime(User))
            {
                if (menorHp == -1 || _targetCheck.GetComponent<MobHealth>().Health < menorHp)
                {
                    Debug.LogError("ShootSkillAndSeachTarget -> alvo("+_targetCheck.name+" / "+ _targetCheck.GetComponent<MobHealth>().Health + "<"+menorHp+")");
                    _target = _targetCheck;
                    menorHp = _targetCheck.GetComponent<MobHealth>().Health;
                }
            }
        }

        if (_target==null)
        {
            Debug.LogError("ShootSkillAndSeachTarget -> Não encontrou nenhum alvo");
            return;
        }
        target = _target;

        Debug.LogError(Nome + " ShotSkill(" + target + ")");

        if (GameManagerScenes._gms.Adm)
        {
            EffectManager.Instance.PopUpDamageEffect(mobManager.MesmoTime(RespawMob.Instance.PlayerTime) ? "<color=#055b05>" + Nome + "</color>" : "<color=#962209>" + Nome + "</color>", User);
        }

        mobManager.ActivePassive(Passive.ShootSkill, target);

        if (target != null && User != target)
            User.transform.LookAt(target.transform);

        if (skillAttack != null)
        {
            skillAttack.gameObject.transform.position = User.transform.position;

            if (target != null)
                skillAttack.gameObject.transform.LookAt(target.transform);

            skillAttack.UseSkill(target, this);
        }
        else
        {
            target.GetComponent<MobHealth>().Damage(User, CurrentDamage, mobManager.chanceCritical);
            Hit(true, Target);
        }
    }

    public virtual void Hit(bool endTurn, GameObject targetDbuff)
    {
        int count = _DbuffBuff.Count;
       
        Debug.LogError(name + " HIT(" + endTurn + "," + targetDbuff + ")" + count);

       // if (GameManagerScenes._gms)
       //     GameManagerScenes._gms.NewInfo(name + " HIT(" + endTurn + "," + targetDbuff + ")" + count, 4, true);

        if (Target.GetComponent<MobManager>())
            Target.GetComponent<MobManager>().ActivePassive(Passive.EnimyHitSkill, User);

        if (targetDbuff == Target)        
            mobManager.ActivePassive(Passive.TargetHitSkill, targetDbuff);

        mobManager.ActivePassive(Passive.HitSkill, targetDbuff);

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                CheckDbuff(targetDbuff, i);
            }
        }

        if (endTurn)
            EndSkill();
    }

    protected void ResetCoolDownManager()
    {
        Debug.LogError("ResetCoolDownManager (" + Nome + ")");

        if (useSkill)
            useSkill = false;

        CooldownCurrent = CooldownMax + 1;

        mobCooldown.AttCooldown(CooldownCurrent, Skill-1);
    }

    protected virtual void EndSkill()
    {
        Debug.LogError("EndSkill()");

        mobManager.ActivePassive(Passive.EndSkill, Target);      

        ResetCoolDownManager();       

        UseSkillChecket = false;

        useSkill = false;

        // mobManager.EndTurn();
        mobManager.EndAttackTurn();
    }
    #endregion

    #region Use Mouse or Touch
    protected IEnumerator UpdateCoroutine()
    {
        if (mobManager.isPlayer)
        {
            Debug.LogError("UpdateCoroutine()");

            while (SelectTouch && mobManager.myTurn)
            {
                ClickTarget();
                yield return null;
            }
        }

        yield return null;
    }

    protected virtual void UseTouchSkill()
    {
        Debug.LogError("UseTouchSkill() - "+User.name);

        if (mobManager.isPlayer)
            ButtonManager.Instance.ClickAudio(true);
    }

    protected void ClickTarget()
    {
        if (GameManagerScenes._gms.Paused == true)
            return;

        Ray ray;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        else
        if (Input.GetMouseButton(0)/* || Input.GetMouseButton(1)*/)
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        else
            return;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.LogError("Player["+User+"] Use Touch!!!");

                if (RulesClickTarget(hit.collider.transform.gameObject))
                {
                    UseTouchSkill();
                }
                else
                    RulesClickTargetElse();
            }
        }

    }

    protected virtual bool RulesClickTarget(GameObject hitObject)
    {
        HexManager ground = null;      

        #region Caso Click no Hexagono                             
        #region ITEM
        if (hitObject.GetComponent<HexManager>() != null)
            if (hitObject.GetComponent<HexManager>().currentItem != null)
                if (hitObject.GetComponent<HexManager>().currentItem.GetComponent<PortalManager>() == null)
                    ground = hitObject.GetComponent<HexManager>();
        #endregion

        #region ITEM RC HP
        if (hitObject.GetComponent<ItemRecHp>() != null)
            if (hitObject.GetComponent<ItemRecHp>().Here != null)
                if (hitObject.GetComponent<ItemRecHp>().Here.GetComponent<HexManager>().currentItem != null)
                    ground = hitObject.GetComponent<ItemRecHp>().Here.GetComponent<HexManager>();
        #endregion

        #region ground
        if (hitObject.GetComponent<HexManager>() != null)
            if (hitObject.GetComponent<HexManager>().free)
                ground = hitObject.GetComponent<HexManager>();
        #endregion

        #region Mob
        if (hitObject.GetComponent<MoveController>() != null)
            if (hitObject.GetComponent<MoveController>().Solo != null)
                ground = hitObject.GetComponent<MoveController>().Solo;
        #endregion
        #endregion

        if (!HexList.Contains(ground))
        {
            for (int i = 0; i < HexList.Count; i++)
                CheckGrid.Instance.ColorGrid(3, HexList[i].x, HexList[i].y);
            return false;
        }

        objectSelectTouch = ground.gameObject;

        EffectManager.Instance.TargetEffect(ground.gameObject);
        EffectManager.Instance.TargetTargeteado(ground.gameObject);

        CheckGrid.Instance.ColorGrid(0, 0, 0, clear: true);
        return true;
    }

    protected virtual void RulesClickTargetElse()
    {
        if (mobManager.isPlayer)
        ButtonManager.Instance.ClickAudio(false);       
    }
    #endregion

    #region Other
    /// <summary>
    /// Use By Create Area Damage
    /// </summary>
    public virtual void EndTurnAttack()
    {
       // GameManagerScenes._gms.NewInfo("EndTurnAttack - " + Nome, 3, true);
    }

    public virtual void DesactiveTurnAttack()
    {

    }
    #endregion

    #region Dbuff
    /// <summary>
    /// Aplica Dbuff
    /// </summary>
    /// <param name="targetDbuff">Alvo do Dbuff</param>
    /// <param name="Buff">Buff ou Dbuff a ser Aplicado</param>
    /// <param name="forMe">é para mim</param>
    /// <param name="Chance">% de chance para ativar</param>
    /// <param name="MinDuration">Duração minima [pode ser index em alguns casos]</param>
    /// <param name="MaxDuration">Duração maxima [pode ser o valor caso o min seja index]</param>
    /// <param name="Acumule">Acumulativo (Apenas Escudo e Bonus)</param>
    /// <param name="indexList">index na Lista Dbuff</param>
    protected bool ApplyDbuff(GameObject targetDbuff, Dbuff Buff, bool forMe, float Chance, int MinDuration = -1, int MaxDuration = -1, bool Acumule = false, int MaxAcumule = 1, int indexList = -1)
    {
        bool _r = GameManagerScenes._gms.Dbuff(User, currentdamage, targetDbuff, Buff, forMe, Chance, MinDuration, MaxDuration, Acumule, MaxAcumule, indexList,GetInstanceID());

        if (indexList!=-1)
        {
            if (_r)
                DbuffActive(targetDbuff, indexList);
            else
                DbuffFail(targetDbuff, indexList);
        }

        return _r;
        #region Old
        //if (Chance > 1)
        //    Chance /= 100;

        //if (MinDuration == -1)
        //    MinDuration = currentdamage;

        //if (MaxDuration == -1)
        //    MaxDuration = currentdamage;

        //int Duration = UnityEngine.Random.Range(MinDuration, MaxDuration + 1);
        //int i        = indexList;       

        //if (forMe)
        //    targetDbuff = User;

        //if (!forMe && User == targetDbuff)
        //    targetDbuff = null;

        //Debug.LogError("Check Hit[" + Buff + "] Skill(" + nome + ") -  Duration[" + Duration + "] / Chance[" + Chance * 100 + "%] no " + targetDbuff);

        //if (targetDbuff == null ||
        //    targetDbuff.GetComponent<MobHealth>() == null ||
        //    targetDbuff.GetComponent<MobDbuff>() == null ||
        //    targetDbuff.GetComponent<MoveController>() == null ||
        //    targetDbuff.GetComponent<MobCooldown>() == null ||
        //    targetDbuff.GetComponent<MobHealth>() == null)
        //{
        //    DbuffFail(targetDbuff, i);
        //    return false;
        //}

        //switch (Buff)
        //{
        //    #region Fire
        //    case Dbuff.Fire:
        //        if (targetDbuff.GetComponent<MobDbuff>() != null)
        //            if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffFire(User, duration: Duration, _chance: Chance))
        //            {
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //                DbuffFail(targetDbuff, i);
        //        break;
        //    #endregion

        //    #region Poison
        //    case Dbuff.Envenenar:
        //        if (targetDbuff.GetComponent<MobDbuff>() != null)
        //            if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPoison(User, duration: Duration, _chance: Chance))
        //            {
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //                DbuffFail(targetDbuff, i);
        //        break;
        //    #endregion

        //    #region Petrify
        //    case Dbuff.Petrificar:
        //        if (targetDbuff.GetComponent<MobDbuff>() != null)
        //            if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPetrify(User, duration: Duration, _chance: Chance))
        //            {
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //                DbuffFail(targetDbuff, i);
        //        break;
        //    #endregion

        //    #region Stun
        //    case Dbuff.Stun:
        //        if (targetDbuff.GetComponent<MobDbuff>() != null)
        //            if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffStun(User, duration: Duration, _chance: Chance))
        //            {
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //                DbuffFail(targetDbuff, i);
        //        break;
        //    #endregion

        //    #region Bleed
        //    case Dbuff.Bleed:
        //        if (targetDbuff.GetComponent<MobDbuff>() != null)
        //            if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffBleed(User, duration: Duration, _chance: Chance))
        //            {
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //                DbuffFail(targetDbuff, i);
        //        break;
        //    #endregion

        //    #region Recuar
        //    case Dbuff.Recuar:
        //        if (targetDbuff.GetComponent<MoveController>())
        //        {
        //            bool _RecuarAtivou = false;
        //            for (int buff = 0; buff < Duration; buff++)
        //            {
        //                if (CheckChance(Chance))
        //                {
        //                    //Debug.LogError(User.name + " Fez o " + targetDbuff.name + " Recuar");
        //                    targetDbuff.GetComponent<MoveController>().EnemyWalk(User.GetComponent<MoveController>(), true, true, Call: true);
        //                    DbuffActive(targetDbuff, i);
        //                    _RecuarAtivou = true;
        //                }
        //                else
        //                {
        //                    //Debug.LogError(User.name + " Falhou em Recuar o " + targetDbuff.name);
        //                    DbuffFail(targetDbuff, i);
        //                }
        //            }
        //            return _RecuarAtivou;
        //        }
        //        break;
        //    #endregion

        //    #region chamar
        //    case Dbuff.Chamar:
        //        if (targetDbuff.GetComponent<MoveController>())
        //        {
        //            bool _ChamarAtivou = false;

        //            for (int buff = 0; buff < Duration; buff++)
        //            {
        //                if (CheckChance(Chance))
        //                {
        //                    _ChamarAtivou = true;
        //                    // Debug.LogError(User.name + " Chamou o " + targetDbuff.name);
        //                    targetDbuff.GetComponent<MoveController>().EnemyWalk(User.GetComponent<MoveController>(), true, Call: true);
        //                    DbuffActive(targetDbuff, i);
        //                }
        //                else
        //                {
        //                    //Debug.LogError(User.name + " Falhou em Chamar o " + targetDbuff.name);
        //                    DbuffFail(targetDbuff, i);
        //                }
        //            }

        //            return _ChamarAtivou;
        //        }
        //        break;
        //    #endregion

        //    #region cooldown
        //    case Dbuff.Cooldown:
        //        if (targetDbuff.GetComponent<MobCooldown>() && targetDbuff.GetComponent<SkillManager>())
        //        {
        //            if (targetDbuff.GetComponent<SkillManager>().Skills.Count >= MinDuration
        //                && CheckChance(Chance))
        //            {
        //                // Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
        //                targetDbuff.GetComponent<MobCooldown>().AttCooldown(MaxDuration, MinDuration);
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //            {
        //                // Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
        //                DbuffFail(targetDbuff, i);
        //            }
        //        }
        //        break;
        //    #endregion

        //    #region Recupera Hp
        //    case Dbuff.Recupera_HP:
        //        if (targetDbuff.GetComponent<MobHealth>())
        //        {
        //            if (CheckChance(Chance) && targetDbuff.GetComponent<MobHealth>().RecHp(User, Duration))
        //            {
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //                DbuffFail(targetDbuff, i);
        //        }
        //        break;
        //    #endregion

        //    #region Escudo
        //    case Dbuff.Escudo:
        //        if (targetDbuff.GetComponent<MobHealth>())
        //        {
        //            if (CheckChance(Chance))
        //            {
        //                if (targetDbuff.GetComponent<MobHealth>().defense < Duration || Acumule)
        //                {
        //                    //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
        //                    targetDbuff.GetComponent<MobHealth>().Defense
        //                        (Acumule
        //                        ? targetDbuff.GetComponent<MobHealth>().defense + Duration
        //                        : Duration, User);
        //                    DbuffActive(targetDbuff, i);
        //                    return true;
        //                }
        //            }

        //            //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
        //            DbuffFail(targetDbuff, i);
        //        }
        //        break;
        //    #endregion

        //    #region Bonus Atk
        //    case Dbuff.Buff_Atk:
        //        if (targetDbuff.GetComponent<MobDbuff>())
        //        {
        //            if (CheckChance(Chance))
        //            {
        //                //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
        //                targetDbuff.GetComponent<MobDbuff>().AtiveBuffDamage
        //                    (
        //                    MinDuration, //valor
        //                    MaxDuration, //tempo,
        //                    User,
        //                    /*Acumule ?*/ this.GetInstanceID() /*: -1*/,
        //                    Acumule,//Acumulativo
        //                    MaxAcumule);
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //            {
        //                //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
        //                DbuffFail(targetDbuff, i);
        //            }
        //        }
        //        break;
        //    #endregion

        //    #region Silence
        //    case Dbuff.Silence:
        //        if (targetDbuff.GetComponent<MobDbuff>())
        //        {
        //            if (CheckChance(Chance) /*&& targetDbuff.GetComponent<SkillManager>().Skills.Count >= MinDuration*/)
        //            {
        //                // Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
        //                targetDbuff.GetComponent<MobDbuff>().AtiveDbuffSilenceSkill(
        //                    MaxDuration, //time
        //                    MinDuration, //index
        //                    User,
        //                    /*Acumule ?*/ this.GetInstanceID() /*: -1*/,
        //                    Acumule,//Acumulativo
        //                    MaxAcumule);
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //            {
        //                // Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
        //                DbuffFail(targetDbuff, i);
        //            }
        //        }
        //        break;
        //    #endregion

        //    #region DbuffAtk
        //    case Dbuff.Dbuff_Atk:
        //        if (targetDbuff.GetComponent<MobDbuff>())
        //        {
        //            if (CheckChance(Chance))
        //            {
        //                //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
        //                targetDbuff.GetComponent<MobDbuff>().AtiveDBuffDamage
        //                    (
        //                    MinDuration, //valor
        //                    MaxDuration, //tempo,
        //                    User,
        //                    /*Acumule ?*/ this.GetInstanceID() /*: -1*/,
        //                    Acumule,//Acumulativo
        //                    MaxAcumule);
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //            {
        //                //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
        //                DbuffFail(targetDbuff, i);
        //            }
        //        }
        //        break;
        //    #endregion

        //    #region Silence Passive
        //    case Dbuff.SilencePassive:
        //        if (targetDbuff.GetComponent<MobDbuff>())
        //        {
        //            if (CheckChance(Chance) /*&& targetDbuff.GetComponent<SkillManager>().Skills.Count >= MinDuration*/)
        //            {
        //                // Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
        //                targetDbuff.GetComponent<MobDbuff>().AtiveDbuffSilencePassive(
        //                    MaxDuration, //time
        //                    MinDuration, //index
        //                    User,
        //                    /*Acumule ?*/ this.GetInstanceID() /*: -1*/,
        //                    Acumule,//Acumulativo
        //                    MaxAcumule);
        //                DbuffActive(targetDbuff, i);
        //                return true;
        //            }
        //            else
        //            {
        //                // Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
        //                DbuffFail(targetDbuff, i);
        //            }
        //        }
        //        break;
        //        #endregion
        //}
        //return false;
        #endregion

    }

    /// <summary>
    /// Ativa Dbuff Da Lista
    /// </summary>
    /// <param name="targetDbuff">Alvo do Dbuff</param>
    /// <param name="i">Index na lista</param>
    protected void CheckDbuff(GameObject targetDbuff, int i = -1)
    {
        ApplyDbuff(targetDbuff
            ,_DbuffBuff[i]._buff
            , _DbuffBuff[i]._forMe
            , _DbuffBuff[i]._dbuffChance
            , _DbuffBuff[i]._dbuffDuracaoMin
            , _DbuffBuff[i]._dbuffDuracaoMax
            , _DbuffBuff[i]._acumule
            , _DbuffBuff[i]._acumuleMax
            , i);

        #region Old
        /*
        if (_DbuffBuff[i]._dbuffDuracaoMin == -1)
            _DbuffBuff[i]._dbuffDuracaoMin = currentdamage;

        if (_DbuffBuff[i]._dbuffDuracaoMax == -1)
            _DbuffBuff[i]._dbuffDuracaoMax = currentdamage;

        int Duration = UnityEngine.Random.Range(_DbuffBuff[i]._dbuffDuracaoMin, _DbuffBuff[i]._dbuffDuracaoMax + 1);
        float Chance = _DbuffBuff[i]._dbuffChance;
        bool Acumule = _DbuffBuff[i]._acumule;

        if (_DbuffBuff[i]._forMe)
        {
            targetDbuff = User;
        }

        //if (!_DbuffBuff[i]._forMe && User == targetDbuff)
        //    targetDbuff = null;

        Debug.LogError("Check Hit[" + _DbuffBuff[i]._buff + "( " + (i) + " )] Skill(" + nome + ") -  Duration[" + Duration + "] / Chance[" + Chance * 100 + "%] no " + targetDbuff);

        if (GameManagerScenes._gms)
            GameManagerScenes._gms.NewInfo(name+" Check Hit[" + _DbuffBuff[i]._buff + "( " + (i) + " )] Skill(" + nome + ") -  Duration[" + Duration + "] / Chance[" + Chance * 100 + "%] no " + targetDbuff, 4, true);


        #region Buffs
        if (targetDbuff != null)
        {
            switch (_DbuffBuff[i]._buff)
            {
                #region Fire
                case Dbuff.Fire:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffFire(User, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Poison
                case Dbuff.Envenenar:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPoison(User, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Petrify
                case Dbuff.Petrificar:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPetrify(User, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Stun
                case Dbuff.Stun:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffStun(User, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Bleed
                case Dbuff.Bleed:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffBleed(User, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Recuar
                case Dbuff.Recuar:
                    if (targetDbuff.GetComponent<MoveController>())
                    {
                        for (int buff = 0; buff < Duration; buff++)
                        {
                            if (CheckChance(Chance))
                            {
                                //Debug.LogError(User.name + " Fez o " + targetDbuff.name + " Recuar");
                                targetDbuff.GetComponent<MoveController>().EnemyWalk(User.GetComponent<MoveController>(), true, true, Call: true);
                                DbuffActive(targetDbuff, i); ;
                            }
                            else
                            {
                                //Debug.LogError(User.name + " Falhou em Recuar o " + targetDbuff.name);
                                DbuffFail(targetDbuff, i);
                            }
                        }
                    }
                    break;
                #endregion

                #region chamar
                case Dbuff.Chamar:
                    if (targetDbuff.GetComponent<MoveController>())
                    {
                        for (int buff = 0; buff < Duration; buff++)
                        {
                            if (CheckChance(Chance))
                            {
                                // Debug.LogError(User.name + " Chamou o " + targetDbuff.name);
                                targetDbuff.GetComponent<MoveController>().EnemyWalk(User.GetComponent<MoveController>(), true, Call: true);
                                DbuffActive(targetDbuff, i);
                            }
                            else
                            {
                                //Debug.LogError(User.name + " Falhou em Chamar o " + targetDbuff.name);
                                DbuffFail(targetDbuff, i);
                            }
                        }
                    }
                    break;
                #endregion

                #region cooldown
                case Dbuff.Cooldown:
                    if (targetDbuff.GetComponent<MobCooldown>() && targetDbuff.GetComponent<SkillManager>())
                    {
                        if (targetDbuff.GetComponent<SkillManager>().Skills.Count >= _DbuffBuff[i]._dbuffDuracaoMin
                            && CheckChance(Chance))
                        {
                            // Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
                            targetDbuff.GetComponent<MobCooldown>().AttCooldown(_DbuffBuff[i]._dbuffDuracaoMax, _DbuffBuff[i]._dbuffDuracaoMin);
                            DbuffActive(targetDbuff, i);
                        }
                        else
                        {
                            // Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
                            DbuffFail(targetDbuff, i);
                        }
                    }
                    break;
                #endregion

                #region Recupera Hp
                case Dbuff.Recupera_HP:
                    if (targetDbuff.GetComponent<MobHealth>())
                    {
                        if (CheckChance(Chance) && targetDbuff.GetComponent<MobHealth>().RecHp(User, Duration))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    }
                    break;
                #endregion

                #region Escudo
                case Dbuff.Escudo:
                    if (targetDbuff.GetComponent<MobHealth>())
                    {
                        if (CheckChance(Chance))
                        {
                            if (targetDbuff.GetComponent<MobHealth>().defense < Duration || Acumule)
                            {
                                //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                                targetDbuff.GetComponent<MobHealth>().Defense
                                    (Acumule
                                    ? targetDbuff.GetComponent<MobHealth>().defense + Duration
                                    : Duration, User);
                                DbuffActive(targetDbuff, i);
                                return;
                            }
                        }


                            //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                            DbuffFail(targetDbuff, i);
                    }
                    break;
                #endregion

                #region Bonus Atk
                case Dbuff.Buff_Atk:
                    if (targetDbuff.GetComponent<MobManager>())
                    {
                        if (CheckChance(Chance))
                        {
                            //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                            targetDbuff.GetComponent<MobManager>().AtiveBuffDamage
                                (
                                _DbuffBuff[i]._dbuffDuracaoMin, //valor
                                _DbuffBuff[i]._dbuffDuracaoMax, //tempo
                               _DbuffBuff[i]._acumule ? -1 : this.GetInstanceID()); //Acumulativo
                            DbuffActive(targetDbuff, i);
                        }
                        else
                        {
                            //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                            DbuffFail(targetDbuff, i);
                        }
                    }
                    break;
                    #endregion
            }
        }
        #endregion
        */
        #endregion
    }

    /// <summary>
    /// Ativa Dbuff Na mesma hora que e criado
    /// </summary>
    /// <param name="targetDbuff"></param>
    /// <param name="Buff"></param>
    /// <param name="forMe"></param>
    /// <param name="Chance"></param>
    /// <param name="MinDuration"></param>
    /// <param name="MaxDuration"></param>
    public bool CreateDbuff(GameObject targetDbuff, Dbuff Buff, bool forMe, float Chance, int MinDuration=-1, int MaxDuration=-1,bool Acumule=false,int MaxAcumule=1)
    {
        return ApplyDbuff(targetDbuff,Buff,forMe,Chance,MinDuration,MaxDuration, Acumule, MaxAcumule);

        #region Old
        /*
        if (Chance > 1)
            Chance /= 100;

        if (MinDuration == -1)
            MinDuration = currentdamage;

        if (MaxDuration == -1)
            MaxDuration = currentdamage;

        int Duration = UnityEngine.Random.Range(MinDuration, MaxDuration + 1);

        if (targetDbuff == null)
        {
            targetDbuff = Target;
        }

        if (forMe)
        {
            targetDbuff = User;
        }

        if (!forMe && User == targetDbuff)
            targetDbuff = null;

        Debug.LogError("Check Hit[" + Buff + "( Create )] Skill(" + nome + ") -  Duration[" + Duration + "] / Chance[" + Chance * 100 + "%] no " + targetDbuff);

        if (GameManagerScenes._gms)
        {
            GameManagerScenes._gms.NewInfo("Check Hit[" + Buff + "( Create )] Skill(" + nome + ") -  Duration[" + Duration + "] / Chance[" + Chance * 100 + "%] no " + targetDbuff, 4,true);
        }

        #region Buffs
        if (targetDbuff != null)
        {
            switch (Buff)
            {
                #region Fire
                case Dbuff.Fire:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                    {
                       return targetDbuff.GetComponent<MobDbuff>().AtiveDbuffFire(User, duration: Duration, _chance: Chance);
                    }
                    break;
                #endregion

                #region Poison
                case Dbuff.Envenenar:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                    {
                        return targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPoison(User, duration: Duration, _chance: Chance);
                    }
                    break;
                #endregion

                #region Petrify
                case Dbuff.Petrificar:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                    {
                        return (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPetrify(User, duration: Duration, _chance: Chance));
                    }
            break;
                #endregion

                #region Stun
                case Dbuff.Stun:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                    {
                        return targetDbuff.GetComponent<MobDbuff>().AtiveDbuffStun(User, duration: Duration, _chance: Chance);
                    }
            break;
                #endregion

                #region Bleed
                case Dbuff.Bleed:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        return targetDbuff.GetComponent<MobDbuff>().AtiveDbuffBleed(User, duration: Duration, _chance: Chance);
                    break;
                #endregion

                #region Recuar
                case Dbuff.Recuar:
                    if (targetDbuff.GetComponent<MoveController>())
                    {
                        for (int _buff = 0; _buff < Duration; _buff++)
                        {
                            if (CheckChance(Chance))
                            {
                                Debug.LogError(User.name + " Fez o " + targetDbuff.name + " Recuar");
                                targetDbuff.GetComponent<MoveController>().EnemyWalk(User.GetComponent<MoveController>(), true, true, Call: true);
                                return true;
                            }
                            else
                            {
                                Debug.LogError(User.name + " Falhou em Recuar o " + targetDbuff.name);
                                return false;
                            }
                        }
                    }
                    break;
                #endregion

                #region chamar
                case Dbuff.Chamar:
                    if (targetDbuff.GetComponent<MoveController>())
                    {
                        for (int buff = 0; buff < Duration; buff++)
                        {
                            if (CheckChance(Chance))
                            {
                                Debug.LogError(User.name + " Chamou o " + targetDbuff.name);
                                targetDbuff.GetComponent<MoveController>().EnemyWalk(User.GetComponent<MoveController>(), true, Call: true);
                                return true;
                            }
                            else
                            {
                                Debug.LogError(User.name + " Falhou em Chamar o " + targetDbuff.name);
                                return false;
                            }
                        }
                    }
                    break;
                #endregion

                #region cooldown
                case Dbuff.Cooldown:
                    if (targetDbuff.GetComponent<MobCooldown>())
                    {
                        if (skillManager.Skills.Count > MinDuration)
                            if (CheckChance(Chance))
                            {
                                Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[MinDuration].Nome + ") do " + targetDbuff.name + " para " + MaxDuration);
                                targetDbuff.GetComponent<MobCooldown>().AttCooldown(MaxDuration, MinDuration);
                                //skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].CooldownCurrent = _DbuffBuff[i]._dbuffDuracaoMax;
                                //DbuffActive(targetDbuff,i);
                                return true;
                            }
                            else
                            {
                                Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[MinDuration].Nome + ") do " + targetDbuff.name + " para " + MaxDuration);
                                //DbuffFail(i);
                                return false;
                            }
                    }
                    break;
                #endregion

                #region Recupera Hp
                case Dbuff.Recupera_HP:
                    if (targetDbuff.GetComponent<MobHealth>())
                    {
                        if (CheckChance(Chance))
                        {
                            Debug.LogError(User.name + " Curou o " + targetDbuff.name + " em " + Duration);
                            return targetDbuff.GetComponent<MobHealth>().RecHp(User, Duration);
                        }
                        else
                        {
                            Debug.LogError(User.name + " Falhou em Curar o " + targetDbuff.name + " em " + Duration);
                            //DbuffFail(i);
                            return false;
                        }
                    }
                    break;
                #endregion

                #region Escudo
                case Dbuff.Escudo:
                    if (targetDbuff.GetComponent<MobHealth>())
                    {
                        if (CheckChance(Chance))
                        {
                            Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                            targetDbuff.GetComponent<MobHealth>().Defense(Duration,User);
                            return true;
                        }
                        else
                        {
                            Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                            return false;
                        }
                    }
                    break;
            }
            #endregion
        }
        #endregion

        return false;
        */
        #endregion
    }

    /// <summary>
    /// Insere Dbuff na Lista
    /// </summary>
    /// <param name="Buff"></param>
    /// <param name="forMe"></param>
    /// <param name="Chance"></param>
    /// <param name="MinDuration"></param>
    /// <param name="MaxDuration"></param>
    ///  <param name="addInDescription">Assim q cria ja coloca na descrição da skill</param>
    /// <returns>index na lista</returns>
    protected int CreateDbuff(Dbuff Buff, bool forMe, float Chance, int MinDuration = -1, int MaxDuration = -1,bool Acumule=false, int MaxAcumule = 1,bool addInDescription=true)
    {
        DbuffBuff dbuff = new DbuffBuff();
        
        if (Chance > 1)
            Chance /= 100;

        if (MinDuration == -1)
            MinDuration = currentdamage;

        if (MaxDuration == -1)
            MaxDuration = currentdamage;

        dbuff._buff            = Buff;
        dbuff._dbuffChance     = Chance;
        dbuff._forMe           = forMe;
        dbuff._dbuffDuracaoMin = MinDuration;
        dbuff._dbuffDuracaoMax = MaxDuration;
        dbuff._acumule         = Acumule;
        dbuff._acumuleMax      = MaxAcumule;

        if (!_dbuffBuff.Contains(dbuff))
        {
            Debug.Log("DBuff[" + Buff + "( Create )] Skill(" + nome + ") -  Duration[" + MinDuration + " - "+ MaxDuration + "] / Chance(" + Chance * 100 + "%)] inserida na lista");

            _DbuffBuff.Add(dbuff);

            if (addInDescription)
            {
                Description += " "+(forMe ? "<color=blue>" + User.GetComponent<ToolTipType>()._name : "<color=red>Alvo") + " tem $NewBuff-1";

                int get = 251; //[b]{0}[/b]% de chance de Ganhar [b]{1} - {2}[/b] de {3}
                string get0 = "" + Chance * 100,
                       get1 = "" + MinDuration,
                       get2 = "" + MaxDuration,
                       get3 = XmlMenuInicial.Instance.DbuffTranslate(Buff);

                if (MinDuration == MaxDuration) { 
                    get = 252;//[b]{0}[/b]% de chance de Ganhar [b]{1}[/b] de {2} Por {3} Turno(s)
                    get0 = "" + Chance * 100;
                    get1 = "" + MinDuration;
                    get2 = XmlMenuInicial.Instance.DbuffTranslate(Buff);
                    get3 = ""+ MaxDuration;
                }

                AttDescription("$NewBuff-1",
            GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(get)
                 , get0
                 , get1
                 , get2
                 , get3
                ));

                if (Acumule)
                {
                    Description += ", $NewBuffA-1";
                    get = 254;//[b]Acumula por x{0} vezes[/b]
                    get0 = "" + MaxAcumule;
                    AttDescription("$NewBuffA-1",
                GameManagerScenes._gms.AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(get)
                     , get0
                    ));
                }


                Description += ".</color>";
                //AttDescriptionDbuff(-1);
            }

            return _dbuffBuff.Count - 1;
        }

        return - 1;
    }

    protected virtual void DbuffActive(GameObject targetDbuff, int index)
    {
        if (index < 0)
            return;

        Debug.LogError(_DbuffBuff[index]._buff + "("+index+ ") foi Ativo para o " + targetDbuff+"!!");

        //if (GameManagerScenes._gms)
        //    GameManagerScenes._gms.NewInfo(name+" "+_DbuffBuff[index]._buff + "(" + index + ") foi Ativo para o " + targetDbuff + "!!", 4, true);

    }

    protected virtual void DbuffFail(GameObject targetDbuff, int index)
    {
        if (index<0)
            return;
        
        Debug.LogError(_DbuffBuff[index]._buff + "("+index+ ") Falhou para o " + targetDbuff);

       // if (GameManagerScenes._gms)
       //     GameManagerScenes._gms.NewInfo(name + " " + _DbuffBuff[index]._buff + "(" + index + ") foi Falhou para o " + targetDbuff + "!!", 4, true);

    }

    public bool CheckChance(float chance)
    {
        if (chance > 1)
            chance /= 100;

        if (chance >= 1)
            return true;
          else
        if (chance <= 0)
            return false;

        float value = UnityEngine.Random.value;
        
        Debug.LogError("CheckChance -> chance de (" + (chance * 100) + "%) >= " + (value * 100).ToString("F1"));

        return (value <= chance);
    }
    #endregion

    #region Hex
    protected void RegisterOtherHex(int _X = -1, int _Y = -1, int _range = 1, bool _colore = true, int _color = 3, bool _clearList = false,bool _onlyFree=false)
    {
        if (_clearList)
            HexList.Clear();

        if (_X == -1)
            _X = moveController.hexagonX;

        if (_Y == -1)
            _Y = moveController.hexagonY;

        foreach (var hex in CheckGrid.Instance.RegisterRadioHex(_X, _Y, _range,_colore, _color, _onlyFree))
        {
            HexList.Add(hex);
        }   
    }

    protected virtual void RegisterOtherHexOnlyFree()
    {
        if (HexList.Count >= 1)
            for (int i = 0; i < HexList.Count; i++)
            {
                if (!HexList[i].free || HexList[i].currentMob != null)
                    HexList.Remove(HexList[i]);
            }
    }

    public void ColorHex(int color = 3)
    {
        for (int i = 0; i < HexList.Count; i++)
            CheckGrid.Instance.ColorGrid(color, HexList[i].x, HexList[i].y);
    }
    #endregion

    #region Passive
    public virtual void ActivePassive(Passive _passive, GameObject gO)
    {
       /* if (_passive == Passive.EndSkill && useSkill)
            ResetCoolDownManager();*/

        if (passive == null)
            return;

        passive.StartPassive(gO,_passive);        
    }

    public virtual void ActivePassive(Passive _passive, float value, GameObject gO)
    {
       /* if (_passive == Passive.EndSkill && useSkill)
            ResetCoolDownManager();*/

        if (passive == null)
            return;

        passive.StartPassive(gO, value, _passive);
    }
    #endregion  

    protected bool CheckUseSkill()
    {
        if (UseSkillChecket || ChanceUseSkill>=1)
            return true;

        float R = UnityEngine.Random.value;

        Debug.LogError("***Chance Use Skill("+ (ChanceUseSkill < R) + ") :"+ChanceUseSkill+" < "+R);

        if (ChanceUseSkill < R)
        {
            EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(198), User);//Falhou

            EndSkill();
            return false;
        }

        return true;
    }
}
