using UnityEngine;

namespace Kyrnel
{
    public class ClampedRangeAttribute : PropertyAttribute
    {
        public float min, max;

        public ClampedRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}