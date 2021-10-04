using UnityEngine;

public class DetalSkillToolTip : MonoBehaviour
{
    [TextArea]
    public string skill1, skill2, skill3;

    [HideInInspector]
    public string damageSkill1="0",      damageSkill2 = "0",      damageSkill3 = "0",
                  maxcooldownSkill1, maxcooldownSkill2, maxcooldownSkill3,
                  cooldownSkill1,    cooldownSkill2,    cooldownSkill3;
}
