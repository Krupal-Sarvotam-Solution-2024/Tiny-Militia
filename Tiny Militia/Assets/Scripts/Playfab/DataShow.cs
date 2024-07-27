using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataShow : MonoBehaviour
{
    public static DataShow Instance;

    public int High_Score_Count;



    private void Awake()
    {
        Instance = this;
        if(Instance != null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

   

    public void SetApperanceHighScore(int HighScore)
    {
        Menu.Instance.High_Score.text = HighScore.ToString();
        High_Score_Count = HighScore;
    }

    public void SetApperancePlayerName()
    {
        Menu.Instance.PlayerName.text = Menu.Instance.player_nametext.text;
    }
}
