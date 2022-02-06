using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TapAnimation : MonoBehaviour
{
    [SerializeField] private int animType;
    [SerializeField] private RectTransform rect;
    private Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        if (animType == 0)
            StartCoroutine(Scaling());
        if (animType == 1)
        {
            startPos = rect.anchoredPosition;
            StartCoroutine(Movement());
        }
    }

    private IEnumerator Scaling()
    {
        transform.DOScale(1.1f, 0.5f).OnComplete(() => transform.DOScale(1, 0.5f));
        yield return new WaitForSeconds(1f);
        StartCoroutine(Scaling());
    }

    private IEnumerator Movement()
    {
        rect.DOAnchorPos(startPos + new Vector2(25, -25), 0.2f).OnComplete(() =>
            rect.DOAnchorPos(startPos, 0.2f));
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(Movement());
    }
}
