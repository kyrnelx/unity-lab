using System.Collections.Generic;
using UnityEngine;

namespace Zxs
{
    public class UIModelExample : MonoBehaviour
    {
        [SerializeField] private UIModelRenderer _modelRenderer;

        private void Start()
        {
            var models = new List<UIModel>()
            {
                new UIModel()
                {
                    Name = "Zxs/UIModel_Cube",
                    Rotation = new Vector3(0f, 30f, 0f),
                }
            };
            _modelRenderer.SetModels(models);
        }
    }
}