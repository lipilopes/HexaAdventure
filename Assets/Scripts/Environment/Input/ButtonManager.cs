using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    bool cheatAtive = false;
    public void Cheat(GameObject _B)
    {
        if (cheatAtive)
            return;

        cheatAtive = true;

        if (!gms.Adm)
            return;

        GameObject obj = RespawMob.Instance.Player;

        if (obj.GetComponent<SkillManager>())
        {
            for (int i = 0; i < obj.GetComponent<SkillManager>().Skills.Count; i++)
            {
                obj.GetComponent<SkillManager>().Skills[i].CooldownMax = 0;
                obj.GetComponent<MobCooldown>().AttCooldown(0, i);
            }

           /* obj.GetComponent<MobCooldown>().timeCooldownSkill1 = 0;
            obj.GetComponent<MobCooldown>().timeCooldownSkill2 = 0;
            obj.GetComponent<MobCooldown>().timeCooldownSkill3 = 0;*/
        }

        obj.GetComponent<MobManager>().chanceCritical = 100;
        obj.GetComponent<MobManager>().maxTimeWalk    = 99;
        obj.GetComponent<MoveController>().time       = 99;
        obj.GetComponent<MobHealth>().MaxHealth       = 9999999;
        obj.GetComponent<MobHealth>().Health          = 9999999;
        obj.GetComponent<MobHealth>().Defense(99999, obj);

        GameManagerScenes._gms.NewInfo("Trapaça <b>GOD MODE</b> ativada", 1.5f, true);

        _B.SetActive(false);
    }

    public static ButtonManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);
    }

    [Header("Audio")]
                     AudioSource _audioSource;
    [SerializeField] AudioClip   _clickCerto;
    [SerializeField] AudioClip   _clickErrado;
    [SerializeField] AudioClip   _gameOver;
    [SerializeField] AudioClip   _win;
    [SerializeField] AudioClip   _audioMenu;

    [SerializeField] Fade _hudPlayer;

    [Header("Painel 0")]
    public GameObject painelWalk;
    [SerializeField]
    Button _buttonWalk;
    [SerializeField]
    Button _buttonAttack, 
           _buttonEndTurn;

    public Button Walk
    {
        get { return _buttonWalk; }
    }
    public Button Attack
    {
        get { return _buttonAttack; }
    }
    public Button EndTurn
    {
        get { return _buttonEndTurn; }
    }

    [Header("Painel 1")]
    public GameObject painelSkill;
    [SerializeField]
    Button _buttonSkill1;
    [SerializeField]
    Button _buttonSkill2, 
           _buttonSkill3,
           _buttonBack;

    bool ShowRageSkill,
         /*ShowRageSkill1, 
          ShowRageSkill2,
          ShowRageSkill3,*/
         clearColor;
           
    [Header("Painel Game over")]   
    public GameObject  painelGameOver;
    [SerializeField] Button     _buttonTryAgain, 
                                _buttonNextLevel;
    [SerializeField] GameObject imageGameOver,
                                imageYouWin;

    [Header("Painel Pause")]
    [SerializeField] GameObject painelPaused;
    [SerializeField] GameObject painelConfig;
    [SerializeField] Toggle     _changeMoveCamera;
                     bool       _moveCameraArrow = false;

    [SerializeField] Slider     _sliderMouseSensibility;
    [SerializeField] Slider     _sliderSoundEffect;
    [SerializeField] Slider     _sliderSoundMusic;

    MobManager        mobManager;
    EnemyAttack       playerAttack;
//MobAttack         mobAttack;
    RespawMob         listRespaw;
    GameManagerScenes gms;
    SkillManager      playerSkill;
    MobCooldown       playerCooldown;
     
    [HideInInspector]
    public GameObject player;

    bool CheckButtonWalk = false/*,badGameOver*/;
    InfoTable _info;

    [Space]
    [Header("Icon Player")]
    [SerializeField] List<GameObject> _icons = new List<GameObject>();

    public List<GameObject> Icons { get {return _icons; } }

    [SerializeField] GameObject IconBattleModeRound;

    public void CreateIconBattleModeRound(GameObject mob)
    {
        GameObject obj = Instantiate(IconBattleModeRound);

        obj.SetActive(true);

        obj.GetComponent<PlayerGUI>().GetPlayer(mob);

        //_icons.Add(obj);
    }

    public GameObject FindIcon(int index,bool active,string txt)
    {
        if (index<0 || index >= _icons.Count) 
            return null;

        print("FindIcon("+index+","+txt+")");

        GameObject i = _icons[index];

        i.SetActive(active);

        if (i.GetComponentInChildren<Text>())
            i.GetComponentInChildren<Text>().text = txt;

        ToolTip.Instance.AttTooltip();

        return i;
    }
    public GameObject FindIcon(int index, bool active)
    {
        if (index < 0 || index >= _icons.Count)
            return null;

        GameObject i = _icons[index];

        i.SetActive(active);

        if (i.GetComponentInChildren<Text>())
            i.GetComponentInChildren<Text>().text = "";

        return i;
    }

    #region Audio Canvas
    public void ClickAudio(bool certo=true)
    {
        if (certo)
            _audioSource.clip = _clickCerto;
        else
            _audioSource.clip = _clickErrado;

        _audioSource.Play();
    }

    void GameOverAudio()
    {
        gms.GetComponentInChildren<MusicLevel>().StartMusic(true, _gameOver);
    }

    void WinAudio()
    {
        gms.GetComponentInChildren<MusicLevel>().StartMusic(true, _win);
    }
    #endregion

    void Start()
    {
        listRespaw   = GetComponent<RespawMob>();
        _audioSource = GetComponent<AudioSource>();
        _info        = GetComponent<InfoTable>();
        gms          = GameManagerScenes._gms;


        painelGameOver.SetActive(false);
        painelSkill.SetActive(false);
        _buttonBack.gameObject.SetActive(false);

       // _changeMoveCamera.onClick.AddListener(ChangeMoveCamera);
        painelPaused.SetActive(false);

        //Load
        Load();

        //PlayerInf(listRespaw.Player);
        ClearHUD();
    }

    private void Load()
    {
        ChangeMoveCamera();

        if (gms != null)
        {
            _sliderMouseSensibility.GetComponentsInChildren<Text>()[0].text = gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(0), gms.IsMobile ? "Touch" : "Mouse");
            _sliderMouseSensibility.value = gms.MouseSensibility;

            _sliderSoundEffect.GetComponentsInChildren<Text>()[0].text = /*!gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(3) /*: "Efeito"*/;
            _sliderSoundEffect.value      = gms.SoundEffect;

            _sliderSoundMusic.GetComponentsInChildren<Text>()[0].text =/* !gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(4) /*: "Música"*/;
            _sliderSoundMusic.value       = gms.SoundMusic;

            _changeMoveCamera.GetComponentsInChildren<Text>()[0].text = /*!gms.IsMobile ? */XmlMenuInicial.Instance.Get(5) /*: "Mover Camera (Setas)"*/;
            _moveCameraArrow              = gms.MoveCameraArrow;
        }

        GetComponent<InputManager>().cameraOrbit.HowMoveCamera(_moveCameraArrow);

        SensibilityMouse();
    }

    /// <summary>
    /// Muda Controle do player
    /// </summary>
    /// <param name="_player"></param>
    public void PlayerInf(GameObject _player=null)
    {
        if (_player == null)
        {
            player = listRespaw.Player;

            if (player == null)
            {
                if (!GameManagerScenes.BattleMode)
                PlayerInf();

                return;
           }               
        }
        else
            player = _player;


        Debug.LogError("Player Controls alterado para " + player.name);

        if (gms.Adm)
            InfoTable.Instance.NewInfo("Player Controls alterado para " + player.name,3);      

        RespawMob.Instance.SetPlayer = _player;

        PlayerGUI.Instance.GetPlayer(_player);

        mobManager     = player.GetComponent<MobManager>();
        playerAttack   = player.GetComponent<EnemyAttack>();
        //mobAttack    = player.GetComponent<MobAttack>();
        playerSkill    = player.GetComponent<SkillManager>();
        playerCooldown = player.GetComponent<MobCooldown>();

        _buttonSkill1.GetComponentInChildren<ToolTipType>()._name = playerSkill.Skills[0].Nome;
        _buttonSkill2.GetComponentInChildren<ToolTipType>()._name = playerSkill.Skills[1].Nome;
        _buttonSkill3.GetComponentInChildren<ToolTipType>()._name = playerSkill.Skills[2].Nome;

        _buttonSkill1.onClick.RemoveAllListeners();
        _buttonSkill2.onClick.RemoveAllListeners();
        _buttonSkill3.onClick.RemoveAllListeners();

        _buttonSkill1.onClick.AddListener(player.GetComponent<IaAttackMob>().Skill1);
        _buttonSkill1.onClick.AddListener(SkillInUse);       

        _buttonSkill2.onClick.AddListener(player.GetComponent<IaAttackMob>().Skill2);
        _buttonSkill2.onClick.AddListener(SkillInUse);

        _buttonSkill3.onClick.AddListener(player.GetComponent<IaAttackMob>().Skill3);
        _buttonSkill3.onClick.AddListener(SkillInUse);

        player.GetComponent<MobDbuff>().CheckIconsDbuff(-1);

        float defense = player.GetComponent<MobHealth>().defense;

        if (defense > 0)
            FindIcon(5, true, defense.ToString("F0")).GetComponent<ToolTipType>()._descricao =
                gms.AttDescriçãoMult(
                             XmlMenuInicial.Instance.Get(181)//"Efeito _b;Defesa_/b;,\n_b;{1}_/b; tem _color=blue;_b;{2}_/b;_/color; de vida extra."
                            ,XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Escudo)
                            ,player.GetComponent<ToolTipType>()._name//1
                            ,(int)defense+""//2
                            );
        else
            FindIcon(5, false);

        GameManagerScenes._gms.Paused = false;

        if (!mobManager.isPlayer && GameManagerScenes.BattleMode)
            ClearHUD();
    }

    /// <summary>
    /// Desativa Botoes principais para usar skill
    /// </summary>
   public void SkillInUse()
    {
        _buttonAttack.interactable = false;
        _buttonWalk.interactable   = false;

        _buttonBack.gameObject.SetActive(false);

        _buttonAttack.gameObject.SetActive(false);
        _buttonWalk.gameObject.SetActive(false);

        _buttonEndTurn.gameObject.SetActive(false);
        painelSkill.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ativa Botoes principais pois skill foi cancelada
    /// </summary>
    public void SkillInUseCanceled()
    {
        _buttonAttack.interactable = true;
        _buttonWalk.interactable   = true;

        _buttonBack.gameObject.SetActive(true);

        _buttonAttack.gameObject.SetActive(false);
        _buttonWalk.gameObject.SetActive(true);

        _buttonEndTurn.gameObject.SetActive(true);
        painelSkill.gameObject.SetActive(true);
    }

    protected  void Update()
    {
        if (player != null && mobManager != null && playerAttack != null)
        {
            if (/*playerSkill != null && */playerSkill.SkillInUse || playerAttack.useSkill)
            {
            //    SkillInUse();
                return;
            }

            if (mobManager.myTurn/* && playerSkill != null && !playerAttack.useSkill ||
                mobManager.myTurn && !playerAttack.useSkill*/)
            {
                if (/*playerSkill != null && */playerSkill.SkillInUse || playerAttack.useSkill)
                {
                //    SkillInUse();
                    return;
                }

                if (player.GetComponent<MoveController>().time <= 0 && mobManager.walkTurn)
                {
                    _buttonWalk.interactable = false;
                    _buttonBack.interactable = false;
                }

                if (player.GetComponent<MoveController>().time > 0 && mobManager.walkTurn)
                {
                    if (gms.FaseAtual <= 1)
                        _buttonWalk.GetComponentInChildren<ToolTipType>()._descricao = 
                            XmlMenuInicial.Instance.Get(178)+/*Selecione as casas azuis para andar  */
                             "\n <b>" + player.GetComponent<MoveController>().time + "</b> / " + player.GetComponent<MobManager>().maxTimeWalk;

                    _buttonBack.interactable = true;
                }

                #region Skill's Cooldown
                if(playerSkill.Skills[0].SilenceSkill)
                {
                    _buttonSkill1.GetComponentInChildren<ToolTipType>()._descricao =
                        GameManagerScenes._gms.AttDescriçãoMult(
                            XmlMenuInicial.Instance.Get(133)
                            , playerAttack.nameSkill1
                            , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Silence)
                            , "" + playerSkill.Skills[0].SilenceTime);//"A Skill {0} está {1} por {2}"
                    _buttonSkill1.interactable = false;
                }
                else
                if (/*playerCooldown.timeCooldownSkill[0]*/playerSkill.Skills[0].CooldownCurrent > 0)
                {
                    _buttonSkill1.GetComponentInChildren<ToolTipType>()._descricao = 
                        GameManagerScenes._gms.AttDescriçãoMult(
                            XmlMenuInicial.Instance.Get(182)
                            ,""+ playerSkill.Skills[0].CooldownCurrent/*playerCooldown.timeCooldownSkill[0]*/);//"Espere {0} turno(s)!"
                    _buttonSkill1.interactable = false;
                }
                else
                {
                    if (gms.FaseAtual <= 1)
                    {
                        if (!playerAttack.ListTargetSkill1.Contains(playerAttack.target) && playerSkill.Skills[0].NeedTarget)
                            _buttonSkill1.GetComponentInChildren<ToolTipType>()._descricao =/*Sem Alvo */XmlMenuInicial.Instance.Get(184)+" " + playerAttack.nameSkill1;
                        else
                            if(playerAttack.target != null)
                            _buttonSkill1.GetComponentInChildren<ToolTipType>()._descricao =
                                GameManagerScenes._gms.AttDescriçãoMult(
                                 XmlMenuInicial.Instance.Get(185)//"Usar <b>{0}</b> no(a) {1}"
                                , playerAttack.nameSkill1
                                , playerAttack.target.GetComponent<ToolTipType>()._name);
                    }

                    if (playerAttack.target == null && _buttonSkill1.interactable && playerSkill.Skills[0].NeedTarget)
                        _buttonSkill1.GetComponentInChildren<ToolTipType>()._descricao = XmlMenuInicial.Instance.Get(183);//"Selecione Algum Alvo";


                    if (playerAttack.ListTargetSkill1.Contains(playerAttack.target) && playerAttack.ListTargetSkill1.Count > 0 && playerSkill.Skills[0].TargetFriend && !playerSkill.Skills[0].SilenceSkill ||
                        playerAttack.ListTargetSkill1.Contains(playerAttack.target) && playerAttack.ListTargetSkill1.Count > 0 && playerSkill.Skills[0].NeedTarget   && !playerSkill.Skills[0].SilenceSkill ||
                        playerAttack.ListTargetSkill1.Contains(playerAttack.target) && playerAttack.ListTargetSkill1.Count > 0 && playerSkill.Skills[0].TargetMe     && !playerSkill.Skills[0].SilenceSkill/**/)
                        _buttonSkill1.interactable = true;
                    else
                        _buttonSkill1.interactable = false;
                }

                //skill2
                if (playerSkill.Skills[1].SilenceSkill)
                {
                    _buttonSkill1.GetComponentInChildren<ToolTipType>()._descricao =
                        GameManagerScenes._gms.AttDescriçãoMult(
                            XmlMenuInicial.Instance.Get(133)
                            , playerAttack.nameSkill2
                            , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Silence)
                            , "" + playerSkill.Skills[1].SilenceTime);//"A Skill {0} está {1} por {2}"
                    _buttonSkill2.interactable = false;
                }
                else
                if (/*playerCooldown.timeCooldownSkill[1]*/ 
                    playerSkill.Skills[1].CooldownCurrent >= 1)
                {
                    _buttonSkill2.interactable = false;
                    _buttonSkill2.GetComponentInChildren<ToolTipType>()._descricao = GameManagerScenes._gms.AttDescriçãoMult(
                            XmlMenuInicial.Instance.Get(182)
                            , "" + /*playerCooldown.timeCooldownSkill[1]*/playerSkill.Skills[1].CooldownCurrent);//"Espere {0} turno(s)!"
                }
                else
                {
                    if (gms.FaseAtual <= 1)
                    {
                        if (!playerAttack.ListTargetSkill2.Contains(playerAttack.target) && playerSkill.Skills[1].NeedTarget)
                            _buttonSkill2.GetComponentInChildren<ToolTipType>()._descricao = /*Sem Alvo */XmlMenuInicial.Instance.Get(184)+" " +  playerAttack.nameSkill2;
                        else                        
                            if (playerAttack.target != null)
                            _buttonSkill2.GetComponentInChildren<ToolTipType>()._descricao =
                                GameManagerScenes._gms.AttDescriçãoMult(
                                 XmlMenuInicial.Instance.Get(185)//"Usar <b>{0}</b> no(a) {1}"
                                , playerAttack.nameSkill2
                                , playerAttack.target.GetComponent<ToolTipType>()._name);
                    }

                    if (playerAttack.target == null && _buttonSkill2.interactable && playerSkill.Skills[1].NeedTarget)
                        _buttonSkill2.GetComponentInChildren<ToolTipType>()._descricao = XmlMenuInicial.Instance.Get(183);//"Selecione Algum Alvo";


                    if (playerAttack.ListTargetSkill2.Contains(playerAttack.target) && playerAttack.ListTargetSkill2.Count > 0 && playerSkill.Skills[1].TargetFriend && !playerSkill.Skills[1].SilenceSkill ||
                        playerAttack.ListTargetSkill2.Contains(playerAttack.target) && playerAttack.ListTargetSkill2.Count > 0 && playerSkill.Skills[1].NeedTarget   && !playerSkill.Skills[1].SilenceSkill ||
                        playerAttack.ListTargetSkill2.Contains(playerAttack.target) && playerAttack.ListTargetSkill2.Count > 0 && playerSkill.Skills[1].TargetMe     && !playerSkill.Skills[1].SilenceSkill /**/)
                        _buttonSkill2.interactable = true;
                    else
                        _buttonSkill2.interactable = false;
                }

                //SKILL3
                if (playerSkill.Skills[2].SilenceSkill)
                {
                    _buttonSkill1.GetComponentInChildren<ToolTipType>()._descricao =
                        GameManagerScenes._gms.AttDescriçãoMult(
                            XmlMenuInicial.Instance.Get(133)
                            , playerAttack.target.GetComponent<ToolTipType>()._name
                            , XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Silence)
                            , "" + playerSkill.Skills[2].SilenceTime);//"A Skill {0} está {1} por {2}"

                    _buttonSkill3.interactable = false;
                }
                else
                if (/*playerCooldown.timeCooldownSkill[2]*/
                    playerSkill.Skills[2].CooldownCurrent > 0)
                {
                    _buttonSkill3.interactable = false;
                    _buttonSkill3.GetComponentInChildren<ToolTipType>()._descricao = GameManagerScenes._gms.AttDescriçãoMult(
                            XmlMenuInicial.Instance.Get(182)
                            , "" + /*playerCooldown.timeCooldownSkill[2]*/playerSkill.Skills[2].CooldownCurrent);//"Espere {0} turno(s)!"
                }
                else
                {
                    if (gms.FaseAtual <= 1)
                    {
                        if (!playerAttack.ListTargetSkill3.Contains(playerAttack.target) && playerSkill.Skills[2].NeedTarget)
                            _buttonSkill3.GetComponentInChildren<ToolTipType>()._descricao = /*Sem Alvo */XmlMenuInicial.Instance.Get(184) + " " + playerAttack.nameSkill3;
                        else
                            if (playerAttack.target != null)
                            _buttonSkill3.GetComponentInChildren<ToolTipType>()._descricao =
                                GameManagerScenes._gms.AttDescriçãoMult(
                                 XmlMenuInicial.Instance.Get(185)//"Usar <b>{0}</b> no(a) {1}"
                                , playerAttack.nameSkill3
                                , playerAttack.target.GetComponent<ToolTipType>()._name);
                    }


                    if (playerAttack.target == null && _buttonSkill3.interactable && playerSkill.Skills[2].NeedTarget)
                        _buttonSkill3.GetComponentInChildren<ToolTipType>()._descricao = XmlMenuInicial.Instance.Get(183);//"Selecione Algum Alvo";


                    if (playerAttack.ListTargetSkill3.Contains(playerAttack.target) && playerAttack.ListTargetSkill3.Count > 0 && playerSkill.Skills[2].TargetFriend && !playerSkill.Skills[2].SilenceSkill ||
                        playerAttack.ListTargetSkill3.Contains(playerAttack.target) && playerAttack.ListTargetSkill3.Count > 0 && playerSkill.Skills[2].NeedTarget   && !playerSkill.Skills[2].SilenceSkill ||
                        playerAttack.ListTargetSkill3.Contains(playerAttack.target) && playerAttack.ListTargetSkill3.Count > 0 && playerSkill.Skills[2].TargetMe     && !playerSkill.Skills[2].SilenceSkill/**/)
                        _buttonSkill3.interactable = true;
                    else
                        _buttonSkill3.interactable = false;
                }
                #endregion
            }
            else
            {
                //GetComponent<InputManager>().canMoveCamera = false;
                ButtonBackToWalk();
                //painelWalk.gameObject.SetActive(false);
                _buttonAttack.interactable = false;
                _buttonWalk.interactable   = false;
                _buttonEndTurn.gameObject.SetActive(false);
                _buttonBack.gameObject   .SetActive(false);
                painelSkill.gameObject   .SetActive(false);
            }
        }
    }

    public void ClearHUD()//deixa dela limpa
    {
        _buttonEndTurn.gameObject.SetActive   (false);

        painelSkill.gameObject.SetActive      (false);

        painelWalk.gameObject.SetActive       (false);

        painelPaused.SetActive                (false);

        GetComponent<ToolTip>().TargetTooltip (null);       

        for (int i = 0; i < _icons.Count; i++)
        {
            FindIcon(i,false);
        }
    }

    public void ClearHUD(bool clear = true)//deixa dela limpa
    {
        if (_hudPlayer == null)
        {
            _buttonEndTurn.gameObject.SetActive(!clear);

            painelSkill.gameObject.SetActive(!clear);

            painelWalk.gameObject.SetActive(!clear);
        }
        else
        {           
            if (clear)
            {
                _hudPlayer.timeToFade = 1;
                _hudPlayer.delay      = 0;

                _hudPlayer.GetComponent<CanvasGroup>().interactable = false;
                _hudPlayer.FadeOff();
            }
            else
            {
                _hudPlayer.timeToFade = 0.5f;
                _hudPlayer.delay      = 0;

                _hudPlayer.FadeOn();
                _hudPlayer.GetComponent<CanvasGroup>().interactable = true;
            }
        }

        //GetComponent<ToolTip>().TargetTooltip(null);
    }

    public void PlayerTurn()
    {
        Debug.LogError("PlayerTurn() - "+player.name);

        if (!mobManager.isPlayer && GameManagerScenes.BattleMode)
        {
            ClearHUD();
            return;
        }

        //GetComponent<InputManager>().canMoveCamera = false;
        if (!GetComponent<InputManager>().enabled)
            GetComponent<InputManager>().enabled = false;

        if (!GetComponent<ToolTip>().enabled)
            GetComponent<ToolTip>().enabled = false;

        if (painelSkill.activeInHierarchy)
            _buttonBack.gameObject.SetActive(false);


        _buttonAttack.gameObject.SetActive(true);
        _buttonWalk.gameObject.SetActive(true);

        CheckButtonWalk          = false;
        _buttonBack.interactable = true;
        _buttonWalk.interactable = true;

        _buttonEndTurn.gameObject.SetActive(true);
        painelWalk.gameObject.SetActive    (true);

        _buttonAttack.interactable = true;
        _buttonSkill1.interactable = true;
        _buttonSkill2.interactable = true;
        _buttonSkill3.interactable = true;

        if (painelSkill.activeInHierarchy)
            painelSkill.SetActive(false);     
    }

    public void ButtonEndTurn()
    {
        ClickAudio();

        clearColor = true;
        ClearColorHex();

        _buttonEndTurn.gameObject.SetActive(false);
        painelSkill.gameObject.SetActive   (false);
        _buttonBack.gameObject.SetActive   (false);

        if (mobManager != null)
            mobManager.EndTurn();
        else
            player.GetComponent<MobManager>().EndTurn();
    }

    public void ButtonWalk()//botao walk
    {
        if (mobManager.attackTurn)
        {
            ClickAudio(false);
            return;          
        }

        if (player.GetComponent<MoveController>().time <= 0 && CheckButtonWalk || !_buttonWalk.enabled && mobManager.myTurn)
        {
           //_buttonWalk.GetComponentInChildren<ToolTipType>()._descricao = "Você Não pode andar.";
            _info.NewInfo(
                XmlMenuInicial.Instance.Get(180)//"Você Não pode andar."
                , 3);
            ClickAudio(false);
            return;
        }

        if (!mobManager.myTurn && !_buttonWalk.enabled)
        {
            //_buttonWalk.GetComponentInChildren<ToolTipType>()._descricao = "Espere seu turno para poder andar.";
            _info.NewInfo(
                 XmlMenuInicial.Instance.Get(179)//"Espere seu turno para poder andar."
                , 3);
            ClickAudio(false);
            return;
        }    

        if (gms.FaseAtual <= 1)
            if (player.GetComponent<MoveController>().time >= 1 && mobManager.walkTurn && _buttonWalk.enabled && !mobManager.attackTurn)
            {
                //_buttonWalk.GetComponentInChildren<ToolTipType>()._descricao = "Selecione as casas azuis para andar ";        
                _info.NewInfo(
                    XmlMenuInicial.Instance.Get(178)//"Selecione as casas azuis para andar"
                    , 3);

                ClickAudio();
            }   
              
        //GetComponent<InputManager>().canMoveCamera = false;

        if (!CheckButtonWalk)
        {
            ClickAudio();
            mobManager.attackTurn = false;
            mobManager.WalkTurn();           
            CheckButtonWalk = true;
            player.GetComponent<MoveController>().StartWalkTurn();
            player.GetComponent<MoveController>().CheckHome(null, player.GetComponent<MoveController>().hexagonX, player.GetComponent<MoveController>().hexagonY);
            if (ToolTip.Instance != null)
                ToolTip.Instance.AttTooltip();
        }
        else 
        {
            ClickAudio();
            mobManager.walkTurn   = true;
            mobManager.attackTurn = false;
            player.GetComponent<MoveController>().StartWalkTurn();
            player.GetComponent<MoveController>().CheckHome(null, player.GetComponent<MoveController>().hexagonX, player.GetComponent<MoveController>().hexagonY);
        }

        if (!_buttonEndTurn.gameObject.activeSelf)
        _buttonEndTurn.gameObject.SetActive(true);

        if (_buttonBack.gameObject.activeSelf)
            _buttonBack.gameObject.SetActive(false);
    }

    public void ButtonAttack()//botao de atk
    {
        //GetComponent<InputManager>().canMoveCamera = true;

        if (!_buttonEndTurn.gameObject.activeSelf)
            _buttonEndTurn.gameObject.SetActive(true);

        if (!_buttonBack.gameObject.activeSelf)
            _buttonBack.gameObject.SetActive(true);

        _buttonWalk.interactable = false;
        mobManager.walkTurn      = false;

        playerAttack.CheckDistance(1);
        playerAttack.CheckDistance(2);
        playerAttack.CheckDistance(3);
        CheckGrid.Instance.ColorGrid(0, 0, 0, true);

        clearColor = true;
        ClearColorHex();

        //_buttonWalk.GetComponentInChildren<ToolTipType>()._descricao = "";

        if (playerAttack.ListTarget != null && mobManager.myTurn
           // && !playerSkill.Skills[0].SilenceSkill 
           // && !playerSkill.Skills[1].SilenceSkill
          //  && !playerSkill.Skills[2].SilenceSkill
            // ||
            //mobAttack.Skill1NeedTarget && mobManager.myTurn ||
            //mobAttack.Skill2NeedTarget && mobManager.myTurn ||
            //mobAttack.Skill3NeedTarget && mobManager.myTurn
            )
        {
            
            ClickAudio();

            mobManager.AttackTurn();

            if (gms.FaseAtual <= 1)
            {
                if (GetComponent<InputManager>().canMoveCamera)
                {
                    if (!gms.IsMobile)
                    {
                        if (_changeMoveCamera.isOn)
                            //_buttonAttack.GetComponentInChildren<ToolTipType>()._descricao = "Movimente a <b>CAMERA</b> usando as setas.";
                            _info.NewInfo(
                                XmlMenuInicial.Instance.Get(177)//"Movimente a <b>CAMERA</b> usando as setas."
                                , 5);
                        else
                            _info.NewInfo(
                                XmlMenuInicial.Instance.Get(176)//"Movimente a <b>CAMERA</b> usando o mouse."
                                , 5);
                        //_buttonAttack.GetComponentInChildren<ToolTipType>()._descricao = "Movimente a <b>CAMERA</b> usando o mouse.";
                    }
                    else
                    {
                            _info.NewInfo(
                                XmlMenuInicial.Instance.Get(175)//"De Zoom na <b>CAMERA</b> fazendo movimento de Pinça."
                                , 5);
                    }
                }
                else
                {
                    if (!gms.IsMobile)
                    {
                        if (_changeMoveCamera.isOn)
                            //_buttonAttack.GetComponentInChildren<ToolTipType>()._descricao = "Aperte <b>'C'</b> para trocar o estilo da Camera e a \n Movimente usando as setas.";
                            _info.NewInfo(
                                XmlMenuInicial.Instance.Get(174)//"Aperte _b;'C'_/b; para trocar o estilo da Camera e a \n Movimente usando as setas."
                                , 5);
                        else
                            _info.NewInfo(
                                XmlMenuInicial.Instance.Get(173)//"Aperte <b>'C'</b> para trocar o estilo da Camera e a \n Movimente usando o mouse."
                                , 7);
                        //_buttonAttack.GetComponentInChildren<ToolTipType>()._descricao = "Aperte <b>'C'</b> para trocar o estilo da Camera e a \n Movimente usando o mouse.";
                    }
                }
            }

           // if(!playerSkill.Skills[0].SilenceSkill)
            if (playerAttack.ListTargetSkill1.Count > 0 || !playerSkill.Skills[0].NeedTarget)
                _buttonSkill1.interactable = true;

            //if (!playerSkill.Skills[1].SilenceSkill)
                if (playerAttack.ListTargetSkill2.Count > 0  || !playerSkill.Skills[1].NeedTarget)
                _buttonSkill2.interactable = true;

            //if (!playerSkill.Skills[2].SilenceSkill)
                if (playerAttack.ListTargetSkill3.Count > 0  || !playerSkill.Skills[2].NeedTarget)
                _buttonSkill3.interactable = true;

            _buttonAttack.interactable = true;

            painelSkill.gameObject.SetActive(true);
        }
        else
        {

            _buttonSkill1.interactable = false;
            _buttonSkill2.interactable = false;
            _buttonSkill3.interactable = false;

            if (mobManager.attackTurn)
            {
                _buttonSkill1.interactable = !playerSkill.Skills[0].NeedTarget;
                _buttonSkill2.interactable = !playerSkill.Skills[1].NeedTarget;
                _buttonSkill3.interactable = !playerSkill.Skills[2].NeedTarget;

                _buttonAttack.interactable = true;
                painelSkill.gameObject.SetActive(true);

                return;
            }

            if (mobManager.myTurn)
            _info.NewInfo(XmlMenuInicial.Instance.Get(172), 4);//Sem alvo
        }

        if (!mobManager.myTurn)
              _info.NewInfo(XmlMenuInicial.Instance.Get(171), 4);//Espere seu turno para poder Atacar.

    }

    public void ButtonBackToWalk(bool audio=false)//si clicar no botao de atack e quer andar clica nesse
    {     
        if (!playerAttack.useSkill)
        {
            if(audio)
            ClickAudio();
            //GetComponent<InputManager>().canMoveCamera = false;

            EffectManager effect = EffectManager.Instance;
            effect.TargetReset();

            _buttonWalk.interactable = true;
            clearColor               = true;
            ClearColorHex();
            mobManager.attackTurn    = false;
            painelSkill.gameObject.SetActive(false);
            mobManager.attackTurn    = false;

            playerAttack.TargetAttack(false);

            _buttonBack.gameObject.SetActive(false);
            clearColor = false;
            ClearColorHex();
            return;
        }       
    }

    #region Painel Game Over
    public void AtivePainelGameOver(int desc = -1, string tex = null, bool bad = true, bool forKill=true)
    {
        if (painelGameOver.activeInHierarchy)
            return;

        GameManagerScenes._gms.Paused = true;

        painelPaused.SetActive(false);
        GetComponent<InputManager>().ChangeCamera();

        GetComponent<InputManager>().enabled = false;
        GetComponent<ToolTip>().enabled      = false;       

        _buttonTryAgain.gameObject.SetActive(bad);

        _buttonNextLevel.gameObject.SetActive(!bad);
      
        if (_gameOver!=null)
        {
            if (bad)
            {
                GameOverAudio();
            }
            else
            {
                WinAudio();               
            }
        }

        for (int i = 0; i < listRespaw.allRespaws.Count; i++)
        {
            listRespaw.allRespaws[i].SetActive(false);
        }

        _buttonEndTurn.gameObject.SetActive(false);
        painelSkill.gameObject.SetActive   (false);
        painelWalk.gameObject.SetActive    (false);

        painelGameOver.SetActive(true);

        imageGameOver.SetActive(bad);
        imageYouWin.SetActive(!bad);

        #region Bad
        if (bad || GameManagerScenes.BattleMode)
        {
            //_textTryAgain.text = "Try Again";

            GetComponent<TotalTime>().GameOver(true);

            if (desc == -1)
                desc = Random.Range(0, 5);

            switch (desc)
            {
                case 0:
                    painelGameOver.GetComponentInChildren<Text>().text = "Mais sorte na próxima.";
                    break;

                case 1:
                    painelGameOver.GetComponentInChildren<Text>().text = "Use suas habilidades com sabedoria.";
                    break;

                case 2:
                    painelGameOver.GetComponentInChildren<Text>().text = "Fique atento nos inimigos próximos.";
                    break;

                case 3:
                    painelGameOver.GetComponentInChildren<Text>().text = "Caro Jogador você Falhou com esse jogo.";
                    break;

                case 4:
                    painelGameOver.GetComponentInChildren<Text>().text = "Vai Chorar!!??";
                    break;
            }
        }
        #endregion

        #region Good
        if(!bad)
        {
            GetComponent<TotalTime>().GameOver(false);

            int    faseAtual = gms.FaseAtual;
            string nameFase  = gms.NameFase(faseAtual + 1);

            GetComponent<InfoTable>().NewInfo(tex,5);

            if (nameFase=="" || gms.Demo && gms.MaxFaseDemo == faseAtual + 1)
            {
                _buttonNextLevel.gameObject.SetActive(false);
                _buttonTryAgain.gameObject.SetActive(true);

                string txt = gms.Demo ? "DEMO\n" : "" ;

                gms.NewInfo(txt+"<b>GAME OVER</b>\nThanks", 5);
            }
            else
                if (!gms.CheckBlockFase(faseAtual + 1))
                gms.NewInfo(
                    GameManagerScenes._gms.AttDescriçãoMult(
                        XmlMenuInicial.Instance.Get(170), nameFase)//"Nova Fase Desbloqueada \n _b;{0}_/b;"
                    , 5);
            /* 
             if (listRespaw.faseAtual == 3)
                 AtivePainelGameOver(desc, tex,false,forKill);
            */

            gms.CompleteFase(faseAtual, forKill);


            if (desc == -1)
                desc = Random.Range(0, 2);

            switch (desc)
            {
                case 0:
                    painelGameOver.GetComponentInChildren<Text>().text = "Bom Trabalho.";
                    break;

                case 1:
                    painelGameOver.GetComponentInChildren<Text>().text = "Parabens";
                    break;
            }
        }
        #endregion


        if (tex != null)
            painelGameOver.GetComponentInChildren<Text>().text = tex;
    }

    public void ButtonTry(bool gameover)
    {
        ClickAudio();

        GameManagerScenes._gms.Paused = false;

        if (gameover)
        {
            Scene scene = SceneManager.GetActiveScene();
            gms.LoadLevel(scene.name);
            return;
        }
        else
        {         
            //Application.LoadLevel(1);

            //RespawMob respaw = GetComponent<RespawMob>();

            int nextFase = gms.FaseAtual + 1;

            if (nextFase >= (gms.FaseCount)) //zerou
            {               
                gms.FaseAtual = (0);

                gms.GetComponentInChildren<MusicLevel>().StartMusic(true, gms.FaseAtual);

                Scene scene = SceneManager.GetActiveScene();
                gms.LoadLevel(scene.name);
                return; 
            }
            else
            {
                gms.FaseAtual = (nextFase);

                gms.GetComponentInChildren<MusicLevel>().StartMusic(true, gms.FaseAtual);

                Scene scene = SceneManager.GetActiveScene();
                gms.LoadLevel(scene.name);
                return;

            }           
        }
               
        //painelGameOver.SetActive(false);
    }

    public void Exit()
    {
        ClickAudio();

        UnityEngine.Events.UnityAction[] _t =
    {
         () => GetComponent<TotalTime>().SalveTotalTime(),
         () => Application.Quit()
    };

        GameManagerScenes._gms.QuestionPainel(/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(38) /*: "Deseja Sair do jogo??"*/,
                            /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(39) /*: "Sair"*/,
                            /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(13) /*: "<color=red>Cancelar</color>"*/,
                            _t, null);
    }
    #endregion

    #region Fade Button Skill
    public void FadeInSkill(int skill)
    {
        clearColor = false;

        int damage = player.GetComponent<SkillManager>().Skills[skill - 1].CurrentDamage * player.GetComponent<SkillManager>().Skills[skill - 1].DividedDamage;

        if (damage < 0)
            damage = 0;

        if (!ShowRageSkill)
        {
            if (GetComponent<ToolTip>().target != player)
                if (!player.GetComponent<MobManager>().MesmoTime(GetComponent<ToolTip>().target))
                        GetComponent<ToolTip>().TargetTooltip(GetComponent<ToolTip>().target, damage);/**/

            if (GetComponent<ToolTip>().target != null)
                if (playerSkill.Skills[skill - 1].TargetFriend || !playerSkill.Skills[skill - 1].NeedTarget)
                    CheckGrid.Instance.ColorGrid(3, 0, 0, true);
                else
                    CheckGrid.Instance.ColorGrid(0, 0, 0, true);

            playerAttack.CheckDistance(skill);

            ShowRageSkill = true;
        }
        /*
        switch (skill)
        {
            case 1:
                if (!ShowRageSkill1)
                {
                    

                    ShowRageSkill1 = true;

                    if (GetComponent<ToolTip>().target != player)
                        if (!player.GetComponent<MobManager>().MesmoTime(GetComponent<ToolTip>().target))
                            if (player.GetComponent<SkillManager>())
                                GetComponent<ToolTip>().TargetTooltip(GetComponent<ToolTip>().target, damage);

                    if (GetComponent<ToolTip>().target != null)
                        if (playerSkill.Skills[skill - 1].TargetFriend || !playerSkill.Skills[skill - 1].NeedTarget)
                            CheckGrid.Instance.ColorGrid(3, 0, 0, true);
                        else
                            CheckGrid.Instance.ColorGrid(0, 0, 0, true);

                    playerAttack.CheckDistance(skill);
                }
                break;

            case 2:
                if (!ShowRageSkill2)
                {
                    if (GetComponent<ToolTip>().target != player)
                        if (!player.GetComponent<MobManager>().MesmoTime(GetComponent<ToolTip>().target))
                            if (player.GetComponent<SkillManager>())
                                GetComponent<ToolTip>().TargetTooltip(GetComponent<ToolTip>().target, damage);

                    ShowRageSkill2 = true;

                    if (mobAttack.Skill2TargetFriend || !mobAttack.Skill2NeedTarget)
                        CheckGrid.Instance.ColorGrid(3, 0, 0, true);
                    else
                        CheckGrid.Instance.ColorGrid(0, 0, 0, clear: true);

                    playerAttack.CheckDistance(skill);
                }
                break;

            case 3:
                if (!ShowRageSkill3)
                {
                    if (GetComponent<ToolTip>().target != player)
                        if (!player.GetComponent<MobManager>().MesmoTime(GetComponent<ToolTip>().target))
                            if (player.GetComponent<SkillManager>())
                                GetComponent<ToolTip>().TargetTooltip(GetComponent<ToolTip>().target, damage);

                    ShowRageSkill3 = true;

                    if (mobAttack.Skill3TargetFriend || !mobAttack.Skill3NeedTarget)
                        CheckGrid.Instance.ColorGrid(3, 0, 0, true);
                    else
                        CheckGrid.Instance.ColorGrid(0, 0, 0, clear: true);

                    playerAttack.CheckDistance(skill);
                }
                break;
        }
        */
    }

    public void FadeOutSkill(int skill)
    {
        if (!ShowRageSkill/*!ShowRageSkill1 && !ShowRageSkill2 && !ShowRageSkill3*/)
           return;

        clearColor = true;

        ShowRageSkill = false;

        Invoke("ClearColorHex", 1);

        if (GetComponent<ToolTip>().target != player)
            if (!player.GetComponent<MobManager>().MesmoTime(GetComponent<ToolTip>().target))
                GetComponent<ToolTip>().TargetTooltip(GetComponent<ToolTip>().target,0,prop:false);
       
        /*
        switch (skill)
        {
            case 1:
                if (ShowRageSkill1)
                {
                    ShowRageSkill1 = false;
                    Invoke("ClearColorHex", 1);
                }
                break;

            case 2:
                if (ShowRageSkill2)
                {
                    ShowRageSkill2 = false;
                    Invoke("ClearColorHex", 1);
                }
                break;

            case 3:
                if (ShowRageSkill3)
                {
                    ShowRageSkill3 = false;
                    Invoke("ClearColorHex",1);
                }
                break;
        }
        */
    }

    void ClearColorHex()
    {
        if (clearColor)
            CheckGrid.Instance.ColorGrid(0, 0, 0, true);
    }
    #endregion

    #region Painel Config
    public void AtivePausedPainel()
    {
       if (gms.IsMobile)
        {
            _changeMoveCamera.gameObject.SetActive(false);

           // _sliderMouseSensibility.GetComponentInChildren<Text>().text = "<b>Touch</b> Sensibility:";
        }

        painelPaused.SetActive(!painelPaused.gameObject.activeSelf);

        if (painelPaused.activeInHierarchy)
        {
            GameManagerScenes._gms.Paused = true;

            //Att com o salve
            if (_changeMoveCamera.gameObject.activeInHierarchy)
            _changeMoveCamera.isOn        = gms.MoveCameraArrow;

            if(_sliderMouseSensibility.gameObject.activeInHierarchy)
            _sliderMouseSensibility.GetComponentInChildren<Slider>().value = gms.MouseSensibility;
        }
        else
            GameManagerScenes._gms.Paused = false;


        return;
    }

    public void AtiveConfigPainel()
    {
        painelConfig.SetActive(!painelConfig.gameObject.activeSelf);

        GameManagerScenes._gms.Paused = painelPaused.gameObject.activeSelf;

        painelPaused.SetActive(false);      
    }

    public void ChangeMoveCamera()
    {
        ClickAudio();

        _moveCameraArrow = !_moveCameraArrow;

        if (gms != null)
            gms.MoveCameraArrow = _moveCameraArrow;

        GetComponent<InputManager>().cameraOrbit.HowMoveCamera(_moveCameraArrow);

        _changeMoveCamera.isOn = _moveCameraArrow;

    }

    public void ButtonMenu()
    {
        ClickAudio();

        UnityEngine.Events.UnityAction[] _t =
   {
         () => GetComponent<TotalTime>().SalveTotalTime(),
         () => GameManagerScenes._gms.Paused = false,
         () => gms.GetComponentInChildren<MusicLevel>().StartMusic(true,_audioMenu),
         () => gms.LoadLevel(0)
    };

        GameManagerScenes._gms.QuestionPainel(
            /*!gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(49)/* : "Deseja Voltar ao menu??"*/,
            /*!gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(50) /*: "Yep"*/,
            /*!gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(13) /*: "Nop"*/, 
            _t, null);               
    }

    public void SensibilityMouse()
    {
        float value = _sliderMouseSensibility.GetComponentInChildren<Slider>().value;

        GetComponent<InputManager>().AttSensibilityMouse(value);

        if (gms!=null)
        gms.MouseSensibility = value;
    }

    public void EffecSound()
    {
        float value = _sliderSoundEffect.value;
       
        gms.SoundEffect = (value);
    }

    public void MusicSound()
    {
        float value = _sliderSoundMusic.value;

        gms.SoundMusic = (value);
    }
    #endregion

}
