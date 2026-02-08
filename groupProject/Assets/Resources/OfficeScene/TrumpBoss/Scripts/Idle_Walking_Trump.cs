using Cinemachine;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class NPC_CineMove : MonoBehaviour
{
    public CinemachineDollyCart cart;
    public Animator anim;
    public AudioSource audioSource;

    public AudioClip welcomeClip;
    public AudioClip workplaceClip;

    public float walkSpeed = 2f;
    public float stopPoint1 = 4f;
    //public float stopPoint2 = 4f;
    private int Stage = 0;

    private bool moving = false;

    void Start()
    {
        // start idle talking
        cart.m_Speed = 0f;
        anim.SetBool("isWalking", false);
    }

    public void TriggerStartWelcome()
    {
        audioSource.clip = welcomeClip;
        audioSource.Play();
        Invoke("StartMoving", audioSource.clip.length);
    }

    public void TriggerWorkplaceClip()
    {
        audioSource.clip = workplaceClip;
        audioSource.Play();
    }

    public void WorkstationMove()
    {
        Stage = 1;
        Invoke("StartMoving", 0f);
    }    

    void StartMoving()
    {
        moving = true;
        cart.m_Speed = walkSpeed;
        anim.SetBool("isWalking", true);
    }

    void Update()
    {
        if (moving && cart.m_Position >= stopPoint1 && Stage == 0)
        {
            // reached first stop point
            cart.m_Speed = 0f;
            anim.SetBool("isWalking", false);
            moving = false;
        }


        if (moving && cart.m_Position >= cart.m_Path.PathLength && Stage == 1)
        {
            // reached end of path
            cart.m_Speed = 0f;
            anim.SetBool("isWalking", false);
            moving = false;
        }
    }
}
