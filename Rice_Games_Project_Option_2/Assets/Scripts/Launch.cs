using System.Collections;
using System.IO;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using BallData;

public class Launch : MonoBehaviour
{

    [SerializeField]
    private GameObject diedPanel;

    Color newCol, newColTwo, newColThree; //the colors that are assigned to each button

    [SerializeField]
    private List<Button>buttons; //serialize a list of buttons

    public HighScore highScoreObject;

    private GameObject theBullet; //private gameobject that I used to assign colors to the material.

    [SerializeField]
    private GameObject bullet; //gets the prefab bullet that is being shot by the cannon
    [SerializeField]
    private GameObject throwlocation; //location of where it is getting launched

    float rockImpulse = 10f; //speed

    public static bool playerDied = false; //check if the player died
    
    /*
     * Fetches number of balls that are shot, indexes of both buttons and the ball data list, and the wrong color/buttons for the other two buttons. 
     * Also has the increment that doubles every 6 balls guessed correctly.
     */
    private int ballCount, randomIndexButton, wrongButton, wrongColor, wrongSecondButton, wrongSecondColor, increment=1;
    

    int ranIndex, randomIndex; //used for the color check method

    string json, name;

    private InitalizeJson manager = new InitalizeJson(); //instantiate 

    private void Start()
    {
        manager.Initialize(); //loads the JSON file in persistent data files.
        GameManager.highScore = manager.highScore.highScore; //sets the high score from the JSON file
        StartCoroutine(LaunchBall()); //starts the coroutine of launching balls
    }

    public IEnumerator LaunchBall()
    {
        while(!playerDied) //while the player hasn't been hit with the ball
        {

            ColorCheck(); //this method returns the index of the ball that is currently launching
            theBullet = (GameObject)Instantiate(bullet, throwlocation.transform.position + throwlocation.transform.forward, throwlocation.transform.rotation);
            //physics instantiating bulelt shot.
            Debug.Log("the random index is in the following" + ranIndex);
            theBullet.GetComponent<Renderer>().material.color = newCol; //sets the material of the bullet to the correct color

            theBullet.GetComponent<Rigidbody>().AddForce(throwlocation.transform.forward * rockImpulse, ForceMode.Impulse); //sends off the bullet
            yield return new WaitForSeconds(2f); //waits a few seconds so that it doesn't launch again and again
           


        }
        Died(); //called when player dies

    }

  

    void Died() //checks if the player has died
    {

        diedPanel.SetActive(true); //pulls up died panel
        
        if(manager.highScore.highScore <= GameManager.score) //if the high score in the JSON file is less than the current score
        {
            manager.highScore.highScore = GameManager.score; //sets the current score of the game to the high score
            GameManager.highScore = manager.highScore.highScore;
           json = JsonUtility.ToJson(manager.highScore);
           File.WriteAllText(Application.persistentDataPath + "/AssetsHighScore.json", json); //Writes that new high score to the JSON file
        }
        ballCount = 0;
        GameManager.score = 0; //resets score back to zero
    }

    public void TryAgain() //Resets the game
    {
        playerDied = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit() //Quits the game
    {
        Application.Quit();
    }

    Color GetColor(string name) //Assigns a Color color based on the string name that is passed by the ball data.
    {

        switch (name)
        {
            case "Red":
                {
                    newCol = Color.red;
                    break;
                }
            case "Blue":
                {
                    newCol = Color.blue;
                    break;
                }
            case "Green":
                {
                    newCol = Color.green;
                    break;
                }
            case "Yellow":
                {
                    newCol = Color.yellow;
                    break;
                }
            case "Purple":
                {
                    newCol = Color.magenta;
                    break;
                }
        }
        return newCol;
    }

    public void ColorCheck() //Fetches a random index that is the correct color, and assigns random indexes for wrong colors for the buttons
    {
       

        randomIndex = Random.Range(0, manager.ballList.ballList.Count); //gets a random index to pick a random color 
        name = manager.ballList.ballList[randomIndex].name; //set the name to a value in the JSON ball data
        newCol = GetColor(name); //changes string name to a Color

        randomIndexButton = Random.Range(0, 3); //gets a random index of buttom
        
        buttons[randomIndexButton].GetComponent<Image>().color = newCol; //sets that color to the button on that index

        //Fetches a button that is not the correct button, and a color from the ballList that is not the correct color.
        wrongButton = RandomExceptList(0, 3, new List<int>() { randomIndexButton});
        wrongColor = RandomExceptList(0, manager.ballList.ballList.Count, new List<int>() { randomIndex });

        name = manager.ballList.ballList[wrongColor].name; //set the name from the ball data
        newColTwo = GetColor(name); //get the specific color of that name
        buttons[wrongButton].GetComponent<Image>().color = newColTwo; //sets one of the other buttons to that color
        /*Fetches a button that is not the correct button or the other wrong button, and a color from the ballList that is not the correct color
         * or wrong button
         */
        wrongSecondButton = RandomExceptList(0,3, new List<int>(){ randomIndexButton, wrongButton }); 

        //gets a random second color that is not the other two colors already fetched
        wrongSecondColor = RandomExceptList(0,manager.ballList.ballList.Count, new List<int>() { randomIndex, wrongColor }); 
   
        name = manager.ballList.ballList[wrongSecondColor].name;
        newColThree = GetColor(name);
        buttons[wrongSecondButton].GetComponent<Image>().color = newColThree;

    }

    //Function that destroys gameobject and incraeses score if the player clicks on the right button
    public void ButtonClick(Button btn)
    {
        if(btn.GetComponent<Image>().color == newCol) //the button color matches the color of the bullet being shot
        {
            Destroy(theBullet); //destroy the bullet
            if (ballCount % 6 == 0 && ballCount >= 6) //if there have been 6 successful picks in a row.
            {
                increment *= 2;
            }
            ballCount++; //increase ball count
            GameManager.score = GameManager.score + increment; //increment the score
            rockImpulse += 1f; //makes the ball faster each time you guess it correctly.
        }
    }


    //Helper function created that takes in a list and gets a random value EXCEPT for the values in that range
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
