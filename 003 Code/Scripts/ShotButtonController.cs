using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotButtonController : MonoBehaviour
{
    [SerializeField] private Player player;

    private IGun gun;

    public void OnClickChangeButton(){
        gun = player.CURRENTGUN;

        if(gun.CANSHOOTAUTO == true){
            gun.SINGLE = !gun.SINGLE;
            gun.AUTO = !gun.AUTO;
            Debug.Log("SINGLE is "+ gun.SINGLE + " and AUTO is " + gun.AUTO);
        }
    }
}
