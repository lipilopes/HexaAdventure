using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillFuracao : MobSkillBasicDamage
{
    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        transform.LookAt(target.transform);
        
        CameraOrbit.Instance.ChangeTarget(User);
        CameraOrbit.Instance.MaxOrbitCamera();

        iTween.ShakeRotation(Camera.main.gameObject, iTween.Hash("z", 0.7f, "x", 0.8f, "time", 3 + 0.25f, "easetype", iTween.EaseType.easeOutBounce));           
    }

    protected override void ShootSkill()
    {
        base.ShootSkill();

        skillAttack.transform.position = User.transform.position;

        MoveHits();

        //ResetCoolDownManager();
    }

    protected void MoveHits()
    {
        RespawMob turn = RespawMob.Instance;

        int Xx = 0
          , Yy = 0;

        List<GameObject> ChangeList = new List<GameObject>();

        for (int i = 0; i < turn.allRespaws.Count; i++)
        {
            if (turn.allRespaws[i].GetComponent<MoveController>() || turn.allRespaws[i].GetComponent<ItemRecHp>())
            {
                if (ChangeList.Contains(turn.allRespaws[i]) == false)
                {
                    if (turn.allRespaws[i].GetComponent<MoveController>() != null)
                    {
                        if (Random.Range(0, 1) == 1)
                            Xx = turn.allRespaws[i].GetComponent<MoveController>().hexagonX + Random.Range(0, 1);
                        else
                            Xx = turn.allRespaws[i].GetComponent<MoveController>().hexagonX - Random.Range(0, 1);

                        if (Random.Range(0, 1) == 1)
                            Yy = turn.allRespaws[i].GetComponent<MoveController>().hexagonY + Random.Range(0, 1);
                        else
                            Yy = turn.allRespaws[i].GetComponent<MoveController>().hexagonY - Random.Range(0, 1);
                    }
                    else
                    if (turn.allRespaws[i].GetComponent<ItemRecHp>() != null)
                    {
                        if (turn.allRespaws[i].GetComponent<ItemRecHp>().Here != null)
                        {
                            if (Random.Range(0, 1) == 1)
                                Xx = turn.allRespaws[i].GetComponent<ItemRecHp>().Here.x + Random.Range(0, 3);
                            else
                                Xx = turn.allRespaws[i].GetComponent<ItemRecHp>().Here.x - Random.Range(0, 3);

                            if (Random.Range(0, 1) == 1)
                                Yy = turn.allRespaws[i].GetComponent<ItemRecHp>().Here.y + Random.Range(0, 3);
                            else
                                Yy = turn.allRespaws[i].GetComponent<ItemRecHp>().Here.y - Random.Range(0, 3);
                        }
                    }

                    HexManager Here = CheckGrid.Instance.HexGround(Yy, Xx);

                    if (Here && turn.GetComponent<MoveController>() ||
                        Here && turn.allRespaws[i].GetComponent<MobHealth>())
                    {
                        if (turn.allRespaws[i].GetComponent<MoveController>() != null && turn.allRespaws[i] != User)
                        {
                            if (turn.allRespaws[i].GetComponent<MoveController>().Walk(Here.gameObject, Yy, Xx, 1, true))
                                ChangeList.Add(turn.allRespaws[i]);

                            if (turn.allRespaws[i] == RespawMob.Instance.Player)
                                turn.allRespaws[i].transform.LookAt(RespawMob.Instance.Player.transform);
                        }


                        if (turn.allRespaws[i].GetComponent<ItemRecHp>() != null)
                            turn.allRespaws[i].GetComponent<ItemRecHp>().AttPosition(Here.GetComponent<HexManager>());
                    }

                    if (!mobManager.MesmoTime(turn.allRespaws[i]) &&
                        turn.allRespaws[i].GetComponent<MobHealth>())
                    {
                        turn.allRespaws[i].GetComponent<MobHealth>().Damage(User, currentdamage, mobManager.chanceCritical);
                    }
                }             
            }
        }        
    }
}
