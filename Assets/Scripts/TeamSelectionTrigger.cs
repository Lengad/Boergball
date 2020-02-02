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
            if (other.gameObject.CompareTag("Player"))
            {
                TeamPlayer.CreateComponent(other.gameObject, team);

                if (spawnIndex >= spawnPoints.Length - 1)
                    spawnIndex = 0;

                other.transform.position = spawnPoints[spawnIndex].transform.position;
            }
        }
    }
}
