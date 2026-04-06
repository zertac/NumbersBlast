using VContainer.Unity;

namespace NumbersBlast.Multiplayer
{
    /// <summary>
    /// VContainer tick adapter that drives the multiplayer manager's update loop each frame.
    /// </summary>
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
