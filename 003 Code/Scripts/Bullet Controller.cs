using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEditor;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;

    private Camera mainCamera;
    private Vector3 direction;
    private bool isInitialized = false, hasHit = false;
    private float damage;

    void Start(){
        mainCamera = Camera.main;
        damage = 10f;
    }
    void Update(){
        if (!isInitialized) return;

        transform.position += direction * bulletSpeed * Time.deltaTime;
        if(Vector3.Distance(mainCamera.transform.position, transform.position) > 100f){
            Destroy(gameObject);
        }
    }
    public void Init(Vector3 dir)
    {
        direction = dir.normalized;
        isInitialized = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        hasHit = true;

        Hitbox hitbox = other.GetComponentInParent<Hitbox>();
        if (hitbox != null)
        {
            hitbox.ApplyDamage(damage);
            Destroy(gameObject);
            return;
        }
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
