using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerData
{
    public int fishbones;
    public int[] max;
    public int[] progress;
    public int[] currentProgress;
    public int[] reward;
    public string[] missionType;
    public int[] characterCost;
}

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public int fishbones;
    public int[] characterCost;

    public int characterIndex;

    private MissionBase[] missions;
    private string filePath;

    private void Awake()
    {
        if(gm == null)
        {
            gm = this;
        }
        else if(gm != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        filePath = Application.persistentDataPath + "/playerInfo.dat";

        missions = new MissionBase[2];

        if (File.Exists(filePath))
        {
            Load();
        }

        else
        {
            for (int i = 0; i < missions.Length; i++)
            {
                GameObject newMission = new GameObject("Mission" + i);
                newMission.transform.SetParent(transform);
                MissonType[] missionType = { MissonType.SingleRun, MissonType.TotalMeter, MissonType.FishBonesSingleRun };
                int randomType = Random.Range(0, missionType.Length);
                if (randomType == (int)MissonType.SingleRun)
                {
                    missions[i] = newMission.AddComponent<SingleRun>();
                }
                else if (randomType == (int)MissonType.TotalMeter)
                {
                    missions[i] = newMission.AddComponent<TotalMeters>();
                }
                else if (randomType == (int)MissonType.FishBonesSingleRun)
                {
                    missions[i] = newMission.AddComponent<FishBonesSingleRun>();
                }
                missions[i].Created();
            }
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);

        PlayerData data = new PlayerData();

        data.fishbones = fishbones;

        data.max = new int[2];
        data.progress = new int[2];
        data.currentProgress = new int[2];
        data.reward = new int[2];
        data.missionType = new string[2];
        data.characterCost = new int[characterCost.Length];

        for(int i = 0; i < 2; i++)
        {
            data.max[i] = missions[i].max;
            data.progress[i] = missions[i].progress;
            data.currentProgress[i] = missions[i].currentProgress;
            data.reward[i] = missions[i].reward;
            data.missionType[i] = missions[i].missonType.ToString();
        }

        for(int i = 0; i < characterCost.Length; i++)
        {
            data.characterCost[i] = characterCost[i];
        }

        bf.Serialize(file, data);
        file.Close();

    }

    void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filePath, FileMode.Open);

        PlayerData data = (PlayerData)bf.Deserialize(file);
        file.Close();

        fishbones = data.fishbones;

        for(int i = 0; i < 2; i++)
        {
            GameObject newMission = new GameObject("Mission" + i);
            newMission.transform.SetParent(transform);
            if(data.missionType[i] == MissonType.SingleRun.ToString())
            {
                missions[i] = newMission.AddComponent<SingleRun>();
                missions[i].missonType = MissonType.SingleRun;
            }
            else if (data.missionType[i] == MissonType.TotalMeter.ToString())
            {
                missions[i] = newMission.AddComponent<TotalMeters>();
                missions[i].missonType = MissonType.TotalMeter;
            }
            else if (data.missionType[i] == MissonType.FishBonesSingleRun.ToString())
            {
                missions[i] = newMission.AddComponent<FishBonesSingleRun>();
                missions[i].missonType = MissonType.FishBonesSingleRun;
            }

            missions[i].max = data.max[i];
            missions[i].progress = data.progress[i];
            missions[i].currentProgress = data.currentProgress[i];
            missions[i].reward = data.reward[i];
        }

        for(int i = 0; i < data.characterCost.Length; i++)
        {
            characterCost[i] = data.characterCost[i];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRun(int charIndex)
    {
        characterIndex = charIndex;
        SceneManager.LoadScene("Scene1");
    }

    public void Endrun()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public MissionBase GetMission(int index)
    {
        return missions[index];
    }

    public void StartMissions()
    {
        for(int i = 0; i < 2; i++)
        {
            missions[i].RunStart();
        }
    }

    public void GenerateMission(int i)
    {
        Destroy(missions[i].gameObject);

        GameObject newMission = new GameObject("Mission" + i);
        newMission.transform.SetParent(transform);
        MissonType[] missionType = { MissonType.SingleRun, MissonType.TotalMeter, MissonType.FishBonesSingleRun };
        int randomType = Random.Range(0, missionType.Length);
        if (randomType == (int)MissonType.SingleRun)
        {
            missions[i] = newMission.AddComponent<SingleRun>();
        }
        else if (randomType == (int)MissonType.TotalMeter)
        {
            missions[i] = newMission.AddComponent<TotalMeters>();
        }
        else if (randomType == (int)MissonType.FishBonesSingleRun)
        {
            missions[i] = newMission.AddComponent<FishBonesSingleRun>();
        }
        missions[i].Created();

        FindObjectOfType<Menu>().SetMission();
    }
}
