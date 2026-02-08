using UnityEngine;

public class AITakeOverTransform : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 60f, 0f);
    public int duration;
    public AudioSource AIaudio;

    private float startTime, stopTime;

    public void rotation()
    {
        startTime = Time.time;
        stopTime = startTime + duration;
        Debug.Log("START TIME = " + Time.time);
        while (Time.time <= stopTime)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
            // AIaudio.Play();
        }
        Debug.Log("STOP TIME = " + Time.time);
        QuitApplication.instance.QuitApp(); 
    }

}
