using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;


public class XmlMenuInicial : XmlManager
{
    public static XmlMenuInicial Instance;

    public StringDatabase SDB;

    protected override IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        Load();

        yield return base.Start();       

        GameManagerScenes.DelegateChangeLanguageXml += Load;
    }

    void Load()
    {
        if (GameManagerScenes._gms.IsMobile)
        {

            return;
        }

        SDB.list.Clear();

        string language = GameManagerScenes._gms.Language().ToString();

        XmlSerializer serializer = new XmlSerializer(typeof(StringDatabase));
      
        string nameStream = "";

#if UNITY_EDITOR
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Configuracoes/" + _nameXml + ".xml";

#elif UNITY_ANDROID
        nameStream ="jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/" + language + "/Configuracoes/" + _nameXml + ".xml";             
       
        //nameStream = Application.streamingAssetsPath + "/StreamingAssets/Xml/" + language + "/Configuracoes/" + _nameXml + ".xml";

        WWW reader = new WWW(nameStream);

        while (!reader.isDone)
        {
            //wait for the reader to finish downloading        
        }

        MemoryStream streamb = new MemoryStream(reader.bytes);

        if (streamb.CanRead)
        {
            SDB = serializer.Deserialize(streamb) as StringDatabase;
            streamb.Close();
            Debug.LogError("Android LoadXml- MenuInicial");
        }

#elif UNITY_IPHONE
        nameStream =Application.dataPath + "/Raw/StreamingAssets/Xml/" + language + "/Configuracoes/" + _nameXml + ".xml";
#else
      nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Configuracoes/" + _nameXml + ".xml";
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
        FileStream stream = new FileStream(nameStream, FileMode.Open);

        if (stream.CanRead)
        {
            print("String Menu Inicial Carregadas.");
            SDB = serializer.Deserialize(stream) as StringDatabase;
            stream.Close();
        }
        else
        {
            print("Passiva DB Não encontrada!!!");
            return;
        }
#endif

        if (SDB.list.Count > 0)
        {
            foreach (var s in SDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
            }
        }
        else
        {
            GameManagerScenes._gms.NewInfo("Arquivo não encontrado\n" + nameStream, 5);
        }
    }

    public string Get(int ID)
    {
        if (SDB.list.Count > 0)
        {
            if (ID > -1 && ID < SDB.list.Count)
            {
               // print("GET("+ID+") "+ SDB.list[ID]._nameX);
                return CheckAndChangeWords(SDB.list[ID]._nameX);
            }
        }

        string _return = "";

        Load();

        if (SDB.list.Count > 0)
        {
            _return = CheckAndChangeWords(SDB.list[ID]._nameX);
        }

        return _return;
    }

    public string PassiveTranslate(Passive d)
    {
        string r = "";

        switch (d)
        {
            case Passive.ShootSkill: r =Get(224);
                break;

            case Passive.EndSkill: r =Get(225);
                break;

            case Passive.StartSkill:r = Get(226);
                break;

            case Passive.HitSkill: r =Get(227);
                break;

            case Passive.RestoreHp: r =Get(228);
                break;

            case Passive.GetDefense:
                r = Get(229);
                r = GameManagerScenes._gms.AttDescrição(r, "{0}", Get(231), r);
                break;

            case Passive.GetDamage: r =Get(230);
                break;

            case Passive.GetCriticalDamage: r =Get(231);
                break;

            case Passive.DefenseDamage: r =Get(232);
                break;

            case Passive.KillEnemy:r =Get(233);
                break;

            case Passive.SetDamageInEnemy: r =Get(234);
                break;

            case Passive.Kill: r =Get(235);
                break;

            case Passive.EndTurn: r =Get(236);
                break;

            case Passive.EnimyHitSkill: r =Get(237);
                break;

            case Passive.AreaAttack: r =Get(238);
                break;

            case Passive.TargetHitSkill: r =Get(239);
                break;

            case Passive.GetRealDamage: r =Get(240);
                break;

            case Passive.Walk: r =Get(241);
                break;

            case Passive.WalkDbuff: r =Get(242);
                break;

            case Passive.BeforeGetDamage: r =Get(243);
                break;

            case Passive.GetArmor:
                r = Get(229);
                r = GameManagerScenes._gms.AttDescrição(r, "{0}", Get(221), r);
                break;

            case Passive.Criation: r = Get(244);
                break;

            case Passive.SetCriticalDamage: r = Get(245);
                break;

            case Passive.GetDodge: r = Get(246);
                    break;

            case Passive.EnemyDodge: r = Get(247); 
                    break;

            case Passive.DodgeFail: r = Get(248); 
                    break;

            case Passive.EnemyHit: r = Get(249);
                break;

            default:               
                break;
        }

        return r;
    }

    public string DbuffTranslate(Dbuff d,bool color=false)
    {
        switch (d)
        {
            case Dbuff.Fire:
                return color ? "<color=red>"+ Get(122) + "</color>" : Get(122);

            case Dbuff.Envenenar:
                return color ? "<color=green>" + Get(123) + "</color>" : Get(123);

            case Dbuff.Petrificar:
                return color ? "<color=grey>" + Get(124) + "</color>" : Get(124);

            case Dbuff.Stun:
                return color ? "<color=#7E7C0A>" + Get(125) + "</color>" : Get(125);

            case Dbuff.Bleed:
                return color ? "<color=#7E0A0C>" + Get(126) + "</color>" : Get(126);

            case Dbuff.Recuar:
                return Get(127);

            case Dbuff.Chamar:
                return Get(128);

            case Dbuff.Cooldown:
                return Get(129);

            case Dbuff.Recupera_HP:
                return Get(130);

            case Dbuff.Escudo:
                return Get(131);

            case Dbuff.Buff_Atk:
                return Get(192);

            case Dbuff.Silence:
                return Get(195);

            case Dbuff.Dbuff_Atk:
                return Get(197);

            case Dbuff.Armadura:
                return Get(221);

            default:
                return "";
        }
    }

    public string DbuffTranslate(Dbuff d,float chance,float valueMin,float valueMax,int acumule, bool color = false)
    {
        string _d = "";
        string _chance = chance>1 ? chance+"%" : (chance*100)+"%";
        string _value = valueMax == valueMin ? ""+valueMax : valueMin+" - "+valueMax;

        switch (d)
        {
            case Dbuff.Fire:
                {
                    _d = Get(122) + "(" + Get(139);//Fire (Duração: X turno(s).)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", _value, _d);

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return color ? "<color=red>" + _d + "</color>" : _d;
                }

            case Dbuff.Envenenar:
                {
                    _d = Get(123) + "(" + Get(139);//Envenevar (Duração: X turno(s).)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", _value, _d);

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return color ? "<color=green>" + _d + "</color>" : _d;
                }               

            case Dbuff.Petrificar:
                {
                    _d = Get(124) + "(" + Get(139);//Petrificar (Duração: X turno(s).)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", _value, _d);

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return color ? "<color=grey>" + _d + "</color>" : _d;
                }

            case Dbuff.Stun:
                {
                    _d = Get(125) + "(" + Get(139);//Stun (Duração: X turno(s).)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", _value, _d);

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return color ? "<color=#7E7C0A>" + _d + "</color>" : _d;
                }

            case Dbuff.Bleed:
                {
                    _d = Get(126) +"("+Get(139);//Sangrar (Duração: X turno(s).)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", _value, _d);

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return color ? "<color=#7E0A0C>" + _d + "</color>" : _d;
                }

            case Dbuff.Recuar:
                {
                    _d = Get(127)+" ("+ Get(216)+")";//Recuar (Casa(s): X.)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", _value, _d);

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.Chamar:
                {
                    _d = Get(128) + " (" + Get(216) + ")";//Chamar (Casa(s): X.)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", _value, _d);

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.Cooldown:
                {
                    _d = Get(129) + " (" + Get(217) + ")";//Recarga (Skill(s): X,Tempo: X)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", Get(214), _d);

                    string skill = "#"+(valueMin + 1);                

                    _value = valueMax == -2 ? "1" : "-"+valueMax; //-2 Retirar 1 

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{1}", skill, _d);

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{2}", _value, _d);

                    if (acumule > 0)
                    {
                        _d += Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.Recupera_HP:
                {
                    _d = Get(130) + " (" + Get(34) + ")";//Recupera Hp (Efeito: X)
                    
                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "+"+_value, _d);

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.Escudo:
                {
                    _d = Get(131) + " (" + Get(34);//Escudo (Efeito: X)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", _value, _d);

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.Buff_Atk:
                {
                    _d = Get(192) + " (" + Get(35);//Aumenta Dano (Efeito: X,Duração: X)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "+" + valueMin, _d);//Efeito

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{1}",valueMax !=-2 ? "" + valueMax : Get(222)/*Permanente*/, _d);//Duração

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.Silence:
                {
                    _d = Get(195) + " (" + Get(217);//Silencia (X(s): X,Tempo: X)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", Get(214), _d);

                    string skill = "#" + (valueMin + 1);

                    if (valueMin == -2)//-2 Todas 
                    {
                        skill = Get(220);//Todas as {0}(s)

                        skill = GameManagerScenes._gms.AttDescrição(_d, "{0}", Get(214), _d);
                    }
                    else
                   if (valueMin == -3)//-3 Menor Cooldown    
                    {
                        skill = Get(218); // Menor Cooldown
                    }
                    else
                       if (valueMin == -4)//-4 disponiveis  
                    {
                        skill = Get(219);//{0}(s) Ativa(s)
                        skill = GameManagerScenes._gms.AttDescrição(_d, "{0}", Get(214), _d);
                    }

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{1}", skill, _d);

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{2}", ""+valueMax, _d);

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.Dbuff_Atk:
                {
                    _d = Get(197) + " (" + Get(35);//Aumenta Dano (Efeito: X,Duração: X)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "-" + valueMin, _d);//Efeito

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{1}", valueMax != -2 ? "" + valueMax : Get(222)/*Permanente*/, _d);//Duração

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.SilencePassive:
                {
                    _d = Get(195) + " (" + Get(217);//Silencia (X(s): X,Tempo: X)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", Get(67), _d);

                    string skill = "#" + (valueMin + 1);

                    if (valueMin == -2)//-2 Todas 
                    {                
                        skill = Get(220);//Todas as {0}(s)

                        skill = GameManagerScenes._gms.AttDescrição(_d, "{0}", Get(67), _d);
                    }
                    else
                    if (valueMin == -3)//-3 Menor Cooldown    
                    {              
                        skill = Get(218); // Menor Cooldown
                    }
                    else
                        if (valueMin == -4)//-4 disponiveis  
                    {               
                        skill = Get(219);//{0}(s) Ativa(s)
                        skill = GameManagerScenes._gms.AttDescrição(_d, "{0}", Get(67), _d);
                    }

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{1}", skill, _d);

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{2}", "" + valueMax, _d);

                    if (acumule > 0)
                    {
                        _d += "," + Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            case Dbuff.Armadura:
                {
                    _d = Get(221) + " (" + Get(35);//Aumenta Dano (Efeito: X,Duração: X)

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "+" + valueMin, _d);//Efeito

                    _d = GameManagerScenes._gms.AttDescrição(_d, "{1}", valueMax != -2 ? "" + valueMax : Get(222)/*Permanente*/, _d);//Duração

                    if (acumule > 0)
                    {
                        _d += ","+Get(223);
                        _d = GameManagerScenes._gms.AttDescrição(_d, "{0}", "" + acumule, _d);
                    }
                    _d += ")";

                    return /*color ? "<color=#7E0A0C>" + _d + "</color>" :*/ _d;
                }

            default:
                return "";
        }
    }
}
