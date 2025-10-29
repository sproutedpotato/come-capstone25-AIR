using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private int bulletSpeed;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Sprite offButtonImage, onButtonImage;
    [SerializeField] private Button attackButton;
    [SerializeField] private Player player;
    [SerializeField] private IGun[] guns;
    [SerializeField] private ShotButtonController sbController;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Transform firePoint;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip insertAmmoClip;
    [SerializeField] private AudioClip reloadClip;

    private float nextAttackTime = 0f, attackCoolDown = 0.1f;
    private string howToShot;
    private bool isShooting;

    private IGun currentGun;

    public static Action onTriggerBulletAmount;

    private bool isReloading;

    void Start() {
        Application.targetFrameRate = 60;
        currentGun = player.CURRENTGUN;

        isShooting = false;
        isReloading = false;
    }

    void Update() {
        if (howToShot == "single" && isShooting) {
            if (currentGun.CURRENTBULLET > 0) {
                Attack(currentGun);
                isShooting = false;
            }
            else {
                if (!isReloading)
                {
                    Reloading();
                }
            }
        }
        else if (howToShot == "auto" && isShooting) {
            if (currentGun.CURRENTBULLET > 0 && Time.time >= nextAttackTime) {
                Attack(currentGun);
                nextAttackTime = Time.time + attackCoolDown;
            }
            else if (currentGun.CURRENTBULLET <= 0) {
                isShooting = false;
                if (!isReloading)
                {
                    Reloading();
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isShooting = true;
        howToShot = currentGun.ReturnHowToShot();
        currentGun = player.CURRENTGUN;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isShooting = false;
    }

    private void DrawLineFromCameraToAim(GameObject aimObject)
    {
        // UI AIM 이미지의 위치를 월드 좌표로 변환
        RectTransform aimRectTransform = aimObject.GetComponent<RectTransform>();
        Vector3 aimWorldPosition = aimRectTransform.position;

        // 카메라 위치를 가져옵니다
        Vector3 cameraPosition = Camera.main.transform.position;

        // 카메라에서 AIM 이미지까지 선을 그립니다.
        Debug.DrawLine(cameraPosition, aimWorldPosition, Color.red, 10f); // 선의 색상은 빨간색으로 설정, 0.5초 동안 표시
    }

    private Vector3 GetAttackDirectionFromUI(GameObject aimObject)
    {
        RectTransform aimRectTransform = aimObject.GetComponent<RectTransform>();
        Vector3 aimWorldPosition = aimRectTransform.position;

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 direction = (aimWorldPosition - cameraPosition).normalized;

        return direction;
    }

    private Vector3 GetAttackDirection(Transform firePoint)
    {
        Vector3 screenPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        Vector3 direction = (ray.GetPoint(100f) - firePoint.position).normalized;

        return direction;
    }

    private void AttackInDirection(Vector3 direction, Vector3 startPos)
    {
        GameObject bulletObject = Instantiate(bulletPrefab, startPos, Quaternion.identity);
        BulletController bullet = bulletObject.GetComponent<BulletController>();
        bullet.Init(direction);
    }

    private void Attack(IGun gun) {
        ///GameObject obg_aim = GameObject.Find("Aim");
        Vector3 attackDirection = GetAttackDirection(firePoint);
        Vector3 cameraPosition = Camera.main.transform.position;
        //DrawLineFromCameraToAim(obg_aim);

        AttackInDirection(attackDirection, cameraPosition);
        gun.CURRENTBULLET -= 1;
        audioSource.PlayOneShot(shootClip);
        onTriggerBulletAmount?.Invoke();
    }

    private IEnumerator Reloading(IGun gun) {
        isReloading = true;
        buttonImage.sprite = offButtonImage;
        attackButton.interactable = false;
        PlayReloadSound();
        yield return new WaitForSeconds(2);
        gun.CURRENTBULLET += gun.RELOADBULLET;
        onTriggerBulletAmount?.Invoke();
        buttonImage.sprite = onButtonImage;
        attackButton.interactable = true;
        isReloading = false;
    }

    private void Reloading()
    {
        StartCoroutine(Reloading(currentGun));
    }

    private IEnumerator ReloadSoundRoutine()
    {
        audioSource.PlayOneShot(insertAmmoClip);
        yield return new WaitForSeconds(1.2f);
        audioSource.PlayOneShot(reloadClip);
    }

    private void PlayReloadSound()
    {
        StartCoroutine(ReloadSoundRoutine());
    }
}
