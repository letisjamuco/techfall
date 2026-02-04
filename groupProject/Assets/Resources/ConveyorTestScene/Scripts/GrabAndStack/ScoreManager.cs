using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public int highScore;

    int score = 0;
    int lastScore = -1;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score " + score.ToString();
    }

    private void Update()
    {
        StackHeightCalculator.instance.UpdateMaxHeight();
        score = (int)(StackHeightCalculator.instance.maxHeight * 100);
        //update only if score changed
        if (score != lastScore)
        {
            scoreText.text = "Score " + score.ToString();
            lastScore = score;
        }

        if (score > highScore)
        {
            Debug.Log("HIGH SCORE");
        }
    }
}
