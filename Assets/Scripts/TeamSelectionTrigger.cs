using UnityEngine;

namespace Assets.Scripts
{
    public class TeamSelectionTrigger : MonoBehaviour
    {
        [SerializeField]
        private Team team = Team.Red;

        [SerializeField]
        private string teamTag = string.Empty;

        private GameObject[] spawnPoints;
        private int spawnIndex = 0;

        void Start()
        {
            spawnPoints = GameObject.FindGameObjectsWithTag(teamTag);
        }

        void OnTriggerEnter(Collider other)
        {
            GameObject player = other.gameObject;

            // Ugly hack
            Transform parent = other.transform.parent;
            if (parent != null)
                player = parent.gameObject;


            if (player.CompareTag("Player"))
            {
                TeamPlayer.CreateComponent(player, team);

                if (spawnIndex >= spawnPoints.Length - 1)
                    spawnIndex = 0;

                player.transform.position = spawnPoints[spawnIndex].transform.position;
            }
        }
    }
}
