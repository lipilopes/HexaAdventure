using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillAreaDamageFollow : MobSkillAreaDamage
{
    [Space]
    [Header("Area Damage Follow")]
    [SerializeField]
    protected int hexAtual;
    [Space]
    [SerializeField,Range(0,6),Tooltip("Distancia para seguir alvo , 0 para mapa todo")]
    protected int _rangeFollow;

    public override void RespawSkill(GameObject user, GameObject target, HexManager _hex)
    {
        base.RespawSkill(user, target, _hex);

        hexAtual = 0;
    }
    public override void RespawSkill(GameObject user, GameObject target, HexManager _hex, int damage, int walkDamage, int maxHit, float chanceHit, int time, float critico)
    {
        base.RespawSkill(user, target, _hex, damage, walkDamage, maxHit, chanceHit, time, critico);

        hexAtual = 0;
    }

    public override void Damage(MobHealth target)
    {
        ChangeHex(target.GetComponent<MoveController>().Solo);

        base.Damage(target);
    }

    public override void Attack(bool desative = true)
    {       
        hex[hexAtual].currentItem = null;
        hex[hexAtual].puxeItem    = false;

        for (int i = 0; i < hex.Count; i++)
        {
            if (hex[i] != null)
                if (hex[i].currentMob != null)
                {
                    if (hex[i].currentMob.GetComponent<MobHealth>() != null)
                    {
                        if (hex[i].currentMob.GetComponent<MobHealth>().Alive)
                        {
                            if (hex[i].currentMob == _target && _target.GetComponent<MobHealth>().Alive)
                            {
                                Debug.LogError(name + " achou o target " + _target.name);

                                hexAtual = i;

                                hex[i].currentItem = gameObject;

                                hex[i].puxeItem = false;

                                Damage(_target.GetComponent<MobHealth>());
                                break;
                            }
                            else
                                if (!hex[i].currentMob.GetComponent<MobManager>().MesmoTime(MyTime))
                            {
                                Debug.LogError(name + " achou outro " + hex[i].currentMob.name);

                                _target = hex[i].currentMob;

                                //hexAtual            = i;

                                //hex[i].currentItem = gameObject;

                                //hex[i].puxeItem    = false;

                                Damage(_target.GetComponent<MobHealth>());
                                break;
                            }
                        }
                    }
                }
        }

        if (hex[hexAtual].currentMob != _target)
        {
            if (FindTarget())
                return;
        }

        if (desative)
            TimerDesactive();
    }

    protected override void RegisterOtherHex(HexManager _hex)
    {
        #region Register Verdadeiro
        //int X = _hex.x, Y= _hex.y;

        //if (hex[hexAtual].GetComponent<HexManager>().y % 2 == 1) //impar
        //{
        //    #region Volta 1
        //    if (GameObject.Find("Hex" + (X + 1) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //    #endregion

        //    #region Volta 2
        //    if (GameObject.Find("Hex" + (X + 2) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //    #endregion

        //    #region Falha na Volta 2
        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)).GetComponent<HexManager>());
        //    #endregion

        //    #region Volta 3
        //    if (GameObject.Find("Hex" + (X + 3) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 3) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 3) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 3)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //    #endregion           

        //    #region Falhas na Volta 3
        //    if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y - 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 4) + "x" + (Y)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 4) + "x" + (Y)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 3) + "x" + (Y)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + (Y)).GetComponent<HexManager>());
        //    #endregion
        //}
        //else
        //{
        //    #region volta 1
        //    if (GameObject.Find("Hex" + (X + 1) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 1)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 1)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 1)).GetComponent<HexManager>());
        //    #endregion

        //    #region volta 2
        //    if (GameObject.Find("Hex" + (X + 2) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 2)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 2)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 2)).GetComponent<HexManager>());
        //    #endregion

        //    #region Falha na volta 2
        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 2)).GetComponent<HexManager>());


        //    #endregion

        //    #region volta 3
        //    if (GameObject.Find("Hex" + (X + 3) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 3) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 3) + "x" + Y) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 3) + "x" + Y).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y + 3)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X) + "x" + (Y - 3)).GetComponent<HexManager>());
        //    if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 3)).GetComponent<HexManager>());
        //    #endregion

        //    #region Falha na volta 3
        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 1) + "x" + (Y - 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X + 2) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 1) + "x" + (Y - 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 2) + "x" + (Y - 3)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 1)).GetComponent<HexManager>());


        //    //4
        //    if (GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y + 2)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 4) + "x" + (Y)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y)).GetComponent<HexManager>());


        //    if (GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y - 1)).GetComponent<HexManager>());

        //    if (GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 4) + "x" + (Y + 1)).GetComponent<HexManager>());


        //    if (GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)) != null)
        //        hex.Add(GameObject.Find("Hex" + (X - 3) + "x" + (Y - 2)).GetComponent<HexManager>());
        //    #endregion
        //}
        #endregion

        base.RegisterOtherHex(hex[hexAtual].GetComponent<HexManager>());
    }

    protected override void RegisterInAllHex()
    {   
           
    }

    void ChangeHex(HexManager _new)
    {
        hex[hexAtual].currentItem = null;
        hex[hexAtual].puxeItem    = false;

        hex.Clear();

        hex.Add(_new);

        hexAtual = 0;

        hex[hexAtual].currentItem = gameObject;
        hex[hexAtual].puxeItem    = false;

        transform.position = hex[hexAtual].transform.position;
        RegisterOtherHex(_new);

        Solo = hex[hexAtual];
    }

    protected virtual bool FindTarget()
    {
        EnemyAttack e = User.GetComponent<EnemyAttack>();

        if (e!=null && _rangeFollow>0)
        {
            List<HexManager> _findT = new List<HexManager>();
            _findT                  = e.RegisterOtherHex(hex[hexAtual].x, hex[hexAtual].y,_rangeFollow,true,2);

            foreach (var h in _findT)
            {
                if (h.currentMob == _target)
                {
                    ChangeHex(h);
                    return true;
                }
            }
        }
        else
            if (_rangeFollow == 0 && _target!=null)
        {
            ChangeHex(_target.GetComponent<MoveController>().Solo);
            return true;
        }

        return false;
    }
}
