using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class LoadDll : MonoBehaviour
{
    public ConsoleToScreen console;
    
    void Start()
    {
        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出现问题。
#if !UNITY_EDITOR
    Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#else
        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
        
        Type hotUpdateType = hotUpdateAss.GetType("HotUpdate");
        object hotUpdateInstance = Activator.CreateInstance(hotUpdateType);
        FieldInfo messageField = hotUpdateType.GetField("Message");
        string message = messageField.GetValue(hotUpdateInstance) as string;
        console.Log(message, message, LogType.Error);
    }
}
