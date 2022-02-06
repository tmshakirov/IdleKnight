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
    [SerializeField] private CanvasGroup mainCanvas;
    [SerializeField] private CanvasGroup damageCanvas;
    [SerializeField] private CanvasGroup bossCanvas;
    [SerializeField] private bool moving;
    [SerializeField] private int coins;
    [SerializeField] private TMP_Text coinsText, bossCoinsText;
    [SerializeField] private EnemyScript enemy;
    [SerializeField] private GameObject blood;
    [SerializeField] private TMP_Text coinPopup;

    [SerializeField] private float attackTimer;
    private Animator anim;
    private Vector3 mousePos, newPos, oldPos, curPos; 
    private float distance;
    private float startPosX, targetPosX;

    [SerializeField] private Image mileProgress;
    [SerializeField] private TMP_Text milesText;
    [SerializeField] private float miles, maxMiles = 1.0f;
    [SerializeField] private float mileTimer;

    [SerializeField] private bool hit;
    [SerializeField] private Image bossProgress;
    [SerializeField] private float bossAmount = 0.8f;

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
        coinsText.text = coins.ToString();
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
                    if (GameHandler.Instance.gameMode != GameMode.BOSSFIGHT)
                        enemy = null;
                    return null;
                }
            }
        }
        return null;
    }

    public void SetSpeed (float _animSpeed)
    {
        anim.speed = _animSpeed;
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
            mileProgress.DOFillAmount(miles / maxMiles, 0.5f);
            mileTimer = 180;
        }
        if (mainCanvas.alpha >= 1)
            Movement();
        if (GameHandler.Instance.gameMode == GameMode.BOSSFIGHT)
        {
            if (bossAmount > 0)
                bossAmount -= Time.deltaTime * 20;
            if (Input.GetMouseButtonDown (0))
            {
                bossAmount += 10f;
                if (bossAmount >= 100f)
                {
                    hit = true;
                    bossAmount -= 30;
                }
            }
            bossProgress.DOKill();
            bossProgress.DOFillAmount(bossAmount/100, 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Border")
        {
            transform.DOMoveX(0, 0.5f);
            mainCanvas.DOFade(0, 0.5f).OnComplete (() => mainCanvas.gameObject.SetActive (false));
            Camera.main.transform.DOMove(new Vector3(5f, Camera.main.transform.position.y + 1f, -4.25f), 1f);
            Camera.main.transform.DORotate(new Vector3(30, -90, 0), 1f);
        }
        if (other.tag == "BorderStop")
        {
            anim.Rebind();
            anim.Update(0f);
            bossCanvas.gameObject.SetActive(true);
            bossCanvas.DOFade(1, 0.5f);
            bossCoinsText.text = coins.ToString();
            GameHandler.Instance.StartBossfight();
            Camera.main.DOFieldOfView(50, 0.5f);
            var enemies = FindObjectsOfType<EnemyScript>();
            foreach (var e in enemies)
            {
                if (e.GetEnemyType() == EnemyType.BOSS)
                {
                    enemy = e;
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameHandler.Instance.gameMode == GameMode.BOSSFIGHT)
        {
            if (attackTimer <= 0 && hit && enemy != null && enemy.GetHealth() > 0)
            {
                anim.SetBool("Attacking", true);
            }
            else
            {
                attackTimer -= Time.deltaTime * 30;
                anim.Play("Idle");
            }
        }
        else
        {
            if (EnemyInFront() != null)
            {
                if (!anim.GetBool("Hit"))
                {
                    anim.SetBool("Attacking", true);
                    anim.SetBool("Riding", UpgradeHandler.Instance.GetHorseLevel() > 0);
                }
            }
            else
            {
                anim.SetBool("Attacking", false);
            }
            if (GameHandler.Instance.moveSpeed > 0)
            {
                if (UpgradeHandler.Instance.GetHorseLevel() > 0)
                    anim.Play("Riding");
                else
                    anim.Play(GameHandler.Instance.moveSpeed >= 2.5f ? "Run" : "Walk");
            }
            else
            {
                anim.Play("Idle");
            }
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
            if (GameHandler.Instance.gameMode != GameMode.BOSSFIGHT)
                enemy = null;
            else
            {
                hit = false;
                anim.SetBool("Attacking", false);
                if (enemy.GetHealth() <= 0)
                {
                    AddCoins(100);
                    bossCanvas.DOFade(0, 0.5f).SetUpdate(true);
                }
            }
        }
    }

    public void GetDamage (int _level, bool _piercing = false)
    {
        if (_level > UpgradeHandler.Instance.GetLevel() || _piercing)
        {
            if (!anim.GetBool("Hit"))
                anim.SetBool("Hit", true);
            int _amount;
            if (_piercing && _level - UpgradeHandler.Instance.GetLevel() <= 0)
                _amount = 5;
            else
                _amount = (_level - UpgradeHandler.Instance.GetLevel()) * 5;
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

    private void AttackEnd()
    {
        if (GameHandler.Instance.gameMode == GameMode.BOSSFIGHT)
        {
            anim.SetBool("Attacking", false);
            attackTimer = 20;
        }
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

                    if (targetPosX > 1.25f)
                        targetPosX = 1.25f;
                    if (targetPosX < -1.25f)
                        targetPosX = -1.25f;

                    transform.DOMoveX(targetPosX, Mathf.Abs(distance) * 0.25f).OnComplete(() =>
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
        StartCoroutine(ToAdd(_amount));
        UpgradeHandler.Instance.CheckUpgrades();
        var pop = Instantiate(coinPopup, mainCanvas.transform);
        pop.text = "+" + _amount;
        var targetPos = Camera.main.WorldToScreenPoint(transform.position);
        pop.rectTransform.anchoredPosition = new Vector2(targetPos.x - 150, targetPos.y);
        pop.rectTransform.DOAnchorPosY(pop.rectTransform.anchoredPosition.y + 50, 1);
        StartCoroutine(Fading(pop, 0.25f));
    }

    private IEnumerator ToAdd (int _amount)
    {
        if (_amount > 0)
        {
            coins++;
            if (GameHandler.Instance.gameMode == GameMode.BOSSFIGHT)
                bossCoinsText.text = coins.ToString();
            else
                coinsText.text = coins.ToString();
        }
        yield return new WaitForSecondsRealtime(0.02f);
        if (_amount > 0)
            StartCoroutine(ToAdd(_amount-1));
    }

    private IEnumerator Fading (TMP_Text _text, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _text.DOFade(0, 0.75f).OnComplete(() => Destroy(_text.gameObject));
    }
}
