using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MoreMountains.NiceVibrations;


public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private int health = 100, maxHealth = 100;
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private CanvasGroup damageCanvas;
    [SerializeField] private bool moving;
    [SerializeField] private int coins;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private EnemyScript enemy;
    [SerializeField] private GameObject blood;

    private Animator anim;
    private Vector3 mousePos, newPos, oldPos, curPos; 
    private float distance;
    private float startPosX, targetPosX;

    [SerializeField] private Image mileProgress;
    [SerializeField] private TMP_Text milesText;
    [SerializeField] private float miles, maxMiles = 1.0f;
    [SerializeField] private float mileTimer;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool EnoughCoins (int _amount)
    {
        return coins >= _amount;
    }

    public void SpendCoins (int _amount)
    {
        coins -= _amount;
        coinsText.text = _amount.ToString();
    }

    private EnemyScript EnemyInFront()
    {
        RaycastHit objectHit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position + Vector3.up, fwd, out objectHit, 1.8f))
        {
            if (objectHit.transform.CompareTag("Enemy"))
            {
                enemy = objectHit.transform.GetComponent<EnemyScript>();
                if (!enemy.Dead())
                    return enemy;
                else
                {
                    enemy = null;
                    return null;
                }
            }
        }
        return null;
    }

    void Update()
    {
        mileTimer -= Time.deltaTime * 50 * GameHandler.Instance.moveSpeed;
        if (mileTimer <= 0)
        {
            miles += 0.1f;
            if (miles > maxMiles)
            {
                miles = 0;
                maxMiles += 0.1f;
                GameHandler.Instance.moveSpeed += 0.1f;
            }
            milesText.text = string.Format("{0:0.0}/{1:0.0} miles", miles, maxMiles);
            mileProgress.DOFillAmount(miles / maxMiles, 0.25f);
            mileTimer = 120;
        }
        Movement();
    }

    private void FixedUpdate()
    {
        if (EnemyInFront() != null)
        {
            if (!anim.GetBool("Hit"))
                anim.SetBool("Attacking", true);
        }
        else
        {
            anim.SetBool("Attacking", false);
            anim.Play(GameHandler.Instance.moveSpeed >= 2.5f ? "Run" : "Walk");
        }
    }

    private void Hit()
    {
        if (enemy != null)
        {
            if (enemy.transform.position.z > transform.position.z)
            {
                if (enemy.GetLevel() <= UpgradeHandler.Instance.GetLevel())
                {
                    AddCoins(enemy.GetLevel() + 1);
                    enemy.Death();
                }
            }
            enemy = null;
        }
    }

    public void GetDamage (int _level)
    {
        if (_level > UpgradeHandler.Instance.GetLevel())
        {
            anim.SetBool("Hit", true);
            int _amount = (_level - UpgradeHandler.Instance.GetLevel()) * 5;
            health -= _amount;
            healthText.text = health.ToString();
            healthBar.DOFillAmount((float)health / maxHealth, 0.25f);
            Instantiate(blood, transform.position, blood.transform.rotation);
            Camera.main.transform.DOShakePosition(0.25f, 0.2f);
            MMVibrationManager.Haptic(HapticTypes.Failure);
            damageCanvas.gameObject.SetActive(true);
            damageCanvas.DOFade (1, 0.05f).OnComplete (() =>
            {
                damageCanvas.DOFade(0, 0.15f).OnComplete(() => damageCanvas.gameObject.SetActive(false));
            });
        }
    }

    private void HitEnd()
    {
        anim.SetBool("Hit", false);
    }

    private void Movement()
    {
        if (!moving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                transform.rotation = Quaternion.identity;
                transform.DOKill();
                startPosX = Input.mousePosition.x;
                moving = true;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                transform.rotation = Quaternion.identity;
                transform.DOKill();
                moving = false;
            }
            else
            {
                curPos = Input.mousePosition;
                curPos.z = 10;
                if (curPos.x != startPosX)
                {
                    mousePos = Input.mousePosition;
                    mousePos.z = 10;

                    newPos = Camera.main.ScreenToWorldPoint(mousePos);

                    distance = newPos.x - oldPos.x;
                    targetPosX = transform.position.x + distance;

                    transform.DOKill();
                    if (distance > 0)
                    {
                        transform.DORotate(new Vector3(0, 15, 0), 0.1f);
                    }
                    else
                    {
                        transform.DORotate(new Vector3(0, -15, 0), 0.1f);
                    }

                    if (targetPosX > 1.5f)
                        targetPosX = 1.5f;
                    if (targetPosX < -1.5f)
                        targetPosX = -1.5f;

                    transform.DOMoveX(targetPosX, Mathf.Abs(distance) * 0.5f).OnComplete(() =>
                    {
                        transform.DOKill();
                        transform.rotation = Quaternion.identity;
                    });
                }
                oldPos = Camera.main.ScreenToWorldPoint(curPos);
                startPosX = curPos.x;
            }
        }
    }

    public void AddCoins (int _amount = 1)
    {
        MMVibrationManager.Haptic(HapticTypes.Success);
        coins += _amount;
        coinsText.text = coins.ToString();
    }
}
