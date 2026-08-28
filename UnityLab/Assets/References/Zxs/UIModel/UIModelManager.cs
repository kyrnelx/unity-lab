using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zxs
{
    public class UIModelManager : MonoBehaviour
    {
        private static UIModelManager _instance;
        public static  UIModelManager Instance => _instance;
        
        [SerializeField] private Transform _modelRoot;
        [SerializeField] private Camera _camera;
        
        private CanvasScaler          _canvasScaler;
        private List<UIModelRenderer> _renderers = new();
        private CustomRenderTexture _renderTexture;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            _canvasScaler = FindObjectOfType<CanvasScaler>();
            CreateRenderTexture();
        }

        public void AddRenderer(UIModelRenderer renderer)
        {
            _renderers.Add(renderer);
            renderer.SetRenderTexture(_renderTexture);
            UpdateRenderer();
        }

        public void RemoveRenderer(UIModelRenderer renderer)
        {
            _renderers.Remove(renderer);
            UpdateRenderer();
        }

        public void UpdateRenderer()
        {
            if (_renderers.Count == 0)
            {
                return;
            }

            var renderer = _renderers[^1];
           LoadModels(renderer);
        }

        private void LoadModels(UIModelRenderer renderer)
        {
            StartCoroutine(LoadModelsCoroutine(renderer));
        }

        private IEnumerator LoadModelsCoroutine(UIModelRenderer renderer)
        {
            var models = renderer.Models;
            foreach (var model in models)
            {
                var modelRequest = Resources.LoadAsync<GameObject>(model.Name);
                yield return modelRequest;
                var modelGo = Instantiate(modelRequest.asset as GameObject, _modelRoot);
                modelGo.transform.localPosition += model.Offset;
                modelGo.transform.localRotation = Quaternion.Euler(model.Rotation);
            }
            
            yield return null;
        }

        private void DisposeModels()
        {
        }

        public void CreateRenderTexture()
        {
            var logicWidth    = Screen.width  / _canvasScaler.scaleFactor;
            var logicHeight   = Screen.height / _canvasScaler.scaleFactor;
            _renderTexture = new CustomRenderTexture((int)logicWidth, (int)logicHeight, RenderTextureFormat.ARGB32);
            _camera.targetTexture = _renderTexture;
        }

        public void DisposeRenderTexture()
        {
        }
    }
}