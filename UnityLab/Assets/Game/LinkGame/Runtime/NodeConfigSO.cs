using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kyrnel.LinkGame
{
    [Serializable]
    public struct NodeTypeSpriteData
    {
        public NodeType type;
        public Sprite sprite;
    }

    [CreateAssetMenu(fileName = "NodeConfig", menuName = "LinkGame/NodeConfig")]
    public class NodeConfigSO : ScriptableObject
    {
        [SerializeField] private List<NodeTypeSpriteData> _typeSprites;

        private Dictionary<NodeType, Sprite> _spriteDict;

        public void Initialize()
        {
            _spriteDict = new Dictionary<NodeType, Sprite>();
            foreach (var item in _typeSprites)
            {
                if (item.sprite != null && !_spriteDict.ContainsKey(item.type))
                {
                    _spriteDict.Add(item.type, item.sprite);
                }
            }
        }

        public Sprite GetSprite(NodeType type)
        {
            if (_spriteDict == null)
            {
                Initialize();
            }

            if (_spriteDict.TryGetValue(type, out Sprite sprite))
            {
                return sprite;
            }

            Debug.LogWarning($"[NodeConfigSO] 未找到类型 {type} 对应的 Sprite！");
            return null;
        }
    }
}