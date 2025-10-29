using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class MiniGun : MonoBehaviour, IGun
{
    private int maxBullet;
    public int MAXBULLET { get { return maxBullet; } set { maxBullet = value; } }

    private int currentBullet;
    public int CURRENTBULLET { get { return currentBullet; } set { currentBullet = value; } }

    private int reloadBullet;
    public int RELOADBULLET { get { return reloadBullet; } set { reloadBullet = value; } }

    private bool canShootAuto;
    public bool CANSHOOTAUTO { get { return canShootAuto; } set { canShootAuto = value; } }

    private bool auto;
    public bool AUTO { get { return auto; } set { auto = value; } }

    private bool single;
    public bool SINGLE { get { return single; } set { single = value; } }

    void Awake(){
        this.maxBullet = 30;
        this.currentBullet = 30;
        this.canShootAuto = true;
        this.auto = false;
        this.single = true;
        this.reloadBullet = 30;
    }

    public string ReturnHowToShot(){
        if(single == true)  return "single";
        else    return "auto";
    }
}
