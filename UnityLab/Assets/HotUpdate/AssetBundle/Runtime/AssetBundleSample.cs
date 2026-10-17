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
            var cube = AssetBundleManager.Instance.LoadAsset<GameObject>("model", "Cube");
            Instantiate(cube);
        }

        private IEnumerator LoadAssetAsync<T>(string bundleName, string assetName, Action<T> callback = null) where T : UnityEngine.Object
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