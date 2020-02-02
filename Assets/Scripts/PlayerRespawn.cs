using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerRespawn : NetworkBehaviour
{
    Rigidbody playerRb;

    private GameObject[] blueSpawnPoints;
    private GameObject[] redSpawnPoints;
    [SyncVar] private Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        blueSpawnPoints = new GameObject[4];
        redSpawnPoints = new GameObject[4];

        blueSpawnPoints[0] = GameObject.Find("Blue_Spawn_1");
        blueSpawnPoints[1] = GameObject.Find("Blue_Spawn_2");
        blueSpawnPoints[2] = GameObject.Find("Blue_Spawn_3");
        blueSpawnPoints[3] = GameObject.Find("Blue_Spawn_4");

        redSpawnPoints[0] = GameObject.Find("Red_Spawn_1");
        redSpawnPoints[1] = GameObject.Find("Red_Spawn_2");
        redSpawnPoints[2] = GameObject.Find("Red_Spawn_3");
        redSpawnPoints[3] = GameObject.Find("Red_Spawn_4");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Lava"))
        {
            Debug.Log("Player In Lava");
            //Teams not implemented - everyone starts at blue side lol


            CmdPlayerRespawn(GetComponent<NetworkIdentity>().netId);


        }
    }

    [Command]
    void CmdPlayerRespawn(NetworkInstanceId netId)
    {
        RpcPlayerRespawn(netId);
    }

    [ClientRpc]
    void RpcPlayerRespawn(NetworkInstanceId netId)
    {
        GameObject player = ClientScene.FindLocalObject(netId);
        TeamPlayer team = player.GetComponent<TeamPlayer>();
        if(team.Team == Team.Blue)
            spawnPoint = blueSpawnPoints[Random.Range(0, 4)].transform.position;
        else if (team.Team == Team.Red)
            spawnPoint = redSpawnPoints[Random.Range(0, 4)].transform.position;
        player.transform.position = spawnPoint;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
