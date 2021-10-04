
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Time Attack")]
    public int timeAttack;

    [Header("List Target Skill")]
    public List<GameObject> ListTarget = new List<GameObject>();
    public List<GameObject> ListTargetSkill1 = new List<GameObject>();
    public List<GameObject> ListTargetSkill2 = new List<GameObject>();
    public List<GameObject> ListTargetSkill3 = new List<GameObject>();

    [Header("Target")]
    public GameObject target;

    // [Header("Name Skills")]
    [HideInInspector]
    public string nameSkill1;
    [HideInInspector]
    public string nameSkill2,
                                    nameSkill3;

    //[Header("Maior Distance Skill")]
    [HideInInspector]
    public int moreDistanceSkill;

    bool checkTarget = false;

    [Header("Quais Skill's Pode Usar")]
    public bool canSkill1 = false;
    public bool canSkill2 = false,
                canSkill3 = false;


    SkillManager         skillManager;
    MobCooldown          mobCooldown;
    MobManager           mobManager;
    //MobAttack            mobAttack;
    InfoTable            infoTable;
    EffectManager        effect;
    ButtonManager        buttonManager;
    public ButtonManager ButtonManager
    {
        get
        {
            return buttonManager;
        }
    }

    [HideInInspector]
    public bool useSkill = false;

    //GetHexa
           List<HexManager> hexList = new List<HexManager>();
    public List<HexManager> HexList
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

    /// <summary>
    /// Apenas um retun pra calcular o tooltip
    /// </summary>
    [HideInInspector]
    public int _damageSkill1;

    /// <summary>
    /// Apenas um retun pra calcular o tooltip
    /// </summary>
    [HideInInspector]
    public int _damageSkill2;

    /// <summary>
    /// Apenas um retun pra calcular o tooltip
    /// </summary>
    [HideInInspector]
    public int _damageSkill3;

    void Start()
    {
        mobCooldown  = this.GetComponent<MobCooldown>();
        mobManager   = this.GetComponent<MobManager>();
        //mobAttack    = this.GetComponent<MobAttack>();
        skillManager = this.GetComponent<SkillManager>();

        if (!mobManager.MesmoTime(RespawMob.Instance.PlayerTime))
            target = RespawMob.Instance.Player;   

        Att();

        if (infoTable == null)
            infoTable = (InfoTable)GameObject.FindObjectOfType(typeof(InfoTable));

        if (effect == null)
            effect = EffectManager.Instance;
    }

    #region Player
    private void Update()
    {
        if (!mobManager.isPlayer)
            return;

        if (mobManager.attackTurn && !skillManager.SkillInUse || mobManager.attackTurn && !useSkill)
            ClickTarget();
    }

    void ClickTarget()
    {
        if (GameManagerScenes._gms.Paused == true)
        return;

        Ray ray;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
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

            if (Input.GetMouseButtonDown(0))
            {
                #region Caso Click no Hexagono                             
                #region ITEM
                if (hitObject.GetComponent<HexManager>() != null)
                    if (hitObject.GetComponent<HexManager>().currentItem != null)
                        if (hitObject.GetComponent<HexManager>().currentItem.GetComponent<PortalManager>() == null)
                            hitObject = hitObject.GetComponent<HexManager>().currentItem;
                #endregion



                #region ITEM RC HP
                if (hitObject.GetComponent<ItemRecHp>() != null)
                    if (hitObject.GetComponent<ItemRecHp>().Here != null)
                        if (hitObject.GetComponent<ItemRecHp>().Here.GetComponent<HexManager>().currentItem != null)
                            hitObject = hitObject.GetComponent<ItemRecHp>().Here.GetComponent<HexManager>().currentItem;
                #endregion


                #region MOB
                if (hitObject.GetComponent<HexManager>() != null)
                    if (hitObject.GetComponent<HexManager>().currentMob != null)
                        if (hitObject.GetComponent<HexManager>().currentMob.GetComponent<MobManager>() != null)
                            if (hitObject.GetComponent<HexManager>().currentMob.GetComponent<MobManager>().Alive)
                                hitObject = hitObject.GetComponent<HexManager>().currentMob;
                #endregion
                #endregion

                #region Skill1
                if (ListTargetSkill1.Contains(hitObject))
                {
                    target    = hitObject;
                    canSkill1 = true;
                    //print("Voce Targetou[Na lista skill1] o obj: " + target.name);
                }
                else
                {
                    canSkill1 = false;
                    //Debug.LogError("Você nao pode escolher um alvo que nao existe na lista skill1.");
                }
                #endregion

                #region Skill2
                if (ListTargetSkill2.Contains(hitObject))
                {
                    target = hitObject;
                    canSkill2 = true;
                    //print("Voce Targetou[Na lista skill2] o obj: " + target.name);
                }
                else
                {
                    canSkill2 = false;
                    //Debug.LogError("Você nao pode escolher um alvo que nao existe na lista skill2.");
                }
                #endregion

                #region Skill3
                if (ListTargetSkill3.Contains(hitObject))
                {
                    target = hitObject;
                    canSkill3 = true;
                    //print("Voce Targetou[Na lista skill3] o obj: " + target.name);
                }
                else
                {
                    canSkill3 = false;
                    //Debug.LogError("Você nao pode escolher um alvo que nao existe na lista skill3.");
                }
                #endregion               

                Debug.LogError(name + " tem com Target o " + hit.collider.transform.gameObject);

                effect.TargetTargeteado(target);

                if (ListTarget.Contains(target) && skillManager != null ||
                    ListTarget.Contains(target) && !useSkill)
                {
                    //if (target != null)
                        transform.LookAt(target.transform);

                    if (!skillManager.SkillInUse)
                    {
                        if (GameManagerScenes._gms.PlayerID == 1 || GameManagerScenes._gms.PlayerID == 9 || GameManagerScenes._gms.PlayerID == 25)
                            if (GameManagerScenes._gms.FaseAtual <= 1)
                                if (hitObject.GetComponent<ItemRecHp>() != null)
                                    infoTable.NewInfo("Quando você pegar um <b>Rec Hp</b> com o <b>" + nameSkill2 + "</b>, \n você ganha um bonus de \n <b>-1 no Cooldown de todas as suas Skill's</b>.", 10);
                    }
                }
            }
        }

    }//Escolhe na list um inimigo
    #endregion

    public void StartAttackTurn()
    {
        Debug.LogError(name + " StartAttackTurn()");

        if (buttonManager == null)
        {
            buttonManager = ButtonManager.Instance;
        }

        if (GetComponent<MobManager>().myTurn)
            if (timeAttack > 0)
            {
                Debug.LogError(name + " StartAttackTurn() - iniciando");

                AttDistanceSkill();

                CheckDistance();
                CheckDistance(1);
                CheckDistance(2);
                CheckDistance(3);

                if (ListTarget.Count <= 0 ||
                    skillManager.Skills[0].SilenceSkill && skillManager.Skills[1].SilenceSkill && skillManager.Skills[2].SilenceSkill)
                    mobManager.EndAttackTurn();
                else
                {                    
                    Invoke("CheckInList", 2);
                }
            }
            else if (timeAttack <= 0)
            {

                Debug.LogError(name + " StartAttackTurn() finalizando");

                GetComponent<MobManager>().EndAttackTurn();
            }
    }

    public GameObject RandomTarget()
    {
        GameObject _t = null;

        if (ListTarget.Count != 0)
        {
            if (ListTarget.Count > 1)
            {
                float _hp = 99999;

                for (int i = 0; i < ListTarget.Count; i++)
                {
                    if (ListTarget[i].GetComponent<MobHealth>())
                    {
                        if (ListTarget[i].GetComponent<MobHealth>().Health <= _hp)
                        {
                            _hp = ListTarget[i].GetComponent<MobHealth>().Health;
                            _t  = ListTarget[i];
                        }
                    }
                }
            }
            else
                _t = ListTarget[0];

            if (_t == null)
              if (ListTarget.Count != 0)
                return ListTarget[Random.Range(0, ListTarget.Count)];
        }

        return _t;
    }

    public void CheckInList()
    {
        target = RandomTarget();

        if (timeAttack > 0)
        {
            #region ListSkill1
            /*if (ListTargetSkill1.Contains(target) && mobCooldown.timeCooldownSkill[0] <= 0 && mobAttack.Skill1NeedTarget ||
                mobCooldown.timeCooldownSkill[0] <= 0 && !mobAttack.Skill1NeedTarget)*/
            if (ListTargetSkill1.Contains(target) && ListTargetSkill1.Count > 0 && skillManager.Skills[0].TargetFriend ||
                ListTargetSkill1.Contains(target) && ListTargetSkill1.Count > 0 && skillManager.Skills[0].NeedTarget ||
                ListTargetSkill1.Contains(target) && ListTargetSkill1.Count > 0)
            {
                canSkill1 = true;

                print(name + " Targetou[Na lista skill1] o obj: " + target);

            }
            else
            {
                ListTargetSkill1.Remove(target);
                ListTarget.Remove(target);
                canSkill1 = false;
            }
            #endregion

            #region ListSkill2
            /*if (ListTargetSkill2.Contains(target) && mobCooldown.timeCooldownSkill[1] <= 0 && mobAttack.Skill2NeedTarget ||
                mobCooldown.timeCooldownSkill[1] <= 0 && !mobAttack.Skill2NeedTarget)*/
            if (ListTargetSkill2.Contains(target) && ListTargetSkill2.Count > 0 && skillManager.Skills[1].TargetFriend ||
                ListTargetSkill2.Contains(target) && ListTargetSkill2.Count > 0 && skillManager.Skills[1].NeedTarget ||
                ListTargetSkill2.Contains(target) && ListTargetSkill2.Count > 0)
            {
                canSkill2 = true;
                print(name + " Targetou[Na lista skill2] o obj: " + target);
            }
            else
            {
                ListTargetSkill2.Remove(target);
                ListTarget.Remove(target);
                canSkill2 = false;
            }
            #endregion

            #region ListSkill3
            /*if (ListTargetSkill3.Contains(target) && mobCooldown.timeCooldownSkill[2] <= 0 && mobAttack.Skill3NeedTarget ||
                mobCooldown.timeCooldownSkill[2] <= 0 && !mobAttack.Skill3NeedTarget)*/
            if (ListTargetSkill3.Contains(target) && ListTargetSkill3.Count > 0 && skillManager.Skills[2].TargetFriend ||
                ListTargetSkill3.Contains(target) && ListTargetSkill3.Count > 0 && skillManager.Skills[2].NeedTarget ||
                ListTargetSkill3.Contains(target) && ListTargetSkill3.Count > 0)
            {
                canSkill3 = true;
                print(name + " Targetou[Na lista skill3] o obj: " + target);
            }
            else
            {
                canSkill3 = false;
                ListTargetSkill3.Remove(target);
                ListTarget.Remove(target);
            }
            #endregion

            #region Attack's Sem player na Area
            CheckListNoPlayer();
            #endregion

            if (target != null)
            {
                effect.TargetTargeteado(target);

                transform.LookAt(target.transform);

                Debug.LogError(name + " S1(" + canSkill1 + " - " + ListTargetSkill1.Count + " na list)");
                Debug.LogError(name + " S2(" + canSkill2 + " - " + ListTargetSkill2.Count + " na list)");
                Debug.LogError(name + " S3(" + canSkill3 + " - " + ListTargetSkill3.Count + " na list)");

                if (canSkill1 || canSkill2 || canSkill3)
                {
                    if (!mobManager.isPlayer)
                    {
                        //    if (skillManager == null)
                        //    {

                        Debug.LogError("TARGET :" + target);
                        this.GetComponent<IaAttackMob>().Attack();
                        return;
                        //}
                        //else
                        //    for (int i = 0; i < skillManager.sequenceSkill.Length; i++)
                        //    {
                        //        if (FindSkill(skillManager.sequenceSkill[i]))
                        //        {
                        //            skillManager.UseSkill(skillManager.sequenceSkill[i]);
                        //            return;
                        //        }
                        //    }
                    }
                }
            }
        }

            GetComponent<MobManager>().EndAttackTurn();
            print(this.name + " Sem timeAttack");
    }

    void CheckListNoPlayer()//Check List Sem player na area
    {
        //GameObject manager = GameObject.FindGameObjectWithTag("Manager");
        //TurnSystem respaw  = manager.GetComponent<TurnSystem>();

        #region Planta Carnivora
        if (tag == "Plantacarnivora")
        {
            if (!canSkill1 && !canSkill3 && mobCooldown.timeCooldownSkill[1] <= 0)
                canSkill2 = true;

            return;
        }
        #endregion

        #region Unicornio
        if (tag == "Unicorniocorrompido")
        {
            if (!canSkill1 && !canSkill3 && mobCooldown.timeCooldownSkill[1] <= 0)
            {
                canSkill2 = true;

                List<GameObject> _friends = TurnSystem.Instance.GetMob(mobManager.TimeMob, false);

                if (_friends.Count <= 0)
                    _friends.Add(gameObject);

                int mob = Random.Range(0, _friends.Count);

                target = _friends[mob];

                if (target.GetComponent<EnemyAttack>() == null)
                    canSkill2 = false;
            }
            return;
        }
        #endregion

        #region Gnobus
        if (tag == "Gnobus")
        {
            if (mobCooldown.timeCooldownSkill[0] <= 0 && !canSkill2 && !canSkill3)
            {
                canSkill1 = true;

                List<GameObject> _friends = TurnSystem.Instance.GetMob(mobManager.TimeMob, false);

                if (_friends.Count <= 0)
                    _friends.Add(gameObject);

                int mob = Random.Range(0, _friends.Count);

                target = _friends[mob];

                if (target == null)
                {
                    canSkill1 = false;
                }
                else
                {
                    transform.LookAt(target.transform);

                    if (target.GetComponent<EnemyAttack>() == null)
                        canSkill1 = false;
                }
            }

            return;
        }
        #endregion

        #region Cyber
        if (tag == "Cyber")
        {
            if (mobCooldown.timeCooldownSkill[0] <= 0 && !canSkill2)
                canSkill1 = true;

            if (mobCooldown.timeCooldownSkill[2] <= 0 && !canSkill2 && mobCooldown.timeCooldownSkill[0] >= 1 && mobCooldown.timeCooldownSkill[1] >= 1)
                canSkill3 = true;

            return;
        }
        #endregion

        #region Saci
        if (tag == "Saci")
        {
            if (mobCooldown.timeCooldownSkill[2] <= 0 && !canSkill2 && !canSkill1)
                canSkill3 = true;

            return;
        }
        #endregion

        #region Macaco Rei
        if (tag == "Macacorei" ||
           tag == "Macacoreiclone")
        {
            #region Skill2
            if (mobCooldown.timeCooldownSkill[2] <= 0 && !canSkill3 && !canSkill1)
            {
                List<GameObject> _friends = TurnSystem.Instance.GetMob(mobManager.TimeMob, false, gameObject);

                if (_friends.Count < 0)
                    canSkill2 = true;
            }
            #endregion

            if (mobCooldown.timeCooldownSkill[2] <= 0 && !canSkill2 && !canSkill1)
                canSkill3 = true;

            return;
        }
        #endregion
    }

    public void Att()
    {
        if (skillManager != null)
        {
            if (skillManager.Skills[0] != null)
               nameSkill1 = skillManager.Skills[0].Nome;

            if (skillManager.Skills[1] != null)
                nameSkill2 = skillManager.Skills[1].Nome;

            if (skillManager.Skills[2] != null)
                nameSkill3 = skillManager.Skills[2].Nome;
        }


        AttDistanceSkill();

        //if (GetComponent<DetalSkillToolTip>() != null)
        //    GetComponent<DetalSkillToolTip>().AttDamage();

    }//atualiza esse script com base no  mob Attack

    public GameObject TargetAttack(bool Add = true, GameObject _target = null, int listSkill = -1)
    {
        // print("Add:"+Add+" _target "+_target+" skill "+listSkill);

        if (Add && listSkill>=0 && skillManager.Skills[listSkill-1].SilenceSkill)
            return null;

        switch (listSkill)
        {
            #region Skill list 1
            case 1:
                if (Add && _target != null && mobCooldown.timeCooldownSkill[0] <= 0 && !skillManager.Skills[0].SilenceSkill)
                {
                    if (!ListTargetSkill1.Contains(_target))
                    {
                        /**/ListTarget.Add(_target);
                        ListTargetSkill1.Add(_target);
                        effect.TargetEffect(_target);
                        //print("Obj:" + _target.name + " Add na list(Skill1)");
                    }
                }
                //if (Add && _target != null && _target.GetComponent<MobManager>() != null && skillManager.Skills[listSkill-1].NeedTarget)
                //{
                //    if (!skillManager.Skills[listSkill - 1].TargetFriend &&  _target.GetComponent<MobManager>().MesmoTime(mobManager.TimeMob) ||
                //         skillManager.Skills[listSkill - 1].TargetFriend && !_target.GetComponent<MobManager>().MesmoTime(mobManager.TimeMob))
                //    {
                //        TargetAttack(false, _target, listSkill);
                //    }
                //}
                if (!Add && _target != null)
                {
                    if (ListTargetSkill1.Contains(_target))
                    {
                        ListTargetSkill1.Remove(_target);
                        //print("Obj:" + _target.name + " <b>Remove</b> na list(Skill1)");
                    }
                }
                break;
            #endregion

            #region list skill2
            case 2:
                if (Add && _target != null && mobCooldown.timeCooldownSkill[1] <= 0 && !skillManager.Skills[1].SilenceSkill)
                {
                    if (!ListTargetSkill2.Contains(_target))
                    {
                        /**/ListTarget.Add(_target);
                        ListTargetSkill2.Add(_target);
                        effect.TargetEffect(_target);
                        //print("Obj:" + _target.name + " Add na list(Skill2)");
                    }
                }
                //if (Add && _target != null && _target.GetComponent<MobManager>() != null && skillManager.Skills[listSkill - 1].NeedTarget)
                //{
                //    if (!skillManager.Skills[listSkill - 1].TargetFriend &&  _target.GetComponent<MobManager>().MesmoTime(mobManager.TimeMob) ||
                //         skillManager.Skills[listSkill - 1].TargetFriend && !_target.GetComponent<MobManager>().MesmoTime(mobManager.TimeMob))
                //    {
                //        TargetAttack(false, _target, listSkill);
                //    }
                //}
                if (!Add && _target != null)
                {
                    if (ListTargetSkill2.Contains(_target))
                    {
                        ListTargetSkill2.Remove(_target);
                        //print("Obj:" + _target.name + " <b>Remove</b> na list(Skill2)");
                    }
                }
                break;
            #endregion

            #region list skill3
            case 3:
                if (Add && _target != null && mobCooldown.timeCooldownSkill[2] <= 0 && !skillManager.Skills[1].SilenceSkill)
                {
                    if (!ListTargetSkill3.Contains(_target))
                    {
                        /**/ListTarget.Add(_target);

                        ListTargetSkill3.Add(_target);
                        effect.TargetEffect(_target);
                       // print("Obj:" + _target.name + " Add na list(Skill3)");
                    }
                }
                //if (Add && _target != null && _target.GetComponent<MobManager>() != null && skillManager.Skills[listSkill - 1].NeedTarget)
                //{
                //    if (!skillManager.Skills[listSkill - 1].TargetFriend &&  _target.GetComponent<MobManager>().MesmoTime(mobManager.TimeMob) ||
                //         skillManager.Skills[listSkill - 1].TargetFriend && !_target.GetComponent<MobManager>().MesmoTime(mobManager.TimeMob))
                //    {
                //        TargetAttack(false, _target, listSkill);
                //    }
                //}
                if (!Add && _target != null)
                {
                    if (ListTargetSkill3.Contains(_target))
                    {
                        ListTargetSkill3.Remove(_target);
                        //print("Obj:" + _target.name + " <b>Remove</b> na list(Skill3)");
                    }
                }
                break;
                #endregion
        }

        #region list
        if (!mobManager.isPlayer)
            if (Add && _target != null)
            {
                if (!ListTarget.Contains(_target))
                {
                    ListTarget.Add(_target);
                    target = _target;
                    //print("Obj:" + _target.name + " Add na list(<b>Geral</b>)\n target: "+target);
                    return _target;
                }
            }

        if (!Add && _target != null)
        {
            if (ListTarget.Contains(_target))
            {
                ListTarget.Remove(_target);
                //print("Obj:" + _target.name + " <b>Remove</b> na list(<b>Geral</b>)");
            }
        }
        #endregion

        #region Clear list
        if (!Add && _target == null && listSkill==-1)
        {
            ListTargetSkill1.Clear();
            canSkill1 = false;
            //  Debug.LogError("Lista Target Skill1 "+transform.name+" Reset!!");

            ListTargetSkill2.Clear();
            canSkill2 = false;
            //  Debug.LogError("Lista Target Skill2 "+transform.name+" Reset!!");

            ListTargetSkill3.Clear();
            canSkill3 = false;
            //  Debug.LogError("Lista Target Skill3 "+transform.name+" Reset!!");

            if (effect!=null)
            effect.TargetReset();

            ListTarget.Clear();
            target = null;
        }
        #endregion

        return null;
    }//Add inimigos proximos na list

    public void AttDistanceSkill()
    {
        //mobAttack.CheckMoreDistanceSkill();

        // moreDistanceSkill = mobAttack.moreDistanceSkill;

        moreDistanceSkill = skillManager.CheckMoreDistanceSkill();
    }

    public bool CheckDistance(int useSkill = 0, bool skill = true)
    {
        //print("Check Distance "+ useSkill);
        checkTarget = false;
        CheckGrid check = (CheckGrid)FindObjectOfType(typeof(CheckGrid));

        int range    = 0,
            hexagonX = this.GetComponent<MoveController>().hexagonX,
            hexagonY = this.GetComponent<MoveController>().hexagonY;

        AttDistanceSkill();

        if (skill)
        {
            if (useSkill <= 0)
            {
                range = moreDistanceSkill;
            }
            else
                if (skillManager != null && (1 - useSkill) < skillManager.Skills.Count)
                range = skillManager.Skills[useSkill - 1].Range;
            else
                return false;

            //switch (useSkill)
            //{
            //    case 0:
            //        range = moreDistanceSkill;
            //        break;

            //    case 1:
            //        range = mobAttack.distanceSkill1;
            //        break;

            //    case 2:
            //        range = mobAttack.distanceSkill2;
            //        break;

            //    case 3:
            //        range = mobAttack.distanceSkill3;
            //        break;

            //}
        }
        else
            range = useSkill;

        if (range==-1)
        {
            bool friend = skillManager.Skills[useSkill - 1].TargetFriend, 
                 enemy  = skillManager.Skills[useSkill - 1].NeedTarget, 
                 me     = skillManager.Skills[useSkill - 1].TargetMe;

            if (me)
            {
                TargetAttack(true, gameObject, useSkill);

                if (!checkTarget)
                    checkTarget = true;
            }

            List<GameObject> _targets = new List<GameObject>();

            if (friend)
            {
                _targets = TurnSystem.Instance.GetMob(mobManager.TimeMob, false);

                if (_targets.Count > 0)
                {
                    for (int i = 0; i < _targets.Count; i++)
                        TargetAttack(true, _targets[i], useSkill);

                    _targets.Clear();

                    if (!checkTarget)
                        checkTarget = true;
                }               
            }
            if (enemy)
            {
                _targets = TurnSystem.Instance.GetMob(mobManager.TimeMob, true);

                if (_targets.Count > 0)
                {
                    for (int i = 0; i < _targets.Count; i++)
                        TargetAttack(true, _targets[i], useSkill);

                    _targets.Clear();

                    if (!checkTarget)
                        checkTarget = true;
                }
            }        
        }


        if (check.HexGround((hexagonX), hexagonY))
        {
            check.CheckAttack(hexagonX, hexagonY, gameObject, useSkill);

            if (!checkTarget)
                checkTarget = check.CheckAttack(hexagonX, hexagonY, gameObject, useSkill);
        }


        #region Distancia 1
        if (range >= 1)
        {
            #region Range 1
            #region Check Horizontal <->
            if (check.HexGround((hexagonX + 1), hexagonY))
            {
                check.CheckAttack(hexagonX + 1, hexagonY, gameObject, useSkill);
                if (!checkTarget)
                    checkTarget = check.CheckAttack(hexagonX + 1, hexagonY, gameObject, useSkill);
            }

            if (check.HexGround((hexagonX - 1), hexagonY))
            {
                check.CheckAttack(hexagonX - 1, hexagonY, gameObject, useSkill);
                if (!checkTarget)
                    checkTarget = check.CheckAttack(hexagonX - 1, hexagonY, gameObject, useSkill);
            }
            #endregion
            #region vertical^v
            if (check.HexGround((hexagonX), (hexagonY + 1)))
            {
                check.CheckAttack(hexagonX, hexagonY + 1, gameObject, useSkill);
                if (!checkTarget)
                    checkTarget = check.CheckAttack(hexagonX, hexagonY + 1, gameObject, useSkill);
            }

            if (check.HexGround((hexagonX), (hexagonY - 1)))
            {
                check.CheckAttack(hexagonX, hexagonY - 1, gameObject, useSkill);
                if (!checkTarget)
                    checkTarget = check.CheckAttack(hexagonX, hexagonY - 1, gameObject, useSkill);
            }
            #endregion

            #region Check  Diagonal Left 
            if (hexagonY % 2 == 1)//Caso impar
            {
                if (check.HexGround((hexagonX), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 1, gameObject, useSkill);
                }
            }
            if (hexagonY % 2 == 0)//Caso casa par
            {
                if (check.HexGround((hexagonX - 1), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 1, gameObject, useSkill);
                }
            }
            #endregion
            #region check Diagonal Right ->
            if (hexagonY % 2 == 1) //Caso impar
            {
                if (check.HexGround((hexagonX + 1), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 1, gameObject, useSkill);
                }
            }
            if (hexagonY % 2 == 0)  //Caso casa == 0
            {
                if (check.HexGround((hexagonX), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 1, gameObject, useSkill);
                }
            }
            #endregion
            #endregion
        }
        #endregion

        #region Distancia 2
        if (range >= 2)
        {
            if (hexagonY % 2 == 0)//impar
            {
                #region Superior
                //Diagonal Esquerda
                if (check.HexGround((hexagonX - 1), (hexagonY + 2)) != null)
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 2, gameObject, useSkill);
                }
                //Cima
                if (check.HexGround((hexagonX), (hexagonY + 2)) != null)
                {
                    check.CheckAttack(hexagonX, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 2, gameObject, useSkill);
                }
                //Diagonal Direita
                if (check.HexGround((hexagonX + 1), (hexagonY + 2)) != null)
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 2, gameObject, useSkill);
                }
                #endregion

                #region Lado Direito
                //Diagonal Direita lado 
                if (check.HexGround((hexagonX + 1), (hexagonY + 1)) != null)
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 1, gameObject, useSkill);
                }
                //Diagonal Direita lado 
                if (check.HexGround((hexagonX + 2), (hexagonY)) != null)
                {
                    check.CheckAttack(hexagonX + 2, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY, gameObject, useSkill);
                }
                //lado  Direito
                if (check.HexGround((hexagonX + 1), (hexagonY - 1)) != null)
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 1, gameObject, useSkill);
                }
                #endregion

                #region Inferior
                //baixo direito
                if (check.HexGround((hexagonX + 1), (hexagonY - 2)) != null)
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 2, gameObject, useSkill);
                }

                //baixo Meio
                if (check.HexGround((hexagonX + 0), (hexagonY - 2)) != null)
                {
                    check.CheckAttack(hexagonX + 0, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 0, hexagonY - 2, gameObject, useSkill);
                }

                //baixo esquerdo
                if (check.HexGround((hexagonX - 1), (hexagonY - 2)) != null)
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 2, gameObject, useSkill);
                }
                #endregion

                #region Lado Esquerdo
                //Diagonal Esquerda inferior 
                if (check.HexGround((hexagonX - 2), (hexagonY - 1)) != null)
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 1, gameObject, useSkill);
                }
                //Diagonal Direita 
                if (check.HexGround((hexagonX - 2), (hexagonY)) != null)
                {
                    check.CheckAttack(hexagonX - 2, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY, gameObject, useSkill);
                }
                //lado  Esquerdo superior
                if (check.HexGround((hexagonX - 2), (hexagonY + 1)) != null)
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 1, gameObject, useSkill);
                }
                #endregion
            }
            else
            {
                #region Superior
                //Diagonal Esquerda
                if (check.HexGround((hexagonX - 1), (hexagonY + 2)) != null)
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 2, gameObject, useSkill);
                }
                //Cima
                if (check.HexGround((hexagonX), (hexagonY + 2)) != null)
                {
                    check.CheckAttack(hexagonX, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 2, gameObject, useSkill);
                }
                //Diagonal Direita
                if (check.HexGround((hexagonX + 1), (hexagonY + 2)) != null)
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 2, gameObject, useSkill);
                }
                #endregion

                #region Lado Direito
                //Diagonal Direita superior 
                if (check.HexGround((hexagonX + 2), (hexagonY + 1)) != null)
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 1, gameObject, useSkill);
                }

                //Diagonal Direita meio 
                if (check.HexGround((hexagonX + 2), (hexagonY + 0)) != null)
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 0, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 0, gameObject, useSkill);
                }

                //Diagonal Direita inferior 
                if (check.HexGround((hexagonX + 2), (hexagonY - 1)) != null)
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 1, gameObject, useSkill);
                }
                #endregion

                #region Inferior
                //baixo direito
                if (check.HexGround((hexagonX + 1), (hexagonY - 2)) != null)
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 2, gameObject, useSkill);
                }

                //baixo meio
                if (check.HexGround((hexagonX - 0), (hexagonY - 2)) != null)
                {
                    check.CheckAttack(hexagonX - 0, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 0, hexagonY - 2, gameObject, useSkill);
                }

                //baixo esquerdo
                if (check.HexGround((hexagonX - 1), (hexagonY - 2)) != null)
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 2, gameObject, useSkill);
                }
                #endregion

                #region Lado Esquerdo
                //Diagonal Esquerda inferior 
                if (check.HexGround((hexagonX - 1), (hexagonY - 1)) != null)
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 1, gameObject, useSkill);
                }
                //Diagonal Esquerda meio 
                if (check.HexGround((hexagonX - 2), (hexagonY - 0)) != null)
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 0, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 0, gameObject, useSkill);
                }
                //Diagonal Esquerda superior 
                if (check.HexGround((hexagonX - 1), (hexagonY + 1)) != null)
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 1, gameObject, useSkill);
                }
                #endregion
            }
        }
        #endregion

        #region Distancia 3
        if (range >= 3)
        {
            #region Falhas 3 Par
            if (hexagonY % 2 == 0)//Caso par
            {
                //lefts
                if (check.HexGround((hexagonX - 2), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 1, gameObject, useSkill);
                }


                if (check.HexGround((hexagonX - 3), (hexagonY)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 2, gameObject, useSkill);
                }


                //rights
                if (check.HexGround((hexagonX + 2), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY, gameObject, useSkill);
                }


                if (check.HexGround((hexagonX + 2), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 2, gameObject, useSkill);
                }


                //UP
                if (check.HexGround((hexagonX - 2), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 3, gameObject, useSkill);
                }


                //downs
                if (check.HexGround((hexagonX + 1), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 3, gameObject, useSkill);
                }

            }

            #endregion
            #region Falhar 3 impar
            if (hexagonY % 2 == 1)//Caso Impar
            {
                //up
                if (check.HexGround((hexagonX - 1), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 3, gameObject, useSkill);
                }


                //vertical right up
                if (check.HexGround((hexagonX + 2), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 1, gameObject, useSkill);
                }


                //Right
                if (check.HexGround((hexagonX + 3), (hexagonY)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY, gameObject, useSkill);
                }


                //vertical right down
                if (check.HexGround((hexagonX + 3), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 2, gameObject, useSkill);
                }


                //down
                if (check.HexGround((hexagonX + 2), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 3, gameObject, useSkill);
                }


                //vertical left Down
                if (check.HexGround((hexagonX - 1), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 1, gameObject, useSkill);
                }


                //left
                if (check.HexGround((hexagonX - 3), (hexagonY)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY, gameObject, useSkill);
                }


                //vertical left Up
                if (check.HexGround((hexagonX - 2), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 2, gameObject, useSkill);
                }


            }
            #endregion
        }
        #endregion

        #region Distancia 4
        if (range >= 4)
        {
            #region Falhas 4 Par
            //lefts
            if (hexagonY % 2 == 0)
            {
                if (check.HexGround((hexagonX - 3), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 1, gameObject, useSkill);
                }


                if (check.HexGround((hexagonX - 1), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 2, gameObject, useSkill);
                }


                //rights
                if (check.HexGround((hexagonX + 4), (hexagonY)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 4, gameObject, useSkill);
                }


                if (check.HexGround((hexagonX + 3), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 3, gameObject, useSkill);
                }


                //UP
                if (check.HexGround((hexagonX - 3), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 4, gameObject, useSkill);
                }


                //downs
                if (check.HexGround((hexagonX - 2), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 4, gameObject, useSkill);
                }

            }
            #endregion
            #region Falhar 4 Impar
            if (hexagonY % 2 == 1)//Caso Impar
            {
                //up
                if (check.HexGround((hexagonX - 2), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 4, gameObject, useSkill);
                }


                //verical right up
                if (check.HexGround((hexagonX + 3), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 1, gameObject, useSkill);

                }

                //right
                if (check.HexGround((hexagonX + 4), (hexagonY)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY, gameObject, useSkill);
                }


                //vertical right down
                if (check.HexGround((hexagonX + 4), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 3, gameObject, useSkill);
                }


                //down
                if (check.HexGround((hexagonX + 2), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 4, gameObject, useSkill);
                }


                //vertical left down
                if (check.HexGround((hexagonX - 2), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 1, gameObject, useSkill);
                }


                //left
                if (check.HexGround((hexagonX - 4), (hexagonY)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY, gameObject, useSkill);
                }


                //vertical left up
                if (check.HexGround((hexagonX - 3), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 3, gameObject, useSkill);
                }



            }
            #endregion
        }
        #endregion

        #region Distancia  5
        if (range >= 5)
        {
            #region Falhas 5 Par
            if (hexagonY % 2 == 0)//Caso par
            {
                //Down
                if (check.HexGround((hexagonX + 1), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 5, gameObject, useSkill);
                }


                //UP
                if (check.HexGround((hexagonX - 2), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 5, gameObject, useSkill);
                }


                //vertical right up
                if (check.HexGround((hexagonX + 3), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 1, gameObject, useSkill);
                }


                //right 
                if (check.HexGround((hexagonX + 5), (hexagonY)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY, gameObject, useSkill);
                }


                //vertical right down
                if (check.HexGround((hexagonX + 4), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 5, gameObject, useSkill);
                }


                //vertical left up
                if (check.HexGround((hexagonX - 3), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 5), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY + 1, gameObject, useSkill);
                }


                //left
                if (check.HexGround((hexagonX - 5), (hexagonY)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY, gameObject, useSkill);
                }


                //vertical right down
                if (check.HexGround((hexagonX - 5), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 4, gameObject, useSkill);
                }

            }
            #endregion
            #region Falhas 5 Impar
            if (hexagonY % 2 == 1)//Caso Impar
            {
                //up
                if (check.HexGround((hexagonX - 2), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 5, gameObject, useSkill);
                }


                //vertical left up
                if (check.HexGround((hexagonX + 3), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 5), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY + 1, gameObject, useSkill);
                }


                //left
                if (check.HexGround((hexagonX + 5), (hexagonY)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY, gameObject, useSkill);
                }


                //vertical left down
                if (check.HexGround((hexagonX + 5), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 4, gameObject, useSkill);
                }


                //down
                if (check.HexGround((hexagonX + 3), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 5, gameObject, useSkill);
                }


                //vertical left Down
                if (check.HexGround((hexagonX - 3), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 1, gameObject, useSkill);
                }


                //left
                if (check.HexGround((hexagonX - 5), (hexagonY)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY, gameObject, useSkill);
                }


                //vertical Left Up
                if (check.HexGround((hexagonX - 4), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 4, gameObject, useSkill);
                }


            }
            #endregion
        }
        #endregion

        #region Distancia  6
        if (range >= 6)
        {
            #region Falhas 6 Par
            if (hexagonY % 2 == 0)//Caso par
            {
                //Down
                if (check.HexGround((hexagonX + 2), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 6, gameObject, useSkill);
                }


                //up
                if (check.HexGround((hexagonX - 3), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 6, gameObject, useSkill);
                }


                //vertical right up
                if (check.HexGround((hexagonX + 3), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 5), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 5), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY + 1, gameObject, useSkill);
                }


                //right 
                if (check.HexGround((hexagonX + 6), (hexagonY)))
                {
                    check.CheckAttack(hexagonX + 6, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 6, hexagonY, gameObject, useSkill);
                }


                //vertical right down
                if (check.HexGround((hexagonX + 5), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 5), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 3), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 6, gameObject, useSkill);
                }


                //vertical left down
                if (check.HexGround((hexagonX - 4), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 5), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 5), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 6), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 6, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 6, hexagonY - 1, gameObject, useSkill);
                }


                //left
                if (check.HexGround((hexagonX - 6), (hexagonY)))
                {
                    check.CheckAttack(hexagonX - 6, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 6, hexagonY, gameObject, useSkill);
                }


                //vertical left Up
                if (check.HexGround((hexagonX - 6), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 6, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 6, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 5), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 5), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 5, gameObject, useSkill);
                }

            }
            #endregion
            #region Falhas 6 Impar
            if (hexagonY % 2 == 1)//Caso Impar
            {
                //up
                if (check.HexGround((hexagonX - 2), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY + 6, gameObject, useSkill);
                }


                //vertical right up
                if (check.HexGround((hexagonX + 3), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY + 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 5), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 5), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 6), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX + 6, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 6, hexagonY + 1, gameObject, useSkill);
                }


                //right
                if (check.HexGround((hexagonX + 6), (hexagonY)))
                {
                    check.CheckAttack(hexagonX + 6, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 6, hexagonY, gameObject, useSkill);
                }


                //vertical right down
                if (check.HexGround((hexagonX + 6), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX + 6, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 6, hexagonY - 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 5), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 5), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX + 5, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 5, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 4), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX + 4, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 4, hexagonY - 5, gameObject, useSkill);
                }


                //down
                if (check.HexGround((hexagonX + 3), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX + 3, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 3, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 2), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX + 2, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 2, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX + 1), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX + 1, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX + 1, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 1), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX - 1, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 1, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 2), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX - 2, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 2, hexagonY - 6, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY - 6)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 6, gameObject, useSkill);
                }


                //vertical left down
                if (check.HexGround((hexagonX - 3), (hexagonY - 5)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY - 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY - 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY - 4)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY - 3)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY - 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY - 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 5), (hexagonY - 2)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY - 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY - 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 5), (hexagonY - 1)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY - 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY - 1, gameObject, useSkill);
                }


                //left
                if (check.HexGround((hexagonX - 6), (hexagonY)))
                {
                    check.CheckAttack(hexagonX - 6, hexagonY, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 6, hexagonY, gameObject, useSkill);
                }


                //verical left up
                if (check.HexGround((hexagonX - 5), (hexagonY + 1)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY + 1, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY + 1, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 5), (hexagonY + 2)))
                {
                    check.CheckAttack(hexagonX - 5, hexagonY + 2, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 5, hexagonY + 2, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY + 3)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 3, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 3, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 4), (hexagonY + 4)))
                {
                    check.CheckAttack(hexagonX - 4, hexagonY + 4, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 4, hexagonY + 4, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY + 5)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 5, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 5, gameObject, useSkill);
                }

                if (check.HexGround((hexagonX - 3), (hexagonY + 6)))
                {
                    check.CheckAttack(hexagonX - 3, hexagonY + 6, gameObject, useSkill);
                    if (!checkTarget)
                        checkTarget = check.CheckAttack(hexagonX - 3, hexagonY + 6, gameObject, useSkill);
                }

            }
            #endregion
        }
        #endregion

        return checkTarget;
    }

    /// <summary>
    /// Salva no Hex List
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="range">Distance For Check</param>
    /// <param name="colore">Color Ground</param>
    /// <param name="color">0: White, 1:Blue, 2:Red, 3:Green</param>
    public List<HexManager> RegisterOtherHex(int X = -1, int Y = -1, int range = 1, bool colore = true, int color = 3, bool clearList = false)
    {
        if (X == -1)
            X = GetComponent<MoveController>().hexagonX;

        if (Y == -1)
            Y = GetComponent<MoveController>().hexagonY;

        if (clearList)
            HexList.Clear();

        foreach (var hex in CheckGrid.Instance.RegisterRadioHex(X, Y, range, colore, color))
            HexList.Add(hex);
        

        #region Depois apaga
        //if (GameObject.Find("Hex" + (X) + "x" + Y))
        //{
        //    HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y)).GetComponent<HexManager>());
        //}

        //#region Range 1
        //if (range >= 1)
        //{
        //    #region Check Horizontal <->
        //    if (GameObject.Find("Hex" + (X + 1) + "x" + Y))
        //    {
        //        HexList.Add(GameObject.Find("Hex" + (X+1) + "x" + (Y)).GetComponent<HexManager>());
        //    }

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + Y))
        //    {
        //        HexList.Add(GameObject.Find("Hex" + (X-1) + "x" + (Y)).GetComponent<HexManager>());
        //    }

        //    #endregion
        //    #region vertical^v         
        //    if (GameObject.Find("Hex" + (X) + "x" + (Y + 1)))
        //    {
        //        HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 1)).GetComponent<HexManager>());
        //    }

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)))
        //    {
        //        HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //    }

        //    #endregion

        //    #region Check  Diagonal Left 
        //    if (Y % 2 == 1)//Caso impar
        //    {
        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //    }
        //    if (Y % 2 == 0)//Caso casa par
        //    {
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X-1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X-1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //    }
        //    #endregion
        //    #region check Diagonal Right ->
        //    if (Y % 2 == 1) //Caso impar
        //    {

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X+1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X+1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //    }
        //    else  //Caso casa == 0
        //    {
        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //    }
        //    #endregion
        //}
        //#endregion

        //#region Range 2
        //if (range >= 2)
        //{
        //    if (Y % 2 == 0)//impar
        //    {
        //        #region Superior
        //        //Diagonal Esquerda
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        //Cima
        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 0) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Direita
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Lado Direito
        //        //Diagonal Direita lado 
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Direita lado 
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 0)).GetComponent<HexManager>());
        //        }
        //        //lado  Direito
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y -1)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Inferior
        //        //baixo direito
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        //baixo Meio
        //        if (GameObject.Find("Hex" + (X + 0) + "x" + (Y - 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 0) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        //baixo esquerdo
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Lado Esquerdo
        //        //Diagonal Esquerda inferior 
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Direita 
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 0)).GetComponent<HexManager>());
        //        }
        //        //lado  Esquerdo superior
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }
        //        #endregion
        //    }
        //    else
        //    {
        //        #region Superior
        //        //Diagonal Esquerda
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        //Cima
        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 0) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Direita
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Lado Direito
        //        //Diagonal Direita superior 
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        //Diagonal Direita meio 
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 0)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y+0)).GetComponent<HexManager>());
        //        }

        //        //Diagonal Direita inferior 
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Inferior
        //        //baixo direito
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        //baixo meio
        //        if (GameObject.Find("Hex" + (X + 0) + "x" + (Y - 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 0) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        //baixo esquerdo
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Lado Esquerdo
        //        //Diagonal Esquerda inferior 
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Esquerda meio 
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 0)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 0)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Esquerda superior 
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)) != null)
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }
        //        #endregion
        //    }
        //}
        //#endregion

        //#region Range 3
        //if (range >= 3)
        //{
        //    #region Falhas 3 Par
        //    if (Y % 2 == 0)//Caso par
        //    {
        //        //lefts
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }


        //        //rights
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }


        //        //UP
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }


        //        //downs
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //    }

        //    #endregion
        //    #region Falhar 3 impar
        //    if (Y % 2 == 1)//Caso Impar
        //    {
        //        //up
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }


        //        //vertical right up
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //Right
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }


        //        //down
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }


        //        //vertical left Down
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical left Up
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }


        //    }
        //    #endregion
        //}
        //#endregion

        //#region Range 4
        //if (range >= 4)
        //{
        //    #region Falhas 4 Par
        //    //lefts
        //    if (Y % 2 == 0)
        //    {
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }


        //        //rights
        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }


        //        //UP
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }


        //        //downs
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //    }
        //    #endregion
        //    #region Falhar 4 Impar
        //    if (Y % 2 == 1)//Caso Impar
        //    {
        //        //up
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }


        //        //verical right up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        /*if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }*/

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        //right
        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }


        //        //down
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }


        //        //vertical left down
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical left up
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }
        //    }
        //    #endregion
        //}
        //#endregion

        //#region Range 5
        //if (range >= 5)
        //{
        //    #region Falhas 5 Par
        //    if (Y % 2 == 0)//Caso par
        //    {
        //        //Down
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }


        //        //UP
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }


        //        //vertical right up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //right 
        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }


        //        //vertical left up
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //    }
        //    #endregion
        //    #region Falhas 5 Impar
        //    if (Y % 2 == 1)//Caso Impar
        //    {
        //        //up
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }


        //        //vertical left up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical left down
        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }


        //        //down
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }


        //        //vertical left Down
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical Left Up
        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }
        //    }
        //    #endregion
        //}
        //#endregion

        //#region Range 6
        //if (range >= 6)
        //{
        //    #region Falhas 6 Par
        //    if (Y % 2 == 0)//Caso par
        //    {
        //        //Down
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }


        //        //up
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }


        //        //vertical right up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //right 
        //        if (GameObject.Find("Hex" + (X + 6) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 6) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }


        //        //vertical left down
        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 6) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 6) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 6) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 6) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical left Up
        //        if (GameObject.Find("Hex" + (X - 6) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 6) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //    }
        //    #endregion
        //    #region Falhas 6 Impar
        //    if (Y % 2 == 1)//Caso Impar
        //    {
        //        //up
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        //if (GameObject.Find("Hex" + (X) + "x" + (Y + 6)))
        //        //{
        //        //    HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        //}

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }


        //        //vertical right up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 6) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 6) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //right
        //        if (GameObject.Find("Hex" + (X + 6) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 6) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 6) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 6) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }


        //        //down
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }


        //        //vertical left down
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 6) + "x" + (Y)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 6) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //verical left up
        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 1)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 2)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 3)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 4)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 5)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 6)))
        //        {
        //            HexList.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //    }
        //    #endregion
        //}
        //#endregion        
        #endregion

        return HexList;
    }

    /// <summary>
    ///  NÃO Salva no Hex List
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="range">Distance For Check</param>
    /// <param name="colore">Color Ground</param>
    /// <param name="color">0: White, 1:Blue, 2:Red, 3:Green</param>
    public List<HexManager> RegisterOtherHex(int X = -1, int Y = -1, int range = 1, bool colore = true, int color = 3)
    {
        if (X == -1)
            X = GetComponent<MoveController>().hexagonX;

        if (Y == -1)
            Y = GetComponent<MoveController>().hexagonY;

        List<HexManager> Rh = CheckGrid.Instance.RegisterRadioHex(X, Y, range, colore, color);


        #region Apaga depois
        //if (GameObject.Find("Hex" + (X) + "x" + Y))
        //{
        //    Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y)).GetComponent<HexManager>());
        //}

        //#region Range 1
        //if (range >= 1)
        //{
        //    #region Check Horizontal <->
        //    if (GameObject.Find("Hex" + (X + 1) + "x" + Y))
        //    {
        //        Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y)).GetComponent<HexManager>());
        //    }

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + Y))
        //    {
        //        Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y)).GetComponent<HexManager>());
        //    }

        //    #endregion
        //    #region vertical^v         
        //    if (GameObject.Find("Hex" + (X) + "x" + (Y + 1)))
        //    {
        //        Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 1)).GetComponent<HexManager>());
        //    }

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)))
        //    {
        //        Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //    }

        //    #endregion

        //    #region Check  Diagonal Left 
        //    if (Y % 2 == 1)//Caso impar
        //    {
        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //    }
        //    if (Y % 2 == 0)//Caso casa par
        //    {
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //    }
        //    #endregion
        //    #region check Diagonal Right ->
        //    if (Y % 2 == 1) //Caso impar
        //    {

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //    }
        //    else  //Caso casa == 0
        //    {
        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //    }
        //    #endregion
        //}
        //#endregion

        //#region Range 2
        //if (range >= 2)
        //{
        //    if (Y % 2 == 0)//impar
        //    {
        //        #region Superior
        //        //Diagonal Esquerda
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        //Cima
        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 0) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Direita
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Lado Direito
        //        //Diagonal Direita lado 
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Direita lado 
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 0)).GetComponent<HexManager>());
        //        }
        //        //lado  Direito
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Inferior
        //        //baixo direito
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        //baixo Meio
        //        if (GameObject.Find("Hex" + (X + 0) + "x" + (Y - 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 0) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        //baixo esquerdo
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Lado Esquerdo
        //        //Diagonal Esquerda inferior 
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Direita 
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 0)).GetComponent<HexManager>());
        //        }
        //        //lado  Esquerdo superior
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }
        //        #endregion
        //    }
        //    else
        //    {
        //        #region Superior
        //        //Diagonal Esquerda
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        //Cima
        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 0) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Direita
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Lado Direito
        //        //Diagonal Direita superior 
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        //Diagonal Direita meio 
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 0)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 0)).GetComponent<HexManager>());
        //        }

        //        //Diagonal Direita inferior 
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Inferior
        //        //baixo direito
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        //baixo meio
        //        if (GameObject.Find("Hex" + (X + 0) + "x" + (Y - 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 0) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        //baixo esquerdo
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }
        //        #endregion

        //        #region Lado Esquerdo
        //        //Diagonal Esquerda inferior 
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Esquerda meio 
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 0)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 0)).GetComponent<HexManager>());
        //        }
        //        //Diagonal Esquerda superior 
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)) != null)
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }
        //        #endregion
        //    }
        //}
        //#endregion

        //#region Range 3
        //if (range >= 3)
        //{
        //    #region Falhas 3 Par
        //    if (Y % 2 == 0)//Caso par
        //    {
        //        //lefts
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }


        //        //rights
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }


        //        //UP
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }


        //        //downs
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //    }

        //    #endregion
        //    #region Falhar 3 impar
        //    if (Y % 2 == 1)//Caso Impar
        //    {
        //        //up
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }


        //        //vertical right up
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //Right
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }


        //        //down
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }


        //        //vertical left Down
        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical left Up
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }


        //    }
        //    #endregion
        //}
        //#endregion

        //#region Range 4
        //if (range >= 4)
        //{
        //    #region Falhas 4 Par
        //    //lefts
        //    if (Y % 2 == 0)
        //    {
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }


        //        //rights
        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }


        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }


        //        //UP
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }


        //        //downs
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //    }
        //    #endregion
        //    #region Falhar 4 Impar
        //    if (Y % 2 == 1)//Caso Impar
        //    {
        //        //up
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }


        //        //verical right up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        /*if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }*/

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        //right
        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }


        //        //down
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }


        //        //vertical left down
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical left up
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }
        //    }
        //    #endregion
        //}
        //#endregion

        //#region Range 5
        //if (range >= 5)
        //{
        //    #region Falhas 5 Par
        //    if (Y % 2 == 0)//Caso par
        //    {
        //        //Down
        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }


        //        //UP
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }


        //        //vertical right up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //right 
        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }


        //        //vertical left up
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //    }
        //    #endregion
        //    #region Falhas 5 Impar
        //    if (Y % 2 == 1)//Caso Impar
        //    {
        //        //up
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }


        //        //vertical left up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical left down
        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }


        //        //down
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }


        //        //vertical left Down
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical Left Up
        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }
        //    }
        //    #endregion
        //}
        //#endregion

        //#region Range 6
        //if (range >= 6)
        //{
        //    #region Falhas 6 Par
        //    if (Y % 2 == 0)//Caso par
        //    {
        //        //Down
        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }


        //        //up
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }


        //        //vertical right up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //right 
        //        if (GameObject.Find("Hex" + (X + 6) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 6) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }


        //        //vertical left down
        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 6) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 6) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 6) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 6) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical left Up
        //        if (GameObject.Find("Hex" + (X - 6) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 6) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //    }
        //    #endregion
        //    #region Falhas 6 Impar
        //    if (Y % 2 == 1)//Caso Impar
        //    {
        //        //up
        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        //if (GameObject.Find("Hex" + (X) + "x" + (Y + 6)))
        //        //{
        //        //    Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        //}

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }


        //        //vertical right up
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 6) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 6) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }


        //        //right
        //        if (GameObject.Find("Hex" + (X + 6) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 6) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //vertical right down
        //        if (GameObject.Find("Hex" + (X + 6) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 6) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 5) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 5) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }


        //        //down
        //        if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 6)).GetComponent<HexManager>());
        //        }


        //        //vertical left down
        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y - 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y - 1)).GetComponent<HexManager>());
        //        }


        //        //left
        //        if (GameObject.Find("Hex" + (X - 6) + "x" + (Y)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 6) + "x" + (Y)).GetComponent<HexManager>());
        //        }


        //        //verical left up
        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 1)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 1)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 5) + "x" + (Y + 2)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 5) + "x" + (Y + 2)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 3)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 3)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 4)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 4)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 5)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 5)).GetComponent<HexManager>());
        //        }

        //        if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 6)))
        //        {
        //            Rh.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 6)).GetComponent<HexManager>());
        //        }

        //    }
        //    #endregion
        //}
        //#endregion
    #endregion

        return Rh;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color">0: White, 1:Blue, 2:Red, 3:Green</param>
    public void ColorHex(int color = 3)
    {
        for (int i = 0; i < HexList.Count; i++)
            CheckGrid.Instance.ColorGrid(color, HexList[i].x, HexList[i].y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color">0: White, 1:Blue, 2:Red, 3:Green</param>
    public void ColorHex(List<HexManager> _list,int color = 3)
    {
        for (int i = 0; i < _list.Count; i++)
            CheckGrid.Instance.ColorGrid(color, _list[i].x, _list[i].y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color">0: White, 1:Blue, 2:Red, 3:Green</param>
    public void ColorHex(HexManager _list, int color = 3)
    {
       CheckGrid.Instance.ColorGrid(color, _list.x, _list.y);
    }

    /// <summary>
    /// Tira a cor dos hex da HexList
    /// </summary>
    /// <param name="color"></param>
    public void ColorHexClear()
    {
        for (int i = 0; i < HexList.Count; i++)
            CheckGrid.Instance.ColorGrid(0, HexList[i].x, HexList[i].y);
    }

    /// <summary>
    /// Retirar a Cor dos hex da lista
    /// </summary>
    /// <param name="color"></param>
    public void ColorHexClear(List<HexManager> _list)
    {
        for (int i = 0; i < _list.Count; i++)
            CheckGrid.Instance.ColorGrid(0, _list[i].x, _list[i].y);
    }

    /// <summary>
    /// Retirar a Cor do hex desejado
    /// </summary>
    /// <param name="color"></param>
    public void ColorHexClear(HexManager _list)
    {
        CheckGrid.Instance.ColorGrid(0, _list.x, _list.y);
    }

    public List<HexManager> RegisterOtherHexOnlyFree()
    {
        if (HexList.Count >= 1)
        for (int i = 0; i < HexList.Count; i++)
        {
            if (!HexList[i].free || HexList[i].currentMob != null)
                HexList.Remove(HexList[i]);
        }

        return HexList;
    }

    public bool FindSkill(int skillM)
    {
        skillM++;

        if (skillM == 1 && canSkill1)
            return true;
        else
        if (skillM == 2 && canSkill2)
            return true;
        else 
        if (skillM == 3 && canSkill3)
            return true;

        return false;
    }
}
