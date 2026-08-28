using System;
using System.Collections.Generic;
using Kyrnel;
using UnityEngine;
using UnityEngine.UI;

namespace Zxs
{
    [RequireComponent(typeof(AutoScreenSizeUI))]
    public class UIModelRenderer : MonoBehaviour
    {
        public RawImage RawImage;
        public List<UIModel>   Models;

        public void SetModels(List<UIModel> models = null)
        {
            if(this.Models != null)
                this.Models = models;
            UIModelManager.Instance.AddRenderer(this);
        }

        public void SetRenderTexture(RenderTexture renderTexture)
        {
            this.RawImage.texture = renderTexture;
        }

        private void OnDestroy()
        {
            UIModelManager.Instance.RemoveRenderer(this);
        }
    }
    
    [Serializable]
    public class UIModel
    {
        public string  Name;
        public Vector3 Offset;
        public Vector3 Rotation;
    }
}