using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum GameMode { DEFAULT, BOSSFIGHT }

public class GameHandler : Singleton<GameHandler>
{
    public GameMode gameMode;
    public float moveSpeed = 1.5f;
    public float animatorSpeed = 1f;
    public GameObject road;
    public Transform spawner;
    public CanvasGroup victoryCanvas, defeatCanvas;

    public void StartBossfight()
    {
        moveSpeed = 0;
        gameMode = GameMode.BOSSFIGHT;
    }

    public void Victory()
    {
        victoryCanvas.gameObject.SetActive(true);
        victoryCanvas.DOFade(1, 0.5f).SetUpdate (true);
    }

    public void Defeat()
    {
        moveSpeed = 0;
        Invoke("Defeat", 1f);
    }

    private void DefeatUI()
    {
        defeatCanvas.gameObject.SetActive(true);
        defeatCanvas.DOFade(1, 0.5f).SetUpdate(true);
    }

    public void Claim()
    {
        Application.LoadLevel(0);
    }
}
