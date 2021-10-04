using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MobCooldown), typeof(MobDbuff),       typeof(EnemyAttack))]
[RequireComponent(typeof(IaAttackMob))]
[RequireComponent(typeof(BoxCollider), typeof(MoveController), typeof(MobHealth))]
public class MobManager : MonoBehaviour
{
    //public delegate void MobDead();
    //public static event MobDead DelegateMobDead;

    [HideInInspector]
    public InfoTable     infoTable;
    MoveController       moveController;
    MobDbuff             dbuff;
    SkillManager         skillManager;
    PassiveManager       passive;


    public SkillManager SkillManager
    {
        get { return skillManager; }
    }

    [Header("Skin")]
    public float _SkinID=0;
    public List<GameObject> _PrefabSkillsSkin= new List<GameObject>();

    [Header("Classe")]
    public bool isPlayer;
    [Tooltip("Caso seja o player ganha bonus nos status")]
    public bool getBonusPlayer=true;

    [SerializeField] MobTime _mobTime;
    public enum MobTime
    {
        Player,
        Enemy,
        None,
        Black,
        White
    }
    public MobTime TimeMob { get { return _mobTime; } set { _mobTime = value; /*Debug.LogError(name+" Mudou de time para ->"+value);*/ } }

    public bool MesmoTime(MobTime time)
    {
        return time == _mobTime;
    }

    public bool MesmoTime(GameObject time)
    {
        if (time!=null)
        if (time.GetComponent<MobManager>())
        {
            return TimeMob == time.GetComponent<MobManager>().TimeMob;
        }

        return false;
    }

    public Classe classe;

    public enum Classe
    {
        manual,
        random,
        /// <summary>HP ++++ Damage ----  Velocidade ---</summary>
        tanker,
        /// <summary>HP ---- Damage ++++ Velocidade ---</summary>
        adc,
        /// <summary>HP +--  Damage +--  Velocidade ++-</summary>
        suporte,  //
        /// <summary>HP ---  Damage +++  Velocidade +--</summary>
        assassino,
        /// <summary>HP +++  Damage +--  Velocidade ---</summary>
        soldado,
        /// <summary>HP +--  Damage ++-  Velocidade +--</summary>
        mago,
        /// <summary>HP ??? Damage ???  Velocidade ???</summary>
        other
    }

    [HideInInspector]
    public int _dificuldade;

    [Header("Status")]
    public int   damage;

    public float health;

    //public float chanceCriticalEasy;
    //public float chanceCriticalMedium;
    //public float chanceCriticalHard;

    [HideInInspector]
    public float chanceCritical=0;

    [Header("Resistencias")]
    [Range(0, 100)] public float DamageResistenceFire    = 0;
    [Range(0, 100)] public float DamageResistencePoison  = 0,
                                 DamageResistencePetrify = 0,
                                 DamageResistenceBleed   = 0;

    [Header("Turnos")]
    public bool myTurn;
    public bool walkTurn,
                attackTurn;

    [HideInInspector]
    public bool fire    = false,  // so leva dano no inicio de cada turn de ''everyone'' [Dano 5%]    Duração: 1 Turno/List
                poison  = false,  // leva dano si andar                                  [Dano 10%]   Duração: 2 Turno/na sua vez
                petrify = false,  // perde a vez, e leva dano no inicio da sua vez       [Dano 25%]   Duração: 1 Turno/na sua vez
                stun    = false,  // perde a vez
                bleed   = false;  //Leva dano no inicio do turn e ao andar               [Dano 5%]    Duração: 2 Turno/List

    [Header("Times")]
    public int maxTimeWalk;

    [SerializeField] int maxTimeAttack;

    public int MaxTimeAttack { get { return maxTimeAttack; } set { maxTimeAttack = value; } }
    [HideInInspector]
    public int currentTimeAttack;

    //WaitForSeconds waitEndTurn = new WaitForSeconds(2);

    public Animator anim;
    public bool WalkAnim
    {
        get
        {
            if (anim!=null)
            {
                return anim.GetBool("Walk");
            }

            return false;
        }
        set
        {
            if (anim!=null)
            {
                anim.SetBool("Walk",value);
            }
        }
    }

    int countKill = 0;
    public int CountKill
    {
        set
        {
            countKill = value;

            string n = "";

            if (CountKill == 2)
                n = "Double kill";

            if (countKill == 3)
                n = "Triplo kill";

            if (countKill == 4)
            {
                n = "Quadra kill";

                if (isPlayer)
                    GameManagerScenes._gms.TotalQuadraKill = (1);
            }
            if (countKill == 5)
            {
                n = "Penta kill";

                if (isPlayer)
                    GameManagerScenes._gms.TotalPentaKill = (1);
            }
            if (countKill > 5)
            {
                n = "Hack Mode on [" + countKill + " Kills]";

                if (isPlayer)
                    GameManagerScenes._gms.TotalPentaKill = (1);
            }

            if (GameManagerScenes._gms.Adm && countKill > 2)
                GameManagerScenes._gms.NewInfo(GetComponent<ToolTipType>()._name + " fez " + n, 3);

            if (n != "")
                Debug.LogError(GetComponent<ToolTipType>()._name + " fez " + n);
        }

        get { return countKill; }
    } 

    public void UpdateCountKill()
    {
        CountKill = 0;
        foreach (var targets in GetComponent<EnemyAttack>().ListTarget)
        {
            if (targets==null)
            {
                CountKill++;
            }
        }
    }

    void Start()
    {
        infoTable = InfoTable.Instance;
        moveController = this.GetComponent<MoveController>();
        dbuff = GetComponent<MobDbuff>();
        skillManager = GetComponent<SkillManager>();
        passive = GetComponent<PassiveManager>();
        GameManagerScenes _gms = GameManagerScenes._gms;

        if (GetComponent<Animator>() != null)
            anim = GetComponent<Animator>();

        //moveController.isPlayer = isPlayer;

        if (TurnSystem.Instance)
        {
            List<GameObject> target = TurnSystem.Instance.GetMob(TimeMob, true);

            transform.transform.LookAt(target.Count > 0 ? target[Random.Range(0, target.Count)].transform : transform);
        }

        if (Classe.manual != classe)
        {

            int id = -1;

            if (GetComponent<ToolTipType>() &&
                GetComponent<ToolTipType>()._type == ToolTipType.Type.Mob &&
                GetComponent<ToolTipType>()._XmlID != -1)
                id = GetComponent<ToolTipType>()._XmlID;
            else
                id = _gms.HeroID(gameObject);

            Debug.LogError(name + " id: " + id);

            if (id != -1)
            {
                DamageResistenceFire = _gms.GetHeroFireResistence(id);
                DamageResistencePoison = _gms.GetHeroPoisonResistence(id);
                DamageResistencePetrify = _gms.GetHeroPetrifyResistence(id);
                DamageResistenceBleed = _gms.GetHeroBleedResistence(id);

                maxTimeWalk = _gms.GetHeroWalk(id);
                maxTimeAttack = _gms.GetHeroAttack(id);
            }

            #region Classe (Não player) ou esteja no modo batalha
            if (!isPlayer || GameManagerScenes.BattleMode)
            {
                if (classe == Classe.random)
                {
                    #region old
                    /*
                    int V = Random.Range((int)Classe.tanker, (int)Classe.mago + 1);

                    #region Random Classe
                    switch (V)
                    {
                        case 2:
                            classe = Classe.tanker;
                            break;

                        case 3:
                            classe = Classe.adc;
                            break;

                        case 4:
                            classe = Classe.suporte;
                            break;

                        case 5:
                            classe = Classe.assassino;
                            break;

                        case 6:
                            classe = Classe.soldado;
                            break;

                        case 7:
                            classe = Classe.mago;
                            break;
                    }
                    #endregion
                    */
                    #endregion

                    var values = System.Enum.GetValues(typeof(Classe));
                    classe = (Classe)Random.Range(2, values.Length-1);

                    print(name + " ficou da classe " + classe.ToString());
                }               

                moveController.time = maxTimeWalk;

                damage = _gms.GetHeroDamage(id);
                health = _gms.GetHeroHealth(id);
                chanceCritical = _gms.GetHeroCritico(id) * 100;

                if (GameManagerScenes.BattleMode)
                {
                    print("Health[" + id + " - " + name + "]: H:" + health + " - HB:" + _gms.BattleModeOptionStatusHpBaseExtra);

                    health += _gms.BattleModeOptionStatusHpBaseExtra;
                    damage += _gms.BattleModeOptionStatusDamageBaseExtra;
                    chanceCritical += _gms.BattleModeOptionStatusCriticalChanceBaseExtra;

                    DamageResistenceFire += _gms.BattleModeOptionDbuffFireResistenceExtra;
                    DamageResistencePoison += _gms.BattleModeOptionDbuffPoisonResistenceExtra;
                    DamageResistencePetrify += _gms.BattleModeOptionDbuffPetrifyResistenceExtra;
                    DamageResistenceBleed += _gms.BattleModeOptionDbuffBleedResistenceExtra;

                    chanceCritical = (chanceCritical > 100 ? 100 : chanceCritical);
                    DamageResistenceFire = (DamageResistenceFire > 100 ? 100 : DamageResistenceFire);
                    DamageResistencePoison = (DamageResistencePoison > 100 ? 100 : DamageResistencePoison);
                    DamageResistencePetrify = (DamageResistencePetrify > 100 ? 100 : DamageResistencePetrify);
                    DamageResistenceBleed = (DamageResistenceBleed > 100 ? 100 : DamageResistenceBleed);

                    //this.GetComponent<MobHealth>().StartHealth(health);

                    //this.GetComponent<IaAttackMob>().AttAttack();

                    //skillManager.CheckMoreDistanceSkill();

                    //if (GetComponent<ToolTipType>() != null)
                    //    GetComponent<ToolTipType>().AttToltip();

                    this.GetComponent<MobHealth>().StartHealth(health);

                    this.GetComponent<IaAttackMob>().AttAttack();

                    skillManager.BonusCooldown();

                    if (GetComponent<EnemyAttack>() != null)
                        GetComponent<EnemyAttack>().Att();

                    skillManager.CheckMoreDistanceSkill();

                    if (GetComponent<ToolTipType>() != null)
                        GetComponent<ToolTipType>().AttToltip();

                    return;
                }
            }
            #endregion

            if (isPlayer)
            {
                health = _gms.Mob[id]._health;
                damage = _gms.Mob[id]._damage;

                chanceCritical += (getBonusPlayer ? _gms.BonusCritical : 0);
                health = health + (getBonusPlayer ? _gms.Health() : 0);
                damage = damage + (getBonusPlayer ? _gms.Damage() : 0);

                if (getBonusPlayer)
                    maxTimeWalk = maxTimeWalk == 0 && _gms.Walk() == 0 ? 1 : maxTimeWalk + (_gms.Walk());

                DamageResistenceFire += (getBonusPlayer ? _gms.FireResistence() : 0);

                DamageResistencePoison += (getBonusPlayer ? _gms.PoisonResistence() : 0);

                DamageResistencePetrify += (getBonusPlayer ? _gms.PetrifyResistence() : 0);

                DamageResistenceBleed += (getBonusPlayer ? _gms.BleedResistence() : 0);

                moveController.time = maxTimeWalk;
            }
        }

        chanceCritical = (chanceCritical > 100 ? 100 : chanceCritical);

        DamageResistenceFire = (DamageResistenceFire > 100 ? 100 : DamageResistenceFire);

        DamageResistencePoison = (DamageResistencePoison > 100 ? 100 : DamageResistencePoison);

        DamageResistencePetrify = (DamageResistencePetrify > 100 ? 100 : DamageResistencePetrify);

        DamageResistenceBleed = (DamageResistenceBleed > 100 ? 100 : DamageResistenceBleed);

        this.GetComponent<MobHealth>().StartHealth(health);

        this.GetComponent<IaAttackMob>().AttAttack();

        skillManager.BonusCooldown();

        if (GetComponent<EnemyAttack>() != null)
            GetComponent<EnemyAttack>().Att();

        skillManager.CheckMoreDistanceSkill();

        if (GetComponent<ToolTipType>() != null)
            GetComponent<ToolTipType>().AttToltip();
    }

    public void MyTurn()
    {
        myTurn = true;

        //infoTable.NewInfo("Turno do: "+GetComponent<ToolTipType>()._nameS,1);      
        CameraOrbit.Instance.ChangeTarget(gameObject);

        if (GetComponent<ToolTipType>() != null)
            GetComponent<ToolTipType>().AttToltip();                

        if (stun || petrify)
        {
            if (stun)
                EffectManager.Instance.PopUpDamageEffect("<color=black>"+XmlMenuInicial.Instance.Get(190)/*Stunado*/+"</color>", gameObject, 2);

            if (petrify)
                EffectManager.Instance.PopUpDamageEffect("<color=black>" + XmlMenuInicial.Instance.Get(191)/*Petrificado*/+ "</color>", gameObject, 2);

            EndTurn();
            return;
        }
        else
        {
            Debug.LogError("Agora e a vez do " + this.name);

            ButtonManager.Instance.ClearHUD(!isPlayer);

            skillManager.CheckMoreDistanceSkill();

            if (!isPlayer)
            {
                this.GetComponent<EnemyAttack>().CheckDistance();
                this.GetComponent<EnemyAttack>().CheckDistance(1);
                this.GetComponent<EnemyAttack>().CheckDistance(2);
                this.GetComponent<EnemyAttack>().CheckDistance(3);

                CheckGrid.Instance.ColorGrid(0,0,0,true);

                WalkTurn();

                Debug.LogError(this.name + " Isn't a Player mob");
            }
            else
            {
                GetComponent<EnemyAttack>().useSkill = false;

                if (GetComponent<PlayerControl>())
                {
                    Debug.LogError("Call PlayerControl " + name);
                    GetComponent<PlayerControl>().PlayerControlThis();
                }
                else
                if (RespawMob.Instance.Player == gameObject)
                {
                    Debug.LogError("Change Controller for " + name);
                    ButtonManager.Instance.PlayerInf(gameObject);
                }

                Debug.LogError(this.name+" Is a Player mob");
            }                        
        }           
    }

    #region Turnos Walk
    public void WalkTurn()
    {
        if (stun || petrify)
        {
            EndTurn();
            return;
        }
        else
        if (myTurn && !walkTurn)
        {
            Debug.LogError(name + " Start WalkTurn - " + moveController.time);

            walkTurn = true;

            moveController.time = maxTimeWalk;

            if (!isPlayer)
            {
                if (moveController.time <= 0)
                    EndWalkTurn();
                else
                    moveController.StartWalkTurn();
            }
        }
    }
    public void EndWalkTurn()//Finaliza turno
    {
        CheckGrid.Instance.ColorGrid(x:0,clear: true);

        walkTurn = false;
        //nao tem inimigos proximos / si nao aparece o botao das skill's logo apos o turno acaba

        if (stun || petrify)
        {
            EndTurn();
            return;
        }

        if (!isPlayer)
        {
            if (this.GetComponent<EnemyAttack>())
                AttackTurn();
        }         

        Debug.LogError(name + " End WalkTurn - " + moveController.time);
    }
    #endregion

    #region Turnos Attack
    public void AttackTurn()
    {
        if (stun || petrify)
        {
            EndTurn();
            return;
        }
        else
   if (myTurn)
        {
            currentTimeAttack = maxTimeAttack;

            if (!isPlayer)
            {
                this.GetComponent<EnemyAttack>().timeAttack = maxTimeAttack;
                this.GetComponent<EnemyAttack>().StartAttackTurn();
            }

            attackTurn = true;
            Debug.LogError(name + " Start AttackTurn - " + GetComponent<EnemyAttack>().timeAttack);
        }
    }

    public void EndAttackTurn()
    {
        CheckGrid.Instance.ColorGrid(0, 0, 0, true);

        if (GetComponent<ToolTipType>() != null)
            GetComponent<ToolTipType>().AttToltip();

        Debug.LogError(name + " End AttackTurn - " + GetComponent<EnemyAttack>().timeAttack);
        attackTurn = false;
        // moveController.walkTurn = false;
        EndTurn();        
    }
    #endregion   

    public void EndTurn()//button / IA
    {
        if (skillManager != null)
            skillManager.ResetUseSkill();

        Debug.LogError(name + " EndTurn");

        CheckGrid.Instance.ColorGrid(x: 0, clear: true);

        if (GetComponent<ToolTipType>() != null)
            GetComponent<ToolTipType>().AttToltip();

        if (myTurn)
        {
            ActivePassive(Passive.EndTurn,gameObject);

            moveController.StartWalkTurn(false);

            if (dbuff != null /*&& dbuff.enabled*/)
            {
                dbuff.CooldownDbuffFire();

                dbuff.CooldownDbuffPoison();

                dbuff.CooldownDbuffPetrify();

                dbuff.CooldownDbuffStun();

                dbuff.CooldownDbuffBleed();
            }

            UpdateCountKill();

            myTurn     = false;
            attackTurn = false;
            walkTurn   = false;

            PassiveSkills();

            GetComponent<EnemyAttack>().useSkill = false;
            GetComponent<EnemyAttack>().TargetAttack(false);

            countKill = 0;

            if (isPlayer)
            {
                if (RespawMob.Instance.Player == gameObject)
                    GameManagerScenes._gms.TotalTurnos = (1);

                Debug.LogWarning(this.name + " Finalizou seu turno");

                //TurnSystem.Instance.EndTurn();

                ButtonManager.Instance.ClearHUD(true);
            }

                StartCoroutine(EndTurnAi());
        }
    }

    IEnumerator EndTurnAi()
    {
        Debug.LogError(name + " EndTurnAi");

        yield return new WaitForSeconds(1f);

        Debug.LogError(this.name + " Finalizou seu turno");

        TurnSystem.Instance.EndTurn();

        ToolTip.Instance.AttTooltip();
    }

    void PassiveSkills()
    {
        if (passive==null)
        {
            passive = GetComponent<PassiveManager>();
        }


        //GameManagerScenes._gms.NewInfo("PassiveSkills - "+name,3,true);

        if (GetComponent<SkillManager>())
            GetComponent<SkillManager>().CheckAreaDamage();
    }

    public void ActivePassive(Passive _passive, GameObject gO, float value)
    {
        Debug.LogError("ActivePassive:("+_passive+")");

        if (passive == null)
            passive = GetComponent<PassiveManager>();

        if (passive!=null)
            passive.StartPassive(gO, value, _passive);

        if (skillManager!=null)
            skillManager.ActivePassive(_passive,gO, value);
    }
    public void ActivePassive(Passive _passive, GameObject gO)
    {
        Debug.LogError("ActivePassive:(" + _passive + ")");

        if (passive == null)
            passive = GetComponent<PassiveManager>();

        if (passive != null)
            passive.StartPassive(gO, _passive);

        if (skillManager != null)
            skillManager.ActivePassive(_passive, gO);
    }

    public bool Alive
    {
        get
        {
            if (GetComponent<MobHealth>())
            {
                return GetComponent<MobHealth>().Alive;
            }
            else
                return true ;
        }
    }
}
