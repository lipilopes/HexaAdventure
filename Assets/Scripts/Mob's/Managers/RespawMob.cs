using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.ComponentModel;

public class RespawMob : MonoBehaviour
{
    public static RespawMob Instance;

    [Header("Fases")]
    public int faseAtual;
    public Fase fase;
    public enum Fase
    {
        tutorial,
        fase1,
        fase2,
        fase3,
        fase4,
        fase5,
        fase6,
        fase7,
        fase8,
        fase9,
        fase10,
        fase11,
        fase12
    }
    [HideInInspector]
    public int maxFase;
    public int[] build;

    [Header("Mob's")]
    public GameObject[] mob;

    [Header("Cenario")]
    public GameObject[] obstaculo;

    [SerializeField,Header("SkyBox")]
                     Material[]  skybox;
    [SerializeField] Light   [] lightScene;

    [Space,SerializeField]
    RespawRecHp    respawItem;
    //EffectManager effectManager;
    ButtonManager buttonManager;

     GameManagerScenes _gms;

    EventManager em;

    int _playerID;
    [SerializeField] GameObject _player;
    public GameObject SetPlayer
    {
        set
        {
            //Debug.LogError(_player.name+" é o player");

            _player = value;
        }
    }
    public GameObject Player
    {
        get
        {
            //Debug.LogError(_player.name+" é o player");

            return _player;
        }
    }

    public MobManager.MobTime PlayerTime
    {
        get
        {
            if (_player != null)
                return _player.GetComponent<MobManager>().TimeMob;
            else
                return MobManager.MobTime.None;
        }
    }

    [Header("Todos os Respawnados")]
    public List<GameObject> allRespaws;

    WaitForSeconds waitShowMob = new WaitForSeconds(0.5f);

    void Reset()
    {
        buttonManager = GetComponent<ButtonManager>();

        mob = new GameObject[24];

        mob[0] = GameObject.FindGameObjectWithTag("Portal");
        mob[1] = GameObject.FindGameObjectWithTag("Player");

        #region mobs
        //Easy      
        mob[2] = GameObject.FindGameObjectWithTag("Slime");
        mob[3] = GameObject.FindGameObjectWithTag("Slimefogo");
        mob[4] = GameObject.FindGameObjectWithTag("Slimedark");
        mob[5] = GameObject.FindGameObjectWithTag("Cogumelo");
        mob[6] = GameObject.FindGameObjectWithTag("Dargumelo");
        mob[7] = GameObject.FindGameObjectWithTag("Diabinho");
        mob[8] = GameObject.FindGameObjectWithTag("Plantacarnivora");
        mob[9] = GameObject.FindGameObjectWithTag("Caveira");
        //Médios
        mob[10] = GameObject.FindGameObjectWithTag("Morcego");
        mob[11] = GameObject.FindGameObjectWithTag("Zunby");
        mob[12] = GameObject.FindGameObjectWithTag("Unicorniocorrompido");
        mob[13] = GameObject.FindGameObjectWithTag("Ogro");
        mob[14] = GameObject.FindGameObjectWithTag("Ogrofogo");
        mob[15] = GameObject.FindGameObjectWithTag("Macacorei");
        mob[16] = GameObject.FindGameObjectWithTag("Saci");
        //Hard Core mobs
        mob[17] = GameObject.FindGameObjectWithTag("Cavaleiro");
        mob[18] = GameObject.FindGameObjectWithTag("Mago");
        mob[19] = GameObject.FindGameObjectWithTag("Arqueiro");
        mob[20] = GameObject.FindGameObjectWithTag("Poetamaligno");
        mob[21] = GameObject.FindGameObjectWithTag("Sereia");
        mob[22] = GameObject.FindGameObjectWithTag("Meduza");
        mob[23] = GameObject.FindGameObjectWithTag("Gnobus");

        //BOSS
        mob[24] = GameObject.FindGameObjectWithTag("Boss1");
        mob[25] = GameObject.FindGameObjectWithTag("Boss2");

        //Easter Egg
        mob[26] = GameObject.FindGameObjectWithTag("Cyber");

        //DLC I
        #endregion

        #region Obstaculos
        obstaculo = new GameObject[10];

        obstaculo[0] = GameObject.FindGameObjectWithTag("Arvore");
        #endregion
    }

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        //effectManager = (EffectManager)FindObjectOfType(typeof(EffectManager));
        if (buttonManager==null)
        buttonManager = GetComponent<ButtonManager>();        

        _gms = GameManagerScenes._gms;

        if (_gms==null)
            _gms = GameManagerScenes._gms;

        EventManager em = GameObject.FindObjectOfType<EventManager>();

        if (em == null)
            EventBuildFase();
    }

    /// <summary>
    /// Atualiza os Sprites e o nome
    /// </summary>
    /// <param name="Mob"></param>
    /// <param name="original"></param>
    void AttMob(GameObject Mob,GameObject original)
    {
        int idMob = IndexMob(Mob);

        Mob.name = idMob != -1 ? _gms.HeroName(idMob) : original.name;

        if (Mob.GetComponent<ToolTipType>())
        {
            if (idMob != -1 && Mob.GetComponent<MobManager>())
                Mob.GetComponent<ToolTipType>()._name = Mob.name;
        }
    }

    void EffectRespawMob(GameObject Respaw)
    {
        if (Respaw==null)
            return;

        EffectManager.Instance.RespawEffect(Respaw, 2f);

        CameraOrbit.Instance.ChangeTarget(Respaw);

        if (Respaw.GetComponent<ToolTipType>())
            EffectManager.Instance.PopUpDamageEffect(Respaw.GetComponent<ToolTipType>()._name, Respaw, 3);
    }

    public int IndexMob(GameObject obj)
    {
        if (obj!=null)
        {
            for (int i = 0; i < mob.Length; i++)
            {
                if (mob[i].name == obj.name ||
                    mob[i]      == obj)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    GameObject Skin(GameObject mob,int skin)
    {
        if (skin == -1)
            skin = Random.Range(0, _gms.SkinCount(mob));

        GameObject _Skin = _gms.SkinHero(mob, skin);

        if (_Skin!=null  && _Skin!=mob)
        {
            mob = _Skin;
        }

        return mob;
    }

    GameObject CreateMob(int index_Mob, int X,int Y, MobManager.MobTime Time_Mob)
    {
        GameObject grid = GameObject.Find("Hex" + (X) + "x" + (Y));

        GameObject Aliado = mob[index_Mob];

        if (grid != null)//ve si a casa existe
            if (grid.GetComponent<HexManager>().free)
            {
                GameObject Mob = this.GetComponent<CheckGrid>().Respaw(Aliado, X, Y, true, true);

                //int idMob = IndexMob(Mob);

                //Mob.name = idMob != -1 ? _gms.HeroName(idMob) : Aliado.name;           

                //if (Mob.GetComponent<ToolTipType>() != null && idMob != -1)
                //    Mob.GetComponent<ToolTipType>()._nameS = Mob.name;

                if (Mob.GetComponent<MobManager>() != null)
                    Mob.GetComponent<MobManager>().TimeMob = Time_Mob;

                AttMob(Mob, Aliado);

                Debug.LogError("mob criado foi " + Mob.name + " Hex" + X + "x" + Y);

                return Mob;
            }

        Debug.LogWarning("Erro no Respaw Aliado do "+Aliado+" (" + X + "," + Y + "),a casa ja esta ocupada return");
        return null;
    }
    GameObject CreateMob(int index_MobMin, int index_MobMinMax, int X, int Y, MobManager.MobTime Time_Mob)
    {
        int index_Aliado = Random.Range(index_MobMin, index_MobMinMax);

        GameObject Aliado = mob[index_Aliado];

        GameObject grid = GameObject.Find("Hex" + (X) + "x" + (Y));

        if (grid != null)//ve si a casa existe
            if (grid.GetComponent<HexManager>().free)
            {
                GameObject Mob = this.GetComponent<CheckGrid>().Respaw(Aliado, X, Y, true, true);

                AttMob(Mob, Aliado);

                Debug.LogError("mob criado foi " + Mob.name + " Hex" + X + "x" + Y);

                if (Mob.GetComponent<MobManager>() != null)
                    Mob.GetComponent<MobManager>().TimeMob = Time_Mob;
                               
                return Mob;
            }

        Debug.LogWarning("Erro no Respaw Aliado do "+ Aliado + " (" + X + "," + Y + "),a casa ja esta ocupada return");
        return null;
    }

    /// <summary>
    /// Respaw de Varios Mob's em casas aleatorias
    /// </summary>
    /// <param name="Mob0"></param>
    /// <param name="Mob1"></param>
    /// <param name="Mob2"></param>
    /// <param name="Mob3"></param>
    /// <param name="X0"></param>
    /// <param name="Y0"></param>
    /// <param name="X1"></param>
    /// <param name="Y1"></param>
    /// <param name="X2"></param>
    /// <param name="Y2"></param>
    /// <param name="X3"></param>
    /// <param name="Y3"></param>
    public GameObject CreateMob(GameObject Mob0, GameObject Mob1, GameObject Mob2, GameObject Mob3, int X0, int Y0, int X1, int Y1, int X2, int Y2, int X3, int Y3, bool OccupSpace=true, bool Record=true, MobManager.MobTime Time = MobManager.MobTime.None)
    {
        GameObject Mob = null;      
        int V0         = Random.Range(0, 4);

        int X, Xmin = 0, Xmax = 0;
        int Y, Ymin = 0, Ymax = 0;

        #region MAX X/Y
        #region MAX X
        if (X0 > X1)
            Xmax = X0;
        if (X0 > X2)
            Xmax = X0;
        if (X0 > X3)
            Xmax = X0;

        if (X1 > X0)
            Xmax = X1;
        if (X1 > X2)
            Xmax = X1;
        if (X1 > X3)
            Xmax = X1;

        if (X2 > X0)
            Xmax = X2;
        if (X2 > X1)
            Xmax = X2;
        if (X2 > X3)
            Xmax = X2;

        if (X3 > X0)
            Xmax = X3;
        if (X3 > X1)
            Xmax = X3;
        if (X3 > X2)
            Xmax = X3;
        #endregion
        if (Xmax > this.GetComponent<GridMap>().height - 1)
            Xmax = this.GetComponent<GridMap>().height - 1;

        #region max Y
        if (Y0 > Y1)
            Ymax = Y0;
        else if (Y0 > Y2)
            Ymax = Y0;
        else if (Y0 > Y3)
            Ymax = Y0;

        else if (Y1 > Y0)
            Ymax = Y1;
        else if (Y1 > Y2)
            Ymax = Y1;
        else if (Y1 > Y3)
            Ymax = Y1;

        else if (Y2 > Y0)
            Ymax = Y2;
        else if (Y2 > Y1)
            Ymax = Y2;
        else if (Y2 > Y3)
            Ymax = Y2;

        else if (Y3 > Y0)
            Ymax = Y3;
        else if (Y3 > Y1)
            Ymax = Y3;
        else if (Y3 > Y2)
            Ymax = Y3;
        #endregion
        if (Ymax > this.GetComponent<GridMap>().width - 1)
            Ymax = this.GetComponent<GridMap>().width - 1;
        #endregion

        #region MIN X/Y
        #region MIN X
        if (X0 < X1)
            Xmin = X0;
        if (X0 < X2)
            Xmin = X0;
        if (X0 < X3)
            Xmin = X0;

        if (X1 < X0)
            Xmin = X1;
        if (X1 < X2)
            Xmin = X1;
        if (X1 < X3)
            Xmin = X1;

        if (X2 < X0)
            Xmin = X2;
        if (X2 < X1)
            Xmin = X2;
        if (X2 < X3)
            Xmin = X2;

        if (X3 < X0)
            Xmin = X3;
        if (X3 < X1)
            Xmin = X3;
        if (X3 < X2)
            Xmin = X3;
        #endregion
        if (Xmin < 0)
            Xmin = 0;

        #region min Y
        if (Y0 < Y1)
            Ymin = Y0;
        else if (Y0 < Y2)
            Ymin = Y0;
        else if (Y0 < Y3)
            Ymin = Y0;

        else if (Y1 < Y0)
            Ymin = Y1;
        else if (Y1 < Y2)
            Ymin = Y1;
        else if (Y1 < Y3)
            Ymin = Y1;

        else if (Y2 < Y0)
            Ymin = Y2;
        else if (Y2 < Y1)
            Ymin = Y2;
        else if (Y2 < Y3)
            Ymin = Y2;

        else if (Y3 < Y0)
            Ymin = Y3;
        else if (Y3 < Y1)
            Ymin = Y3;
        else if (Y3 < Y2)
            Ymin = Y3;
        #endregion
        if (Ymin < 0)
            Ymin = 0;
        #endregion

        //Debug.LogError("X " + Xmin + " ate " + Xmax);
        //Debug.LogError("Y " + Ymin + " ate " + Ymax);

        X = Random.Range(Xmin, Xmax);
        Y = Random.Range(Ymin, Ymax);

        GameObject grid = GameObject.Find("Hex" + (X) + "x" + (Y));

        if (grid != null)//ve si a casa existe
            if (grid.GetComponent<HexManager>().free)
            {              
                switch (V0)
                {
                    case 0:
                            Mob = this.GetComponent<CheckGrid>().Respaw(Mob0, X, Y,OccupSpace,Record);

                        AttMob(Mob,Mob0);
                        break;

                    case 1:
                            Mob = this.GetComponent<CheckGrid>().Respaw(Mob1, X, Y, OccupSpace, Record);

                        AttMob(Mob, Mob1);
                        break;

                    case 2:
                            Mob = this.GetComponent<CheckGrid>().Respaw(Mob2, X, Y, OccupSpace, Record);

                        AttMob(Mob, Mob2);
                        break;

                    case 3:
                            Mob = this.GetComponent<CheckGrid>().Respaw(Mob3, X, Y, OccupSpace, Record);
                        AttMob(Mob, Mob3);
                        break;
                }
            }

        if (Mob!=null)
        {
            if (Mob.GetComponent<MobManager>() != null)
                Mob.GetComponent<MobManager>().TimeMob = Time;

            Debug.LogWarning("mob criado foi " + Mob.name + " Hex" + X + "x" + Y);
            return Mob;
        }

        Debug.LogWarning("Erro no Respaw do "+Mob+" Aleatorio (" + X + "," + Y + "),a casa ja esta ocupada return");

        return CreateMob(Mob0, Mob1, Mob2, Mob3, X0, Y0, X1, Y1, X2, Y2, X3, Y3, OccupSpace, Record,Time);
    }

    /// <summary>
    /// Respaw um mob em casas aleatorias
    /// </summary>
    /// <param name="Mob0">Mob</param>
    /// <param name="X0"></param>
    /// <param name="Y0"></param>
    /// <param name="X1"></param>
    /// <param name="Y1"></param>
    /// <param name="X2"></param>
    /// <param name="Y2"></param>
    /// <param name="X3"></param>
    /// <param name="Y3"></param>
    /// <param name="OccupSpace">Ocupa espaço</param>
    /// <param name="Record">Vai ser registrado nos turnos</param>
    public GameObject CreateMob(GameObject Mob0, int X0, int Y0, int X1, int Y1, int X2, int Y2, int X3, int Y3, bool OccupSpace = true, bool Record = true, MobManager.MobTime Time = MobManager.MobTime.None)
    {
        int X, Xmin = 0, Xmax = 0;
        int Y, Ymin = 0, Ymax = 0;

        #region MAX X/Y
        #region MAX X
        if (X0 > X1)
            Xmax = X0;
        else if (X0 > X2)
            Xmax = X0;
        else if (X0 > X3)
            Xmax = X0;

        else if (X1 > X0)
            Xmax = X1;
        else if (X1 > X2)
            Xmax = X1;
        else if (X1 > X3)
            Xmax = X1;

        else if (X2 > X0)
            Xmax = X2;
        else if (X2 > X1)
            Xmax = X2;
        else if (X2 > X3)
            Xmax = X2;

        else if (X3 > X0)
            Xmax = X3;
        else if (X3 > X1)
            Xmax = X3;
        else if (X3 > X2)
            Xmax = X3;
        #endregion
        if (Xmax > this.GetComponent<GridMap>().height - 1)
            Xmax = this.GetComponent<GridMap>().height - 1;

        #region max Y
        if (Y0 > Y1)
            Ymax = Y0;
        else if (Y0 > Y2)
            Ymax = Y0;
        else if (Y0 > Y3)
            Ymax = Y0;

        else if (Y1 > Y0)
            Ymax = Y1;
        else if (Y1 > Y2)
            Ymax = Y1;
        else if (Y1 > Y3)
            Ymax = Y1;

        else if (Y2 > Y0)
            Ymax = Y2;
        else if (Y2 > Y1)
            Ymax = Y2;
        else if (Y2 > Y3)
            Ymax = Y2;

        else if (Y3 > Y0)
            Ymax = Y3;
        else if (Y3 > Y1)
            Ymax = Y3;
        else if (Y3 > Y2)
            Ymax = Y3;
        #endregion
        if (Ymax > this.GetComponent<GridMap>().width - 1)
            Ymax = this.GetComponent<GridMap>().width - 1;
        #endregion

        #region MIN X/Y
        #region MIN X
        if (X0 < X1)
            Xmin = X0;
        else if (X0 < X2)
            Xmin = X0;
        else if (X0 < X3)
            Xmin = X0;

        else if (X1 < X0)
            Xmin = X1;
        else if (X1 < X2)
            Xmin = X1;
        else if (X1 < X3)
            Xmin = X1;

        else if (X2 < X0)
            Xmin = X2;
        else if (X2 < X1)
            Xmin = X2;
        else if (X2 < X3)
            Xmin = X2;

        else if (X3 < X0)
            Xmin = X3;
        else if (X3 < X1)
            Xmin = X3;
        else if (X3 < X2)
            Xmin = X3;
        #endregion
        if (Xmin < 0)
            Xmin = 0;

        #region min Y
        if (Y0 < Y1)
            Ymin = Y0;
        else if (Y0 < Y2)
            Ymin = Y0;
        else if (Y0 < Y3)
            Ymin = Y0;

        else if (Y1 < Y0)
            Ymin = Y1;
        else if (Y1 < Y2)
            Ymin = Y1;
        else if (Y1 < Y3)
            Ymin = Y1;

        else if (Y2 < Y0)
            Ymin = Y2;
        else if (Y2 < Y1)
            Ymin = Y2;
        else if (Y2 < Y3)
            Ymin = Y2;

        else if (Y3 < Y0)
            Ymin = Y3;
        else if (Y3 < Y1)
            Ymin = Y3;
        else if (Y3 < Y2)
            Ymin = Y3;
        #endregion
        if (Ymin < 0)
            Ymin = 0;
        #endregion

        //Debug.LogError("X " + Xmin + " ate " + Xmax);
        //Debug.LogError("Y " + Ymin + " ate " + Ymax);

        X = Random.Range(Xmin, Xmax);
        Y = Random.Range(Ymin, Ymax);

        GameObject grid = GameObject.Find("Hex" + (X) + "x" + (Y));

        if (grid != null)//ve si a casa existe
            if (grid.GetComponent<HexManager>().free)
            {
                GameObject Mob= this.GetComponent<CheckGrid>().Respaw(Mob0, X, Y, OccupSpace, Record);

                AttMob(Mob, Mob0);

                Debug.LogError("mob criado foi " + Mob.name + " Hex" + X + "x" + Y);

                if (Mob.GetComponent<MobManager>() != null)
                    Mob.GetComponent<MobManager>().TimeMob = Time;
                return Mob;
            }

        Debug.LogWarning("Erro no Respaw Aleatorio do "+Mob0+" (" + X + "," + Y + "),a casa ja esta ocupada return");
        //CreateMob(Mob0, X0, Y0, X1, Y1, X2, Y2, X3, Y3, OccupSpace, Record);
        return CreateMob(Mob0, X0, Y0, X1, Y1, X2, Y2, X3, Y3, OccupSpace, Record,Time);
    }

    public GameObject CreateMob(GameObject Mob0, int X,int Y, bool OccupSpace = true, bool Record = true,MobManager.MobTime Time = MobManager.MobTime.None)
    {
        GameObject grid = GameObject.Find("Hex" + (X) + "x" + (Y));

        if (grid != null)//ve si a casa existe
            if (grid.GetComponent<HexManager>().free)
            {
                GameObject Mob = this.GetComponent<CheckGrid>().Respaw(Mob0, X, Y, OccupSpace, Record);

                //int idMob = IndexMob(Mob);

                //Mob.name = idMob != -1 ? _gms.HeroName(idMob) : Mob0.name;

                //if (Mob.GetComponent<ToolTipType>() != null && idMob != -1)
                //    Mob.GetComponent<ToolTipType>()._nameS = Mob.name;

                Debug.LogError("mob criado foi " + Mob.name + " Hex" + X + "x" + Y);

                //if (Mob.GetComponent<ToolTipType>() != null && idMob != -1)
                //    Mob.GetComponent<ToolTipType>()._nameS = Mob.name;

                if (Mob.GetComponent<MobManager>() != null)
                    Mob.GetComponent<MobManager>().TimeMob = Time;

                return Mob;
            }

        Debug.LogWarning("Erro no Respaw do "+Mob0+" (" + X + "," + Y + "),a casa ja esta ocupada return");
        return CreateMob(Mob0, X-1, Y-1, X+1, Y+1, X+2, Y+2, X-2, Y-2, OccupSpace, Record,Time);
    }

    /// <summary>
    /// inicia o jogo
    /// </summary>
    /// <param name="FirstTime"></param>
    public void StartTurn(int FirstTime = 0)
    {
        TotalTime.Instance.StartTime();

        if (_gms.GameMode == Game_Mode.History)
            respawItem.ConfigItem();

        GetComponent<TurnSystem>().FirstTurn(FirstTime);
    }

    public void EventBuildFase()
    {
        if (_gms==null)
        {
            Debug.LogWarning("ERRO RespawMob[EventBuildFase]: GameManager nao encontrado");
            return;
        }

        maxFase = _gms.FaseCount;
        RespawFases(_gms.FaseAtual);       
    }

    /// <summary>
    /// Respaw Obstaculos na Fase
    /// </summary>
    public void EventRespawMobFase()
    {
        if (_gms == null)
        {
            Debug.LogWarning("ERRO RespawMob[EventRespawMobFase]: GameManager nao encontrado");
            return;
        }

        buttonManager.ClearHUD();
        
        if (GameManagerScenes.BattleMode)
        {
            RespawBattleMode();
            return;
        }

        int _fase = _gms.FaseAtual;

        switch (_fase)
        {           
            case 0:  RespawTest(); break;
            case 1:  RespawFase1(); break;
            case 2:  RespawFase2(); break;
            case 3:  RespawFase3(); break;
            case 4:  RespawFase4(); break;
            case 5:  RespawFase5(); break;
            case 6:  RespawFase6(); break;
            case 7:  RespawFase7(); break;
            case 8:  RespawFase8(); break;
            case 9:  RespawFase9(); break;
            case 10: RespawFase10(); break;
            case 11: RespawFase11(); break;
            case 12: RespawFase12(); break;
            //case 13: RespawFase13(); break;
        }
    }

    /// <summary>
    /// Respaw Mobs Na Fase
    /// </summary>
    void EventRespawMobOnFase()
    {
        if (_gms == null)
        {
            Debug.LogWarning("ERRO RespawMob[EventRespawMobOnFase]: GameManager nao encontrado");
            return;
        }

        buttonManager.ClearHUD();

        if (GameManagerScenes.BattleMode)
        {
            StartCoroutine(RespawMobBattleModeCoroutine());
            return;
        }

        int _fase = _gms.FaseAtual;

        switch (_fase)
        {
            case 0: StartCoroutine(RespawMobTutorialCoroutine()); break;
            case 01: StartCoroutine(RespawMobFase1Coroutine()); break;
            case 02: StartCoroutine(RespawMobFase2Coroutine()); break;
            case 03: StartCoroutine(RespawMobFase3Coroutine()); break;
            case 04: StartCoroutine(RespawMobFase4Coroutine()); break;
            case 05: StartCoroutine(RespawMobFase5Coroutine()); break;
            case 06: StartCoroutine(RespawMobFase6Coroutine()); break;
            case 07: StartCoroutine(RespawMobFase7Coroutine()); break;
            case 08: StartCoroutine(RespawMobFase8Coroutine()); break;
            case 09: StartCoroutine(RespawMobFase9Coroutine()); break;
            case 10: StartCoroutine(RespawMobFase10Coroutine()); break;
            case 11: StartCoroutine(RespawMobFase11Coroutine()); break;
            case 12: StartCoroutine(RespawMobFase12Coroutine()); break;
            //case 13:  StartCoroutine(RespawMobFase13Coroutine()) break;
        }
    }

    void CompleteBuildFase()
    {
        Debug.LogWarning("___BuildFase Complete__");


        if (GetComponent<GridMap>() != null)
        {           
            GetComponent<GridMap>().CreateGrid();
        }
        else
            Debug.LogWarning("ERRO RespawMob[CompleteBuildFase]: <b>GridMap</b> Não encontrado");

    }
    void CompleteRespawMob()
    {
        if (Player != null && !GameManagerScenes.BattleMode)
            Player.GetComponent<MobManager>().TimeMob = MobManager.MobTime.Player;

        Debug.LogWarning("__RespawMob Complete__");

        PlayerGUI     pG     = GameObject.FindObjectOfType<PlayerGUI>();
        ButtonManager button = GameObject.FindObjectOfType<ButtonManager>();

        pG.GetPlayer(Player);

        button.PlayerInf(Player);

        if (InputManager.Instance != null)
            InputManager.Instance.StartUpdate = true;
        else
        {
            InputManager iM = GameObject.FindObjectOfType<InputManager>();

            iM.StartUpdate = true;
        }

        StartTurn();
    }

    public void RespawFases(int fase)
    {
        if (_gms == null)
            _gms = GameManagerScenes._gms;

        if (GameManagerScenes.BattleMode)
        {
            print("Iniciando Modo Batalha");

            _gms.GetComponentInChildren<MusicLevel>().StartMusic(true, -1);

            ChangeSkybox(Random.Range(0, skybox.Length), Random.Range(0, lightScene.Length));

            BuildBattleMode();
            return;
        }

        print("Iniciando Fase: "+fase);

        int _light  = 0;
        int _skybox = 0;

        _gms.GetComponentInChildren<MusicLevel>().StartMusic(true, fase);

        if (buttonManager.enabled == false)
            buttonManager.enabled = true;

            if (fase != 11 && fase != 12)
            {
                GetComponent<InfoTable>().NewInfo("<b>Para Ganhar</b>" + "\n você tem que" + "\n <b>Chegar vivo ao PORTAL</b>", 8 + 5);
                GetComponent<InfoTable>().NewInfo("Ou" + "\n <b>Matar</b> todos os <b>INIMIGOS</b>", 5 + 5);
            }

            GetComponent<InfoTable>().NewInfo("Boa Sorte!!", 3);           

        if (faseAtual > 0)
            faseAtual = (int)fase - 1;

        switch (fase)
        {
            case 0:
                _light  = Random.Range(0, lightScene.Length);
                _skybox = Random.Range(0, skybox.Length);
                ChangeSkybox(_skybox, _light);

                BuildFaseTest();
                break;             

            #region Fase Easy 1-4
            case 1:
                _light  = 0;
                _skybox = 0;
                ChangeSkybox(_skybox, _light);

                BuildFase1();
                break;

            case 2:
                _skybox = Random.Range(0, 7);
                _light  = 0;
                ChangeSkybox(_skybox, _light);

                BuildFase2();
                break;

            case 3:
                _skybox = Random.Range(0, 7);
                _light  = 0;
                ChangeSkybox(_skybox, _light);

                BuildFase3();
                break;

            case 4:
                _skybox = Random.Range(0, 6);
                _light  = 0;
                ChangeSkybox(_skybox, _light);

                BuildFase4();
                break;
            #endregion

            #region Meduim 5-8
            case 5:
                _skybox = 3;
                _light  = 2;              
                ChangeSkybox(_skybox, _light);

                BuildFase5();
                break;

            case 6:
                _skybox = 3;
                _light  = 2;
                ChangeSkybox(_skybox, _light);

                BuildFase6();
                break;

            case 7:
                _skybox = 3;
                _light  = 2;
                ChangeSkybox(_skybox, _light);

                BuildFase7();
                break;

            case 8:
                _skybox = 3;
                _light  = 2;
                ChangeSkybox(_skybox, _light);

                BuildFase8();
                break;
            #endregion

            #region Hard 9-12
            case 9:
                _skybox = 3;
                _light  = 2;
                ChangeSkybox(_skybox, _light);

                BuildFase9();
                break;

            case 10:
                _skybox = 3;
                _light  = 2;
                ChangeSkybox(_skybox, _light);

                BuildFase10();
                break;

            case 11:
                _skybox = 3;
                _light  = 2;
                ChangeSkybox(_skybox, _light);

                BuildFase11();
                break;

            case 12:
                _skybox = 3;
                _light  = 2;
                ChangeSkybox(_skybox, _light);

                BuildFase12();
                break;
            #endregion
        }        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skyboxIndex"></param>
    /// <param name="lightIndex"></param>
    void ChangeSkybox(int skyboxIndex,int lightIndex=-1)
    {
        if (skyboxIndex <= -1 || lightIndex <= -1)
            return;

        for (int i = 0; i < lightScene.Length; i++)
        {
            lightScene[i].gameObject.SetActive(false);
        }
            

       lightScene[lightIndex].gameObject.SetActive(true);

        RenderSettings.skybox = skybox[skyboxIndex];
    }

    #region Battle Mode
    void BuildBattleMode()
    {
        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[3];
        _ground[0] = 4;
        _ground[1] = 5;
        _ground[2] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();
    }
    public void RespawBattleMode()
    {
        StartCoroutine(RespawObstaculosBattleModeCoroutine());
    }

    IEnumerator RespawObstaculosBattleModeCoroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int number = 0;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                case 0:

                break;
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase12Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobBattleModeCoroutine()
    {
        while (_gms == null)
            yield return null;        

        int numThings = _gms._battleModeGamePlay._battleModeMobs.Count;

        MobManager.MobTime Time0 = MobManager.MobTime.White;
        MobManager.MobTime Time1 = MobManager.MobTime.Black;

        for (int i = 0; i < numThings; i++)
        {
            yield return waitShowMob;

            int mobID  = _gms._battleModeGamePlay._battleModeMobs[i]._idMob,
                skinId = _gms._battleModeGamePlay._battleModeMobs[i]._idMobSkin,
                X      = _gms._battleModeGamePlay._battleModeMobs[i]._XRespaw,
                Y      = _gms._battleModeGamePlay._battleModeMobs[i]._YRespaw,
                Time   = _gms._battleModeGamePlay._battleModeMobs[i]._Time;

            MobManager.MobTime _time = Time == 0 ? Time0 : Time1;

            GameObject Respaw = CreateMob(_gms.SkinHero(mobID, skinId), X, Y, Time: _time);

            Respaw.name = _gms.HeroName(mobID) + " - " +_time+" ["+ Time + "]";            

            bool _isPlayer = _gms._battleModeGamePlay._battleModeMobs[i]._isPlayer;

            if (_isPlayer)
            {
                _player = Respaw;

                Respaw.tag = "Player";

                Respaw.GetComponent<MobManager>().isPlayer = _isPlayer;

                Respaw.AddComponent<PlayerControl>();

                Respaw.GetComponent<ToolTipType>()._classe = "<b><color=green>Player " + (1 + Time) + "</color></b>";
            }

            if (_player == null && i == 1-numThings)
                _player = Respaw;

            EffectRespawMob(Respaw);

            //GameObject P  = mob[1+mobID];
            //MobManager.MobTime _time = Time == 0 ? Time0 : Time1;

            ////if (_gms != null)
            ////    P = _gms.SkinHero(-1, -1);
            ////if (P == null)
            ////    P = mob[_gms.PlayerID];

            //string _tag      = P.tag;
            //bool   _isPlayer = _gms._battleModeGamePlay._battleModeMobs[i]._isPlayer;

            //if (_isPlayer)
            //{
            //    P.tag = "Player";

            //    P.GetComponent<MobManager>().isPlayer = true;
            //}

            //_player = CreateMob(P, X, Y, Time: _time);

            //P.tag = _tag;

            //if (_isPlayer)
            //{
            //    P.GetComponent<MobManager>().isPlayer = _isPlayer;

            //    _player.AddComponent<PlayerControl>();

            //    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player "+(1+ Time)+"</color></b>";
            //}

            EffectRespawMob(_player);

            yield return waitShowMob;
        }        

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Tutorial
    void BuildFaseTest()
    {
        int width  = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        #region Colunas
        int maxObj = GetComponent<GridMap>().objects.Length;

        #region Coluna 0-6
        #region Coluna 0
        build[0]  = Random.Range(0,maxObj); // 0,0
        build[1]  = Random.Range(0, maxObj); // 0,1
        build[2]  = Random.Range(0, maxObj); // 0,2
        build[3]  = Random.Range(0, maxObj); // 0,3
        build[4]  = Random.Range(0, maxObj); // 0,4
        build[5]  = Random.Range(0, maxObj); // 0,5
        build[6]  = Random.Range(0, maxObj); // 0,6
        build[7]  = Random.Range(0, maxObj); // 0,7
        build[8]  = Random.Range(0, maxObj); // 0,8
        build[9]  = Random.Range(0, maxObj); // 0,9
        build[10] = Random.Range(0, maxObj);// 0,10
        build[11] = Random.Range(0, maxObj);// 0,11
        #endregion

        #region Coluna 1
        build[12] = Random.Range(0, maxObj); // 1,0
        build[13] = Random.Range(0, maxObj); // 1,1
        build[14] = Random.Range(0, maxObj); // 1,2
        build[15] = Random.Range(0, maxObj); // 1,3
        build[16] = Random.Range(0, maxObj); // 1,4
        build[17] = Random.Range(0, maxObj); // 1,5
        build[18] = Random.Range(0, maxObj); // 1,6
        build[19] = Random.Range(0, maxObj); // 1,7
        build[20] = Random.Range(0, maxObj); // 1,8
        build[21] = Random.Range(0, maxObj); // 1,9
        build[22] = Random.Range(0, maxObj); // 1,10
        build[23] = Random.Range(0, maxObj); // 1,11
        #endregion

        #region Coluna 2
        build[24] = Random.Range(0, maxObj); // 2,0
        build[25] = Random.Range(0, maxObj); // 2,1
        build[26] = Random.Range(0, maxObj); // 2,2
        build[27] = Random.Range(0, maxObj); // 2,3
        build[28] = Random.Range(0, maxObj); // 2,4
        build[29] = Random.Range(0, maxObj); // 2,5
        build[30] = Random.Range(0, maxObj); // 2,6
        build[31] = Random.Range(0, maxObj); // 2,7
        build[32] = Random.Range(0, maxObj); // 2,8
        build[33] = Random.Range(0, maxObj); // 2,9
        build[34] = Random.Range(0, maxObj); // 2,10
        build[35] = Random.Range(0, maxObj); // 2,11
        #endregion

        #region Coluna 3
        build[36] = Random.Range(0, maxObj); // 3,0
        build[37] = Random.Range(0, maxObj); // 3,1
        build[38] = Random.Range(0, maxObj); // 3,2
        build[39] = Random.Range(0, maxObj); // 3,3
        build[40] = Random.Range(0, maxObj); // 3,4
        build[41] = Random.Range(0, maxObj); // 3,5
        build[42] = Random.Range(0, maxObj); // 3,6
        build[43] = Random.Range(0, maxObj); // 3,7
        build[44] = Random.Range(0, maxObj); // 3,8
        build[45] = Random.Range(0, maxObj); // 3,9
        build[46] = Random.Range(0, maxObj); // 3,10
        build[47] = Random.Range(0, maxObj); // 3,11
        #endregion

        #region Coluna 4
        build[48] = Random.Range(0, maxObj); // 4,0
        build[49] = Random.Range(0, maxObj); // 4,1
        build[50] = Random.Range(0, maxObj); // 4,2
        build[51] = Random.Range(0, maxObj); // 4,3
        build[52] = Random.Range(0, maxObj); // 4,4
        build[53] = Random.Range(0, maxObj); // 4,5
        build[54] = Random.Range(0, maxObj); // 4,6
        build[55] = Random.Range(0, maxObj); // 4,7
        build[56] = Random.Range(0, maxObj); // 4,8
        build[57] = Random.Range(0, maxObj); // 4,9
        build[58] = Random.Range(0, maxObj); // 4,10
        build[59] = Random.Range(0, maxObj); // 4,11
        #endregion

        #region Coluna 5
        build[60] = Random.Range(0, maxObj); // 5,0
        build[61] = Random.Range(0, maxObj); // 5,1
        build[62] = Random.Range(0, maxObj); // 5,2
        build[63] = Random.Range(0, maxObj); // 5,3
        build[64] = Random.Range(0, maxObj); // 5,4
        build[65] = Random.Range(0, maxObj); // 5,5
        build[66] = Random.Range(0, maxObj); // 5,6
        build[67] = Random.Range(0, maxObj); // 5,7
        build[68] = Random.Range(0, maxObj); // 5,8
        build[69] = Random.Range(0, maxObj); // 5,9
        build[70] = Random.Range(0, maxObj); // 5,10
        build[71] = Random.Range(0, maxObj); // 5,11
        #endregion

        #region Coluna 6
        build[72] = Random.Range(0, maxObj); // 6,0
        build[73] = Random.Range(0, maxObj); // 6,1
        build[74] = Random.Range(0, maxObj); // 6,2
        build[75] = Random.Range(0, maxObj); // 6,3
        build[76] = Random.Range(0, maxObj); // 6,4
        build[77] = Random.Range(0, maxObj); // 6,5
        build[78] = Random.Range(0, maxObj); // 6,6
        build[79] = Random.Range(0, maxObj); // 6,7
        build[80] = Random.Range(0, maxObj); // 6,8
        build[81] = Random.Range(0, maxObj); // 6,9
        build[82] = Random.Range(0, maxObj); // 6,10
        build[83] = Random.Range(0, maxObj); // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = Random.Range(0, maxObj); // 7,0
        build[85] = Random.Range(0, maxObj); // 7,1
        build[86] = Random.Range(0, maxObj); // 7,2
        build[87] = Random.Range(0, maxObj); // 7,3
        build[88] = Random.Range(0, maxObj); // 7,4
        build[89] = Random.Range(0, maxObj); // 7,5
        build[90] = Random.Range(0, maxObj); // 7,6
        build[91] = Random.Range(0, maxObj); // 7,7
        build[92] = Random.Range(0, maxObj); // 7,8
        build[93] = Random.Range(0, maxObj); // 7,9
        build[94] = Random.Range(0, maxObj); // 7,10
        build[95] = Random.Range(0, maxObj); // 7,11
        #endregion

        #region Coluna 8
        build[96]  = Random.Range(0, maxObj);  // 8,0
        build[97]  = Random.Range(0, maxObj);  // 8,1
        build[98]  = Random.Range(0, maxObj);  // 8,2
        build[99]  = Random.Range(0, maxObj);  // 8,3
        build[100] = Random.Range(0, maxObj); // 8,4
        build[101] = Random.Range(0, maxObj); // 8,5
        build[102] = Random.Range(0, maxObj); // 8,6
        build[103] = Random.Range(0, maxObj); // 8,7
        build[104] = Random.Range(0, maxObj); // 8,8
        build[105] = Random.Range(0, maxObj); // 8,9
        build[106] = Random.Range(0, maxObj); // 8,10
        build[107] = Random.Range(0, maxObj); // 8,11
        #endregion

        #region Coluna 9
        build[108] = Random.Range(0, maxObj); // 9,0
        build[109] = Random.Range(0, maxObj); // 9,1
        build[110] = Random.Range(0, maxObj); // 9,2
        build[111] = Random.Range(0, maxObj); // 9,3
        build[112] = Random.Range(0, maxObj); // 9,4
        build[113] = Random.Range(0, maxObj); // 9,5
        build[114] = Random.Range(0, maxObj); // 9,6
        build[115] = Random.Range(0, maxObj); // 9,7
        build[116] = Random.Range(0, maxObj); // 9,8
        build[117] = Random.Range(0, maxObj); // 9,9
        build[118] = Random.Range(0, maxObj); // 9,10
        build[119] = Random.Range(0, maxObj); // 9,11
        #endregion

        #region Coluna 10
        build[120] = Random.Range(0, maxObj); // 10,0
        build[121] = Random.Range(0, maxObj); // 10,1
        build[122] = Random.Range(0, maxObj); // 10,2
        build[123] = Random.Range(0, maxObj); // 10,3
        build[124] = Random.Range(0, maxObj); // 10,4
        build[125] = Random.Range(0, maxObj); // 10,5
        build[126] = Random.Range(0, maxObj); // 10,6
        build[127] = Random.Range(0, maxObj); // 10,7
        build[128] = Random.Range(0, maxObj); // 10,8
        build[129] = Random.Range(0, maxObj); // 10,9
        build[130] = Random.Range(0, maxObj); // 10,10
        build[131] = Random.Range(0, maxObj); // 10,11
        #endregion

        #region Coluna 11
        build[132] = Random.Range(0, maxObj); // 10,0
        build[133] = Random.Range(0, maxObj); // 10,1
        build[134] = Random.Range(0, maxObj); // 10,2
        build[135] = Random.Range(0, maxObj); // 10,3
        build[136] = Random.Range(0, maxObj); // 10,4
        build[137] = Random.Range(0, maxObj); // 10,5
        build[138] = Random.Range(0, maxObj); // 10,6
        build[139] = Random.Range(0, maxObj); // 10,7
        build[140] = Random.Range(0, maxObj); // 10,8
        build[141] = Random.Range(0, maxObj); // 10,9
        build[142] = Random.Range(0, maxObj); // 10,10
        build[143] = Random.Range(0, maxObj); // 10,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawTest();
    }
    void RespawTest()
    {
        StartCoroutine(RespawObstaculosTutorialCoroutine());     
    }

    IEnumerator RespawObstaculosTutorialCoroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int number = 1;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                case 0:
                    this.GetComponent<CheckGrid>().Respaw(obstaculo[0], 6, 6, true, false);
                    break;

                case 1://arvore
                       /*GameObject Respaw5 =*/
                    CreateMob(obstaculo[0], 11, 11, 0, 0, 6, 6, 5, 5, Record: false);
                    break;
            }
        }

        yield return null;

        //StartCoroutine(RespawMobTutorialCoroutine());
        EventRespawMobOnFase();
    } 
    IEnumerator RespawMobTutorialCoroutine()
    {
        while (_gms==null)
        {
            yield return null;
        }

        int numThings = 4;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {              
                case 0://_player              
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag    = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 0, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.AddComponent<PlayerControl>();

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);                   

                    yield return waitShowMob;

                    break;


                case 2://Cyber
                    GameObject Respaw2 = CreateMob(_gms.SkinHero(25,0)/*mob[26]*/, 3, 6, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;
                    break;

                case 1://portal
                    GameObject Respaw3 = CreateMob(mob[0], 6, 11, false, false);

                    EffectRespawMob(Respaw3);

                    if (Respaw3.GetComponent<PortalManager>())
                        Respaw3.GetComponent<PortalManager>().Player.Add(Player);

                    yield return waitShowMob;
                    break;

                case 3:
                    if (_gms.Adm)
                    {
                        int X = Player.GetComponent<MoveController>().hexagonX, Y = Player.GetComponent<MoveController>().hexagonY;

                        if (Random.value >= 0.5f)
                            X += Random.Range(1, 3);
                        else
                            X -= Random.Range(1, 3);

                        if (Random.value >= 0.5f)
                            Y += Random.Range(1, 3);
                        else
                            Y -= Random.Range(1, 3);

                        if (X < 0)
                            X = 0;
                        if (X >= GetComponent<GridMap>().Height)
                            X = (int)GetComponent<GridMap>().Height - 1;

                        if (Y < 0)
                            Y = 0;
                        if (Y >= GetComponent<GridMap>().Width)
                            Y = (int)GetComponent<GridMap>().Width - 1;

                       GameObject Respaw4 = CreateMob(Skin(mob[Random.Range(1, _gms.PlayerCount)], 0), X, Y, true, true, PlayerTime);

                        EffectRespawMob(Respaw4);

                        yield return waitShowMob;
                    }
                    break;


                    #region teste
                    //case 3://Normal Slime
                    //    /*GameObject Respaw4 = */CreateMob(mob[2], 7, 6,Time: enemyTime);
                    //    break;

                    //case 4://Fire Slime
                    //    /*GameObject Respaw5 = */CreateMob(mob[3],, 8, 6,Time: enemyTime);
                    //    break;

                    //case 5://Dark Slime
                    //   /*GameObject Respaw6 = */CreateMob(mob[4], 6, 8,Time: enemyTime);
                    //    break;

                    //case 6://COGUMELO
                    //    /*GameObject Respaw7 = */CreateMob(mob[5], 7, 7,Time: enemyTime);
                    //    break;

                    //case 7://DARGOMELO
                    //    /*GameObject Respaw8 = */CreateMob(mob[6], 8, 7,Time: enemyTime);
                    //    break;

                    //case 8://DIABINHO
                    //    /*GameObject Respaw9 = */CreateMob(mob[7], 6, 7,Time: enemyTime);
                    //    break;

                    //case 9://Planta Carnivora
                    //    /*GameObject Respaw10 = */CreateMob(mob[8], 8, 8,Time: enemyTime);
                    //    break;

                    //case 10://Caveira
                    //    /*GameObject Respaw11 = */CreateMob(mob[9], 7, 8,Time: enemyTime);
                    //    break;

                    //case 11://MORCEGO
                    //    /*GameObject Respaw12 = */CreateMob(mob[10], 8, 8,Time: enemyTime);
                    //    break;

                    //case 12://ZUMBY
                    //    /*GameObject Respaw13 = */CreateMob(mob[11],5, 6,Time: enemyTime);
                    //    break;

                    //case 13://UNICORNIO
                    //    /*GameObject Respaw14 = */CreateMob(mob[12], 5, 7,Time: enemyTime);
                    //    break;

                    //case 14://OGRO
                    //    /*GameObject Respa15 = */CreateMob(mob[13], 5, 8,Time: enemyTime);
                    //    break;

                    //case 15://OGRO de FOGO
                    //    /*GameObject Respaw16 = */CreateMob(mob[14], 7, 9,Time: enemyTime);
                    //    break;

                    //case 16://macaco rei
                    //    /*GameObject Respaw17 = */(mob[15], 8, 9,Time: enemyTime);
                    //    break;


                    //case 17://SACI
                    //    /*GameObject Respaw18 = */(mob[16], 6, 9,Time: enemyTime);
                    //    break;

                    //case 18://Cavaleiro
                    //    /*GameObject Respaw19 = */(mob[17], 4, 6,Time: enemyTime);
                    //    break;

                    //case 19://Mago
                    //    /*GameObject Respaw20 = */(mob[18],, 4, 7,Time: enemyTime);
                    //    break;

                    //case 20://Arqueiro
                    //    /*GameObject Respaw21 = */(mob[19], 4, 8,Time: enemyTime);
                    //    break;

                    ////case 21://Xacon
                    ////    /*GameObject Respaw22 = */(mob[20], 7, 0,Time: enemyTime);
                    ////    break;

                    //case 22://Sereia
                    //    /*GameObject Respaw23 = */(mob[21], 7, 9,Time: enemyTime);
                    //    break;

                    //case 23://meduza
                    //    /*GameObject Respaw24 = */(mob[22], 7, 10,Time: enemyTime);
                    //    break;

                    //case 24://Gnobus
                    //    /*GameObject Respaw25 = */(mob[23], 5, 4,Time: enemyTime);
                    //    break;

                    //case 25://Boss1
                    //    /*GameObject Respaw26 = */(mob[24], 4, 4,Time: enemyTime);
                    //    break;

                    //case 26://Boss2
                    //    /*GameObject Respaw27 = */(mob[25], 3, 6,Time: enemyTime);
                    //    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();        
    }
    #endregion

    #region Fases Easy 1 - 4
    #region Fase 1
    void BuildFase1()
    {

        int width = GetComponent<GridMap>().width, 
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[2];
        _ground[0] = 3;
        _ground[1] = 5;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion


        CompleteBuildFase();
        //GetComponent<GridMap>().CreateGrid();
        //RespawFase1();
    }
    void RespawFase1()
    {
        StartCoroutine(RespawObstaculosFase1Coroutine());
    }
    
    IEnumerator RespawObstaculosFase1Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[10];
        _obst[00] = obstaculo[1];
        _obst[01] = obstaculo[4];
        _obst[02] = obstaculo[2];
        _obst[03] = obstaculo[3];
        _obst[04] = obstaculo[0];
        _obst[05] = obstaculo[15];
        _obst[06] = obstaculo[16];
        _obst[07] = obstaculo[17];
        _obst[08] = obstaculo[20];
        _obst[09] = obstaculo[21];

        int number = 11;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                       /*GameObject Respaw4 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 0, true, false);
                    break;

                case 1://Cenario
                       /*GameObject Respaw5 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 1, true, false);
                    break;

                case 2://Cenario
                       /*GameObject Respaw6 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 1, true, false);
                    break;

                case 3://Cenario
                       /*GameObject Respaw7 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 2, true, false);
                    break;

                case 4://Cenario
                       /*GameObject Respaw8 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 3, true, false);
                    break;

                case 5://Cenario
                       /*GameObject Respaw9 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 4, true, false);
                    break;

                case 6://Cenario
                        /*GameObject Respaw10 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 5, true, false);
                    break;

                case 7://Cenario
                        /*GameObject Respaw11 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 5, true, false);
                    break;

                case 8://Cenario
                        /*GameObject Respaw12 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 11, 6, true, false);
                    break;

                case 9://Cenario
                        /*GameObject Respaw13 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 7, true, false);
                    break;

                case 10://Cenario
                        /*GameObject Respaw14 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 7, true, false);
                    break;

                case 11://Cenario
                        /*GameObject Respaw15 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 9, true, false);
                    break;
                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase1Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase1Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 5;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;       

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 =  CreateMob(mob[0], 6, 0, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.AddComponent<PlayerControl>();

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://Slime Dark [4]
                    GameObject Respaw2 = CreateMob(mob[4], 8, 0, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Dargumelo [6]
                    GameObject Respaw3 = CreateMob(mob[6], 6, 2, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;
                #endregion

                #region Respaw mob
                case 4://PlantC[8], diabinho[7], slime[2], cogumelo[5]
                    GameObject Respaw4 = CreateMob(mob[8], mob[7], mob[2], mob[5], 0, 0, 0, 4, 5, 0, 5, 4, Time: enemyTime);

                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;

                case 5://Caveira[9] ,slime[2] ,SlimeFire[3] ,Cogumelo[5]
                    GameObject Respaw5 = CreateMob(mob[9], mob[2], mob[3], mob[5], 7, 3, 7, 6, 11, 3, 11, 6, Time: enemyTime);

                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Fase 2
           void BuildFase2()
    {
        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[2];
        _ground[0] = 3;
        _ground[1] = 5;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();
        //GetComponent<GridMap>().CreateGrid();
        //RespawFase2();
    }
    public void RespawFase2()
    {
        StartCoroutine(RespawObstaculosFase2Coroutine());        
    }

    IEnumerator RespawObstaculosFase2Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[10];
        _obst[00] = obstaculo[1];
        _obst[01] = obstaculo[4];
        _obst[02] = obstaculo[2];
        _obst[03] = obstaculo[3];
        _obst[04] = obstaculo[0];
        _obst[05] = obstaculo[15];
        _obst[06] = obstaculo[16];
        _obst[07] = obstaculo[17];
        _obst[08] = obstaculo[20];
        _obst[09] = obstaculo[21];

        int number = 14;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                       /* GameObject Respaw4 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 1, true, false);
                    break;

                case 1://Cenario
                       /*GameObject Respaw5 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 4, true, false);
                    break;

                case 2://Cenario
                       /*GameObject Respaw6 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 7, true, false);
                    break;

                case 3://Cenario
                       /* GameObject Respaw7 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 5, true, false);
                    break;

                case 4://Cenario
                       /* GameObject Respaw8 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 1, true, false);
                    break;

                case 5://Cenario
                       /*GameObject Respaw9 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 8, true, false);
                    break;

                case 6://Cenario
                        /*GameObject Respaw10 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 4, true, false);
                    break;

                case 7://Cenario
                        /* GameObject Respaw11 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 6, true, false);
                    break;

                case 8://Cenario
                        /*GameObject Respaw12 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 9, true, false);
                    break;

                case 9://Cenario
                        /*GameObject Respaw13 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 3, true, false);
                    break;

                case 10://Cenario
                        /*GameObject Respaw14 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 7, true, false);
                    break;

                case 11://Cenario
                        /*GameObject Respaw15 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 1, true, false);
                    break;

                case 12://Cenario
                        /*GameObject Respaw16 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 3, true, false);
                    break;

                case 13://Cenario
                        /*GameObject Respaw17 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 6, true, false);
                    break;
                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase2Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase2Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 6;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                     GameObject Respaw1 =this.GetComponent<CheckGrid>().Respaw(mob[0], 6, 0, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    _player.AddComponent<PlayerControl>();

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://cogumelo [5]
                       GameObject Respaw2 = CreateMob(mob[5], 7, 0, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Caveira [9]
                       GameObject Respaw3 = CreateMob(mob[9], 3, 2, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;

                #endregion

                #region Random mob's
                case 4://PlantC[8], slimefogo[3], slime[2], PlantC[8]
                        GameObject Respaw4 = CreateMob(mob[8], mob[3], mob[2], mob[8], 6, 4, 6, 5, 8, 4, 8, 5, Time: enemyTime);
                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;

                case 5://Dargumelo[6] ,Diabinho[7] ,Cogumelo[5] ,Cogumelo[5]
                        GameObject Respaw5 = CreateMob(mob[6], mob[7], mob[5], mob[5], 0, 0, 0, 6, 2, 6, 2, 5, Time: enemyTime);
                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Fase 3
           void BuildFase3()
    {

        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }
        int[] _ground = new int[2];
        _ground[0] = 3;
        _ground[1] = 5;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawFase3();
    }
    public void RespawFase3()
    {
        StartCoroutine(RespawObstaculosFase3Coroutine());
    }

     IEnumerator RespawObstaculosFase3Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[10];
        _obst[00] = obstaculo[1];
        _obst[01] = obstaculo[4];
        _obst[02] = obstaculo[2];
        _obst[03] = obstaculo[3];
        _obst[04] = obstaculo[0];
        _obst[05] = obstaculo[15];
        _obst[06] = obstaculo[16];
        _obst[07] = obstaculo[17];
        _obst[08] = obstaculo[20];
        _obst[09] = obstaculo[21];

        int number = 14;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                       /* GameObject Respaw4 = */
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 1, true, false);
                    break;

                case 1://Cenario
                       /*GameObject Respaw5 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 4, true, false);
                    break;

                case 2://Cenario
                       /*GameObject Respaw6 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 7, true, false);
                    break;

                case 3://Cenario
                       /* GameObject Respaw7 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 5, true, false);
                    break;

                case 4://Cenario
                       /* GameObject Respaw8 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 1, true, false);
                    break;

                case 5://Cenario
                       /*GameObject Respaw9 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 8, true, false);
                    break;

                case 6://Cenario
                       /*GameObject Respaw10 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 4, true, false);
                    break;

                case 7://Cenario
                       /* GameObject Respaw11 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 6, true, false);
                    break;

                case 8://Cenario
                       /*GameObject Respaw12 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 9, true, false);
                    break;

                case 9://Cenario
                       /*GameObject Respaw13 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 3, true, false);
                    break;

                case 10://Cenario
                        /*GameObject Respaw14 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 7, true, false);
                    break;

                case 11://Cenario
                        /*GameObject Respaw15 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 1, true, false);
                    break;

                case 12://Cenario
                        /*GameObject Respaw16 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 3, true, false);
                    break;

                case 13://Cenario
                        /*GameObject Respaw17 =*/
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 6, true, false);
                    break;
                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase3Coroutine());
        EventRespawMobOnFase();
    }
     IEnumerator RespawMobFase3Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 6;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 = CreateMob(mob[0], 6, 0, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    _player.AddComponent<PlayerControl>();

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://carnivora [8]
                       GameObject Respaw2 = CreateMob(mob[8], 5, 0, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Caveira [9]
                       GameObject Respaw3 =  CreateMob(mob[9], 4, 1, Time: enemyTime);
                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;

                case 4://Diabinho [7]
                       GameObject Respaw4 = CreateMob(mob[7], 2, 3, Time: enemyTime);
                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;
                #endregion

                #region Random mob's
                case 24://Cogumelo[5], slimefogo[3], slime[2], Slimedark[4]
                     GameObject Respaw5 =CreateMob(mob[5], mob[3], mob[2], mob[4], 6, 4, 6, 5, 8, 4, 8, 5, Time: enemyTime);
                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;

                case 25://Dargumelo[6] ,Diabinho[7] ,Cogumelo[5] ,Slimedark[4]
                    GameObject Respaw6 =CreateMob(mob[6], mob[7], mob[5], mob[4], 0, 0, 0, 6, 6, 2, 5, 2, Time: enemyTime);
                    EffectRespawMob(Respaw6);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Fase 4
           void BuildFase4()
    {

        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[2];
        _ground[0] = 3;
        _ground[1] = 5;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion


        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawFase4();
    }
    public void RespawFase4()
    {
        StartCoroutine(RespawObstaculosFase4Coroutine());
    }

    IEnumerator RespawObstaculosFase4Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[10];
        _obst[00] = obstaculo[1];
        _obst[01] = obstaculo[4];
        _obst[02] = obstaculo[2];
        _obst[03] = obstaculo[3];
        _obst[04] = obstaculo[0];
        _obst[05] = obstaculo[15];
        _obst[06] = obstaculo[16];
        _obst[07] = obstaculo[17];
        _obst[08] = obstaculo[20];
        _obst[09] = obstaculo[21];

        int number = 17;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 1, true, false);
                    break;

                case 1://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 2, true, false);
                    break;

                case 2://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 2, true, false);
                    break;

                case 3://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 3, true, false);
                    break;

                case 4://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 4, true, false);
                    break;

                case 5://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 5, true, false);
                    break;

                case 6://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 5, true, false);
                    break;

                case 7://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 4, true, false);
                    break;

                case 8://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 7, true, false);
                    break;

                case 9://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 9, true, false);
                    break;

                case 10://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 0, true, false);
                    break;

                case 11://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 8, true, false);
                    break;

                case 12://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 6, true, false);
                    break;

                case 13://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 10, true, false);
                    break;

                case 14://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 1, true, false);
                    break;

                case 15://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 9, true, false);
                    break;

                case 16://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 3, true, false);
                    break;

                case 17://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 6, true, false);
                    break;
                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase4Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase4Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 7;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 = CreateMob(mob[0], 6, 0, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    _player.AddComponent<PlayerControl>();

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://Unicornio [12]
                    GameObject Respaw2 = CreateMob(mob[12], 7, 0, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Diabinho [7]
                    GameObject Respaw3 = CreateMob(mob[7], 2, 1, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;

                case 4://Caveira [9]
                    if (_gms.Dificuldade() == GameManagerScenes.dificuldade.dificil)
                    {
                        GameObject Respaw4 = CreateMob(mob[9], 4, 2, Time: enemyTime);
                        EffectRespawMob(Respaw4);

                        yield return waitShowMob;
                    }
                    break;

                case 5://Planta Carnv [8]
                    GameObject Respaw5 = CreateMob(mob[8], 6, 2, Time: enemyTime);
                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;
                #endregion

                #region Random mob's
                case 6://Cogumelo[5], slimefogo[3], slime[2], Slimedark[4]
                    GameObject Respaw6 = CreateMob(mob[5], mob[3], mob[2], mob[4], 6, 6, 6, 8, 7, 6, 7, 8, Time: enemyTime);
                    EffectRespawMob(Respaw6);

                    yield return waitShowMob;

                    break;

                case 7://Dargumelo[6] ,Diabinho[7] ,Cogumelo[5] ,Slimedark[4]
                    if (_gms.Dificuldade() == GameManagerScenes.dificuldade.dificil)
                    {
                        GameObject Respaw7 = CreateMob(mob[6], mob[7], mob[5], mob[4], 0, 0, 0, 6, 6, 2, 5, 2, Time: enemyTime);
                        EffectRespawMob(Respaw7);

                        yield return waitShowMob;
                    }
                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion
    #endregion

    #region Fases Medias 5 - 8
    #region Fase 5
           void BuildFase5()
    {

        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[4];
        _ground[0] = 0;
        _ground[1] = 1;
        _ground[2] = 2;
        _ground[3] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[00] = 5; // 0,0
        build[01] = 5; // 0,1
        build[02] = 5; // 0,2
        build[03] = 5; // 0,3
        build[04] = 5; // 0,4
        build[05] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[06] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[07] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[08] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[09] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = 5; // 1,0
        build[13] = 5; // 1,1
        build[14] = 5; // 1,2
        build[15] = 5; // 1,3
        build[16] = 5; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = 5; // 2,0
        build[25] = 5; // 2,1
        build[26] = 5; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = 5; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = _ground[Random.Range(0, _ground.Length)]; // 4,0
        build[49] = _ground[Random.Range(0, _ground.Length)]; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = _ground[Random.Range(0, _ground.Length)]; // 6,0
        build[73] = _ground[Random.Range(0, _ground.Length)]; // 6,1
        build[74] = _ground[Random.Range(0, _ground.Length)]; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = _ground[Random.Range(0, _ground.Length)]; // 7,0
        build[85] = _ground[Random.Range(0, _ground.Length)]; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[096] = _ground[Random.Range(0, _ground.Length)];  // 8,0
        build[097] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[098] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[099] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawFase5();
    }
    public void RespawFase5()
    {
        StartCoroutine(RespawObstaculosFase5Coroutine());
    }

    IEnumerator RespawObstaculosFase5Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[13];
        _obst[00] = obstaculo[09];
        _obst[01] = obstaculo[08];
        _obst[02] = obstaculo[11];
        _obst[03] = obstaculo[22];
        _obst[04] = obstaculo[23];
        _obst[05] = obstaculo[24];
        _obst[06] = obstaculo[15];
        _obst[07] = obstaculo[16];
        _obst[08] = obstaculo[17];
        _obst[09] = obstaculo[21];
        _obst[10] = obstaculo[05];
        _obst[11] = obstaculo[06];
        _obst[12] = obstaculo[07];

        int number = 15;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 8, true, false);
                    break;

                case 1://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 1, true, false);
                    break;

                case 2://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 8, true, false);
                    break;

                case 3://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 3, true, false);
                    break;

                case 4://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 6, true, false);
                    break;

                case 5://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 1, true, false);
                    break;

                case 6://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 8, true, false);
                    break;

                case 7://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 1, true, false);
                    break;

                case 8://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 6, true, false);
                    break;

                //Renew
                case 9://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 4, true, false);
                    break;

                case 10://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 10, true, false);
                    break;

                case 11://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 4, true, false);
                    break;

                case 12://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 1, true, false);
                    break;

                case 13://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 8, true, false);
                    break;

                case 14://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 11, true, false);
                    break;

                case 15://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 5, true, false);
                    break;
                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase5Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase5Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 5;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 =  CreateMob(mob[0], 0, 0, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    _player.AddComponent<PlayerControl>();

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://Ogro [13]
                       GameObject Respaw2 = CreateMob(mob[13], 2, 0, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Ogro de fogo [14]
                     GameObject Respaw3 = CreateMob(mob[14], 0, 2, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;
                #endregion


                #region Respaw mob
                case 4://zunby[11], saci[16], morcego[10], carnivora[5]
                        GameObject Respaw4 =CreateMob(mob[11], mob[16], mob[10], mob[8], 4, 2, 8, 2, 4, 3, 8, 3, Time: enemyTime);

                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;

                case 5://morcego[10] ,macacorei[15] ,unicornio[12] ,diabinho[6]
                        GameObject Respaw5 =CreateMob(mob[10], mob[15], mob[12], mob[6], 0, 5, 4, 5, 0, 4, 4, 4, Time: enemyTime);

                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Fase 6
           void BuildFase6()
    {
        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[4];
        _ground[0] = 0;
        _ground[1] = 1;
        _ground[2] = 2;
        _ground[3] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[00] = 5; // 0,0
        build[01] = 5; // 0,1
        build[02] = 5; // 0,2
        build[03] = 5; // 0,3
        build[04] = 5; // 0,4
        build[05] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[06] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[07] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[08] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[09] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = 5; // 1,0
        build[13] = 5; // 1,1
        build[14] = 5; // 1,2
        build[15] = 5; // 1,3
        build[16] = 5; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = 5; // 2,0
        build[25] = 5; // 2,1
        build[26] = 5; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = 5; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = _ground[Random.Range(0, _ground.Length)]; // 4,0
        build[49] = _ground[Random.Range(0, _ground.Length)]; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = _ground[Random.Range(0, _ground.Length)]; // 6,0
        build[73] = _ground[Random.Range(0, _ground.Length)]; // 6,1
        build[74] = _ground[Random.Range(0, _ground.Length)]; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = _ground[Random.Range(0, _ground.Length)]; // 7,0
        build[85] = _ground[Random.Range(0, _ground.Length)]; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[096] = _ground[Random.Range(0, _ground.Length)];  // 8,0
        build[097] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[098] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[099] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawFase6();
    }
    public void RespawFase6()
    {
        StartCoroutine(RespawObstaculosFase6Coroutine());
    }

    IEnumerator RespawObstaculosFase6Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[13];
        _obst[00] = obstaculo[09];
        _obst[01] = obstaculo[08];
        _obst[02] = obstaculo[11];
        _obst[03] = obstaculo[22];
        _obst[04] = obstaculo[23];
        _obst[05] = obstaculo[24];
        _obst[06] = obstaculo[15];
        _obst[07] = obstaculo[16];
        _obst[08] = obstaculo[17];
        _obst[09] = obstaculo[21];
        _obst[10] = obstaculo[05];
        _obst[11] = obstaculo[06];
        _obst[12] = obstaculo[07];

        int number = 19;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 0, true, false);
                    break;

                case 1://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 1, true, false);
                    break;

                case 2://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 2, true, false);
                    break;

                case 3://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 1, true, false);
                    break;

                case 4://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 4, true, false);
                    break;

                case 5://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 4, true, false);
                    break;

                case 6://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 4, true, false);
                    break;

                case 7://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 6, true, false);
                    break;

                case 8://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 6, true, false);
                    break;

                case 9://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 6, true, false);
                    break;

                case 10://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 4, true, false);
                    break;

                //Renew
                case 11://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 7, true, false);
                    break;

                case 12://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 10, true, false);
                    break;

                case 13://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 8, true, false);
                    break;

                case 14://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 1, true, false);
                    break;

                case 15://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 9, true, false);
                    break;

                case 16://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 6, true, false);
                    break;

                case 17://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 3, true, false);
                    break;

                case 18://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 10, true, false);
                    break;

                case 19://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 11, 0, true, false);
                    break;
                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase6Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase6Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 6;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 = CreateMob(mob[0], 0, 0, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.AddComponent<PlayerControl>();

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://Unicornio [12]
                       GameObject Respaw2 = CreateMob(mob[12], 2, 0, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Ogro de fogo [14]
                       GameObject Respaw3 = CreateMob(mob[14], 0, 1, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;

                case 4://MacacoRei [15]
                    yield return waitShowMob;
                    // x 1 , y 2
                    GameObject Respaw4 = CreateMob(mob[15], 1, 2, 0, 1, 1, 3, 2, 2, Time: enemyTime);


                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;
                #endregion

                #region Respaw mob
                case 5://morcego[10], saci[16], MacacoRei[15],Ogro[13]
                        GameObject Respaw5 =CreateMob(mob[10], mob[16], mob[15], mob[13], 4, 3, 6, 6, 4, 5, 6, 7, Time: enemyTime);

                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;

                case 6://macacorei[15] ,saci[16] ,unicornio[12] ,zunby[11]
                        GameObject Respaw6 = CreateMob(mob[15], mob[16], mob[12], mob[11], 0, 5, 4, 5, 0, 4, 4, 4, Time: enemyTime);

                    EffectRespawMob(Respaw6);

                    yield return waitShowMob;

                    break;
                    #endregion

            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Fase 7
    void BuildFase7()
    {
        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[4];
        _ground[0] = 0;
        _ground[1] = 1;
        _ground[2] = 2;
        _ground[3] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[00] = 5; // 0,0
        build[01] = 5; // 0,1
        build[02] = 5; // 0,2
        build[03] = 5; // 0,3
        build[04] = 5; // 0,4
        build[05] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[06] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[07] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[08] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[09] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = 5; // 1,0
        build[13] = 5; // 1,1
        build[14] = 5; // 1,2
        build[15] = 5; // 1,3
        build[16] = 5; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = 5; // 2,0
        build[25] = 5; // 2,1
        build[26] = 5; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = 5; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = _ground[Random.Range(0, _ground.Length)]; // 4,0
        build[49] = _ground[Random.Range(0, _ground.Length)]; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = _ground[Random.Range(0, _ground.Length)]; // 6,0
        build[73] = _ground[Random.Range(0, _ground.Length)]; // 6,1
        build[74] = _ground[Random.Range(0, _ground.Length)]; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = _ground[Random.Range(0, _ground.Length)]; // 7,0
        build[85] = _ground[Random.Range(0, _ground.Length)]; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[096] = _ground[Random.Range(0, _ground.Length)];  // 8,0
        build[097] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[098] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[099] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion


        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawFase7();
    }
    public void RespawFase7()
    {
        StartCoroutine(RespawObstaculosFase7Coroutine());
    }

    IEnumerator RespawObstaculosFase7Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[13];
        _obst[00] = obstaculo[09];
        _obst[01] = obstaculo[08];
        _obst[02] = obstaculo[11];
        _obst[03] = obstaculo[22];
        _obst[04] = obstaculo[23];
        _obst[05] = obstaculo[24];
        _obst[06] = obstaculo[15];
        _obst[07] = obstaculo[16];
        _obst[08] = obstaculo[17];
        _obst[09] = obstaculo[21];
        _obst[10] = obstaculo[05];
        _obst[11] = obstaculo[06];
        _obst[12] = obstaculo[07];

        int number = 22;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 0, true, false);
                    break;

                case 1://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 2, true, false);
                    break;

                case 2://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 2, true, false);
                    break;

                case 3://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 2, true, false);
                    break;

                case 4://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 8, true, false);
                    break;

                case 5://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 4, true, false);
                    break;

                case 6://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 4, true, false);
                    break;

                case 7://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 4, true, false);
                    break;

                case 8://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 5, true, false);
                    break;

                case 9://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 6, true, false);
                    break;

                case 10://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 6, true, false);
                    break;

                case 11://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 9, true, false);
                    break;

                case 12://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 7, true, false);
                    break;

                case 13://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 8, true, false);
                    break;

                //Renew
                case 14://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 9, true, false);
                    break;

                case 15://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 0, true, false);
                    break;

                case 16://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 9, true, false);
                    break;

                case 17://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 3, true, false);
                    break;

                case 18://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 7, true, false);
                    break;

                case 19://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 0, true, false);
                    break;

                case 20://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 11, 9, true, false);
                    break;

                case 21://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 11, 2, true, false);
                    break;

                case 22://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 11, 6, true, false);
                    break;
                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase7Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase7Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 6;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 = CreateMob(mob[0], 0, 0, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    _player.AddComponent<PlayerControl>();

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://Unicornio [12]
                       GameObject Respaw2 = CreateMob(mob[12], 2, 0, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Ogro de fogo [14]
                       GameObject Respaw3 = CreateMob(mob[14], 4, 1, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;

                case 4://MacacoRei [15]
                       GameObject Respaw4 = CreateMob(mob[15], 1, 2, Time: enemyTime);

                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;
                #endregion

                #region Respaw mob
                case 5://morcego[10], zunby[11], zunby[11],Ogro[13]
                        GameObject Respaw5 = CreateMob(mob[10], mob[11], mob[11], mob[13], 3, 3, 4, 3, 3, 5, 4, 5, Time: enemyTime);

                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;

                case 6://Ogro[13] ,morcego[10] ,Caveira[9] ,CarnivoraC[8]
                        GameObject Respaw6 = CreateMob(mob[13], mob[10], mob[9], mob[8], 5, 5, 6, 9, 10, 5, 10, 9, Time: enemyTime);

                    EffectRespawMob(Respaw6);

                    yield return waitShowMob;

                    break;
                    #endregion

            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Fase 8
           void BuildFase8()
    {
        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[4];
        _ground[0] = 0;
        _ground[1] = 1;
        _ground[2] = 2;
        _ground[3] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[00] = 5; // 0,0
        build[01] = 5; // 0,1
        build[02] = 5; // 0,2
        build[03] = 5; // 0,3
        build[04] = 5; // 0,4
        build[05] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[06] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[07] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[08] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[09] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = 5; // 1,0
        build[13] = 5; // 1,1
        build[14] = 5; // 1,2
        build[15] = 5; // 1,3
        build[16] = 5; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = 5; // 2,0
        build[25] = 5; // 2,1
        build[26] = 5; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = 5; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = _ground[Random.Range(0, _ground.Length)]; // 4,0
        build[49] = _ground[Random.Range(0, _ground.Length)]; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = _ground[Random.Range(0, _ground.Length)]; // 6,0
        build[73] = _ground[Random.Range(0, _ground.Length)]; // 6,1
        build[74] = _ground[Random.Range(0, _ground.Length)]; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = _ground[Random.Range(0, _ground.Length)]; // 7,0
        build[85] = _ground[Random.Range(0, _ground.Length)]; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[096] = _ground[Random.Range(0, _ground.Length)];  // 8,0
        build[097] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[098] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[099] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawFase8();
    }
    public void RespawFase8()
    {
        StartCoroutine(RespawObstaculosFase8Coroutine());
    }

    IEnumerator RespawObstaculosFase8Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[13];
        _obst[00] = obstaculo[09];
        _obst[01] = obstaculo[08];
        _obst[02] = obstaculo[11];
        _obst[03] = obstaculo[22];
        _obst[04] = obstaculo[23];
        _obst[05] = obstaculo[24];
        _obst[06] = obstaculo[15];
        _obst[07] = obstaculo[16];
        _obst[08] = obstaculo[17];
        _obst[09] = obstaculo[21];
        _obst[10] = obstaculo[05];
        _obst[11] = obstaculo[06];
        _obst[12] = obstaculo[07];

        int number = 19;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 0, true, false);
                    break;

                case 1://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 1, true, false);
                    break;

                case 2://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 1, true, false);
                    break;

                case 3://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 2, true, false);
                    break;

                case 4://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 3, true, false);
                    break;

                case 5://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 2, true, false);
                    break;

                case 6://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 4, true, false);
                    break;

                case 7://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 4, true, false);
                    break;

                case 8://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 4, true, false);
                    break;

                case 9://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 5, true, false);
                    break;

                case 10://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 6, true, false);
                    break;

                case 11://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 7, true, false);
                    break;

                case 12://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 8, true, false);
                    break;

                //Renew
                case 13://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 10, true, false);
                    break;

                case 14://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 3, true, false);
                    break;

                case 15://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 9, true, false);
                    break;

                case 16://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 0, true, false);
                    break;

                case 17://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 6, true, false);
                    break;

                case 18://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 10, true, false);
                    break;

                case 19://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 11, 3, true, false);
                    break;
                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase8Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase8Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 7;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 =  CreateMob(mob[0], 0, 0, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.AddComponent<PlayerControl>();

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://Morcego [10]
                     GameObject Respaw2 = CreateMob(mob[10], 2, 0, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Ogro [13]
                       GameObject Respaw3 =  CreateMob(mob[13], 1, 1, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;

                case 4://MacacoRei [15]
                     GameObject Respaw4 = CreateMob(mob[15], 0, 2, Time: enemyTime);

                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;

                case 5://Cavaleiro [17]
                       GameObject Respaw5 = CreateMob(mob[17], 2, 2, Time: enemyTime);

                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;
                #endregion

                #region Respaw mob
                case 6://Saci[16], zunby[11], Ogro fogo[14],Ogro[13]
                       GameObject Respaw6 =  CreateMob(mob[16], mob[11], mob[14], mob[13], 0, 6, 0, 8, 4, 8, 4, 5, Time: enemyTime);

                    EffectRespawMob(Respaw6);

                    yield return waitShowMob;

                    break;

                case 7://Unicornio[12] ,Meduza[22] ,Macaco rei[15] ,Unicornio[12]
                        GameObject Respaw7 = CreateMob(mob[12], mob[22], mob[15], mob[12], 7, 5, 8, 1, 11, 5, 11, 1, Time: enemyTime);

                    EffectRespawMob(Respaw7);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion
    #endregion

    #region Fases Hard 9-12
    #region Fase 9
           void BuildFase9()
    {
        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[4];
        _ground[0] = 0;
        _ground[1] = 1;
        _ground[2] = 2;
        _ground[3] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawFase9();
    }
    public void RespawFase9()
    {
        StartCoroutine(RespawObstaculosFase9Coroutine());
    }

    IEnumerator RespawObstaculosFase9Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[9];
        _obst[00] = obstaculo[12];
        _obst[01] = obstaculo[13];
        _obst[02] = obstaculo[14];
        _obst[03] = obstaculo[10];
        _obst[04] = obstaculo[8];
        _obst[05] = obstaculo[15];
        _obst[06] = obstaculo[16];
        _obst[07] = obstaculo[17];
        _obst[08] = obstaculo[11];

        int number = 20;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 0, true, false);
                    break;

                case 1://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 1, true, false);
                    break;

                case 2://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 1, true, false);
                    break;

                case 3://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 1, true, false);
                    break;

                case 4://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 2, true, false);
                    break;

                case 5://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 3, true, false);
                    break;

                case 6://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 3, true, false);
                    break;

                case 7://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 4, true, false);
                    break;

                case 8://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 5, true, false);
                    break;

                case 9://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 5, true, false);
                    break;

                case 10://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 6, true, false);
                    break;

                case 11://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 7, true, false);
                    break;

                case 12://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 7, true, false);
                    break;

                case 13://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 8, true, false);
                    break;


                //Renew

                case 14://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 10, true, false);
                    break;

                case 15://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 8, true, false);
                    break;

                case 16://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 2, true, false);
                    break;

                case 17://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 10, true, false);
                    break;

                case 18://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 9, 10, true, false);
                    break;

                case 19://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 3, true, false);
                    break;

                case 20://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 8, true, false);
                    break;

                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase9Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase9Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 6;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 = CreateMob(mob[0], 0, 11, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 11, 0, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.AddComponent<PlayerControl>();

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://Cavaleiro [17]
                       GameObject Respaw2 = CreateMob(mob[17], 0, 4, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Mago [18]
                       GameObject Respaw3 = CreateMob(mob[18], 2, 6, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;

                case 4://Sereia [21]
                       GameObject Respaw4 = CreateMob(mob[21], 2, 11, Time: enemyTime);

                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;
                #endregion

                #region Respaw mob
                case 5://Arq[19], Meduza[22], Poeta[20] Cyber[26],Meduza[22]
                        GameObject Respaw5 = CreateMob(mob[19], mob[22], /*mob[20]*/mob[26], mob[22], 7, 4, 7, 8, 11, 8, 11, 4, Time: enemyTime);

                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;

                case 6://Meduza[19], Arq[22], Poeta[20] Cyber[26],Gnobus[23]
                        GameObject Respaw6 = CreateMob(mob[22], mob[19], /*mob[20]*/ mob[26], mob[23], 3, 5, 3, 0, 6, 3, 6, 0, Time: enemyTime);

                    EffectRespawMob(Respaw6);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Fase 10
    void BuildFase10()
    {

        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[3];
        _ground[0] = 4;
        _ground[1] = 5;
        _ground[2] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();
        //RespawFase10();
    }
    public void RespawFase10()
    {
        StartCoroutine(RespawObstaculosFase10Coroutine());
    }

    IEnumerator RespawObstaculosFase10Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        GameObject[] _obst = new GameObject[9];
        _obst[00] = obstaculo[12];
        _obst[01] = obstaculo[13];
        _obst[02] = obstaculo[14];
        _obst[03] = obstaculo[10];
        _obst[04] = obstaculo[8];
        _obst[05] = obstaculo[15];
        _obst[06] = obstaculo[16];
        _obst[07] = obstaculo[17];
        _obst[08] = obstaculo[11];

        int number = 19;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                #region obstaculos
                case 0://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 0, true, false);
                    break;

                case 1://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 0, 0, true, false);
                    break;

                case 2://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 1, true, false);
                    break;

                case 3://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 2, true, false);
                    break;

                case 4://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 2, true, false);
                    break;

                case 5://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 6, 2, true, false);
                    break;


                case 6://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 3, true, false);
                    break;

                case 7://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 4, true, false);
                    break;

                case 8://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 5, true, false);
                    break;

                case 9://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 5, true, false);
                    break;

                case 10://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 1, 7, true, false);
                    break;

                case 11://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 5, 7, true, false);
                    break;

                case 12://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 3, 8, true, false);
                    break;

                //Renew

                case 13://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 2, 10, true, false);
                    break;

                case 14://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 4, 9, true, false);
                    break;

                case 15://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 1, true, false);
                    break;

                case 16://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 7, 7, true, false);
                    break;

                case 17://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 8, 9, true, false);
                    break;


                case 18://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 10, 5, true, false);
                    break;

                case 19://Cenario
                    this.GetComponent<CheckGrid>().Respaw(_obst[Random.Range(0, _obst.Length)], 11, 8, true, false);
                    break;

                    #endregion
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase10Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase10Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 7;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 1://portal
                       GameObject Respaw1 = CreateMob(mob[0], 0, 11, false, false);

                    if (Respaw1.GetComponent<PortalManager>())
                        Respaw1.GetComponent<PortalManager>().Player.Add(Player);
                    break;

                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 11, 0, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.AddComponent<PlayerControl>();

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 2://Cavaleiro [17]
                       GameObject Respaw2 = CreateMob(mob[17], 0, 4, Time: enemyTime);

                    EffectRespawMob(Respaw2);

                    yield return waitShowMob;

                    break;

                case 3://Meduza [22]
                       GameObject Respaw3 = CreateMob(mob[22], 0, 6, Time: enemyTime);

                    EffectRespawMob(Respaw3);

                    yield return waitShowMob;

                    break;

                case 4://Arq [19]
                       GameObject Respaw4 =  CreateMob(mob[19], 3, 6, Time: enemyTime);

                    EffectRespawMob(Respaw4);

                    yield return waitShowMob;

                    break;

                case 5://Sereia [21]
                       GameObject Respaw5 = CreateMob(mob[21], 1, 8, Time: enemyTime);

                    EffectRespawMob(Respaw5);

                    yield return waitShowMob;

                    break;
                #endregion

                #region Respaw mob
                case 6://Gnobus[23], Meduza[22], poeta[20] Cyber[26],Gnobus[23]
                       GameObject Respaw6 =  CreateMob(mob[23], mob[22], /*mob[20]*/mob[26], mob[23], 1, 11, 1, 3, 6, 3, 6, 11, Time: enemyTime);

                    EffectRespawMob(Respaw6);

                    yield return waitShowMob;

                    break;

                case 7://Unicornio[17] ,Arq[19] ,Mago[18] ,Mago[18]
                       GameObject Respaw7 = CreateMob(mob[17], mob[19], mob[18], mob[18], 11, 4, 6, 4, 5, 11, 11, 11, Time: enemyTime);

                    EffectRespawMob(Respaw7);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();
    }
    #endregion

    #region Fase 11
    void BuildFase11()
    {

        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[3];
        _ground[0] = 4;
        _ground[1] = 5;
        _ground[2] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();
    }
    public void RespawFase11()
    {
        StartCoroutine(RespawObstaculosFase11Coroutine());
    }

    IEnumerator RespawObstaculosFase11Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int number = 0;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                case 0:

                 break;
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase11Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase11Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 5;

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.AddComponent<PlayerControl>();

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's

                #endregion

                #region Respaw mob
                case 1://Cavaleiro[17] 
                    GameObject respaw1 = CreateMob(mob[17], 11, 6, Time: enemyTime);

                    EffectRespawMob(respaw1);

                    yield return waitShowMob;

                    break;


                case 2://Cavaleiro[17]
                    GameObject respaw2 = CreateMob(mob[17], 0, 6, Time: enemyTime);

                    EffectRespawMob(respaw2);

                    yield return waitShowMob;

                    break;

                case 4://Cavaleiro [17]
                       GameObject respaw4 =CreateMob(mob[17], 4, 1, Time: enemyTime);

                    EffectRespawMob(respaw4);

                    yield return waitShowMob;

                    break;

                case 3://Cavaleiro [17]
                    GameObject respaw3 =CreateMob(mob[17], 7, 1, Time: enemyTime);

                    EffectRespawMob(respaw3);

                    yield return waitShowMob;

                    break;

                case 5://Boss1 [24]
                       GameObject respaw5 = CreateMob(mob[24], 6, 0, Time: enemyTime);

                    EffectRespawMob(respaw5);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();

        GetComponent<InfoTable>().NewInfo("<b>Para Ganhar</b>" + "\n você tem que" + "\n <b>Derrotar todos os inimigos</b>", 8 + 5);
    }
    #endregion

    #region Fase 12
    void BuildFase12()
    {
        int width = GetComponent<GridMap>().width,
            height = GetComponent<GridMap>().height;

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int[] _ground = new int[3];
        _ground[0] = 4;
        _ground[1] = 5;
        _ground[2] = 6;

        #region Colunas
        #region Coluna 0-6
        #region Coluna 0
        build[0] = _ground[Random.Range(0, _ground.Length)]; // 0,0
        build[1] = _ground[Random.Range(0, _ground.Length)]; // 0,1
        build[2] = _ground[Random.Range(0, _ground.Length)]; // 0,2
        build[3] = _ground[Random.Range(0, _ground.Length)]; // 0,3
        build[4] = _ground[Random.Range(0, _ground.Length)]; // 0,4
        build[5] = _ground[Random.Range(0, _ground.Length)]; // 0,5
        build[6] = _ground[Random.Range(0, _ground.Length)]; // 0,6
        build[7] = _ground[Random.Range(0, _ground.Length)]; // 0,7
        build[8] = _ground[Random.Range(0, _ground.Length)]; // 0,8
        build[9] = _ground[Random.Range(0, _ground.Length)]; // 0,9
        build[10] = _ground[Random.Range(0, _ground.Length)];// 0,10
        build[11] = _ground[Random.Range(0, _ground.Length)];// 0,11
        #endregion

        #region Coluna 1
        build[12] = _ground[Random.Range(0, _ground.Length)]; // 1,0
        build[13] = _ground[Random.Range(0, _ground.Length)]; // 1,1
        build[14] = _ground[Random.Range(0, _ground.Length)]; // 1,2
        build[15] = _ground[Random.Range(0, _ground.Length)]; // 1,3
        build[16] = _ground[Random.Range(0, _ground.Length)]; // 1,4
        build[17] = _ground[Random.Range(0, _ground.Length)]; // 1,5
        build[18] = _ground[Random.Range(0, _ground.Length)]; // 1,6
        build[19] = _ground[Random.Range(0, _ground.Length)]; // 1,7
        build[20] = _ground[Random.Range(0, _ground.Length)]; // 1,8
        build[21] = _ground[Random.Range(0, _ground.Length)]; // 1,9
        build[22] = _ground[Random.Range(0, _ground.Length)]; // 1,10
        build[23] = _ground[Random.Range(0, _ground.Length)]; // 1,11
        #endregion

        #region Coluna 2
        build[24] = _ground[Random.Range(0, _ground.Length)]; // 2,0
        build[25] = _ground[Random.Range(0, _ground.Length)]; // 2,1
        build[26] = _ground[Random.Range(0, _ground.Length)]; // 2,2
        build[27] = _ground[Random.Range(0, _ground.Length)]; // 2,3
        build[28] = _ground[Random.Range(0, _ground.Length)]; // 2,4
        build[29] = _ground[Random.Range(0, _ground.Length)]; // 2,5
        build[30] = _ground[Random.Range(0, _ground.Length)]; // 2,6
        build[31] = _ground[Random.Range(0, _ground.Length)]; // 2,7
        build[32] = _ground[Random.Range(0, _ground.Length)]; // 2,8
        build[33] = _ground[Random.Range(0, _ground.Length)]; // 2,9
        build[34] = _ground[Random.Range(0, _ground.Length)]; // 2,10
        build[35] = _ground[Random.Range(0, _ground.Length)]; // 2,11
        #endregion

        #region Coluna 3
        build[36] = _ground[Random.Range(0, _ground.Length)]; // 3,0
        build[37] = _ground[Random.Range(0, _ground.Length)]; // 3,1
        build[38] = _ground[Random.Range(0, _ground.Length)]; // 3,2
        build[39] = _ground[Random.Range(0, _ground.Length)]; // 3,3
        build[40] = _ground[Random.Range(0, _ground.Length)]; // 3,4
        build[41] = _ground[Random.Range(0, _ground.Length)]; // 3,5
        build[42] = _ground[Random.Range(0, _ground.Length)]; // 3,6
        build[43] = _ground[Random.Range(0, _ground.Length)]; // 3,7
        build[44] = _ground[Random.Range(0, _ground.Length)]; // 3,8
        build[45] = _ground[Random.Range(0, _ground.Length)]; // 3,9
        build[46] = _ground[Random.Range(0, _ground.Length)]; // 3,10
        build[47] = _ground[Random.Range(0, _ground.Length)]; // 3,11
        #endregion

        #region Coluna 4
        build[48] = 5; // 4,0
        build[49] = 5; // 4,1
        build[50] = _ground[Random.Range(0, _ground.Length)]; // 4,2
        build[51] = _ground[Random.Range(0, _ground.Length)]; // 4,3
        build[52] = _ground[Random.Range(0, _ground.Length)]; // 4,4
        build[53] = _ground[Random.Range(0, _ground.Length)]; // 4,5
        build[54] = _ground[Random.Range(0, _ground.Length)]; // 4,6
        build[55] = _ground[Random.Range(0, _ground.Length)]; // 4,7
        build[56] = _ground[Random.Range(0, _ground.Length)]; // 4,8
        build[57] = _ground[Random.Range(0, _ground.Length)]; // 4,9
        build[58] = _ground[Random.Range(0, _ground.Length)]; // 4,10
        build[59] = _ground[Random.Range(0, _ground.Length)]; // 4,11
        #endregion

        #region Coluna 5
        build[60] = 5; // 5,0
        build[61] = 5; // 5,1
        build[62] = _ground[Random.Range(0, _ground.Length)]; // 5,2
        build[63] = _ground[Random.Range(0, _ground.Length)]; // 5,3
        build[64] = _ground[Random.Range(0, _ground.Length)]; // 5,4
        build[65] = _ground[Random.Range(0, _ground.Length)]; // 5,5
        build[66] = _ground[Random.Range(0, _ground.Length)]; // 5,6
        build[67] = _ground[Random.Range(0, _ground.Length)]; // 5,7
        build[68] = _ground[Random.Range(0, _ground.Length)]; // 5,8
        build[69] = _ground[Random.Range(0, _ground.Length)]; // 5,9
        build[70] = _ground[Random.Range(0, _ground.Length)]; // 5,10
        build[71] = _ground[Random.Range(0, _ground.Length)]; // 5,11
        #endregion

        #region Coluna 6
        build[72] = 5; // 6,0
        build[73] = 5; // 6,1
        build[74] = 5; // 6,2
        build[75] = _ground[Random.Range(0, _ground.Length)]; // 6,3
        build[76] = _ground[Random.Range(0, _ground.Length)]; // 6,4
        build[77] = _ground[Random.Range(0, _ground.Length)]; // 6,5
        build[78] = _ground[Random.Range(0, _ground.Length)]; // 6,6
        build[79] = _ground[Random.Range(0, _ground.Length)]; // 6,7
        build[80] = _ground[Random.Range(0, _ground.Length)]; // 6,8
        build[81] = _ground[Random.Range(0, _ground.Length)]; // 6,9
        build[82] = _ground[Random.Range(0, _ground.Length)]; // 6,10
        build[83] = _ground[Random.Range(0, _ground.Length)]; // 6,11
        #endregion
        #endregion

        #region 7-11
        #region Coluna 7
        build[84] = 5; // 7,0
        build[85] = 5; // 7,1
        build[86] = _ground[Random.Range(0, _ground.Length)]; // 7,2
        build[87] = _ground[Random.Range(0, _ground.Length)]; // 7,3
        build[88] = _ground[Random.Range(0, _ground.Length)]; // 7,4
        build[89] = _ground[Random.Range(0, _ground.Length)]; // 7,5
        build[90] = _ground[Random.Range(0, _ground.Length)]; // 7,6
        build[91] = _ground[Random.Range(0, _ground.Length)]; // 7,7
        build[92] = _ground[Random.Range(0, _ground.Length)]; // 7,8
        build[93] = _ground[Random.Range(0, _ground.Length)]; // 7,9
        build[94] = _ground[Random.Range(0, _ground.Length)]; // 7,10
        build[95] = _ground[Random.Range(0, _ground.Length)]; // 7,11
        #endregion

        #region Coluna 8
        build[96] = 5;  // 8,0
        build[97] = _ground[Random.Range(0, _ground.Length)];  // 8,1
        build[98] = _ground[Random.Range(0, _ground.Length)];  // 8,2
        build[99] = _ground[Random.Range(0, _ground.Length)];  // 8,3
        build[100] = _ground[Random.Range(0, _ground.Length)]; // 8,4
        build[101] = _ground[Random.Range(0, _ground.Length)]; // 8,5
        build[102] = _ground[Random.Range(0, _ground.Length)]; // 8,6
        build[103] = _ground[Random.Range(0, _ground.Length)]; // 8,7
        build[104] = _ground[Random.Range(0, _ground.Length)]; // 8,8
        build[105] = _ground[Random.Range(0, _ground.Length)]; // 8,9
        build[106] = _ground[Random.Range(0, _ground.Length)]; // 8,10
        build[107] = _ground[Random.Range(0, _ground.Length)]; // 8,11
        #endregion

        #region Coluna 9
        build[108] = _ground[Random.Range(0, _ground.Length)]; // 9,0
        build[109] = _ground[Random.Range(0, _ground.Length)]; // 9,1
        build[110] = _ground[Random.Range(0, _ground.Length)]; // 9,2
        build[111] = _ground[Random.Range(0, _ground.Length)]; // 9,3
        build[112] = _ground[Random.Range(0, _ground.Length)]; // 9,4
        build[113] = _ground[Random.Range(0, _ground.Length)]; // 9,5
        build[114] = _ground[Random.Range(0, _ground.Length)]; // 9,6
        build[115] = _ground[Random.Range(0, _ground.Length)]; // 9,7
        build[116] = _ground[Random.Range(0, _ground.Length)]; // 9,8
        build[117] = _ground[Random.Range(0, _ground.Length)]; // 9,9
        build[118] = _ground[Random.Range(0, _ground.Length)]; // 9,10
        build[119] = _ground[Random.Range(0, _ground.Length)]; // 9,11
        #endregion

        #region Coluna 10
        build[120] = _ground[Random.Range(0, _ground.Length)]; // 10,0
        build[121] = _ground[Random.Range(0, _ground.Length)]; // 10,1
        build[122] = _ground[Random.Range(0, _ground.Length)]; // 10,2
        build[123] = _ground[Random.Range(0, _ground.Length)]; // 10,3
        build[124] = _ground[Random.Range(0, _ground.Length)]; // 10,4
        build[125] = _ground[Random.Range(0, _ground.Length)]; // 10,5
        build[126] = _ground[Random.Range(0, _ground.Length)]; // 10,6
        build[127] = _ground[Random.Range(0, _ground.Length)]; // 10,7
        build[128] = _ground[Random.Range(0, _ground.Length)]; // 10,8
        build[129] = _ground[Random.Range(0, _ground.Length)]; // 10,9
        build[130] = _ground[Random.Range(0, _ground.Length)]; // 10,10
        build[131] = _ground[Random.Range(0, _ground.Length)]; // 10,11
        #endregion

        #region Coluna 11
        build[132] = _ground[Random.Range(0, _ground.Length)]; // 11,0
        build[133] = _ground[Random.Range(0, _ground.Length)]; // 11,1
        build[134] = _ground[Random.Range(0, _ground.Length)]; // 11,2
        build[135] = _ground[Random.Range(0, _ground.Length)]; // 11,3
        build[136] = _ground[Random.Range(0, _ground.Length)]; // 11,4
        build[137] = _ground[Random.Range(0, _ground.Length)]; // 11,5
        build[138] = _ground[Random.Range(0, _ground.Length)]; // 11,6
        build[139] = _ground[Random.Range(0, _ground.Length)]; // 11,7
        build[140] = _ground[Random.Range(0, _ground.Length)]; // 11,8
        build[141] = _ground[Random.Range(0, _ground.Length)]; // 11,9
        build[142] = _ground[Random.Range(0, _ground.Length)]; // 11,10
        build[143] = _ground[Random.Range(0, _ground.Length)]; // 11,11
        #endregion
        #endregion
        #endregion

        CompleteBuildFase();

        //GetComponent<GridMap>().CreateGrid();

        //RespawFase12();
    }
    public void RespawFase12()
    {
        StartCoroutine(RespawObstaculosFase12Coroutine());
    }

    IEnumerator RespawObstaculosFase12Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int number = 0;

        for (int i = 0; i <= number; i++)
        {
            switch (i)
            {
                case 0:

                    break;
            }
        }

        yield return null;

        //StartCoroutine(RespawMobFase12Coroutine());
        EventRespawMobOnFase();
    }
    IEnumerator RespawMobFase12Coroutine()
    {
        while (_gms == null)
        {
            yield return null;
        }

        int numThings = 2;

        string bossName = "";

        MobManager.MobTime enemyTime = MobManager.MobTime.Enemy;

        for (int i = 0; i <= numThings; i++)
        {
            yield return waitShowMob;

            switch (i)
            {
                case 0://_player
                    GameObject P = mob[1];
                    MobManager.MobTime _time = MobManager.MobTime.Player;
                    if (_gms != null)
                    {
                        P = _gms.SkinHero(-1, -1);
                    }

                    if (P == null)
                        P = mob[_gms.PlayerID];

                    string _tag = P.tag;
                    bool _isPlayer = P.GetComponent<MobManager>().isPlayer;

                    P.tag = "Player";

                    P.GetComponent<MobManager>().isPlayer = true;

                    _player = CreateMob(P, 6, 11, Time: _time);

                    P.tag = _tag;

                    P.GetComponent<MobManager>().isPlayer = _isPlayer;

                    _player.AddComponent<PlayerControl>();

                    _player.GetComponent<ToolTipType>()._classe = "<b><color=green>Player</color></b>";

                    EffectRespawMob(_player);

                    yield return waitShowMob;

                    break;

                #region Mob's
                case 1://Boss2 [25]
                    GameObject respaw1 = null;

                    if (_gms.PlayerID != 25)
                    {
                        respaw1 = CreateMob(mob[25], 6, 0, Time: enemyTime);

                        bossName = respaw1.GetComponent<ToolTipType>()._name;
                    }
                    else
                    if (_gms.PlayerID != 1)
                    {
                        respaw1 = CreateMob(mob[1], 6, 0, Time: enemyTime);

                        respaw1.GetComponent<MobManager>().TimeMob = enemyTime;

                        respaw1.GetComponent<MobManager>().classe = MobManager.Classe.manual;

                        respaw1.GetComponent<ToolTipType>()._classe = "<b><color=red>Boss</color></b>";

                        int _dificuldade = System.Convert.ToInt32(_gms.Dificuldade());

                        if (_dificuldade == 1)
                        {
                            respaw1.GetComponent<MobManager>().MaxTimeAttack = 1;
                            respaw1.GetComponent<MobManager>().damage = 80;
                            respaw1.GetComponent<MobManager>().health = 250;
                        }
                        else
                        if (_dificuldade == 2)
                        {
                            respaw1.GetComponent<MobManager>().MaxTimeAttack = 2;
                            respaw1.GetComponent<MobManager>().damage = 100;
                            respaw1.GetComponent<MobManager>().health = 300;
                        }
                        else
                        if (_dificuldade == 3 || _dificuldade == 0)
                        {
                            respaw1.GetComponent<MobManager>().MaxTimeAttack = 3;
                            respaw1.GetComponent<MobManager>().damage = 125;
                            respaw1.GetComponent<MobManager>().health = 400;
                        }

                        bossName = respaw1.GetComponent<ToolTipType>()._name;
                    }

                    EffectRespawMob(respaw1);

                    yield return waitShowMob;

                    break;
                    #endregion
            }
        }

        yield return waitShowMob;

        CameraOrbit.Instance.ResetChangeTarget();

        CompleteRespawMob();

        GetComponent<InfoTable>().NewInfo("<b>Para Ganhar</b>" + "\n você tem que" + "\n <b>Derrotar o '<color=red>" + bossName + "</color>'</b>", 8 + 5);
    }
    #endregion
    #endregion
}