using UnityEngine;

public class RandomizeYRotation : MonoBehaviour
{
    [ContextMenu("Randomize Y Rotation")]
    private void Randomize()
    {
        float y = Random.Range(0f, 360f);
        Vector3 e = transform.eulerAngles;
        transform.eulerAngles = new Vector3(e.x, y, e.z);
    }
}
