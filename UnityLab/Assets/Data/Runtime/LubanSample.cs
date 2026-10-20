using System.Collections;
using System.Collections.Generic;
using cfg;
using Luban.SimpleJSON;
using UnityEngine;

public class LubanSample : MonoBehaviour
{
    public Tables tables;

    void Start()
    {
        tables = new Tables(Loader);
        Debug.Log(tables.Tbitem.DataList[0].Name);
    }

    private JSONNode Loader(string fileName)
    {
        var config = Resources.Load<TextAsset>("Config/" + fileName);
        return JSON.Parse(config.text);
    }
}