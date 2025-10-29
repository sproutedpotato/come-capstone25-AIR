using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Wyvern : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed, attackDistance;
    //[SerializeField] private float hp, damage;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    public float hp { get; private set; }
    private float damage;

    [SerializeField] private int[] skillDamage; //To be modified by applying directly to each animation.
    [SerializeField] private VisualEffect effect;
    private string status;
    private GameObject playerObject;
    private Player player;
    private float distance, playerDamage, maxHp, rotationSpeed, flyHeight, upSpeed, glideSpeed;
    private Vector3 direction;
    private bool canAttack, isDead, canWalk, isFlying, isGlideMode, setRand;
    private int rand;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();

        hp = 100;
        maxHp = hp;
        damage = 5;
        flyHeight = 5f;
        upSpeed = 3f;
        glideSpeed = 7f;

        status = "Normal";

        canAttack = false;
        SetCanAttackInStart();
        isDead = false;
        canWalk = true;
        isFlying = false;
        isGlideMode = false;
        setRand = false;

        playerDamage = player.DAMAGE;
        rotationSpeed = 10f;
        //effect.Stop();
        audioSource.PlayOneShot(clips[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (isGlideMode)
        {
            return;
        }

        distance = GetDistance(playerObject);

        if (canAttack && !setRand)
        {
            rand = Random.Range(0, 2);
            setRand = true;
            Debug.Log("rand is " + rand);
        }

        direction = GetDirection();
        Rotate(direction);
        if (canAttack)
        {
            canAttack = false;
            DefineBehavior(rand);
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
            animator.SetTrigger("SpitFireBall");
        }
        else if (num == 1)
        {
            animator.SetBool("isGliding", true);
            Glide();
        }
        setRand = false;
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("isWalking", false);
        audioSource.PlayOneShot(clips[1]);

        this.GetComponent<BoxCollider>().enabled = false;

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
            Falling();
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

    private IEnumerator GlideCoroutine()
    {
        isGlideMode = true;
        canAttack = false;

        direction = GetDirection();
        Rotate(direction);
        direction.Normalize();

        float flyDistance = Vector3.Distance(transform.position, player.transform.position) * 2f;
        if(flyDistance < 10f)
        {
            flyDistance = 10f;
        }

        Vector3 startPos = this.transform.position;
        Vector3 targetPos = startPos + direction * flyDistance;

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            if (isDead)
            {
                break;
            }
            transform.position += direction * glideSpeed * Time.deltaTime;
            yield return null;
        }

        isGlideMode = false;
        animator.SetBool("isGliding", false);

        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }

    private void Glide()
    {
        StartCoroutine(GlideCoroutine());
    }

    private IEnumerator FallingCoroutine()
    {
        isDead = true;
        animator.SetTrigger("Falling");
        canAttack = false;
        while (transform.position.y > -1f)
        {
            transform.Translate(Vector3.down * upSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetTrigger("deathTrigger");
        Die();
    }

    private void Falling()
    {
        StartCoroutine(FallingCoroutine());
    }

    public void PlayNotFlyBreathParticle()
    {
        //particleSystems[0].Play();
        effect.SendEvent("OnPlay");
    }

    public void SetCanAttackInStart()
    {
        StartCoroutine(SetCanAttackRoutine());
    }

    private IEnumerator SetCanAttackRoutine()
    {
        yield return new WaitForSeconds(2f);

        canAttack = true;
    }
}