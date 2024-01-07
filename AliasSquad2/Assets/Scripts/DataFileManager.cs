using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DataFileManager : MonoBehaviour {

    //ディレクトリのパス Application.dataPath に含まれない追加ディレクトリのみ
    private string dataDirectoryPath = "/Data";
    //ファイルの拡張子
    private string fileExtension = ".data";
    //AESのKey 16bit
    private byte[] aesKey = new byte[] { 0x89, 0xA0, 0x93, 0x1F, 0x7F, 0xC4, 0x14, 0x06, 0x8C, 0x62, 0x9A, 0x0D, 0xEE, 0x30, 0x39, 0x31, 0xF6, 0x37, 0x01, 0xA4, 0x9D, 0x86, 0x59, 0x57, 0xC9, 0xAD, 0x04, 0x88, 0x08, 0xED, 0x60, 0x46 };

    void Start () {
        //ディレクトリの確認
        DirectoryConfirmation();
    }

    public void FileSave(string fileName, string str)
    {
        //ディレクトリの確認
        DirectoryConfirmation();
        //byteにUTF-16でエンコード
        byte[] data = Encoding.Unicode.GetBytes(str);
        //AesManagedのインスタンス生成
        using (var aesManaged = new AesManaged())
        {
            //キーの設定
            aesManaged.Key = aesKey;
            //初期化ベクターの生成
            aesManaged.GenerateIV();
            //書き込みのためのStream
            using (var fileStream = new FileStream(Application.dataPath + dataDirectoryPath + "/" + fileName + fileExtension, FileMode.OpenOrCreate))
            {
                //初期化ベクターを書き込む
                fileStream.Write(aesManaged.IV, 0, 16);
                //暗号化のためのStream
                using (var cryptoStream = new CryptoStream(fileStream, aesManaged.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    //書き込み
                    cryptoStream.Write(data, 0, data.Length);
                }
            }
        }
    }

    public string FileLoad(string fileName)
    {
        DirectoryConfirmation();

        //復号データ用配列
        byte[] data;
        //UnicodeEncodingのインスタンス生成
        var unicodeEncoding = new UnicodeEncoding();

        try
        {
            //読み込みのためのStream
            using (var fileStream = new FileStream(Application.dataPath + dataDirectoryPath + "/" + fileName + fileExtension, FileMode.Open))
            {
                //復号データ用配列 初期化
                data = new byte[fileStream.Length - 16];
                //AesManagedのインスタンス生成
                using (var aesManaged = new AesManaged())
                {
                    //キーの設定
                    aesManaged.Key = aesKey;
                    //初期化ベクターを読み込み設定
                    byte[] aesIV = new byte[16];
                    fileStream.Read(aesIV, 0, 16);
                    aesManaged.IV = aesIV;
                    //復号のためのStream
                    using (var cryptoStream = new CryptoStream(fileStream, aesManaged.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        //復号データ
                        cryptoStream.Read(data, 0, data.Length);
                    }
                }
            }
            //文字列にUTF-16でエンコード
            return unicodeEncoding.GetString(data);
        }
        catch (IsolatedStorageException)
        {
            return null;
        }
        catch (FileNotFoundException)
        {
            //ファイルなければ null
            return null;
        }
    }

    private void DirectoryConfirmation()
    {
        //ディレクトリがなければ作成
        if (!Directory.Exists(Application.dataPath + dataDirectoryPath))
        {
            Directory.CreateDirectory(Application.dataPath + dataDirectoryPath);
        }
    }

}
