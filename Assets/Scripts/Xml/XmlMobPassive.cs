using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

[System.Serializable]
public class PassiveXml
{
   // [XmlNamespaceDeclarations()/*XmlArrayItem("_nameX")*/]
    public string _nameX;
    //public int    _id;
    [TextArea]
   // [XmlNamespaceDeclarations()]
    public string _description;
}

[System.Serializable]
public class PassiveDatabase
{
    [XmlArray("Passive")]//Nome da tag no XML
    public List<PassiveXml> list = new List<PassiveXml>();
}

public class XmlMobPassive : XmlManager
{
    public static XmlMobPassive Instance;

    public PassiveDatabase PDB;

    protected override IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        //PDB = new PassiveDatabase();

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

        PDB.list.Clear();

        string language = GameManagerScenes._gms.Language().ToString();

        XmlSerializer serializer = new XmlSerializer(typeof(PassiveDatabase));

        FileStream stream = null;

        string nameStream = "";

#if UNITY_EDITOR
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Skills/" + _nameXml + "." + _typeArq.ToString();
#elif UNITY_ANDROID
        nameStream = "jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/" + language + "/Skills/" + _nameXml + "."+_typeArq.ToString();
        //nameStream = Application.streamingAssetsPath + "/StreamingAssets/Xml/" + language + "/Skills/" + _nameXml + "." + _typeArq.ToString();

        WWW reader = new WWW(nameStream);

        if (reader!=null)
	{
        while (!reader.isDone)
        {
            //wait for the reader to finish downloading        
        }

        MemoryStream streamMs = new MemoryStream(reader.bytes);

        if (streamMs.CanRead)
        {
            PDB = serializer.Deserialize(streamMs) as PassiveDatabase;
            streamMs.Close();

            print("Android LoadXml- Passivas Carregadas.");
        }
	}
        
#elif UNITY_IPHONE
        nameStream = Application.dataPath + "/Raw/StreamingAssets/Xml/" + language + "/Skills/" + _nameXml + "."+_typeArq.ToString();
#else
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Skills/" + _nameXml + "."+_typeArq.ToString();
#endif


#if !UNITY_ANDROID || UNITY_EDITOR
        stream = new FileStream(nameStream, FileMode.Open);

        if (stream!=null && stream.CanRead)
        {
            PDB = serializer.Deserialize(stream) as PassiveDatabase;
            stream.Close();

        }
        else
        {
            print("Passiva DB Não encontrada!!!");
        }
#endif

        if (PDB.list.Count > 0)
        {
            foreach (var s in PDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._description = CheckAndChangeWords(s._description);
            }

            print("Passivas Carregadas.");
        }
        else
        {
            GameManagerScenes._gms.NewInfo("Arquivo não encontrado\n" + nameStream, 5);
        }
       
    }

    public PassiveXml GetPassive(int ID)
    {
        PassiveXml _return = null;

        print("GetPassive(" + ID + ")");

        if (PDB.list.Count > 0 && ID!=-1)
        {
            if (ID >=0 && ID < PDB.list.Count)
            {
                _return = PDB.list[ID];
            }

            if (_return != null)
            {
                return _return;
            }
        }

        Load();

            if (PDB.list.Count > 0 && ID!=-1)              
                _return = PDB.list[ID];

        return _return;
    }

    public string GetDescription(int ID)
    {
        string _return = "";

        if (PDB.list.Count > 0)
        {
            if (ID >= 0 && ID < PDB.list.Count)
            {
                _return = CheckAndChangeWords(PDB.list[ID]._description);
            }

            if (_return != null)
            {
                return _return;
            }
        }

        Load();

        if (PDB.list.Count > 0 && ID < PDB.list.Count)
            _return = CheckAndChangeWords(PDB.list[ID]._description);

        return _return;
    }
}


