using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillBasicDamage : MobSkillManager
{
    [Space]
    [Header("Basic Damage")]
    [SerializeField,Tooltip("Skill Fica na minha posição")]
    protected bool inMyPosition = false;
    [Space]
    [SerializeField, Tooltip("Tempo em segundos usando a skill")]
    protected float waitTimeShootSkill = 0;

    protected WaitForSeconds wait;

    protected bool _endturn = false;

    protected override void Start()
    {
        base.Start();



        wait = new WaitForSeconds(waitTimeShootSkill == 0 ? 0.01f  : waitTimeShootSkill);
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        if (skillAttack != null)
            skillAttack.positionWho = inMyPosition;

        if (target != null)
            Debug.LogWarning(User.name + " usou a skill " + Nome + " no " + target);

        ShootSkill();
    }

    protected virtual IEnumerator TimerCoroutine()
    {
        yield return wait;

        //mobManager.ActivePassive(Passive.EndSkill, Target);

        Debug.LogWarning("TimerCoroutine");


        EndSkill();
    }

    protected override void ShootSkill()
    {
        if (target!=null &&  User != target)
        User.transform.LookAt(target.transform);

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

        mobManager.ActivePassive(Passive.ShootSkill, target);

        if (GameManagerScenes._gms.Adm)
        {
            EffectManager.Instance.PopUpDamageEffect(mobManager.MesmoTime(RespawMob.Instance.PlayerTime) ? "<color=#055b05>" + Nome + "</color>" : "<color=#962209>" + Nome + "</color>", User);
        }

        CameraOrbit.Instance.ChangeTarget(Target);

        //Não mudar
        if (skillAttack != null)
        {
            skillAttack.transform.position = inMyPosition ? User.transform.position : target.transform.position;
          
            skillAttack.UseSkill(target,this);
        }
        else
        {
            target.GetComponent<MobHealth>().Damage(User, CurrentDamage, mobManager.chanceCritical);
            Hit(true,null);
        }       

        //ResetCoolDownManager();
    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        endTurn = waitTimeShootSkill < 0.01f;

        Debug.LogError(name+" HIT("+endTurn+","+targetDbuff+")");

      //  if (GameManagerScenes._gms)
      //      GameManagerScenes._gms.NewInfo(name + " HIT(" + endTurn + "," + targetDbuff + ")", 4,true);

            _endturn = endTurn;

        base.Hit(endTurn, targetDbuff);

        if (!endTurn)
        StartCoroutine(TimerCoroutine());
    }
}
