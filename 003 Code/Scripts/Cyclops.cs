using UnityEngine;

public class Cyclops : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed, attackDistance;
    //[SerializeField] private float hp, damage;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    public float hp { get; private set; }
    private float damage;

    [SerializeField] private int[] skillDamage; //To be modified by applying directly to each animation.
    private GameObject playerObject;
    private Player player;
    private int skillCount;
    private float distance, rotationSpeed;
    private Vector3 direction;
    private bool canAttack, isDead, canWalk, canRoar;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();

        hp = 100;
        damage = 5;
        skillCount = 4;

        canAttack = true;
        isDead = false;
        canWalk = true;
        canRoar = true;

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
            direction = GetDirection();

            animator.SetBool("isWalking", true);

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
                int rand = Random.Range(0, skillCount);
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
        else if (num == 2)
        {
            animator.SetTrigger("2ComboAttack");
        }
        else if (num == 3)
        {
            animator.SetTrigger("Crush");
        }
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

    private void Die()
    {
        isDead = true;

        animator.SetBool("isWalking", false);
        animator.SetTrigger("deathTrigger");
        audioSource.PlayOneShot(clips[1]);

        this.GetComponent<CapsuleCollider>().enabled = false;

        Destroy(gameObject, 4f);
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
        animator.SetTrigger("GetHit");
    }

    public void SetCanAttack()
    {
        canAttack = true;
    }

    public void SetAfterHitted()
    {
        canAttack = true;
        canWalk = true;
    }
}