using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillShootable : MobSkillManager
{
    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        skillAttack.gameObject.transform.position = User.transform.position;

        base.UseSkill();
           
        Debug.LogError(User.name + " usou a skill " + Nome + " no " + target);       

        ShootSkill();                   
    }


}
