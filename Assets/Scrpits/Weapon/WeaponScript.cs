using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponScript : MonoBehaviour
{
    public Transform cameraPos;
    //Object pool
    [Header("Object Pool")]
    public Transform pool_parent;
    private int currElemId = 0;
    public int maxSize;
    private GameObject[] pool_AR;
    //Характеристика
    [Header("Stats")]
    public int damage;
    public float fireRate, spread, range, reloadTime, shotDelay;
    public int magazineMax, shotsPerClick;
    public bool canHold;
    int magazineCurrent, bulletsShot;

    //Состояния
    [Header("States")]
    bool shooting, readyToShoot, reloading;

    //Чо к чему
    public Camera _cam;
    //public Transform _muzzlePlace;
    public RaycastHit _ray;
    public LayerMask whatIsEnemy, whatIsWhall, whatIsGround;

    //Лейтенант Графоуни
    [Header("Graphics")]
    public Shaker camShake;
    public float shakeMagni, shakeDur;
    public ParticleSystem muzzleShot;
    public GameObject BulletHole;
    public TextMeshProUGUI text;
    public AudioSource _audiosource;
    public AudioClip shotSFX;
    public AudioClip reloadSFX;
    private void Awake()
    {
        pool_AR = new GameObject[maxSize];
        for (int i = 0; i < maxSize; i++)
        {
            pool_AR[i] = Instantiate(BulletHole, transform.position, transform.rotation, pool_parent);
            pool_AR[i].SetActive(false);
        }

        magazineCurrent = magazineMax;
        readyToShoot = true;
    }
    private void Update()
    {
        ShootInput();

        //Интерфейс
        text.SetText(magazineCurrent + "/" + magazineMax);
    }
    private void ShootInput()
    {
        if (canHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && !reloading && magazineCurrent!=magazineMax)
        {
            Reload();
        }
        if (readyToShoot && magazineCurrent != 0 && !reloading && shooting)
        {
            Shoot();
        }
    }

    private void Shoot()
    {

        //Разброс
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        readyToShoot = false;
        
        Vector3 dir = _cam.transform.forward + new Vector3(x, y, 0);
        //Vector3 debugdir = cameraPos.transform.forward + new Vector3(x, y, 0);
        
        //Определяем куда попали рейкастом
        if (Physics.Raycast(_cam.transform.position, dir, out _ray, range, whatIsWhall) || Physics.Raycast(_cam.transform.position, dir, out _ray, range, whatIsGround))
        {
            //Debug.Log(_ray.collider.name);
            // Debug.DrawLine(cameraPos.transform.position, debugdir, Color.green);

            //-----------------------------ObjectPool---------------------------------------------------
            GameObject obj = pool_parent.GetChild(currElemId).gameObject;
            obj.SetActive(true);
            obj.transform.position = _ray.point + _ray.normal * 0.01f;
            obj.transform.rotation = Quaternion.Euler(0, 0, 0);
            obj.transform.rotation = Quaternion.FromToRotation(-obj.transform.forward, _ray.normal);
            currElemId++;
            if (currElemId > pool_parent.childCount - 1) currElemId = 0;
            //-----------------------------ObjectPool---------------------------------------------------

        }
        if(Physics.Raycast(_cam.transform.position, dir, out _ray, range, whatIsEnemy))
        {
            Debug.Log(_ray.collider.name);

            if (_ray.collider.CompareTag("Enemy"))
               _ray.collider.GetComponent<AiEnemy>().TakeDamage(damage);

        }

        //Invoke( "Muzzle"  , fireRate);
        _audiosource.PlayOneShot(shotSFX);
        muzzleShot.Play();
        StartCoroutine(camShake.Shake(shakeDur, shakeMagni));
        magazineCurrent--;
        Invoke("shotReset", fireRate);
       
    }

    private void shotReset()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        _audiosource.PlayOneShot(reloadSFX);
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        magazineCurrent = magazineMax;
        reloading = false;
    }
    private void Muzzle()
    {
        muzzleShot.Play();
    }
}
