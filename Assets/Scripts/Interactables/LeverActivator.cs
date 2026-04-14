using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverActivator : MonoBehaviour
{
    public Animator animator;
    public void SetAnimTrigger()
    {
        animator.SetTrigger("PullLever");
    }
}
