using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MissonType
{
    SingleRun, TotalMeter, FishBonesSingleRun
}

public abstract class MissionBase : MonoBehaviour
{
    public int max;
    public int progress;
    public int reward;
    public Player player;
    public int currentProgress;
    public MissonType missonType;

    public abstract void Created();
    public abstract string GetMissionDescription();
    public abstract void RunStart();
    public abstract void Update();

    public bool GetMissionComplete()
    {
        if ((progress + currentProgress) >= max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class SingleRun : MissionBase
{
    public override void Created()
    {
        missonType = MissonType.SingleRun;
        int[] maxValues = { 1000, 2000, 3000, 4000 };
        int randomMaxValue = Random.Range(0, maxValues.Length);
        int[] rewards = { 100, 200, 300, 400 };
        reward = rewards[randomMaxValue];
        max = maxValues[randomMaxValue];
        progress = 0;
    }

    public override string GetMissionDescription()
    {
        return "Run " + max + "m in one run";
    }

    public override void RunStart()
    {
        progress = 0;
        player = FindObjectOfType<Player>();
    }

    public override void Update()
    {
        if (player == null)
            return;

        progress = (int)player.score;


    }
}

public class TotalMeters : MissionBase
{
    public override void Created()
    {
        missonType = MissonType.TotalMeter;
        int[] maxValues = { 1000, 2000, 3000, 4000 };
        int randomMaxValue = Random.Range(0, maxValues.Length);
        int[] rewards = { 100, 200, 300, 400 };
        reward = rewards[randomMaxValue];
        max = maxValues[randomMaxValue];
        progress = 0;
    }

    public override string GetMissionDescription()
    {
        return "Run for " + max + "m total";
    }

    public override void RunStart()
    {
        progress += currentProgress;
        player = FindObjectOfType<Player>();
    }

    public override void Update()
    {
        if (player == null)
            return;

        currentProgress = (int)player.score;
    }
}

public class FishBonesSingleRun : MissionBase
{
    public override void Created()
    {
        missonType = MissonType.FishBonesSingleRun;
        int[] maxValues = { 100, 200, 300, 400, 500 };
        int randomMaxValue = Random.Range(0, maxValues.Length);
        int[] rewards = { 100, 200, 300, 400, 500 };
        reward = rewards[randomMaxValue];
        max = maxValues[randomMaxValue];
        progress = 0;
    }

    public override string GetMissionDescription()
    {
        return "Collect " + max + "fisbones in one run";
    }

    public override void RunStart()
    {
        progress += currentProgress;
        player = FindObjectOfType<Player>();
    }

    public override void Update()
    {
        if (player == null)
            return;

        progress = (int)player.fishBone;
    }
} 