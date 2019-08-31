using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject[] obstacles;
    [SerializeField] Vector2 numberOfObstacles;
    [SerializeField] GameObject reward;
    [SerializeField] Vector2 numberOfStars;

    [SerializeField] List<GameObject> newObstacles;
    [SerializeField] List<GameObject> newRewards;

    // Start is called before the first frame update
    void Start()
    {
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y);
        int newNumberOfRewards = (int)Random.Range(numberOfStars.x, numberOfStars.y);

        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            newObstacles[i].SetActive(false);
        }

        for (int i = 0; i < newNumberOfRewards; i++)
        {
            newRewards.Add(Instantiate(reward, transform));
            newRewards[i].SetActive(false);
        }
        GenerateMap();
        GenerateReward();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMap()
    {
        for (int i = 0; i < newObstacles.Count; i++)
        {
            float positionZMin = (309f / newObstacles.Count) + (309f / newObstacles.Count) * i;
            float positionZMax = (309f / newObstacles.Count) + (309f / newObstacles.Count) * i + 1;
            newObstacles[i].transform.localPosition = new Vector3(0, 0, Random.Range(positionZMin, positionZMax));
            newObstacles[i].SetActive(true);
            if(newObstacles[i].GetComponent<ChangeLane>() != null)
            {
                newObstacles[i].GetComponent<ChangeLane>().PositionLane();
            }
        }
    }

    void GenerateReward()
    {
        float minZPos = 10f;
        for (int i = 0; i < newRewards.Count; i++)
        {
            float maxZPos = minZPos + 5f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            newRewards[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZPos);
            newRewards[i].SetActive(true);
            newRewards[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().IncreaseSpeed();
            transform.position = new Vector3(0, 0, transform.position.z + 309 * 2);
            GenerateMap();
            GenerateReward();
        }
    }
}
