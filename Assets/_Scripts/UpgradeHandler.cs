using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum UpgradeType { COINS, WEAPON, ARMOR }

public class UpgradeHandler : Singleton<UpgradeHandler>
{
    [SerializeField] private int currentAmount;
    [SerializeField] private Transform player;
    [SerializeField] private List<Upgrade> upgrades;
    [SerializeField] private GameObject upgradeEffect;

    private void Start()
    {
        CheckUpgrades();
    }

    public void Upgrade (int _type)
    {
        if (upgrades[_type].PressUpgrade())
        {
            Instantiate(upgradeEffect, player.position, upgradeEffect.transform.rotation).GetComponent<ParticleScript>().SetTarget(player);
            if (upgrades[_type].type != UpgradeType.COINS)
                player.transform.localScale = new Vector3(player.transform.localScale.x + 0.04f, player.transform.localScale.y + 0.04f, player.transform.localScale.z + 0.04f);
        }
    }

    public void CheckUpgrades()
    {
        foreach (var u in upgrades)
        {
            u.CheckAvailability();
        }
    }

    public void SetCoinPrice()
    {
        if (currentAmount < (upgrades[0].level+2))
            currentAmount++;
        else
            currentAmount = 1;
    }

    public int GetCoinPrice()
    {
        return currentAmount;
    }

    public int GetLevel ()
    {
        return upgrades[1].level+1 + upgrades[2].level+1;
    }
}
[System.Serializable]
public class Upgrade
{
    [SerializeField] private Button upgradeButton;
    public UpgradeType type;
    public int level = 1;
    public List<GameObject> upgradeObjects;
    public int price;
    public TMP_Text levelText, priceText;

    public void CheckAvailability()
    {
        upgradeButton.interactable = PlayerController.Instance.EnoughCoins(price);
    }

    public bool PressUpgrade()
    {
        if (PlayerController.Instance.EnoughCoins (price))
        {
            upgradeButton.transform.DOScale(1.3f, 0.25f);
            upgradeButton.transform.DORotate(new Vector3(0, 0, -20), 0.25f).OnComplete(() =>
            {
                upgradeButton.transform.DOScale(1.2f, 0.15f);
                upgradeButton.transform.DORotate(Vector3.zero, 0.15f);
            });
            PlayerController.Instance.SpendCoins(price);
            level++;
            if (level < upgradeObjects.Count)
            {
                foreach (var u in upgradeObjects)
                    u.SetActive(false);
                upgradeObjects[level].SetActive(true);
            }
            levelText.text = string.Format("Level {0}", level+1);
            if (type == UpgradeType.COINS)
                price = Mathf.FloorToInt(price * 4);
            else
            {
                price = Mathf.FloorToInt(price * 2.8f);
            }
            priceText.text = price.ToString();
            UpgradeHandler.Instance.CheckUpgrades();
            return true;
        }
        return false;
    }
}
