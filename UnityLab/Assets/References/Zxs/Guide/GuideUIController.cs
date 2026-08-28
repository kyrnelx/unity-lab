using UnityEngine;

namespace Zxs
{
    public class CompleteStepEvent : EventBase { }

    public class GuideUIController : MonoBehaviour
    {
        private void Awake()
        {
            GlobalSignalBus.Instance.Subscribe<StartGuideEvent>(StartGuide);
        }

        private void StartGuide(StartGuideEvent args)
        {
            args.GuideCtrl.RefreshStep();
            var currentStep = args.GuideCtrl.GetCurrentStep();
            if (currentStep.type == GuideStepType.Dialog)
            {
                var dialogPrefab = Resources.Load<GuideDialogView>("Zxs/GuideDialogView");
                UIManager.Instance.ShowView(dialogPrefab, ViewLayer.Guide, new GuideDialogView.Args(currentStep));
            }
        }

        private void CompleteStep()
        {
        }
    }
}