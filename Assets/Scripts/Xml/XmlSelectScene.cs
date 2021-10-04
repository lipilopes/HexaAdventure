using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;


public class XmlSelectScene : XmlManager
{
    public GameObject[] _file;

    public StringDatabase ADB;

    protected override IEnumerator Start()
    {
        Load();

        yield return base.Start();
    }

    void Load()
    {
        if (GameManagerScenes._gms.IsMobile)
        {
            GameManagerScenes._gms.BuildAchievement();
            return;
        }

        ADB.list.Clear();

        string language = GameManagerScenes._gms.Language().ToString();

        XmlSerializer serializer = new XmlSerializer(typeof(StringDatabase));

        string nameStream = "";

#if UNITY_EDITOR
            nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Configuracoes/" + _nameXml + ".xml";
#elif UNITY_ANDROID
        nameStream = "jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/" + language + "/Configuracoes/" + _nameXml + ".xml";

        //nameStream = Application.streamingAssetsPath + "/StreamingAssets/Xml/" + language + "/Configuracoes/" + _nameXml + ".xml";

        WWW reader = new WWW(nameStream);

        while (!reader.isDone)
        {
            //wait for the reader to finish downloading        
        }

        MemoryStream streamb = new MemoryStream(reader.bytes);

        if (streamb.CanRead)
        {
            ADB = serializer.Deserialize(streamb) as StringDatabase;           

            streamb.Close();

            print("Android LoadXml-SelectSceneXml Carregadas.");
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
            ADB = serializer.Deserialize(stream) as StringDatabase;
            stream.Close();
        }
        else
        {
            print("SelectScene Não encontrada!!!");

            GameManagerScenes._gms.LoadingBar("<color=red>ERRO Load SelectScene</color>", 1);
            return;
        }
#endif

        if (ADB.list.Count >= 1)
        {
            foreach (var s in ADB.list)
            {
                if (s._nameX != null)
                    s._nameX = CheckAndChangeWords(s._nameX);
            }


            print("SelectSceneXml Carregadas.");

            GameManagerScenes._gms.BuildAchievement();
        }
        else
        {
            GameManagerScenes._gms.NewInfo("Arquivo não encontrado\n" + nameStream, 5);
        }
    }

    public string Get(int ID)
    {
        string _return = null;

        if (ADB.list.Count > 0)
        {
            return ADB.list[ID]._nameX;
        }

        Load();

        if (ADB.list.Count > 0)
            _return = CheckAndChangeWords(ADB.list[ID]._nameX);

        return _return;
    }

    public string[] Get()
    {
        List<string> _return = new List<string>();

        if (ADB.list.Count > 0)
        {
            foreach (var i in ADB.list)
            {
                _return.Add(i._nameX);
            }

            if (_return.Count != 0)
                return _return.ToArray();
        }

        Load();

        if (ADB.list.Count > 0)
            foreach (var i in ADB.list)
            {
                _return.Add(i._nameX);
            }

        return _return.ToArray();
    }
}

