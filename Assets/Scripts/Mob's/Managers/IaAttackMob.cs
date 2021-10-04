using UnityEngine;

public class IaAttackMob : MonoBehaviour
{
    SkillManager skill;
    EnemyAttack enemyAttack;
    GameManagerScenes gms;

    void Start()
    {       
        enemyAttack = GetComponent<EnemyAttack>();
        gms         = GameManagerScenes._gms;
        skill       = GetComponent<SkillManager>();
    }

    public void Attack()
    {
        if (GetComponent<MobManager>().attackTurn)
        {
            if (enemyAttack.target!=null)
            transform.LookAt(enemyAttack.target.transform);

            bool silence = false;

            if (skill)
            {
                for (int i = 0; i < skill.sequenceSkill.Length; i++)
                {
                    if (!skill.Skills[skill.sequenceSkill[i]].SilenceSkill)
                    {
                        if (enemyAttack.FindSkill(skill.sequenceSkill[i]))
                        {
                            Debug.LogError(name + " Attack[" + skill.sequenceSkill[i] + "] - SkillManager in " + enemyAttack.target);
                            skill.UseSkill(skill.sequenceSkill[i]);
                            skill.Skills[skill.sequenceSkill[i]].Target = enemyAttack.target;
                            return;
                        }
                    }
                    else
                        silence = true;
                }
            }

            if(silence)
            EffectManager.Instance.PopUpDamageEffect("<color=black>"+XmlMenuInicial.Instance.Get(196)+"</color>",gameObject);

            GetComponent<MobManager>().EndAttackTurn();
        }
    }

    public void AttAttack()
    {
        if (skill)
        {
            skill.ActiveSkill();
            return;
        }
    }

    public void Skill1()
    {
        Debug.LogError(name+" Use Skill1");

        if (skill != null)
        {
            if(gms.Adm)
            InfoTable.Instance.NewInfo(GetComponent<ToolTipType>()._name + " usou a skill" + skill.Skills[0].name + " no " + enemyAttack.target, 5);

            skill.UseSkill(0);       
            return;
        }
    }

    public void Skill2()
    {
        Debug.LogError(name + " Use Skill2");
        if (skill != null)
        {
            if (gms.Adm)
                InfoTable.Instance.NewInfo(GetComponent<ToolTipType>()._name + " usou a skill" + skill.Skills[1].name + " no " + enemyAttack.target, 5);

            skill.UseSkill(1);
            return;
        }
    }

    public void Skill3()
    {
        Debug.LogError(name + " Use Skill3");
        if (skill != null)
        {
            if (gms.Adm)
                InfoTable.Instance.NewInfo(GetComponent<ToolTipType>()._name + " usou a skill" + skill.Skills[2].name + " no " + enemyAttack.target, 5);
            skill.UseSkill(2);
            return;
        }
    }
}


