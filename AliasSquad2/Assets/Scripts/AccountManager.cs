using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : MonoBehaviour
{
	public static AccountManager accountManager;

	//サーバーのIPアドレスとポート番号
	[SerializeField]private string serverIP;
	[SerializeField]private int port;

	private const string loginSendMsg = "/login";

	private string account;

	public string AccountName{ get { return account; } }

	void Awake ()
	{
		if (accountManager != null) {
			accountManager = this;
		} else {
			Destroy (gameObject);
		}

	}

	void Start ()
	{
		string[] str = GameSettings.gameSettings.DataServer;
		serverIP = str [0];
		port = int.Parse (str [1]); 
	}

	public bool Login (string name, string password)
	{
		if (account == null) {
			try {
				string resMsg;
				//サーバーとの接続
				using (TcpClient tcp = new TcpClient (serverIP, port)) {
					using (NetworkStream ns = tcp.GetStream ()) {
						//タイムアウトの設定
						ns.ReadTimeout = 5000;
						ns.WriteTimeout = 5000;
						//送信する文字列
						string sendMsg = loginSendMsg;
						Encoding enc = Encoding.UTF8;
						byte[] sendBytes = enc.GetBytes (sendMsg + name + " " + password);
						//送信
						ns.Write (sendBytes, 0, sendBytes.Length);
						using (MemoryStream ms = new MemoryStream ()) {
							byte[] resBytes = new byte[256];
							int resSize = 0;
							do {
								//受信
								resSize = ns.Read (resBytes, 0, resBytes.Length);
								if (resSize == 0) {
									break;
								}
								ms.Write (resBytes, 0, resSize);
							} while(ns.DataAvailable);//resBytes [resSize - 1] != '\n'
							resMsg = enc.GetString (ms.GetBuffer (), 0, (int)ms.Length);
						}
					}
				}
                string[] splitMsg = resMsg.Split(' ');

                if (splitMsg[1] == "true") {
					account = name;
					return true;
				}
			} catch (System.Exception) {
				
			}
		}       
		return false;
	}

	public bool Logout ()
	{
		if (account != null) {
			
			account = null;
			return true;
		}
		return false;
	}

    public string GetPreset(int index)
    {
        string preset = "";

        return preset;
    }
}
