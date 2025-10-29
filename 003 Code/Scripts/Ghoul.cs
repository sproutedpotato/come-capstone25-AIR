using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Ghoul : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed, attackDistance;
    //[SerializeField] private float hp, damage;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    public float hp { get; private set; }
    private float damage;
    
    [SerializeField] private int[] skillDamage; //To be modified by applying directly to each animation.
    private string status;
    private GameObject playerObject;
    private Player player;
    private float distance, playerDamage, maxHp, rotationSpeed;
    private Vector3 direction;
    private bool canAttack, isProtected, isDead, canWalk, isForwarding;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();

        hp = 30;
        maxHp = hp;
        damage = 5;

        status = "Normal";

        canAttack = true;
        isDead = false;
        canWalk = true;

        playerDamage = player.DAMAGE;
        rotationSpeed = 10f;
        audioSource.PlayOneShot(clips[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        distance = GetDistance(playerObject);

        if (distance > attackDistance && canWalk)
        {
            animator.SetBool("isWalking", true);

            direction = GetDirection();
            Rotate(direction);
            direction.Normalize();

            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        }
        else if (distance <= attackDistance)
        {
            direction = GetDirection();
            Rotate(direction);

            animator.SetBool("isWalking", false);
            if (canAttack)
            {
                canAttack = false;
                int rand = Random.Range(0, 2);
                DefineBehavior(rand);
            }
        }
    }

    #region Move
    private void Rotate(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private Vector3 GetDirection()
    {
        Vector3 dir = playerObject.transform.position - transform.position;
        dir.y = 0f;

        return dir;
    }

    private float GetDistance(GameObject obg1)
    {
        Vector3 obg1_transform = obg1.transform.position;
        Vector3 obg2_transform = this.transform.position;

        obg1_transform.y = 0;
        obg2_transform.y = 0;
        float distance = Vector3.Distance(obg1_transform, obg2_transform);

        return distance;
    }
    #endregion

    private void DefineBehavior(int num)
    {
        if (num == 0)
        {
            animator.SetTrigger("Attack1");
        }
        else if (num == 1)
        {
            animator.SetTrigger("Attack2");
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("deathTrigger");
        audioSource.PlayOneShot(clips[1]);

        Destroy(gameObject, 3f);
    }

    public void DealDamage()
    {
        if (playerObject == null)
        {
            return;
        }
        float distance = GetDistance(playerObject);
        if (distance <= attackDistance)
        {
            IDamageable damageable = playerObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        canWalk = false;
        hp -= damage;

        if (hp <= 0)
        {
            Die();
            return;
        }

        animator.SetTrigger("Hitted");
    }

    public void SetCanAttack()
    {
        Debug.Log("SetCanAttack is Triggered!!");
        canAttack = true;
    }

    public void SetAfterHitted()
    {
        canAttack = true;
        canWalk = true;
    }
}