using Cinemachine;
using UnityEngine;
using UnityEngine.Audio;

public class NPC_CineMove : MonoBehaviour
{
    public CinemachineDollyCart cart;
    public Animator anim;
    public AudioSource audioSource;

    // public AudioClip welcomeClip; // assign in inspector

    public float walkSpeed = 2f;

    private bool moving = false;

    void Start()
    {
        // start idle talking
        cart.m_Speed = 0f;
        anim.SetBool("isWalking", false);
    }

    public void TriggerStartWelcome()
    {
        //audioSource.clip =  assign the audio clip for his welcome talk here in the inspector
        audioSource.Play();
        Invoke("StartMoving", audioSource.clip.length);
    }

    public void TriggerWorkplaceClip()
    {
        //audioSource.clip =  assign the audio clip for his welcome talk here in the inspector
        audioSource.Play();
    }

    void StartMoving()
    {
        moving = true;
        cart.m_Speed = walkSpeed;
        anim.SetBool("isWalking", true);
    }

    void Update()
    {
        if (moving && cart.m_Position >= cart.m_Path.PathLength)
        {
            // reached end of path
            cart.m_Speed = 0f;
            anim.SetBool("isWalking", false);
            moving = false;
        }
    }
}
