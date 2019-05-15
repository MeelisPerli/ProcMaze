using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Text scoreField;
    private int score;

    public GameObject endScreen;
    public Text endScoreField;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        reset();
        Cursor.visible = false;
    }

    public void reset() {
        score = 0;
        setScore(0);
    }

    private void setScore(int score) {
        scoreField.text = "Score: " + score;
    }


    public void increaseScore() {
        score++;
        setScore(score);
    }

    public void gameOver() {
        endScreen.SetActive(true);
        endScoreField.text = "Score: " + score;
        Invoke("restart", 2);
    }

    public void restart() {
        SceneManager.LoadScene("SampleScene");
    }
}
