using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowmanScript : EnemyScript
{
    [SerializeField] private ArrowScript arrow;
    [SerializeField] private float meleeDistance = 1.5f;

    public override void Update()
    {
        if (state != EnemyState.DEATH)
        {
            cross.SetActive(level > UpgradeHandler.Instance.GetLevel());
            if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
            {
                rotationDirection = player.transform.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotationDirection), 6f * Time.deltaTime);
            }
            HandleUI();
        }
        switch (state)
        {
            case EnemyState.IDLE:
                anim.Play("Idle");
                if (player.transform.position.z < transform.position.z && Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
                {
                    state = EnemyState.ATTACK;
                }
                break;
            case EnemyState.ATTACK:
                if (player.transform.position.z > transform.position.z)
                {
                    state = EnemyState.IDLE;
                }
                else
                {
                    if (Vector3.Distance(transform.position, player.transform.position) > meleeDistance)
                        anim.Play("Shooting");
                    else
                        anim.Play("Attack");
                }
                break;
            case EnemyState.DEATH:
                anim.Play("Death");
                break;
        }
    }

    protected override void Hit()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > meleeDistance)
            Instantiate(arrow, transform.position + new Vector3 (0, 0.75f, 0) * 1.5f, arrow.transform.rotation).SetTarget(player.transform.position, level + 1);
        else
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
            {
                player.GetComponent<PlayerController>().GetDamage(level);
            }
        }
        state = EnemyState.IDLE;
    }
}
