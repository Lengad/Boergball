using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MusicController))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public MusicController MusicController { get; private set; }
        public GameStats GameStats { get; private set; }

        private void Awake()
        {
            Instance = this;
            MusicController = GetComponent<MusicController>();
            GameStats = GetComponent<GameStats>();
        }
    }
}