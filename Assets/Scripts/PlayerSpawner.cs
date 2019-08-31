using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject[] players;

    void Awake()
    {
        Instantiate(players[GameManager.gm.characterIndex], transform.position, Quaternion.identity);   
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
