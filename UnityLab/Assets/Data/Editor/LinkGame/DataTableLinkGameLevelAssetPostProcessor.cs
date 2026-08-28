using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
///
public class DataTableLinkGameLevelAssetPostprocessor : AssetPostprocessor 
{
    private static readonly string filePath = "Assets/Data/Editor/Excel/LinkGame.xlsx";
    private static readonly string assetFilePath = "Assets/Data/Editor/Excel/DataTableLinkGameLevel.asset";
    private static readonly string sheetName = "DataTableLinkGameLevel";
    
    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets) 
        {
            if (!filePath.Equals (asset))
                continue;
                
            DataTableLinkGameLevel data = (DataTableLinkGameLevel)AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(DataTableLinkGameLevel));
            if (data == null) {
                data = ScriptableObject.CreateInstance<DataTableLinkGameLevel> ();
                data.SheetName = filePath;
                data.WorksheetName = sheetName;
                AssetDatabase.CreateAsset ((ScriptableObject)data, assetFilePath);
                //data.hideFlags = HideFlags.NotEditable;
            }
            
            //data.dataArray = new ExcelQuery(filePath, sheetName).Deserialize<DataTableLinkGameLevelData>().ToArray();		

            //ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
            //EditorUtility.SetDirty (obj);

            ExcelQuery query = new ExcelQuery(filePath, sheetName);
            if (query != null && query.IsValid())
            {
                data.dataArray = query.Deserialize<DataTableLinkGameLevelData>().ToArray();
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
                EditorUtility.SetDirty (obj);
            }
        }
    }
}
