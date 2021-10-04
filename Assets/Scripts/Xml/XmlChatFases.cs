using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

[System.Serializable]
public class History
{
    public int    _fase;
    public int    _mobID;
    public bool   _left;
    public bool   _startFase;
    public string _mobName;
    [TextArea]
    public string _chat;      
}

[System.Serializable]
public class HistoryDatabase
{
    [XmlArray("History")]//Nome da tag no XML
    public List<History> list = new List<History>();
}

public class XmlChatFases : XmlManager
{
    public static XmlChatFases Instance;

    protected override IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        return base.Start();
    }

    public List<History> GetHistory(int fase, bool start = true)
    {
        if (GameManagerScenes._gms.IsMobile)
        {

            return null;
        }

        List<History> G = new List<History>();

        //Pega as Falas da Mob Atual!!!
        int    countFase   = GameManagerScenes._gms.FaseCount;
        string language    = GameManagerScenes._gms.Language().ToString();

        XmlSerializer serializer = new XmlSerializer(typeof(HistoryDatabase));
        HistoryDatabase _historyDB = null;      

        string nameStream = "";

        print("faseAtual: " + fase + "/" + countFase);

#if UNITY_EDITOR
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Historia/" + _nameXml + fase + ".xml";
#elif UNITY_ANDROID
       nameStream ="jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/" + language + "/Historia/" + _nameXml + fase + ".xml";       

         //nameStream = Application.streamingAssetsPath + "/StreamingAssets/Xml/" + language + "/Historia/" + _nameXml + fase + ".xml";

        WWW reader = new WWW(nameStream);

        while (!reader.isDone)
        {
            //wait for the reader to finish downloading        
        }

        MemoryStream streamMs = new MemoryStream(reader.bytes);

        if (streamMs != null)
        {
            _historyDB = serializer.Deserialize(streamMs) as HistoryDatabase;           

            streamMs.Close();

            Debug.LogError("Android LoadXml- ChatFases");
        }
#elif UNITY_IPHONE
        nameStream =Application.dataPath + "/Raw/StreamingAssets/Xml/" + language + "/Historia/" + _nameXml + fase + ".xml";
#else
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Historia/" + _nameXml + fase + ".xml";
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
        FileStream stream = new FileStream(nameStream, FileMode.Open);

        if (stream.CanRead)
        {
            _historyDB = serializer.Deserialize(stream) as HistoryDatabase;
            stream.Close();
        }
        else
        {
            print("Historia Não encontrada!!!");
           return null;
        }     
#endif

        if (_historyDB.list.Count > 0)
        {
            foreach (var s in _historyDB.list)
            {
                if (fase == s._fase)
                {
                    print(fase + " = " + s._fase);

                    if (s._mobID == -1)
                        s._mobID = GameManagerScenes._gms.PlayerID;

                    s._mobName = GameManagerScenes._gms.HeroName(s._mobID);

                    s._chat = CheckAndChangeWords(s._chat);

                    if (s._startFase && start)
                        G.Add(s);
                    else
                        if (!s._startFase && !start)
                        G.Add(s);
                }
            }

            _historyDB.list.Clear();


            print("Historia <b>" + GameManagerScenes._gms.NameFase(fase) + "</b> Carregada.");
        }

        return G;
    }
}
