using System.Collections;
using System.IO;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Launch : MonoBehaviour
{
    private string ballPath;

    [SerializeField]
    private GameObject diedPanel;

    Color newCol, newColTwo, newColThree;

    [SerializeField]
    private List<Button>buttons;


    private GameObject theBullet;

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject throwlocation;

    float rockImpulse = 10f; //speed

    [SerializeField]
    private Material[] matList;

    [SerializeField]
    private TMP_Text scoreText, highScoreText;

   
    public static bool playerDied = false;
    
    private int score, ballCount, highScore, randomIndexButton, wrongButton, wrongMat, wrongSecondButton, wrongSecondMat, increment=1;

    int ranIndex;

    Material theMat;

    string json;


    List<Color> colorList = new List<Color>()
    {
        Color.red,
        Color.green,
        Color.yellow,
        new Color(178F, 0F, 255F),
        Color.blue
    };

    int count, randomIndex;
    private BallList ballList = new BallList();

    private void Start()
    {
       // Color newCol = new Color();
       // Color newColTwo = new Color();
       // Color newColThree = new Color();
       
        ballPath = Application.persistentDataPath + "/AssetsballData.json";
        Debug.Log(Application.persistentDataPath + "/AssetsballData.json");
        if(!File.Exists(Application.persistentDataPath + "/AssetsballData.json"))
        {

            Load();
        }
        using (StreamReader stream = new StreamReader(ballPath))
        {
           json = stream.ReadToEnd();
           ballList = JsonUtility.FromJson<BallList>(json);
        }
        StartCoroutine(LaunchBall());
        highScore = ballList.highScore;



    }

    void Update()
    {
        scoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }
    public IEnumerator LaunchBall()
    {
        while(!playerDied)
        {

            ranIndex = ColorCheck();
            theBullet = (GameObject)Instantiate(bullet, throwlocation.transform.position + throwlocation.transform.forward, throwlocation.transform.rotation);
            Debug.Log("the random index is in the following" + ranIndex);
            theBullet.GetComponent<Renderer>().material.color = ballList.ballList[ranIndex].color;
            theMat = theBullet.GetComponent<Renderer>().material;
            Debug.Log("The ball's material index is: " + theMat);
            theBullet.GetComponent<Rigidbody>().AddForce(throwlocation.transform.forward * rockImpulse, ForceMode.Impulse);
            yield return new WaitForSeconds(2f);
            if (ballCount % 6 == 0 && ballCount >= 6)
            {
                increment *= 2;
            }
            ballCount++;
            score += increment;
            rockImpulse += 1f;


        }
        Died();

    }

    [ContextMenu("Reset High Score")]

    private void Reset()
    {
        var ballProjectile = new BallList();
        ballProjectile.ballList = new List<Ball>
        {
            new Ball{name = "Red", mat = matList[0], color = matList[0].color, speed = 5f},
            new Ball{name = "Blue", mat = matList[1], color = matList[1].color, speed = 5f},
            new Ball{name = "Green", mat = matList[2], color = matList[2].color, speed = 5f},
            new Ball{name = "Yellow", mat = matList[3], color = matList[3].color, speed = 5f},
            new Ball{name = "Purple", mat = matList[4], color = matList[4].color, speed = 5f},
           
    };
        ballProjectile.highScore = 0;
        string json = JsonUtility.ToJson(ballProjectile);
        Debug.Log(json);

        File.WriteAllText(Application.persistentDataPath + "/AssetsballData.json", json);
    }

    void Load()
    {
        var ballProjectile = new BallList();
        ballProjectile.ballList = new List<Ball>
        {
            new Ball{name = "Red", mat = matList[0], color = matList[0].color, speed = 5f},
            new Ball{name = "Blue", mat = matList[1], color = matList[1].color, speed = 5f},
            new Ball{name = "Green", mat = matList[2], color = matList[2].color, speed = 5f},
            new Ball{name = "Yellow", mat = matList[3], color = matList[3].color, speed = 5f},
            new Ball{name = "Purple", mat = matList[4], color = matList[4].color, speed = 5f},

    };
        ballProjectile.highScore = highScore;
        string json = JsonUtility.ToJson(ballProjectile);
        Debug.Log(json);
        Debug.Log("File written!");
        Debug.Log("Path Name: " + Application.persistentDataPath + "/AssetsballData.json");
        File.WriteAllText(Application.persistentDataPath + "/AssetsballData.json", json);
    }


    void Died()
    {
        score-=increment;
        diedPanel.SetActive(true);
        if(highScore < score)
        {
            highScore = score;
            ballList.highScore = highScore;
           json = JsonUtility.ToJson(ballList);
            Debug.Log("Died json! " + json);
           File.WriteAllText(Application.persistentDataPath + "/AssetsballData.json", json);
        }
    }

    public void TryAgain()
    {
        playerDied = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public int ColorCheck()
    {
        randomIndex = Random.Range(0, ballList.ballList.Count);
        randomIndexButton = Random.Range(0, 3);
        Debug.Log("correct random index is " + randomIndex);
        newCol = ballList.ballList[randomIndex].color;
        Debug.Log("successfully made it past color!");
        buttons[randomIndexButton].GetComponent<Image>().color = newCol;
        wrongButton = RandomExceptList(0, 3, new List<int>() { randomIndexButton});
        wrongMat = RandomExceptList(0,ballList.ballList.Count, new List<int>() { randomIndex });
        Debug.Log(wrongMat);
        newColTwo = ballList.ballList[wrongMat].color;
        buttons[wrongButton].GetComponent<Image>().color = newColTwo;
        wrongSecondButton = RandomExceptList(0,3, new List<int>(){ randomIndexButton, wrongButton });
        Debug.Log("after all that" + wrongMat);
        Debug.Log("the current ball list count" + ballList.ballList.Count);
        wrongSecondMat = RandomExceptList(0,ballList.ballList.Count, new List<int>() { randomIndex, wrongMat });
        Debug.Log(wrongSecondMat);
        newColThree = ballList.ballList[wrongSecondMat].color;
        buttons[wrongSecondButton].GetComponent<Image>().color = newColThree;
        return randomIndex;
    }

    public void ButtonClick(Button btn)
    {
        if(btn.GetComponent<Image>().color == theMat.color)
        {
            Destroy(theBullet);
        }
    }



    private int RandomExceptList(int fromNr, int exclusiveToNr, List<int> exceptNr)
    {
       
        int randomNr = exceptNr[0];

    
        while (exceptNr.Contains(randomNr))
        {
            randomNr = UnityEngine.Random.Range(fromNr, exclusiveToNr);
        }

      
        return randomNr;
    }

}
