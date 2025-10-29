using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Dragon : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed, attackDistance;
    //[SerializeField] private float hp, damage;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private GameObject wyverns;

    public float hp { get; private set; }
    private float damage;

    [SerializeField] private int[] skillDamage; //To be modified by applying directly to each animation.
    [SerializeField] private VisualEffect effect;
    private string status;
    private GameObject playerObject;
    private Player player;
    private float distance, playerDamage, maxHp, rotationSpeed, flyHeight, upSpeed, glideSpeed;
    private Vector3 direction;
    private bool canAttack, isDead, canWalk, isFlying, hasTakenOff, isStartingFly, isGlideMode, setRand;
    private int rand;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();

        hp = 300;
        maxHp = hp;
        damage = 5;
        flyHeight = 5f;
        upSpeed = 3f;
        glideSpeed = 7f;

        status = "Normal";

        canAttack = true;
        isDead = false;
        canWalk = true;
        isFlying = false;
        hasTakenOff = false;
        isStartingFly = false;
        isGlideMode = false;
        setRand = false;

        playerDamage = player.DAMAGE;
        rotationSpeed = 10f;
        effect.Stop();
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

        if (isFlying)
        {
            direction = GetDirection();
            Rotate(direction);
            direction.Normalize();

            if (hasTakenOff && canAttack)
            {
                int num = Random.Range(0, 3);
                DefineFlyBehavior(num);
                canAttack = false;
            }
            else if(!isStartingFly && !hasTakenOff)
            {
                Fly();
            }

            return;
        }

        distance = GetDistance(playerObject);

        if (canAttack && !setRand)
        {
            rand = Random.Range(0, 4);
            setRand = true;
            Debug.Log("rand is " + rand);
        }

        direction = GetDirection();
        Rotate(direction);
        direction.Normalize();

        if (distance > attackDistance && canWalk && rand < 3)
        {
            animator.SetBool("isWalking", true);

            direction = GetDirection();
            Rotate(direction);
            direction.Normalize();

            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        }
        else if (distance <= attackDistance || rand >= 3)
        {
            direction = GetDirection();
            Rotate(direction);

            animator.SetBool("isWalking", false);
            Debug.Log("canAttack is " + canAttack);
            if (canAttack)
            {
                Debug.Log("DoAttack");
                canAttack = false;
                setRand = false;
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
        if (isFlying)
        {
            return;
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
            animator.SetTrigger("SpreadFire");
        }
        else if (num == 3)
        {
            animator.SetBool("isFly", true);
        }

    }

    private void DefineFlyBehavior(int num)
    {
        if (num == 0)
        {
            animator.SetTrigger("SpreadFireWhileFly");
            Vector3 spawnDirection = playerObject.transform.position - transform.position;
            spawnDirection.y = 0;
            spawnDirection.Normalize();

            float spawnDistance = 15f;
            Vector3 spawnPos = playerObject.transform.position + spawnDirection * spawnDistance;
            spawnPos.y = 0f;

            Instantiate(wyverns, spawnPos, Quaternion.identity);
        }
        else if (num == 1)
        {
            animator.SetBool("Glide", true);
            Glide();
        }
        else if(num == 2)
        {
            animator.SetTrigger("GlideToLanding");
            Landing();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("deathTrigger");
        audioSource.PlayOneShot(clips[1]);

        this.GetComponent<BoxCollider>().enabled = false;

        if (isFlying)
        {
            Landing();
            Destroy(gameObject, 6f);
        }
        else
        {
            Destroy(gameObject, 3f);
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

    public void SetFly()
    {
        isFlying = true;
    }

    private IEnumerator FlyCoroutine()
    {
        isStartingFly = true;
        canAttack = false;
        while(transform.position.y < flyHeight)
        {
            transform.Translate(Vector3.up * upSpeed * Time.deltaTime);
            yield return null;
        }
        hasTakenOff = true;

        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }

    private void Fly()
    {
        StartCoroutine(FlyCoroutine());
    }

    private IEnumerator GlideCoroutine()
    {
        isGlideMode = true;
        canAttack = false;

        direction = GetDirection();
        Rotate(direction);
        direction.Normalize();

        float flyDistance = 15f;

        Vector3 startPos = this.transform.position;
        Vector3 targetPos = startPos + direction * flyDistance;

        while(Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position += direction * glideSpeed * Time.deltaTime;
            yield return null;
        }

        isGlideMode = false;
        animator.SetBool("Glide", false);

        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }

    private void Glide()
    {
        StartCoroutine(GlideCoroutine());
    }

    private IEnumerator LandingCoroutine()
    {
        isFlying = false;
        
        animator.SetBool("isFly", false);
        canAttack = false;
        while (transform.position.y > 0)
        {
            transform.Translate(Vector3.down * upSpeed * Time.deltaTime);
            yield return null;
        }

        hasTakenOff = false;
        
        isStartingFly = false;

        if (isDead)
        {
            animator.SetBool("isFalling", true);
        }

        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }

    private void Landing()
    {
        StartCoroutine(LandingCoroutine());
    }

    public void PlayNotFlyBreathParticle()
    {
        //particleSystems[0].Play();
        effect.SendEvent("OnPlay");
    }

    public void PlayFlyBreathParticle()
    {
        
    }
}