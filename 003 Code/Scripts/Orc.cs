using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Orc : MonoBehaviour, IDamageable
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
    private bool canAttack, isDead, canWalk, isForwarding;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();

        hp = 70;
        maxHp = hp;
        damage = 5;
        
        status = "Normal";
        animator.SetBool("isHpOverThenHalf", true);

        canAttack = true;
        isDead = false;
        canWalk = true;
        isForwarding = false;

        playerDamage = player.DAMAGE;
        rotationSpeed = 10f;
        audioSource.PlayOneShot(clips[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead){
            return;
        }

        distance = GetDistance(playerObject);

        bool isBackWards = animator.GetBool("isBackwards");

        if (isBackWards && distance < 5f)
        {
            transform.Translate(direction * -1 * 0.5f * Time.deltaTime, Space.World);
            return;
        }

        if (distance > attackDistance && canWalk){

            animator.SetBool("isWalking", true);

            direction = GetDirection();
            Rotate(direction);

            direction.Normalize();

            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
            
        }
        else if(distance <= attackDistance)
        {
            direction = GetDirection();
            Rotate(direction);

            animator.SetBool("isWalking", false);
            if (canAttack)
            {
                canAttack = false;
                int rand = Random.Range(0, 6);
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

    private float GetDistance(GameObject obg1){
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
        if(num == 0)
        {
            animator.SetTrigger("Attack1");
        }
        else if (num == 1)
        {
            animator.SetTrigger("Attack2");
        }
        else if (num == 2)
        {
            animator.SetTrigger("2ComboAttack");
        }
        else if (num == 3)
        {
            animator.SetTrigger("3ComboAttack");
        }
        else if (num == 4)
        {
            animator.SetTrigger("4ComboAttack");
        }
        else if (num == 5)
        {
            animator.SetBool("isBackwards", true);
        }
    }

    private void Die()
    {
        isDead = true;
        animator.ResetTrigger("Hitted");
        animator.SetBool("isWalking", false);
        animator.SetBool("isDead", true);
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
        bool isBackWards = animator.GetBool("isBackwards");
        Debug.Log("in TakeDamage, isBackWards is " + isBackWards);
        

        if (!isBackWards)
        {
            hp -= damage;
        }
        else
        {
            if (animator.GetBool("isBackwards") && !isForwarding)
            {
                animator.SetTrigger("Blocked");
                StartCoroutine(WaitAndForwardAfterBlocked());
            }
        }

        if (hp <= 0)
        {
            Die();
            return;
        }

        if (!isBackWards)
        {
            animator.SetTrigger("Hitted");
        }
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

    public void ForwardToPlayer()
    {
        if (isForwarding) return;

        isForwarding = true;
        Debug.Log("ForwardToPlayer");
        StartCoroutine(ForwardToPlayerCoroutine(player.transform));
    }

    private IEnumerator ForwardToPlayerCoroutine(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        float duration = clips.First(c => c.name == "4HitComboForward_Pre").length;
        float speed = distance / duration;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(target.position.x, startPosition.y, target.position.z);

        transform.LookAt(target);

        while (Vector3.Distance(transform.position, targetPosition) > attackDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += new Vector3(direction.x, 0f, direction.z) * speed * Time.deltaTime;
            yield return null;
        }

        animator.SetBool("isBackwards", false);
        canAttack = true;
        isForwarding = false;
    }

    private IEnumerator WaitAndForwardAfterBlocked()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        float duration = clips.First(c => c.name == "Blocked_Pre").length;

        yield return new WaitForSeconds(duration - 0.25f);

        if (!isForwarding && animator.GetBool("isBackwards"))
        {
            ForwardToPlayer();
        }
    }
}
