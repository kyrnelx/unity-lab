using UnityEngine;

namespace Zxs
{
    public class GuideFingerView : ViewBase
    {
        public class Args : UIArgsBase
        {
            public RectTransform RectTransform;
            public Args(RectTransform rectTransform) => RectTransform = rectTransform;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }
    }
}