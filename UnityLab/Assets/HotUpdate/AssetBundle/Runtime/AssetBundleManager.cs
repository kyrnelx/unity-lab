using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kyrnel
{
    public class AssetBundleManager
    {
        public static AssetBundleManager Instance => _instance ??= new AssetBundleManager();

        private static AssetBundleManager _instance;

        private AssetBundle _mainAb;
        private AssetBundleManifest _manifest;

        private readonly string _pathPrefix = Application.streamingAssetsPath + "/";

        private Dictionary<string, AssetBundle> _loadedAb;
        private HashSet<string> _abNameSet;

        public T LoadAsset<T>(string abName, string assetName) where T : Object
        {
            Initialzie();
            var ab = LoadAb(abName);
            return ab.LoadAsset<T>(assetName);
        }

        public Object LoadAsset(string abName, string assetName, Type type)
        {
            Initialzie();
            var ab = LoadAb(abName);
            return ab.LoadAsset(assetName, type);
        }

        private AssetBundle LoadAb(string abName)
        {
            if (!_abNameSet.Contains(abName))
            {
                this.Error("ab包不存在" + abName);
                return null;
            }

            if (_loadedAb.TryGetValue(abName, out var ab))
                return ab;

            var dependenciesName = _manifest.GetAllDependencies(abName);
            foreach (var dependencyName in dependenciesName)
                LoadAb(dependencyName);

            ab = AssetBundle.LoadFromFile(_pathPrefix + abName);
            if (ab == null)
            {
                this.Error($"ab包加载失败: {abName}");
                return null;
            }

            _loadedAb[abName] = ab;
            return ab;
        }

        public void Unload(string abName)
        {
            if (_loadedAb.ContainsKey(abName))
            {
                _loadedAb[abName].Unload(false);
                _loadedAb.Remove(abName);
            }
        }

        public void ClearAll()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            _loadedAb.Clear();
            _mainAb = null;
            _manifest = null;
        }

        private void Initialzie()
        {
            if (_mainAb != null) return;
            _loadedAb = new Dictionary<string, AssetBundle>();
            _mainAb = AssetBundle.LoadFromFile(_pathPrefix + GetMainAbName());
            _manifest = _mainAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            _abNameSet = new HashSet<string>(_manifest.GetAllAssetBundles());
        }

        private string GetMainAbName()
        {
#if UNITY_STANDALONE_WIN
            return AbPlatformNames.Windows;
#elif UNITY_STANDALONE_OSX
            return AbPlatformNames.OSX;
#elif UNITY_IOS
            return AbPlatformNames.iOS;
#elif UNITY_ANDROID
            return AbPlatformNames.Android;
#else
            Debug.LogError($"未支持的平台: {Application.platform}");
            return string.Empty;
#endif
        }
    }

    public static class AbPlatformNames
    {
        public const string Windows = "StandaloneWindows64";
        public const string OSX = "StandaloneOSX";
        public const string iOS = "iOS";
        public const string Android = "Android";
    }
}