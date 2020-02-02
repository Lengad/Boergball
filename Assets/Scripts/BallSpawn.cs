using UnityEngine;
using UnityEngine.Networking;

public class BallSpawn : NetworkBehaviour
{
    private GameObject[] ballSpawnPoints;
    public float maxSpawnOffset;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballSpawnPoints = new GameObject[2];
        ballSpawnPoints[0] = GameObject.Find("BallSpawnPoint_Blue");
        ballSpawnPoints[1] = GameObject.Find("BallSpawnPoint_Red");
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Ball in Lava");
        if (collision.transform.CompareTag("Lava"))
        {

            CmdResetBallPosition(GetComponent<NetworkIdentity>().netId);
        }
    }

    [Command]
    void CmdResetBallPosition(NetworkInstanceId netId)
    {
        RpcResetBallPosition(netId);
    }

    [ClientRpc]
    void RpcResetBallPosition(NetworkInstanceId netId)
    {

        GameObject ball = ClientScene.FindLocalObject(netId);
        var ballRb = ball.GetComponent<Rigidbody>();
        // Pick one of the two sides randomly
        int r = Random.Range(0, 2);
        ball.transform.position = new Vector3(Random.Range(-maxSpawnOffset, maxSpawnOffset) + ballSpawnPoints[r].transform.position.x, ballSpawnPoints[r].transform.position.y, Random.Range(-maxSpawnOffset, maxSpawnOffset) + ballSpawnPoints[r].transform.position.z);
        ballRb.velocity = Vector3.zero;
    }
}
