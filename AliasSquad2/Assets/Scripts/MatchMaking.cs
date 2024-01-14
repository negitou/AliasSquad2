using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class MatchMaking : MonoBehaviour {
    
    //サーバーのIPアドレスとポート番号
    //最終外部ファイルに記述
    [SerializeField]
    private string serverIP;
    [SerializeField]
    private int port = 7776;

    [SerializeField]
    private Text output;

    private bool request = false;

    public enum Mode
    {
        DE,
        TDM,
        SB,
    }

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        if (request)
        {
            /*
             * マッチング待ち 
             */

        }

    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Join(string address, int port)
    {
        //NetworkManager.singleton.networkAddress = address;
        //NetworkManager.singleton.networkPort = port;
        //NetworkManager.singleton.StartClient();
    }

    public void MatchRequest(Mode mode,string map)
    {
        //TcpClientを作成し、サーバーと接続する
        TcpClient tcp = new TcpClient(serverIP, port);

        //NetworkStreamを取得する
        NetworkStream ns = tcp.GetStream();

        //読み取り、書き込みのタイムアウトを5秒にする
        ns.ReadTimeout = 5000;
        ns.WriteTimeout = 5000;

        //サーバーにデータを送信する
        string msg = "/" + mode.ToString() + "," + map;
        //文字列をByte型配列に変換
        Encoding enc = Encoding.UTF8;
        byte[] sendBytes = enc.GetBytes(msg + '\n');
        //データを送信する
        ns.Write(sendBytes, 0, sendBytes.Length);

        //サーバーから送られたデータを受信する
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        byte[] resBytes = new byte[256];
        int resSize = 0;
        do
        {
            //データの一部を受信する
            resSize = ns.Read(resBytes, 0, resBytes.Length);
            //Readが0を返した時はサーバーが切断したと判断
            if (resSize == 0)
            {
                output.text += "サーバーが切断しました。\n";
                break;
            }
            //受信したデータを蓄積する
            ms.Write(resBytes, 0, resSize);
            //まだ読み取れるデータがあるか、データの最後が\nでない時は、
            //受信を続ける
        } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');
        //受信したデータを文字列に変換
        string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        ms.Close();
        //末尾の\nを削除
        resMsg = resMsg.TrimEnd('\n');
        output.text += resMsg + "\n";

        //閉じる
        ns.Close();
        tcp.Close();
        output.text += "切断しました。\n";
    }
}
