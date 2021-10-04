using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBuffWalk : MobSkillManager
{
    [Space]
    [Header("Buff Walk")]
    [SerializeField, Tooltip("Quantidade de passos que pode dar $P0")]
    protected int _buff = 6;

    protected int counter = 0;

    [SerializeField, Tooltip("Efeito q permanece ate o final da skill")]
    protected GameObject _effectWalk;

    WaitForSeconds waitUpdate = new WaitForSeconds(0.25f);

    WaitForSeconds wait = new WaitForSeconds(0.5f);

    protected override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + _buff + "</b>");
    }

    public override    void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        StopAllCoroutines();

        counter = 0;

        //skillAttack.endTurn = false;      

        ShootSkill();

        StartWalk();
    }

    protected override void ShootSkill()
    {
        if (target != null && User != target)
            User.transform.LookAt(target.transform);

        mobManager.ActivePassive(Passive.ShootSkill, target);

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

        if (mobManager.isPlayer)
            EffectManager.Instance.PopUpDamageEffect(mobManager.MesmoTime(RespawMob.Instance.PlayerTime) ? "<color=#055b05>" + Nome + "</color>" : "<color=#962209>" + Nome + "</color>", User);

        CameraOrbit.Instance.ChangeTarget(User);

        //Não mudar
        if (skillAttack != null)
        {
            skillAttack.transform.position = target.transform.position;

            skillAttack.TakeDamage = false;
            skillAttack.UseSkill(target, this);
        }
        else
        {
            //target.GetComponent<MobHealth>().Damage(User, 0, mobManager.chanceCritical);
            Hit(false, null);
        }       

        //ResetCoolDownManager();

        objectSelectTouch = null;
    }

    protected virtual  void StartWalk()
    {
        EffectWalk();
        StartCoroutine(WalkCoroutine());
    }

    protected virtual  IEnumerator WalkCoroutine()
    {
        if (mobManager.isPlayer)
            InfoTable.Instance.NewInfo(
                GameManagerScenes._gms.AttDescriçãoMult(
                    XmlMenuInicial.Instance.Get(154)//Selecione um dos hexas verdes para andar\n<color=green>{0}</color>/{1}
                ,""+(counter + 1),
                ""+_buff),
                99);

        yield return wait;

        while (_buff + 1 > counter)
        {
            CameraOrbit.Instance.ChangeTarget(User);

            RegisterOtherHex(_range: 1, _clearList: true);
            RegisterOtherHexOnlyFree();

            AfterWalk();

            if (counter != 0)
                yield return wait;

            SelectTouch = true;

            while (SelectTouch)
            {
                IAWalk();

                yield return waitUpdate;               
            }

            counter++;

            BeforeWalk();

            //EffectManager.Instance.TargetReset();

            if (counter >= _buff)
            {
                EffectWalk(false);

                if (mobManager.isPlayer)
                    ToolTip.Instance.AttTooltip();

                EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(132), User);//Acabou

                yield return wait;

                //mobManager.ActivePassive(Passive.EndSkill, target);
                EndSkill();
            }
            else
            EffectManager.Instance.PopUpDamageEffect("<color=white>" + counter + "/" + _buff + "</color>", moveController.Solo.gameObject);
        }
    }

    protected virtual void IAWalk()
    {
        if (!mobManager.isPlayer)
        {
            //GameManagerScenes._gms.NewInfo(User.name + "- " + nome + " IAWALK()", 6);

            objectSelectTouch = Target;

            UseTouchSkill();
        }
    }

    protected override void UseTouchSkill()
    {
        base.UseTouchSkill();

        CanWalk();
    }

    protected virtual  bool CanWalk()
    {
        if (counter <= _buff && SelectTouch)
        {
            //GameManagerScenes._gms.NewInfo(User.name + "- " + nome + " CanWalk() - counter <= _buff && SelectTouch", 6);
            if (mobManager.isPlayer)
            {
                if (objectSelectTouch != null)
                {
                    if (objectSelectTouch.GetComponent<HexManager>())
                    {
                        if (objectSelectTouch.GetComponent<HexManager>().free)
                        {
                            if (moveController.Walk(null, objectSelectTouch.GetComponent<HexManager>().x, objectSelectTouch.GetComponent<HexManager>().y, 1, true))
                            {
                                SelectTouch = false;
                                return true;
                            }
                        }
                    }
                }
            }
            else
            //if (mobManager.isPlayer == false)
            {
                //GameManagerScenes._gms.NewInfo(User.name + "- " + nome + " CanWalk() - !mobManager.isPlayer", 6);
                moveController.EnemyWalk(objectSelectTouch.GetComponent<MoveController>(),true);
                SelectTouch = false;
                return true;
            }
        }


        //GameManagerScenes._gms.NewInfo(User.name + "- " + nome + " CanWalk() Return false", 6);
        return false;
    }   

    protected virtual  void AfterWalk()
    {
        objectSelectTouch = null;

        if (mobManager.isPlayer)
        ToolTip.Instance.TargetTooltipCanvas(nome,
               GameManagerScenes._gms.AttDescriçãoMult(
                   XmlMenuInicial.Instance.Get(154)//Selecione um dos hexas verdes para andar\n<color=green>{0}</color>/{1}
                , "" + (counter + 1),
                "" + _buff));
    }

    protected virtual  void BeforeWalk()
    {
        SelectTouch       = false;

        objectSelectTouch = null;
    }

    protected override void RegisterOtherHexOnlyFree()
    {
        if (HexList.Count > 0)
            for (int i = 0; i < HexList.Count; i++)
            {
                if (HexList[i].currentMob != Target)
                    if (!HexList[i].free)
                        HexList.Remove(HexList[i]);
            }
    }

    public override    void Hit(bool endTurn, GameObject targetDbuff)
    {
        if (Target.GetComponent<MobManager>())
            Target.GetComponent<MobManager>().ActivePassive(Passive.EnimyHitSkill, User);

        if (targetDbuff == Target)
            mobManager.ActivePassive(Passive.TargetHitSkill, targetDbuff);

        mobManager.ActivePassive(Passive.HitSkill, targetDbuff);

        int count = _DbuffBuff.Count;

        Debug.LogError(Nome + " - Hit(" + endTurn + ") - " + count);
      
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                CheckDbuff(targetDbuff, i);
            }
        }

        if (endTurn)
        {
            EndSkill();
        }
    }

    protected virtual void EffectWalk(bool active = true,GameObject _target=null)
    {
        if (_effectWalk != null)
        {
            if (_target == null)
                _target = User;

            //_effectWalk.GetComponent<Effects>().target = _target;
            _effectWalk.transform.position = new Vector3(_target.transform.position.x,_effectWalk.transform.position.y,_target.transform.position.z);
            _effectWalk.SetActive(active);
        }
    }

}
