using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossScript : EnemyScript
{
    [SerializeField] private int health = 5, maxHealth;
    private float attackTimer;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject confetti;

    void Start()
    {
        maxHealth = health;
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void Update()
    {
        if (state != EnemyState.DEATH)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= attentionDistance)
            {
                rotationDirection = player.transform.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotationDirection), 6f * Time.deltaTime);
            }
        }
        switch (state)
        {
            case EnemyState.IDLE:
                if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
                {
                    state = EnemyState.ATTACK;
                }
                else
                {
                    anim.Play("Idle");
                }
                break;
            case EnemyState.ATTACK:
                attackTimer -= Time.deltaTime * 30;
                if (attackTimer <= 0)
                {
                    anim.Play("Attack");
                }
                else
                {
                    anim.Play("Idle");
                }
                break;
            case EnemyState.DEATH:
                if (transform.position.z >= player.transform.position.z)
                {
                    switch (UpgradeHandler.Instance.GetWeapon())
                    {
                        case WeaponType.CUTTING:
                            if (deathType <= 50)
                                anim.Play("DeathHalf");
                            else
                                anim.Play("DeathHead");
                            break;
                        case WeaponType.BLUNT:
                            anim.Play("DeathExplosion");
                            break;
                        default:
                            anim.Play("Death");
                            break;
                    }
                }
                break;
        }
    }

    protected override void Hit()
    {
        player.GetComponent<PlayerController>().GetDirectDamage(20);
    }

    protected override void AttackEnd()
    {
        attackTimer = 30;
    }

    public override void AddLevel()
    {
        
    }

    public override int GetHealth()
    {
        return health;
    }

    public override void Death()
    {
        health--;
        if (healthBar == null)
            healthBar = GameObject.Find("HealthBoss").GetComponent<Image>();
        healthBar.DOFillAmount((float)health / maxHealth, 0.25f);
        Instantiate(blood, transform.position, blood.transform.rotation);
        if (health <= 0)
        {
            Time.timeScale = 0.25f;
            if (UpgradeHandler.Instance.GetWeapon() == WeaponType.BLUNT)
                Instantiate(headExplosion, transform.position + Vector3.up * 0.5f, headExplosion.transform.rotation);
            Invoke("Confetti", 1f);
            MR.material.DOColor(deathMat.color, 0.15f).OnComplete(() =>
            {
                var s = Instantiate(splash, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
                s.transform.SetParent(transform);
                s.transform.DOScale(0.24f, 0.35f);
            });
            state = EnemyState.DEATH;
            deathType = Random.Range(1, 101);
        }
    }

    private void Confetti()
    {
        Instantiate(confetti, transform.position, confetti.transform.rotation);
        GameHandler.Instance.Victory();
    }
}
