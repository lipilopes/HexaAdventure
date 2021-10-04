using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackGoTo : SkillAttack
{
    protected CheckGrid checkGrid;

    [Space]
    [Header("Go To Attack")]

    [HideInInspector]
    public HexManager Solo;
    [HideInInspector]
    public int hexagonX;
    [HideInInspector]
    public int hexagonY;
    [SerializeField, Tooltip("Tempo de espera ao chegar no ground")]
    protected float _delay = 0;
    [SerializeField, Tooltip("Tempo que leva para chegar ao outro ground")]
    protected float _time = 0.5f;
    [SerializeField]
    protected iTween.EaseType easeTypeGoTo = iTween.EaseType.linear;
    [Space]
    [SerializeField, Tooltip("Skill Vai ate o Target")]
    protected bool followTarget = true;


    protected MoveController Follow;
    //WaitForSeconds waitCheckPositionCoroutine = new WaitForSeconds(30);

    #region Apaga depois
    //[Space]   
    //[Header("Propriedades  Dbuff Ground")]    
    //[SerializeField, Tooltip("Prefab do effect Dbuff Ground")]
    //protected GameObject DbuffGround;
    //[Space]
    //[SerializeField, Tooltip("Tempo que ficara ativo")]
    //protected int maxTimeDbuffGround = 3;
    //[SerializeField, Range(0, 100), Tooltip("Porcentagem de dano do ground com base no dano da skill")]
    //protected float porcenDamageDbuffGround = 50f;
    //[SerializeField, Range(0, 100), Tooltip("Porcentagem de dano ao andar sobe o ground")]
    //protected float porcenWalkDamageDbuffGround = 25f;
    //[Space]
    //[SerializeField, Range(0, 1), Tooltip("Chance de Dar mais hits $P2")]
    //protected float chanceMaxHitDbuffGround;
    //[SerializeField, Tooltip("Maximo de hits Extras $P3")]
    //protected int   maxHitAreaDbuffGround;

    //int _damageDbuffGround;
    //int _damageWalkDbuffGround;

    //protected List<GameObject> DbuffGroundList = new List<GameObject>();

    ///// <summary>
    ///// Skill deu hit em algo
    ///// </summary>
    ///// <param name="hitted"></param>
    //protected override void HittedSkill(GameObject hitted)
    //{
    //    if (DbuffGround != null && hitted != null && hitted.GetComponent<HexManager>() && hitted.GetComponent<HexManager>().currentItem == null)
    //    {
    //        Debug.LogError("<color=green>HittedSkill in " + hitted + " ->" + DbuffGround + "</color>");
    //        ActiveDbuffGround().RespawSkill(
    //            who,
    //            target,
    //            hitted.GetComponent<HexManager>(),
    //            _damageDbuffGround,
    //            _damageWalkDbuffGround,
    //            maxHitAreaDbuffGround,
    //            chanceMaxHitDbuffGround,
    //            maxTimeDbuffGround,
    //            critical);
    //    }

    //    base.HittedSkill(hitted);
    //}   

    //protected GroundEffect ActiveDbuffGround()
    //{
    //    if (DbuffGroundList.Count > 0)
    //    {
    //        foreach (var i in DbuffGroundList)
    //        {
    //            if (!i.gameObject.activeInHierarchy && i.GetComponent<GroundEffect>().TimeOn<=0)
    //            {
    //                Debug.LogError("<color=green>ActiveDbuffGround -> <b>Find</b> " + i + "</color>");
    //                return i.GetComponent<GroundEffect>();
    //            }
    //        }
    //    }

    //    GameObject obj = Instantiate(DbuffGround, new Vector3(-99, -99, -99), who.transform.rotation);

    //    obj.name = DbuffGround.name + " " + (DbuffGroundList.Count + 1) + "  (" + who.GetComponent<ToolTipType>()._nameS + ")";

    //    DbuffGroundList.Add(obj);

    //    if (RespawMob.Instance)
    //        RespawMob.Instance.allRespaws.Add(obj.gameObject);

    //    obj.gameObject.SetActive(false);

    //    Debug.LogError("<color=green>ActiveDbuffGround -> <b>Create</b> " + obj + "</color>");

    //    return obj.GetComponent<GroundEffect>();
    //}
    #endregion

    protected override void Start()
    {
        base.Start();

        checkGrid = CheckGrid.Instance;

        //for (int i = 0; i < 5; i++)
        //{
        //    ActiveDbuffGround();
        //}        
    }

    public override void UseSkill(GameObject _target, MobSkillManager _skill)
    {
        who = _skill.User;

        transform.position = who.transform.position;

        if (who != null)
        {
            Solo = who.GetComponent<MoveController>().Solo;

            hexagonX = Solo.x;
            hexagonY = Solo.y;

            transform.position = Solo.transform.position;
        }

        base.UseSkill(_target, _skill);

        transform.position = who.transform.position;

        if (who != null)
        {
            Solo = who.GetComponent<MoveController>().Solo;

            hexagonX = Solo.x;
            hexagonY = Solo.y;

            transform.position = Solo.transform.position;
        }

        //_damageDbuffGround     = (int)((damage * porcenDamageDbuffGround)     / 100);
        //_damageWalkDbuffGround = (int)((damage * porcenWalkDamageDbuffGround) / 100);

        transform.LookAt(_target.transform);

        if (target.GetComponent<MoveController>())
        {
            Follow = target.GetComponent<MoveController>();
            EnemyWalk(Follow, !followTarget);
            //Debug.LogError("<color=green>UseSkill -> " + _target + ", Tem move Controller</color>");
        }
        else
        {
            if (target.GetComponent<HexManager>())
            {
                EnemyWalk(target.GetComponent<HexManager>(), !followTarget);
            }
            else
                Move(Solo.transform.position, target.transform.position, _time);
            //Debug.LogError("<color=red>UseSkill -> " + _target + ",Não Tem move Controller</color>");
        }
    }

    /// <summary>
    /// Usar skill,com ponto de inicio diferente de quem esta usando
    /// </summary>
    /// <param name="_target">posição final</param>
    /// <param name="_skill"></param>
    /// <param name="_startPosition">posição inicial</param>
    public virtual void UseSkillGoTo(GameObject _target, MobSkillManager _skill, HexManager _startPosition)
    {
        Solo      = _startPosition;

        hexagonX = Solo.x;
        hexagonY = Solo.y;

        transform.position = Solo.transform.position;

        base.UseSkill(_target, _skill);

        transform.LookAt(_target.transform);

        if (target.GetComponent<MoveController>())
        {
            Follow = target.GetComponent<MoveController>();
            EnemyWalk(Follow, !followTarget);
            //Debug.LogError("<color=green>UseSkill -> " + _target + ", Tem move Controller</color>");
        }
        else
        {
            if (target.GetComponent<HexManager>())
            {
                EnemyWalk(target.GetComponent<HexManager>(), !followTarget);
            }
            else
                Move(Solo.transform.position, target.transform.position, _time);
            //Debug.LogError("<color=red>UseSkill -> " + _target + ",Não Tem move Controller</color>");
        }
    }

    /// <summary>
    /// Ve o melhor hex para seguir ou fugir do target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="foge"></param>
    protected virtual void EnemyWalk(MoveController target = null, bool foge = false)
    {
        if (target == null)
        {
            //Debug.LogError("EnemyWalk Target -> Null");

            Desactive();

            return;
        }

        if (target.gameObject != gameObject)
            transform.LookAt(target.transform);

        Debug.LogError("enemyWalk (" + Solo + " / " + target.Solo + ") -> " + (hexagonY % 2 == 1) + " par");

        if (!foge)
        {
            Debug.LogError(gameObject.name + " esta indo pra cima do " + target.name);

            #region Vai pra cima
            if (hexagonY < target.hexagonY)//Player em baixo
            {
                Debug.LogError(gameObject.name + " esta abaixo do " + target.name);
                #region 
                if (hexagonY % 2 == 1)//par
                {
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), hexagonY + 1);
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                   if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), hexagonY + 1);
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), hexagonY + 1);
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta abaixo do " + target.name + " Par não encontrou");
                        //time--;
                        //return;
                    }
                }
                else //Impar
                {
                    if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta abaixo do " + target.name + " Impar não encontrou");
                        //time--;
                        //return;
                    }

                }
                #endregion
            }
            else if (hexagonY > target.hexagonY)//Player cima
            {
                Debug.LogError(gameObject.name + " esta acima do " + target.name);
                #region
                if (hexagonY % 2 == 1)//Impar
                {

                    if (CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                     if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX - 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta acima do " + target.name + " Par não encontrou");
                        //time--;
                        //return;
                    }
                }
                else
                {
                    if (CheckWalk(hexagonX - 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta acima do " + target.name + " impar não encontrou");
                        //time--;
                        //return;
                    }
                }
                #endregion
            }
            else if (hexagonY == target.hexagonY)
            {
                Debug.LogError(gameObject.name + " esta na mesma coluna do " + target.name);
                #region
                if (hexagonX < target.hexagonX)//Player esta la ->
                {
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                     if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX + 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta na mesma coluna do " + target.name + " X < targetX não encontrou");
                        //time--;
                        //return;
                    }
                }
                else
                {
                    if (CheckWalk(hexagonX - 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                     if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                     if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                     if (CheckWalk(hexagonX + 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta na mesma coluna do " + target.name + " X > targetX não encontrou");
                        //time--;
                        //return;
                    }
                }
                #endregion
            }
            else if (hexagonX < target.hexagonX)//Player esta la ->
            {
                Debug.LogError(gameObject.name + " esta a direita do " + target.name);
                #region
                if (CheckWalk(hexagonX + 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                    CheckHome(grid, grid.x, grid.y);
                    return;
                }
                else
                {
                    Debug.LogError(gameObject.name + " esta a direita do " + target.name + "não encontrou");
                    //time--;
                    //return;
                }
                #endregion
            }
            else if (hexagonX > target.hexagonX)//Player esta la <-
            {
                Debug.LogError(gameObject.name + " esta a esquerda do " + target.name);
                #region
                if (CheckWalk(hexagonX - 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                    CheckHome(grid, grid.x, grid.y);
                    return;
                }
                else
                {
                    Debug.LogError(gameObject.name + " esta a esquerda do " + target.name + "não encontrou");
                    //time--;
                    //return;
                }
                #endregion
            }
            else if (hexagonX == target.hexagonX)//Player esta la <-
            {
                Debug.LogError(gameObject.name + " esta na mesma linha " + target.name);
                #region
                if (hexagonY > target.hexagonY)//_targetFollow esta em cima
                {
                    #region
                    if (hexagonY % 2 == 1)//Impar
                    {
                        if (CheckWalk(hexagonX, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                      if (CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            Debug.LogError(gameObject.name + " esta na mesma linha " + target.name + " impar target a cima não encontrou");
                            //time--;
                            //return;
                        }
                    }
                    else
                    {
                        if (CheckWalk(hexagonX - 1, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        if (CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            Debug.LogError(gameObject.name + " esta na mesma linha " + target.name + " par target a cima não encontrou");
                            //time--;
                            //return;
                        }
                    }
                    #endregion
                }
                else //Player esta em baixo
                {
                    #region 
                    if (hexagonY % 2 == 1)//Impar
                    {
                        if (CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                       if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            Debug.LogError(gameObject.name + " esta na mesma linha " + target.name + " impar target a baixo não encontrou");
                            //time--;
                            //return;
                        }
                    }
                    else //Par
                    {
                        if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        if (CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            Debug.LogError(gameObject.name + " esta na mesma linha " + target.name + " par target a baixo não encontrou");
                            //time--;
                            //return;
                        }

                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                Debug.LogError(gameObject.name + " falhou em ir pra cima do " + target.name);
                //time--;
                //return;
            }
            #endregion
        }
        else
        //if (foge)
        {
            Debug.LogError(gameObject.name + " esta correndo do " + target.name);
            #region Fuga
            if (hexagonY > target.hexagonY)//Player em cima
            {
                #region 
                if (hexagonY % 2 == 1)//Impar
                {
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                   if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                else //Par
                {
                    if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }

                }
                #endregion
            }
            else if (hexagonY < target.hexagonY)//Player baixo
            {
                #region
                if (hexagonY % 2 == 1)//Impar
                {
                    if (CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                  if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                else
                {
                    if (CheckWalk(hexagonX - 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                #endregion
            }
            else if (hexagonY == target.hexagonY)
            {
                #region
                if (hexagonY > target.hexagonX)//Player esta la <-
                {
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                else
                {
                    if (CheckWalk(hexagonX - 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                #endregion
            }
            else if (hexagonX > target.hexagonX)//Player esta la <-
            {
                #region
                if (CheckWalk(hexagonX + 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                    CheckHome(grid, grid.x, grid.y);
                    return;
                }
                else
                {
                    //time--;
                    //return;
                }
                #endregion
            }
            else if (hexagonX < target.hexagonX)//Player esta la ->
            {
                #region
                if (CheckWalk(hexagonX - 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                    CheckHome(grid, grid.x, grid.y);
                    return;
                }
                else
                {
                    //time--;
                    //return;
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
                        if (CheckWalk(hexagonX, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                      if (CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            //time--;
                            //return;
                        }
                    }
                    else
                    {
                        if (CheckWalk(hexagonX - 1, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        if (CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            //time--;
                            //return;
                        }
                    }
                    #endregion
                }
                else //Player esta em cima
                {
                    #region 
                    if (hexagonX % 2 == 1)//Impar
                    {
                        if (CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                       if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            //time--;
                            //return;
                        }
                    }
                    else //Par
                    {
                        if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        if (CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            //time--;
                            //return;
                        }

                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
    }

    /// <summary>
    /// Ve o melhor hex para seguir ou fugir do target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="foge"></param>
    protected virtual void EnemyWalk(HexManager target = null, bool foge = false)
    {
        if (target == null)
        {
            //Debug.LogError("EnemyWalk Target -> Null");

            Desactive();

            return;
        }

        if (target.gameObject != gameObject)
            transform.LookAt(target.transform);

        Debug.LogError("enemyWalk (" + Solo + " / " + target + ") -> " + (hexagonY % 2 == 1) + " par");

        if (!foge)
        {
            Debug.LogError(gameObject.name + " esta indo pra cima do " + target.name);

            #region Vai pra cima
            if (hexagonY < target.y)//Player em baixo
            {
                Debug.LogError(gameObject.name + " esta abaixo do " + target.name);
                #region 
                if (hexagonY % 2 == 1)//par
                {
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), hexagonY + 1);
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                   if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), hexagonY + 1);
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), hexagonY + 1);
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta abaixo do " + target.name + " Par não encontrou");
                        //time--;
                        //return;
                    }
                }
                else //Impar
                {
                    if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta abaixo do " + target.name + " Impar não encontrou");
                        //time--;
                        //return;
                    }

                }
                #endregion
            }
            else if (hexagonY > target.y)//Player cima
            {
                Debug.LogError(gameObject.name + " esta acima do " + target.name);
                #region
                if (hexagonY % 2 == 1)//Impar
                {

                    if (CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                     if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX - 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta acima do " + target.name + " Par não encontrou");
                        //time--;
                        //return;
                    }
                }
                else
                {
                    if (CheckWalk(hexagonX - 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta acima do " + target.name + " impar não encontrou");
                        //time--;
                        //return;
                    }
                }
                #endregion
            }
            else if (hexagonY == target.y)
            {
                Debug.LogError(gameObject.name + " esta na mesma coluna do " + target.name);
                #region
                if (hexagonX < target.x)//Player esta la ->
                {
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                     if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX + 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta na mesma coluna do " + target.name + " X < targetX não encontrou");
                        //time--;
                        //return;
                    }
                }
                else
                {
                    if (CheckWalk(hexagonX - 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                     if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                    if (CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                     if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else//*
                     if (CheckWalk(hexagonX + 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + " esta na mesma coluna do " + target.name + " X > targetX não encontrou");
                        //time--;
                        //return;
                    }
                }
                #endregion
            }
            else if (hexagonX < target.x)//Player esta la ->
            {
                Debug.LogError(gameObject.name + " esta a direita do " + target.name);
                #region
                if (CheckWalk(hexagonX + 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                    CheckHome(grid, grid.x, grid.y);
                    return;
                }
                else
                {
                    Debug.LogError(gameObject.name + " esta a direita do " + target.name + "não encontrou");
                    //time--;
                    //return;
                }
                #endregion
            }
            else if (hexagonX > target.x)//Player esta la <-
            {
                Debug.LogError(gameObject.name + " esta a esquerda do " + target.name);
                #region
                if (CheckWalk(hexagonX - 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                    CheckHome(grid, grid.x, grid.y);
                    return;
                }
                else
                {
                    Debug.LogError(gameObject.name + " esta a esquerda do " + target.name + "não encontrou");
                    //time--;
                    //return;
                }
                #endregion
            }
            else if (hexagonX == target.x)//Player esta la <-
            {
                Debug.LogError(gameObject.name + " esta na mesma linha " + target.name);
                #region
                if (hexagonY > target.y)//_targetFollow esta em cima
                {
                    #region
                    if (hexagonY % 2 == 1)//Impar
                    {
                        if (CheckWalk(hexagonX, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                      if (CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            Debug.LogError(gameObject.name + " esta na mesma linha " + target.name + " impar target a cima não encontrou");
                            //time--;
                            //return;
                        }
                    }
                    else
                    {
                        if (CheckWalk(hexagonX - 1, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        if (CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            Debug.LogError(gameObject.name + " esta na mesma linha " + target.name + " par target a cima não encontrou");
                            //time--;
                            //return;
                        }
                    }
                    #endregion
                }
                else //Player esta em baixo
                {
                    #region 
                    if (hexagonY % 2 == 1)//Impar
                    {
                        if (CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                       if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            Debug.LogError(gameObject.name + " esta na mesma linha " + target.name + " impar target a baixo não encontrou");
                            //time--;
                            //return;
                        }
                    }
                    else //Par
                    {
                        if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        if (CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            Debug.LogError(gameObject.name + " esta na mesma linha " + target.name + " par target a baixo não encontrou");
                            //time--;
                            //return;
                        }

                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                Debug.LogError(gameObject.name + " falhou em ir pra cima do " + target.name);
                //time--;
                //return;
            }
            #endregion
        }
        else
        //if (foge)
        {
            Debug.LogError(gameObject.name + " esta correndo do " + target.name);
            #region Fuga
            if (hexagonY > target.y)//Player em cima
            {
                #region 
                if (hexagonY % 2 == 1)//Impar
                {
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                   if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                else //Par
                {
                    if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX, hexagonY + 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }

                }
                #endregion
            }
            else if (hexagonY < target.y)//Player baixo
            {
                #region
                if (hexagonY % 2 == 1)//Impar
                {
                    if (CheckWalk(hexagonX, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                  if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                else
                {
                    if (CheckWalk(hexagonX - 1, hexagonY - 1, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                #endregion
            }
            else if (hexagonY == target.y)
            {
                #region
                if (hexagonY > target.x)//Player esta la <-
                {
                    if (CheckWalk(hexagonX + 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                else
                {
                    if (CheckWalk(hexagonX - 1, hexagonY, false))
                    {
                        HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                        CheckHome(grid, grid.x, grid.y);
                        return;
                    }
                    else
                    {
                        //time--;
                        //return;
                    }
                }
                #endregion
            }
            else if (hexagonX > target.x)//Player esta la <-
            {
                #region
                if (CheckWalk(hexagonX + 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                    CheckHome(grid, grid.x, grid.y);
                    return;
                }
                else
                {
                    //time--;
                    //return;
                }
                #endregion
            }
            else if (hexagonX < target.x)//Player esta la ->
            {
                #region
                if (CheckWalk(hexagonX - 1, hexagonY, false))
                {
                    HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                    CheckHome(grid, grid.x, grid.y);
                    return;
                }
                else
                {
                    //time--;
                    //return;
                }
                #endregion

            }
            else if (hexagonX == target.x)//Player esta la <-
            {
                #region
                if (hexagonY < target.y)//_targetFollow esta em baixo
                {
                    #region
                    if (hexagonY % 2 == 1)//Impar
                    {
                        if (CheckWalk(hexagonX, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                      if (CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            //time--;
                            //return;
                        }
                    }
                    else
                    {
                        if (CheckWalk(hexagonX - 1, hexagonY - 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY - 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        if (CheckWalk(hexagonX + 1, hexagonY, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            //time--;
                            //return;
                        }
                    }
                    #endregion
                }
                else //Player esta em cima
                {
                    #region 
                    if (hexagonX % 2 == 1)//Impar
                    {
                        if (CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                       if (CheckWalk(hexagonX + 1, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX + 1), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            //time--;
                            //return;
                        }
                    }
                    else //Par
                    {
                        if (CheckWalk(hexagonX - 1, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX - 1), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        if (CheckWalk(hexagonX, hexagonY + 1, false))
                        {
                            HexManager grid = checkGrid.HexGround((hexagonX), (hexagonY + 1));
                            CheckHome(grid, grid.x, grid.y);
                            return;
                        }
                        else
                        {
                            //time--;
                            //return;
                        }

                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    protected virtual void CheckHome(HexManager hex, int x, int y)
    {
        // Debug.LogError("CheckHome -> "+ hex);

        Solo = hex;

        Move(transform.position, hex.transform.position, _time);
    }

    /// <summary>
    /// move skill do ponto A ao B
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="_time"></param>
    protected void Move(Vector3 A, Vector3 B, float _time)
    {
        //Debug.LogError("Move de " + A + " para "+ B+" em "+_time);

        if (transform.position != A)
        {
            transform.position = A;
        }

        iTween.MoveTo(gameObject,
            iTween.Hash(
                "delay", _delay,
                "time", _time,
                "position", B,
                "easetype", easeTypeGoTo,
                "oncomplete", "CheckPosition")
                );
    }

    protected virtual void CheckPosition()
    {
        hexagonX = Solo.x;
        hexagonY = Solo.y;

        //Debug.LogError("CheckPosition");

        if (target.GetComponent<HexManager>() && Follow == null)
        {
            EnemyWalk(target.GetComponent<HexManager>(), !followTarget);
        }
        else
            EnemyWalk(Follow, !followTarget);
    }

    /// <summary>
    /// Ve se hex esta apto para a skill
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="Color"></param>
    /// <returns></returns>
    protected virtual bool CheckWalk(int X, int Y, bool Color)
    {
        if (checkGrid == null)
            checkGrid = CheckGrid.Instance;

        if (checkGrid == null)
            return false;

        HexManager hex = checkGrid.HexGround((hexagonX), (hexagonY/* - 1*/));

        if (Color)
            checkGrid.CheckWalk(hexagonX, hexagonY);

        if (hex != null)
        {
            if (hex.free)
            {
                //Debug.LogError("<color=green>CheckWalk ->"+hex+"</color>");
                return true;
            }
            else
                if (hex.currentMob.GetComponent<MobManager>())
            {
                //Debug.LogError("<color=green>CheckWalk ->" + hex + "</color>");
                return true;
            }
            else
                if (!hex.free && hex.currentMob.GetComponent<MobManager>() == null)
            {
                return true;
            }
        }
        //Debug.LogError("CheckWalk -> False");
        return false;
    }


    public override void Desactive()
    {
        base.Desactive();

        if (who != null)
        {
            Solo = who.GetComponent<MoveController>().Solo;
            hexagonX = Solo.x;
            hexagonY = Solo.y;
        }

        transform.position = Solo.transform.position;
    }

    protected override IEnumerator WaitHitTargetCoroutine()
    {
        yield return waitHitTarget;
    }
}
