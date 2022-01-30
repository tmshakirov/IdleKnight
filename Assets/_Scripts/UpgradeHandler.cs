using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum UpgradeType { COINS, WEAPON, ARMOR }

public class UpgradeHandler : Singleton<UpgradeHandler>
{
    [SerializeField] private Transform player;
    [SerializeField] private List<Upgrade> upgrades;
    [SerializeField] private GameObject upgradeEffect;

    public void Upgrade (int _type)
    {
        if (upgrades[_type].PressUpgrade())
            Instantiate(upgradeEffect, player.position, Quaternion.identity).GetComponent<ParticleScript>().SetTarget (player);
    }

    public int GetLevel ()
    {
        return (upgrades[1].level+1) + (upgrades[2].level+1);
    }
}
[System.Serializable]
public class Upgrade
{
    public UpgradeType type;
    public int level = 1;
    public List<GameObject> upgradeObjects;
    public int price;
    public TMP_Text levelText, priceText;

    public bool PressUpgrade()
    {
        if (PlayerController.Instance.EnoughCoins (price))
        {
            PlayerController.Instance.SpendCoins(price);
            level++;
            if (level < upgradeObjects.Count)
            {
                foreach (var u in upgradeObjects)
                    u.SetActive(false);
                upgradeObjects[level].SetActive(true);
            }
            levelText.text = string.Format("Level {0}", (level+1));
            price = Mathf.FloorToInt(price * 2.8f);
            priceText.text = price.ToString();
            return true;
        }
        return false;
    }
}
