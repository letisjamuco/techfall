using UnityEngine;

public class HeightDetector : MonoBehaviour
{
    public float height { get; private set; }

    // Update is called once per frame
    private void Update()
    {
        height = GetComponent<Renderer>().bounds.max.y;
    }


}
