using System;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MusicController))]
    public class GameManager : MonoBehaviour
    {
        private bool isCursorLocked;
        public static GameManager Instance { get; private set; }
        public MusicController MusicController { get; private set; }
        public GameStats GameStats { get; private set; }

        private void Awake()
        {
            Instance = this;
            MusicController = GetComponent<MusicController>();
            GameStats = GetComponent<GameStats>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (this.isCursorLocked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = true;
                }
            }
        }

        public void KingHit(Team loserTeam)
        {
            switch (loserTeam)
            {
                case Team.Red:
                    GameStats.BlueTeamPoints++;
                    break;
                case Team.Blue:
                    GameStats.RedTeamPoints++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loserTeam), loserTeam, null);
            }
        }
    }
}