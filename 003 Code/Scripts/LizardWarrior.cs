using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using static UnityEngine.GraphicsBuffer;

public class LizardWarrior : MonoBehaviour, IDamageable
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
    private float distance, playerDamage, maxHp, rotationSpeed, prevDis;
    private Vector3 direction;
    private bool canAttack, isProtected, isDead, canWalk, canRoar, canThrow;
    private float rotationThreshold = 0f;

    private float throwCooldown = 1f;
    private float lastThrowTime = -Mathf.Infinity;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();

        hp = 50;
        maxHp = hp;
        damage = 5;

        status = "Normal";

        canAttack = true;
        isDead = false;
        canWalk = true;
        canRoar = true;
        canThrow = false;

        playerDamage = player.DAMAGE;
        rotationSpeed = 10f;
        animator.SetBool("isDead", false);
        prevDis = -1f;

        WaitForThrow();
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

            int num = Random.Range(0, 100);
            if (num > 94 && canThrow)
            {
                Throw();
            }
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
                int rand = Random.Range(0, 5);
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
        if (num == 4 && !canRoar)
        {
            num = Random.Range(0, 4);
        }

        if (num == 0)
        {
            animator.SetTrigger("Attack1");
        }
        else if (num == 1)
        {
            animator.SetTrigger("Attack2");
        }
        else if (num == 2)
        {
            animator.SetTrigger("Attack3");
        }
        else if (num == 3)
        {
            animator.SetTrigger("ComboAttack");
        }
        else if (num == 4)
        {
            animator.SetTrigger("Roar");
        }
    }

    private void Die()
    {
        isDead = true;

        animator.SetBool("isWalking", false);
        animator.SetTrigger("deathTrigger");
        audioSource.PlayOneShot(clips[1]);

        this.GetComponent<CapsuleCollider>().enabled = false;

        Destroy(gameObject, 3f);
    }

    public void DealDamage()
    {
        if (playerObject == null)
        {
            return;
        }
        float distance = GetDistance(playerObject);
        if (distance <= attackDistance || !canThrow)
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
        hp -= 10;

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

    public void Roar()
    {
        StartCoroutine(RoarRoutine());
    }

    private void Throw()
    {
        StartCoroutine(ThrowRoutine());
    }

    private void WaitForThrow()
    {
        StartCoroutine(WaitForThrowRoutine());
    }

    private IEnumerator RoarRoutine()
    {
        canRoar = false;
        Debug.Log("canRoar is false");
        this.damage *= 1.5f;
        yield return new WaitForSeconds(5);
        this.damage /= 1.5f;
        Debug.Log("canRoar is true");
        canRoar = true;
    }

    private IEnumerator ThrowRoutine()
    {
        canThrow = false;
        canWalk = false;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("ThrowAttack");
        yield return new WaitForSeconds(5f);
        canThrow = true;
    }

    private IEnumerator WaitForThrowRoutine()
    {
        yield return new WaitForSeconds(2f);
        canThrow = true;
    }
}