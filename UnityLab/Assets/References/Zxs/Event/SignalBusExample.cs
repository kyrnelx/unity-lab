using UnityEngine;

namespace Zxs
{
    public record PlayerDiedEvent; // 玩家死亡
    public record PlayerBornEvent; // 玩家出生
    public record PlayerWinEvent;  // 玩家胜利
    public record PlayerLoseEvent; // 玩家失败
    
    public class SignalBusExample : MonoBehaviour
    {
        private SignalBus _bus = new();
        private SignalSubscription _loseSubscription;
        private SignalSubscriptionCollection _subscriptions;
        
        private void Awake()
        {
            _loseSubscription = _bus.Subscribe<PlayerLoseEvent>(OnPlayerLose);
            _subscriptions = new SignalSubscriptionCollection(_bus)
                            .Subscribe<PlayerDiedEvent>(OnPlayerDied)
                            .Subscribe<PlayerBornEvent>(OnPlayerBorn)
                            .Subscribe<PlayerWinEvent>(OnPlayerWin);
        }

        private void Start()
        {
            _bus.Fire(new PlayerBornEvent());
        }

        private void OnDestroy()
        {
            _loseSubscription?.Dispose();
            _subscriptions?.Dispose();
        }

        private void OnPlayerDied(PlayerDiedEvent args)
        {
            Debug.Log("玩家死亡，触发相关逻辑");
        }

        private void OnPlayerBorn(PlayerBornEvent args)
        {
            Debug.Log("玩家出生，触发相关逻辑");
        }

        private void OnPlayerWin(PlayerWinEvent args)
        {
            Debug.Log("玩家胜利，触发相关逻辑");
        }

        public void OnPlayerLose(PlayerLoseEvent args)
        {
            Debug.Log("玩家失败，触发相关逻辑");
        }
    }
}