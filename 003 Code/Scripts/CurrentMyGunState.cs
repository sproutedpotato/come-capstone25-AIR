using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class CurrentMyGunState : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Player player;

    private IGun gun;
    private string magazine;
    // Start is called before the first frame update
    void Start()
    {
        gun = player.CURRENTGUN;
        magazine = gun.MAXBULLET.ToString();
        text.text = gun.CURRENTBULLET.ToString() + " / " + magazine;
    }

    private void OnEnable(){
        AttackButton.onTriggerBulletAmount += EnumerateCurrentBullet;
    }

    private void OnDisable(){
        AttackButton.onTriggerBulletAmount -= EnumerateCurrentBullet;
    }

    private void EnumerateCurrentBullet(){
        gun = player.CURRENTGUN;
        magazine = gun.MAXBULLET.ToString();
        text.text = gun.CURRENTBULLET.ToString() + " / " + magazine;
    }
}
