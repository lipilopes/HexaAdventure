using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

[System.Serializable]
public class SkillXml
{
    // [XmlNamespaceDeclarations()/*XmlArrayItem("_nameX")*/]
    public string _nameX;
    public float  _ID;
    [TextArea]
    // [XmlNamespaceDeclarations()]
    public string _description;
}

[System.Serializable]
public class SkillDatabase
{
    [XmlArray("Skill")]//Nome da tag no XML
    public List<SkillXml> list = new List<SkillXml>();
}

public class XmlMobSkill : XmlManager
{

    public static XmlMobSkill Instance;

    public SkillDatabase SDB;

    protected override IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        //SDB = new PassiveDatabase();

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

        XmlSerializer serializer = new XmlSerializer(typeof(SkillDatabase));        

        string nameStream = "";

#if UNITY_EDITOR
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Skills/" + _nameXml + "." + _typeArq.ToString();
#elif UNITY_ANDROID
        nameStream = "jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/" + language + "/Skills/" + _nameXml + "."+_typeArq.ToString();               
         //nameStream = Application.streamingAssetsPath + "/StreamingAssets/Xml/" + language + "/Skills/" + _nameXml + "." + _typeArq.ToString();

        WWW reader = new WWW(nameStream);

        while (!reader.isDone)
        {
            //wait for the reader to finish downloading        
        }

        MemoryStream streamMs = new MemoryStream(reader.bytes);

        if (streamMs.CanRead)
        {
            SDB = serializer.Deserialize(streamMs) as SkillDatabase;

            streamMs.Close();

            Debug.LogError("Android LoadXml- MobSkill");
        }
#elif UNITY_IPHONE
        nameStream = Application.dataPath + "/Raw/StreamingAssets/Xml/" + language + "/Skills/" + _nameXml + "."+_typeArq.ToString();
#else
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Skills/" + _nameXml + "."+_typeArq.ToString();
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
        FileStream stream = new FileStream(nameStream, FileMode.Open);

        if (stream.CanRead)
        {
            if (SDB.list.Count <= 0)
                SDB = serializer.Deserialize(stream) as SkillDatabase;

            stream.Close();        
        }
        else
        {
            print("Skill DB Não encontrada!!!");
            return;
        }
#endif

        if (SDB.list.Count > 0)
        {
            foreach (var s in SDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._description = CheckAndChangeWords(s._description);
            }

            print("Skills Carregadas.");
        }
        else
        {
            GameManagerScenes._gms.NewInfo("Arquivo não encontrado\n" + nameStream, 5);
        }
    }

    public SkillXml GetSkill(int ID)
    {
        SkillXml _return = null;

        if (SDB.list.Count > 0)
        {
            foreach (var i in SDB.list)
            {
                if (i._ID== ID)
                {
                    return i;
                }
            }

            if (_return != null)
            {
                return _return;
            }
        }

        Load();

        if (SDB.list.Count > 0)
            foreach (var i in SDB.list)
            {
                if (i._ID == ID)
                {
                    return i;
                }
            }

        return _return;
    }

    public string Name(float ID)
    {
        string _return = "";

        if (SDB.list.Count > 0)
        {
            foreach (var i in SDB.list)
            {
                if (i._ID == ID)
                {
                    return i._nameX;
                }
            }

            if (_return != null)
            {
                return _return;
            }
        }

        Load();

        if (SDB.list.Count > 0)
            foreach (var i in SDB.list)
            {
                if (i._ID == ID)
                {
                    return i._nameX;
                }
            }

        return _return;
    }

    public string Description(float ID)
    {
        print("Description("+ID+")");

        string _return = "";

        if (SDB.list.Count > 0)
        {
            foreach (var i in SDB.list)
            {
                if (i._ID == ID)
                {
                    print("Achei (" + ID + ")");
                    _return = i._description;
                    break;
                }
            }

            if (_return != "")
            {
                print("return (" + ID + ")");
                return _return;
            }
        }

        Load();

        if (SDB.list.Count > 0)
            foreach (var i in SDB.list)
            {
                if (i._ID == ID)
                {
                    return i._description;
                }
            }

        return _return;
    }
}
