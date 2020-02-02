using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class TeamSelectionTrigger : NetworkBehaviour
    {
        [SerializeField]
        private Team team = Team.Red;

        [SerializeField]
        private string teamTag = string.Empty;

        [SerializeField] private Material teamColor;

        private GameObject[] spawnPoints;
        [SyncVar] private int spawnIndex = 0;

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
                var renderers = player.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    r.material = teamColor;
                }
                CmdTeleportToTeamCastle(player.GetComponent<NetworkIdentity>().netId);
            }
        }

        [Command]
        void CmdTeleportToTeamCastle(NetworkInstanceId netId)
        {
            if (spawnIndex >= spawnPoints.Length - 1)
                spawnIndex = 0;

            RpcTeleportToTeamCastle(netId);
            spawnIndex++;
        }

        [ClientRpc]
        void RpcTeleportToTeamCastle(NetworkInstanceId netId)
        {
            GameObject player = ClientScene.FindLocalObject(netId);
            player.transform.position = spawnPoints[spawnIndex].transform.position;
        }
    }
}
