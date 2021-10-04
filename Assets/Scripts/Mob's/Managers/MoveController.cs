using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour 
{
    //MoveController _targetFollow;
    CheckGrid      checkGrid;
    MobManager     mobManager;
    EnemyAttack    enemyAttack;

    [Header("Check Manager")]
    [SerializeField] bool fuga=false;
    [SerializeField,Tooltip("Chamar inimigos so funcionam com  os q tem maxWalktime>0")] bool call=true;

    //public bool isPlayer { get { return mobManager != null ? mobManager.isPlayer : GetComponent<MobManager>().isPlayer; } }   

    /// <summary>
    /// Quantidade de vezes q pode andar
    /// </summary>
    [HideInInspector] public int  time;

    [Header("Casa Atual")]
    public int hexagonX;
    public int hexagonY;
    protected HexManager solo;
    public    HexManager Solo
    {
        get
        {
            if (solo == null)
            {
                GameObject obj = GameObject.Find("Hex" + hexagonX + "x" + hexagonY);
                if (obj!=null && obj.GetComponent<HexManager>())
                {
                    solo = obj.GetComponent<HexManager>();
                    return solo;
                }

                return null;
            }
            else
                return solo;
        }
    }

    protected bool startCoroutine = false;

    GameManagerScenes _gms;

    WaitForSeconds wait = new WaitForSeconds(0.5f);

    protected List<HexManager> colorGround = new List<HexManager>();
[Space]
    [SerializeField, Tooltip("Tempo de espera ao chegar no ground")]
    protected float _delayEffect = 0.1f;
    [SerializeField, Tooltip("Tempo que leva para chegar ao outro ground")]
    protected float _timeEffect = 0.2f;
    [SerializeField]
    protected iTween.EaseType easeTypeGoToEffect = iTween.EaseType.linear;

    void Start()
    {
        mobManager    = GetComponent<MobManager>();
        checkGrid     = GameObject.FindObjectOfType<CheckGrid>();      
        enemyAttack   = GetComponent<EnemyAttack>();
        _gms          = GameManagerScenes._gms;
    }

    public void StartWalkTurn(bool start = true)
    {
        if (start)
        {
            if (!startCoroutine)
                StartCoroutine(WalkCoroutine());
        }
        else
        {
            if (startCoroutine)
                StopCoroutine(WalkCoroutine());
        }
    }
    IEnumerator WalkCoroutine()
    {
        startCoroutine = true;

        while (mobManager.walkTurn && time>0)
        {
            /*if (transform.position != GameObject.Find("Hex" + (hexagonX) + "x" + hexagonY).transform.position)
transform.position  = GameObject.Find("Hex" + (hexagonX) + "x" + hexagonY).transform.position;*/

            //Player
            if (mobManager.isPlayer)
            {
                yield return null;

                if (time <= 0)
                {
                    checkGrid.ColorGrid(x:0,clear: true);

                    if (enemyAttack.useSkill && mobManager.attackTurn)
                    {
                        DesColorGround();
                        mobManager.EndTurn();
                        break;
                    }
                }
                else
                    PlayerRaycast();               
            }
            else
            if (!mobManager.isPlayer && mobManager != null)
            {
                yield return wait;

                if (mobManager.myTurn && time >= 1)
                    EnemyWalk();

                if (mobManager.myTurn && mobManager.walkTurn && time <= 0)
                {
                    DesColorGround();
                    mobManager.EndWalkTurn();
                    break;
                }        
            }
        }

        startCoroutine = false;
        DesColorGround();
    }

    public void FugaActive(bool Fuga)
    {
        if (Fuga != fuga && GameManagerScenes._gms.Adm)
            EffectManager.Instance.PopUpDamageEffect(Fuga ? "Fuga On" : "Fuga Off", gameObject);

        fuga = Fuga;
    }

    public void PlayerRaycast()
    {
        if (GameManagerScenes._gms.Paused == true)
            return;

        Ray ray;

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        else
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        else
            return;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.transform.gameObject;

            if (Input.GetMouseButtonDown(0) && mobManager.walkTurn)
            {
                if (time > 0)
                {
                    if(_gms.Adm)
                    EffectManager.Instance.RespawEffect(hitObject, 0.5f);

                    transform.LookAt(hitObject.transform);

                    if (hitObject.GetComponentInChildren<HexManager>() != null)
                        CheckHome(hitObject, hitObject.GetComponentInChildren<HexManager>().x, hitObject.GetComponentInChildren<HexManager>().y);

                    if (hitObject.GetComponent<MoveController>() !=null)
                    {
                        GameObject grid = checkGrid.HexGround(hitObject.GetComponent<MoveController>().hexagonX,hitObject.GetComponent<MoveController>().hexagonY).gameObject;// GameObject.Find("Hex" + (hitObject.GetComponent<MoveController>().hexagonX) + "x" + (hitObject.GetComponent<MoveController>().hexagonY));
                        CheckHome(grid, grid.GetComponent<HexManager>().x, grid.GetComponent<HexManager>().y);
                    }

                    if (hitObject.GetComponent<ItemRecHp>() != null)
                        if (hitObject.GetComponent<ItemRecHp>().Here != null)
                                    CheckHome(hitObject.GetComponent<ItemRecHp>().Here.GetComponent<HexManager>().gameObject, hitObject.GetComponent<ItemRecHp>().Here.GetComponent<HexManager>().x, hitObject.GetComponent<ItemRecHp>().Here.GetComponent<HexManager>().y);
                }
            }
            else if (time == 0)
            {
                checkGrid.ColorGrid(0, 0, 0, clear: true);
                mobManager.walkTurn = false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="Dbuff"></param>
    /// <param name="foge"></param>
    /// <param name="Call">Mob e puxado ou chamado [SOMENTE DBUFF]</param>
    public void EnemyWalk(MoveController target = null,bool Dbuff=false,bool foge=false, bool Call = false)
    {
        if (target == null)
            target = ChangeTargetFollow().GetComponent<MoveController>();

        if (time <= 0 && !Dbuff || target==null)
            return;

        if (call)
        {
            if (Dbuff && Call && mobManager != null && mobManager.maxTimeWalk <= 0)
                return;
        }

        if (target.gameObject != gameObject)
            transform.LookAt(target.transform);

        if (!fuga && !Dbuff || !foge && Dbuff)
        {
            Debug.LogError(gameObject.name+" esta indo pra cima do "+target.name);
            #region Vai pra cima
            if (hexagonY < target.hexagonY)//Player em baixo
            {
                #region 
                if (hexagonY % 2 == 1)//Impar
                {
                    if (checkGrid.CheckWalk(hexagonX, hexagonY + 1, false))
                    {                       
                        HexManager grid = checkGrid.HexGround((hexagonX), hexagonY + 1);
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                   if (checkGrid.CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX+1), hexagonY + 1);
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                else //Par
                {
                    if (checkGrid.CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX-1), (hexagonY + 1));
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                    if (checkGrid.CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }

                }
                #endregion
            }
            else if (hexagonY > target.hexagonY)//Player cima
            {
                #region
                if (hexagonY % 2 == 1)//Impar
                {
                    if (checkGrid.CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                  if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                else
                {
                    if (checkGrid.CheckWalk(hexagonX - 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                    if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                #endregion
            }
            else if (hexagonY == target.hexagonY)
            {
                #region
                if (hexagonX < target.hexagonX)//Player esta la ->
                {
                    if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                else
                {
                    if (checkGrid.CheckWalk(hexagonX - 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                        CheckHome(grid.gameObject, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                #endregion
            }
            else if (hexagonX < target.hexagonX)//Player esta la ->
            {
                #region
                if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                    CheckHome(grid.gameObject, grid.x, grid.y);
                    return;
                }
                else
                {
                    time--;
                    return;
                }
                #endregion
            }
            else if (hexagonX > target.hexagonX)//Player esta la <-
            {
                #region
                if (checkGrid.CheckWalk(hexagonX - 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                    CheckHome(grid.gameObject, grid.x, grid.y);
                    return;
                }
                else
                {
                    time--;
                    return;
                }
                #endregion
            }
            else if (hexagonX == target.hexagonX)//Player esta la <-
            {
                #region
                if (hexagonY > target.hexagonY)//_targetFollow esta em cima
                {
                    #region
                    if (hexagonY % 2 == 1)//Impar
                    {
                        if (checkGrid.CheckWalk(hexagonX, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                            CheckHome(grid.gameObject, grid.x, grid.y);
                            return;
                        }
                        else
                      if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX+ 1), (hexagonY));
                            CheckHome(grid.gameObject, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            time--;
                            return;
                        }
                    }
                    else
                    {
                        if (checkGrid.CheckWalk(hexagonX - 1, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                            CheckHome(grid.gameObject, grid.x, grid.y);
                            return;
                        }
                        else
                        if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid.gameObject, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            time--;
                            return;
                        }
                    }
                    #endregion
                }
                else //Player esta em baixo
                {
                    #region 
                if (hexagonY % 2 == 1)//Impar
                {
                    if (checkGrid.CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY+1));
                            CheckHome(grid.gameObject, grid.x, grid.y);
                            return;
                    }
                    else
                   if (checkGrid.CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                            CheckHome(grid.gameObject, grid.x, grid.y);
                            return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                else //Par
                {
                    if (checkGrid.CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                            CheckHome(grid.gameObject, grid.x, grid.y);
                            return;
                    }
                    else
                    if (checkGrid.CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid.gameObject, grid.x, grid.y);
                            return;
                    }
                    else
                    {
                        time--;
                        return;
                    }

                }
                #endregion
                }
                #endregion
            }
            else
            {
                Debug.LogError(gameObject.name + " falhou em ir pra cima do " + target.name);
                time--;
                return;
            }
#endregion
        }
        if(fuga && !Dbuff || foge && Dbuff)
        {
            Debug.LogError(gameObject.name + " esta correndo do " + target.name);
            #region Fuga
            if (hexagonY > target.hexagonY)//Player em cima
            {
                #region 
                if (hexagonY % 2 == 1)//Impar
                {
                    if (checkGrid.CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX) + "x" + (hexagonY + 1));
                        CheckHome(grid, hexagonX, hexagonY + 1);
                        return;
                    }
                    else
                   if (checkGrid.CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX + 1) + "x" + (hexagonY + 1));
                        CheckHome(grid, hexagonX + 1, hexagonY + 1);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                else //Par
                {
                    if (checkGrid.CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX - 1) + "x" + (hexagonY + 1));
                        CheckHome(grid, hexagonX - 1, hexagonY + 1);
                        return;
                    }
                    else
                    if (checkGrid.CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX) + "x" + (hexagonY + 1));
                        CheckHome(grid, hexagonX, hexagonY + 1);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }

                }
                #endregion
            }
            else if (hexagonY < target.hexagonY)//Player baixo
            {
                #region
                if (hexagonY % 2 == 1)//Impar
                {
                    if (checkGrid.CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX) + "x" + (hexagonY - 1));
                        CheckHome(grid, hexagonX, hexagonY - 1);
                        return;
                    }
                    else
                  if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX + 1) + "x" + (hexagonY));
                        CheckHome(grid, hexagonX + 1, hexagonY);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                else
                {
                    if (checkGrid.CheckWalk(hexagonX - 1, hexagonY - 1, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX - 1) + "x" + (hexagonY - 1));
                        CheckHome(grid, hexagonX - 1, hexagonY - 1);
                        return;
                    }
                    else
                    if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX + 1) + "x" + (hexagonY));
                        CheckHome(grid, hexagonX + 1, hexagonY);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                #endregion
            }
            else if (hexagonY == target.hexagonY)
            {
                #region
                if (hexagonY > target.hexagonX)//Player esta la <-
                {
                    if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX + 1) + "x" + (hexagonY));
                        CheckHome(grid, hexagonX + 1, hexagonY);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                else
                {
                    if (checkGrid.CheckWalk(hexagonX - 1, hexagonY, false))
                    {
                        GameObject grid = GameObject.Find("Hex" + (hexagonX - 1) + "x" + (hexagonY));
                        CheckHome(grid, hexagonX - 1, hexagonY);
                        return;
                    }
                    else
                    {
                        time--;
                        return;
                    }
                }
                #endregion
            }
            else if (hexagonX > target.hexagonX)//Player esta la <-
            {
                #region
                if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                {
                    GameObject grid = GameObject.Find("Hex" + (hexagonX + 1) + "x" + (hexagonY));
                    CheckHome(grid, hexagonX + 1, hexagonY);
                    return;
                }
                else
                {
                    time--;
                    return;
                }
                #endregion
            }
            else if (hexagonX < target.hexagonX)//Player esta la ->
            {
                #region
                if (checkGrid.CheckWalk(hexagonX - 1, hexagonY, false))
                {
                    GameObject grid = GameObject.Find("Hex" + (hexagonX - 1) + "x" + (hexagonY));
                    CheckHome(grid, hexagonX - 1, hexagonY);
                    return;
                }
                else
                {
                    time--;
                    return;
                }
                #endregion

            }
            else if (hexagonX == target.hexagonX)//Player esta la <-
            {
                #region
                if (hexagonY < target.hexagonY)//_targetFollow esta em baixo
                {
                    #region
                    if (hexagonY % 2 == 1)//Impar
                    {
                        if (checkGrid.CheckWalk(hexagonX, hexagonY - 1, false))
                        {
                            GameObject grid = GameObject.Find("Hex" + (hexagonX) + "x" + (hexagonY - 1));
                            CheckHome(grid, hexagonX, hexagonY - 1);
                            return;
                        }
                        else
                      if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            GameObject grid = GameObject.Find("Hex" + (hexagonX + 1) + "x" + (hexagonY));
                            CheckHome(grid, hexagonX + 1, hexagonY);
                            return;
                        }
                        else
                        {
                            time--;
                            return;
                        }
                    }
                    else
                    {
                        if (checkGrid.CheckWalk(hexagonX - 1, hexagonY - 1, false))
                        {
                            GameObject grid = GameObject.Find("Hex" + (hexagonX - 1) + "x" + (hexagonY - 1));
                            CheckHome(grid, hexagonX - 1, hexagonY - 1);
                            return;
                        }
                        else
                        if (checkGrid.CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            GameObject grid = GameObject.Find("Hex" + (hexagonX + 1) + "x" + (hexagonY));
                            CheckHome(grid, hexagonX + 1, hexagonY);
                            return;
                        }
                        else
                        {
                            time--;
                            return;
                        }
                    }
                    #endregion
                }
                else //Player esta em cima
                {
                    #region 
                    if (hexagonX % 2 == 1)//Impar
                    {
                        if (checkGrid.CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            GameObject grid = GameObject.Find("Hex" + (hexagonX) + "x" + (hexagonY + 1));
                            CheckHome(grid, hexagonX, hexagonY + 1);
                            return;
                        }
                        else
                       if (checkGrid.CheckWalk(hexagonX + 1, hexagonY + 1, false))
                        {
                            GameObject grid = GameObject.Find("Hex" + (hexagonX + 1) + "x" + (hexagonY + 1));
                            CheckHome(grid, hexagonX + 1, hexagonY + 1);
                            return;
                        }
                        else
                        {
                            time--;
                            return;
                        }
                    }
                    else //Par
                    {
                        if (checkGrid.CheckWalk(hexagonX - 1, hexagonY + 1, false))
                        {
                            GameObject grid = GameObject.Find("Hex" + (hexagonX - 1) + "x" + (hexagonY + 1));
                            CheckHome(grid, hexagonX - 1, hexagonY + 1);
                            return;
                        }
                        else
                        if (checkGrid.CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            GameObject grid = GameObject.Find("Hex" + (hexagonX) + "x" + (hexagonY + 1));
                            CheckHome(grid, hexagonX, hexagonY + 1);
                            return;
                        }
                        else
                        {
                            time--;
                            return;
                        }

                    }
                    #endregion
                }
                #endregion
            }
            else if (!Dbuff)
            {
                Debug.LogError(gameObject.name + " falhou em seguir o " + target.name);
                time--;
                return;
            }
            #endregion
        }
    }

    /// <summary>
    /// checa si andou e caso tenho o dbuff poison leva dano
    /// </summary>
    void WalkedDbuff()
    {
        if (ButtonManager.Instance.player == gameObject)
                _gms.TotalWalkers = (1);

        if (time<=0)
        checkGrid.ColorGrid(0, 0, 0, clear: true);

        if (mobManager.poison)
            this.GetComponent<MobDbuff>().DamageDbuffPoison();

        if (mobManager.bleed)
            this.GetComponent<MobDbuff>().DamageDbuffBleed();

        if (Solo.currentItem == null)
            return;

        GameObject item = Solo.currentItem;

        if (item.GetComponent<MobSkillAreaDamage>() != null)
            item.GetComponent<MobSkillAreaDamage>().WalkIn(gameObject);
    }

    public void CheckHome(GameObject grid, int x, int y, int homes = 1)
    {
        if (mobManager.walkTurn)
        {
            if (time > 0)
                EffectManager.Instance.PopUpDamageEffect(((/*mobManager.maxTimeWalk -*/ time)) + "/" + mobManager.maxTimeWalk, Solo.gameObject);
            else
            {
                DesColorGround();
                EffectManager.Instance.PopUpDamageEffect("Acabou", Solo.gameObject);
            }

            if (time <= 0)
                return;
        }

        checkGrid.CheckWalk(x, y);
        colorGround.Add(checkGrid.HexGround(x, y));

        if (grid != null)
            Walk(grid, x, y, homes);

        #region Colore apenas as casas em volta do player

        HexManager _grid = null;
        #region Check Horizontal <->
        if (_grid = checkGrid.HexGround((hexagonX + homes), hexagonY))
            if (_grid != null)
            {
                checkGrid.CheckWalk(_grid.x, _grid.y);
                colorGround.Add(_grid);
            }
        if (_grid = checkGrid.HexGround((hexagonX - homes), hexagonY))
            if (_grid != null)
            {
                checkGrid.CheckWalk(_grid.x, _grid.y);
                colorGround.Add(_grid);
            }
        #endregion

        #region Check  Vertical Left 
        if (hexagonY % 2 == 1)// Caso impar
        {
            if (_grid = checkGrid.HexGround((hexagonX), (hexagonY + homes)))
                if (_grid != null)
                {
                    checkGrid.CheckWalk(_grid.x, _grid.y);
                    colorGround.Add(_grid);
                }
            if (_grid = checkGrid.HexGround((hexagonX), (hexagonY - homes)))
                if (_grid != null)
                {
                    checkGrid.CheckWalk(_grid.x, _grid.y);
                    colorGround.Add(_grid);
                }
        }
        if (hexagonY % 2 == 0)// Caso casa == 0
        {
            if (_grid = checkGrid.HexGround((hexagonX - homes), (hexagonY + homes)))
                if (_grid != null)
                {
                    checkGrid.CheckWalk(_grid.x, _grid.y);
                    colorGround.Add(_grid);
                }
            if (_grid = checkGrid.HexGround((hexagonX - homes), (hexagonY - homes)))
                if (_grid != null)
                {
                    checkGrid.CheckWalk(_grid.x, _grid.y);
                    colorGround.Add(_grid);
                }
        }
        #endregion

        #region check Vertical Right ->
        if (hexagonY % 2 == 1)// Caso impar
        {
            if (_grid = checkGrid.HexGround((hexagonX + homes), (hexagonY + homes)))
                if (_grid != null)
                {
                    checkGrid.CheckWalk(_grid.x, _grid.y);
                    colorGround.Add(_grid);
                }
            if (_grid = checkGrid.HexGround((hexagonX + homes), (hexagonY - homes)))
                if (_grid != null)
                {
                    checkGrid.CheckWalk(_grid.x, _grid.y);
                    colorGround.Add(_grid);
                }
        }
        if (hexagonY % 2 == 0)// Caso casa == 0
        {
            if (_grid = checkGrid.HexGround((hexagonX), (hexagonY + homes)))
                if (_grid != null)
                {
                    checkGrid.CheckWalk(_grid.x, _grid.y);
                    colorGround.Add(_grid);
                }
            if (_grid = checkGrid.HexGround((hexagonX), (hexagonY - homes)))
                if (_grid != null)
                {
                    checkGrid.CheckWalk(_grid.x, _grid.y);
                    colorGround.Add(_grid);
                }
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// andar
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="home"></param>
    /// <param name="Dbuff"></param>    
    /// <returns>Andou??</returns>
    public bool Walk(GameObject grid, int x, int y, int home = 1,bool Dbuff=false)
    {
        if (grid == null)
            grid = checkGrid.HexGround(x , y).gameObject;

        if (grid == null || !mobManager.Alive)
            return false;

        if (!Dbuff)
        {
            if (grid != null)
            {
                if (hexagonX == x && hexagonY == y)
                {
                    Debug.LogError(name + " Você nao pode mover pra casa em que você esta!!");
                    return false;
                }
                else
                {
                    #region Horizontal
                    if (grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX + home) + "x" + hexagonY       //horizontal   cima
                    ||  grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX - home) + "x" + hexagonY)       //horizontal   baixo
                    {
                        if (grid.GetComponent<HexManager>().free)
                        {

                            HexManager Here;

                            if (Solo == null)
                                Here = checkGrid.HexGround(hexagonX, hexagonY);// Pega hexa da casa q vc passou
                            else
                                Here = Solo;

                            if (Here != null && Here.currentMob == gameObject)
                            {
                                Here.free = true;                                        // Aonde eu estava agora esta free
                                Here.currentMob = null;
                            }

                            checkGrid.Check();
                            grid.GetComponent<HexManager>().free = false; // Pega Hexa atual Aonde estou agora esta ocupado
                            checkGrid.ColorGrid(1, x, y, true);
                            grid.GetComponent<HexManager>().currentMob = gameObject;
                            grid.GetComponent<HexManager>().WalkInHere();

                            time--;
                            hexagonX = x;
                            hexagonY = y;
                            solo = grid.GetComponent<HexManager>();

                            Vector3 hex = new Vector3(grid.GetComponentInChildren<Transform>().position.x, transform.position.y, grid.GetComponentInChildren<Transform>().position.z);

                            Move(grid.transform.position, hex, 5);

                            if (mobManager != null)
                                mobManager.ActivePassive(Passive.Walk, gameObject, 1);

                            //transform.position = Vector3.Lerp(grid.transform.position,hex,10f);
                            //this.transform.position = new Vector3(grid.GetComponentInChildren<Transform>().position.x, transform.position.y, grid.GetComponentInChildren<Transform>().position.z);
                            print(name + " Andou para a casa " + grid.GetComponentInChildren<Transform>().name);

                            WalkedDbuff();
                            return true;
                        }
                        else
                        {
                            if (!mobManager.isPlayer) EnemyWalk();
                            Debug.LogWarning(name + " Você nao pode andar nessa casa pois ela já esta ocupada");
                            return false;
                        }
                    }
                    else
                    #endregion
                    #region Diagonal ipar
            if (hexagonY % 2 == 1)//diagonal ipar
                    {
                        if (grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX) + "x" + (hexagonY + home)     //diagonal ipar left cima
                        || grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX) + "x" + (hexagonY - home)     //diagonal ipar left baixo
                        || grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX + home) + "x" + (hexagonY + home) //diagonal ipar right cima
                        || grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX + home) + "x" + (hexagonY - home)) //diagonal ipar right baixo
                        {
                            if (grid.GetComponent<HexManager>().free)
                            {
                                HexManager Here;

                                if (Solo == null)
                                    Here = checkGrid.HexGround(hexagonX, hexagonY);// Pega hexa da casa q vc passou
                                else
                                    Here = Solo;

                                if (Here != null && Here.currentMob == gameObject)
                                {
                                    Here.free = true;                                        // Aonde eu estava agora esta free
                                    Here.currentMob = null;
                                }

                                checkGrid.Check();
                                grid.GetComponent<HexManager>().free = false;// Pega Hexa atual Aonde estou agora esta ocupado
                                checkGrid.ColorGrid(1, x, y, true);
                                grid.GetComponent<HexManager>().currentMob = gameObject;
                                grid.GetComponent<HexManager>().WalkInHere();

                                time--;
                                hexagonX = x;
                                hexagonY = y;
                                solo = grid.GetComponent<HexManager>();

                                Vector3 hex = new Vector3(grid.GetComponentInChildren<Transform>().position.x, transform.position.y, grid.GetComponentInChildren<Transform>().position.z);

                                Move(grid.transform.position, hex, 5);
                                if (mobManager != null)
                                    mobManager.ActivePassive(Passive.Walk, gameObject, 1);

                                //transform.position = Vector3.Lerp(grid.transform.position, hex, 10f);
                                //this.transform.position = new Vector3(grid.GetComponentInChildren<Transform>().position.x, transform.position.y, grid.GetComponentInChildren<Transform>().position.z);
                                print(name + " Andou para a casa " + grid.GetComponentInChildren<Transform>().name);

                                WalkedDbuff();
                                return true;
                            }
                            else
                            {
                                if (!mobManager.isPlayer) EnemyWalk();
                                Debug.LogWarning(name + " Você nao pode andar nessa casa pois ela já esta ocupada");
                                return false;
                            }
                        }
                    }
                    else
                    #endregion
                    #region Diagonal Par
                if (hexagonY % 2 == 0) //diagonal par
                    {
                        if (grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX - home) + "x" + (hexagonY + home) //diagonal par  left cima
                         || grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX - home) + "x" + (hexagonY - home) //diagonal par  left baixo
                         || grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX) + "x" + (hexagonY + home)     //diagonal par  right cima
                         || grid.GetComponentInChildren<Transform>().name == "Hex" + (hexagonX) + "x" + (hexagonY - home))    //diagonal par  right baixo
                        {
                            if (grid.GetComponent<HexManager>().free)
                            {
                                HexManager Here;

                                if (Solo == null)
                                    Here = checkGrid.HexGround(hexagonX, hexagonY);// Pega hexa da casa q vc passou
                                else
                                    Here = Solo;

                                if (Here != null && Here.currentMob == gameObject)
                                {
                                    Here.free       = true;   // Aonde eu estava agora esta free
                                    Here.currentMob = null;
                                }                          

                                checkGrid.Check();
                                grid.GetComponent<HexManager>().free = false;//Pega Hexa atual Aonde estou agora esta ocupado
                                checkGrid.ColorGrid(1, x, y, true);
                                grid.GetComponent<HexManager>().currentMob = gameObject;
                                grid.GetComponent<HexManager>().WalkInHere();                               

                                time--;
                                hexagonX = x;
                                hexagonY = y;
                                solo     = grid.GetComponent<HexManager>();

                                Vector3 hex = new Vector3(grid.GetComponentInChildren<Transform>().position.x, transform.position.y, grid.GetComponentInChildren<Transform>().position.z);

                                //transform.position = Vector3.Lerp(grid.transform.position, hex, 10f);
                                //this.transform.position = new Vector3(grid.GetComponentInChildren<Transform>().position.x, transform.position.y, grid.GetComponentInChildren<Transform>().position.z);
                                Move(grid.transform.position, hex, 5);
                                if (mobManager != null)
                                    mobManager.ActivePassive(Passive.Walk, gameObject, 1);

                                print(name + " Andou para a casa " + grid.GetComponentInChildren<Transform>().name);

                                WalkedDbuff();
                            return true;
                            }
                            else
                            {
                                if (!mobManager.isPlayer) EnemyWalk();
                                Debug.LogWarning(name + " Você nao pode andar nessa casa pois ela já esta ocupada");
                                return false;
                            }
                        }
                    }
                    #endregion
                }
                if (x > hexagonX + home || y > hexagonY + home ||
                       x < hexagonX - home || y < hexagonY - home)
                {
                    checkGrid.ColorGrid(1, x, y, true);
                    Debug.LogError(name + " Você escolheu uma casa que esta Fora de alcance/limite.");
                    return false;
                }
            }
            else
            {
                print(name + " Esta casa nao existe");
                time--;
                return false;
            }
        }
        else
        {           
            if (grid.GetComponent<HexManager>().free)
            {
                HexManager Here;

                if (Solo == null)
                    Here = checkGrid.HexGround(hexagonX, hexagonY);// Pega hexa da casa q vc passou
                else
                    Here = Solo;

                if (Here != null && Here.currentMob == gameObject)
                {
                    Here.free = true;                                        // Aonde eu estava agora esta free
                    Here.currentMob = null;
                }

                checkGrid.Check();

                hexagonX = x;
                hexagonY = y;
                solo     = grid.GetComponent<HexManager>();


                grid.GetComponent<HexManager>().free = false; // Pega Hexa atual Aonde estou agora esta ocupado
                checkGrid.ColorGrid(1, x, y, true);
                grid.GetComponent<HexManager>().currentMob = gameObject;
                grid.GetComponent<HexManager>().WalkInHere();                

                //this.transform.position = new Vector3(grid.GetComponentInChildren<Transform>().position.x, transform.position.y, grid.GetComponentInChildren<Transform>().position.z);

                Vector3 hex = new Vector3(grid.GetComponentInChildren<Transform>().position.x, transform.position.y, grid.GetComponentInChildren<Transform>().position.z);

                Move(grid.transform.position, hex, 10f);

                if (mobManager != null)
                    mobManager.ActivePassive(Passive.WalkDbuff, gameObject, 1);

                Debug.LogError(name + " Foi puxada para a casa " + grid.GetComponentInChildren<Transform>().name);

                WalkedDbuff();

                return true;
            }
        }
        return false;
    }

    private void Move(Vector3 A,Vector3 B,float _time)
    {
        CameraOrbit.Instance.ChangeTarget(gameObject/*,true*/);

        DesColorGround();

        if (mobManager.walkTurn)
        {
            if (time > 0)
                EffectManager.Instance.PopUpDamageEffect(((/*mobManager.maxTimeWalk - */time)) + "/" + mobManager.maxTimeWalk, Solo.gameObject);
            else
            {                
                EffectManager.Instance.PopUpDamageEffect("Acabou", Solo.gameObject);
            }
        }

        if (ToolTip.Instance != null)
            ToolTip.Instance.AttTooltip();

        //transform.position = Vector3.Lerp(A, B, _time);

        mobManager.WalkAnim = true;

        iTween.MoveTo(gameObject,
            iTween.Hash(
                "delay", _delayEffect,
                "time", _timeEffect,
                "position", B,
                "easetype", easeTypeGoToEffect,
                "oncomplete", "StopAnimMove")
                );

        DesColorGround();
    }

    void StopAnimMove()
    {
        mobManager.WalkAnim = false;
    }

    GameObject ChangeTargetFollow()
    {
        if (mobManager.isPlayer)
            return null;

        List<GameObject> T = new List<GameObject>();

        T = TurnSystem.Instance.GetMob(mobManager.TimeMob,true);

        int i = Random.Range(0, T.Count);

        return T[i];
    }

    /// <summary>
    /// Apaga hexColoridos que simbolizam onde pode e nao andar
    /// </summary>
    /// <param name="clearList">Apagar Lista de hex coloridos</param>
    void DesColorGround(bool clearList=true)
    {
        foreach (var i in colorGround)
            checkGrid.ColorGrid(_hex: i,clear: true);

        if(clearList)
        colorGround.Clear();
    }
}

