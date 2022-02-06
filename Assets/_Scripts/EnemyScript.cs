using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum EnemyType { SWORDSMAN, RUNNER, BOWMAN, BOSS }
public enum EnemyState { IDLE, ATTACK, DEATH }

public class EnemyScript : MonoBehaviour
{
    [SerializeField] protected EnemyType type;
    [SerializeField] protected EnemyState state;
    [SerializeField] protected GameObject cross;
    [SerializeField] protected GameObject blood, splash, headExplosion;
    protected int deathType;
    [SerializeField] protected Material deathMat;
    [SerializeField] protected SkinnedMeshRenderer MR;
    protected Animator anim;
    protected Transform player;
    protected int level;
    [SerializeField] protected float attentionDistance = 2.5f;
    [SerializeField] protected float attackDistance = 0.75f;
    [SerializeField] protected GameObject levelImage, levelPrefab;
    [SerializeField] protected TMP_Text levelText;
    [SerializeField] protected List<GameObject> weapons, armor;
    [SerializeField] protected RectTransform mainCanvas;
    protected Vector3 rotationDirection;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual EnemyType GetEnemyType()
    {
        return type;
    }

    public virtual int GetHealth ()
    {
        return 1;
    }

    public void Init ()
    {
        if (type == EnemyType.RUNNER)
        {
            level = Random.Range(0, UpgradeHandler.Instance.GetLevel());
        }
        else
        {
            level = UpgradeHandler.Instance.GetLevel();
            if (Random.Range(1, 101) <= 30)
            {
                if (Random.Range(1, 101) <= 40)
                {
                    level += Random.Range(1, 3);
                }
                else
                {
                    level += Random.Range(-2, 3);
                    if (level < 0)
                        level = 0;
                }
            }
        }
        transform.localScale += new Vector3(level * 0.03f, level * 0.03f, level * 0.03f);
        try
        {
            if (weapons.Count > level)
                weapons[level].gameObject.SetActive(true);
            else
                weapons[weapons.Count - 1].gameObject.SetActive(true);
            if (armor.Count > level)
                armor[level].gameObject.SetActive(true);
            else
                armor[armor.Count - 1].gameObject.SetActive(true);
        }
        catch
        {

        }
        mainCanvas = GameObject.Find("PopupCanvas").GetComponent<RectTransform>();
        levelImage = Instantiate(levelPrefab, mainCanvas.transform);
        levelText = levelImage.GetComponentInChildren<TMP_Text>();
        levelText.text = string.Format("Level {0}", (level+1));
    }

    protected virtual void AttackEnd()
    {
        if (GameHandler.Instance.gameMode == GameMode.BOSSFIGHT)
            anim.SetBool("Attacking", false);
    }

    public virtual void AddLevel()
    {
        if (level < UpgradeHandler.Instance.GetLevel())
        {
            level++;
            foreach (var w in weapons)
                w.SetActive(false);
            foreach (var a in armor)
            {
                if (a != null)
                    a.SetActive(false);
            }
            try
            {
                if (weapons.Count > level)
                    weapons[level].gameObject.SetActive(true);
                else
                    weapons[weapons.Count - 1].gameObject.SetActive(true);
                if (armor.Count > level)
                    armor[level].gameObject.SetActive(true);
                else
                    armor[armor.Count - 1].gameObject.SetActive(true);
            }
            catch
            {

            }
            levelText.text = string.Format("Level {0}", level + 1);
            transform.localScale += new Vector3(0.03f, 0.03f, 0.03f);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (state != EnemyState.DEATH)
        {
            cross.SetActive(level > UpgradeHandler.Instance.GetLevel());
            if (Vector3.Distance(transform.position, player.transform.position) <= attentionDistance)
            {
                rotationDirection = player.transform.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotationDirection), 6f * Time.deltaTime);
            }
            HandleUI();
        }
        switch (state)
        {
            case EnemyState.IDLE:
                if (type == EnemyType.RUNNER)
                {
                    if (Vector3.Distance (transform.position, player.transform.position) <= attentionDistance)
                    {
                        anim.Play("Run");
                        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, GameHandler.Instance.moveSpeed * Time.deltaTime);
                        if (player.transform.position.z > transform.position.z)
                        {
                            type = EnemyType.SWORDSMAN;
                        }
                    }
                    else
                    {
                        anim.Play("Idle");
                    }
                }
                else
                {
                    anim.Play("Idle");
                }
                if (Vector3.Distance (transform.position, player.transform.position) <= attackDistance)
                {
                    state = EnemyState.ATTACK;
                }
            break;
            case EnemyState.ATTACK:
                anim.Play("Attack");
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

    public void Destruction()
    {
        Invoke("Destroy", 0.1f);
    }

    private void Destroy()
    {
        Destroy(levelImage);
        Destroy(transform.parent.gameObject);
    }

    protected virtual void Hit()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            player.GetComponent<PlayerController>().GetDamage(level);
        }
        state = EnemyState.IDLE;
    }

    public int GetLevel ()
    {
        return level;
    }

    public bool Dead()
    {
        return state == EnemyState.DEATH;
    }

    public virtual void Death()
    {
        if (UpgradeHandler.Instance.GetWeapon() != WeaponType.BLUNT)
            transform.DOMoveZ(transform.position.z + Random.Range(0.7f, 1.25f), 0.15f);
        else
            Instantiate(headExplosion, transform.position + Vector3.up * 0.5f, headExplosion.transform.rotation);
        //GetComponent<Rigidbody>().AddForce(Vector3.forward * 125);
        Instantiate(blood, transform.position, blood.transform.rotation);
        levelImage.transform.DOScale(0, 0.15f).OnComplete(() => Destroy(levelImage.gameObject));
        MR.material.DOColor(deathMat.color, 0.15f).OnComplete (() =>
        {
            var s = Instantiate(splash, transform.position + new Vector3 (0, 0.1f, 0), Quaternion.identity);
            s.transform.SetParent(transform);
            s.transform.DOScale(0.24f, 0.35f);
        });
        state = EnemyState.DEATH;
        deathType = Random.Range(1, 101);
    }

    protected void HandleUI()
    {
        float offsetPosY = transform.position.y + (1.5f + level * 0.05f);
        Vector3 offsetPos = new Vector3(transform.position.x, offsetPosY, transform.position.z);
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas, screenPoint, null, out canvasPos);
        if (levelImage != null)
        {
            levelImage.SetActive(Vector3.Distance(transform.position, player.transform.position) <= 6f);
            levelImage.transform.localPosition = new Vector2(canvasPos.x, canvasPos.y + 50);
        }
    }
}
