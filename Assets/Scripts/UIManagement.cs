using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour
{
    [SerializeField] Image[] heartAmount;
    [SerializeField] Text rewardText;
    public GameObject gameOverPanel;
    public Text score;
    public void UpdateHealth (int health)
    {
        for (int i = 0; i < heartAmount.Length; i++)
        {
            if (health > i)
            {
                heartAmount[i].color = Color.white;
            }
            else
            {
                heartAmount[i].color = Color.black;
            }
        }
    }

    public void UpdateReward (int fishbone)
    {
        rewardText.text = fishbone.ToString();
    }

    public void UpdateScore(int s)
    {
        score.text = "Score: " + s + "m";
    }
}
