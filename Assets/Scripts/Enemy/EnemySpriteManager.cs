using UnityEngine;

public class EnemySpriteManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public Animator anim;
    public SpriteRenderer sprite;
    public float rotatorXawOffsetDeg;
    public float rotatorLerpSpeed = 1;

    public bool isFront = false;
    public bool isSide  = false;
    public bool isBack  = false;
    public bool isLeft = false;

    public bool sideFaceLeft = false;

    void Start() {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update() {
        CheckSpriteRotation();
        RotateToFacePlayer();
    }

    private void CheckSpriteRotation() {
        isFront = false;
        isSide = false;
        isBack = false;
        Transform parent        = transform.parent != null ? transform.parent : transform;
        Quaternion parentRot    = parent.rotation;

        // XXX maybe store this so it isn't
        Vector3 dirParRot   = parentRot * Vector3.forward;
        Vector3 dirToPlayer = playerTransform.position - parent.position;
        dirParRot.y     = 0f;
        dirToPlayer.y   = 0f;
        dirParRot   = dirParRot.normalized;
        dirToPlayer = dirToPlayer.normalized;


        float angle = Vector3.Angle(dirParRot, dirToPlayer);

        if (angle < 45) isFront = true;
        else if (angle < 135) isSide = true;
        else isBack = true;

        anim.SetBool("isFront", isFront);
        anim.SetBool("isSide", isSide);
        anim.SetBool("isBack", isBack);

        if (isSide) {
            float crossY = Vector3.Cross(dirParRot, dirToPlayer).y;
            isLeft = crossY < 0f;
            sprite.flipX = isLeft != sideFaceLeft;
        }
        else {
            sprite.flipX = false;
        }
    }

    private void RotateToFacePlayer()
    {
        /*If the player is not found, ignore this function*/
        if (playerTransform == null) return;

        /*Find the direction of the player*/
        Vector3 target = playerTransform.position;
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        /*Angle of which to look at the player (plus a given offset)*/
        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        yaw += rotatorXawOffsetDeg;

        /*Rotate towards the player with the new "yaw" angle*/
        Quaternion desired = Quaternion.Euler(0f, yaw, 0f);
        float alpha = 1f - Mathf.Exp(-rotatorLerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired, alpha);
    }
}