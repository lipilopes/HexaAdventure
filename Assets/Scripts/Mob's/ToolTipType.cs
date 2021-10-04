using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipType : MonoBehaviour
{
    
    public Type _type;
    public enum Type
    {
        Item,
        Mob
    }

    [Tooltip("ID do obj para pegar a tradução")]
    [SerializeField]
    public int _XmlID =-1;
    [Space]
    public string _name;
    [Tooltip("Icone do mob caso mob esteja na DB nao precisa colocar aki")]
    public Sprite _spritePerfil;
    [Tooltip("Icone de fala do mob caso mob esteja na DB nao precisa colocar aki")]
    public Sprite _spritePerfilChat;

    [Space(7), Header("Mob")]
    [HideInInspector] public float _hp;
    [HideInInspector] public float _maxHp,_defense,_armor,_dodge;
    [HideInInspector] public string _walk;
    [Space(7), Header("Mob")]
    public string _classe = null;
    //[HideInInspector]public string _nameSkill1,_nameSkill2,_nameSkill3;
    [HideInInspector] public List<string> _nameSkills = new List<string>();

    [HideInInspector]
    public string _descricao;

    [Space(7), Header("Item"), TextArea, SerializeField]
    string[] descricao;

    bool att = false;

    [Space,Header("Extra"), TextArea]
    public string extraInInfo;

    [HideInInspector]
    public string _passiveDesc;

    public int Count { get { return descricao.Length; } }

    string _classeStatic = "";

    private void Start()
    {
        switch (_type)
        {
            case Type.Item:
                if (_XmlID == -1)
                    break;

                _name = XmlEnviroment.Instance.GetName(_XmlID);

                descricao = XmlEnviroment.Instance.Get(_XmlID);

                extraInInfo = XmlEnviroment.Instance.GetExtraInf(_XmlID);
                break;

            case Type.Mob:
                float idMob = _XmlID;

                if (idMob == -1)               
                     idMob = GameManagerScenes._gms.HeroID(gameObject);

                    if (idMob == -1)
                        break;

                    idMob += GetComponent<MobManager>()._SkinID;

                Debug.LogWarning("Tooltip IDXML:" + idMob);

                if (GetComponent<MobManager>()._SkinID != 0)
                    _name = XmlMobManager.Instance.Name(idMob);
                else
                    _name = XmlMobManager.Instance.Name((int)idMob);

                    _classeStatic = XmlMobManager.Instance.Class((int)idMob);

                    if (_classe == "")
                        _classe = _classeStatic;                      
                break;
        }
    }

    public void AttToltip()
    {
        switch (_type)
        {
            #region Item
            case Type.Item:
                if (descricao.Length > 1)
                    _descricao = descricao[Random.Range(0, descricao.Length)];
                else
                if (descricao.Length == 1)
                    _descricao = descricao[0];
                return;
            #endregion

            #region Mob
            case Type.Mob:

                #region Hp / Def
                if (GetComponent<MobHealth>() != null)
                {
                    _hp = GetComponent<MobHealth>().Health;
                    _maxHp = GetComponent<MobHealth>().MaxHealth;

                    _defense = GetComponent<MobHealth>().defense;

                    _armor = GetComponent<MobHealth>().armor;

                    _dodge = GetComponent<MobHealth>().dodge;
                }
                #endregion

                #region Classe / walk
                if (GetComponent<MobManager>() != null)
                {
                    _classe = GameManagerScenes.BattleMode ? "" : _classeStatic;
                   
                    if (RespawMob.Instance != null && RespawMob.Instance.Player == gameObject)
                        _classe += (_classe == "" ? "" : " - ") + GetComponent<MobManager>().TimeMob;
                    else
                    if (RespawMob.Instance != null && RespawMob.Instance.Player != gameObject)
                        _classe += (_classe == "" ? "" : " - ") + (GetComponent<MobManager>().MesmoTime(RespawMob.Instance.Player) ? "<color=green>Aliado</color>" : GetComponent<MobManager>().TimeMob.ToString());
                    else
                        _classe += (_classe == "" ? "" : " - ") + GetComponent<MobManager>().TimeMob;

                    _walk = GetComponent<MobManager>().maxTimeWalk.ToString();
                }
                #endregion

                #region Skill's
                //if (GetComponent<SkillManager>()!=null && 
                //    GetComponent<SkillManager>().Skills[0]!=null && 
                //    GetComponent<SkillManager>().Skills[1]!=null &&
                //    GetComponent<SkillManager>().Skills[2]!=null)
                //{

                //    _nameSkill1 = GetComponent<SkillManager>().Skills[0].Nome;
                //    _nameSkill2 = GetComponent<SkillManager>().Skills[1].Nome;
                //    _nameSkill3 = GetComponent<SkillManager>().Skills[2].Nome;
                //}

                if (GetComponent<SkillManager>() != null)
                    for (int i = 0; i < GetComponent<SkillManager>().Skills.Count; i++)
                        if (GetComponent<SkillManager>().Skills[i] != null)
                            _nameSkills.Add(GetComponent<SkillManager>().Skills[i].Nome);
                #endregion
                return;
                #endregion
        }

        if (!att)
            Att();
    }

    void Att()
    {
        if (_name == null)
            _name = /*tag*/ name;
        else
            name = _name;

        att = true;

        if (tag == "Rec hp")
        {
            bool _is = false;

            GameObject p = RespawMob.Instance.Player;

            string nameP = "", nameSkill = "";

            nameP = p.GetComponent<ToolTipType>()._name;            

            SkillManager skillM = p.GetComponent<SkillManager>();

            if (skillM)
            {
                for (int i = 0; i < skillM.Skills.Count; i++)
                {
                    if (skillM.Skills[i].GetComponent<MobSkillPull>())
                    {
                        _is        = skillM.Skills[i].GetComponent<MobSkillPull>().PullItem;
                        nameSkill += skillM.Skills[i].GetComponent<MobSkillPull>().Nome;
                        break;
                    }
                }
            }          


            if (_is || nameSkill.Length>0)
            {
                descricao[1] = GameManagerScenes._gms.AttDescriçãoMult(
                  XmlMenuInicial.Instance.Get(160) //{0} Consegue Pegar usando seu/sua  Skill {1}
                    , nameP,
                    nameSkill);

            }
            else        
            descricao[1] = descricao[0];
        }

        for (int i = 0; i < descricao.Length; i++)
        {
            if (descricao[i].Contains("N%"))
            {
                string _old = descricao[i];
                string _new = _name;

                descricao[i] = _old.Replace("N%", _new);
            }
        }
    }

    public void AttDescrição(int _index, string change, string to)
    {
        if (descricao.Length <= 0)
            return;        

            if (_index < 0 || _index > descricao.Length)
                return;

            if (descricao[_index] == "")
                return;

            print("AttDescrição(" + _index + " / " + descricao.Length + ", <color=red><b>" + change + "</b></color>  ,<color=green><b>" + to + "</b></color>)");

            descricao[_index] = GameManagerScenes._gms.AttDescrição(descricao[_index], change, to, descricao[_index]);        
    }

    public void AttExtraDescrição(string change, string to)
    {
        if (extraInInfo.Length == 0)
            return;

        extraInInfo = GameManagerScenes._gms.AttDescrição(extraInInfo, change, to, extraInInfo);
    }
}
