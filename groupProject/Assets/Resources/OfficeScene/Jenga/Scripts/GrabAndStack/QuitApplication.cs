using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    public static QuitApplication instance;
    public void QuitApp()
    {
        Application.Quit();
    }
}
