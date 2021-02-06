using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using BallData;
using TMPro;


/*
 * Updates the score and high score in the 
 * 
 */
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText, highScoreText;

    public static int score; //so that this directly get changed in Launch.cs

    public static int highScore;

    void Update() //updates score and high score in Canvas
    {
        scoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }
}

