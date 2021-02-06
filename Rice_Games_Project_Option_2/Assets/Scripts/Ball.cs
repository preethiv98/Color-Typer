using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

/*
 * Initalizes ball data (name) that is exported as a JSON file.
 */
namespace BallData
{
    [System.Serializable]
    public class Ball
    {
        public string name;
    }

    [System.Serializable]
    public class BallList
    {
        public List<Ball> ballList;
        //a list of balls with color names

    }

    [System.Serializable]
    public class HighScore
    {
        //Running tally on scores
        public int highScore;
    }

    [System.Serializable]
    public class InitalizeJson //Reads, and loads JSON files
    {
        private string ballPath; //the path of the JSON files
        string json;

        public BallList ballList = new BallList(); // create a new object that contains list of colors the player can select from
    

        public int score;

        public HighScore highScore;


        public void Initialize()
        {
            /*
             * Takes the json files saved to the persistent data path and loads it in game. Meant to work for both the build and the editor.
             * 
             */
            
            ballPath = Application.persistentDataPath + "/AssetsballData.json";

            Debug.Log(Application.persistentDataPath + "/AssetsballData.json");
            if (!File.Exists(Application.persistentDataPath + "/AssetsballData.json") || !File.Exists(Application.persistentDataPath + "/AssetsHighScore.json")) 
            {
                /*
                 * checks to see if there doesn't exist both a High Score and Ball Data file.
                 * This is just a fail safe in case the user does not have either file.
                 */
                Load();
            }
            using (StreamReader stream = new StreamReader(ballPath)) //reads the path for the Ball JSON file
            {
                json = stream.ReadToEnd();
                ballList = JsonUtility.FromJson<BallList>(json);
            }
            ballPath = Application.persistentDataPath + "/AssetsHighScore.json";
            using (StreamReader stream = new StreamReader(ballPath)) //reads the path for the High Score json file
            {
                json = stream.ReadToEnd();
                highScore = JsonUtility.FromJson<HighScore>(json);
            }




        }

        [ContextMenu("Reset")]

        /*
         * A method I created just to reset the colors and the high score in the editor. Not necessary for build.
         */
        public void Reset()
        {
            var ballProjectile = new BallList();
            var highScoreCurrent = new HighScore();
            ballProjectile.ballList = new List<Ball>
        {
            new Ball{name = "Red"},
            new Ball{name = "Blue"},
            new Ball{name = "Green"},
            new Ball{name = "Yellow"},
            new Ball{name = "Purple"},

    };
            highScoreCurrent = new HighScore
            {
                highScore = 0
            };


            string json = JsonUtility.ToJson(ballProjectile);
            string jsonHigh = JsonUtility.ToJson(highScoreCurrent);
           

            File.WriteAllText(Application.persistentDataPath + "/AssetsballData.json", json);
            File.WriteAllText(Application.persistentDataPath + "/AssetsHighScore.json", jsonHigh);
           
        }

        public void Load() //Creates new JSON files for if the player does not have the high score json file or the ball data file.
        {
            var ballProjectile = new BallList();
            var highScoreCurrent = new HighScore();
            ballProjectile.ballList = new List<Ball>
        {
            new Ball{name = "Red"},
            new Ball{name = "Blue"},
            new Ball{name = "Green"},
            new Ball{name = "Yellow"},
            new Ball{name = "Purple"},

    };
            highScoreCurrent.highScore = highScore.highScore; //gets the high score stored on the object.

            //write the high score and the ball data to JSON files
            string json = JsonUtility.ToJson(ballProjectile);
            File.WriteAllText(Application.persistentDataPath + "/AssetsballData.json", json);
            json = JsonUtility.ToJson(highScoreCurrent);
            File.WriteAllText(Application.persistentDataPath + "/AssetsHighScore.json", json);

        }


    }

}
