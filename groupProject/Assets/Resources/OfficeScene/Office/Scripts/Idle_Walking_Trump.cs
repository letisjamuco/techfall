using UnityEngine;
using Cinemachine;

public class NPC_CineMove : MonoBehaviour
{
    public CinemachineDollyCart cart;
    public Animator anim;

    public float walkSpeed = 2f;
    public float startDelay = 4f; // time he talks before walking

    private bool moving = false;

    void Start()
    {
        // start idle talking
        cart.m_Speed = 0f;
        anim.SetBool("isWalking", false);

        Invoke("StartMoving", startDelay);
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
