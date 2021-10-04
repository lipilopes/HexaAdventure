using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealth : MonoBehaviour
{
    PlayerGUI playerGui;
    EffectManager effect;
    MobManager mobManager;
    //MoveController moveController;

    [SerializeField]
    private float health;
    public float Health { get { return health; } set { health = value; } }
    [SerializeField]
    private float maxHealth;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
    
    [Header("Prop's")]
    [SerializeField, Tooltip("Pode Levar Dano Critico")]
    protected bool takeCriticalDamage = true;
    public    bool TakeCriticalDamage { get { return takeCriticalDamage; } }
    [SerializeField,Range(0, 100), Tooltip("Resistencia a Dano Critico")]
    protected float criticalDamageResistence = 0;
    [SerializeField,Range(0, 100), Tooltip("Bonus de Resistencia a Dano Critico")]
    public    float bonusCriticalDamageResistence = 0;
    /// <summary>
    /// Total de Resistencia Critica
    /// </summary>
    public float CriticalDamageResistence
    {
        get { return criticalDamageResistence + bonusCriticalDamageResistence; }
    }
    [Space]
    [SerializeField,Range(0,100),Tooltip("Precisa de % de vida abaixo para fugir")]
    float needToFuga;
    [Tooltip("Resistencia ao dano")]
    public float defense;
    [HideInInspector]
    public float maxDefense;
    [Tooltip("Reduz ou aumenta Dano")]
    public float armor;
    [Tooltip("Esquivar de Danos"),Range(0f,1f)]
    public float dodge=0;

    GameManagerScenes _gms;

    GameObject LastDamage;

    bool criation = false;

    public bool Alive
    {
        get { return Health > 0; }
    }

    void Start()
    {
        effect         = EffectManager.Instance;
        
        mobManager     = this.GetComponent<MobManager>();

        if (mobManager.isPlayer)
            playerGui = (PlayerGUI)FindObjectOfType(typeof(PlayerGUI));
        //moveController = this.GetComponent<MoveController>();

        _gms = GameManagerScenes._gms;

        if (GameManagerScenes.BattleMode)
              takeCriticalDamage = _gms.BattleModeOptionCriticalDamageActive;

        //playerGui.AttBarHP();
    }

    /// <summary>
    /// Porcentagem da vida atual
    /// </summary>
    /// <param name="_porcent">Porcentagem</param>
    /// <returns>Valor da Porcentagem da vida atual</returns>
    public float CurrentHealthPorcent(float _porcent) { return (Health * _porcent) / 100; }
    /// <summary>
    /// Porcentagem da vida maxima
    /// </summary>
    /// <param name="_porcent">Porcentagem</param>
    /// <returns>Valor da Porcentagem da vida maxima</returns>
    public float MaxHealthPorcent(float _porcent) { return (MaxHealth * _porcent) / 100; }

    /// <summary>
    /// Get Damage, Caso Tenho armadura ja calcula
    /// </summary>
    /// <param name="Who"></param>
    /// <param name="Damage"></param>
    /// <param name="ChanceCritical"></param>
    /// <returns>Se Matou</returns>
    public bool Damage(GameObject Who, float Damage, float ChanceCritical = 0, bool activePassive = true,bool _dodge=true)
    {      
        if (Damage == -99)
        {
            Health = 0;
            CheckHp();
            return true;
        }

        if (armor!=0)
        {
            if (armor > 0)            
                Damage -= armor;           
            else
                Damage += armor;
        }

        if (activePassive)
        {
            if (mobManager != null)
            {
                mobManager.ActivePassive(Passive.BeforeGetDamage, Who, Damage);
            }
        }

        if (_dodge && Dodge())
        {
            if (activePassive)
            {
                if (mobManager != null)
                    mobManager.ActivePassive(Passive.GetDodge, Who);

                Who.GetComponent<MobManager>().ActivePassive(Passive.EnemyDodge, gameObject);
            }
        }
        else
        {
            if (activePassive)
                if (mobManager != null)
                    mobManager.ActivePassive(Passive.DodgeFail, Who);

            GetDamage(Who, Damage, ChanceCritical, activePassive, false);            
        }


        #region Old      
        //string color = null;
        //LastDamage = Who;

        //bool critico = false;

        //if (ChanceCritical > 0 && takeCriticalDamage)
        //{
        //    if (ChanceCritical > 1) { ChanceCritical = ChanceCritical / 100; }

        //    ChanceCritical -= CriticalDamageResistence;

        //    critico = (Random.value) <= ChanceCritical;

        //    if (critico)
        //        Damage = Damage * 2;
        //}

        //#region Defesa
        //if (defense > 0)
        //{
        //    float damage   = Damage;
        //    float totalDef = defense;

        //    Debug.LogError(this.name + " tem " + defense + " / " + maxDefense + " de escudo, Levou dano de " + damage);

        //    Damage -= defense;

        //    defense -= (damage);

        //    if (defense < 0)
        //        Damage = defense / -1;

        //    int calDef = (int)(totalDef - defense);

        //    if (calDef < 0)
        //        calDef /= -1;

        //    if (activePassive)
        //        if (mobManager != null)
        //        mobManager.ActivePassive(Passive.DefenseDamage, Who, calDef);

        //    if (mobManager.isPlayer)
        //    {
        //        if (ButtonManager.Instance.player == gameObject)
        //            if (_gms != null)
        //                _gms.TotalDanoDefendido = (calDef);
        //    }
        //    Debug.LogWarning(this.name + " defendeu " + calDef + " de dano");
        //}
        //#endregion

        //if (defense <= 0 && Damage > 0)
        //{
        //    if (critico)
        //    {
        //        #region Critical
        //        effect.CriticalHitEffect(gameObject);               

        //        Health -= Damage;                              

        //        color = "<color=magenta>"/*+"<b>CRITICO</b>  "*/;

        //        Debug.LogError(this.name + " Recebeu dano critico de " + Damage + " do " + Who);

        //        if (mobManager.isPlayer)
        //            iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("y", 0.5f, "x", 0.5f, "time", 0.4f, "easetype", iTween.EaseType.easeOutBounce));
        //        #endregion
        //    }
        //    else
        //    {
        //        #region Normal damage
        //        effect.HitEffect(gameObject);

        //        Health -= Damage;

        //        color = "<color=red>";

        //        Debug.LogError(this.name + " Recebeu Dano de " + Damage);

        //        if (mobManager.isPlayer)
        //            iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("y", 0.2f, "x", 0.5f, "time", 0.2f, "easetype", iTween.EaseType.easeOutBounce));
        //        #endregion
        //    }

        //    if (activePassive)
        //    {
        //        if (critico && mobManager != null)
        //            mobManager.ActivePassive(Passive.GetCriticalDamage, Who, Damage);

        //        if (mobManager != null)
        //            mobManager.ActivePassive(Passive.GetDamage, Who, Damage);

        //        //if (Who.GetComponent<MobManager>())
        //        //    Who.GetComponent<MobManager>().ActivePassive(Passive.SetDamageInEnemy, gameObject, Damage);
        //    }
        //    //Passiva
        //    if (Who.GetComponent<MobManager>())
        //        Who.GetComponent<MobManager>().ActivePassive(Passive.SetDamageInEnemy, gameObject, Damage);
        //}
        //else
        //{
        //    effect.HitBlockEffect(gameObject);

        //    color = null;

        //    effect.PopUpDamageEffect("<color=#A4A4A4><b>BLOCK</b></color>", gameObject);
        //}

        //CheckFuga();

        //if (_gms != null)
        //{
        //    if (ButtonManager.Instance.player == gameObject)
        //        _gms.TotalDanoRecebido = ((int)Damage);
        //    //else
        //    if (ButtonManager.Instance.player == Who)
        //    {
        //        _gms.TotalDanoCausado = ((int)Damage);

        //        if (critico)
        //            _gms.CheckAchievement(16, 1, true);
        //    }
        //}

        //if (color != null && gameObject.activeInHierarchy)
        //    effect.PopUpDamageEffect(color + Damage.ToString("F0") + "</color>", gameObject);

        //if (defense <= 0)
        //    effect.DefeseReset(gameObject);

        //if (mobManager.isPlayer)
        //{
        //    if (playerGui != null)
        //        playerGui.AttBarHP();

        //    print(ButtonManager.Instance.Icons.Count);

        //    if (ButtonManager.Instance.Icons.Count >= 5)
        //    {
        //        ButtonManager.Instance.FindIcon(5, defense > 0, defense.ToString("F0")).GetComponent<ToolTipType>()._descricao =
        //            "Efeito <b>Defesa</b>,\n" +
        //            "<b>" + GetComponent<ToolTipType>()._name + "</b> tem <color=blue><b>" + (int)defense + "</b></color> de vida extra.";

        //        ToolTip.Instance.AttTooltip(ButtonManager.Instance.Icons[5]);
        //    }
        //}       
        #endregion

        return CheckHp();
    }

    /// <summary>
    ///  Caso Tenha escudo dano não pode ser defendido
    /// </summary>
    /// <param name="Who"></param>
    /// <param name="Damage"></param>
    /// <param name="ChanceCritical"></param>
    /// <returns>Se Matou</returns>
    public bool RealDamage(GameObject Who, float Damage, float ChanceCritical = 0,bool activePassive=true, bool _dodge = true)
    { 
        if (Damage == -99)
        {           
            Health = 0;
            CheckHp();
            return true;
        }

        if (activePassive)
        {
            if (mobManager != null)
                mobManager.ActivePassive(Passive.BeforeGetDamage, Who, Damage);
        }

        if (_dodge && Dodge())
        {           
            if (activePassive)
            {
                if (mobManager != null)
                    mobManager.ActivePassive(Passive.GetDodge, Who);

                Who.GetComponent<MobManager>().ActivePassive(Passive.EnemyDodge, gameObject);
            }
        }
        else
        {
            if (activePassive)
                if (mobManager != null)
                    mobManager.ActivePassive(Passive.DodgeFail, Who);

            GetDamage(Who, Damage, ChanceCritical, activePassive, true);           
         }

        #region Old
        //string color = null;

        //LastDamage = Who;

        //bool critico = false;

        //if (ChanceCritical > 0 && takeCriticalDamage)
        //{
        //    if (ChanceCritical > 1) { ChanceCritical = ChanceCritical / 100; }

        //    ChanceCritical -= CriticalDamageResistence;

        //    critico = Random.value <= ChanceCritical;

        //    if (critico)
        //        Damage = Damage * 2;           
        //}

        //if (Damage > 0)
        //{
        //    if (critico)
        //    {
        //        #region Critical
        //        effect.CriticalHitEffect(gameObject);            

        //        Health -= Damage;                

        //        color = "<color=magenta>"/*+"<b>CRITICO</b>  "*/;

        //        Debug.LogError(this.name + " Recebeu dano critico real de " + Damage + " do " + Who);

        //        if (mobManager.isPlayer)
        //            iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("y", 0.5f, "x", 0.5f, "time", 0.4f, "easetype", iTween.EaseType.easeOutBounce));
        //        #endregion
        //    }
        //    else
        //    {
        //        #region Normal damage
        //        effect.HitEffect(gameObject);

        //        Health -= Damage;

        //        color = "<color=yellow>";

        //        Debug.LogError(this.name + " Recebeu Dano Real de " + Damage);

        //        if (mobManager.isPlayer)
        //            iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("y", 0.2f, "x", 0.5f, "time", 0.2f, "easetype", iTween.EaseType.easeOutBounce));
        //        #endregion
        //    }

        //    if (activePassive)
        //    {
        //        if (critico && mobManager != null)
        //            mobManager.ActivePassive(Passive.GetCriticalDamage, Who, Damage);

        //        if (mobManager != null)
        //            mobManager.ActivePassive(Passive.GetRealDamage, Who, Damage);

        //        //if (Who.GetComponent<MobManager>())
        //        //    Who.GetComponent<MobManager>().ActivePassive(Passive.SetDamageInEnemy, gameObject, Damage);
        //    }
        //    //Passiva
        //    if (Who.GetComponent<MobManager>())
        //        Who.GetComponent<MobManager>().ActivePassive(Passive.SetDamageInEnemy, gameObject, Damage);
        //}
        //else
        //{
        //    effect.HitBlockEffect(gameObject);

        //    color = null;

        //    effect.PopUpDamageEffect("<color=#A4A4A4><b>BLOCK</b></color>", gameObject);
        //}

        //CheckFuga();

        //if (_gms != null)
        //{
        //    if (ButtonManager.Instance.player == gameObject)
        //        _gms.TotalDanoRecebido = ((int)Damage);
        //    //else
        //    if (ButtonManager.Instance.player == Who)
        //    { 
        //        _gms.TotalDanoCausado = ((int)Damage);

        //        if (critico)
        //            _gms.CheckAchievement(16, 1, true);
        //    }
        //}

        //if (color != null && gameObject.activeInHierarchy)
        //    effect.PopUpDamageEffect(color + Damage.ToString("F0") + "</color>", gameObject);

        //if (defense <= 0)
        //    effect.DefeseReset(gameObject);

        //if (mobManager.isPlayer)
        //{
        //    if (playerGui != null)
        //        playerGui.AttBarHP();

        //    print(ButtonManager.Instance.Icons.Count);

        //    if (ButtonManager.Instance.Icons.Count >= 5)
        //    {
        //        ButtonManager.Instance.FindIcon(5, defense > 0, defense.ToString("F0")).GetComponent<ToolTipType>()._descricao =
        //            "Efeito <b>Defesa</b>,\n" +
        //            "<b>" + GetComponent<ToolTipType>()._name + "</b> tem <color=blue><b>" + (int)defense + "</b></color> de vida extra.";

        //        ToolTip.Instance.AttTooltip(ButtonManager.Instance.Icons[5]);
        //    }
        //}
        #endregion

        return CheckHp();
    }
    
    /// <summary>
    ///  Recomendado usar  somente nos dBuffs
    /// </summary>
    /// <param name="Who"></param>
    /// <param name="Damage"></param>
    /// <param name="ChanceCritical"></param>
    /// <returns>Se Matou</returns>
    public float GetDamage(GameObject Who, float _damage,float _chanceCritical,bool _activePassive,bool _realDamage)
    {
        string color = null;
        LastDamage = Who;

        bool critico = false;

        if (_chanceCritical > 0 && takeCriticalDamage)
        {
            if (_chanceCritical > 1) { _chanceCritical = _chanceCritical / 100; }

            _chanceCritical -= CriticalDamageResistence;

            critico = (Random.value) <= _chanceCritical;

            if (critico)
                _damage = _damage * 2;
        }

        #region Defesa
        if (defense > 0 && !_realDamage)
        {
            float damage = _damage;
            float totalDef = defense;

            Debug.LogError(this.name + " tem " + defense + " / " + maxDefense + " de escudo, Levou dano de " + damage);

            _damage -= defense;

            defense -= (damage);

            if (defense < 0)
                _damage = defense / -1;

            int calDef = (int)(totalDef - defense);

            if (calDef < 0)
                calDef /= -1;

            if (_activePassive)
                if (mobManager != null)
                    mobManager.ActivePassive(Passive.DefenseDamage, Who, calDef);

            if (mobManager.isPlayer)
            {
                if (ButtonManager.Instance.player == gameObject)
                    if (_gms != null)
                        _gms.TotalDanoDefendido = (calDef);
            }
            Debug.LogWarning(this.name + " defendeu " + calDef + " de dano");
        }
        #endregion

        if (defense <= 0 && _damage > 0 ||
            _damage > 0 && _realDamage)
        {
            if (critico)
            #region Critical
            {
                effect.CriticalHitEffect(gameObject);

                    Health -= _damage;

                    color = "<color=magenta>"/*+"<b>CRITICO</b>  "*/;

                    Debug.LogError(this.name + " Recebeu dano critico real de " + _damage + " do " + Who);

                    if (mobManager.isPlayer)
                        iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("y", 0.5f, "x", 0.5f, "time", 0.4f, "easetype", iTween.EaseType.easeOutBounce));
                   
            }
            #endregion
            else
            if (!_realDamage)
            #region Normal Damage
            {
                    #region Normal damage
                    effect.HitEffect(gameObject);

                    Health -= _damage;

                    color = "<color=red>";

                    Debug.LogError(this.name + " Recebeu Dano de " + _damage);

                    if (mobManager.isPlayer)
                        iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("y", 0.2f, "x", 0.5f, "time", 0.2f, "easetype", iTween.EaseType.easeOutBounce));
                    #endregion
            }
            #endregion
            else
            #region Real Damage
            {
                    #region Real damage
                    effect.HitEffect(gameObject);

                    Health -= _damage;

                    color = "<color=yellow>";

                    Debug.LogError(this.name + " Recebeu Dano Real de " + _damage);

                    if (mobManager.isPlayer)
                        iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("y", 0.2f, "x", 0.5f, "time", 0.2f, "easetype", iTween.EaseType.easeOutBounce));
                    #endregion            
            }
            #endregion

            if (_activePassive)
            {
                if (critico && mobManager != null)
                    mobManager.ActivePassive(Passive.GetCriticalDamage, Who, _damage);

                if (mobManager != null)
                {
                    mobManager.ActivePassive(
                        _realDamage ? Passive.GetRealDamage : Passive.GetDamage
                        , Who, _damage);

                    mobManager.ActivePassive(Passive.EnemyHit, Who, _damage);
                }
            }
            //Passiva
            if (Who.GetComponent<MobManager>())
                Who.GetComponent<MobManager>().ActivePassive(Passive.SetDamageInEnemy, gameObject, _damage);
            //Passiva
            if (critico && Who.GetComponent<MobManager>())
                Who.GetComponent<MobManager>().ActivePassive(Passive.SetCriticalDamage, gameObject, _damage);
        }
        else
        {
            effect.HitBlockEffect(gameObject);

            color = null;

            effect.PopUpDamageEffect("<color=#A4A4A4><b>BLOCK</b></color>", gameObject);
        }

        if (_damage > 0)
        {

        }

        CheckFuga();

        if (_gms != null)
        {
            if (ButtonManager.Instance.player == gameObject)
                _gms.TotalDanoRecebido = ((int)_damage);
            //else
            if (ButtonManager.Instance.player == Who)
            {
                _gms.TotalDanoCausado = ((int)_damage);

                if (critico)
                    _gms.CheckAchievement(16, 1, true);
            }
        }

        if (color != null && gameObject.activeInHierarchy)
            effect.PopUpDamageEffect(color + _damage.ToString("F0") + "</color>", gameObject);

        if (defense <= 0)
            effect.DefeseReset(gameObject);

        if (mobManager.isPlayer)
        {
            if (playerGui != null)
                playerGui.AttBarHP();

            print(ButtonManager.Instance.Icons.Count);

            if (ButtonManager.Instance.Icons.Count >= 5)
            {
                ButtonManager.Instance.FindIcon(5, defense > 0, defense.ToString("F0")).GetComponent<ToolTipType>()._descricao =
                    "Efeito <b>Defesa</b>,\n" +
                    "<b>" + GetComponent<ToolTipType>()._name + "</b> tem <color=blue><b>" + (int)defense + "</b></color> de vida extra.";

                ToolTip.Instance.AttTooltip(ButtonManager.Instance.Icons[5]);
            }
        }

        return _damage;
    }

    public void HitKill(GameObject Who, bool popUp = true)
    {
        LastDamage = Who;

        Health = 0;

        defense = 0;

        if (effect != null)
        {
            effect.CriticalHitEffect(gameObject);

            if (defense <= 0)
                effect.DefeseReset(gameObject);

            if (popUp)
                effect.PopUpDamageEffect("<color=#red><b>HIT KILL</b></color>", gameObject);
        }

        if (mobManager.isPlayer)
            playerGui.AttBarHP();

        CheckHp(false);
    }

    public bool RecHp(GameObject Who, float Value)
    {
        if (Health >= MaxHealth)
        {
            effect.PopUpDamageEffect("<color=#F2F2F2>Hp Max</color>", gameObject);
            return false;
        }

        effect.RecHpEffect(gameObject);

        if (Health + Value > MaxHealth)
        {
            Value = MaxHealth - Health;
        }

        Health += Value;

        if (mobManager != null)
            mobManager.ActivePassive(Passive.RestoreHp, Who, Value);

        if (RespawMob.Instance.Player == gameObject)
            _gms.TotalVidaRecuperada = ((int)Value);

        effect.PopUpDamageEffect("<color=#01DF01> +" + Value.ToString("F0") + "</color>", gameObject);

        if (mobManager.isPlayer && playerGui!=null)
            playerGui.AttBarHP();

        return true;
    }

    public bool CheckHp(bool countKill = true)
    {
        if (ToolTip.Instance)
            ToolTip.Instance.AttTooltip(gameObject);

        if (Health <= 0)
        {
            

            if (LastDamage != null)
            {
                MobManager Mm = LastDamage.GetComponent<MobManager>();

                if (Mm && countKill && tag == "Mob")
                {
                    Mm.CountKill++;
                }

                if (Mm != null)
                    Mm.ActivePassive(Passive.KillEnemy, gameObject);

                if (LastDamage == RespawMob.Instance.Player && tag == "Mob")
                    _gms.TotalMorteMob = (1);
            }
            mobManager.ActivePassive(Passive.Kill, gameObject);

            if (effect != null)
                effect.DeadEffect(gameObject);

            if (TurnSystem.Instance)
            {
                TurnSystem.Instance.Clear(gameObject);

                TurnSystem.Instance.GetComponent<CheckGrid>().Check();
            }

            GridMap gridMap = GridMap.Instance;
            gridMap.FreeHex(gameObject);       

            if (GetComponent<SkillManager>())
                GetComponent<SkillManager>().DesactiveAreaDamage();

            if (GetComponent<MobDbuff>())
                GetComponent<MobDbuff>().ClearDbuff();            

            if (RespawMob.Instance && gameObject == RespawMob.Instance.Player)
            {
                _gms.TotalGameOver = (1);
                effect.PopUpDamageEffect("<color=#8A0808>Game Over!</color>", gameObject);
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false;

                TurnSystem.Instance.GameOver();
                return true;
            }
            else
            {
                //transform.position = new Vector3(0,-10,0);
                gameObject.SetActive(false);

                if (GetComponent<MoveController>() && GetComponent<MoveController>().Solo !=null)
                GetComponent<MoveController>().Solo.CurrentMobDead();
            }

            if (mobManager.myTurn)
                mobManager.EndTurn();

            return true;
        }
        else
            return false;
    }

    public void StartHealth(float _health)
    {
        MaxHealth = _health;

        Health = MaxHealth;

        if (!criation)        
            StartCoroutine(ICriation());      
    }

    public bool Defense(float Def, GameObject Who)
    {
        if (defense >= Def)
            return false;

        Debug.LogWarning(name + " Ganhou " + Def + " de Defense");

        if (mobManager.isPlayer)
        {
            ButtonManager buttonManager = ButtonManager.Instance;

            if (buttonManager != null && buttonManager.Icons.Count >= 5)
            {
                buttonManager.FindIcon(5, true, Def.ToString("F0")).GetComponent<ToolTipType>()._descricao =
                    "Efeito <b>Defesa</b>,\n" +
                    "<b>" + GetComponent<ToolTipType>()._name + "</b> tem <color=blue><b>" + (int)Def + "</b></color> de vida extra.";
            }

            if (playerGui != null)
                playerGui.AttBarHP();
        }
        else
        {
            string color = "<color=yellow>";

            if (mobManager.MesmoTime(RespawMob.Instance.PlayerTime))
                color = "<color=blue>";

            GetComponent<MobManager>().infoTable.NewInfo(color + "<b>" + GetComponent<ToolTipType>()._name + "</b> \n Ganhou <b>" + (int)Def + "</b> \n de <b>Defesa</b></color>", 7);
            GetComponent<MobManager>().infoTable.NewInfo(color + "Efeito <b>Defesa</b>, \n <b>" + GetComponent<ToolTipType>()._name + "</b> tem <b>" + (int)Def + "</b> de vida extra.</color>", 7);
        }

        if (mobManager != null)
            mobManager.ActivePassive(Passive.GetDefense, Who, defense);

        effect.HitBlockEffect(gameObject);
        effect.DefeseEffect(gameObject);

        defense = Def;
        maxDefense = defense;

        ToolTip.Instance.AttTooltip(gameObject);
        effect.PopUpDamageEffect("<color=blue>" + Def + "</color>", gameObject);

        if (playerGui != null)
            if (mobManager.isPlayer)
                playerGui.AttBarHP();

        return true;
    }

    public bool Armor(float _armor, GameObject Who)
    {
        string detal = "Ganhou";

        if (_armor<0)
        {
            armor -= _armor;
            detal = "Perdeu";
        }
        else
        if (armor >= _armor)
            return false;

        Debug.LogWarning(name + " "+ detal + " " + _armor + " de Armadura");
        
        if (_armor > 0)
        {
            if (mobManager.isPlayer)
            {
                ButtonManager buttonManager = ButtonManager.Instance;

                if (buttonManager != null && buttonManager.Icons.Count >= 5)
                {
                    buttonManager.FindIcon(5, true, _armor.ToString("F0")).GetComponent<ToolTipType>()._descricao =
                        "Efeito <b>Armadura</b>,\n" +
                        "<b>" + GetComponent<ToolTipType>()._name + "</b> tem <color=black><b>" + (int)_armor + "</b></color> de redução de dano.";
                }

                if (playerGui != null)
                    playerGui.AttBarHP();
            }
            else
            {
                string color = "<color=yellow>";

                if (mobManager.MesmoTime(RespawMob.Instance.PlayerTime))
                    color = "<color=black>";

                GetComponent<MobManager>().infoTable.NewInfo(color + "<b>" + GetComponent<ToolTipType>()._name + "</b> \n " + detal + " <b>" + (int)_armor + "</b> \n de <b>Armadura</b></color>", 7);
                GetComponent<MobManager>().infoTable.NewInfo(color + "Efeito <b>Armadura</b>, \n <b>" + GetComponent<ToolTipType>()._name + "</b> tem <b>" + (int)_armor + "</b> de redução de dano.</color>", 7);
            }

            if (mobManager != null)
                mobManager.ActivePassive(Passive.GetArmor, Who, _armor);

            effect.HitBlockEffect(gameObject);
            effect.DefeseEffect(gameObject);
        }
        else
            effect.DefeseReset(gameObject);

        armor = _armor;

        ToolTip.Instance.AttTooltip(gameObject);
        effect.PopUpDamageEffect("<color=black>" + _armor + "</color>", gameObject);

        /*if (playerGui != null)
            if (mobManager.isPlayer)
                playerGui.AttBarHP();*/

        return true;
    }

    protected bool Dodge(float _dodgeBonus=0,bool showPopUp=true)
    {
        float _d = (dodge > 1 ? (dodge / 100) : dodge);

        bool _r = (Random.value) <= _d + _dodgeBonus;

        if (_r)
        effect.PopUpDamageEffect("<color=white><b>Dodge</b></color>", gameObject);

        return _r;
    }

    public void ReBorn(bool attTurn=true)
    {
        Debug.LogError(gameObject.name+" Reborn");

        if (MaxHealth != 0)
        {
            StartHealth(MaxHealth);

            //GetComponent<MeshRenderer>().enabled = true;
            //GetComponent<BoxCollider>().enabled  = true;
            gameObject.SetActive(true);

            if (attTurn)
            {
                TurnSystem respaw = TurnSystem.Instance;

                respaw.RegisterTurn(gameObject, true);
            }
        }
        else
        {
            if (_gms!=null)
            {
                _gms.NewInfo("ReBorn Fail Health is zero",3,true);
            }
        }
    }

    void CheckFuga()
    {
        if (needToFuga > 0 && Alive)
        GetComponent<MoveController>().FugaActive(Health <= ((Health * needToFuga) / 100));
    }

    IEnumerator ICriation()
    {
        yield return new WaitForSeconds(3);

        mobManager.ActivePassive(Passive.Criation, gameObject);

        criation = true;
    }
}
