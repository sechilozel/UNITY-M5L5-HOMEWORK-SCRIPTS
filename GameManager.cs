using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{

     [SerializeField] List<Transform> spawns = new List<Transform>();
     [SerializeField] List<Transform> spawnsWE = new List<Transform>();
     [SerializeField] List<Transform> spawnsTurret = new List<Transform>();

     int randomSpawn;
    // Start is called before the first frame update
    void Start()
    {
        randomSpawn = Random.Range(0, spawns.Count);
        PhotonNetwork.Instantiate("Player", spawns[randomSpawn].position,
                                 spawns[randomSpawn].rotation);
        
        Invoke(nameof(SpawnEnemy), 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < spawnsWE.Count; i++)
            {
                PhotonNetwork.Instantiate("WalkEnemy", spawnsWE[i].position, spawnsWE[i].rotation);
            }
            for (int i = 0; i < spawnsTurret.Count; i++)
            {
                PhotonNetwork.Instantiate("Turret", spawnsTurret[i].position, spawnsTurret[i].rotation);
            }
        }
    }
}
