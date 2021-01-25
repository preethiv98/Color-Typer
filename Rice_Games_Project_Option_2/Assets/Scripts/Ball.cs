using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ball
{
    public string name;

    public Color color;

    public Material mat;

    public float speed;
}

[System.Serializable]
public class BallList
{
    public List<Ball> ballList;

    public int highScore;
}