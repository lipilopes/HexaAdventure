using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[System.Serializable]
public class RoundBattleMode
{
    public MobManager.MobTime _time;

    public GameObject _mob;

    public int _x;

    public int _y;
}

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DelegateTurnEnd     = null;
        DelegateTurnCurrent = null;
    }

    public int totalUpdateTurn=-1,
               totalZeroTurn   =0;

    public List<GameObject> turn = new List<GameObject>();
    public int isTurn       = -1,
               tamanhoLista = 1;

    bool gameOver = false;

    [Space]
    [Header("Battle Mode")]
    public List<RoundBattleMode> roundTurn = new List<RoundBattleMode>();
    [Space]
    [Header("HUD")]
    [SerializeField] Fade _hudTurn;
    [SerializeField] UnityEngine.UI.Text  _hudTurnName;
    [SerializeField] UnityEngine.UI.Image _hudTurnIcon;

    ButtonManager buttonManager;
    RespawRecHp     respawitem;
    EffectManager  effect;

    string faseBattleMode;
    MobManager.MobTime _timeBattleMode = MobManager.MobTime.None;

    public delegate void  TurnEndDelegate();
    public static   event TurnEndDelegate DelegateTurnEnd;

    public delegate void TurnCurrentDelegate();
    public static  event TurnCurrentDelegate DelegateTurnCurrent;

    void Reset()
    {
        turn            = new List<GameObject>(0);
        tamanhoLista    = 0;
        isTurn          = -1;
        totalUpdateTurn = -1;
        totalZeroTurn   = 0;
    }

    void Start()
    {
        buttonManager = this.GetComponent<ButtonManager>();
        respawitem    = (RespawRecHp)FindObjectOfType(typeof(RespawRecHp));

        faseBattleMode = XmlMenuInicial.Instance.Get(209);//"Agora é a vez do {0}!!!"

        if (GameManagerScenes.BattleMode && GameManagerScenes._gms.BattleModeOptionRoundActive)
            DelegateTurnCurrent += CheckBattleModeRound;
    }

    void Update()
    {
        if (isTurn > turn.Count && turn.Count != 0)
        {
            isTurn = 0;
            Turn();
        }
    }

    public void GameOver(int desc=-1,string text=null,bool lose=true,bool forKill=true)
    {
        gameOver = true;

        if (turn.Count > 0)
            Clear();

        if (!lose)
            GameManagerScenes._gms.AchievementFase();

        print("Game Over");

        if (!lose)
        {
            if (HudHistory.Instance != null && !GameManagerScenes._gms.CheckBlockFase(GameManagerScenes._gms.FaseAtual))
                HudHistory.Instance.StartChat(false);
        }   

        StartCoroutine(IGameOver(desc, text, lose, forKill));
    }

    IEnumerator IGameOver(int desc = -1, string text = null, bool lose = true, bool forKill = true)
    {
        buttonManager.ClearHUD();

        while(HudHistory.Instance.HistoryPass == true)
        yield return null;

        buttonManager.AtivePainelGameOver(desc, text, lose, forKill);
    }

    public void RegisterTurn(GameObject obj,bool att=true)
    {
        if (GameManagerScenes.BattleMode && GameManagerScenes._gms.BattleModeOptionRoundActive)
        {
            RoundBattleMode R = new RoundBattleMode();

            R._time = obj.GetComponent<MobManager>().TimeMob;

            R._mob = obj;

            roundTurn.Add(R);

            CheckBattleModeRound();
        }
        else//History or  Normal battle Mode
        {
            turn.Add(obj);

            if (att)
                AttList();
        }
    }

    public void FirstTurn(int first)
    {
        if (first == -1 || GameManagerScenes.BattleMode)
            first = UnityEngine.Random.Range(0, turn.Count);

        if (effect == null)
            effect = EffectManager.Instance;

        if (turn[first] != null)
            isTurn = first;
        else
            isTurn = 0;

        print("Turno iniciado com o "+ turn[first].name);

        ShowEnemy();

        Turn();       
    }

    public void Turn()
    {
        AttList();
        GetComponent<CheckGrid>().ColorGrid(0, 0, 0, true);
        AttToolTip();

        #region Dbuff's
        for (int i = 0; i < turn.Count; i++)
        {
            if (turn[i].GetComponent<MobDbuff>() != null)
            {
                if (turn[i].GetComponent<MobManager>().bleed)
                {
                    //iTween.LookTo(Camera.main.gameObject, turn[i].transform.position,0.5f);
                    turn[i].GetComponent<MobDbuff>().DamageDbuffBleed();
                }
                                     

                if (turn[i].GetComponent<MobManager>().fire)
                {
                    //iTween.LookTo(Camera.main.gameObject, turn[i].transform.position, 0.5f);
                    turn[i].GetComponent<MobDbuff>().DamageDbuffFire();
                }

                if (turn[i].GetComponent<MobManager>().petrify)
                {
                    //iTween.LookTo(Camera.main.gameObject, turn[i].transform.position, 0.5f);
                    turn[i].GetComponent<MobDbuff>().DamageDbuffPetrify();
                }
                //turn[i].GetComponent<MobDbuff>().CooldownDbuffPoison();
                //turn[i].GetComponent<MobDbuff>().CooldownDbuffBleed();
                //turn[i].GetComponent<MobDbuff>().CooldownDbuffFire();
            }
        }
        #endregion

        totalUpdateTurn++;

        if (isTurn >= 0 && isTurn < turn.Count && turn[isTurn].activeInHierarchy)
        {
            #region Battle Mode
            if (GameManagerScenes.BattleMode)
            {
                if (turn[isTurn].GetComponent<MobManager>().isPlayer)
                {
                    MobManager.MobTime timeCurrentMob = turn[isTurn].GetComponent<MobManager>().TimeMob;

                    if (_timeBattleMode != timeCurrentMob)
                    {
                        //Aviso
                        History H = new History();

                        H._mobID = GameManagerScenes._gms.HeroID(turn[isTurn]);
                        H._left = _timeBattleMode == MobManager.MobTime.White;
                        H._mobName = GameManagerScenes._gms.HeroName(turn[isTurn]);
                        H._chat = GameManagerScenes._gms.AttDescriçãoMult(
                        ("<color=" + (H._left ? "green" : "red") + ">" + faseBattleMode + "</color>"),
                        "<b>Player " + (H._left ? "1" : "2") + "</b>");

                        _timeBattleMode = turn[isTurn].GetComponent<MobManager>().TimeMob;
                    }
                }
                else
                    if (buttonManager.player != null)
                {
                    if(!buttonManager.player.GetComponent<MobManager>().isPlayer)
                    {
                        PlayerGUI.Instance.GetPlayer(turn[isTurn]);
                    }
                }
                else
                    if (buttonManager.player == null)
                {
                    if (!turn[isTurn].GetComponent<MobManager>().isPlayer)
                    {
                        PlayerGUI.Instance.GetPlayer(turn[isTurn]);
                    }
                }
            }
            #endregion

            MobManager move = turn[isTurn].GetComponent<MobManager>();

            if (move && move.Alive)
            {
                Debug.LogError("Turn(" + isTurn + ") - " + move.name);

                effect.TurnEffect(move.gameObject);
                HudTurn(move);

                move.MyTurn();

                if (move.gameObject == buttonManager.player)
                {
                    Debug.LogError("Turn(" + isTurn + ") Player - " + move.name);

                    buttonManager.PlayerTurn();
                    ShowEnemy();
                }

                return;
            }
            else
            {
                Clear(turn[isTurn]);
                return;
            }
        }      

        EndTurn();
    }

    public void EndTurn()
    {
        GetComponent<CheckGrid>().ColorGrid(0, 0, 0, true);
        AttList();

        int checkTime = CheckTime();

        //Debug.LogError("Check Time Retun: "+checkTime);

        if (checkTime == 0)
        {
            ShowEnemy();

            if (isTurn >= turn.Count - 1 || isTurn >= 0 && isTurn < turn.Count && turn[isTurn] == null)
            {
                if (DelegateTurnEnd != null)
                    DelegateTurnEnd();

                totalZeroTurn++;
                isTurn = 0;
                Turn();

                respawitem.CheckItem();

                AttList();
                print("isTurn chegou no max e resetou-se " + isTurn + "/" + turn.Count);

                ClearKill();
                return;
            }
            else
            {
                if (DelegateTurnCurrent != null)
                    DelegateTurnCurrent();

                isTurn++;
                Turn();
                return;
            }
        }
        else
            if (checkTime == 2 && !gameOver)
        {
            gameOver = true;

            if (GameManagerScenes.BattleMode)//Player 1 Win
            {
                GameOver(4,"Jogador 1 Venceu" /*XmlMenuInicial.Instance.Get(167)*/, true, true);
            }
            else
            GameOver(0, XmlMenuInicial.Instance.Get(167), false, true);//_b;Parabéns_/b; Você foi o ultimo Sobrevivente.
        }
        else
        if (checkTime == 1 && !gameOver)
        {
            gameOver = true;

            if (GameManagerScenes.BattleMode)//Player 2 Win
            {
                GameOver(4, "Jogador 2 Venceu" /*XmlMenuInicial.Instance.Get(167)*/, true, true);
            }
            else
                GameOver();
        }
    }

    void HudTurn(MobManager mob)
    {
        _hudTurnName.text = 
            "<color=" + (mob.MesmoTime(RespawMob.Instance.PlayerTime) ? "blue" : "red") + ">" +
            mob .GetComponent<ToolTipType>()._name+ "</color>";

        _hudTurnIcon.sprite = mob.GetComponent<ToolTipType>()._spritePerfil;

        if (_hudTurnIcon.sprite == null)
            _hudTurnIcon.sprite = GameManagerScenes._gms.SpritePerfil(mob.GetComponent<ToolTipType>()._name);

        _hudTurn.delay = 0f;
        _hudTurn.FadeOn();

        _hudTurn.delay = 0.5f;
        _hudTurn.FadeOff();
    }

    void AttList()//Atualiza o tamanho da lista
    {
        tamanhoLista = turn.Count;
        //print("Size list "+turn.Count);
    }

    void ClearKill()
    {
        for (int i = 0; i < turn.Count; i++)
        {
            if (turn[i].GetComponent<MobManager>())
            {
                turn[i].GetComponent<MobManager>().CountKill=0;
            }
        }
    }

    void AttToolTip()
    {
        for (int i = 0; i < turn.Count; i++)
        {
            if (turn[i].GetComponent<ToolTipType>())
            {
                turn[i].GetComponent<ToolTipType>().AttToltip();
            }
        }
    }

    void ShowEnemy()
    {
        if (effect==null)
        {
            effect = EffectManager.Instance;
        }

        if (isTurn >= 0)
            foreach (var item in turn)
            {
                if (item != null && turn[isTurn]!=null)
                    if (item != turn[isTurn])
                    {
                        if (item.GetComponent<MobManager>() != null)

                            if (!item.GetComponent<MobManager>().MesmoTime(turn[isTurn]))
                            {
                                if (item.GetComponent<MobManager>().Alive)
                                effect.ShowEnemyEffect(item);                 
                            }
                    }
            }
    }

    public void Clear(GameObject This=null)
    {
        if (This == null)
        {
            turn.Clear(); //limpa a lista
        }
        else
        if (turn.Contains(This))
        {
            if (turn[isTurn] == This)
            {
                turn.Remove(This); //apaga obj da lista

                EndTurn();
            }

            GetComponent<RespawMob>().allRespaws.Remove(This);

            print(This.name + " foi removido da lista");
        }
        else
            return;

        CheckBattleModeRound();

        AttList();

        if (turn.Count == 1 && totalUpdateTurn>=1)
            EndTurn();
    }

    void CheckBattleModeRound()
    {
        if (GameManagerScenes.BattleMode && GameManagerScenes._gms.BattleModeOptionRoundActive)
        {
            List<MobManager.MobTime> _t = new List<MobManager.MobTime>();

            foreach (var i in roundTurn)
            {
                if (i._mob != null && i._mob.GetComponent<MobManager>().Alive)
                {
                    if (_t.Contains(i._time))
                    {
                        turn.Remove(i._mob);

                       i._x = i._mob.GetComponent<MoveController>().hexagonX;
                       i._y = i._mob.GetComponent<MoveController>().hexagonY;

                        GridMap gridMap = GridMap.Instance;
                        gridMap.FreeHex(i._mob);
                        //if (gridMap != null)
                        //    for (int j = 0; j < gridMap.hex.Count; j++)
                        //    {
                        //        if (gridMap.hexManager[j].currentMob == i._mob)
                        //        {
                        //            gridMap.hexManager[j].currentMob = null;
                        //            gridMap.hexManager[j].free       = true;
                        //        }
                        //    }

                        i._mob.SetActive(false);
                    }
                    else
                    {
                        i._mob.SetActive(true);

                        i._mob.GetComponent<MoveController>().Walk(null,i._x,i._y,Dbuff: true);

                        turn.Add(i._mob);
                        _t.Add(i._time);
                    }
                }
            }

            AttList();

            if (CheckTime() != 0)
                EndTurn();
        }
    }

    public GameObject CurrentTurn() { return turn[isTurn]; }

    int CheckTime()
    {
        if (gameOver == false)
        {
            int playerTime = 0,
                enimyTime = 0;
            MobManager.MobTime pTime = (GameManagerScenes.BattleMode ? MobManager.MobTime.White : RespawMob.Instance.PlayerTime);

            #region Round Battle Mode
            if (GameManagerScenes.BattleMode && GameManagerScenes._gms.BattleModeOptionRoundActive)
            {
                foreach (var i in roundTurn)
                {
                    if (i._mob != null && i._mob.GetComponent<MobManager>().Alive)
                    {
                        if (i._time == (pTime))
                            playerTime++;
                        else
                            enimyTime++;

                        print(i._mob + " é do time " + i._time);
                    }
                }
            }
            #endregion
            #region History Game or Normal Battle Mode
            else
            {
                for (int i = 0; i < turn.Count; i++)
                {
                    if (turn[i].GetComponent<MobManager>().Alive)
                    {
                        if (turn[i].GetComponent<MobManager>().MesmoTime(pTime))
                            playerTime++;
                        else
                            enimyTime++;

                        print(turn[i].name + " é do time " + turn[i].GetComponent<MobManager>().TimeMob);
                    }
                }
            }
            #endregion

            Debug.LogError("Time Player["+pTime+"]: " + playerTime + " TimeEnemy[Black]: " + enimyTime);

            if (enimyTime > 0 && playerTime <= 0 || playerTime == 0 && enimyTime == 0)
                return 1; //Player or White (P1) Lose
            else
            if (playerTime > 0 && enimyTime == 0)
                return 2; //Player or White (P1) win
            else
                return 0; //In Game
        }

        return -1;
    }

    public GameObject GetRandomMob(MobManager.MobTime Time,bool otherTime = true)
    {
        List<GameObject> mob = new List<GameObject>();

        for (int i = 0; i < turn.Count; i++)
        {
            if (!turn[i].GetComponent<MobManager>().MesmoTime(Time) && otherTime || //Não e do mesmo time && pega do outro time
                 turn[i].GetComponent<MobManager>().MesmoTime(Time) && !otherTime)   //Mesmo time && nao pega do outro time
                mob.Add(turn[i]);
        }

        return mob.Count>=1 ? mob[UnityEngine.Random.Range(0,mob.Count)] : null;
    }
    public List<GameObject> GetMob(MobManager.MobTime Time,bool otherTime = true, GameObject me = null, bool getMe=false)
    {
        List<GameObject> mob = new List<GameObject>();

        for (int i = 0; i < turn.Count; i++)
        {
            if (!getMe && turn[i] != me || getMe && turn[i] == me)
            {
                if (turn[i].GetComponent<MobManager>().MesmoTime(Time) && !otherTime)//Mesmo time && nao pega do outro time
                    mob.Add(turn[i]);

                if (!turn[i].GetComponent<MobManager>().MesmoTime(Time) && otherTime)//Não e do mesmo time && pega do outro time
                    mob.Add(turn[i]);
            }
        }

        return mob;
    }
    public List<GameObject> GetMob(MobManager.MobTime Time,bool otherTime = true)
    {
        List<GameObject> mob = new List<GameObject>();

        for (int i = 0; i < turn.Count; i++)
        {
                if (!turn[i].GetComponent<MobManager>().MesmoTime(Time) &&   otherTime || //Não e do mesmo time && pega do outro time
                     turn[i].GetComponent<MobManager>().MesmoTime(Time) &&  !otherTime)   //Mesmo time && nao pega do outro time
                mob.Add(turn[i]);
        }

        return mob;
    }
}
