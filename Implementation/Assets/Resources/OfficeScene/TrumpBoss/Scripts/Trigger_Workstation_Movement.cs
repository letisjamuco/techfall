using UnityEngine;

public class Trigger_Workstation_Movement : MonoBehaviour
{
    public NPC_CineMove npc;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {

        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            npc.WorkstationMove();
        }
    }
}
