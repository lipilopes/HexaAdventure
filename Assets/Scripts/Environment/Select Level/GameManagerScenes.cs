using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

#region Skins
[Serializable]
public enum SkinType
{
    /// <summary>Skin ID 0, Não tem nada de mais</summary> /// 
    Basica,
    /// <summary>Skin que muda o Material</summary> /// 
    Normal,
    /// <summary>Skin que muda o Material / Efeitos das skills</summary> ///
    Rara,
    /// <summary>Skin que muda o Material / Efeitos das skills / Modelo diferente / Name Skills diferentes</summary> ///
    Lendaria,
    /// <summary>Mesma coisa do Lendario, so pode pegar em epocas especificas</summary> ///
    Limitada
}

[Serializable]
public enum SkinTag
{
    //[Tooltip("Nada")]
    ///// <summary>Nada</summary> /// 
    //Null,
    [Tooltip("Tem Efeitos Alterados")]
    /// <summary>Tem Efeitos Alterados</summary> ///
    Effect,
    [Tooltip("Skills Diferentes (Modelo do efeito e tals)")]
    /// <summary>Skills Diferentes (Modelo do efeito e tals)</summary> /// 
    Skill,
    [Tooltip("Skills tem nome alterado")]
    /// <summary>Skills tem nome alterado</summary> ///
    Name_Skill,
    [Tooltip("Skin Modelo diferente")]
    /// <summary>Skin Modelo diferente</summary> ///
    Model,
    [Tooltip("Arma do Mob alterada")]
    /// <summary>Arma do Mob alterada</summary> ///
    Arma
}

[Serializable]
public class SkinSystem
{
    [Tooltip("Nome da Skin")]
    public string     _nameSkin    = "Padrão";
    [Tooltip("Skin Bloqueada")]
    public bool       _skinBlocked = true;
    [Tooltip("Prefab da Skin")]
    public GameObject _skinPrefab;
    [Tooltip("Modelo da Skin")]
    public Mesh       _skinMesh;
    [Tooltip("Materiais da Skin")]
    public Material[] _skinMaterials = { null };
    [Space]
    [Tooltip("Foto de Perfil da Skin")]
    public Sprite _spritePerfil     /*= GameManagerScenes._gms._defaultSpritePerfil*/;
    [Tooltip("Foto da classe da Skin")]
    public Sprite _spriteIconClasse /*= GameManagerScenes._gms._defaultSpriteClass*/;
    [Tooltip("Foto de chat da Skin")]
    public Sprite _spritePerfilChat /*= GameManagerScenes._gms._defaultSpriteChat*/;
    [Space]
    [Tooltip("Raridade da Skin")]
    public SkinType _skinType = SkinType.Basica;
    [Tooltip("Tag da skin")]
    public SkinTag[]  _skinTag = {/* SkinTag.Null*/ };
}
#endregion

#region Hero
[Serializable]
public class HerosBlocked
{
    [Tooltip("ID Hero"), Range(1, 99)]
    public int _heroID;

    [Tooltip("Game Mode")]
    public bool _AllGamesMode = false;

    [Tooltip("Game Mode")]
    public Game_Mode _mode = Game_Mode.History;
}

[Serializable]
public class HeroFase
{
    [Tooltip("Nome do Heroi")]
    public string     _nameHero;
    [Tooltip("Hero Bloqueado")]
    public bool       _blocked=true;
    [Tooltip("Prefab do Heroi")]
    public GameObject _prefabHero;
    [Space]
    [Header("Player")]
    [Tooltip("Classe")]
    public MobManager.Classe _classe;
    [Tooltip("Dano")]
    public int   _damage;
    [Tooltip("Hp")]
    public float _health;
    [Header("Valor Indivitual")]
    [Range(1.1f,1.2f),ContextMenuItem("Reset", "ContextResetI"),ContextMenuItem("Teste", "ContextTesteDamageMobs"),ContextMenuItem("CalculeAll", "ContextCalculeAll")]
    public float _Idamage;
    [Range(1.1f,1.25f),ContextMenuItem("Reset", "ContextResetI"), ContextMenuItem("Teste", "ContextTesteHealthMobs")]
    public float _Ihealth;
    //[Range(0, 4),Tooltip("")]
    //public float _Iwalk;
    [Space]
    [Tooltip("Passos por turno")]
    public int _walk;
    [Tooltip("Ataques por turno")]
    public int _attack = 1;
    [Space]
    [Range(0,1),Tooltip("Chance Critico")]
    public float _critico;
    [Space]
    [Range(0, 1),Tooltip("Resistencia")]
    public float _damageResistenceFire;
    [Range(0, 1),Tooltip("Resistencia")]
    public float _damageResistencePoison;
    [Range(0, 1),Tooltip("Resistencia")]
    public float _damageResistencePetrify;
    [Range(0, 1),Tooltip("Resistencia")]
    public float _damageResistenceBleed;

    [Space]
    [Tooltip("Status Mob Ia Dificuldade: 0-Easy 1-Medio 2-Dificil 3-Modo Batalha")]
    public List<StatusIaMob> _statusIaMob = new List<StatusIaMob>();

    [Space][Tooltip("Outros modelos/Skins")]
    public List<SkinSystem> _skinHero = new List<SkinSystem>();
    [Space][Tooltip("Fases do heroi")]
    public List<Fases> _Fases;
}

[Serializable]
public class StatusIaMob
{
    //[Tooltip("Classe")]
    //public MobManager.Classe _classe;
    [Tooltip("Dano Minimo")]
    public int _damageMin;
    [Tooltip("Dano Maximo")]
    public int _damageMax;
    [Space]
    [Tooltip("Hp Minimo")]
    public float _healthMin;
    [Tooltip("Hp Max")]
    public float _healthMax;
    [Space]
    [Tooltip("Passos por turno")]
    public int _walk   = 1;
    [Tooltip("Ataques por turno")]
    public int _attack = 1;
    [Space]
    [Range(0, 1), Tooltip("Chance Critico")]
    public float _critico;
    [Space]
    [Range(0, 1), Tooltip("Resistencia Fire")]
    public float _damageResistenceFire;
    [Range(0, 1), Tooltip("Resistencia Poison")]
    public float _damageResistencePoison;
    [Range(0, 1), Tooltip("Resistencia Petrify")]
    public float _damageResistencePetrify;
    [Range(0, 1), Tooltip("Resistencia Bleed")]
    public float _damageResistenceBleed;
}

[Serializable]
public class Fase
{
    [Tooltip("Nome da fase")]
    public string _fase;            // Nome da Fase
    [Space]
    [Tooltip("Tempo Record min")]
    public float _min = -1;
    [Tooltip("Tempo Record seg")]
    public float _seg = -1;
    [Space]
    [Tooltip("Clip Music")]
    public AudioClip _clip;
    [Tooltip("Transition Clip Music")]
    public AudioClip _clipTransicao;
    [Space]
    [Tooltip("Fase não esta bloqueada")]
    public bool   _dontBlock;       // Fase block
    [Tooltip("Fase já foi completada")]
    public bool   _complete;        // Fase Completa
    [Tooltip("Fase foi completada por kill")]
    public bool   _completeKill;    // Fase Completa por matar todos [bonus em resistence(Fire,poison)]
    [Tooltip("Fase foi completada apos entrar no PORTAL")]
    //public bool?   _completePortal;  // Fase Completa por chegar no portal futuros bonus [bonus em resistence(Petri,Bleed)]
    [Range(-1,1)]
    public int     _completePortal;  // Fase Completa por chegar no portal futuros bonus [bonus em resistence(Petri,Bleed)]
}

[Serializable]
public class Fases
{
    public List<Fase> Fase;
}
#endregion

#region Battle
[Serializable]
public class BattleModeGamePlay
{
    public List<BattleModeMob> _battleModeMobs = new List<BattleModeMob>();
}

[Serializable]
public class BattleModeMob
{
    public int  _Time     = 0;
    public bool _isPlayer = false;
    [Space]
    public int _idMob     = -1;
    public int _idMobSkin = -1;
    [Space]
    public int _XRespaw = -1;
    public int _YRespaw = -1;
}
#endregion

[Serializable]
public class Achievement
{
    [Header("Informacao")]
    [Tooltip("Nome")]
    public string _name = "null";            // Nome da Fase
    [Tooltip("Descrição Coloque X% para ser trocado pelo valor maximo"), TextArea]
    public string _descricao;
    [Tooltip("Icon")]
    public Sprite _icon;
    [Tooltip("Audio")]
    public AudioClip _audio;
    [Space]
    [Header("Progress")]
    [Tooltip("Feito")]
    public float _feito=0;
    [Tooltip("Total")]
    public float _max=1;
    [Space]
    [Header("Other")]
    [Tooltip("esta bloqueada")]
    public bool   _complete=false;             // block
    [Tooltip("Secreta nao aparece descrição")]
    public bool   _secret = true;        // Fase Completa
    [Tooltip("Tipo da Conquista")]
    public _Type _type;
    [Tooltip("DLC")]
    public _DLC _dlc =0;
    [Tooltip("O que Ganha quando completa")]
    public UnityEvent _bonus;

    public enum _Type
    {
        Bronze,
        Prata,
        Ouro,
        Platina,
    };

    public enum _DLC
    {
        O_resgate_da_honra,
        Missões,
        Aventuras_de_outro_mundo
    };
}

[Serializable]
public enum Skill_Type
{
    /// <summary>
    /// Dano Magico: Geralmente a Longa Distancia
    /// </summary>
  Magic_Dmg,
    /// <summary>
    /// Dano Fisico: Geralmente Faz Contato com o inimigo
    /// </summary>
    Fisic_Dmg,
    /// <summary>
    /// Special: Não contém dano, puramente Buff ou Dbuff ou Criar mobs
    /// </summary>
    Special
}

[Serializable]
public enum Game_Mode
{
   History,
   Battle,
   Moba
}

[Serializable]
public enum Game_language
{
   PT_BR,
   EN_US
}

public class GameManagerScenes : MonoBehaviour
{
    public static GameManagerScenes _gms;

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
    public bool Dbuff(GameObject User,int currentdamage,GameObject targetDbuff, Dbuff Buff, bool forMe, float Chance, int MinDuration = -1, int MaxDuration = -1, bool Acumule = false, int MaxAcumule = 1, int indexList = -1, int idAcumule=-999)
    {
        if (Chance > 1)
            Chance /= 100;

        bool Active = (UnityEngine.Random.value <= Chance);

        if (!Active)       
            return false;    

        if (MinDuration == -1)
            MinDuration = currentdamage;

        if (MaxDuration == -1)
            MaxDuration = currentdamage;

        int Duration = UnityEngine.Random.Range(MinDuration, MaxDuration + 1);
        int i = indexList;

        if (forMe)
            targetDbuff = User;

        if (!forMe && User == targetDbuff)
            targetDbuff = null;

        Debug.LogError("Check Hit[" + Buff + "] -  Duration[" + Duration + "] / Chance[" + Chance * 100 + "%] no " + targetDbuff);

        if (targetDbuff == null ||
            targetDbuff.GetComponent<MobHealth>() == null ||
            targetDbuff.GetComponent<MobDbuff>() == null ||
            targetDbuff.GetComponent<MoveController>() == null ||
            targetDbuff.GetComponent<MobCooldown>() == null ||
            targetDbuff.GetComponent<MobHealth>() == null)
        {
            //DbuffFail(targetDbuff, i);
            return false;
        }

        switch (Buff)
        {
            #region Fire
            case global::Dbuff.Fire:
                if (targetDbuff.GetComponent<MobDbuff>() != null)
                    if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffFire(User, duration: Duration, _chance: Chance))
                    {
                        //DbuffActive(targetDbuff, i);
                        return true;
                    }
                   /* else
                        DbuffFail(targetDbuff, i);*/
                break;
            #endregion

            #region Poison
            case global::Dbuff.Envenenar:
                if (targetDbuff.GetComponent<MobDbuff>() != null)
                    if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPoison(User, duration: Duration, _chance: Chance))
                    {
                       // DbuffActive(targetDbuff, i);
                        return true;
                    }
                   // else
                      //  DbuffFail(targetDbuff, i);
                break;
            #endregion

            #region Petrify
            case global::Dbuff.Petrificar:
                if (targetDbuff.GetComponent<MobDbuff>() != null)
                    if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffPetrify(User, duration: Duration, _chance: Chance))
                    {
                       // DbuffActive(targetDbuff, i);
                        return true;
                    }
                   // else
                       // DbuffFail(targetDbuff, i);
                break;
            #endregion

            #region Stun
            case global::Dbuff.Stun:
                if (targetDbuff.GetComponent<MobDbuff>() != null)
                    if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffStun(User, duration: Duration, _chance: Chance))
                    {
                      //  DbuffActive(targetDbuff, i);
                        return true;
                    }
                  //  else
                   //     DbuffFail(targetDbuff, i);
                break;
            #endregion

            #region Bleed
            case global::Dbuff.Bleed:
                if (targetDbuff.GetComponent<MobDbuff>() != null)
                    if (targetDbuff.GetComponent<MobDbuff>().AtiveDbuffBleed(User, duration: Duration, _chance: Chance))
                    {
                   //     DbuffActive(targetDbuff, i);
                        return true;
                    }
                  //  else
                    //    DbuffFail(targetDbuff, i);
                break;
            #endregion

            #region Recuar
            case global::Dbuff.Recuar:
                if (targetDbuff.GetComponent<MoveController>())
                {
                    bool _RecuarAtivou = false;
                    for (int buff = 0; buff < Duration; buff++)
                    {
                        if (/*CheckChance(Chance)*/true)
                        {
                            //Debug.LogError(User.name + " Fez o " + targetDbuff.name + " Recuar");
                            targetDbuff.GetComponent<MoveController>().EnemyWalk(User.GetComponent<MoveController>(), true, true, Call: true);
                          //  DbuffActive(targetDbuff, i);
                            _RecuarAtivou = true;
                        }
                        else
                        {
                            //Debug.LogError(User.name + " Falhou em Recuar o " + targetDbuff.name);
                         //   DbuffFail(targetDbuff, i);
                        }
                    }
                    return _RecuarAtivou;
                }
                break;
            #endregion

            #region chamar
            case global::Dbuff.Chamar:
                if (targetDbuff.GetComponent<MoveController>())
                {
                    bool _ChamarAtivou = false;

                    for (int buff = 0; buff < Duration; buff++)
                    {
                        if (/*CheckChance(Chance)*/true)
                        {
                            _ChamarAtivou = true;
                            // Debug.LogError(User.name + " Chamou o " + targetDbuff.name);
                            targetDbuff.GetComponent<MoveController>().EnemyWalk(User.GetComponent<MoveController>(), true, Call: true);
                        //    DbuffActive(targetDbuff, i);
                        }
                        else
                        {
                            //Debug.LogError(User.name + " Falhou em Chamar o " + targetDbuff.name);
                       //     DbuffFail(targetDbuff, i);
                        }
                    }

                    return _ChamarAtivou;
                }
                break;
            #endregion

            #region cooldown
            case global::Dbuff.Cooldown:
                if (targetDbuff.GetComponent<MobCooldown>() && targetDbuff.GetComponent<SkillManager>())
                {
                    if (targetDbuff.GetComponent<SkillManager>().Skills.Count >= MinDuration
                       /*&& CheckChance(Chance)*/)
                    {
                        // Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
                        targetDbuff.GetComponent<MobCooldown>().AttCooldown(MaxDuration, MinDuration);
                      //  DbuffActive(targetDbuff, i);
                        return true;
                    }
                    else
                    {
                        // Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
                      //  DbuffFail(targetDbuff, i);
                    }
                }
                break;
            #endregion

            #region Recupera Hp
            case global::Dbuff.Recupera_HP:
                if (targetDbuff.GetComponent<MobHealth>())
                {
                    if (/*CheckChance(Chance) &&*/ targetDbuff.GetComponent<MobHealth>().RecHp(User, Duration))
                    {
                      //  DbuffActive(targetDbuff, i);
                        return true;
                    }
                  //  else
                       // DbuffFail(targetDbuff, i);
                }
                break;
            #endregion

            #region Escudo
            case global::Dbuff.Escudo:
                if (targetDbuff.GetComponent<MobHealth>())
                {
                    if (/*CheckChance(Chance)*/true)
                    {
                        if (targetDbuff.GetComponent<MobHealth>().defense < Duration || Acumule)
                        {
                            //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                            targetDbuff.GetComponent<MobHealth>().Defense
                                (Acumule
                                ? targetDbuff.GetComponent<MobHealth>().defense + Duration
                                : Duration, User);
                           // DbuffActive(targetDbuff, i);
                            return true;
                        }
                    }

                    //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                  //  DbuffFail(targetDbuff, i);
                }
                break;
            #endregion

            #region Bonus Atk
            case global::Dbuff.Buff_Atk:
                if (targetDbuff.GetComponent<MobDbuff>())
                {
                    if (/*CheckChance(Chance)*/true)
                    {
                        //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                        targetDbuff.GetComponent<MobDbuff>().AtiveBuffDamage
                            (
                            MinDuration, //valor
                            MaxDuration, //tempo,
                            User,
                            idAcumule==-999 ? this.GetInstanceID() : idAcumule,
                            Acumule,//Acumulativo
                            MaxAcumule);
                     //   DbuffActive(targetDbuff, i);
                        return true;
                    }
                    else
                    {
                        //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                        //DbuffFail(targetDbuff, i);
                    }
                }
                break;
            #endregion

            #region Silence
            case global::Dbuff.Silence:
                if (targetDbuff.GetComponent<MobDbuff>())
                {
                    if (true/*CheckChance(Chance) *//*&& targetDbuff.GetComponent<SkillManager>().Skills.Count >= MinDuration*/)
                    {
                        // Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
                        targetDbuff.GetComponent<MobDbuff>().AtiveDbuffSilenceSkill(
                            MaxDuration, //time
                            MinDuration, //index
                            User,
                            idAcumule == -999 ? this.GetInstanceID() : idAcumule,
                            Acumule,//Acumulativo
                            MaxAcumule);
                       // DbuffActive(targetDbuff, i);
                        return true;
                    }
                    else
                    {
                        // Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
                      //  DbuffFail(targetDbuff, i);
                    }
                }
                break;
            #endregion

            #region DbuffAtk
            case global::Dbuff.Dbuff_Atk:
                if (targetDbuff.GetComponent<MobDbuff>())
                {
                    if (/*CheckChance(Chance)*/true)
                    {
                        //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                        targetDbuff.GetComponent<MobDbuff>().AtiveDBuffDamage
                            (
                            MinDuration, //valor
                            MaxDuration, //tempo,
                            User,
                            idAcumule == -999 ? this.GetInstanceID() : idAcumule,
                            Acumule,//Acumulativo
                            MaxAcumule);
                        //DbuffActive(targetDbuff, i);
                        return true;
                    }
                    else
                    {
                        //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                       // DbuffFail(targetDbuff, i);
                    }
                }
                break;
            #endregion

            #region Silence Passive
            case global::Dbuff.SilencePassive:
                if (targetDbuff.GetComponent<MobDbuff>())
                {
                    if (true/*CheckChance(Chance)*/ /*&& targetDbuff.GetComponent<SkillManager>().Skills.Count >= MinDuration*/)
                    {
                        // Debug.LogError(User.name + " Mudou o  Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
                        targetDbuff.GetComponent<MobDbuff>().AtiveDbuffSilencePassive(
                            MaxDuration, //time
                            MinDuration, //index
                            User,
                            idAcumule == -999 ? this.GetInstanceID() : idAcumule,
                            Acumule,//Acumulativo
                            MaxAcumule);
                     //   DbuffActive(targetDbuff, i);
                        return true;
                    }
                    else
                    {
                        // Debug.LogError(User.name + " Falhou em mudar o Cooldown (" + skillManager.Skills[_DbuffBuff[i]._dbuffDuracaoMin].Nome + ") do " + targetDbuff.name + " para " + _DbuffBuff[i]._dbuffDuracaoMax);
                      //  DbuffFail(targetDbuff, i);
                    }
                }
                break;
            #endregion

            #region Escudo
            case global::Dbuff.Armadura:
                if (targetDbuff.GetComponent<MobDbuff>())
                {
                    if (/*CheckChance(Chance)*/true)
                    {
                        //if (targetDbuff.GetComponent<MobHealth>().armor < Duration || Acumule)
                        {
                            //Debug.LogError(User.name + " Deu " + Duration + " de escudo  para o " + targetDbuff.name);
                            targetDbuff.GetComponent<MobDbuff>().AtiveBuffArmor
                            (MinDuration, //valor
                            MaxDuration, //tempo,
                            User,
                            idAcumule == -999 ? this.GetInstanceID() : idAcumule,
                            Acumule,//Acumulativo
                            MaxAcumule);
                            // DbuffActive(targetDbuff, i);
                            return true;
                        }
                    }

                    //Debug.LogError(User.name + " Falhou em dar " + Duration + " de escudo  para o " + targetDbuff.name);
                    //  DbuffFail(targetDbuff, i);
                }
                break;
                #endregion
        }

        return false;
    }


    bool adm = false;

    //[HideInInspector]
    public bool LoadComplete = false;

    public bool Demo = false;
    public int MaxFaseDemo = 5;

    /// <summary>
    /// Ativa funções de Administrador
    /// </summary>
    public bool Adm { get { return adm; } set { adm = value; } }

    /// <summary>
    /// Jogo esta pausado
    /// </summary>
    public bool Paused { get; set; }

    #region Fases
    [Header("Fase"), Tooltip("Fase Atual")]
    public int _atualFase = 0;
    /// <summary>
    /// Dificuldade na Lista de Fases
    /// </summary>
    public int _dlf
    {
        get
        {
            if (_dificuldade == dificuldade.auto || _dificuldade == dificuldade.facil)
            {
                return 0;
            }
            else
                return (int)_dificuldade - 1;
        }
    }

    /// <summary>
    /// Dificuldade na Lista de Fases Para salvamento
    /// </summary>
    public string DlfSave(int d = -1)
    {
        if (d == -1)
            d = _dlf;

        return (d > 0 ? d + "" : "");
    }

    public int _dificuldadeCount { get { return Enum.GetValues(typeof(dificuldade)).Length; } }

    [SerializeField] Game_Mode _gameMode = Game_Mode.History;
    public Game_Mode GameMode { get { return _gameMode; } }
    public Game_Mode GameModeChange(Game_Mode Change)
    {
        if (Change!=_gameMode)
        {
            _gameMode = Change;

            Balance(Adm);
        }

        return _gameMode;
    }
    [Tooltip("Infos Gerais")]
    public List<HeroFase> Mob;

    WaitForSeconds waitChangeSalve = new WaitForSeconds(0f);
    public WaitForSeconds waitDelayBar;

    public int FaseAtual { get { return _atualFase; } set { if (value > -1) _atualFase = value; } }

    public int FaseCount { get { return Mob[PlayerID - 1]._Fases[_dlf].Fase.Count; } }

    public AudioClip ClipFase(int _player, int _fase)
    {
        AudioClip _r = null;

        if (_playerId >= Mob.Count)
            return null;

        int IdPlayer = _player;

        if (IdPlayer == -1)
            IdPlayer = PlayerID;

        if (_fase == -1)
            _fase = UnityEngine.Random.Range(0, Mob[IdPlayer]._Fases[_dlf].Fase.Count);

        if (_fase >= FaseCount)
            return _r;

        _r = Mob[IdPlayer]._Fases[_dlf].Fase[_fase]._clip;

        return _r;
    }

    public AudioClip TransicaoFase(int _player, int _fase)
    {
        AudioClip _r = null;

        if (_playerId >= Mob.Count)
            return null;

        int IdPlayer = _player;

        if (IdPlayer == -1)
            IdPlayer = PlayerID;

        if (_fase >= FaseCount)
            return _r;

        _r = Mob[IdPlayer]._Fases[_dlf].Fase[_fase]._clipTransicao;

        return _r;
    }

    public string NameFase(int fase = -1)
    {
        if (fase == -1)
            return Mob[(PlayerID - 1)]._Fases[_dlf].Fase[_atualFase]._fase;

        if (fase >= FaseCount)
            return "";

        return Mob[(PlayerID - 1)]._Fases[_dlf].Fase[fase]._fase;
    }

    /// <summary>
    /// check valor si nao esta block
    /// </summary>
    /// <param name="fase">fase na contagem normal</param>
    /// <param name="value">1 = true / 0 = false</param>
    /// <returns></returns>
    public bool CheckBlockFase(int fase, int value = -1, int hero = -1)
    {
        if (fase >= FaseCount)
            return false;

        int idHero = hero;

        if (idHero == -1)
            idHero = PlayerID-1;

        if (value != -1)
        {
            string IdPlayer = (idHero).ToString("F0");

            bool _value = Convert.ToBoolean(value);

            Mob[idHero]._Fases[_dlf].Fase[fase]._dontBlock = _value;

            //string Idlf     = (_dlf > 0 ? _dlf + "" : "");
            string nameFase = (DlfSave() + "F" + (fase) + IdPlayer).ToString();

            //if (PlayerPrefs.HasKey(nameFase))
            {
                PlayerPrefs.SetInt(nameFase, Convert.ToInt32(_value));
                Debug.Log("Save [DesBlock Fase" + fase + "(" + (idHero) + ")] Complete");
            }
        }

       // print("CheckBlockFase(FASE["+fase+" - "+NameFase(fase)+"],VALOR["+ value + "],HEROI["+idHero+" - "+HeroName(idHero)+"])");

        return Mob[idHero]._Fases[_dlf].Fase[fase]._dontBlock;
    }

    /// <summary>
    /// Atualiza a fase para complete
    /// </summary>
    /// <param name="fase">fase na contagem normal</param>
    public void CompleteFase(int fase, bool kill, int hero = -1)
    {
        if (fase >= FaseCount)
            return;

        string IdPlayer = "";
        int idHero = hero;

        if (idHero == -1)
            idHero = PlayerID-1;


        if (idHero > 1)
            IdPlayer = (idHero).ToString("F0");

        //string dfl = _dlf > 0 ? _dlf + "" : "";

        print("CheckCompleteFase(" + fase + " , " + kill + " , " + hero + ") - " + idHero + " / idP: " + IdPlayer);

        bool value = Convert.ToBoolean(1);
        string nameFase = (DlfSave() + "F" + (fase) + "Complete" + IdPlayer).ToString();
        string nameCompletePortalOrKill = null;

        Mob[(idHero)]._Fases[_dlf].Fase[fase]._dontBlock = value;
        Mob[(idHero)]._Fases[_dlf].Fase[fase]._complete = value;

        //if (PlayerPrefs.HasKey(("F" + (fase)+IdPlayer).ToString()))
        PlayerPrefs.SetInt((DlfSave() + "F" + (fase) + IdPlayer).ToString(), 1);

        //if (PlayerPrefs.HasKey(nameFase))
        PlayerPrefs.SetInt(nameFase, 1);

        if (kill)
        {
            nameCompletePortalOrKill = (DlfSave() + "F" + (fase) + "CompleteKill" + IdPlayer).ToString();
            Mob[(idHero)]._Fases[_dlf].Fase[fase]._completeKill = value;
        }
        else
        {
            if (Mob[(idHero)]._Fases[_dlf].Fase[fase]._completePortal == 0/*null*/)
            {
                nameCompletePortalOrKill = (DlfSave() + "F" + (fase) + "CompletePortal" + IdPlayer).ToString();
                Mob[(idHero)]._Fases[_dlf].Fase[fase]._completePortal = 1;//value;
            }
        }

        if (nameCompletePortalOrKill != null)
        //if (PlayerPrefs.HasKey(nameCompletePortalOrKill))
        {
            PlayerPrefs.SetInt(nameCompletePortalOrKill, 1);
            Debug.LogWarning("Save [Complete Fase" + fase + " (" + _gms.HeroName(idHero) + ") - D:" + _dlf + " [" + DlfSave() + "]] Complete");
        }

        CheckBlockFase((fase + 1), 1); //Desbloqueia proxima fase
    }

    /// <summary>
    /// Atualiza a fase para complete
    /// </summary>
    /// <param name="fase">fase na contagem normal</param>
    public void CompleteFase(int fase, bool kill, float seg, float min, int hero = -1)
    {
        if (fase >= FaseCount)
            return;

        string IdPlayer = "";
        int idHero = hero;

        if (idHero == -1)
            idHero = PlayerID-1;


        if (idHero > 1)
            IdPlayer = (idHero - 1).ToString("F0");

        //string dfl = _dlf > 0 ? _dlf + "" : "";

        print("CheckCompleteFase(" + fase + " , " + kill + " , " + hero + ") - " + idHero + " / idP: " + IdPlayer);

        bool value = Convert.ToBoolean(1);
        string nameFase = (DlfSave() + "F" + (fase) + "Complete" + IdPlayer).ToString();
        string nameCompletePortalOrKill = null;

        Mob[(idHero)]._Fases[_dlf].Fase[fase]._dontBlock = value;
        Mob[(idHero)]._Fases[_dlf].Fase[fase]._complete = value;

        //if (PlayerPrefs.HasKey(("F" + (fase)+IdPlayer).ToString()))
        PlayerPrefs.SetInt((DlfSave() + "F" + (fase) + IdPlayer).ToString(), 1);

        //if (PlayerPrefs.HasKey(nameFase))
        PlayerPrefs.SetInt(nameFase, 1);

        if (kill)
        {
            nameCompletePortalOrKill = (DlfSave() + "F" + (fase) + "CompleteKill" + IdPlayer).ToString();
            Mob[(idHero)]._Fases[_dlf].Fase[fase]._completeKill = value;
        }
        else
        {
            if (Mob[(idHero)]._Fases[_dlf].Fase[fase]._completePortal == 0/*null*/)
            {
                nameCompletePortalOrKill = (DlfSave() + "F" + (fase) + "CompletePortal" + IdPlayer).ToString();
                Mob[(idHero)]._Fases[_dlf].Fase[fase]._completePortal = 1;//value;
            }
        }

        if (nameCompletePortalOrKill != null)
        //if (PlayerPrefs.HasKey(nameCompletePortalOrKill))
        {
            PlayerPrefs.SetInt(nameCompletePortalOrKill, 1);
            Debug.LogWarning("Save [Complete Fase" + fase + " (" + _gms.HeroName(idHero) + ") - D:" + _dlf + " [" + DlfSave() + "]] Complete");
        }

        TimeRecord(fase, seg, min, hero);

        CheckBlockFase((fase + 1), 1); //Desbloqueia proxima fase
    }

    ///Check fase is complete
    public bool CheckCompleteFase(int fase = -1, int hero = -1)
    {
        if (fase >= FaseCount)
            return false;

        string IdPlayer = "";
        int idHero = hero;

        if (idHero == -1)
            idHero = PlayerID-1;

        if (fase == -1)
            fase = FaseAtual;

        if (idHero > 1)
            IdPlayer = (idHero).ToString("F0");

        return Mob[(idHero)]._Fases[_dlf].Fase[fase]._complete;
    }

    //Check fase is complete
    public bool CheckCompleteFase(int fase = -1, int hero = -1, int dificuldade = -1)
    {
        if (fase >= FaseCount || dificuldade < 0 || dificuldade > 2)
            return false;

        string IdPlayer = "";
        int idHero = hero;

        if (idHero == -1)
            idHero = PlayerID-1;

        if (fase == -1)
            fase = FaseAtual;

        if (dificuldade == -1)
            dificuldade = _dlf;

        if (idHero > 1)
            IdPlayer = (idHero).ToString("F0");

        return Mob[(idHero)]._Fases[dificuldade].Fase[fase]._complete;
    }

    public void TimeRecord(int fase, float seg, float min, int idPlayer = -1)
    {
        print("Time Record");

        if (fase > achievement.Count)
            return;

        if (idPlayer == -1)
            idPlayer = PlayerID-1;

        int idHero = idPlayer;

        if (fase == -1)
            fase = _atualFase;

        float valueS = Mob[(idHero)]._Fases[_dlf].Fase[fase]._seg,
              valueM = Mob[(idHero)]._Fases[_dlf].Fase[fase]._min;

        if (valueS != -1 && valueM != -1)
            if (/*min > valueM || */
                min >= valueM && seg >= valueS)
                return;

        float _Min = min;

        if (_Min < 0)
            _Min = 0;

        Debug.LogError("New Record[" + Mob[idHero]._nameHero + "] - " + Mob[(idHero)]._Fases[_dlf].Fase[fase]._fase);

        FaseMin(fase, _Min, idPlayer: idHero);

        FaseSeg(fase, seg, idPlayer: idHero);
    }

    /// <summary>
    /// Return Record Seg
    /// </summary>
    /// <param name="fase">fase id</param>
    /// <param name="value">value for change</param>
    /// <param name="change">change</param>
    /// <returns>current Record seg value</returns>
    public float FaseSeg(int fase, float value, bool change = true, int idPlayer = -1)
    {
        if (idPlayer == -1)
            idPlayer = (PlayerID - 1);

        if (fase == -1)
            fase = _atualFase;

        if (change)
        {
            string IdPlayer = "";

            if (idPlayer > 0)
                IdPlayer = idPlayer.ToString("F0");

            Mob[idPlayer]._Fases[_dlf].Fase[fase]._seg = value;

            //string Idlf = (_dlf > 0 ? _dlf + "" : "");
            PlayerPrefs.SetFloat((DlfSave() + "F" + fase + "Seg" + IdPlayer).ToString(), (float)value);

            Debug.LogError(Mob[idPlayer]._Fases[_dlf].Fase[fase]._fase + "(D=" + _dlf + " - " + DlfSave() + ") [" + Mob[idPlayer]._nameHero + "] Seg: " + value);
        }

        return Mob[idPlayer]._Fases[_dlf].Fase[fase]._seg;
    }

    /// <summary>
    /// Return Record Min
    /// </summary>
    /// <param name="fase">fase id</param>
    /// <param name="value">value for change</param>
    /// <param name="change">change</param>
    /// <returns>current Record min value</returns>
    public float FaseMin(int fase, float value, bool change = true, int idPlayer = -1)
    {
        if (idPlayer == -1)
            idPlayer = (PlayerID - 1);

        if (fase == -1)
            fase = _atualFase;

        if (change)
        {
            string IdPlayer = "";

            if (idPlayer > 0)
                IdPlayer = idPlayer.ToString("F0");

            Mob[idPlayer]._Fases[_dlf].Fase[fase]._min = value;

            string Idlf = (_dlf > 0 ? _dlf + "" : "");
            PlayerPrefs.SetFloat((Idlf + "F" + fase + "Min" + IdPlayer).ToString(), (float)value);

            Debug.LogError(Mob[idPlayer]._Fases[_dlf].Fase[fase]._fase + "(D=" + _dlf + " - " + Idlf + ") [" + Mob[idPlayer]._nameHero + "] Min: " + value);
        }

        return Mob[idPlayer]._Fases[_dlf].Fase[fase]._min;
    }

    public void ChangeSalveFasePlayer() { StartCoroutine(ChangeSalveFasePlayerCoroutine()); }
    IEnumerator ChangeSalveFasePlayerCoroutine()
    {
        int loadingFases = 0;

        foreach (var heros in Mob)
            if (heros._blocked == false)
                loadingFases++;

        int count = 0;

        int faseCount = FaseCount;

        string descr = "";

        #region Fases
        for (int j = 0; j < PlayerCount; j++)//Player
        {
            //Debug.Log("Loading Fases [" + _gms.HeroName(j+1) + "]");
            if (!CheckMobBlocked(j))
            {
                count++;
                for (int i = 0; i < faseCount; i++)//_Fases
                {
                    yield return waitChangeSalve;

                    for (int d = 0; d < (_dificuldadeCount - 1); d++)
                    {
                        yield return waitChangeSalve;

                        if (loadingFases == 1)
                            descr = "Loading...\n<b>" + HeroName(j) + "</b> - <b>" + NameFase(i) + "</b>";
                        else
                            descr = (count) + "/" + (loadingFases) + "\n<b>" + HeroName(j) + "</b> - <b>" + NameFase(i) + "</b>";

                        string IdPlayer = ""
                            //,Idlf = d>0 ? d + "" : ""
                            ;


                        if (j > 0)
                            IdPlayer = j.ToString("F0");

                        // Debug.LogError("Mob: "+HeroName(j)+" Fase: "+ NameFase(i)+" Dificuldade: "+d+" ->"+ DlfSave(d));

                        Mob[j]._Fases[d].Fase[i]._dontBlock = Convert.ToBoolean(PlayerPrefs.GetInt((DlfSave(d) + "F" + i + IdPlayer).ToString()));

                        Mob[j]._Fases[d].Fase[i]._complete = Convert.ToBoolean(PlayerPrefs.GetInt((DlfSave(d) + "F" + i + "Complete" + IdPlayer).ToString()));

                        Mob[j]._Fases[d].Fase[i]._completeKill = Convert.ToBoolean(PlayerPrefs.GetInt((DlfSave(d) + "F" + i + "CompleteKill" + IdPlayer).ToString()));

                        Mob[j]._Fases[d].Fase[i]._completePortal = /*Convert.ToBoolean(*/PlayerPrefs.GetInt((DlfSave(d) + "F" + i + "CompletePortal" + IdPlayer).ToString())/*)*/;

                        float seg = PlayerPrefs.GetFloat((DlfSave(d) + "F" + i + "Seg" + IdPlayer)),
                              min = PlayerPrefs.GetFloat((DlfSave(d) + "F" + i + "Min" + IdPlayer));

                        //Debug.Log("Record D["+d+" / "+ Idlf + "] Fase[" + i + "] Player[" + PlayerID + "] min" + min + ":" + seg + " seg.");

                        if (seg <= 0 && min <= 0)
                        {
                            seg = -1;
                            min = -1;
                        }

                        //FaseSeg(i, seg, idPlayer: j);
                        //FaseMin(i, min, idPlayer: j);
                        Mob[j]._Fases[d].Fase[i]._min = min;
                        Mob[j]._Fases[d].Fase[i]._seg = seg;

                        LoadingBar(descr, i, faseCount - 1);
                    }
                }
                //yield return waitDelayBar;
            }
        }
        #endregion

        CloseLoadingBar(true);
    }
    #endregion

    #region Modo Battle
    [Space]
    [Header("Battle Mode")]
    public BattleModeGamePlay _battleModeGamePlay;

    public bool BattleModeBlocked { get { return _battleModeBlocked; } }
    bool _battleModeBlocked = true;

    public static bool BattleMode { get { return GameManagerScenes._gms.GameMode == Game_Mode.Battle; } }

    #region Battle Mode - Options
    [Header("Battle Mode - Options")]
    [SerializeField, Tooltip("Dont repeat Mob Selection in battle mode")]
    bool _battleModeOptionUniqueSelection = false;
    public bool BattleModeOptionUniqueSelection
    {
        set { PlayerPrefs.SetInt("BattleModeUniqueSelection", Convert.ToInt32(value)); _battleModeOptionUniqueSelection = value; }
        get { return Convert.ToBoolean(PlayerPrefs.GetInt("BattleModeUniqueSelection")); }
    }

    [SerializeField, Tooltip("battle mode in rounds")]
    bool _battleModeOptionRoundActive = false;
    public bool BattleModeOptionRoundActive
    {
        set { PlayerPrefs.SetInt("BattleModeRoundActive", Convert.ToInt32(value)); _battleModeOptionRoundActive = value; }
        get { return Convert.ToBoolean(PlayerPrefs.GetInt("BattleModeRoundActive")); }
    }

    [SerializeField, Tooltip("Critical Damage Active in battle mode")]
    bool _battleModeOptionCriticalDamageActive = true;
    public bool BattleModeOptionCriticalDamageActive
    {
        set { _battleModeOptionCriticalDamageActive = value; }
        get { return _battleModeOptionCriticalDamageActive; }
    }

    [SerializeField, Tooltip("Passive Mobs Active in battle mode")]
    bool _battleModeOptionPassiveMobActive = true;
    public bool BattleModeOptionPassiveMobActive
    {
        set { PlayerPrefs.SetInt("BattleModePassiveMobActive", Convert.ToInt32(value)); _battleModeOptionPassiveMobActive = value; }
        get { return Convert.ToBoolean(PlayerPrefs.GetInt("BattleModePassiveMobActive"));}
    }
    #endregion

    #region Battle Mode - Options Status
    //[Header("Battle Mode - Options Status")]
    //Max 200
    public int BattleModeOptionStatusHpBaseExtra
    {
        set { PlayerPrefs.SetInt("BattleModeStatusHp", value); }
        get { return PlayerPrefs.GetInt("BattleModeStatusHp"); }
    }

    //Max 150
    public int BattleModeOptionStatusDamageBaseExtra
    {
        set { PlayerPrefs.SetInt("BattleModeStatusDamage", value);}
        get { return PlayerPrefs.GetInt("BattleModeStatusDamage"); }
    }

   //max 100
    public float BattleModeOptionStatusCriticalChanceBaseExtra
    {
        set { PlayerPrefs.SetFloat("BattleModeStatusCriticalChance", value);}
        get { return PlayerPrefs.GetFloat("BattleModeStatusCriticalChance"); }
    }
    #endregion

    #region Battle Mode - Options Dbuff
    [Space]
    [Header("Battle Mode - Options Dbuff")]
    //Fire
    [SerializeField,Tooltip("Porcent Damage Fire Dbuff"), Range(1,25)]
    int _battleModeOptionDbuffFirePorcentDamage = 10;
    public int BattleModeOptionDbuffFirePorcentDamage
    {
        set { _battleModeOptionDbuffFirePorcentDamage = value; }
        get { return _battleModeOptionDbuffFirePorcentDamage; }
    }

    public float BattleModeOptionDbuffFireResistenceExtra
    {
        set { PlayerPrefs.SetFloat("BattleModeDbuffFireResistence", value);}
        get { return PlayerPrefs.GetFloat("BattleModeDbuffFireResistence"); }
    }
    [Space]
    //Poison
    [SerializeField, Tooltip("Porcent Damage Poison Dbuff"), Range(1, 25)]
    int _battleModeOptionDbuffPoisonPorcentDamage = 5;
    public int BattleModeOptionDbuffPoisonPorcentDamage
    {
        set { _battleModeOptionDbuffPoisonPorcentDamage = value; }
        get { return _battleModeOptionDbuffPoisonPorcentDamage; }
    }

    public float BattleModeOptionDbuffPoisonResistenceExtra
    {
        set { PlayerPrefs.SetFloat("BattleModeDbuffPoisonResistence", value); }
        get { return PlayerPrefs.GetFloat("BattleModeDbuffPoisonResistence"); }
    }
    [Space]
    //Petrify
    [SerializeField, Tooltip("Porcent Damage Petrify Dbuff"), Range(1, 25)]
    int _battleModeOptionDbuffPetrifyPorcentDamage = 15;
    public int BattleModeOptionDbuffPetrifyPorcentDamage
    {
        set { _battleModeOptionDbuffPetrifyPorcentDamage = value; }
        get { return _battleModeOptionDbuffPetrifyPorcentDamage; }
    }

    public float BattleModeOptionDbuffPetrifyResistenceExtra
    {
        set { PlayerPrefs.SetFloat("BattleModeDbuffPetrifyResistence", value);}
        get { return PlayerPrefs.GetFloat("BattleModeDbuffPetrifyResistence"); }
    }
    [Space]
    //Bleed
    [SerializeField, Tooltip("Porcent Damage Bleed Dbuff"), Range(1, 25)]
    int _battleModeOptionDbuffBleedPorcentDamage = 10;
    public int BattleModeOptionDbuffBleedPorcentDamage
    {
        set { _battleModeOptionDbuffBleedPorcentDamage = value; }
        get { return _battleModeOptionDbuffBleedPorcentDamage; }
    }

    public float BattleModeOptionDbuffBleedResistenceExtra
    {
        set { PlayerPrefs.SetFloat("BattleModeDbuffBleedResistence", value);}
        get { return PlayerPrefs.GetFloat("BattleModeDbuffBleedResistence"); }
    }
    #endregion

    //Desbloquear Modo Batalha
    public void BattleModeDesblock()
    {
        print("BattleModeDesblock()");

        if (!BattleModeBlocked)
            return;

        NewInfo(
            AttDescriçãoMult(
              XmlMenuInicial.Instance.Get(90),// "Parabens!!!,\n{0}\n Foi Desbloqueado!!!"
               XmlMenuInicial.Instance.Get(203)),// "Modo Batalha"
            6f);

        print("Modo Batalha Desbloqueado!!!");

        _battleModeBlocked = false;
        PlayerPrefs.SetInt("BattleModeBlocked", 0);
    }

    public void BattleModeAddMobsInList(int idMob, int idSkin, int X, int Y,int Time, bool isPlayer)
    {
        BattleModeMob BMM = new BattleModeMob();

        BMM._Time     = Time;
        BMM._isPlayer = isPlayer;

        BMM._idMob     = idMob;
        BMM._idMobSkin = idSkin;

        BMM._XRespaw = X;
        BMM._YRespaw = Y;

        _battleModeGamePlay._battleModeMobs.Add(BMM);

        Debug.LogWarning("AddMobsInList("+idMob+ " - [" + HeroName(idMob) + "]," + idSkin+","+X+","+Y+","+Time+","+isPlayer+")");
    }
    #endregion

    #region Player
    [Header("Default Thinks")]
    public Sprite   _defaultSpriteChat;
    public Sprite   _defaultSpritePerfil;
    public Sprite[] _defaultSpriteClass;
    public Color[]  _colorTypeSkin;

    [Header("Player Select"), Tooltip("Selecionar o player")]
    [SerializeField,Range(-1,99)] int          _playerId     = -1;   
    public int PlayerID
    {
        get
        {
            if (PlayerPrefs.HasKey("PlayerId"))
                return PlayerPrefs.GetInt("PlayerId");
            else
            return _playerId;
        }
        set
        {
            if (value>0)
            {
                _playerId = value;

                print("Change Player["+value+"] for "+HeroName(value-1));

                PlayerPrefs.SetInt("PlayerId", _playerId);

                _playerSkinId = PlayerPrefs.GetInt((_playerId - 1) + "Skin");
            }           
        }
    }
    [SerializeField]             int          _playerSkinId = 0;
    public int PlayerSkinID
    {
        get
        {
            int pID = PlayerID - 1;

            print("GetSkin do "+HeroName()+" -> "+ PlayerPrefs.HasKey((pID) + "Skin")+" = "+ PlayerPrefs.GetInt(pID + "Skin"));

            if (PlayerPrefs.HasKey(pID + "Skin"))
            {
                //print("GetSkin " + _Fases[pID]._skinHero[PlayerPrefs.GetInt(pID + "Skin")]._nameSkin + " -> " + PlayerPrefs.GetInt(pID + "Skin"));
                return PlayerPrefs.GetInt(pID + "Skin");
            }
            else
                return _playerSkinId;
        }
        set
        {
            if (value >= 0)
            {
                int pID = PlayerID - 1;

                _playerSkinId = value;

                PlayerPrefs.SetInt((pID) + "Skin", _playerSkinId);

                //print("Salve skin " + _Fases[pID]._skinHero[value]._nameSkin);
            }
        }
    }

    public string NamePlayer
    {
        get
        {
            int id = (PlayerID - 1);

            if (id < 0 || id >= PlayerCount ||
               PlayerSkinID <= -1 || PlayerSkinID >= SkinCount(id))
                return "<color=red>Erro 404 Name Mob Not Found</color>";

            return XmlMobManager.Instance.Name(id);
        }
    }

    public GameObject SkinPlayer
    {
        get
        {
            GameObject Skin = Mob[PlayerID - 1]._prefabHero;

            if (Mob[PlayerID - 1]._skinHero[PlayerSkinID]._skinPrefab == null)
            {
                Skin = Mob[PlayerID - 1]._skinHero[PlayerSkinID]._skinPrefab;
            }
            else
            {
                Skin.GetComponent<MeshFilter>().mesh        = Mob[PlayerID - 1]._skinHero[PlayerSkinID]._skinMesh;
                Skin.GetComponent<MeshRenderer>().materials = Mob[PlayerID - 1]._skinHero[PlayerSkinID]._skinMaterials;
            }

                MobManager mm = Skin.GetComponent<MobManager>();

                mm.damage                  = 0;
                mm.health                  = 0;

                mm.maxTimeWalk             = 0;
                mm.currentTimeAttack       = 0;

                mm.chanceCritical          = 0;

                mm.DamageResistenceFire    = 0;
                mm.DamageResistencePoison  = 0;
                mm.DamageResistencePetrify = 0;
                mm.DamageResistenceBleed   = 0;

            return Skin;
        }
        set
        {
            PlayerPrefs.SetInt("PlayerSkinId", GetSkinId(value));
        }
    }

    public Sprite SpritePerfilPlayer
    {
        get
        {
            int id = (PlayerID - 1);

            if (id < 0 || id >= PlayerCount ||
               PlayerSkinID <= -1 || PlayerSkinID >= SkinCount(id))
                return _defaultSpritePerfil;

            Sprite sprite = Mob[id]._skinHero[PlayerSkinID]._spritePerfil;

            if (sprite == null)
                return _defaultSpritePerfil;
            else
                return sprite;
        }
    }
    public Sprite SpritePerfilChatPlayer
    {
        get
        {
            int id = (PlayerID - 1);

            if (id < 0 || id >= PlayerCount ||
               PlayerSkinID <= -1 || PlayerSkinID >= SkinCount(id))
                return _defaultSpriteChat;

            Sprite sprite = Mob[id]._skinHero[PlayerSkinID]._spritePerfilChat;

            if (sprite == null)
                return _defaultSpriteChat;
            else
                return sprite;
        }
    }

    public Sprite SpritePerfil(GameObject mob)
    {
        Sprite sprite =null;

        foreach (var f in Mob)
        {
            if (f._prefabHero != null)
                if (mob.name == f._prefabHero.name)
                {
                    sprite = f._skinHero[0]._spritePerfil;
                    break;
                }

            if (f._prefabHero == (mob))
            {
                sprite = f._skinHero[0]._spritePerfil;
                break;
            }

            int count = f._skinHero.Count;
            for (int i = 0; i < count; i++)
            {
                if (f._skinHero[i] != null)
                {
                    if (f._skinHero[i]._skinPrefab != null)
                    {
                        if (mob.name == f._skinHero[i]._skinPrefab.name ||
                           (mob)     == f._skinHero[i]._skinPrefab)
                        {
                            sprite = f._skinHero[i]._spritePerfil;
                            break;
                        }
                    }
                }
            }
        }

       

        if (sprite == null)
        {
            
             NewInfo("Sprite Perfil do "+ mob.GetComponent<ToolTipType>()._name + " Não foi encontrado <color=red>NOT FOUND</color>", 5, true);

            return _defaultSpritePerfil;
        }
        else
            return sprite;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mob">name tooltipType</param>
    /// <returns></returns>
    public Sprite SpritePerfil(string mob)
    {
        Sprite sprite = null;

        foreach (var f in Mob)
        {
            if (mob == f._nameHero)
                {
                    sprite = f._skinHero[0]._spritePerfil;
                    break;
                }
        }

        if (sprite == null)
        {

            NewInfo("Sprite Perfil do " + mob + " Não foi encontrado <color=red>NOT FOUND</color>", 5, true);

            return _defaultSpritePerfil;
        }
        else
            return sprite;
    }
    public Sprite SpritePerfil(int index,int skin=0)
    {
        if (index == -1)
            index = (PlayerID - 1);
        if (skin == -1)
            skin = PlayerSkinID;

        if (index < 0 || index > PlayerCount ||
            skin <= -1 || skin >= SkinCount(index))
        {            
            NewInfo("Sprite Perfil do " + HeroName(index) + " skin[" + skin + "] Não foi encontrado <color=red>index invalido</color>", 5, true);

            return _defaultSpritePerfil;
        }

        Sprite sprite = Mob[index]._skinHero[skin]._spritePerfil;

        if (sprite==null)
        {
           NewInfo("Sprite Perfil do " + HeroName(index) + " skin[" + skin + "] Não foi encontrado <color=red>NOT FOUND</color>", 5, true);

            return _defaultSpritePerfil;
        }
        else
        return sprite;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mob">name tooltipType</param>
    /// <returns></returns>
    public Sprite SpritePerfil(string mob, int skin = 0)
    {
       int index = HeroID(mob);

        if (index < 0 || index > PlayerCount ||
            skin <= -1 || skin >= SkinCount(index))
        {
            NewInfo("Sprite Perfil do " + HeroName(index) + " skin[" + skin + "] Não foi encontrado <color=red>index invalido</color>", 5, true);

            return _defaultSpritePerfil;
        }

        Sprite sprite = Mob[index]._skinHero[skin]._spritePerfil;

        if (sprite == null)
        {
            NewInfo("Sprite Perfil do " + HeroName(index) + " skin[" + skin + "] Não foi encontrado <color=red>NOT FOUND</color>", 5, true);

            return _defaultSpritePerfil;
        }
        else
            return sprite;
    }

    public Sprite SpriteChat(GameObject mob)
    {
        Sprite sprite = null;

        foreach (var f in Mob)
        {
            if (f._prefabHero != null)
                if (mob.name == f._prefabHero.name ||
                    (mob)    == f._prefabHero)
                {
                    sprite = f._skinHero[0]._spritePerfilChat;
                    break;
                }

            int count = f._skinHero.Count;

            for (int i = 0; i < count; i++)
            {
                if (f._skinHero[i] != null)
                {
                    if (f._skinHero[i]._skinPrefab != null)
                    {
                            if (mob.name == f._skinHero[i]._skinPrefab.name ||
                               (mob)     == f._skinHero[i]._skinPrefab)
                            {
                                sprite = f._skinHero[i]._spritePerfilChat;
                                break;
                            }
                    }
                }
            }
        }

        if (sprite == null)
        {
          
            NewInfo("Sprite Perfil do " + mob.GetComponent<ToolTipType>()._name + " Não foi encontrado <color=red>NOT FOUND</color>", 5, true);

            return _defaultSpriteChat;
        }
        else
            return sprite;
    }
    public Sprite SpriteChat(int index, int skin = 0)
    {
        if (index < 0 || index > PlayerCount ||
            skin <= -1 || skin >= SkinCount(index))
        { 
                NewInfo("Sprite Perfil do " + HeroName(index) + " skin[" + skin + "] Não foi encontrado <color=red>index invalido</color>", 5, true);
            return _defaultSpriteChat;
        }

        Sprite sprite = Mob[index]._skinHero[skin]._spritePerfilChat;

        if (sprite == null)
        {
            
                NewInfo("Sprite Perfil do " +HeroName(index) + " skin[" + skin + "] Não foi encontrado <color=red>NOT FOUND</color>", 5, true);

            return _defaultSpriteChat;
        }
        else
            return sprite;
    }

    public Sprite SpriteClass(GameObject mob)
    {
        MobManager.Classe C = MobManager.Classe.random;

        Sprite sprite = _defaultSpriteClass[0];

        foreach (var f in Mob)
        {
            if (f._prefabHero != null)
                if (mob.name == f._prefabHero.name ||
                    (mob) == f._prefabHero)
                {
                    C = f._classe;
                    break;
                }

            int count = f._skinHero.Count;

            for (int i = 0; i < count; i++)
            {
                if (f._skinHero[i] != null)
                {
                    if (f._skinHero[i]._skinPrefab != null)
                    {
                        if (mob.name == f._skinHero[i]._skinPrefab.name ||
                           (mob) == f._skinHero[i]._skinPrefab)
                        {
                            C = f._classe;
                            break;
                        }
                    }
                }
            }
        }
       

        switch (C)
        {
            case MobManager.Classe.tanker:
                sprite = _defaultSpriteClass[1];
                break;
            case MobManager.Classe.adc:
                sprite = _defaultSpriteClass[2];
                break;
            case MobManager.Classe.suporte:
                sprite = _defaultSpriteClass[3];
                break;
            case MobManager.Classe.assassino:
                sprite = _defaultSpriteClass[4];
                break;
            case MobManager.Classe.soldado:
                sprite = _defaultSpriteClass[5];
                break;
            case MobManager.Classe.mago:
                sprite = _defaultSpriteClass[6];
                break;
        }
        if (sprite == null)
        {

            NewInfo("Sprite Classe do " + mob.GetComponent<ToolTipType>()._name + " Não foi encontrado <color=red>NOT FOUND</color>", 5, true);

            return _defaultSpriteClass[0];
        }
        else
            return sprite;
    }
    public Sprite SpriteClass(int index/*, int skin = 0*/)
    {
        if (index < 0 || index > PlayerCount /*||
            skin <= -1 || skin >= SkinCount(index)*/)
        {
            NewInfo("Sprite Classe do " + HeroName(index) + " Não foi encontrado <color=red>index invalido</color>", 5, true);
            return _defaultSpriteClass[0];
        }

        Sprite sprite = _defaultSpriteClass[0];

        switch (Mob[index]._classe)
        {
            case MobManager.Classe.tanker:
                sprite = _defaultSpriteClass[1];
                break;
            case MobManager.Classe.adc:
                sprite = _defaultSpriteClass[2];
                break;
            case MobManager.Classe.suporte:
                sprite = _defaultSpriteClass[3];
                break;
            case MobManager.Classe.assassino:
                sprite = _defaultSpriteClass[4];
                break;
            case MobManager.Classe.soldado:
                sprite = _defaultSpriteClass[5];
                break;
            case MobManager.Classe.mago:
                sprite = _defaultSpriteClass[6];
                break;
        }

        if (sprite == _defaultSpriteClass[0])
        {

            NewInfo("Sprite Classe do " + HeroName(index) + " Não foi encontrado <color=red>NOT FOUND</color>", 5, true);

            return sprite;
        }
        else
            return sprite;
    }

    public string HeroName(int index = -1)
    {
        if (index == -1)
            index = (PlayerID - 1);

        if (index < 0 || index > PlayerCount)
            return "<color=red>" + AttDescriçãoMult(XmlMenuInicial.Instance.Get(116), "Name Mob") + "</color>";//ERRO, {0} Não encontrado

        if (XmlMobManager.Instance != null)
            return XmlMobManager.Instance.Name(index);
        else
            return Mob[index]._nameHero;
    }
    public string HeroName(GameObject mob)
    {
        Debug.LogError("HeroName(" + mob + ")");

        foreach (var f in Mob)
        {
            if (mob.GetComponent<ToolTipType>()._name == f._prefabHero.GetComponent<ToolTipType>()._name)
            {
                return f._nameHero;
            }

            int count = f._skinHero.Count;

            if (f._prefabHero == mob)
            {
                return f._nameHero;
            }

            for (int i = 0; i < count; i++)
            {
                if (f._skinHero[i]._skinPrefab == (mob))
                {
                    return f._nameHero;
                }
            }
        }

        return "<color=red>"+ AttDescriçãoMult(XmlMenuInicial.Instance.Get(116), "Name Mob")+"</color>";//ERRO, {0} Não encontrado
    }   

    public int PlayerCount { get { return Mob.Count; } }

    public int SkinCount(int mobID)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return -1;
        //print(HeroName(mobID)+" Tem "+ _Fases[mobID]._skinHero.Count + " Skins.");
        return Mob[mobID]._skinHero.Count;
    }
    public int SkinCount(GameObject mob)
    {
        int mobID = HeroID(mob);

        if (mobID >= PlayerCount || mobID <= -1)
            return -1;
        //print(HeroName(mobID) + " Tem " + _Fases[mobID]._skinHero.Count + " Skins.");
        return Mob[mobID]._skinHero.Count;
    }
    public int SkinCountDesblock(int mobID)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return -1;
        int count = 0;
        for (int i = 0; i < Mob[mobID]._skinHero.Count; i++)
        {
            if (!Mob[mobID]._skinHero[i]._skinBlocked)
                count++;
        }

        return count;
    }
    #region Teste
    public int GetClasseAtributeDamage(MobManager.Classe classe)
    {
        int value = 1;
        switch (classe)
        {
            case MobManager.Classe.tanker:
                value = -1; break;
            case MobManager.Classe.adc:
                value = 4; break;
            case MobManager.Classe.suporte:
                value = 1; break;
            case MobManager.Classe.assassino:
                value = 3; break;
            case MobManager.Classe.soldado:
                value = 1; break;
            case MobManager.Classe.mago:
                value = 2; break;
        }

        return value*2;
    }
    public int GetClasseAtributeHealth(MobManager.Classe classe)
    {
        int value = 1;
        switch (classe)
        {
            case MobManager.Classe.tanker:
                value = 4; break;
            case MobManager.Classe.adc:
                value = -1; break;
            case MobManager.Classe.suporte:
                value = 1; break;
            case MobManager.Classe.assassino:
                value = 0; break;
            case MobManager.Classe.soldado:
                value = 3; break;
            case MobManager.Classe.mago:
                value = 1; break;
        }

        return value*2;
    }
    public int GetClasseAtributeWalk(MobManager.Classe classe)
    {
        int value = 1;
        switch (classe)
        {
            case MobManager.Classe.tanker:
                value = 0; break;
            case MobManager.Classe.adc:
                value = 0; break;
            case MobManager.Classe.suporte:
                value = 2; break;
            case MobManager.Classe.assassino:
                value = 1; break;
            case MobManager.Classe.soldado:
                value = 0; break;
            case MobManager.Classe.mago:
                value = 1; break;
        }

        return value;
    }

    //https://rextester.com/HLG27907
    public int CalculeDamageValue(int idMob)
    {
        int Base = Mob[idMob]._statusIaMob[(int)_dificuldade]._damageMax;
        float I = Mob[idMob]._Idamage;

        return CalculeDamageValue(Base, I, GetClasseAtributeDamage(Mob[idMob]._classe));
    }
    public int CalculeDamageValue(int idMob,MobManager.Classe classe)
    {
         int Base = Mob[idMob]._statusIaMob[(int)_dificuldade]._damageMax;
         float I = Mob[idMob]._Idamage;

        return CalculeDamageValue(Base,I, GetClasseAtributeDamage(classe));
    }
    public int CalculeDamageValue(int Base,float individual, MobManager.Classe classe)
    {
        return CalculeDamageValue(Base,individual, GetClasseAtributeDamage(classe));
    }
    public int CalculeDamageValue(int Base, float individual,int classe)
    {
        int _return = 0;
        float I = individual;
        int Classe = classe;
        double R = (Base * ((Math.Pow(I, Classe)) / (2 * (I + 1)))) * 1000;

        if (R < 0)
            R /= -1;

        _return = (int)(Math.Sqrt(R)) /10;

        return _return;
    }
    public int CalculeDamageValueBase(int idMob)
    {
        int Base = Mob[idMob]._statusIaMob[(int)_dificuldade]._damageMax;
        float I = Mob[idMob]._Idamage;

        return CalculeDamageValueBase(idMob, Mob[idMob]._classe);
    }
    public int CalculeDamageValueBase(int idMob, MobManager.Classe classe)
    {
        int  Base = Mob[idMob]._statusIaMob[(int)_dificuldade]._damageMax;
        float I = Mob[idMob]._Idamage;

        return CalculeDamageValue(Base, I, GetClasseAtributeDamage(classe)) + (int)Base;
    }
    //https://rextester.com/HVBX32073
    public int CalculeHealthValue(int idMob)
    {
        int Base = Mob[idMob]._statusIaMob[(int)_dificuldade]._damageMax;
        float I = Mob[idMob]._Idamage;

        return CalculeHealthValue(Base, I, GetClasseAtributeDamage(Mob[idMob]._classe));
    }
    public int CalculeHealthValue(int idMob, MobManager.Classe classe)
    {
        float Base = Mob[idMob]._statusIaMob[(int)_dificuldade]._healthMax;
        float I = Mob[idMob]._Ihealth;

        return CalculeHealthValue(Base, I, GetClasseAtributeHealth(classe));
    }
    public int CalculeHealthValue(float Base, float individual, MobManager.Classe classe)
    {
        return CalculeHealthValue(Base, individual, GetClasseAtributeHealth(classe));
    }
    public int CalculeHealthValue(float Base, float individual, int classe)
    {
        int _return = 0;
        float I = individual;
        int Classe = classe;
        double R = (Base * ((Math.Pow(I, Classe)) / (2 * (I + 1)))) * 1000;

        if (R <= -1)
            R /= -1;

        //_return = (int)(Math.Sqrt(R)/7);
        _return = (int)(Math.Sqrt(R/2) / 5);
        return _return;
    }
    public int CalculeHealthValueBase(int idMob)
    {
        int Base = Mob[idMob]._statusIaMob[(int)_dificuldade]._damageMax;
        float I = Mob[idMob]._Idamage;

        return CalculeHealthValueBase(idMob, Mob[idMob]._classe);
    }
    public int CalculeHealthValueBase(int idMob, MobManager.Classe classe)
    {
        float Base = Mob[idMob]._statusIaMob[(int)_dificuldade]._healthMax;
        float I = Mob[idMob]._Ihealth;

        return CalculeHealthValue(Base, I, GetClasseAtributeHealth(classe)) + (int)Base;
    }
    [Serializable]
    public class TesteCalcule { public string c; public int v; }
    public void ContextTesteDamageMobs()
    {
        string Attribute = "Damage";
        int B = 10, C = 0,F,S;
        float I = Mob[0]._Idamage;

        List<TesteCalcule>
                     maxTank = new List<TesteCalcule>(),
                     minTank = new List<TesteCalcule>(),
                     maxAdc = new List<TesteCalcule>(),
                     minAdc = new List<TesteCalcule>(),
                     maxSup = new List<TesteCalcule>(),
                     minSup = new List<TesteCalcule>(),
                     maxAss = new List<TesteCalcule>(),
                     minAss = new List<TesteCalcule>(),
                     maxSold = new List<TesteCalcule>(),
                     minSold = new List<TesteCalcule>(),
                     maxMago = new List<TesteCalcule>(),
                     minMago = new List<TesteCalcule>();

        var values = System.Enum.GetValues(typeof(MobManager.Classe));
        int count = values.Length - 1;


        Debug.Log("**<color=red>"+Attribute+"</color>**");
        Debug.Log("__________");        
        for (int c = 2; c < count; c++)
        {
            C = c;
            F = CalculeDamageValue(B, I, (MobManager.Classe)C);
            S = B + F;
            Debug.Log("Mob:       Teste "+ (MobManager.Classe)C);
            Debug.Log("Classe:     " + (MobManager.Classe)C);
            Debug.Log("Base:       " + B);
            Debug.Log("Individual: " + I);
            Debug.Log("Final:     " + F);
            Debug.Log("Soma:      " + S);
            Debug.Log("__________");
        }
        

        count = Mob.Count;
        for (int i = 0; i < count; i++)
        {
            C = (int)Mob[i]._classe;           
            I = Mob[i]._Idamage;
            B = Mob[i]._statusIaMob[3]._damageMax;
            F = CalculeDamageValue(B,I, GetClasseAtributeDamage(Mob[i]._classe));
            S = B + F;
            string N = Mob[i]._nameHero;

            Debug.Log("__________");
            Debug.Log("Mob:       "+N);
            Debug.Log("Classe:     " + (MobManager.Classe)C);
            Debug.Log("Base:       " + B);
            Debug.Log("Individual: " + I);
            Debug.Log("Final:     " + F);
            Debug.Log("Soma:      " + S);

            TesteCalcule T = new TesteCalcule();
            T.c = N + " ( " + B + " - " + F + ")";
            T.v = S;
            switch ((MobManager.Classe)C)
            {
                case MobManager.Classe.tanker:
                    if (maxTank.Count<=0 ||  F == maxTank[0].v)                   
                        maxTank.Add(T);
                    else
                        if (S > maxTank[0].v)
                    {
                        maxTank.Clear();
                        maxTank.Add(T);
                    }

                    if (maxTank.Count > 0 && S != maxTank[0].v)
                    {
                        if (minTank.Count <= 0 || S == minTank[0].v)
                            minTank.Add(T);
                        else
                          if (S < minTank[0].v)
                        {
                            minTank.Clear();
                            minTank.Add(T);
                        }
                    }                    
                    break;

                case MobManager.Classe.adc:
                    if (maxAdc.Count<=0 || S == maxAdc[0].v)
                        maxAdc.Add(T);
                    else
                        if (S > maxAdc[0].v)
                    {
                        maxAdc.Clear();
                        maxAdc.Add(T);
                    }

                    if (maxAdc.Count > 0 && S != maxAdc[0].v)
                    {
                        if (minAdc.Count <= 0 || S == minAdc[0].v)
                            minAdc.Add(T);
                        else
                       if (S < minAdc[0].v)
                        {
                            minAdc.Clear();
                            minAdc.Add(T);
                        }
                    }
                    break;
                case MobManager.Classe.suporte:
                    if (maxSup.Count <= 0 || S == maxSup[0].v)
                        maxSup.Add(T);
                    else
                        if (S > maxSup[0].v)
                    {
                        maxSup.Clear();
                        maxSup.Add(T);
                    }

                    if (maxSup.Count > 0 && S != maxSup[0].v)
                    {
                        if (minSup.Count <= 0 || S == minSup[0].v)
                            minSup.Add(T);
                        else
                       if (S < minSup[0].v)
                        {
                            minSup.Clear();
                            minSup.Add(T);
                        }
                    }
                    break;
                case MobManager.Classe.assassino:
                    if (maxAss.Count <= 0 || S == maxAss[0].v)
                        maxAss.Add(T);
                    else
                        if (S > maxAss[0].v)
                    {
                        maxAss.Clear();
                        maxAss.Add(T);
                    }

                    if ( maxAss.Count > 0 && S != maxAss[0].v)
                    {
                        if (minAss.Count <= 0 || F == minAss[0].v)
                            minAss.Add(T);
                        else
                       if (S < minAss[0].v)
                        {
                            minAss.Clear();
                            minAss.Add(T);
                        }
                    }
                    break;
                case MobManager.Classe.soldado:
                    if (maxSold.Count <= 0 || F == maxSold[0].v)
                        maxSold.Add(T);
                    else
                        if (S > maxSold[0].v)
                    {
                        maxSold.Clear();
                        maxSold.Add(T);
                    }

                    if (maxSold.Count > 0 && S != maxSold[0].v)
                    {
                        if (minSold.Count <= 0 || S == minSold[0].v)
                            minSold.Add(T);
                        else
                       if (S < minSold[0].v)
                        {
                            minSold.Clear();
                            minSold.Add(T);
                        }
                    }
                    break;
                case MobManager.Classe.mago:
                    if (maxMago.Count <= 0 || S == maxMago[0].v)
                        maxMago.Add(T);
                    else
                        if (S > maxMago[0].v)
                    {
                        maxMago.Clear();
                        maxMago.Add(T);
                    }
                    if (maxMago.Count > 0 && S != maxMago[0].v)
                    {
                        if (minMago.Count <= 0 || S == minMago[0].v)
                            minMago.Add(T);
                        else
                       if (S < minMago[0].v)
                        {
                            minMago.Clear();
                            minMago.Add(T);
                        }
                    }
                    break;
            }
        }

        Debug.LogWarning("__________");
        for (int i = 0; i < maxTank.Count; i++)
            Debug.LogWarning("Tank Max "+Attribute+" :"+maxTank[i].c+", "+maxTank[i].v);
        for (int i = 0; i < minTank.Count; i++)
            Debug.LogWarning("Tank Min "+Attribute+" :" + minTank[i].c + ", " + minTank[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxAdc.Count; i++)
            Debug.LogWarning("Adc Max "+Attribute+" :" + maxAdc[i].c + ", " + maxAdc[i].v);
        for (int i = 0; i < minAdc.Count; i++)
            Debug.LogWarning("Adc Min "+Attribute+" :" + minAdc[i].c + ", " + minAdc[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxSup.Count; i++)
            Debug.LogWarning("Sup Max "+Attribute+" :" + maxSup[i].c + ", " + maxSup[i].v);
        for (int i = 0; i < minSup.Count; i++)
            Debug.LogWarning("Sup Min "+Attribute+" :" + minSup[i].c + ", " + minSup[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxAss.Count; i++)
            Debug.LogWarning("Ass Max "+Attribute+" :" + maxAss[i].c + ", " + maxAss[i].v);
        for (int i = 0; i < minAss.Count; i++)
            Debug.LogWarning("Ass Min "+Attribute+" :" + minAss[i].c + ", " + minAss[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxSold.Count; i++)
            Debug.LogWarning("Sold Max "+Attribute+" :" + maxSold[i].c + ", " + maxSold[i].v);
        for (int i = 0; i < minSold.Count; i++)
            Debug.LogWarning("Sold Min "+Attribute+" :" + minSold[i].c + ", " + minSold[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxMago.Count; i++)
            Debug.LogWarning("Mago Max "+Attribute+" :" + maxMago[i].c + ", " + maxMago[i].v);
        for (int i = 0; i < minMago.Count; i++)
            Debug.LogWarning("Mago Min "+Attribute+" :" + minMago[i].c + ", " + minMago[i].v);
    }
    public void ContextTesteHealthMobs()
    {
        string Attribute = "Health";
        int B = 10, C = 0, F, S;
        float I = Mob[0]._Ihealth;

        List<TesteCalcule>
                     maxTank = new List<TesteCalcule>(),
                     minTank = new List<TesteCalcule>(),
                     maxAdc = new List<TesteCalcule>(),
                     minAdc = new List<TesteCalcule>(),
                     maxSup = new List<TesteCalcule>(),
                     minSup = new List<TesteCalcule>(),
                     maxAss = new List<TesteCalcule>(),
                     minAss = new List<TesteCalcule>(),
                     maxSold = new List<TesteCalcule>(),
                     minSold = new List<TesteCalcule>(),
                     maxMago = new List<TesteCalcule>(),
                     minMago = new List<TesteCalcule>();

        var values = System.Enum.GetValues(typeof(MobManager.Classe));
        int count = values.Length - 1;


        Debug.Log("**<color=red>"+Attribute+"</color>**");
        Debug.Log("__________");
        for (int c = 2; c < count; c++)
        {
            C = c;
            F = CalculeHealthValue(B, I, GetClasseAtributeHealth((MobManager.Classe)C));
            S = B + F;
            Debug.Log("Mob:       Teste " + (MobManager.Classe)C);
            Debug.Log("Classe:     " + (MobManager.Classe)C);
            Debug.Log("Base:       " + B);
            Debug.Log("Individual: " + I);
            Debug.Log("Final:     " + F);
            Debug.Log("Soma:      " + S);
            Debug.Log("__________");
        }


        count = Mob.Count;
        for (int i = 0; i < count; i++)
        {
            C = (int)Mob[i]._classe;          
            I = Mob[i]._Ihealth;
            B = (int)Mob[i]._statusIaMob[3]._healthMax;
            F = CalculeHealthValue(B,I, GetClasseAtributeHealth((MobManager.Classe)C));
            S = B + F;
            string N = Mob[i]._nameHero;

            Debug.Log("__________");
            Debug.Log("Mob:       " + N);
            Debug.Log("Classe:     " + (MobManager.Classe)C);
            Debug.Log("Base:       " + B);
            Debug.Log("Individual: " + I);
            Debug.Log("Final:     " + F);
            Debug.Log("Soma:      " + S);

            TesteCalcule T = new TesteCalcule();
            T.c = N + " ( " + B + " - " + F + ")";
            T.v = S;
            switch ((MobManager.Classe)C)
            {
                case MobManager.Classe.tanker:
                    if (maxTank.Count <= 0 || S == maxTank[0].v)
                        maxTank.Add(T);
                    else
                        if (S > maxTank[0].v)
                    {
                        maxTank.Clear();
                        maxTank.Add(T);
                    }

                    if (maxTank.Count > 0 && S != maxTank[0].v)
                    {
                        if (minTank.Count <= 0 || S == minTank[0].v)
                            minTank.Add(T);
                        else
                          if (S < minTank[0].v)
                        {
                            minTank.Clear();
                            minTank.Add(T);
                        }
                    }
                    break;

                case MobManager.Classe.adc:
                    if (maxAdc.Count <= 0 || S == maxAdc[0].v)
                        maxAdc.Add(T);
                    else
                        if (S > maxAdc[0].v)
                    {
                        maxAdc.Clear();
                        maxAdc.Add(T);
                    }

                    if (maxAdc.Count > 0 && S != maxAdc[0].v)
                    {
                        if (minAdc.Count <= 0 || S == maxAdc[0].v)
                            minAdc.Add(T);
                        else
                       if (S < minAdc[0].v)
                        {
                            minAdc.Clear();
                            minAdc.Add(T);
                        }
                    }
                    break;
                case MobManager.Classe.suporte:
                    if (maxSup.Count <= 0 || S == maxSup[0].v)
                        maxSup.Add(T);
                    else
                        if (S > maxSup[0].v)
                    {
                        maxSup.Clear();
                        maxSup.Add(T);
                    }

                    if (maxSup.Count > 0 && S != maxSup[0].v)
                    {
                        if (minSup.Count <= 0 || S == minSup[0].v)
                            minSup.Add(T);
                        else
                       if (S < minSup[0].v)
                        {
                            minSup.Clear();
                            minSup.Add(T);
                        }
                    }
                    break;
                case MobManager.Classe.assassino:
                    if (maxAss.Count <= 0 || S == maxAss[0].v)
                        maxAss.Add(T);
                    else
                        if (S > maxAss[0].v)
                    {
                        maxAss.Clear();
                        maxAss.Add(T);
                    }

                    if (maxAss.Count > 0 && S != maxAss[0].v)
                    {
                        if (minAss.Count <= 0 || F == minAss[0].v)
                            minAss.Add(T);
                        else
                       if (S < minAss[0].v)
                        {
                            minAss.Clear();
                            minAss.Add(T);
                        }
                    }
                    break;
                case MobManager.Classe.soldado:
                    if (maxSold.Count <= 0 || F == maxSold[0].v)
                        maxSold.Add(T);
                    else
                        if (S > maxSold[0].v)
                    {
                        maxSold.Clear();
                        maxSold.Add(T);
                    }

                    if (maxSold.Count > 0 && S != maxSold[0].v)
                    {
                        if (minSold.Count <= 0 || S == minSold[0].v)
                            minSold.Add(T);
                        else
                       if (S < minSold[0].v)
                        {
                            minSold.Clear();
                            minSold.Add(T);
                        }
                    }
                    break;
                case MobManager.Classe.mago:
                    if (maxMago.Count <= 0 || S == maxMago[0].v)
                        maxMago.Add(T);
                    else
                        if (S > maxMago[0].v)
                    {
                        maxMago.Clear();
                        maxMago.Add(T);
                    }
                    if (maxMago.Count > 0 && S != maxMago[0].v)
                    {
                        if (minMago.Count <= 0 || S == minMago[0].v)
                            minMago.Add(T);
                        else
                       if (S < minMago[0].v)
                        {
                            minMago.Clear();
                            minMago.Add(T);
                        }
                    }
                    break;
            }
        }
        Debug.LogWarning("__________");
        for (int i = 0; i < maxTank.Count; i++)
            Debug.LogWarning("Tank Max "+Attribute+" :" + maxTank[i].c + ", " + maxTank[i].v);
        for (int i = 0; i < minTank.Count; i++)
            Debug.LogWarning("Tank Min "+Attribute+" :" + minTank[i].c + ", " + minTank[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxAdc.Count; i++)
            Debug.LogWarning("Adc Max "+Attribute+" :" + maxAdc[i].c + ", " + maxAdc[i].v);
        for (int i = 0; i < minAdc.Count; i++)
            Debug.LogWarning("Adc Min "+Attribute+" :" + minAdc[i].c + ", " + minAdc[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxSup.Count; i++)
            Debug.LogWarning("Sup Max "+Attribute+" :" + maxSup[i].c + ", " + maxSup[i].v);
        for (int i = 0; i < minSup.Count; i++)
            Debug.LogWarning("Sup Min "+Attribute+" :" + minSup[i].c + ", " + minSup[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxAss.Count; i++)
            Debug.LogWarning("Ass Max "+Attribute+" :" + maxAss[i].c + ", " + maxAss[i].v);
        for (int i = 0; i < minAss.Count; i++)
            Debug.LogWarning("Ass Min "+Attribute+" :" + minAss[i].c + ", " + minAss[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxSold.Count; i++)
            Debug.LogWarning("Sold Max "+Attribute+" :" + maxSold[i].c + ", " + maxSold[i].v);
        for (int i = 0; i < minSold.Count; i++)
            Debug.LogWarning("Sold Min "+Attribute+" :" + minSold[i].c + ", " + minSold[i].v);

        Debug.LogWarning("__________");
        for (int i = 0; i < maxMago.Count; i++)
            Debug.LogWarning("Mago Max "+Attribute+" :" + maxMago[i].c + ", " + maxMago[i].v);
        for (int i = 0; i < minMago.Count; i++)
            Debug.LogWarning("Mago Min "+Attribute+" :" + minMago[i].c + ", " + minMago[i].v);
    }
    public void ContextResetI()
    {
        for (int i = 1; i < Mob.Count; i++)
        {
            Mob[i]._Idamage = Mob[0]._Idamage;
            Mob[i]._Ihealth = Mob[0]._Ihealth;
            //Mob[i]._Iwalk = Mob[0]._Iwalk;
        }
    }
    public void ContextCalculeAll()
    {
       Game_Mode old = GameMode;

       _gameMode = Game_Mode.Battle;

        List<TesteCalcule>
                     maxDano = new List<TesteCalcule>(),
                     maxHp = new List<TesteCalcule>(),
                     maxWalk = new List<TesteCalcule>(),
                     minDano = new List<TesteCalcule>(),
                     minHp = new List<TesteCalcule>(),
                     minWalk = new List<TesteCalcule>();

        for (int i = 0; i < Mob.Count; i++)
        {
            float D = GetHeroDamage(i),
                  H = GetHeroHealth(i),
                  W = GetHeroWalk(i);

            string N = Mob[i]._nameHero,
                   C = Mob[i]._classe.ToString();

            int Vd = CalculeDamageValue(i, Mob[i]._classe),
                Vh = CalculeHealthValue(i, Mob[i]._classe);

            #region Max
            if (maxDano.Count > 0)
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)D;

                if (D > maxDano[0].v)
                {
                    maxDano.Clear();
                    maxDano.Add(T);
                }
                else
                if (D == maxDano[0].v)
                    maxDano.Add(T);
            }
            else
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)D;

                maxDano.Add(T);
            }

            if (maxHp.Count > 0)
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)H;

                if (H > maxHp[0].v)
                {
                    maxHp.Clear();
                    maxHp.Add(T);
                }
                else
                if (H == maxHp[0].v)
                    maxHp.Add(T);
            }
            else
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)H;
                maxHp.Add(T);
            }

            if (maxWalk.Count != 0 )
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)W;

                if (W > maxWalk[0].v)
                {
                    maxWalk.Clear();
                    maxWalk.Add(T);
                }
                else
                if (W == maxWalk[0].v)
                    maxWalk.Add(T);
            }
            else
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)W;
                maxWalk.Add(T);
            }
            #endregion

            #region min
            if (minDano.Count > 0)
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)D;

                if (D < minDano[0].v)
                {
                    minDano.Clear();
                    minDano.Add(T);
                }
                else
                if (D == minDano[0].v)
                    minDano.Add(T);
            }
            else
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)D;

                minDano.Add(T);
            }

            if (minHp.Count > 0)
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)H;

                if (H < minHp[0].v)
                {
                    minHp.Clear();
                    minHp.Add(T);
                }
                else
                if (H == minHp[0].v)
                    minHp.Add(T);
            }
            else
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)H;
                minHp.Add(T);
            }

            if (minWalk.Count != 0)
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)W;

                if (W < minWalk[0].v && W!=0)
                {
                    minWalk.Clear();
                    minWalk.Add(T);
                }
                else
                if (W == minWalk[0].v)
                    minWalk.Add(T);
            }
            else
            {
                TesteCalcule T = new TesteCalcule();
                T.c = N + " - " + C;
                T.v = (int)W;
                minWalk.Add(T);
            }
            #endregion

            Debug.Log("Mob:       " + N);
            Debug.Log("Classe:     " + C);
            Debug.Log("Dano:      ["+Mob[i]._statusIaMob[3]._damageMax+" + "+Vd+"] " + D);
            Debug.Log("Hp:        ["+Mob[i]._statusIaMob[3]._healthMax+" + "+Vh+"] " + H);
            Debug.Log("Walk:      " +W);
            Debug.Log("__________");
        }

        _gameMode = old;

        Debug.LogWarning("____<color=red>Maior Dano</color>____");
        for (int i = 0; i < maxDano.Count; i++)
            Debug.LogWarning("<color=red>Dano, " + maxDano[i].c + ": " + maxDano[i].v+ "</color>");
        Debug.LogWarning("____<color=blue>Menor Dano</color>____");
        for (int i = 0; i < minDano.Count; i++)
            Debug.LogWarning("<color=blue>Dano, " + minDano[i].c + ": " + minDano[i].v + "</color>");

        Debug.LogWarning("____<color=red>Maior Hp</color>____");
        for (int i = 0; i < maxHp.Count; i++)
            Debug.LogWarning("<color=red>Hp, " + maxHp[i].c + ": " + maxHp[i].v + "</color>");
        Debug.LogWarning("____<color=blue>Menor Hp</color>____");
        for (int i = 0; i < minHp.Count; i++)
            Debug.LogWarning("<color=blue>Hp, " + minHp[i].c + ": " + minHp[i].v + "</color>");

        Debug.LogWarning("____<color=red>Maior Walk</color>____");
        for (int i = 0; i < maxWalk.Count; i++)
            Debug.LogWarning("<color=red>Walk, " + maxWalk[i].c + ": " + maxWalk[i].v + "</color>");
        Debug.LogWarning("____<color=blue>Menor Walk</color>____");
        for (int i = 0; i < minWalk.Count; i++)
            Debug.LogWarning("<color=blue>Walk, " + minWalk[i].c + ": " + minWalk[i].v + "</color>");
    }
    #endregion

    public int GetHeroDamage(int mobID     , int Dificuldade= -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return 0;

        if (GameManagerScenes.BattleMode)
        {
            // return ;

            return Mob[mobID]._statusIaMob[3]._damageMax + CalculeDamageValue(mobID,Mob[mobID]._classe);
        }
        else
        {
            if (Dificuldade == -1)
                Dificuldade = _dlf;

            if (Dificuldade == 0)
            {
                if (_gms != null)
                {
                    int fase = _gms.FaseAtual;

                    if (fase <= 4)
                        Dificuldade = 0;
                    else
                    if (fase >= 5 && fase <= 8)
                        Dificuldade = 1;
                    else
                        if (fase >= 9)
                        Dificuldade = 2;
                }
            }
            int min = Mob[mobID]._statusIaMob[Dificuldade]._damageMin;
            int max = Mob[mobID]._statusIaMob[Dificuldade]._damageMax + 1;

            return UnityEngine.Random.Range(min, max);
        }       
    }
    public int GetHeroDamage(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);      

        return GetHeroDamage(mobID,Dificuldade);
    }

    public float GetHeroHealth(int mobID     , int Dificuldade = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return -99;

        if (GameManagerScenes.BattleMode)
        {
            //Dificuldade = 3/*Mob[mobID]._statusIaMob.Count - 1*/;

            return Mob[mobID]._statusIaMob[3]._healthMax + CalculeHealthValue(mobID, Mob[mobID]._classe);
        }
        else
        {
            if (Dificuldade == -1)
                Dificuldade = _dlf;

            if (Dificuldade == 0)
            {
                if (_gms != null)
                {
                    int fase = _gms.FaseAtual;

                    if (fase <= 4)
                        Dificuldade = 0;
                    else
                    if (fase >= 5 && fase <= 8)
                        Dificuldade = 1;
                    else
                        if (fase >= 9)
                        Dificuldade = 2;
                }
            }
        }    

        int max = (int)Mob[mobID]._statusIaMob[Dificuldade]._healthMax + 1;
        int min = (int)Mob[mobID]._statusIaMob[Dificuldade]._healthMin;

        int _v = UnityEngine.Random.Range(min, max);

        Debug.LogError("Hero Health: "+ _v);

        return _v;
    }
    public float GetHeroHealth(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);

        return GetHeroHealth(mobID, Dificuldade);
    }

    public float GetHeroCritico(int mobID     , int Dificuldade = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return -99;
        if (GameManagerScenes.BattleMode)
            Dificuldade = 3/*Mob[mobID]._statusIaMob.Count - 1*/;
        else
        {
            if (Dificuldade == -1)
                Dificuldade = _dlf;

            if (Dificuldade == 0)
            {
                if (_gms != null)
                {
                    int fase = _gms.FaseAtual;

                    if (fase <= 4)
                        Dificuldade = 0;
                    else
                    if (fase >= 5 && fase <= 8)
                        Dificuldade = 1;
                    else
                        if (fase >= 9)
                        Dificuldade = 2;
                }
            }
        }
        
        return Mob[mobID]._statusIaMob[Dificuldade]._critico;
    }
    public float GetHeroCritico(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);

        return GetHeroCritico(mobID, Dificuldade);
    }

    public int GetHeroWalk(int mobID, int Dificuldade = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return 0;

        if (GameManagerScenes.BattleMode)
        {
            return Mob[mobID]._statusIaMob[3]._walk + GetClasseAtributeWalk(Mob[mobID]._classe);
        }
        else
        {
            if (Dificuldade == -1)
                Dificuldade = _dlf;

            if (Dificuldade == 0)
            {
                if (_gms != null)
                {
                    int fase = _gms.FaseAtual;

                    if (fase <= 4)
                        Dificuldade = 0;
                    else
                    if (fase >= 5 && fase <= 8)
                        Dificuldade = 1;
                    else
                        if (fase >= 9)
                        Dificuldade = 2;
                }
            }
        }

        return Mob[mobID]._statusIaMob[Dificuldade]._walk;
    }
    public int GetHeroWalk(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);

        return GetHeroWalk(mobID, Dificuldade);
    }

    public int GetHeroAttack(int mobID     , int Dificuldade = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return 0;
        if (GameManagerScenes.BattleMode)
            Dificuldade = Mob[mobID]._statusIaMob.Count - 1;
        else
        {
            if (Dificuldade == -1)
                Dificuldade = _dlf;

            if (Dificuldade == 0)
            {
                if (_gms != null)
                {
                    int fase = _gms.FaseAtual;

                    if (fase <= 4)
                        Dificuldade = 0;
                    else
                    if (fase >= 5 && fase <= 8)
                        Dificuldade = 1;
                    else
                        if (fase >= 9)
                        Dificuldade = 2;
                }
            }
        }
        
        return Mob[mobID]._statusIaMob[Dificuldade]._attack;
    }
    public int GetHeroAttack(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);

        return GetHeroAttack(mobID, Dificuldade);
    }

    public float GetHeroFireResistence(int mobID     , int Dificuldade = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return 0;

        if (Dificuldade == -1)
            Dificuldade = _dlf;

        if (Dificuldade == 0)
        {
            if (_gms != null)
            {
                int fase = _gms.FaseAtual;

                if (fase <= 4)
                    Dificuldade = 0;
                else
                if (fase >= 5 && fase <= 8)
                    Dificuldade = 1;
                else
                    if (fase >= 9)
                    Dificuldade = 2;
            }
        }

        if (GameManagerScenes.BattleMode)
            Dificuldade = Mob[mobID]._statusIaMob.Count - 1;

        return Mob[mobID]._statusIaMob[Dificuldade]._damageResistenceFire*100;
    }
    public float GetHeroFireResistence(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);

        return GetHeroFireResistence(mobID, Dificuldade);
    }

    public float GetHeroPoisonResistence(int mobID     , int Dificuldade = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return 0;

        if (Dificuldade == -1)
            Dificuldade = _dlf;

        if (Dificuldade == 0)
        {
            if (_gms != null)
            {
                int fase = _gms.FaseAtual;

                if (fase <= 4)
                    Dificuldade = 0;
                else
                if (fase >= 5 && fase <= 8)
                    Dificuldade = 1;
                else
                    if (fase >= 9)
                    Dificuldade = 2;
            }
        }

        if (GameManagerScenes.BattleMode)
            Dificuldade = Mob[mobID]._statusIaMob.Count - 1;

        return Mob[mobID]._statusIaMob[Dificuldade]._damageResistencePoison * 100;
    }
    public float GetHeroPoisonResistence(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);

        return GetHeroPoisonResistence(mobID, Dificuldade);
    }

    public float GetHeroPetrifyResistence(int mobID, int Dificuldade = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return 0;

        if (Dificuldade == -1)
            Dificuldade = _dlf;

        if (Dificuldade == 0)
        {
            if (_gms != null)
            {
                int fase = _gms.FaseAtual;

                if (fase <= 4)
                    Dificuldade = 0;
                else
                if (fase >= 5 && fase <= 8)
                    Dificuldade = 1;
                else
                    if (fase >= 9)
                    Dificuldade = 2;
            }
        }

        if (GameManagerScenes.BattleMode)
            Dificuldade = Mob[mobID]._statusIaMob.Count - 1;

        return Mob[mobID]._statusIaMob[Dificuldade]._damageResistencePetrify * 100;
    }
    public float GetHeroPetrifyResistence(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);

        return GetHeroPetrifyResistence(mobID, Dificuldade);
    }

    public float GetHeroBleedResistence(int mobID, int Dificuldade = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID >= PlayerCount || mobID < 0)
            return 0;

        if (Dificuldade == -1)
            Dificuldade = _dlf;

        if (Dificuldade == 0)
        {
            if (_gms != null)
            {
                int fase = _gms.FaseAtual;

                if (fase <= 4)
                    Dificuldade = 0;
                else
                if (fase >= 5 && fase <= 8)
                    Dificuldade = 1;
                else
                    if (fase >= 9)
                    Dificuldade = 2;
            }
        }

        if (GameManagerScenes.BattleMode)
                Dificuldade = Mob[mobID]._statusIaMob.Count-1;

        return Mob[mobID]._statusIaMob[Dificuldade]._damageResistenceBleed * 100;
    }
    public float GetHeroBleedResistence(GameObject mob, int Dificuldade = -1)
    {
        int mobID = HeroID(mob);

        return GetHeroBleedResistence(mobID, Dificuldade);
    }

    #region  -> Desblock / Ban / Change
    [SerializeField] List<HerosBlocked> _mobBanned;
    public           List<HerosBlocked> MobBanned { get { return _mobBanned; } set { _mobBanned = value; } }

    public int HeroID(GameObject mob)
    {
        Debug.LogError("HeroID(" + mob.name + ")");

        int id = -1;

        id = Mob.FindIndex(x => x._prefabHero.name == mob.name);

        print("ID: " + id);

        if (id > -1 && id<PlayerCount)
            return id;

        for (int i = 0; i < PlayerCount; i++)
        {
            if (Mob[i]._prefabHero == mob ||
                Mob[i]._prefabHero.name == mob.name)
            {
                print("ID: " + i);
                return i;
            }

            if(mob.GetComponent<ToolTipType>() && mob.GetComponent<ToolTipType>()._type == ToolTipType.Type.Mob)
            if (Mob[i]._prefabHero.GetComponent<ToolTipType>()._XmlID !=-1 &&
                Mob[i]._prefabHero.GetComponent<ToolTipType>()._XmlID == mob.GetComponent<ToolTipType>()._XmlID)
            {
                print("ID: " + i);
                return i;
            }

            for (int j = 0; j < SkinCount(i); j++)
            {
                if (Mob[i]._skinHero[j]._skinPrefab      == mob ||
                    Mob[i]._skinHero[j]._skinPrefab.name == mob.name)
                {
                    return i;
                }
            }
        }

        Debug.LogError("HeroID(" + mob.name + ") - Não Encontrado");
        return -1;
    }
    public int HeroID(string mobName)
    {
        Debug.LogError("HeroID(" + mobName + ")");

        int id = -1;

        id = Mob.FindIndex(x => x._nameHero == mobName);

        print("ID: " + id);

        if (id > -1 && id < PlayerCount)
            return id;

        for (int i = 0; i < PlayerCount; i++)
        {
            if (Mob[i]._nameHero == mobName)
            {
                print("ID: " + i);
                return i;
            }
        }


        return -1;
    }

    public bool CheckMobBlocked(int mobID = -1)
    {
        if (Adm)
            return false;

        if (mobID == -1)
            mobID = (PlayerID-1);

        if (mobID >= PlayerCount || mobID <= -1)
            return true;

        return Mob[mobID]._blocked;
    }

    /// <summary>
    /// Desbloqueia o mob Colocado
    /// </summary>
    /// <param name="mobID"></param>
    public void MobDesblock(int mobID,bool info=true)
    {
        if (mobID >= PlayerCount || mobID <= -1)
            return;

        if (Adm || CheckMobBlocked(mobID))
        {
            Mob[mobID]._blocked = false;

            PlayerPrefs.SetInt(mobID + "Blocked", Convert.ToInt32(false));
          
            if (info)
            {
                NewInfo(
                    AttDescriçãoMult(
                        XmlMenuInicial.Instance.Get(90)// "Parabens!!!,\n{0}\n Foi Desbloqueado!!!"
                    ,HeroName(mobID)), 3f);

                NewInfo(
                    AttDescriçãoMult(
                        XmlMenuInicial.Instance.Get(91)// "Voce pode jogar com o {0}!!!\nVa no menu Principal em Extras."
                    , HeroName(mobID))
                    , 2);

                //NewInfo(AttDescriçãoMult("Parabens!!!,\n{0}\n Foi Desbloqueado!!!", HeroName(mobID)), 3f);
                //NewInfo(AttDescriçãoMult("Voce pode jogar com o {0}!!!\nVa no menu Principal em Extras.", HeroName(mobID), 2);
            }

            SkinDesblock(mobID, 0, info);
        }
        else
            print(HeroName(mobID) + "\n Ja esta Desbloqueado!!!");
    }
    /// <summary>
    /// Desbloqueia os mobs do valor minimo ate o valor maximo colocado
    /// </summary>
    /// <param name="MinmobID">1</param>
    /// <param name="MaxmobID"></param>
    public void MobDesblock(int MinmobID, int MaxmobID)
    {
        if (MinmobID >= PlayerCount || MinmobID <= -1 ||
            MaxmobID >= PlayerCount || MaxmobID <= -1)
            return;

        for (int i = MinmobID; i < MaxmobID + 1; i++)
        {
            if (Adm || CheckMobBlocked(i))
            {
                Mob[i]._blocked = false;

                PlayerPrefs.SetInt(i + "Blocked", Convert.ToInt32(false));

                NewInfo(
                    AttDescriçãoMult(
                        XmlMenuInicial.Instance.Get(90)// "Parabens!!!,\n{0}\n Foi Desbloqueado!!!"
                    , HeroName(i)), 3f);

                NewInfo(
                    AttDescriçãoMult(
                        XmlMenuInicial.Instance.Get(91)// "Voce pode jogar com o {0}!!!\nVa no menu Principal em Extras."
                    , HeroName(i))
                    , 2);

                SkinDesblock(i, 0);
            }
            else
                print(HeroName(i) + "\n Ja esta Desbloqueado!!!");
        }        
    }
    /// <summary>
    /// Desbloqueia o mob Colocado
    /// </summary>
    /// <param name="mobID"></param>
    public void MobDesblock(int mobID)
    {
        print("MobDesblock(" + mobID + ")");

        if (mobID >= PlayerCount || mobID <= -1)
            return;

        if (Adm || CheckMobBlocked(mobID))
        {
            Mob[mobID]._blocked = false;

            PlayerPrefs.SetInt(mobID + "Blocked", Convert.ToInt32(false));

            NewInfo(
                    AttDescriçãoMult(
                        XmlMenuInicial.Instance.Get(90)// "Parabens!!!,\n{0}\n Foi Desbloqueado!!!"
                    , HeroName(mobID)), 3f);

            NewInfo(
                AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(91)// "Voce pode jogar com o {0}!!!\nVa no menu Principal em Extras."
                , HeroName(mobID))
                , 2);

            //NewInfo(AttDescriçãoMult("Parabens!!!,\n{0}\n Foi Desbloqueado!!!", HeroName(mobID)), 3f);
            //NewInfo(AttDescriçãoMult("Voce pode jogar com o {0}!!!\nVa no menu Principal em Extras.", HeroName(mobID), 2);

            SkinDesblock(mobID, 0);
        }
        else
            print(HeroName(mobID) + "\n Ja esta Desbloqueado!!!");
    }

    public bool CheckMobBanned(int      mobID=-1)
    {
        if (Adm)
            return false;

        if (mobID == -1)
            mobID = (PlayerID-1);

        if (mobID >= 0 || mobID < PlayerCount)
            foreach (var hero in _mobBanned)
            {
                if (hero._heroID == (mobID+1) &&
                    hero._mode   == _gameMode
                                 ||
                    hero._heroID == (mobID+1) &&
                    hero._AllGamesMode)
                {
                    return true;
                }
            }

            return false;
    }
    public bool CheckMobBanned(string   name)
    {
        if (Adm)
            return false;

        int mobID = -1;

        for (int i = 0; i < PlayerCount; i++)
        {
            if (Mob[i]._nameHero == name)
            {
                mobID = i+1;
                break;
            }
        }

        if (mobID >=1 || mobID<PlayerCount)
        {
            foreach (var hero in _mobBanned)
            {
                if (hero._heroID == mobID &&
                    hero._mode == _gameMode
                                 ||
                    hero._heroID == mobID &&
                    hero._AllGamesMode)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void BannedMob(int mobID = -1, Game_Mode modeGame = Game_Mode.History,bool allmode=false)
    {
        if (mobID == -1)
            mobID = (PlayerID);

        if (CheckMobBanned(mobID))
            return;

        HerosBlocked ban = new HerosBlocked();

        ban._heroID       = mobID;
        ban._mode         = modeGame;
        ban._AllGamesMode = allmode;

        MobBanned.Add(ban);

        print(HeroName(mobID)+" foi banido "+modeGame+" - Todos os modos ("+allmode+").");
    }

    public int ChangePlayer(bool inf=true)
    {
        List<int> id = new List<int>();
       
        for (int i = 0; i < PlayerCount; i++)
        {
            if (!Mob[i]._blocked && !CheckMobBanned(i))
            {
                id.Add(i);
                print("ChangePlayer -> "+HeroName(i));
            }
        }

        if (id.Count > 0)
        {
            int ret      = UnityEngine.Random.Range(0, id.Count);

            print("ChangePlayer = " + HeroName(ret) + " - Skin => " + SkinName(ret, PlayerPrefs.GetInt(id[ret] + "Skin")));

            PlayerID     = id[ret]+1;
            _playerId    = id[ret]+1;

            PlayerSkinID  = PlayerPrefs.GetInt(id[ret] + "Skin");
            _playerSkinId = PlayerPrefs.GetInt(id[ret] + "Skin");

            if (inf)
            NewInfo(
                AttDescriçãoMult(
                    XmlMenuInicial.Instance!=null ?
                    XmlMenuInicial.Instance.Get(92)// "Player trocado para:\n{0} [{1}]"
                    : "Player trocado para:\n{0} [{1}]"
                , HeroName(PlayerID - 1)
                , SkinName())
                , 2.5f);

            return ret;
        }
        else
            return -1;
    }
    #endregion

    #region Skin
    public int GetSkinId(GameObject Skin)
    {
       
        int mobId = HeroID(Skin);
        int count = SkinCount(mobId);
        for (int i = 0; i < count; i++)
        {
            if (Skin == Mob[mobId]._prefabHero)
            {
                return i;
            }
        }

        return 0;
    }

    public bool ChangeSkin(int mobID = -1, int skinID = 0)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (skinID == -1)
            skinID = PlayerSkinID;

        if (CheckSkinBlocked(mobID, skinID))
        {
            NewInfo(
                AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(89)// "Opss!!!\nA Skin[({0})] do mob (({1})) que esta selecionado esta Bloqueada!!!"
                , SkinName()
                , HeroName(PlayerID - 1))
                , 2.5f);

            return false;
        }

        PlayerSkinID = skinID;

        return true;
    }

    public bool CheckSkinBlocked(int mobID = -1,int skinID=-1)
    {
        if (Adm)
            return false;

        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (skinID == -1)
            skinID = PlayerSkinID;

        if (mobID <= -1  || mobID >= PlayerCount ||
            skinID <= -1 || skinID >= SkinCount(mobID))
        {
            Debug.LogWarning("Skin "+skinID+" do "+HeroName(mobID)+"["+mobID+"] esta bloqueada");
            return true;
        }

        Debug.LogWarning("Skin " + skinID + " do " + HeroName(mobID) + " - Block: "+ Mob[mobID]._skinHero[skinID]._skinBlocked);

        return Mob[mobID]._skinHero[skinID]._skinBlocked;
    }

    public string SkinName(int mobID = -1, int skinID = -1,bool color=true)
    {       
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (skinID == -1)
            skinID = PlayerSkinID;

        //print("SkinName(" + mobID + "," + skinID + ")");

        if (mobID  <= -1 || mobID  >= PlayerCount ||
            skinID <= -1 || skinID >= SkinCount(mobID))
            return "<color=red>"+AttDescriçãoMult(XmlMenuInicial.Instance.Get(116), "Name Skin["+skinID+"]") + "</color>";//ERRO, {0} Não encontrado

        return 
            color ? "<color=" + GetColorTypeSkin(mobID, skinID) + ">" +
            (XmlMobManager.Instance !=null ? XmlMobManager.Instance.SkinName(mobID, skinID) + "</color>"
            : Mob[mobID]._skinHero[skinID]._nameSkin + "</color>") 
            : 
            (XmlMobManager.Instance != null ? XmlMobManager.Instance.SkinName(mobID, skinID)
            : Mob[mobID]._skinHero[skinID]._nameSkin);// Mob[mobID]._skinHero[skinID]._nameSkin
    }

    public string GetSkinType(int mobID = -1, int skinID = -1,bool retunBasic=true)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (skinID == -1)
            skinID = PlayerSkinID;

        //print("SkinName(" + mobID + "," + skinID + ")");

        if (mobID <= -1 || mobID >= PlayerCount ||
            skinID <= -1 || skinID >= SkinCount(mobID))
            return AttDescriçãoMult(XmlMenuInicial.Instance.Get(116), "Type Skin");//ERRO, {0} Não encontrado

        if (Mob[mobID]._skinHero[skinID]._skinType == 0)
            return "";

        return "<color=" + GetColorTypeSkin(mobID, skinID) + ">" + GetSkinType(Mob[mobID]._skinHero[skinID]._skinType) +"</color>";
    }

    public GameObject SkinHero(int mobID, int skinID)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (skinID == -1)
            skinID = PlayerSkinID;

        if (mobID >= PlayerCount || mobID <= -1)
            return null;

        GameObject Skin = Mob[mobID]._prefabHero;

        if (skinID <= -2 || skinID >= SkinCount(mobID))
            return Skin;

        if (Mob[mobID]._skinHero[skinID]._skinPrefab != null)
        {
            Skin = Mob[mobID]._skinHero[skinID]._skinPrefab;

            MobManager mm = Skin.GetComponent<MobManager>();

            /*mm.classe = Mob[mobID]._classe;
            mm.damage = Mob[mobID]._damage;
            mm.health = Mob[mobID]._health;

            mm.maxTimeWalk       = Mob[mobID]._walk;
            mm.currentTimeAttack = Mob[mobID]._attack;

            mm.chanceCritical = Mob[mobID]._critico*100;

            mm.DamageResistenceFire    = Mob[mobID]._damageResistenceFire   *100;
            mm.DamageResistencePoison  = Mob[mobID]._damageResistencePoison *100;
            mm.DamageResistencePetrify = Mob[mobID]._damageResistencePetrify*100;
            mm.DamageResistenceBleed   = Mob[mobID]._damageResistenceBleed  *100;*/

            return Skin;
        }

        Skin.GetComponent<MeshFilter>().mesh        = Mob[mobID]._skinHero[skinID]._skinMesh;
        Skin.GetComponent<MeshRenderer>().materials = Mob[mobID]._skinHero[skinID]._skinMaterials;

        return Skin;
    }

    public GameObject SkinHero(GameObject mob, int skinID)
    {
        int mobID = HeroID(mob);

        if (mobID >= PlayerCount || mobID <= -1)
            return mob;

        GameObject Skin = Mob[mobID]._prefabHero;

        if (skinID <= -1 || skinID >= SkinCount(mobID))
            return Skin;

        if (Mob[mobID]._skinHero[skinID]._skinPrefab != null)
        {
            Skin = Mob[mobID]._skinHero[skinID]._skinPrefab;

            MobManager mm = Skin.GetComponent<MobManager>();

            mm.classe = Mob[mobID]._classe;
            mm.damage = Mob[mobID]._damage;
            mm.health = Mob[mobID]._health;

            mm.maxTimeWalk       = Mob[mobID]._walk;
            mm.currentTimeAttack = Mob[mobID]._attack;

            mm.chanceCritical    = Mob[mobID]._critico;

            mm.DamageResistenceFire    = Mob[mobID]._damageResistenceFire;
            mm.DamageResistencePoison  = Mob[mobID]._damageResistencePoison;
            mm.DamageResistencePetrify = Mob[mobID]._damageResistencePetrify;
            mm.DamageResistenceBleed   = Mob[mobID]._damageResistenceBleed;

            return Skin;
        }

        Skin.GetComponent<MeshFilter>().mesh        = Mob[mobID]._skinHero[skinID]._skinMesh;
        Skin.GetComponent<MeshRenderer>().materials = Mob[mobID]._skinHero[skinID]._skinMaterials;

        PlayerPrefs.SetInt("PlayerSkinId", skinID);

        return Skin;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="St"></param>
    /// <returns></returns>
    public string GetSkinType(SkinType St)
    {
        switch (St)
        {
            case SkinType.Basica:
                return XmlMenuInicial.Instance.Get(111);
            case SkinType.Normal:
                return XmlMenuInicial.Instance.Get(112);
            case SkinType.Rara:
                return XmlMenuInicial.Instance.Get(113);
            case SkinType.Lendaria:
                return XmlMenuInicial.Instance.Get(114);
            case SkinType.Limitada:
                return XmlMenuInicial.Instance.Get(115);
            default:
                return AttDescriçãoMult(XmlMenuInicial.Instance.Get(116),"Tipo Skin");//ERRO, {0} Não encontrado
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="St"></param>
    /// <returns></returns>
    public string GetSkinTag(SkinTag St)
    {
        switch (St)
        {
            case SkinTag.Effect:
                return XmlMenuInicial.Instance.Get(117);
            case SkinTag.Skill:
                return XmlMenuInicial.Instance.Get(118);
            case SkinTag.Name_Skill:
                return XmlMenuInicial.Instance.Get(119);
            case SkinTag.Model:
                return XmlMenuInicial.Instance.Get(120);
            case SkinTag.Arma:
                return XmlMenuInicial.Instance.Get(121);
            default:
                return AttDescriçãoMult(XmlMenuInicial.Instance.Get(116), "Tag Skin");//ERRO, {0} Não encontrado
        }
    }

    /// <summary>
    /// Desbloqueia a skin do mob Colocado
    /// </summary>
    /// <param name="mobID"></param>
    /// <param name="skinID"></param>
    /// <param name="info"></param>
    public void SkinDesblock(int mobID, int skinID, bool info = true)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID < 0 || mobID >= PlayerCount ||
            skinID < 0 || skinID >= SkinCount(mobID))
            return;

        if (CheckSkinBlocked(mobID,skinID))
        {
            DesblockSkin(mobID, skinID,info);
        }
        else
             if (info)
            NewInfo("<color=red>"+ HeroName(mobID)+" [<color=blue>"+ SkinName(mobID, skinID) + "</color>]\n Ja esta Desbloqueada!!!</color>", 4);

        NewInfo(
                AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(93)// "<color=red>{0} [<color=blue>{1}</color>]\n Ja esta Desbloqueada!!!</color>"
                , HeroName(mobID)
                , SkinName(mobID, skinID))
                , 4);
    }
    /// <summary>
    /// Desbloqueia o mob Colocado
    /// </summary>
    /// <param name="mobID"></param>
    public void SkinDesblock(int mobID, int skinID)
    {
        if(mobID == -1)
            mobID = (PlayerID - 1);

        if (mobID < 0 || mobID >= PlayerCount ||
            skinID < 0 || skinID >= SkinCount(mobID))
            return;

        if (CheckSkinBlocked(mobID, skinID))
        {
            DesblockSkin(mobID, skinID);
        }
        else
            print("skin[" + SkinName(mobID, skinID) + "] para o " + HeroName(mobID) + "\n Ja esta Desbloqueada!!!");
    }

    /// <summary>
    /// Não utilizar esse
    /// </summary>
    /// <param name="mobID"></param>
    /// <param name="skinID"></param>
    /// <param name="info"></param>
    void DesblockSkin(int mobID, int skinID, bool info = true)
    {
        Mob[mobID]._skinHero[skinID]._skinBlocked = false;

        PlayerPrefs.SetInt(mobID + "Skin" + skinID, Convert.ToInt32(false));

        if (info && skinID != 0 || adm)
        {
            NewInfo(
                AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(94)// "Parabens!!!,\nDesbloqueou uma nova skin {0} para o(a) {1}!!!\n{2}"
                , GetSkinType(mobID, skinID)
                , HeroName(mobID)
                , SkinName(mobID, skinID))
                , 3.5f);
        }

        //ChangeSkin(mobID, skinID);
    }

    public string GetColorTypeSkin(int mobID = -1, int skinID = -1)
    {
        if (mobID == -1)
            mobID = (PlayerID - 1);

        if (skinID == -1)
            skinID = PlayerSkinID;

        string c = ColorUtility.ToHtmlStringRGBA(_colorTypeSkin[0]);

        if (mobID <= -1 || mobID >= PlayerCount ||
            skinID <= -1 || skinID >= SkinCount(mobID))
            return c;

         c = ColorUtility.ToHtmlStringRGBA(_colorTypeSkin[(int)Mob[mobID]._skinHero[skinID]._skinType]);

        return "#" + c;
    }
    #endregion
    #endregion

    #region Prop's Player
    [Header("Player Prop's")]
    [SerializeField, Tooltip("Damage Player")]
    int _damage;
    [SerializeField, Tooltip("Vida Player")]
    float _health;
    [SerializeField, Tooltip("Passos Player")]
    int _walk;
    [SerializeField, Tooltip("Critical Chance Player")]
    float _critical;

    public int   Damage(int value = -1, bool bonus = true)
    {
        _damage = PlayerPrefs.GetInt("Damage");

        if (value != -1)
        {
            _damage = value;
            PlayerPrefs.SetInt("Damage", value);
        }
        if (bonus)
            GetBonus(getBonus.damage);

        return _damage;
    }
    public float Health(float value = -1, bool bonus = true)
    {
        _health = PlayerPrefs.GetFloat("Health");

        if (value != -1)
        {
            _health = value;
            PlayerPrefs.SetFloat("Health", value);
        }
        if (bonus)
            GetBonus(getBonus.health);

        return _health;
    }
    public int   Walk  (int value = -1, bool bonus = true)
    {
        _walk = PlayerPrefs.GetInt("Walk");

        if (value != -1)
        {
            _walk = value;
            PlayerPrefs.SetInt("Walk", value);
        }
        if (bonus)
            GetBonus(getBonus.walk);

        return _walk;
    }
    #endregion

    #region Player Resistence's
    [Header("Player Resistence's")]
    [SerializeField]
    float _fireResistence;
    [SerializeField]
    float _poisonResistence;
    [SerializeField]
    float _petrifyResistence;
    [SerializeField]
    float _bleedResistence;

    public float FireResistence(float value = -1, bool bonus = true)
    {
        _fireResistence = PlayerPrefs.GetFloat("FireResistence");

        if (value != -1)
        {
            _fireResistence = value;
            PlayerPrefs.SetFloat("FireResistence", value);
        }
        if (bonus)
            GetBonus(getBonus.resistenceFire);

        return _fireResistence;
    }
    public float PoisonResistence(float value = -1, bool bonus = true)
    {
        _poisonResistence = PlayerPrefs.GetFloat("PoisonResistence");

        if (value != -1)
        {
            _poisonResistence = value;
            PlayerPrefs.SetFloat("PoisonResistence", value);
        }
        if (bonus)
            GetBonus(getBonus.resistencePoison);

        return _poisonResistence;
    }
    public float PetrifyResistence(float value = -1, bool bonus = true)
    {
        _petrifyResistence = PlayerPrefs.GetFloat("PetrifyResistence");

        if (value != -1)
        {
            _petrifyResistence = value;
            PlayerPrefs.SetFloat("PetrifyResistence", value);
        }
        if (bonus)
            GetBonus(getBonus.resistencePetrify);

        return _petrifyResistence;
    }
    public float BleedResistence(float value = -1, bool bonus = true)
    {
        _bleedResistence = PlayerPrefs.GetFloat("BleedResistence");

        if (value != -1)
        {
            _bleedResistence = value;
            PlayerPrefs.SetFloat("BleedResistence", value);
        }
        if (bonus)
            GetBonus(getBonus.resistenceBleed);

        return _bleedResistence;
    }
    #endregion

    #region Config
    [Header("Config")]
    [SerializeField]
    bool _isMobile = false;
    
    public bool IsMobile
    {
        get { return _isMobile; }
    }

    void CheckMobile()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
        _isMobile = true;

        Debug.LogWarning("Is Mobile");
#else
        _isMobile = false;

        Debug.LogWarning("Isn't Mobile");
#endif
    }

    [SerializeField]
    bool _moveCameraArrow = false;
    [SerializeField]
    float _mouseSensibility = 0;
    [SerializeField]
    dificuldade _dificuldade = dificuldade.medio;
    public enum dificuldade
    {
        auto,
        facil,
        medio,
        dificil,
    }
    [SerializeField]
    Game_language _language = Game_language.PT_BR;

    public delegate void  ChangeLanguageXml();
    public static   event ChangeLanguageXml DelegateChangeLanguageXml;

    public bool MoveCameraArrow
    {
        get
        {
            if (PlayerPrefs.HasKey("MoveCameraArrow"))
                return Convert.ToBoolean(PlayerPrefs.GetInt("MoveCameraArrow"));
            else
                return _moveCameraArrow;
        }
        set
        {
            _moveCameraArrow = Convert.ToBoolean(value);

            PlayerPrefs.SetInt("MoveCameraArrow",Convert.ToInt32(value));
        }
    }   

    public float MouseSensibility
    {
        get
        {
            if (PlayerPrefs.HasKey("MouseSensibility"))
                return PlayerPrefs.GetFloat("MouseSensibility");
            else
                return _mouseSensibility;
        }
        set
        {
            _mouseSensibility = value;
            PlayerPrefs.SetFloat("MouseSensibility", value);
        }
    }

    public dificuldade   Dificuldade(bool Change = false, dificuldade d = dificuldade.facil)
    {
        if (Change)
        {
            _dificuldade = d;
            PlayerPrefs.SetInt("Dificuldade", Convert.ToInt32(d));
            print("Dificuldade["+_dlf+"] Mudou para " + d.ToString());
            _gms.Balance();
        }
        else
            d = _dificuldade;

        return d;
    }

    public void ChangeDificuldade(int d = 0)
    {
        switch (d)
        {
            case 0:
                Dificuldade(true, dificuldade.auto);
                break;
            case 1:
                Dificuldade(true, dificuldade.facil);
                break;
            case 2:
                Dificuldade(true, dificuldade.medio);
                break;
            case 3:
                Dificuldade(true, dificuldade.dificil);
                break;
        }

        PlayerPrefs.SetInt("Dificuldade", Convert.ToInt32(d));
        _gms.Balance();
    }

    public string DificuldadeString(int d = -1)
    {
        if (d==-1)        
            d = (int)_dificuldade;       

        switch (d)
        {
            case 0:
                return  /*_gms.IsMobile ? "AUTO" :*/ XmlMenuInicial.Instance.Get(81);
            case 1:
                return/* _gms.IsMobile ? "FACIL" :*/ XmlMenuInicial.Instance.Get(82);
            case 2:
                return /*_gms.IsMobile ? "MÉDIO" :*/ XmlMenuInicial.Instance.Get(83);
            case 3:
                return /*_gms.IsMobile ? "DIFICIL" :*/ XmlMenuInicial.Instance.Get(84);
        }

        return "Erro_NotFound";
    }

    public Game_language Language(bool Change = false, Game_language l = Game_language.PT_BR)
    {
        if (Change)
        {
            _language = l;
            PlayerPrefs.SetInt("Language", Convert.ToInt32(l));

            print("Lingua alterada para " + l.ToString());

            if (DelegateChangeLanguageXml != null)
                DelegateChangeLanguageXml();
        }
        else
            l = _language;

        return l;
    }
  
    public void ChangeLanguage(int l = 0)
    {
        switch (l)
        {
            case 0:
                Language(true, Game_language.PT_BR);
                break;
            case 1:
                Language(true, Game_language.EN_US);
                break;
           /* case 2:
                Language(true, dificuldade.medio);
                break;
            case 3:
                Language(true, dificuldade.dificil);
                break;*/
        }
    }
   
    #region Sound
    [Header("Sound")]
    [SerializeField]
    AudioMixer _audioMixer;
    [SerializeField]
    float _soundEffect = 0;
    [SerializeField]
    float _soundMusic = 0;

    public float SoundEffect
    {
        get
        {
            if (PlayerPrefs.HasKey("SoundEffect"))
                return PlayerPrefs.GetFloat("SoundEffect");
            else
               return _soundEffect;
        }
        set
        {
            _soundEffect = value;
            PlayerPrefs.SetFloat("SoundEffect", _soundEffect);
            _audioMixer.SetFloat("EffectVol", _soundEffect);
        }
    }

    public float SoundMusic
    {
        get
        {
            if (PlayerPrefs.HasKey("SoundMusic"))
                return PlayerPrefs.GetFloat("SoundMusic");
            else
                return _soundMusic;
        }
        set
        {
            _soundMusic = value;
            PlayerPrefs.SetFloat("SoundMusic", _soundMusic);
            _audioMixer.SetFloat("MusicVol", _soundMusic);
        }
    }
    #endregion
    #endregion

    #region GET_BONUS
    [Header("Zé Bonus")]
    [SerializeField, Tooltip("Ganha quando completa fase por entrar no porta, caso seja 0 vai o bonus kill por completar fase")]
    int   _portalBonusDamage;
    [SerializeField, Tooltip("Ganha quando completa fase por entrar no porta, caso seja 0 vai o bonus kill por completar fase")]
    float _portalBonusHealth;
    [Space]
    [SerializeField, Tooltip("Ganha quando completa fase por kill")]
    int   _killBonusDamage;
    [SerializeField, Tooltip("Ganha quando completa fase por kill")]
    float _killBonusHealth;
    [Space]
    [SerializeField, Tooltip("Para dar bonus walk and resistences")]
    int[] _fasesGetBonus;
    [Space]
    [SerializeField, Tooltip("Ganha quando completa fase por kill")]
    float _bonusCritical;
    [SerializeField, Tooltip("retira % do dano")]
    float _bonusFireResistence;
    [SerializeField, Tooltip("retira % do dano")]
    float _bonusPoisonResistence;
    [SerializeField, Tooltip("retira % do dano")]
    float _bonusPetrifyResistence;
    [SerializeField, Tooltip("retira % do dano")]
    float _bonusBleedResistence;

    [Space, Tooltip("Ajuste Cooldown Skill3")]
    int[] _bonusCooldownSkill = {0, 0, 0 };

    public int BonusSkill(int index)
    {
            GetBonus(getBonus.skill3);

            return _bonusCooldownSkill[index];
    }

    public float BonusCritical
    {
        get
        {
            _critical = 0;

            GetBonus(getBonus.critical);

            return _critical;
        }
    }

    public enum getBonus { damage, health, walk, resistenceFire, resistencePoison, resistencePetrify, resistenceBleed, skill3, critical };
    public void GetBonus(getBonus type)
    {
        //Debug.LogError("GETBonus(" + type + ") for player[" + (PlayerID-1) + "] - " + HeroName(PlayerID));
        int   bonusDamage   = 0,//Bonus no Dano        
              bonusWalk     = 0,//Almento do walk
              bonusSkill3   = 0;//Redução na espera da skill3

        float bonusHealth   = 0,//Bonus de vida
              bonusCritical = 0,//Bonus no critico
              bonusRF       = 0,//Resistencia Fire
              bonusRPo      = 0,//Resistencia Poison
              bonusRPe      = 0,//Resistencia Petrificar
              bonusRB       = 0;//Resistencia Bleed
        switch ((int)type)
        {
            #region Damage
            case 0:              
                for (int i = 0; i < FaseCount; i++)
                {
                    if (Mob[PlayerID - 1]._Fases[_dlf].Fase[i]._complete && _portalBonusDamage <= 0)
                        bonusDamage += _killBonusDamage;
                    else
                    if (Mob[PlayerID - 1]._Fases[_dlf].Fase[i]._completeKill)
                        bonusDamage += _killBonusDamage;
                    else 
                    if (Mob[PlayerID - 1]._Fases[_dlf].Fase[i]._completePortal == 1)//true)
                        bonusDamage += _portalBonusDamage;
                }

                Debug.LogWarning("Bonus Damage:" + bonusDamage);
                _damage += bonusDamage;
                break;
            #endregion

            #region Health
            case 1:              
                for (int i = 0; i < FaseCount; i++)
                {
                    if (Mob[PlayerID - 1]._Fases[_dlf].Fase[i]._complete && _portalBonusHealth <= 0)
                        bonusHealth += _killBonusHealth;
                    else
                     if (Mob[PlayerID - 1]._Fases[_dlf].Fase[i]._completeKill)
                         bonusHealth += _killBonusHealth;
                     else
                     if (Mob[PlayerID - 1]._Fases[_dlf].Fase[i]._completePortal == 1)//true)
                         bonusHealth += _portalBonusHealth;
                }
                Debug.LogWarning("Bonus Health:" + bonusHealth);
                _health += bonusHealth;
                break;
            #endregion

            #region Walk
            case 2:
                int limiteWalk = 2;

                for (int i = 0; i < _fasesGetBonus.Length; i++)
                {
                    if (Mob[PlayerID-1]._Fases[_dlf].Fase[_fasesGetBonus[i]]._complete)//total 4 walk por turno
                        bonusWalk++;
                }

                if (bonusWalk > limiteWalk)
                    bonusWalk = limiteWalk;

                Debug.LogWarning("Bonus Walk:" + bonusWalk);

                _walk += bonusWalk;
                break;

            #endregion

            #region Resistence
            #region Fire
            case 3:                
                for (int i = 0; i < _fasesGetBonus.Length; i++)
                    if (Mob[PlayerID-1]._Fases[_dlf].Fase[_fasesGetBonus[i]]._complete)
                        bonusRF += _bonusFireResistence;

                Debug.LogWarning("Bonus Resistence Fogo:" + bonusRF + " %");
                _fireResistence += bonusRF;
                break;
            #endregion

            #region Poison
            case 4:               
                for (int i = 0; i < _fasesGetBonus.Length; i++)
                    if (Mob[PlayerID-1]._Fases[_dlf].Fase[_fasesGetBonus[i]]._complete)
                        bonusRPo += _bonusPoisonResistence;

                Debug.LogWarning("Bonus Resistence Veneno:" + bonusRPo + "%");
                _poisonResistence += bonusRPo;
                break;
            #endregion

            #region Petrify
            case 5:               
                for (int i = 0; i < _fasesGetBonus.Length; i++)
                    if (Mob[PlayerID-1]._Fases[_dlf].Fase[_fasesGetBonus[i]]._complete)
                        bonusRPe += _bonusPetrifyResistence;

                Debug.LogWarning("Bonus Resistence Petrificar:" + bonusRPe + "%");
                _petrifyResistence += bonusRPe;
                break;
            #endregion

            #region Bleed
            case 6:
                for (int i = 0; i < _fasesGetBonus.Length; i++)
                    if (Mob[PlayerID-1]._Fases[_dlf].Fase[_fasesGetBonus[i]]._complete)
                        bonusRB += _bonusBleedResistence;

                Debug.LogWarning("Bonus Resistence Sangrar:" + bonusRB + "%");
                _bleedResistence += bonusRB;
                break;
            #endregion
            #endregion

            #region Cooldown
            case 7:
                int limite = 3;

                for (int i = 0; i < _fasesGetBonus.Length; i++)
                {
                    if (Mob[PlayerID-1]._Fases[_dlf].Fase[_fasesGetBonus[i]]._complete)
                        bonusSkill3++;
                }

                if (bonusSkill3 > limite)
                    bonusSkill3 = limite;

                Debug.LogWarning("Bonus Skill3: " + bonusSkill3);

                _bonusCooldownSkill[2] = bonusSkill3;
                break;
            #endregion

            #region Critical
            case 8:
                float limiteCrit = 25;

                for (int i = 0; i < _fasesGetBonus.Length; i++)
                    if (Mob[PlayerID-1]._Fases[_dlf].Fase[_fasesGetBonus[i]]._complete)
                        bonusCritical += _bonusCritical;

                if (bonusCritical > limiteCrit)
                    bonusCritical = limiteCrit;

                Debug.LogWarning("Bonus Critico: " + bonusCritical + "%");

                _critical = bonusCritical;
                break;
                #endregion
        }
    }
    #endregion

    #region Extras Player
    [Space, Header("Extras Player")]
    [SerializeField]
    int _totalWalkers;
    [SerializeField]
    int _totalTurnos;//Esta no end turn
    [Space]
    [SerializeField]
    int _totalDanoRecebido;
    [SerializeField]
    int _totalDanoCausado;
    [SerializeField]
    int _totalDanoDefendido;
    [SerializeField]
    int _totalVidaRecuperada;
    [Space]
    [SerializeField]
    int _totalTimeHour;
    [SerializeField]
    int _totalTimeMinutes;
    [SerializeField]
    int _totalTimeSeg;
    [SerializeField] int _totalTimeHourAllPlayers;
    [SerializeField] int _totalTimeMinutesAllPlayers;
    [SerializeField] int _totalTimeSegAllPlayers;
    [Space]
    [SerializeField]
    int _totalGameOver;
    [SerializeField]
    int _totalMorteMob;
    [Space]
    [SerializeField]
    int _totalQuadraKill;
    [SerializeField]
    int _totalPentaKill;
    [Space]//Recebeu/Levou Dbuff
    [SerializeField]
    int _totalGetBurn;
    [SerializeField]
    int _totalGetPoison;
    [SerializeField]
    int _totalGetPetrify;
    [SerializeField]
    int _totalGetStun;
    [SerializeField]
    int _totalGetBleed;
    [Space]//Deu Dbuff
    [SerializeField]
    int _totalSetBurn;
    [SerializeField]
    int _totalSetPoison;
    [SerializeField]
    int _totalSetPetrify;
    [SerializeField]
    int _totalSetStun;
    [SerializeField]
    int _totalSetBleed;

    public void ChangeSalveExtraPlayer()
    {
        StartCoroutine(ChangeSalveExtraPlayerCoroutine());
    }
    IEnumerator ChangeSalveExtraPlayerCoroutine()
    {
        LoadingBar("Loading Extras [" + HeroName(PlayerID-1) + "]", 0,false);

        Debug.LogWarning("[" + HeroName(PlayerID-1) + "]");

        string IdPlayer = "";

        if (PlayerID != 1)
            IdPlayer = PlayerID.ToString("F0");

        _playerSkinId = PlayerPrefs.GetInt((PlayerID - 1) + "Skin");

        yield return new WaitForSeconds(0.25f);

        LoadingBar("[" + HeroName(PlayerID-1) + "]", 50,false);

        #region Extra Player
        _totalWalkers       = PlayerPrefs.GetInt("TotalWalkers"+ IdPlayer);
        _totalTurnos        = PlayerPrefs.GetInt("TotalTurnos" + IdPlayer);

        _totalDanoRecebido  = PlayerPrefs.GetInt("TotalDanoRecebido" + IdPlayer);
        _totalDanoCausado   = PlayerPrefs.GetInt("TotalDanoCausado" + IdPlayer);
        _totalDanoDefendido = PlayerPrefs.GetInt("TotalDanoDefendido" + IdPlayer);

        _totalVidaRecuperada = PlayerPrefs.GetInt("TotalVidaRecuperada" + IdPlayer);

        _totalTimeHour       = PlayerPrefs.GetInt("TotalTimeHour" + IdPlayer);
        _totalTimeMinutes    = PlayerPrefs.GetInt("TotalTimeMinutes" + IdPlayer);
        _totalTimeSeg        = PlayerPrefs.GetInt("TotalTimeSeg" + IdPlayer);

        _totalGameOver       = PlayerPrefs.GetInt("TotalGameOver" + IdPlayer);
        _totalMorteMob       = PlayerPrefs.GetInt("TotalMorteMob" + IdPlayer);

        _totalQuadraKill     = PlayerPrefs.GetInt("TotalQuadraKill" + IdPlayer);
        _totalPentaKill      = PlayerPrefs.GetInt("TotalPentaKill" + IdPlayer);

        _totalGetBurn        = PlayerPrefs.GetInt("TotalGetBurn" + IdPlayer);
        _totalGetPoison      = PlayerPrefs.GetInt("TotalGetPoison" + IdPlayer);
        _totalGetPetrify     = PlayerPrefs.GetInt("TotalGetPetrify" + IdPlayer);
        _totalGetStun        = PlayerPrefs.GetInt("TotalGetStun" + IdPlayer);
        _totalGetBleed       = PlayerPrefs.GetInt("TotalGetBleed" + IdPlayer); 

        _totalSetBurn        = PlayerPrefs.GetInt("TotalSetBurn" + IdPlayer);
        _totalSetPoison      = PlayerPrefs.GetInt("TotalSetPoison" + IdPlayer);
        _totalSetPetrify     = PlayerPrefs.GetInt("TotalSetPetrify" + IdPlayer);
        _totalSetStun        = PlayerPrefs.GetInt("TotalSetStun" + IdPlayer);
        _totalSetBleed       = PlayerPrefs.GetInt("TotalSetBleed" + IdPlayer);
        #endregion

        yield return /*new WaitForSeconds(0.5f)*/null;
        LoadingBar("[" + HeroName(PlayerID-1) + "]", 100,false);
        CloseLoadingBar(true);
    }

    public int TotalWalkers
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalWalkers"+p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            CheckAchievement(8, value);

            int total = (PlayerPrefs.GetInt("TotalWalkers"+p) + value);
         
            PlayerPrefs.SetInt("TotalWalkers"+p, total);
        }
    }

    public int TotalTurnos
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }
            return PlayerPrefs.GetInt("TotalTurnos"+p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            int total = (PlayerPrefs.GetInt("TotalTurnos"+p) + value);

            PlayerPrefs.SetInt("TotalTurnos"+p, total);
        }
    }

    public int TotalVidaRecuperada
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalVidaRecuperada"+p);
        }
        set
        {
            if (value > 0)
            {
                string p = "";

                if (PlayerID != 1)
                {
                    p = PlayerID.ToString("F0");
                }

                CheckAchievement(6, value);

                int total = (PlayerPrefs.GetInt("TotalVidaRecuperada"+p) + value);

                PlayerPrefs.SetInt("TotalVidaRecuperada"+p, total);
            }
        }
    }

    public int TotalGameOver
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }
            return PlayerPrefs.GetInt("TotalGameOver"+p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }
            int total = (PlayerPrefs.GetInt("TotalGameOver"+p) + value);

            PlayerPrefs.SetInt("TotalGameOver"+p, total);
        }
    }

    public int TotalMorteMob
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalMorteMob"+p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            CheckAchievement(14, value);
            CheckAchievement(15, value);

            int total = (PlayerPrefs.GetInt("TotalMorteMob"+p) + value);

            PlayerPrefs.SetInt("TotalMorteMob"+p, total);
        }
    }

    public int TotalQuadraKill
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalQuadraKill" + p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }


            int total = (PlayerPrefs.GetInt("TotalQuadraKill" + p) + value);

            PlayerPrefs.SetInt("TotalQuadraKill" + p, total);
        }
    }

    public int TotalPentaKill
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalPentaKill" + p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }


            int total = (PlayerPrefs.GetInt("TotalPentaKill" + p) + value);

            PlayerPrefs.SetInt("TotalPentaKill" + p, total);
        }
    }

    #region Dano
    public int TotalDanoRecebido
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalDanoRecebido"+p);
        }
        set
        {
            if (value > 0)
            {
                string p = "";

                if (PlayerID != 1)
                {
                    p = PlayerID.ToString("F0");
                }

                CheckAchievement(9, value);

                int total = (PlayerPrefs.GetInt("TotalDanoRecebido"+p) + value);

                PlayerPrefs.SetInt("TotalDanoRecebido"+p, total);
            }
        }
    }

    public int TotalDanoCausado
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalDanoCausado"+p);
        }
        set
        {
            if (value > 0)
            {
                string p = "";

                if (PlayerID != 1)
                {
                    p = PlayerID.ToString("F0");
                }

                CheckAchievement(12, value, false);

                int total = (PlayerPrefs.GetInt("TotalDanoCausado"+p) + value);

                PlayerPrefs.SetInt("TotalDanoCausado"+p, total);
            }
        }
    }

    public int TotalDanoDefendido
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalDanoDefendido"+p);
        }
        set
        {
            if (value > 0)
            {
                string p = "";

                if (PlayerID != 1)
                {
                    p = PlayerID.ToString("F0");
                }

                int total = (PlayerPrefs.GetInt("TotalDanoDefendido"+p) + value);

                PlayerPrefs.SetInt("TotalDanoDefendido"+p, total);
            }
        }
    }
    #endregion

    #region Time
    public int TotalTimeHour
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalTimeHour"+p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            int total = PlayerPrefs.GetInt("TotalTimeHour"+p) + value;

            PlayerPrefs.SetInt("TotalTimeHour"+p, total);

            ArrumaTime();
        }
    }

    public int TotalTimeHourMob(int mobID)
    {
        string p = "";

        if (mobID != 1)
        {
            p = mobID.ToString("F0");
        }

        return PlayerPrefs.GetInt("TotalTimeHour" + p);
    }

    public int TotalTimeMinutes
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalTimeMinutes"+p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            CheckAchievement(10, value);

            CheckAchievement(11, value);

            int total = PlayerPrefs.GetInt("TotalTimeMinutes"+p) + value;

            PlayerPrefs.SetInt("TotalTimeMinutes"+p, total);

            ArrumaTime();
        }
    }

    public int TotalTimeMinutesMob(int mobID)
    {
        string p = "";

        if (mobID != 1)
        {
            p = mobID.ToString("F0");
        }

        return PlayerPrefs.GetInt("TotalTimeMinutes" + p);
    }

    public int TotalTimeSeg
    {
        get
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            return PlayerPrefs.GetInt("TotalTimeSeg"+p);
        }
        set
        {
            string p = "";

            if (PlayerID != 1)
            {
                p = PlayerID.ToString("F0");
            }

            int total = PlayerPrefs.GetInt("TotalTimeSeg"+p) + value;

            PlayerPrefs.SetInt("TotalTimeSeg"+p, total);

            ArrumaTime();
        }
    }

    public int TotalTimeSegMob(int mobID)
    {
        string p = "";

        if (mobID != 1)
        {
            p = mobID.ToString("F0");
        }

        return PlayerPrefs.GetInt("TotalTimeSeg" + p);
    }

    void ArrumaTime()
    {
        _totalTimeSeg     = TotalTimeSeg;
        _totalTimeMinutes = TotalTimeMinutes;
        _totalTimeHour    = TotalTimeHour;


        if (_totalTimeSeg >= 60)
        {
            _totalTimeSeg = 0;
            _totalTimeMinutes++;

            CheckAchievement(10, 1);

            CheckAchievement(11, 1);
        }

        if (_totalTimeMinutes >= 60)
        {
            _totalTimeHour++;

            _totalTimeMinutes = 0;
        }

        if (_totalTimeSeg >= 60 || _totalTimeMinutes >= 60)
        {
            ArrumaTime();
            return;
        }

        string p = "";

        if (PlayerID != 1)
        {
            p = PlayerID.ToString("F0");
        }

        PlayerPrefs.SetInt("TotalTimeHour"+p, _totalTimeHour);
        PlayerPrefs.SetInt("TotalTimeMinutes"+p, _totalTimeMinutes);
        PlayerPrefs.SetInt("TotalTimeSeg"+p, _totalTimeSeg);

        CalculeTotalTimeAllPlayer();
    }

   public void CalculeTotalTimeAllPlayer()
    {
        int seg=0, min=0, hour=0;

        string p = "";

        for (int i = 1; i < PlayerCount; i++)
        {
            if (i != 1)
            {
                p = i.ToString("F0");
            }

            seg  += PlayerPrefs.GetInt("TotalTimeSeg" + p);
            min  += PlayerPrefs.GetInt("TotalTimeMinutes" + p);
            hour += PlayerPrefs.GetInt("TotalTimeHour" + p);

            if (seg >= 60)
            {
                seg = 0;
                min++;
            }

            if (min >= 60)
            {
                hour++;
                min = 0;
            }
        }

        TotalTimeSegAllPlayer     = seg;
        TotalTimeMinutesAllPlayer = min;
        TotalTimeHourAllPlayer    = hour;
    }

    public int TotalTimeHourAllPlayer
    {
        get
        {
            return _totalTimeHourAllPlayers;
        }
        set
        {
            _totalTimeHourAllPlayers = value;
        }
    }

    public int TotalTimeMinutesAllPlayer
    {
        get
        {
            return _totalTimeMinutesAllPlayers;
        }
        set
        {
            _totalTimeMinutesAllPlayers = value;
        }
    }

    public int TotalTimeSegAllPlayer
    {
        get
        {
            return _totalTimeSegAllPlayers;
        }
        set
        {
            _totalTimeSegAllPlayers = value;
        }
    }
    #endregion

    #region Dbuff
    /// <summary>
    /// Total De Dbuff Fire/Burn
    /// </summary>
    /// <param name="Get">Recebeu/Levou o Dbuff</param>
    /// <param name="value">Valor</param>
    /// <returns>Valor total</returns>
    public int TotalBurn(bool Get = true, int value = 1)
    {
        string p = "";

        if (PlayerID != 1)
        {
            p = PlayerID.ToString("F0");
        }

        if (value > 0)
        {
            if (Get)
            {
                CheckAchievement(7, value, true);

                value += PlayerPrefs.GetInt("TotalGetBurn"+p);

                PlayerPrefs.SetInt("TotalGetBurn"+p, value);              
            }
            else
            {
                value += PlayerPrefs.GetInt("TotalSetBurn"+p);

                PlayerPrefs.SetInt("TotalSetBurn"+p, value);
            }
        }
        else
        {
            if (Get)
                value = PlayerPrefs.GetInt("TotalGetBurn"+p);
            else
                value = PlayerPrefs.GetInt("TotalSetBurn"+p);
        }

        return value;
    }

    /// <summary>
    /// Total De Dbuff Veneno/Poison
    /// </summary>
    /// <param name="Get">Recebeu/Levou o Dbuff</param>
    /// <param name="value">Valor</param>
    /// <returns>Valor total</returns>
    public int TotalPoison(bool Get = true, int value = 1)
    {
        string p = "";

        if (PlayerID != 1)
        {
            p = PlayerID.ToString("F0");
        }

        if (value > 0)
        {
            if (Get)
            {
                value += PlayerPrefs.GetInt("TotalGetPoison"+p);

                PlayerPrefs.SetInt("TotalGetPoison"+p, value);
            }
            else
            {
                value += PlayerPrefs.GetInt("TotalSetPoison"+p);

                PlayerPrefs.SetInt("TotalSetPoison"+p, value);
            }
        }
        else
        {
            if (Get)
                value = PlayerPrefs.GetInt("TotalGetPoison"+p);
            else
                value = PlayerPrefs.GetInt("TotalSetPoison"+p);
        }

        return value;
    }

    /// <summary>
    /// Total De Dbuff Petrificar/Petrify
    /// </summary>
    /// <param name="Get">Recebeu/Levou o Dbuff</param>
    /// <param name="value">Valor</param>
    /// <returns>Valor total</returns>
    public int TotalPetrify(bool Get = true, int value = 1)
    {
        string p = "";

        if (PlayerID != 1)
        {
            p = PlayerID.ToString("F0");
        }

        if (value > 0)
        {
            if (Get)
            {
                CheckAchievement(13, value, true);

                value += PlayerPrefs.GetInt("TotalGetPetrify"+p);

                PlayerPrefs.SetInt("TotalGetPetrify"+p, value);             
            }
            else
            {
                value += PlayerPrefs.GetInt("TotalSetPetrify"+p);

                PlayerPrefs.SetInt("TotalSetPetrify"+p, value);
            }
        }
        else
        {
            if (Get)
                value = PlayerPrefs.GetInt("TotalGetPetrify"+p);
            else
                value = PlayerPrefs.GetInt("TotalSetPetrify"+p);
        }

        return value;
    }

    /// <summary>
    /// Total De Dbuff Stun
    /// </summary>
    /// <param name="Get">Recebeu/Levou o Dbuff</param>
    /// <param name="value">Valor</param>
    /// <returns>Valor total</returns>
    public int TotalStun(bool Get = true, int value = 1)
    {
        string p = "";

        if (PlayerID != 1)
        {
            p = PlayerID.ToString("F0");
        }

        if (value > 0)
        {
            if (Get)
            {
                value += PlayerPrefs.GetInt("TotalGetStun"+p);

                PlayerPrefs.SetInt("TotalGetStun"+p, value);
            }
            else
            {
                value += PlayerPrefs.GetInt("TotalSetStun"+p);

                PlayerPrefs.SetInt("TotalSetStun"+p, value);
            }
        }
        else
        {
            if (Get)
                value = PlayerPrefs.GetInt("TotalGetStun"+p);
            else
                value = PlayerPrefs.GetInt("TotalSetStun"+p);
        }

        return value;
    }

    /// <summary>
    /// Total De Dbuff Sangrar/Bleed
    /// </summary>
    /// <param name="Get">Recebeu/Levou o Dbuff</param>
    /// <param name="value">Valor</param>
    /// <returns>Valor total</returns>
    public int TotalBleed(bool Get = true, int value = 1)
    {
        string p = "";

        if (PlayerID != 1)
        {
            p = PlayerID.ToString("F0");
        }

        if (value > 0)
        {
            if (Get)
            {
                value += PlayerPrefs.GetInt("TotalGetBleed"+p);

                PlayerPrefs.SetInt("TotalGetBleed"+p, value);
            }
            else
            {
                value += PlayerPrefs.GetInt("TotalSetBleed"+p);

                PlayerPrefs.SetInt("TotalSetBleed"+p, value);
            }
        }
        else
        {
            if (Get)
                value = PlayerPrefs.GetInt("TotalGetBleed"+p);
            else
                value = PlayerPrefs.GetInt("TotalSetBleed"+p);
        }

        return value;
    }
    #endregion
    #endregion

    #region Achievement's
    [Header("Achievement")]
    public GameObject painelAchievement;

    public Text        nameAchievement;
    public Text        descricaoAchievement;
    public Image       iconAchievement;
    public AudioSource audioAchievement;
    public Sprite      secretAchievement;

   [SerializeField] List<int> completeAchievement = new List<int>();

    static WaitForSeconds waitAchievementSee   = new WaitForSeconds(3);//demora 3s pra Aparece
    static WaitForSeconds waitAchievementClose = new WaitForSeconds(7);//Fica na tela por 7s

    [Space]
    [Tooltip("All Achievement"),SerializeField]
    List<Achievement> achievement = new List<Achievement>();

    public List<Achievement> _achievement
    {
        get { return achievement; }
    }

    /// <summary>
    /// Ganha feitos da conquista
    /// </summary>
    /// <param name="index">id da conquista</param>
    /// <param name="value">valor do feito</param>
    public bool CheckAchievement(int index, float value, bool soma = true)
    {
        if (index >= achievement.Count || index < 0 || BattleMode)
            return false;

        Debug.Log("Ganhou " + value + " feito na conquista " + achievement[index]._name);

        if (soma)
            achievement[index]._feito += value;
        else
        {
            if (value > achievement[index]._feito)
                achievement[index]._feito = value;
        }

        PlayerPrefs.SetFloat(("AchievementFeito" + index).ToString(), achievement[index]._feito);

        print("Achievement[" + index + "]: F-> " + PlayerPrefs.GetFloat(("AchievementFeito" + index).ToString()));

        if (achievement[index]._feito >= achievement[index]._max)
        {
            CompleteAchievement(index);
        }

        return true;
    }

    public bool CompleteAchievement(int index, bool painel = true)
    {
        print("CompleteAchievement(" + index + ")");

        if (index > achievement.Count || achievement[index]._complete || index < 0)
            return false;

        if (achievement[index]._descricao.Contains("X%"))
            BuildAchievement();

        PlayerPrefs.SetInt(("Achievement" + index).ToString(), 1);

        achievement[index]._bonus.Invoke();

        achievement[index]._complete = true;

        completeAchievement.Add(index);

        //if (!AchiementComplete(0))
        {
            #region Platina
            if (achievement[index]._dlc == Achievement._DLC.O_resgate_da_honra)
            {
                int total = 0;
                for (int i = 1; i < achievement.Count; i++)
                {
                    if (achievement[i]._complete && achievement[i]._dlc == Achievement._DLC.O_resgate_da_honra)
                        total++;
                }

                Debug.LogWarning("Faltam :" + (achievement.Count - total) + " para a Platina.");

                CheckAchievement(0, total, false);
            }
            #endregion
        }

        if (painelAchievement.GetComponent<CanvasGroup>().alpha == 0 && painel)
            StartCoroutine(ActiveAchievementPainel());

        return true;
    }

    public void CompleteAchievement(int index)
    {
        print("CompleteAchievement(" + index + ")");

        if (   index >= achievement.Count 
            || index < 0 
            || achievement[index]._complete )
            return;

        if (achievement[index]._descricao.Contains("X%"))
            BuildAchievement();

        PlayerPrefs.SetInt(("Achievement" + index).ToString(), 1);

        achievement[index]._bonus.Invoke();

        achievement[index]._complete = true;

        completeAchievement.Add(index);

        #region Platina
        if (achievement[index]._dlc == Achievement._DLC.O_resgate_da_honra)
        {
            int total = 0;
            for (int i = 1; i < achievement.Count; i++)
            {
                if (achievement[i]._complete && achievement[i]._dlc == Achievement._DLC.O_resgate_da_honra)
                    total++;
            }

            Debug.LogWarning("Faltam :" + (achievement.Count - total) + " para a Platina.");

            CheckAchievement(0, total, false);
        }
        #endregion


        if (painelAchievement.GetComponent<CanvasGroup>().alpha == 0)
            StartCoroutine(ActiveAchievementPainel());

        // return true;
    }

    public void TesteCompleteAchievement(int index)
    {
        CheckAchievement(index, 9999999,true);
    }

    public IEnumerator ActiveAchievementPainel()
    {
        yield return waitAchievementSee;

        if (painelAchievement.GetComponent<CanvasGroup>().alpha == 0)
        {
            painelAchievement.GetComponent<Animator>().SetBool("Close", false);

            Vibrate();

            string color   = "",
                   _desc   = achievement[completeAchievement[0]]._descricao;
            Sprite _sprite = achievement[completeAchievement[0]]._icon;           


            #region Color Type
            switch (achievement[completeAchievement[0]]._type)
            {
                case Achievement._Type.Bronze:
                    color = "<color=#CD7F32>";
                    break;

                case Achievement._Type.Prata:
                    color = "<color=#838383FF>";
                    break;

                case Achievement._Type.Ouro:
                    color = "<color=#C3A607FF>";
                    break;

                case Achievement._Type.Platina:
                    color = "<color=#2C4067FF>";
                    break;
            }
            #endregion

            nameAchievement.text = color + achievement[completeAchievement[0]]._name + "</color>";

            descricaoAchievement.text = _desc;

            if (_sprite != null)
            {
                iconAchievement.sprite         = _sprite;
                iconAchievement.type           = Image.Type.Simple;
                iconAchievement.preserveAspect = true;
            }

            painelAchievement.SetActive(true);

            if (achievement[completeAchievement[0]]._audio != null)
            {
                audioAchievement.clip = achievement[completeAchievement[0]]._audio;
                audioAchievement.Play();
            }

            painelAchievement.GetComponent<Animator>().SetTrigger("Show");

            completeAchievement.Remove(completeAchievement[0]);

            StartCoroutine(CloseAchievementPainel());
        }
    }

    IEnumerator CloseAchievementPainel()
    {
        yield return waitAchievementClose;

        painelAchievement.GetComponent<Animator>().SetBool("Close", true);

        /*
        painelAchievement.SetActive(false);

        nameAchievement.text = "";

        descricaoAchievement.text = "";

        iconAchievement.sprite = null;
        */

        if (completeAchievement.Count >= 1)
            StartCoroutine(ActiveAchievementPainel());
    }

    public float AnchievementProgress(int index)
    {
        if (index >= achievement.Count)
            return -1;

        float p = achievement[index]._feito / achievement[index]._max;

        if (p>1)
            p = 1;

        return p;
    }

    public void AchievementFase()
    {
        int fase = _atualFase;

        Debug.LogError("AchievementFase("+ Mob[PlayerID - 1]._Fases[_dlf].Fase[fase]._fase+ ") -> "+ Mob[PlayerID - 1]._nameHero);

        if (Mob[PlayerID-1]._Fases[_dlf].Fase[fase]._complete ||
            !Mob[PlayerID - 1]._Fases[_dlf].Fase[fase]._complete)
        {
            switch (fase)
            {
                case 0: //Preparado?!               
                    CheckAchievement(1, 1);
                    break;

                case 5://Desafios
                    CheckAchievement(2, 1);
                    break;

                case 8://Novos Desafios
                    CheckAchievement(3, 1);
                    break;

                case 11://Hamburguer no <b>ZOY</b>
                    CheckAchievement(4, 1);
                    break;

                case 12://Viking Honor
                    CheckAchievement(5, 1);
                    break;
            }
        }
    }

    public void BuildAchievement()
    {
        int needPlatinaDlc0 = 0;

        for (int i = 0; i < achievement.Count; i++)
        {
            if (achievement[i]._dlc == Achievement._DLC.O_resgate_da_honra)
                needPlatinaDlc0++;
        }

        achievement[0]._max = needPlatinaDlc0 - 1;

        for (int i = 0; i < achievement.Count; i++)
        {
            string getN = GetComponent<XmlAchievement>().GetNameAchievement(i);
            string getD = GetComponent<XmlAchievement>().GetDescricaoAchievement(i);

            if (getN != null)
                achievement[i]._name = getN;
            else
                Debug.LogError("Get XMLAchievement Name ("+i+")");

            if (getD != null)
                achievement[i]._descricao = getD;
            else
                Debug.LogError("Get XMLAchievement Desc (" + i + ")");

            if (achievement[i]._descricao.Contains("X%"))
            {
                string old = achievement[i]._descricao;
                string max = "<b>" + achievement[i]._max.ToString() + "</b> ";

                achievement[i]._descricao = old.Replace("X%", max);
            }

            if (achievement[i]._bonus.GetPersistentEventCount()>0)
            {
                achievement[i]._descricao += "\n<color=green>"+(/*!IsMobile ?*/ XmlMenuInicial.Instance.Get(54) /*: "Desbloquea Bonus!!!"*/)+"</color>";
            }
        }
    }

    void CompAchieNewVersion(int index)
    {
        //return;
  
        Debug.LogError("Conquista <b>" + achievement[index]._name + "</b> Esta Sendo Atualizada Para a Nova Versão Buscando Dados...");
       
        int value = -99999;

        switch (index)
        {
            #region DLC, Missao
           
            #endregion

            #region DLC,
            #region Platina
            case 0:
                if (!achievement[index]._complete)
                {
                    value = 0;

                    for (int i = 0; i < achievement.Count; i++)
                    {
                        if (achievement[i]._feito == achievement[i]._max && achievement[i]._complete && achievement[i]._dlc == Achievement._DLC.O_resgate_da_honra)
                        {
                            value++;

                            Debug.LogError("Conquista Completas: " + (achievement[i]._name) + ".");
                        }
                    }

                    achievement[index]._feito = value;

                    Debug.LogWarning("Faltam :" + (achievement[0]._max - achievement[index]._feito) + " para a Platina.");              
                }
                break;
            #endregion

            #region Preparado?!
            case 1:
                if (!achievement[index]._complete)
                {
                    if (Mob[0]._Fases[_dlf].Fase[0]._complete)
                    {
                        value = 1;

                        achievement[index]._feito = value;
                    }
                }
                break;
            #endregion

            #region O Inicio do Chaos
            case 2:
                if (!achievement[index]._complete)
                {
                    if (Mob[0]._Fases[_dlf].Fase[5]._complete)
                    {
                        value = 1;

                        achievement[index]._feito = value;
                    }
                }
                break;
            #endregion

            #region O Novo Inicio do Chaos
            case 3:
                if (!achievement[index]._complete)
                {
                    if (Mob[0]._Fases[_dlf].Fase[8]._complete)
                    {
                        value = 1;

                        achievement[index]._feito = value;
                    }
                }
                break;
            #endregion

            #region Hamburguer no ZOY
            case 4:
                if (!achievement[index]._complete)
                {
                    if (Mob[0]._Fases[_dlf].Fase[11]._complete)
                    {
                        value = 1;

                        achievement[index]._feito = value;
                    }
                }
                break;
            #endregion

            #region Viking Honor
            case 5:
                if (!achievement[index]._complete)
                {
                    if (Mob[0]._Fases[_dlf].Fase[12]._complete)
                    {
                        value = 1;

                        achievement[index]._feito = value;
                    }
                }
                break;
            #endregion

            #region Imortal
            case 6:
                if (!achievement[index]._complete)
                {
                    value = _totalVidaRecuperada;

                    achievement[index]._feito = value;
                }
                break;
            #endregion

            #region Ta pegando fogo Bixoo!!!
            case 7:
                if (!achievement[index]._complete)
                {
                    value = _totalGetBurn;

                    achievement[index]._feito = value;
                }
                break;
            #endregion

            #region Andarilho!
            case 8:
                if (!achievement[index]._complete)
                {
                    value = _totalWalkers;

                    achievement[index]._feito = value;
                }
                break;
            #endregion

            #region Saco de Pancada
            case 9:
                if (!achievement[index]._complete)
                {
                    value = _totalDanoRecebido;

                    achievement[index]._feito = value;
                }
                break;
            #endregion

            #region Nem parece mas, ja jogou por 1 hora
            case 10:
                if (!achievement[index]._complete)
                {
                    value = _totalTimeMinutes;

                    achievement[index]._feito = value;
                }
                break;
            #endregion

            #region Nem parece mas, ja jogou por 3 hora
            case 11:
                if (!achievement[index]._complete)
                {
                    value = _totalTimeMinutes;

                    achievement[index]._feito = value;
                }
                break;
            #endregion

            #region O Pode do Protagonismo
            case 12:
                if (!achievement[index]._complete)
                {
                    value = (int)achievement[index]._feito;
                }
                break;
            #endregion

            #region Eeeestatuaaaa a!!!
            case 13:
                if (!achievement[index]._complete)
                {
                    value = _totalGetPetrify;

                    achievement[index]._feito = value;
                }
                break;
            #endregion

            #region Carrasco
            case 14:
                if (!achievement[index]._complete)
                {
                    value = _totalMorteMob;

                    achievement[index]._feito = value;
                }
                break;
            #endregion

            #region Like A Boss
            case 15:
                if (!achievement[index]._complete)
                {
                    value = _totalMorteMob;

                    achievement[index]._feito = value;
                }
                break;
                #endregion
           #endregion

        }

        if (value != -99999)
        {           
            NewInfo("Conquista <color=green>" + achievement[index]._name + "</color> foi alterada para <b>" + achievement[index]._max + "</b>  voce tem "+value+".", 3);

            if (value >= _achievement[index]._max)
            {
                _achievement[index]._feito = value;

                CompleteAchievement(index);
            }
        }
        else
            NewInfo("Conquista <color=green>" + achievement[index]._name + "</color> foi alterada para <b>" + achievement[index]._max + "</b>  voce tem " + achievement[index]._feito + ".", 3);
    }

    /// <summary>
    /// Check If Achiement is Complete
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool AchiementComplete(int id)
    {
        if (id > _achievement.Count || id < 0)
            return false;

        //Debug.LogWarning("ChecK: " + _achievement[id]._nameS + " - " + _achievement[id]._complete);

        return _achievement[id]._feito==_achievement[id]._max;
    }

    public bool CompleteDlcAchievement(Achievement._DLC dlc)
    {
        for (int i = 0; i < achievement.Count; i++)
            if (achievement[i]._dlc == dlc)
                if (!achievement[i]._complete)
                    return false;

        return true;
    }
    public bool CompleteDlcAchievement(int dlc)
    {
        for (int i = 0; i < achievement.Count; i++)
            if ((int)achievement[i]._dlc == dlc)
                if (!achievement[i]._complete)
                    return false;

        return true;
    }

    /// <summary>
    /// Pega o Achievement Platina Da Dlc
    /// </summary>
    /// <param name="dlc"></param>
    /// <returns></returns>
    public Achievement GetAchievementDlc(Achievement._DLC dlc)
    {
        foreach (var Platina in achievement)
        {
            if (Platina._dlc  == dlc &&
                Platina._type == Achievement._Type.Platina)
            {
                return Platina;
            }
        }

        return null;
    }

    /// <summary>
    /// Pega o Achievement Platina Da Dlc
    /// </summary>
    /// <param name="dlc"></param>
    /// <returns></returns>
    public Achievement GetAchievementDlc(int index)
    {
        if (index > 0 && index < achievement.Count)
        {
            Achievement._DLC dlc = achievement[index]._dlc;

            foreach (var Platina in achievement)
            {
                if (Platina._dlc == dlc &&
                    Platina._type == Achievement._Type.Platina)
                {
                    return Platina;
                }
            }
        }

        return null;
    }

   public int SeachIndexAchievement(Achievement _achievement)
    {
        if (_achievement==null)
        {
            return -1;
        }

        print("Seach Index :"+_achievement._name);

        for (int i = 0; i < achievement.Count; i++)
        {
            if (achievement[i] == _achievement)
            {
                print("Seach Index :" + _achievement._name+" index: "+i);
                return i;
            }
        }

        print("Seach Index :" + _achievement._name + " index: Null");
        return -1;
    }

    void ChangeAchievement(int index,int feito,bool complete)
    {
        PlayerPrefs.SetFloat(("AchievementFeito" + index).ToString(), feito);

        PlayerPrefs.SetInt(("Achievement" + index).ToString(), Convert.ToInt32(complete));
    }
    #endregion

    #region Painel Info
    [Header("Painel Info")]
    [SerializeField] GameObject _painelInf;

    [SerializeField] List<string> _painelTxt = new List<string>();

    [SerializeField] List<float> _painelTime = new List<float>();

    public void NewInfo(string txt, float time,bool onlyAdm=false)
    {
        if (onlyAdm && !adm)       
            return;      

        _painelTxt.Add  (txt);
        _painelTime.Add(time);

        if (!_painelInf.activeInHierarchy)
            StartCoroutine(PainelTimer());          
    }

    IEnumerator PainelTimer()
    {
        _painelInf.GetComponentInChildren<Text>().text = _painelTxt[0];

        _painelInf.SetActive(true);

        float time = _painelTime[0];

        yield return new WaitForSeconds(time);

        _painelInf.SetActive(false);

        if (_painelTxt!=null && _painelTxt.Count>0 && _painelTxt.Contains(_painelTxt[0]))
            _painelTxt.Remove(_painelTxt[0]);

        if (_painelTime.Count>0 &&_painelTime != null && _painelTime.Contains(_painelTime[0]))
            _painelTime.Remove(_painelTime[0]);

        yield return new WaitForSeconds(0.5f);

        if (_painelTxt.Count != 0)
            StartCoroutine(PainelTimer());
    }
    #endregion

    #region Loading Bar
    [Space]
    [Header("Loading Bar")]
    [SerializeField] GameObject _loaginBar;
    [SerializeField] Text       _loaginBarDesc;
    [SerializeField] Slider     _loaginBarSlider;
    [SerializeField] Text       _loaginBarProgress;

    public void LoadingBar(string desc,float progress,bool mult=true)
    {
        if (progress > 1)
            progress /= 100;

        string _p = (progress).ToString("F0") + "%";

        /*if (!_loaginBar.activeInHierarchy ||
            _loaginBar.activeInHierarchy && _loaginBarDesc.text == desc)*/
        {            
            if (mult)
                _p = (progress * 100).ToString("F0") + "%";

            //print(_p);

            _loaginBar.SetActive(progress < 1);

            _loaginBarDesc.text = desc;
            _loaginBarSlider.value = progress;
            _loaginBarProgress.text = _p;
        }
    }

    public void LoadingBar(string desc, float progressMin, float progressMax,bool mult=true)
    {
        float progress = progressMin / progressMax;

        string _p = (mult ? (progress * 100).ToString("F2") :  (progress).ToString("F0")) + "%";

        /*if (!_loaginBar.activeInHierarchy ||
             _loaginBar.activeInHierarchy && _loaginBarDesc.text == desc)*/
        {                      
            _loaginBar.SetActive(progress < 1);

            //print(progressMin+"/"+ progressMax+" - "+ _p + "\n"+desc);

            _loaginBarDesc.text = desc;
            _loaginBarSlider.value = progress;
            _loaginBarProgress.text = _p;
        }
    }

    public void CloseLoadingBar(bool forceClose=false)
    {
        if (forceClose)
        {
            _loaginBar.SetActive(false);
            return;
        }
        if (_loaginBar.activeInHierarchy)
        {
            if (_loaginBarSlider.maxValue == _loaginBarSlider.value)
            {
                //print("CloseLoadingBar()");
                if (_loaginBar.GetComponent<Fade>() == null)
                {
                    _loaginBar.SetActive(false);
                }
                else
                {
                    _loaginBar.GetComponent<Fade>().desativeComplete = true;
                    _loaginBar.GetComponent<Fade>().FadeOff();
                }
            }
        }
        
    }
    #endregion

    #region Question Bar
    [Space]
    [Header("Question Bar")]
    [SerializeField] GameObject _questionPainel;
    [SerializeField] Text       _questionDetal;
    [SerializeField] Button     _questionButtonTrue;  
    [SerializeField] Button     _questionButtonFalse;

    public void QuestionPainel(string _detal,string _txtTrue,string _txtFalse,UnityAction[]  _true,UnityAction[] _false)
    {
        Fade _fade = _questionPainel.GetComponentInChildren<Fade>();

        _questionPainel.SetActive(true);
        _fade.desativeComplete = false;
        _fade.FadeOn();
        _questionButtonTrue.interactable  = true;
        _questionButtonFalse.interactable = true;       

        _questionDetal.text = _detal;

        #region True
        _questionButtonTrue.GetComponentInChildren<Text>().text = _txtTrue;
        _questionButtonTrue.onClick.RemoveAllListeners();       
        _questionButtonTrue.onClick.AddListener(() => _questionPainel.GetComponent<AudioSource>().Play());
        _questionButtonTrue.onClick.AddListener(() => _fade.FadeOff());     
        if (_true != null && _true.Length > 0)
        {
            foreach (var t in _true)
            {
                _questionButtonTrue.onClick.AddListener(t);
            }

        }
        _questionButtonTrue.onClick.AddListener(() => _questionButtonTrue.interactable= false);
        _questionButtonTrue.onClick.AddListener(() => _questionButtonFalse.interactable = false);
        _questionButtonTrue.onClick.AddListener(() => _fade.desativeComplete = true);
        _questionButtonTrue.onClick.AddListener(() => _questionPainel.SetActive(false));
        #endregion

        #region False
        _questionButtonFalse.GetComponentInChildren<Text>().text = _txtFalse;
        _questionButtonFalse.onClick.RemoveAllListeners();
        _questionButtonFalse.onClick.AddListener(() => _questionPainel.GetComponent<AudioSource>().Play());
        _questionButtonFalse.onClick.AddListener(() => _fade.FadeOff());     
        if (_false!=null && _false.Length > 0)
        {
            foreach (var f in _false)
            {
                _questionButtonFalse.onClick.AddListener(f);
            }

        }
        _questionButtonFalse.onClick.AddListener(() => _questionButtonTrue.interactable  = false);
        _questionButtonFalse.onClick.AddListener(() => _questionButtonFalse.interactable = false);
        _questionButtonFalse.onClick.AddListener(() => _fade.desativeComplete = true);
        _questionButtonFalse.onClick.AddListener(() => _questionPainel.SetActive(false));
        #endregion
    }
    #endregion

    #region

    List<BattleModeBalance> _mobBalanceAttackDamage = new List<BattleModeBalance>();
    List<BattleModeBalance> _mobBalanceMAttackDamage = new List<BattleModeBalance>();
    List<BattleModeBalance> _mobBalanceHp = new List<BattleModeBalance>();
    List<BattleModeBalance> _mobBalanceMHp = new List<BattleModeBalance>();
    //List<BattleModeBalance> _mobBalanceAttackSkill1 = new List<BattleModeBalance>();
    //List<BattleModeBalance> _mobBalanceAttackSkill2 = new List<BattleModeBalance>();
    //List<BattleModeBalance> _mobBalanceAttackSkill3 = new List<BattleModeBalance>();
    //List<BattleModeBalance> _mobBalanceMAttackSkill1 = new List<BattleModeBalance>();
    //List<BattleModeBalance> _mobBalanceMAttackSkill2 = new List<BattleModeBalance>();
    //List<BattleModeBalance> _mobBalanceMAttackSkill3 = new List<BattleModeBalance>();

    protected int _calculeStatusMobMinAttackDamage = 0;
    /// <summary>
    /// Only Read
    /// </summary>
    public int CalculeStatusMobMinAttackDamage {get { return _calculeStatusMobMinAttackDamage; } }
    protected int _calculeStatusMobMaxAttackDamage = 0;
    /// <summary>
    /// Only Read
    /// </summary>
    public int CalculeStatusMobMaxAttackDamage { get { return _calculeStatusMobMaxAttackDamage; } }

    protected int _calculeStatusMobMinHp = 0;
    /// <summary>
    /// Only Read
    /// </summary>
    public int CalculeStatusMobMinHp { get { return _calculeStatusMobMinHp; } }
    protected int _calculeStatusMobMaxHp = 0;
    /// <summary>
    /// Only Read
    /// </summary>
    public int CalculeStatusMobMaxHp { get { return _calculeStatusMobMaxHp; } }

    /// <summary>
    /// Calcula os maiores e o menos status do jogo
    /// </summary>
   public void Balance(bool showConsole=false)
    {
        Debug.LogWarning("Balance");

        _mobBalanceAttackDamage.Clear();
        _mobBalanceMAttackDamage.Clear();

        _mobBalanceHp.Clear();
        _mobBalanceMHp.Clear();

        int count = Mob.Count;
        int valueMaxAttack = 0
            ,valueMaxHp = 0
            //,valueMaxSkill1 = 0
            //,valueMaxSkill2 = 0
            //,valueMaxSkill3 = 0
            ,valueMinAttack = 999990
            ,valueMinHp = 99999
            //,valueMinSkill1 = 99999
            //,valueMinSkill2 = 99999
            //valueMinSkill3 = 99999
            ;

        for (int i = 0; i < count; i++)
        {
            int countIaMob = Mob[i]._statusIaMob.Count - 1;
            int valueAttack = CalculeDamageValueBase(i, Mob[i]._classe),//_gms.Mob[mobsActiveAll[i]]._statusIaMob[countIaMob]._damageMax,
                valueHp = CalculeHealthValueBase(i, Mob[i]._classe);//(int)_gms.Mob[mobsActiveAll[i]]._statusIaMob[countIaMob]._healthMax;

            #region Skills
            //int countS = Mob[i]._prefabHero.GetComponent<SkillManager>().PrefabSkills.Count;
            //for (int s = 0; s < countS; s++)
            //{
            //    MobSkillManager Skill = _gms.Mob[i]._prefabHero.GetComponent<SkillManager>().PrefabSkills[s].GetComponent<MobSkillManager>();

            //    if (Skill.BalanceMode == Game_Mode.History)
            //    {
            //        foreach (var item in _gms.Mob[i]._prefabHero.GetComponent<SkillManager>().PrefabSkills[s].GetComponents<MobSkillManager>())
            //        {
            //            if (item.BalanceMode == Game_Mode.Battle)
            //            {
            //                Skill = item;
            //                break;
            //            }
            //        }
            //    }

            //    int damageSkill = _gms.Mob[i]._prefabHero.GetComponent<SkillManager>().CalculeTotalDamage(valueAttack, Skill.PorcentDamage, Skill.DividedDamage, Skill.BaseDamage, (int)((valueHp * Skill.MaxHpProcentBase) / 100), Skill.DividedDamage);

            //    if (damageSkill <= 0)
            //    {
            //        Debug.LogError("<color=red>" + _gms.HeroName(i) + " [" + Mob[i]._classe + "] - " + Skill.name + " Tem [" + damageSkill + "] de dano !!</color>");
            //    }
            //    else
            //    {
            //        int max = valueMaxSkill1;

            //        int min = valueMinSkill1;

            //        if (s == 1)
            //        {
            //            max = valueMaxSkill2;
            //            min = valueMinSkill2;
            //        }
            //        if (s == 2)
            //        {
            //            max = valueMaxSkill3;
            //            min = valueMinSkill3;
            //        }

            //        if (damageSkill >= max)
            //        {
            //            BattleModeBalance b = new BattleModeBalance();

            //            b._nameMob = HeroName(i) + " [" + Mob[i]._classe + "] - " + Skill.name;

            //            b._valueStatus = damageSkill;


            //            if (s == 0)
            //            {
            //                if (max < damageSkill)
            //                    _mobBalanceAttackSkill1.Clear();

            //                valueMaxSkill1 = damageSkill;
            //                _mobBalanceAttackSkill1.Add(b);

            //            }
            //            if (s == 1)
            //            {
            //                if (max < damageSkill)
            //                    _mobBalanceAttackSkill2.Clear();

            //                valueMaxSkill2 = damageSkill;
            //                _mobBalanceAttackSkill2.Add(b);
            //            }
            //            if (s == 2)
            //            {
            //                if (max < damageSkill)
            //                    _mobBalanceAttackSkill3.Clear();

            //                valueMaxSkill3 = damageSkill;
            //                _mobBalanceAttackSkill3.Add(b);
            //            }

            //            //Debug.LogWarning(b._nameMob + " Tem o maior ataque[" + valueAttack + "]!!");
            //        }
            //        // else
            //        if (damageSkill <= min)
            //        {
            //            BattleModeBalance b = new BattleModeBalance();

            //            b._nameMob = HeroName(i) + " [" + Mob[i]._classe + "] - " + Skill.name;

            //            b._valueStatus = damageSkill;

            //            if (s == 0)
            //            {
            //                if (min > damageSkill)
            //                    _mobBalanceMAttackSkill1.Clear();

            //                valueMinSkill1 = damageSkill;
            //                _mobBalanceMAttackSkill1.Add(b);

            //            }
            //            if (s == 1)
            //            {
            //                if (min > damageSkill)
            //                    _mobBalanceMAttackSkill2.Clear();

            //                valueMinSkill2 = damageSkill;
            //                _mobBalanceMAttackSkill2.Add(b);
            //            }
            //            if (s == 2)
            //            {
            //                if (min > damageSkill)
            //                    _mobBalanceMAttackSkill3.Clear();

            //                valueMinSkill3 = damageSkill;
            //                _mobBalanceMAttackSkill3.Add(b);
            //            }

            //            //Debug.LogWarning(b._nameMob + " Tem o maior ataque[" + valueAttack + "]!!");
            //        }
            //    }
            //}
            #endregion

            #region Attack           
            if (valueAttack >= valueMaxAttack)
            {
                BattleModeBalance b = new BattleModeBalance();

                b._nameMob = HeroName(i) + " - " + Mob[i]._classe;

                valueMaxAttack = valueAttack;

                b._valueStatus = valueAttack;

                _mobBalanceAttackDamage.Add(b);

                //Debug.LogWarning(b._nameMob + " Tem o maior ataque[" + valueAttack + "]!!");
            }
            // else
            if (valueAttack <= valueMinAttack)
            {
                BattleModeBalance b = new BattleModeBalance();

                b._nameMob = HeroName(i) + " - " +Mob[i]._classe;

                valueMinAttack = valueAttack;

                b._valueStatus = valueAttack;

                _mobBalanceMAttackDamage.Add(b);

                //Debug.LogWarning(b._nameMob + " Tem o maior ataque[" + valueAttack + "]!!");
            }
            #endregion

            #region Hp
            if (valueHp >= valueMaxHp)
            {
                BattleModeBalance b = new BattleModeBalance();

                b._nameMob = HeroName(i) + " - " + Mob[i]._classe;

                valueMaxHp = valueHp;

                b._valueStatus = valueHp;

                _mobBalanceHp.Add(b);

                //Debug.LogWarning(b._nameMob + " Tem o maior Hp[" + valueHp + "]!!");
            }
            //   else
            if (valueHp <= valueMinHp)
            {
                BattleModeBalance b = new BattleModeBalance();

                b._nameMob = HeroName(i) + " - " + Mob[i]._classe;

                valueMinHp = valueHp;

                b._valueStatus = valueHp;

                _mobBalanceMHp.Add(b);

                //Debug.LogWarning(b._nameMob + " Tem o maior Hp[" + valueHp + "]!!");
            }
            #endregion
        }

        RecallBalance(true, valueMaxAttack, true);
        RecallBalance(true, valueMinAttack, false);

        RecallBalance(false, valueMaxHp, true);
        RecallBalance(false, valueMinHp, false);

        _calculeStatusMobMaxAttackDamage = _mobBalanceAttackDamage[0]._valueStatus;
        _calculeStatusMobMinAttackDamage = _mobBalanceMAttackDamage[0]._valueStatus;

        _calculeStatusMobMaxHp = _mobBalanceHp[0]._valueStatus;
        _calculeStatusMobMinHp = _mobBalanceMHp[0]._valueStatus;

        Debug.LogWarning("ATK: "+ _calculeStatusMobMinAttackDamage + " - "+ _calculeStatusMobMaxAttackDamage);
        Debug.LogWarning("Hp: " + _calculeStatusMobMinHp + " - " + _calculeStatusMobMaxHp);


        if (!showConsole)
            return;

        foreach (var item in _mobBalanceMAttackDamage)
            Debug.LogError("<color=blue>Atk Desbalanceado: [ " + item._nameMob + " ] " + item._valueStatus + "</color>");
        foreach (var item in _mobBalanceAttackDamage)
            Debug.LogError("Atk Desbalanceado: [ " + item._nameMob + " ] " + item._valueStatus);
        Debug.LogError("_____");
        foreach (var item in _mobBalanceMHp)
            Debug.LogError("<color=blue>Hp Desbalanceado: [ " + item._nameMob + " ] " + item._valueStatus + "</color>");
        foreach (var item in _mobBalanceHp)
            Debug.LogError("Hp Desbalanceado: [ " + item._nameMob + " ] " + item._valueStatus);
        Debug.LogError("_____");
        //foreach (var item in _mobBalanceMAttackSkill1)
        //    Debug.LogError("<color=blue>Skill1 Desbalanceada: [ " + item._nameMob + " ] " + item._valueStatus + "</color>");
        //foreach (var item in _mobBalanceAttackSkill1)
        //    Debug.LogError("Skill1 Desbalanceada: [ " + item._nameMob + " ] " + item._valueStatus);
        //Debug.LogError("_____");
        //foreach (var item in _mobBalanceMAttackSkill2)
        //    Debug.LogError("<color=blue>Skill2 Desbalanceada: [ " + item._nameMob + " ] " + item._valueStatus + "</color>");
        //foreach (var item in _mobBalanceAttackSkill2)
        //    Debug.LogError("Skill2 Desbalanceada: [ " + item._nameMob + " ] " + item._valueStatus);
        //Debug.LogError("_____");
        //foreach (var item in _mobBalanceMAttackSkill3)
        //    Debug.LogError("<color=blue>Skill3 Desbalanceada: [ " + item._nameMob + " ] " + item._valueStatus + "</color>");
        //foreach (var item in _mobBalanceAttackSkill3)
        //    Debug.LogError("Skill3 Desbalanceada: [ " + item._nameMob + " ] " + item._valueStatus);
    }
    void RecallBalance(bool Attack, int value, bool Max)
    {
        int count = 0;
        if (Attack)
        {
            if (!Max)
            {
                count = _mobBalanceMAttackDamage.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_mobBalanceMAttackDamage[i]._valueStatus > value)
                    {
                        //print(_mobBalanceAttackDamage[i]._nameMob + " Não Tem o menor Ataque!!");
                        _mobBalanceMAttackDamage.Remove(_mobBalanceMAttackDamage[i]);
                        RecallBalance(Attack, value, Max);
                        break;
                    }
                }
            }
            else
            {
                count = _mobBalanceAttackDamage.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_mobBalanceAttackDamage[i]._valueStatus < value)
                    {
                        //print(_mobBalanceAttackDamage[i]._nameMob + " Não Tem o maior Ataque!!");
                        _mobBalanceAttackDamage.Remove(_mobBalanceAttackDamage[i]);
                        RecallBalance(Attack, value, Max);
                        break;
                    }
                }
            }
        }
        else
        {
            if (!Max)
            {
                count = _mobBalanceMHp.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_mobBalanceMHp[i]._valueStatus > value)
                    {
                        //print(_mobBalanceHp[i]._nameMob + " Não Tem o menor Hp!!");
                        _mobBalanceMHp.Remove(_mobBalanceMHp[i]);
                        RecallBalance(Attack, value, Max);
                        break;
                    }
                }
            }
            else
            {
                count = _mobBalanceHp.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_mobBalanceHp[i]._valueStatus < value)
                    {
                        //print(_mobBalanceHp[i]._nameMob + " Não Tem o maior Hp!!");
                        _mobBalanceHp.Remove(_mobBalanceHp[i]);
                        RecallBalance(Attack, value, Max);
                        break;
                    }
                }
            }

        }

    }
    #endregion

    public void Vibrate()
    {
        iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("y", 0.2f, "x", 0.5f, "time", 0.6f, "easetype", iTween.EaseType.easeOutBounce));

        if (!IsMobile)
            return;

#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
        Handheld.Vibrate();
#endif
    }

    private void Awake()
    {
        if (_gms == null)
            _gms = this;
        else
            Destroy(this.gameObject);

        CheckMobile();

        //NewInfo("Bem vindo "   + SystemInfo.deviceName,3);
        //NewInfo("Model: "      + SystemInfo.deviceModel, 3);
        //NewInfo("Type: "       + SystemInfo.deviceType, 3);
        //NewInfo("DeviceID: "   + SystemInfo.graphicsDeviceID, 3);
        //NewInfo("Version: "    + SystemInfo.graphicsDeviceVersion, 3);
        //NewInfo("MemorySize: " + SystemInfo.graphicsMemorySize, 3);

        if (_loaginBar!=null &&
            _loaginBar.GetComponent<Fade>())

            waitDelayBar = new WaitForSeconds(_loaginBar.GetComponent<Fade>().timeToFade);                

        DontDestroyOnLoad(transform.gameObject);

#if UNITY_EDITOR
        Demo = false;

        UnityAction[] _y =
   {
         () => adm=true,
         () => NewInfo("<color=green>Bem vindo\nSr(a) <b>UNITY_EDITOR</b>!!!</color>", 5, true),
         () => NewInfo("<color=green>Beneficios de Adm Ativos!!!</color>", 3, true),
         () => CheckSaves()
    };

        UnityAction[] _n =
    {
         () => CheckSaves()
    };

        QuestionPainel("Deseja Entrar com Adm???","yep","Nop",
            _y,
            _n);      
#else
        CheckSaves();
#endif
    }

    public void CheckSaves()
    {
        LoadingBar("Buscando Save", 0, false);

        StartCoroutine(CheckSaveCoroutine());
    }
    IEnumerator CheckSaveCoroutine()
    {
        LoadingBar("Buscando Save", 0.5f, false);

        yield return new WaitForSeconds(0.01f);

        LoadingBar("Buscando Save", 1, false);

        LoadComplete = false;

        completeAchievement.Clear();

        Debug.LogError("Check Saves \n Tem Outro Save (" + PlayerPrefs.HasKey("F0") + ")");

        int _DificuldadeInicial = Convert.ToInt32(_dificuldade);

        for (int i = 1; i < PlayerCount; i++)
            Mob[i]._blocked = true;

        for (int i = 0; i < achievement.Count; i++)
        {
            achievement[i]._complete = false;
            achievement[i]._feito = 0;
        }

        FaseAtual = (0);

        _playerId = 1;
        _playerSkinId = 0;

        //Tem salve
        if (PlayerPrefs.HasKey("PlayerId"))
        {
            NewInfo(
               "Save Encontrado"
               , 5f);

            Debug.LogWarning("Tem salve " + _achievement.Count);

            _playerId = PlayerPrefs.GetInt("PlayerId");

            _playerSkinId = PlayerPrefs.GetInt((PlayerID - 1) + "Skin");

            #region Mobs / Skins
            for (int mob = 0; mob < PlayerCount; mob++)
            {
                Mob[mob]._blocked = Convert.ToBoolean(PlayerPrefs.GetInt(mob + "Blocked"));

                #region Skins
                int SkinCount = Mob[mob]._skinHero.Count;
                for (int skin = 0; skin < SkinCount; skin++)
                    if (PlayerPrefs.HasKey(mob + "Skin" + skin))
                        Mob[mob]._skinHero[skin]._skinBlocked = Convert.ToBoolean(PlayerPrefs.GetInt(mob + "Skin" + skin));
                #endregion

                if (!Mob[mob]._blocked && Mob[mob]._skinHero[0]._skinBlocked)
                    SkinDesblock(mob, 0, true);

                yield return new WaitForSeconds(0.01f);
            }
            #endregion

            #region Prop's
            _damage = PlayerPrefs.GetInt("Damage");
            _health = PlayerPrefs.GetFloat("Health");
            _walk = PlayerPrefs.GetInt("Walk");
            #endregion                      

            #region Resistences
            _fireResistence = PlayerPrefs.GetFloat("FireResistence");
            _poisonResistence = PlayerPrefs.GetFloat("PoisonResistence");
            _petrifyResistence = PlayerPrefs.GetFloat("PetrifyResistence");
            _bleedResistence = PlayerPrefs.GetFloat("BleedResistence");
            #endregion

            #region Config
            _moveCameraArrow = Convert.ToBoolean(PlayerPrefs.GetInt("MoveCameraArrow"));
            _mouseSensibility = PlayerPrefs.GetFloat("MouseSensibility");
            _soundEffect = PlayerPrefs.GetFloat("SoundEffect");
            _soundMusic = PlayerPrefs.GetFloat("SoundMusic");

            ChangeDificuldade(PlayerPrefs.GetInt("Dificuldade"));

            ChangeLanguage(PlayerPrefs.GetInt("Language"));
            #endregion

            #region Extra Player
            ChangeSalveExtraPlayer();
            #endregion

            #region Achievement
            for (int i = 0; i < _achievement.Count; i++)
            {
                //print("Load Achievement -> "+ achievement[i]._nameS);

                if (!PlayerPrefs.HasKey(("Achievement" + i).ToString()))
                {
                    PlayerPrefs.SetInt(("Achievement" + i).ToString(), 0);
                    PlayerPrefs.SetFloat(("AchievementFeito" + i).ToString(), 0);
                }
                else
                {
                    achievement[i]._complete = Convert.ToBoolean(PlayerPrefs.GetInt(("Achievement" + i).ToString()));

                    achievement[i]._feito = PlayerPrefs.GetFloat(("AchievementFeito" + i).ToString());

                    //if (achievement[i]._complete)
                    //    achievement[i]._bonus.Invoke();
                }

                yield return new WaitForSeconds(0.01f);
            }

            for (int i = achievement.Count - 1; i > -1; i--)
            {
                if (achievement[i]._complete)
                {
                    if (achievement[i]._feito < achievement[i]._max)
                    {
                        Debug.LogError("Conquista <b>" + achievement[i]._name + "</b> foi alterada.\nfeito:" + achievement[i]._feito + " / max:" + achievement[i]._max);

                        achievement[i]._complete = false;
                        PlayerPrefs.SetInt(("Achievement" + i).ToString(), Convert.ToInt32(false));

                        Achievement PlatinaDlc = GetAchievementDlc(achievement[i]._dlc);

                        int platinaIndex = SeachIndexAchievement(PlatinaDlc);

                        PlatinaDlc._complete = false;
                        PlayerPrefs.SetInt(("Achievement" + platinaIndex).ToString(), Convert.ToInt32(false));

                        PlatinaDlc._feito--;
                        PlayerPrefs.SetFloat(("AchievementFeito" + platinaIndex).ToString(), PlatinaDlc._feito);

                        CompAchieNewVersion(i);

                        yield return new WaitForSeconds(0.01f);
                    }
                }

                if (!achievement[i]._complete && achievement[i]._feito >= achievement[i]._max)
                    CheckAchievement(i, achievement[i]._feito, false);
            }
            #endregion            

            #region Fases
            ChangeSalveFasePlayer();
            #endregion

            #region Battle Mode
            _battleModeBlocked = Convert.ToBoolean(PlayerPrefs.GetInt("BattleModeBlocked"));

            #region Config
            BattleModeOptionUniqueSelection = Convert.ToBoolean(PlayerPrefs.GetInt("BattleModeUniqueSelection"));
            BattleModeOptionPassiveMobActive = Convert.ToBoolean(PlayerPrefs.GetInt("BattleModePassiveMobActive"));
            BattleModeOptionRoundActive = Convert.ToBoolean(PlayerPrefs.GetInt("BattleModeRoundActive"));

            //Status
            BattleModeOptionStatusHpBaseExtra = PlayerPrefs.GetInt("BattleModeStatusHp");
            BattleModeOptionStatusDamageBaseExtra = PlayerPrefs.GetInt("BattleModeStatusDamage");
            BattleModeOptionStatusCriticalChanceBaseExtra = PlayerPrefs.GetFloat("BattleModeStatusCriticalChance");
            BattleModeOptionCriticalDamageActive = BattleModeOptionStatusCriticalChanceBaseExtra > 0 ? true : false;

            //Resistence
            BattleModeOptionDbuffFireResistenceExtra = PlayerPrefs.GetFloat("BattleModeDbuffFireResistence");
            BattleModeOptionDbuffPoisonResistenceExtra = PlayerPrefs.GetFloat("BattleModeDbuffPoisonResistence");
            BattleModeOptionDbuffPetrifyResistenceExtra = PlayerPrefs.GetFloat("BattleModeDbuffPetrifyResistence");
            BattleModeOptionDbuffBleedResistenceExtra = PlayerPrefs.GetFloat("BattleModeDbuffBleedResistence");
            #endregion
            #endregion
        }
        else //New game
        {
            PlayerPrefs.SetInt("PlayerId", _playerId);
            PlayerPrefs.SetInt("0Skin", 0);

            #region Mobs               
            for (int mob = 0; mob < PlayerCount; mob++)
            {
                if (mob != 0)
                    PlayerPrefs.SetInt(mob + "Blocked", Convert.ToInt32(true));

                #region Skins
                int SkinCount = Mob[mob]._skinHero.Count;
                for (int skin = 0; skin < SkinCount; skin++)
                {
                    if (skin != 0)
                    {
                        Mob[mob]._skinHero[skin]._skinBlocked = true;
                        PlayerPrefs.SetInt(mob + "Skin" + skin, Convert.ToInt32(true));
                    }
                    else
                    {
                        Mob[mob]._skinHero[skin]._skinBlocked = false;
                        PlayerPrefs.SetInt(mob + "Skin" + skin, Convert.ToInt32(false));
                    }

                    yield return new WaitForSeconds(0.01f);
                }
                #endregion

                PlayerPrefs.SetInt((mob) + "Skin", 0);
            }

            MobDesblock(0, false);
            #endregion

            #region Prop's
            PlayerPrefs.SetInt("Damage", _damage);
            PlayerPrefs.SetFloat("Health", _health);
            PlayerPrefs.SetInt("Walk", _walk);
            #endregion

            #region Resistences
            PlayerPrefs.SetFloat("FireResistence", _fireResistence);
            PlayerPrefs.SetFloat("PoisonResistence", _poisonResistence);
            PlayerPrefs.SetFloat("PetrifyResistence", _petrifyResistence);
            PlayerPrefs.SetFloat("BleedResistence", _bleedResistence);
            #endregion

            #region Config
            PlayerPrefs.SetInt("MoveCameraArrow", Convert.ToInt32(_moveCameraArrow));
            PlayerPrefs.SetFloat("MouseSensibility", _mouseSensibility);
            PlayerPrefs.SetFloat("SoundEffect", _soundEffect);
            PlayerPrefs.SetFloat("SoundMusic", _soundMusic);

            PlayerPrefs.SetInt("Dificuldade", _DificuldadeInicial);
            PlayerPrefs.SetInt("Language", 0);
            #endregion

            #region Extra Player
            for (int p = 1; p < PlayerCount; p++)
            {
                yield return new WaitForSeconds(0.01f);

                string _P = "";

                if (p != 1)
                {
                    _P = p.ToString("F0");
                }

                PlayerPrefs.SetInt("TotalWalkers" + _P, _totalWalkers);
                PlayerPrefs.SetInt("TotalTurnos" + _P, _totalTurnos);

                PlayerPrefs.SetInt("TotalDanoRecebido" + _P, _totalDanoRecebido);
                PlayerPrefs.SetInt("TotalDanoCausado" + _P, _totalDanoCausado);
                PlayerPrefs.SetInt("TotalDanoDefendido" + _P, _totalDanoDefendido);

                PlayerPrefs.SetInt("TotalVidaRecuperada" + _P, _totalVidaRecuperada);

                PlayerPrefs.SetInt("TotalTimeHour" + _P, _totalTimeHour);
                PlayerPrefs.SetInt("TotalTimeMinutes" + _P, _totalTimeMinutes);
                PlayerPrefs.SetInt("TotalTimeSeg" + _P, _totalTimeSeg);

                PlayerPrefs.SetInt("TotalGameOver" + _P, _totalGameOver);
                PlayerPrefs.SetInt("TotalMorteMob" + _P, _totalMorteMob);

                PlayerPrefs.GetInt("TotalGetBurn" + _P, _totalGetBurn);
                PlayerPrefs.GetInt("TotalGetPoison" + _P, _totalGetPoison);
                PlayerPrefs.GetInt("TotalGetPetrify" + _P, _totalGetPetrify);
                PlayerPrefs.GetInt("TotalGetStun" + _P, _totalGetStun);
                PlayerPrefs.GetInt("TotalGetBleed" + _P, _totalGetBleed);

                PlayerPrefs.GetInt("TotalSetBurn" + _P, _totalSetBurn);
                PlayerPrefs.GetInt("TotalSetPoison" + _P, _totalSetPoison);
                PlayerPrefs.GetInt("TotalSetPetrify" + _P, _totalSetPetrify);
                PlayerPrefs.GetInt("TotalSetStun" + _P, _totalSetStun);
                PlayerPrefs.GetInt("TotalSetBleed" + _P, _totalSetBleed);
            }
            #endregion

            #region Achievement
            for (int i = 0; i < achievement.Count; i++)
            {
                yield return new WaitForSeconds(0.01f);
                PlayerPrefs.SetInt(("Achievement" + i).ToString(), 0);
                PlayerPrefs.SetFloat(("AchievementFeito" + i).ToString(), 0);
            }
            #endregion

            #region Fases
            for (int i = 0; i < FaseCount; i++)
            {
                for (int p = 0; p < PlayerCount; p++)
                {

                    for (int d = 0; d < (int)_dificuldadeCount - 1; d++)
                    {
                        yield return new WaitForSeconds(0.01f);

                        string _D = (d == 0 ? "" : d.ToString("F0"));

                        string _P = (p > 0 ? p.ToString("F0") : "");

                        #region Dont Block                   
                        PlayerPrefs.SetInt((_D + "F" + i + _P).ToString(), 0);
                        #endregion

                        #region Complete
                        PlayerPrefs.SetInt((_D + "F" + i + "Complete" + _P).ToString(), 0);
                        #endregion

                        #region Kill
                        PlayerPrefs.SetInt((_D + "F" + i + "CompleteKill" + _P).ToString(), 0);
                        #endregion

                        #region Portal
                        PlayerPrefs.SetInt((_D + "F" + i + "CompletePortal" + _P).ToString(), Mob[p]._Fases[d].Fase[i]._completePortal);
                        #endregion

                        #region Time
                        PlayerPrefs.SetFloat((_D + "F" + i + "Seg" + _P).ToString(), -1);
                        PlayerPrefs.SetFloat((_D + "F" + i + "Min" + _P).ToString(), -1);
                        #endregion
                    }
                }
            }
            #endregion

            #region Battle Mode
            PlayerPrefs.SetInt("BattleModeBlocked", 1);


            #region Config
            PlayerPrefs.SetInt("BattleModeUniqueSelection", Convert.ToInt32(false));
            PlayerPrefs.SetInt("BattleModePassiveMobActive", Convert.ToInt32(true));
            PlayerPrefs.SetInt("BattleModeRoundActive", Convert.ToInt32(false));

            //Status
            PlayerPrefs.SetInt("BattleModeStatusHp", 0);
            PlayerPrefs.SetInt("BattleModeStatusDamage", 0);
            PlayerPrefs.SetFloat("BattleModeStatusCriticalChance", 0);

            //Resistence
            PlayerPrefs.SetFloat("BattleModeDbuffFireResistence", 0);
            PlayerPrefs.SetFloat("BattleModeDbuffPoisonResistence", 0);
            PlayerPrefs.SetFloat("BattleModeDbuffPetrifyResistence", 0);
            PlayerPrefs.SetFloat("BattleModeDbuffBleedResistence", 0);
            #endregion
            #endregion
        }

        if (CheckMobBanned())
        {
            NewInfo(
                AttDescriçãoMult(
                    ("<color=red>" +
                    (XmlMenuInicial.Instance != null ?
                    XmlMenuInicial.Instance.Get(57)
                    : "Opss!!!\nO mob ({0}) que esta selecionado esta banido!!!"
                    )
                    + "</color>"),
                    HeroName())
                , 5f);

            ChangePlayer();
        }

        if (CheckMobBlocked())
        {
            NewInfo(
                AttDescriçãoMult(
                    (XmlMenuInicial.Instance != null ?
                    "<color=red>" + XmlMenuInicial.Instance.Get(58) + "</color>" :
                    "Opss!!!\nO mob ({0}) que esta selecionado esta Bloqueado!!!"),
                    HeroName())
                , 5f);

            ChangePlayer();
        }

        if (CheckSkinBlocked())
        {
            //NewInfo("Opss!!!\nA Skin[" + SkinName() + "] do mob (" + HeroName(PlayerID - 1) + ") que esta selecionado esta Blockeada!!!", 5);

            NewInfo(
                AttDescriçãoMult(
                (XmlMenuInicial.Instance != null ?
                "<color=red>" + XmlMenuInicial.Instance.Get(59) + "</color>" :
                "<color=red>Opss!!!\nA Skin[({0})] do mob (({1})) que esta selecionado esta Blockeada!!!</color>")
                , HeroName()
                , SkinName()),
                5f);

            ChangeSkin(-1, 0);

            NewInfo(
                AttDescriçãoMult(
                (XmlMenuInicial.Instance != null ?
                "<color=red>" + XmlMenuInicial.Instance.Get(66) + "</color>" :
                "<color=red>Skin trocada para:{0}</color>")
                , SkinName()),
                2.5f);
        }

        _audioMixer.SetFloat("EffectVol", _soundEffect);
        _audioMixer.SetFloat("MusicVol", _soundMusic);

        ArrumaTime();

        Debug.LogError("PlayerID[" + _playerId + "] " + HeroName(_playerId - 1) + " - " + SkinName());

        if (IsMobile)
        {
            MoveCameraArrow = false;
        }

        LoadComplete = true;
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();

        MensageVersion.Instance.SalveVersion();

        Destroy(gameObject);
    }

    public string AttDescrição(string descricao,string change, string _New,string _Else="")
    {
        if (descricao.Contains(change))
        {
            string _old = descricao;

              descricao = _old.Replace(change, _New);
        }
        else
            descricao = _Else;

        return descricao;
    }

    /// <summary>
    /// use {0} for change
    /// </summary>
    /// <param name="descricao"></param>
    /// <param name="change"></param>
    /// <param name="_New"></param>
    /// <returns></returns>
    public string AttDescriçãoMult(string descricao, params string[] _New)
    {
        int count = _New.Length;

        for (int i = 0; i < count; i++)
        {
            string change = "{" + i + "}";

            if (descricao.Contains(change) && _New[i]!=null)
            {
                string _old = descricao;

                descricao = _old.Replace(change, _New[i]);
            }
        }

        return descricao;
    }

    public void        LoadLevel         (string nameLevel,  string desc = "Loading...", LoadSceneMode loadMode= LoadSceneMode.Single) { StartCoroutine(LoadLevelCoroutine(nameLevel, desc, loadMode)); }
           IEnumerator LoadLevelCoroutine(string nameLevel,  string desc,                LoadSceneMode loadMode)
    {
        AsyncOperation asyncLoadLevel;
        asyncLoadLevel = SceneManager.LoadSceneAsync(nameLevel, loadMode);

        LoadingBar(desc, asyncLoadLevel.progress);

        while (!asyncLoadLevel.isDone)
        {
            LoadingBar(desc, asyncLoadLevel.progress);
            yield return null;
        }

        LoadingBar(desc, asyncLoadLevel.progress);
    }
    public void        LoadLevel         (int    indexLevel, string desc = "Loading...", LoadSceneMode loadMode = LoadSceneMode.Single) { StartCoroutine(LoadLevelCoroutine(indexLevel, desc, loadMode)); }
           IEnumerator LoadLevelCoroutine(int    indexLevel, string desc,                LoadSceneMode loadMode)
    {
        if (desc== "Loading...")
            desc = XmlMenuInicial.Instance.Get(186)+"..."/*Carregando*/;

        AsyncOperation asyncLoadLevel;
        asyncLoadLevel = SceneManager.LoadSceneAsync(indexLevel, loadMode);

        LoadingBar(desc, asyncLoadLevel.progress);

        while (!asyncLoadLevel.isDone)
        {
            LoadingBar(desc, asyncLoadLevel.progress);
            yield return null;
        }

        LoadingBar(desc, asyncLoadLevel.progress);
    }
}
