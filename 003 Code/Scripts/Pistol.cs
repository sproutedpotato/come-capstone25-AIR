using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class Pistol : MonoBehaviour, IGun
{
    private int maxBullet;
    public int MAXBULLET { get { return maxBullet; } set { maxBullet = value; } }

    private int currentBullet;
    public int CURRENTBULLET { get { return currentBullet; } set { currentBullet = value; } }

    private int reloadBullet;
    public int RELOADBULLET { get { return reloadBullet; } set { reloadBullet = value; } }

    private int magazine;
    public int MAGAZINE { get { return magazine; } set { magazine = value; } }

    private bool canShootAuto;
    public bool CANSHOOTAUTO { get { return canShootAuto; } set { canShootAuto = value; } }

    private bool auto;
    public bool AUTO { get { return auto; } set { auto = value; } }

    private bool single;
    public bool SINGLE { get { return single; } set { single = value; } }

    void Awake(){
        this.maxBullet = 24;
        this.currentBullet = 24;
        this.canShootAuto = false;
        this.magazine = -1;
        this.auto = false;
        this.single = true;
        this.reloadBullet = 12;
    }

    public string ReturnHowToShot(){
        return "single";
    }
}
