using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kyrnel
{
    public class AssetBundleSample : MonoBehaviour
    {
        private void Awake()
        {
            /*var modelBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "model");
            var modelCube = modelBundle.LoadAsset<GameObject>("Cube");
            var modelSphere = modelBundle.LoadAsset<GameObject>("Sphere");
            Instantiate(modelCube);
            Instantiate(modelSphere);*/

            StartCoroutine(LoadAssetAsync<GameObject>("model", "Cube", model => { Instantiate(model); }));
        }

        private IEnumerator LoadAssetAsync<T>(string bundleName, string assetName, Action<T> callback) where T : UnityEngine.Object
        {
            string path = Path.Combine(Application.streamingAssetsPath, bundleName);
            var bundleReq = AssetBundle.LoadFromFileAsync(path);
            yield return bundleReq;

            AssetBundle ab = bundleReq.assetBundle;
            if (ab == null)
            {
                Debug.LogError($"[AssetLoader] 加载 AB 包失败: {path}");
                callback?.Invoke(null);
                yield break;
            }

            var assetReq = ab.LoadAssetAsync<T>(assetName);
            yield return assetReq;
            
            callback?.Invoke(assetReq.asset as T);
        }
    }
}