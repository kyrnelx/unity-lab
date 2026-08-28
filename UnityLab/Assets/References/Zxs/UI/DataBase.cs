using System;
using System.Text;
using UnityEngine;

namespace Zxs
{
    public abstract class DataBase 
    {
        /// <summary>
        /// Base64 → 字节数组 → 字符串
        /// </summary>
        public void Load() 
        {
            var encrypted = PlayerPrefs.GetString(this.GetType().FullName, string.Empty);
            var bytes     = Convert.FromBase64String(encrypted);
            var json      = Encoding.UTF8.GetString(bytes);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    
        /// <summary>
        /// 字符串 → 字节数组 → Base64
        /// </summary>
        public void Save()
        {
            var json      = JsonUtility.ToJson(this);
            var bytes     = Encoding.UTF8.GetBytes(json);
            var encrypted = Convert.ToBase64String(bytes);
            PlayerPrefs.SetString(this.GetType().FullName, encrypted);
        }
    }
}