using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;

    int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = score.ToString();
    }

    private void Update()
    {
        StackHeightCalculator.instance.UpdateMaxHeight();
        score = (int)StackHeightCalculator.instance.maxHeight * 100;
        Debug.Log("Score is " + score);
        if (score > 100)
        {
            Debug.Log("HIGH SCORE");
        }
    }
}
