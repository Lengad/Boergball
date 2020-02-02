using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{
    private GameObject[] ballSpawnPoints;
    public float maxSpawnOffset;

    // Start is called before the first frame update
    void Start()
    {
        ballSpawnPoints = new GameObject[2];
        ballSpawnPoints[0] = GameObject.Find("BallSpawnPoint_Blue");
        ballSpawnPoints[1] = GameObject.Find("BallSpawnPoint_Red");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Lava"))
        {
            // Pick one of the two sides randomly
            int r = Random.Range(0, 1);
            transform.position = new Vector3(Random.Range(-maxSpawnOffset, maxSpawnOffset) + ballSpawnPoints[r].transform.position.x, ballSpawnPoints[r].transform.position.y, Random.Range(-maxSpawnOffset, maxSpawnOffset) + ballSpawnPoints[r].transform.position.z);
        }
    }
}
