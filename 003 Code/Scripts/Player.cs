using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public float hp { get; private set; }
    [SerializeField] private float damage;
    [SerializeField] private GameObject player;
    public float HP{
        get { return this.hp; }
        set {
            this.hp = value;
        }
    }
    public float DAMAGE{
        get { return this.damage; }
        set { this.damage = value; }
    }

    private int index;
    public int INDEX { get { return this.index; } set { this.index = value; } }
    private string status;
    public string STATUS{
        get { return this.status; }
        set { this.status = value; }
    }
    private IGun currentGun;
    public IGun CURRENTGUN{
        get { return currentGun; }
        private set { this.currentGun = value; }
    }
    //private string[] status = {"Normal", "Invincibility"};

    private void Awake()
    {
        currentGun = player.GetComponent<MiniGun>();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        hp = 50;
        damage = 25;
        status = "Normal";
    }

    // Update is called once per frame
    void Update()
    {
        if(status != "Normal"){
            Debug.Log(status);
        }

        if (hp <= 0 && status != "Death")
        {
            status = "Death";
            Debug.Log("player is die...");
        }
    }

    public void OnClickShieldButton(){
        StartCoroutine(WaitForSeconds(3));
    }

    private IEnumerator WaitForSeconds(float time_Second){
        status = "Invincibility";

        yield return new WaitForSeconds(time_Second);

        status = "Normal";
        Debug.Log(status);
    }

    public void TakeDamage(float damage)
    {
        if (!status.Equals("Invincibility") && !status.Equals("Death"))
        {
            this.hp -= damage;
            Debug.Log(hp);
        }
        
        if(hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        this.status = "Death";
    }
}
