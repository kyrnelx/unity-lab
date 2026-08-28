using UnityEngine;
using UnityEngine.UI;

namespace Kyrnel
{
    [RequireComponent(typeof(Image))]
    public class PathfindingNode : MonoBehaviour
    {
        [SerializeField] private Image image;

        public Vector2Int Position { get; private set; }
        public bool IsWalkable { get; private set; }
        public PathfindingNode Parent { get; set; }

        public void SetData(Vector2Int position, bool isWalkable)
        {
            Position = position;
            IsWalkable = isWalkable;
            Parent = null;
            image.color = isWalkable ? Color.green : Color.red;
        }

        public void MarkStart()
        {
            image.color = Color.white;
        }

        public void MarkVisited()
        {
            image.color = Color.gray;
        }

        public void MarkWalked()
        {
            image.color = Color.yellow;
        }

        public void MarkPath()
        {
            image.color = Color.cyan;
        }
    }
}