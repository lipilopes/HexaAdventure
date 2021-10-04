using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillTransforme
{
    public MobSkillManager _skill;
    public int             _cooldown;
}

[System.Serializable]
public class SaveTransformesInfo
{
    public GameObject            _transforme;
    public List<MobSkillManager> _skill = new List<MobSkillManager>();
}

public class MobSkillTransformacao : MobSkillManager
{
    protected ToolTipType toolTipType;

    [Space, Header("Transformação")]
    [SerializeField,Tooltip("Duração da transformação $P0")]
    protected int _duration;
    protected int _currentDuration;

    protected bool _save=false;
    /// <summary>
    /// Minhas Skills originais
    /// </summary>
    protected List<SkillTransforme> _userSkills = new List<SkillTransforme>();
    /// <summary>
    /// Meu nome original
    /// </summary>
    protected string            _userName;
    /// <summary>
    /// Meu modelo original
    /// </summary>
    protected Mesh              _userMesh;
    /// <summary>
    /// Meu Material original
    /// </summary>
    protected Material[]        _userMaterial;

    protected List<SaveTransformesInfo> _myOldTransformesList = new List<SaveTransformesInfo>();

    IEnumerator FirstSaveInf()
    {
        while (User==null)
        { yield return new WaitForSeconds(5);

        SaveMyInf();

        }
       
    }

    protected override void Start()
    {
        base.Start();

        _duration++;

        if(User!=null)
        toolTipType = User.GetComponent<ToolTipType>();       

        currentdamage = 0;

        StartCoroutine(FirstSaveInf());
    }

    protected override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + _duration + "</b>");
    }

    public override   void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        SaveMyInf();      

        ShootSkill();

        _currentDuration = _duration;       
    }

    public override   void Hit(bool endTurn, GameObject targetDbuff)
    {
        base.Hit(endTurn, targetDbuff);

        if (targetDbuff == Target)
        {
            Transformar(Target);
        }
    }

    protected virtual void SaveMyInf()
    {
        if (_save)
            return;

        _save = true;

        for (int i = 0; i < skillManager.Skills.Count; i++)
        {
            SkillTransforme sT = new SkillTransforme();

            sT._skill    = skillManager.Skills[i];
            sT._cooldown = skillManager.CooldownCurrentSkill(i);

            _userSkills.Add(sT);
        }

        //save in the list
        SaveTransformesInfo _SaveTransformesInfo = new SaveTransformesInfo();
        _SaveTransformesInfo._transforme         = User;
        _SaveTransformesInfo._skill              = skillManager.Skills;
        _myOldTransformesList.Add(_SaveTransformesInfo);

        if (User != null)
        {
            _userName = User.GetComponent<ToolTipType>()._name;
            _userMesh = User.GetComponent<MeshFilter>().mesh;
            _userMaterial = User.GetComponent<MeshRenderer>().materials;
        }
    }

    protected virtual void Destransformar()
    {
        //change back
        toolTipType._name                           = _userName;
        //change back
        User.GetComponent<MeshFilter>().mesh        = _userMesh;
        //change back
        User.GetComponent<MeshRenderer>().materials = _userMaterial;

        //change back
        for (int i = 0; i < _userSkills.Count; i++)
        {
            skillManager.Skills[i]                 = _userSkills[i]._skill;

            mobCooldown.AttCooldown(_userSkills[i]._cooldown, i);
        }

        //clear it
        skillManager.SkillsReserva = null;
    }

    protected virtual void Transformar(GameObject _this)
    {
        //change name
        toolTipType._name                           = _this.GetComponent<ToolTipType>()._name;
        //change mesh
        User.GetComponent<MeshFilter>().mesh        = _this.GetComponent<MeshFilter>().mesh;
        //change materials
        User.GetComponent<MeshRenderer>().materials = _this.GetComponent<MeshRenderer>().materials;

        //save 
        skillManager.SkillsReserva = this;

        if (CheckSalveInfo(_this) == false)
        {
            SkillManager skillM = _this.GetComponent<SkillManager>();

            for (int i = 0; i < skillM.Skills.Count; i++)
            {
                //save my current cooldown
                _userSkills[i]._cooldown = skillManager.Skills[i].CooldownCurrent;

                //prepare prefab for create
                skillManager.PrefabSkills[i] = skillM.PrefabSkills[i];

                //clear de list skill for create
                skillManager.Skills[i] = null;
            }

            //create skills
            skillManager.ActiveSkill();

            //save in the list
            SaveTransformesInfo sT = new SaveTransformesInfo();
            sT._transforme = _this;
            sT._skill      = skillManager.Skills;
            _myOldTransformesList.Add(sT);
        }

        //Randomize the cooldown of new skills
        for (int i = 0; i < skillManager.Skills.Count; i++)
        {
            skillManager.Skills[i].CooldownCurrent = Random.Range(0, skillManager.Skills[i].CooldownMax);
        }

        EffectManager.Instance.PopUpDamageEffect("PUFF", User);
        EffectManager.Instance.PopUpDamageEffect("PUFF", _this);

        //ResetCoolDownManager();
        //mobManager.EndTurn();

        EndSkill();
    }

    public override void EndTurnAttack()
    {
        if (_currentDuration > 0)
        {
            base.EndTurnAttack();

            if (_userSkills.Count > 0)
                for (int i = 0; i < _userSkills.Count; i++)
                {
                    if (_userSkills[i]._skill != this)
                    {
                        _userSkills[i]._skill.EndTurnAttack();

                        _userSkills[i]._cooldown--;

                        if (_userSkills[i]._cooldown < 0)
                            _userSkills[i]._cooldown = 0;
                    }
                }

            _currentDuration--;

            if (_currentDuration == 0)
            {
                EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(132), User);//Acabou

                Destransformar();
            }
            else
                EffectManager.Instance.PopUpDamageEffect((_duration-_currentDuration) + "/" + _duration, User);
        }
    }

    protected virtual bool CheckSalveInfo(GameObject _this)
    {
        foreach (var t in _myOldTransformesList)
        {
            if (t._transforme == _this)
            {
                for (int i = 0; i < t._skill.Count; i++)
                {
                    //save my current cooldown
                    _userSkills[i]._cooldown = skillManager.Skills[i].CooldownCurrent;

                    skillManager.Skills = t._skill;
                }

                return true;
            }
        }

        return false;
    }
}
