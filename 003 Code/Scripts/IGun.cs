using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGun
{
    public int MAXBULLET { get; set; }
    public int CURRENTBULLET { get; set; }
    public bool CANSHOOTAUTO { get; set; }
    public bool AUTO { get; set; }
    public bool SINGLE { get; set; }
    public int RELOADBULLET { get; set; }
    public string ReturnHowToShot() { return ""; }
}
