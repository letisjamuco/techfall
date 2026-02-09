using System.Collections;
using UnityEngine;

public class AITakeOverTransform : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 60f, 0f);
    public float duration = 2f;
    public AudioSource AIaudio;

    public void rotation()
    {
        StartCoroutine(RotationRoutine());
    }

    private IEnumerator RotationRoutine()
    {
        float stopTime = Time.time + duration;

        if (AIaudio) AIaudio.Play();

        while (Time.time <= stopTime)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
            yield return null;
        }

        QuitApplication.instance.QuitApp();
    }
}
