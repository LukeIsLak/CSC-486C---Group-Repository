using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public TextMeshPro textMeshPro;
    public GameObject graphicsContainer;

    [Header("Animation Parameters")]
    public float initialSpeed = 1f;
    public float initialY     = 1f;
    public float downwardPull = 1f;
    public float duration    = 0.5f;
    private float curTime   = 0f;

    private Vector3 curVel  = Vector3.zero;
    public void DoDamage(float damage)
    {
        graphicsContainer.SetActive(true);
        textMeshPro.text = "-" + damage.ToString();
        StartCoroutine(FloatUpThenDelete());
    }

    public IEnumerator FloatUpThenDelete()
    {
        // Go to left if forward was towards player
        GameObject player =  GameObject.FindGameObjectWithTag("Player");
        if (!player) yield break;

        Vector3 dirToPlayer = player.transform.position - transform.position;
        dirToPlayer.y       = 0f;
        Vector3 startHDir   = Vector3.Cross(Vector3.up, dirToPlayer);
        curVel              = (startHDir.normalized + Vector3.up).normalized * initialSpeed;

        while (curTime < duration)
        {
            float dt = Time.fixedDeltaTime;
            curTime += dt;
            transform.position += curVel * dt;
            curVel.y -= downwardPull * dt;
            yield return new WaitForFixedUpdate();
        }
        Destroy(this.gameObject);
    }
}
