
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


/// <summary>
/// 数据储存助手
/// 用[Serializable]标记序列，用[NonSerialized]标记不序列化
/// </summary>
public class DataStorageHelper
{
    /// <summary>
    /// 保护有构造
    /// </summary>
    private DataStorageHelper() { }


    /// <summary>
    /// 获取储存
    /// </summary>
    public static T getData<T>(string storageKey, string secretKey)
    {
        string alldata = PlayerPrefs.GetString(storageKey, null);

        if (string.IsNullOrEmpty(alldata))
        {
            return default(T);
        }
        else
        {
            //解密数据
            alldata = ConductDecrypt(alldata, secretKey);

            //反序列化为类对象
            return JsonUtility.FromJson<T>(alldata);
        }
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    public static void saveData(string storageKey, string secretKey, object obj)
    {
        //序列化为json
        string datas = JsonUtility.ToJson(obj);

        // Debug.Log(datas);
        Debug.Assert(!string.IsNullOrEmpty(datas), "储存数据为空！！");

        //加密数据
        datas = ConductEncryption(datas, secretKey);

        //储存数据
        PlayerPrefs.SetString(storageKey, datas);
        //强制保存
        PlayerPrefs.Save();
    }


    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="input">需要加密内容</param>
    /// <param name="keyValue">密码</param>
    private static string ConductEncryption(string input, string keyValue)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(keyValue);

        //加密格式
        RijndaelManaged encryption = new RijndaelManaged();
        encryption.Key = keyArray;
        encryption.Mode = CipherMode.ECB;
        encryption.Padding = PaddingMode.PKCS7;

        //生成加密锁
        ICryptoTransform cTransform = encryption.CreateEncryptor();
        byte[] EncryptArray = UTF8Encoding.UTF8.GetBytes(input);
        byte[] resultArray = cTransform.TransformFinalBlock(EncryptArray, 0, EncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="_valueDense">需要解密内容</param>
    /// <param name="_keyValue">密码</param>
    /// <returns></returns>
    private static string ConductDecrypt(string valueDense, string keyValue)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(keyValue);

        RijndaelManaged decipher = new RijndaelManaged();
        decipher.Key = keyArray;
        decipher.Mode = CipherMode.ECB;
        decipher.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = decipher.CreateDecryptor();
        byte[] EncryptArray = Convert.FromBase64String(valueDense);
        byte[] resultArray = cTransform.TransformFinalBlock(EncryptArray, 0, EncryptArray.Length);
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
}

