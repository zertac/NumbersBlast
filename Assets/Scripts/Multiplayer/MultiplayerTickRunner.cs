using VContainer.Unity;

namespace NumbersBlast.Multiplayer
{
    public class MultiplayerTickRunner : ITickable
    {
        private readonly MultiplayerManager _multiplayerManager;

        public MultiplayerTickRunner(MultiplayerManager multiplayerManager)
        {
            _multiplayerManager = multiplayerManager;
        }

        public void Tick()
        {
            if (!_multiplayerManager.IsActive) return;
            _multiplayerManager.Tick(UnityEngine.Time.deltaTime);
        }
    }
}
