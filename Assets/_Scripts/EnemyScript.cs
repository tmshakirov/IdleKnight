using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum EnemyState { IDLE, ATTACK, DEATH }

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private EnemyState state;
    [SerializeField] private GameObject blood;
    [SerializeField] private Material deathMat;
    [SerializeField] private SkinnedMeshRenderer MR;
    private Animator anim;
    private Transform player;
    private int level;
    [SerializeField] private float attackDistance = 0.75f;
    [SerializeField] private GameObject levelImage, levelPrefab;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private List<GameObject> weapons, armor;
    [SerializeField] private RectTransform mainCanvas;



    private Vector3 rotationDirection;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Init ()
    {
        level = UpgradeHandler.Instance.GetLevel();
        if (Random.Range(1, 101) <= 30)
        {
            level += Random.Range(-2, 3);
            if (level < 0)
                level = 0;
        }
        transform.localScale += new Vector3(level * 0.05f, level * 0.05f, level * 0.05f);
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

    // Update is called once per frame
    void Update()
    {
        if (state != EnemyState.DEATH)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= 2.5f)
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
                if (Vector3.Distance (transform.position, player.transform.position) <= attackDistance)
                {
                    state = EnemyState.ATTACK;
                }
            break;
            case EnemyState.ATTACK:
                anim.Play("Attack");
            break;
            case EnemyState.DEATH:
                anim.Play("Death");
            break;
        }
    }

    private void Hit()
    {
        player.GetComponent<PlayerController>().GetDamage(level);
    }

    public int GetLevel ()
    {
        return level;
    }

    public bool Dead()
    {
        return state == EnemyState.DEATH;
    }

    public void Death()
    {
        Instantiate(blood, transform.position, blood.transform.rotation);
        levelImage.transform.DOScale(0, 0.15f).OnComplete(() => Destroy(levelImage.gameObject));
        MR.material.DOColor(deathMat.color, 0.15f);
        state = EnemyState.DEATH;
    }

    private void HandleUI()
    {
        float offsetPosY = transform.position.y + 1.5f;
        Vector3 offsetPos = new Vector3(transform.position.x, offsetPosY, transform.position.z);
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas, screenPoint, null, out canvasPos);
        levelImage.SetActive(Vector3.Distance(transform.position, player.transform.position) <= 6f);
        levelImage.transform.localPosition = new Vector2(canvasPos.x, canvasPos.y + 50);
    }
}
