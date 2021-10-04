using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillMordida : MobSkillBasicDamage
{
    [Space]
    [Header("Mordida")]
    [SerializeField,Tooltip("Caso hp esteja cheio ganha escudo no msm valor")]
    protected bool _getShield;

    //protected override void Start()
    //{
    //    base.Start();
    //}

    protected override void AttDescription()
    {
        base.AttDescription();

        if (!_getShield)
            AttDescription("$P0", "");
        else
            AttDescription("$P0", ",Caso a vida esteja cheia ganha escudo.");
    }

    protected override void DbuffFail(GameObject targetDbuff, int index)
    {
        base.DbuffFail(targetDbuff, index);

        if (_getShield && mobManager.MesmoTime(targetDbuff) && index>=0)
            if (_DbuffBuff[index]._buff == Dbuff.Recupera_HP)
            {
                User.GetComponent<MobHealth>().Defense(currentdamage,User);
            }
    }
}
