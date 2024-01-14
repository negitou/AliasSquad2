using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameSettings : MonoBehaviour {

    public static GameSettings gameSettings;
    // Path
    private string filePath = "/inti.data";

    private int mode;

    public int port;

    //外部から変更しないようにprivate
    private string[] dataServer = new string[2];
    private string[] mMServer = new string[2];

    //ServerのIPとPort取得用のgetter
    public string[] DataServer{ get { return dataServer; } }
    public string[] MMServer { get{ return mMServer; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        filePath = Application.dataPath + filePath;

        if (gameSettings == null)
        {
            gameSettings = this;
        }
        LoadIntiFile();
    }

    void Start () {
        SceneManager.LoadScene("AliasSquad_MainMenu");
    }
    public void SetupServer()
    {
        //NetworkServer.Listen(port);
    }

    private void LoadIntiFile()
    {
        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {

                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    
                    string[] split = line.Replace("\r", "").Replace("\n", "").Split(' ');
                    if (split[0] != "")
                    {
                        switch (split[0])
                        {
                            //コメント
                            case "//":
                                break;

                            //モード
                            case "Mode":
                                if (split.Length >= 2)
                                {
                                    switch (split[1])
                                    {
                                        //クライアント
                                        case "Client":
                                            mode = 0;
                                            break;
                                        //サーバー
                                        case "Server":
                                            mode = 1;
                                            port = int.Parse(split[2]);
                                            break;
                                    }
                                }
                                break;

                            case "DataServer":
                                if (split.Length == 3)
                                {
                                    dataServer[0] = split[1];
                                    dataServer[1] = split[2];
                                }
                                break;

                            case "MMServer":
                                if (split.Length == 3)
                                {
                                    mMServer[0] = split[1];
                                    mMServer[1] = split[2];
                                }
                                break;

                            default:

                                break;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {

        }

    } 
}
