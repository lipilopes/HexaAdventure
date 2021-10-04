using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MobSkillAreaDamage : MonoBehaviour
{
    [SerializeField, Tooltip("Tamanho da area de dano")]
    protected int   _range    = 2;
    public int Range { get { return _range; } }
    [Space]
    [SerializeField, Tooltip("Tempo Ativo $T")]
    protected int _totalTime  = 3;
    protected int _timeOn;

    public int TimeOn { get { return _timeOn; } }

    [SerializeField, Range(0, 100), Tooltip("$%Da Dano Total")]
    protected int _porcentDamage=100;
   // [SerializeField, Tooltip("Dano real")]
    protected bool _realDamage = false;
    public    bool RealDamage { get { return _realDamage; } set { _realDamage = value; } }
    [SerializeField, Range(0, 100), Tooltip("$MH pega % do dano total para subtrair do maxhitDamage")]
    protected int _porcenMaxHitDamage = 25;
    [SerializeField, Tooltip("pega o dano total para dividir do maxhitDamage em cada chance")]
    protected int _maxHitAreaDivideDamage = 2;
    [SerializeField, Tooltip("Dano extra pode dar Dbuff")]
    protected bool _maxHitDbuff  = false;
    //[SerializeField, Tooltip("Dano real no hit extra")]
    protected bool _maxHitRealDamage = false;
    public    bool MaxHitRealDamage { get { return _maxHitRealDamage; } set { _maxHitRealDamage = value; } }
    [Space]
    [SerializeField, Tooltip("Dano Padrao pode dar dano critico")]
    protected bool _damageCritical       = false;
    [SerializeField, Tooltip("Dano Extra pode dar dano critico")]
    protected bool _maxHitDamageCritical = false;
    [SerializeField, Tooltip("Dano ao andar sobe pode dar dano critico")]
    protected bool _walkDamageCritical   = false;
    //[SerializeField, Tooltip("Dano real ao andar sobe")]
    protected bool _walkRealDamage = false;
    public    bool WalkRealDamage { get { return _walkRealDamage; } set { _walkRealDamage = value; } }
    [Space]
    [Tooltip("Apos User Morrer, Area Damage sera Desativada")][HideInInspector]
    public bool _desactiveAreaDamage = true;
    [Space]
    [SerializeField, Tooltip("Dbuff ao dar dano normal mesma tag das skills")]
    protected List<DbuffBuff> _DbuffBuff = new List<DbuffBuff>();
    [SerializeField, Tooltip("Dbuff ao dar dano quando inimigo anda sobe mesma tag das apos o '$' coloque 'W'")]
    protected List<DbuffBuff> _DbuffBuffInWalk = new List<DbuffBuff>();
    [Space]
    [SerializeField]
    protected List<HexManager> hex = new List<HexManager>();
    [Space]
    [SerializeField]
    protected bool targetFriend;
    [SerializeField]
    protected bool targetMe;
    
    /// <summary>
    /// $D
    /// </summary>
    protected int   _damage;
    protected float _chanceCrit;
    /// <summary>
    /// $W
    /// </summary>
    protected float _damageWalkIn;
    /// <summary>
    /// $MH
    /// </summary>
    protected float _maxHit;
    /// <summary>
    /// $CH
    /// </summary>
    protected float _chanceHit;
    protected MobManager.MobTime _myTime;

    protected GameObject _target;

    [SerializeField]
    protected GameObject _user;
    public GameObject User{ get { return _user; } set { _user = value; } }

    CheckGrid checkGrid;
    [Space]
    [SerializeField]
    protected UnityEvent eventStartDamageArea;
    [SerializeField]
    protected UnityEvent eventTimerDesativeArea;

    public HexManager Solo { get; set; }

    [HideInInspector]
    public bool _attackInThisTurn = false;
    public void AttackInThisTurnReset() { _attackInThisTurn = false; }


    protected virtual void Start()
    {
        checkGrid = CheckGrid.Instance;
    }

    #region Dbuff
    protected virtual void Hit(GameObject targetDbuff, bool walk)
    {
        int count = 0;

        if (walk)
            count = _DbuffBuffInWalk.Count;
        else
            count = _DbuffBuff.Count;

        Debug.LogError(name + " - Hit() - " + count);

        if (count == 0)
            return;

        if (walk)
        {
            for (int i = 0; i < count; i++)
            {
                CheckDbuffWalk(targetDbuff, i);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                CheckDbuff(targetDbuff, i);
            }
        }
    }

    protected         void CheckDbuff(GameObject targetDbuff, int i = -1)
    {
        if (_DbuffBuff[i]._dbuffDuracaoMin == -1)
            _DbuffBuff[i]._dbuffDuracaoMin = _damage;

        if (_DbuffBuff[i]._dbuffDuracaoMax == -1)
            _DbuffBuff[i]._dbuffDuracaoMax = _damage;

        int Duration = UnityEngine.Random.Range(_DbuffBuff[i]._dbuffDuracaoMin, _DbuffBuff[i]._dbuffDuracaoMax + 1);
        float Chance = _DbuffBuff[i]._dbuffChance;


        if (_DbuffBuff[i]._forMe)
        {
            targetDbuff = _user;
        }

        if (!_DbuffBuff[i]._forMe && _user == targetDbuff)
            targetDbuff = null;

        Debug.LogError("Check Hit[" + _DbuffBuff[i]._buff + "( " + (i) + " )] Skill(" + name + ") -  Duration[" + Duration + "] / Chance[" + Chance * 100 + "%] no " + targetDbuff);

        #region Buffs
        if (targetDbuff != null)
        {
            switch (_DbuffBuff[i]._buff)
            {
                #region Fire
                case Dbuff.Fire:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffFire(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Poison
                case Dbuff.Envenenar:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPoison(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Petrify
                case Dbuff.Petrificar:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPetrify(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Stun
                case Dbuff.Stun:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffStun(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i);
                        else
                            DbuffFail(targetDbuff, i);
                    break;
                #endregion

                #region Bleed
                case Dbuff.Bleed:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffBleed(_user, duration: Duration, _chance: Chance))
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
                                targetDbuff.GetComponent<MoveController>().EnemyWalk(_user.GetComponent<MoveController>(), true, true,true);
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
                                targetDbuff.GetComponent<MoveController>().EnemyWalk(_user.GetComponent<MoveController>(), true,Call:true);
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
                    if (targetDbuff.GetComponent<MobCooldown>())
                    {
                        if (_user.GetComponent<SkillManager>().Skills.Count > _DbuffBuff[i]._dbuffDuracaoMin && CheckChance(Chance))
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
                        if (CheckChance(Chance) && targetDbuff.GetComponent<MobHealth>().RecHp(_user, Duration))
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
                        if (targetDbuff.GetComponent<MobHealth>().defense > Duration && CheckChance(Chance))
                        {
                            //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                            targetDbuff.GetComponent<MobHealth>().Defense(Duration,User);
                            DbuffActive(targetDbuff, i);
                        }
                        else
                        {
                            //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                            DbuffFail(targetDbuff, i);
                        }
                    }
                    break;
            }
            #endregion
        }
        #endregion
    }

    protected virtual void DbuffActive(GameObject targetDbuff, int index,bool walk=false)
    {
        if (walk)
        {
            Debug.LogError(_DbuffBuffInWalk[index]._buff + "(" + index + ") foi Ativo para o " + targetDbuff + "!!");
        }
        else
        Debug.LogError(_DbuffBuff[index]._buff + "(" + index + ") foi Ativo para o " + targetDbuff + "!!");
    }

    protected virtual void DbuffFail(GameObject targetDbuff, int index, bool walk = false)
    {
        if (walk)
        {
            Debug.LogError(_DbuffBuffInWalk[index]._buff + "(" + index + ") Falhou para o " + targetDbuff);
        }
        else
            Debug.LogError(_DbuffBuff[index]._buff + "(" + index + ") Falhou para o " + targetDbuff);
    }

    protected void CheckDbuffWalk(GameObject targetDbuff, int i = -1)
    {
        if (targetDbuff == null)
        {
            return;
        }

        if (_DbuffBuffInWalk[i]._dbuffDuracaoMin == -1)
            _DbuffBuffInWalk[i]._dbuffDuracaoMin = _damage;

        if (_DbuffBuffInWalk[i]._dbuffDuracaoMax == -1)
            _DbuffBuffInWalk[i]._dbuffDuracaoMax = _damage;

        int Duration = UnityEngine.Random.Range(_DbuffBuffInWalk[i]._dbuffDuracaoMin, _DbuffBuffInWalk[i]._dbuffDuracaoMax + 1);
        float Chance = _DbuffBuffInWalk[i]._dbuffChance;


        if (_DbuffBuffInWalk[i]._forMe)
        {
            targetDbuff = _user;
        }

        if (!_DbuffBuffInWalk[i]._forMe && _user == targetDbuff)
            targetDbuff = null;

        Debug.LogError("Check Hit[" + _DbuffBuffInWalk[i]._buff + "( " + (i) + " )] Skill(" + name + ") -  Duration[" + Duration + "] / Chance[" + Chance * 100 + "%] no " + targetDbuff);

        #region Buffs
        if (targetDbuff != null)
        {
            switch (_DbuffBuffInWalk[i]._buff)
            {
                #region Fire
                case Dbuff.Fire:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffFire(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i,true);
                        else
                            DbuffFail(targetDbuff, i, true);
                    break;
                #endregion

                #region Poison
                case Dbuff.Envenenar:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPoison(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i, true);
                        else
                            DbuffFail(targetDbuff, i, true);
                    break;
                #endregion

                #region Petrify
                case Dbuff.Petrificar:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPetrify(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i, true);
                        else
                            DbuffFail(targetDbuff, i, true);
                    break;
                #endregion

                #region Stun
                case Dbuff.Stun:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffStun(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i, true);
                        else
                            DbuffFail(targetDbuff, i, true);
                    break;
                #endregion

                #region Bleed
                case Dbuff.Bleed:
                    if (targetDbuff.GetComponent<MobDbuff>() != null)
                        if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffBleed(_user, duration: Duration, _chance: Chance))
                            DbuffActive(targetDbuff, i, true);
                        else
                            DbuffFail(targetDbuff, i, true);
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
                                targetDbuff.GetComponent<MoveController>().EnemyWalk(_user.GetComponent<MoveController>(), true, true,true);
                                DbuffActive(targetDbuff, i, true);
                            }
                            else
                            {
                                //Debug.LogError(User.name + " Falhou em Recuar o " + targetDbuff.name);
                                DbuffFail(targetDbuff, i, true);
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
                                targetDbuff.GetComponent<MoveController>().EnemyWalk(_user.GetComponent<MoveController>(), true,Call: true);
                                DbuffActive(targetDbuff, i, true);
                            }
                            else
                            {
                                //Debug.LogError(User.name + " Falhou em Chamar o " + targetDbuff.name);
                                DbuffFail(targetDbuff, i, true);
                            }
                        }
                    }
                    break;
                #endregion

                #region cooldown
                case Dbuff.Cooldown:
                    if (targetDbuff.GetComponent<MobCooldown>())
                    {
                        if (_user.GetComponent<SkillManager>().Skills.Count > _DbuffBuffInWalk[i]._dbuffDuracaoMin && CheckChance(Chance))
                        {
                            // Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[_DbuffBuffInWalk[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuffInWalk[i]._dbuffDuracaoMax);
                            targetDbuff.GetComponent<MobCooldown>().AttCooldown(_DbuffBuffInWalk[i]._dbuffDuracaoMax, _DbuffBuffInWalk[i]._dbuffDuracaoMin);
                            DbuffActive(targetDbuff, i, true);
                        }
                        else
                        {
                            // Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[_DbuffBuffInWalk[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuffInWalk[i]._dbuffDuracaoMax);
                            DbuffFail(targetDbuff, i, true);
                        }
                    }
                    break;
                #endregion

                #region Recupera Hp
                case Dbuff.Recupera_HP:
                    if (targetDbuff.GetComponent<MobHealth>())
                    {
                        if (CheckChance(Chance) && targetDbuff.GetComponent<MobHealth>().RecHp(_user, Duration))
                            DbuffActive(targetDbuff, i, true);
                        else
                            DbuffFail(targetDbuff, i, true);
                    }
                    break;
                #endregion

                #region Escudo
                case Dbuff.Escudo:
                    if (targetDbuff.GetComponent<MobHealth>())
                    {
                        if (targetDbuff.GetComponent<MobHealth>().defense > Duration && CheckChance(Chance))
                        {
                            //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                            targetDbuff.GetComponent<MobHealth>().Defense(Duration,User);
                            DbuffActive(targetDbuff, i, true);
                        }
                        else
                        {
                            //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                            DbuffFail(targetDbuff, i, true);
                        }
                    }
                    break;
            }
            #endregion
        }
        #endregion
    }

    public    virtual bool CheckChance(float chance)
    {
        float value = UnityEngine.Random.value;

        if (chance > 1)
            chance /= 100;

        Debug.LogError("CheckChance -> chance de (" + (chance * 100) + "%) >= " + (value * 100).ToString("F1"));

        return !(value > chance);
    }
    #endregion

    protected bool CanAttack(GameObject target)
    {
        if ( _user.GetComponent<MobManager>().MesmoTime(target) && targetFriend  ||
            !_user.GetComponent<MobManager>().MesmoTime(target) && !targetFriend ||
            target == _user && targetMe)
        {
            return true;
        }
        else
        return false;
    }

    public MobManager.MobTime MyTime
    {
        get
        {
            return _myTime;
        }

        set
        {
            _myTime = value;
        }
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(UpdateCoroutine());

        if (TurnSystem.Instance)
        {
            if (_desactiveAreaDamage)
            {
                TurnSystem.DelegateTurnEnd += DesactiveUserDead;

                TurnSystem.DelegateTurnEnd += AttackInThisTurnReset;
            }
       }
    }

    protected virtual void OnDisable()
    {
        StopCoroutine(UpdateCoroutine());

        if (TurnSystem.Instance)
        {
            if (_desactiveAreaDamage)
            {
                TurnSystem.DelegateTurnEnd -= DesactiveUserDead;

                TurnSystem.DelegateTurnEnd -= AttackInThisTurnReset;
            }
        }
    }

    protected virtual IEnumerator UpdateCoroutine()
    {
        while (this.gameObject.activeInHierarchy)
        {
            yield return null;

            if (hex.Count>=1)
            {
                transform.position = hex[0].transform.position;
            }
        }
    }

    protected void AttDescription()
    {
        if (GetComponent<ToolTipType>() == null)
            return;
        
            StartCoroutine(AttDescriptionCoroutine());
              
    }

    protected IEnumerator AttDescriptionCoroutine()
    {
        print("Wait AttDescriptionCoroutine");

        yield return new WaitForSeconds(0.5f);

        print("Start AttDescriptionCoroutine");

        string damageMaxHit = "";

        if (_chanceHit != 0 && _maxHit != 0)
        {
            damageMaxHit = _maxHit.ToString("F0") + " hits de ";

            for (int i = 2; i < _maxHit + 2; i++)
            {
                if (i != 2)
                    damageMaxHit += "/";

                int value = (_damage * _porcenMaxHitDamage / 100) / i;

                damageMaxHit += value <= 0 ? 1 : value;
            }
        }

        int count = GetComponent<ToolTipType>().Count;

        for (int i = 0; i < count; i++)
        {
            print("ToolTipType - "+i+"/"+ (count));

            #region Walk Damage DB
            for (int wdb = 0; wdb < _DbuffBuffInWalk.Count; wdb++)
            {
                if (_DbuffBuffInWalk[wdb]._dbuffDuracaoMin == -1)
                    _DbuffBuffInWalk[wdb]._dbuffDuracaoMin = (int)_damageWalkIn;

                if (_DbuffBuffInWalk[wdb]._dbuffDuracaoMax == -1)
                    _DbuffBuffInWalk[wdb]._dbuffDuracaoMax = (int)_damageWalkIn;

                string buff = ("$WBuff" + wdb);

                GetComponent<ToolTipType>().AttDescrição(i, buff, "<b>" + XmlMenuInicial.Instance.DbuffTranslate(_DbuffBuffInWalk[wdb]._buff) + "</b>");
                        
                GetComponent<ToolTipType>().AttDescrição(i, "_", " $Dur" + wdb + " de ");

                string chance = ("$W%" + wdb);
                GetComponent<ToolTipType>().AttDescrição(i, chance, "<b>" + (_DbuffBuffInWalk[wdb]._dbuffChance * 100).ToString() + "%</b>");

                string min = ("$WMin" + wdb);
                GetComponent<ToolTipType>().AttDescrição(i, min, "<b>" + _DbuffBuffInWalk[wdb]._dbuffDuracaoMin + "</b>");


                string max = ("$WMax" + wdb);
                GetComponent<ToolTipType>().AttDescrição(i, max, "<b>" + _DbuffBuffInWalk[wdb]._dbuffDuracaoMax + "</b>");

                string dur = ("$WDur" + wdb);
                string Dur = "<b>" + _DbuffBuffInWalk[wdb]._dbuffDuracaoMin + " - " + _DbuffBuffInWalk[wdb]._dbuffDuracaoMax + "</b>";
                if (_DbuffBuffInWalk[wdb]._dbuffDuracaoMin == _DbuffBuffInWalk[wdb]._dbuffDuracaoMax)
                    Dur = "<b>" + _DbuffBuffInWalk[wdb]._dbuffDuracaoMax + "</b>";

                GetComponent<ToolTipType>().AttDescrição(i, dur, Dur);
            }
            #endregion

            #region Damage DB
            for (int db = 0; db < _DbuffBuff.Count; db++)
            {
                if (_DbuffBuff[db]._dbuffDuracaoMin == -1)
                    _DbuffBuff[db]._dbuffDuracaoMin = _damage;

                if (_DbuffBuff[db]._dbuffDuracaoMax == -1)
                    _DbuffBuff[db]._dbuffDuracaoMax = _damage;

                string buff = ("$Buff" + db);
                GetComponent<ToolTipType>().AttDescrição(i, buff, "<b>" + _DbuffBuff[db]._buff + "</b>");

                GetComponent<ToolTipType>().AttDescrição(i, "_", " $Dur" + db + " de ");


                string chance = ("$%" + db);
                GetComponent<ToolTipType>().AttDescrição(i, chance, "<b>" + (_DbuffBuff[db]._dbuffChance * 100).ToString() + "%</b>");

                string min = ("$Min" + db);
                GetComponent<ToolTipType>().AttDescrição(i, min, "<b>" + _DbuffBuff[db]._dbuffDuracaoMin + "</b>");


                string max = ("$Max" + db);
                GetComponent<ToolTipType>().AttDescrição(i, max, "<b>" + _DbuffBuff[db]._dbuffDuracaoMax + "</b>");

                string dur = ("$Dur" + db);
                string Dur = "<b>" + _DbuffBuff[db]._dbuffDuracaoMin + " - " + _DbuffBuff[db]._dbuffDuracaoMax + "</b>";

                if (_DbuffBuff[db]._dbuffDuracaoMin == _DbuffBuff[db]._dbuffDuracaoMax)
                    Dur = "<b>" + _DbuffBuff[db]._dbuffDuracaoMax + "</b>";

                GetComponent<ToolTipType>().AttDescrição(i, dur, Dur);
            }
            #endregion

            GetComponent<ToolTipType>().AttDescrição(i, "$D",
            "<color="+(_realDamage ? "yellow" : "red") +"><b>" + _damage.ToString("F0") + "</b></color>");

            GetComponent<ToolTipType>().AttDescrição(i, "$W",
                "<color="+(_walkRealDamage ? "yellow" : "red") +"><b>" + _damageWalkIn.ToString("F0") + "</b></color>");

            GetComponent<ToolTipType>().AttDescrição(i, "$MH",
               "<color="+(_maxHitRealDamage ? "yellow" : "red") +"><b>" + damageMaxHit + "</b></color>");

            GetComponent<ToolTipType>().AttDescrição(i, "$CH", (_chanceHit * 100).ToString("F0") + "%");
            GetComponent<ToolTipType>().AttDescrição(i, "$T", (_timeOn-1).ToString("F0"));
        }


        GetComponent<ToolTipType>().AttExtraDescrição("$D",
            "<color=" + (_realDamage ? "yellow" : "red") + "><b>" + _damage.ToString("F0") + "</b></color>");

        GetComponent<ToolTipType>().AttExtraDescrição("$W",
            "<color=" + (_walkRealDamage ? "yellow" : "red") + "><b>" + _damageWalkIn.ToString("F0") + "</b></color>");

        GetComponent<ToolTipType>().AttExtraDescrição("$MH",
           "<color=" + (_maxHitRealDamage ? "yellow" : "red") + "><b>" + damageMaxHit + "</b></color>");

        GetComponent<ToolTipType>().AttExtraDescrição("$CH", (_chanceHit * 100).ToString("F0") + "%");
        GetComponent<ToolTipType>().AttExtraDescrição("$T", (_timeOn - 1).ToString("F0"));
    }

    #region Respaw
    public virtual void RespawSkill(GameObject user, GameObject target,HexManager _hex)
    {
        MyTime = user.GetComponent<MobManager>().TimeMob;

        if (hex.Count > 0)
            for (int i = 0; i < hex.Count; i++)
            {
                if (hex[i].currentItem == gameObject)
                    hex[i].currentItem = null;
            }

        _target = null;

        hex.Clear();

        hex.Add(_hex);

        if (_hex.currentItem == null)
        {
            _hex.currentItem = gameObject;
            _hex.puxeItem    = false;
        }     

        transform.position = _hex.transform.position;

        Solo = _hex;

        RegisterOtherHex(_hex);

        _user       = user;
        _damage     = (user.GetComponent<MobManager>().damage* _porcentDamage)/100;
        _target     = target;
        _timeOn     = _totalTime;
        _chanceCrit = user.GetComponent<MobManager>().chanceCritical;

        SkillRespawned(user, target, _hex);

        AttDescription();
        // Attack(false);
    }

    public virtual void RespawSkill(GameObject user, GameObject target, HexManager _hex, int damage,int walkDamage, int maxHit, float chanceHit, int time, float critico)
    {
        GameManagerScenes._gms.NewInfo("RespawSkill("+user+","+target+","+_hex+","+damage+","+walkDamage+","+maxHit+","+chanceHit+","+time+","+critico+")",3,true);

        if (user!=null)
        MyTime = user.GetComponent<MobManager>().TimeMob;

        if (hex.Count > 0)
            for (int i = 0; i < hex.Count; i++)
            {
                if (hex[i].currentItem == gameObject)
                    hex[i].currentItem = null;
            }

        _target = null;

        hex.Clear();

        hex.Add(_hex);

        if (hex[0].currentItem == null)
        {
            hex[0].currentItem = gameObject;
            hex[0].puxeItem = false;
        }

        transform.position = hex[0].transform.position;

        Solo = hex[0];

        RegisterOtherHex(_hex);

        if (user != null)
            _user          = user;

        _damage        = damage;
        _damageWalkIn  = walkDamage;
        _maxHit        = maxHit;
        _chanceHit     = chanceHit;
        _chanceCrit    = critico;
        _target        = target;
        _timeOn        = time;

        SkillRespawned( user,  target,  _hex);

        AttDescription();
        //Attack(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="_hex"></param>
    protected virtual void SkillRespawned(GameObject user, GameObject target, HexManager _hex)
    {
        _user = user;
        _target = target;

        gameObject.SetActive(true);

        if (GetComponent<PassiveManager>())      
            GetComponent<PassiveManager>().User = user;       
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="_hex"></param>
    public virtual void SkillRespawnedTeste(GameObject user, GameObject target, HexManager _hex, int damage, int walkDamage, int maxHit, float chanceHit, int time, float critico)
    {
        _user = user;
        _damage = damage;
        _damageWalkIn = walkDamage;
        _maxHit = maxHit;
        _maxHitAreaDivideDamage = maxHit;
        _porcenMaxHitDamage    = (int)(chanceHit*100);
        _chanceHit = chanceHit;
        _chanceCrit = critico;
        _target = target;
        _timeOn = time;

        gameObject.SetActive(true);

        if (GetComponent<PassiveManager>())
            GetComponent<PassiveManager>().User = _user;
    }
    #endregion

    #region Pre Attack
    protected virtual void RegisterOtherHex(HexManager _hex)
    {
        int X = _hex.x,
            Y = _hex.y;

        if (checkGrid == null)
            checkGrid = CheckGrid.Instance;

        foreach (HexManager hexList in checkGrid.RegisterRadioHex(X, Y, _range, true, 2))
            hex.Add(hexList);
    
        RegisterInAllHex();
    }

    protected virtual void RegisterInAllHex()
    {
        for (int i = 0; i < hex.Count; i++)
        {
            if (hex[i].currentItem == null)
                hex[i].currentItem = gameObject;
        }
    }

    public virtual void Attack(bool desative = true)
    {

        if (!gameObject.activeInHierarchy)
            return;

        GameManagerScenes._gms.NewInfo("Attack("+desative+") " + name+" - "+hex.Count, 3,true);
        Debug.LogError("Attack(" + desative + ") " + name + " - " + hex.Count);

        _attackInThisTurn = true;

        if (hex.Count > 0 && hex[0] != null)
        {
            if (hex[0].transform.position != transform.position)
                transform.position = hex[0].transform.position;

            for (int i = 0; i < hex.Count; i++)
            {
                if (hex[i] != null)
                    if (hex[i].currentMob != null)
                        if (hex[i].currentMob.GetComponent<MobHealth>() != null)
                        {
                            if (_target != null)
                            {
                                if (hex[i].currentMob == _target)
                                {
                                    Damage(hex[i].currentMob.GetComponent<MobHealth>());
                                }
                            }
                            else
                                if (hex[i].currentMob.GetComponent<MobManager>() != null)
                            {
                                if (!hex[i].currentMob.GetComponent<MobManager>().MesmoTime(MyTime))
                                {
                                    Damage(hex[i].currentMob.GetComponent<MobHealth>());
                                }
                            }
                        }
            }
        }

        if (desative)
            TimerDesactive();
    }
    #endregion

    #region Dano
    public    virtual void Damage(MobHealth target)
    {
        if (!CanAttack(target.gameObject))
            return;

        Debug.LogError(name + " attacou " + target.name);

        CameraOrbit.Instance.ChangeTarget(target.gameObject);

        #region Extra Damage
        if (_chanceHit > 0 && _maxHit > 0)
        {
            float _critChanceHit = _maxHitDamageCritical ? _chanceCrit : 0;

            int currentDamage = 0;

            for (int i = _maxHitAreaDivideDamage; i < _maxHit + _maxHitAreaDivideDamage; i++)
            {
                if (CheckChance(_chanceHit))
                {

                    int valueExtraDamage = (_damage * _porcenMaxHitDamage / 100) / i;

                    if (valueExtraDamage <= 0)
                        valueExtraDamage = 1;

                    if (_maxHitRealDamage)
                    {
                        target.RealDamage(_user, valueExtraDamage, _critChanceHit);

                        currentDamage++;

                        if (_maxHitDbuff)
                            Hit(target.gameObject, false);

                    }
                    else
                    {
                        target.Damage(_user, valueExtraDamage, _critChanceHit);

                        currentDamage++;

                        if (_maxHitDbuff)
                            Hit(target.gameObject, false);

                    }
                }
            }

            EffectManager.Instance.PopUpDamageEffect(currentDamage + "/" + _maxHit, User);
        }
        #endregion

        Debug.LogError(name + " atacou " + target.name + " e deu " + _damage);

        float _crit = _damageCritical ? _chanceCrit : 0;

        if (_realDamage)
        {
            target.RealDamage(_user, _damage, _crit);                                
        }
        else
        {
            target.Damage(_user, _damage, _crit);         
        }

        eventStartDamageArea.Invoke();

        Hit(target.gameObject, false);
    }

    protected virtual void WalkInDamage(MobHealth target)
    {
        if (!CanAttack(target.gameObject))
            return;

            Debug.LogError(name + " attacou " + target.name);

        if (_damageWalkIn > 0)
        {
            float _crit = _walkDamageCritical ? _chanceCrit : 0;

            if (_walkRealDamage)
                target.RealDamage(_user, _damageWalkIn, _crit);
            else
                target.Damage(_user, _damageWalkIn, _crit);
        }

        Hit(target.gameObject, true);
    }

    protected virtual void WalkInBuff(MobHealth target)
    {
        if (!_user.GetComponent<MobManager>().MesmoTime(target.gameObject))
            return;

            Debug.LogError(name + " deu Buff pro amigo " + target.name);

            Hit(target.gameObject, true);
    }

    public virtual void WalkIn(GameObject target)
    {
        if (target.GetComponent<MobHealth>() == null || _damageWalkIn == 0)
            return;

        if (!_user.GetComponent<MobManager>().MesmoTime(target.gameObject))
            WalkInDamage(target.GetComponent<MobHealth>());
    }
    #endregion

    public virtual void TimerDesactive()
    {
        if (_timeOn<=0)        
            return;       

        eventTimerDesativeArea.Invoke();

        _timeOn--;
       
        if (GetComponent<ToolTipType>())
        {
            int Timer = _timeOn;
            GetComponent<ToolTipType>().AttDescrição(0,
                GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(139),""+Timer++),//<b>Duração: {0} turno(s)</b>
                GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(139), ""+_timeOn));//<b>Duração: {0} turno(s)</b>
        }

        if (_timeOn == 0)
        {
            Desactive();
            return;
        }

       EffectManager.Instance.PopUpDamageEffect(
           GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(140),""+_timeOn)//"Acaba em {0} Turnos"
           , gameObject);
    }

    public virtual void Desactive()
    {
       if (!gameObject.activeInHierarchy)
            return;

        gameObject.SetActive(false);

        for (int i = 0; i < hex.Count; i++)
        {
            if (hex[i].currentItem == gameObject)
                hex[i].currentItem = null;
        }

        _target = null;

        hex.Clear();

        EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(132), gameObject);//Acabou
    }

    public virtual void DesactiveUserDead()
    {
        if (User==null)
            return;

        if (User.GetComponent<MobManager>().Alive && _attackInThisTurn)
            return;

        Attack(true);
    }

    public void teste()
    {
        AttDescription();
    }
}
