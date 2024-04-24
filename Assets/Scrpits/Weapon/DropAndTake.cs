using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAndTake : MonoBehaviour
{
    public WeaponScript gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;

    public static bool slotFull;

    private void Start()
    {
        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }

    }
    private void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        
        if(!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();
        if (equipped && Input.GetKeyDown(KeyCode.G)) Drop();
    }

    private void PickUp()
    {
        gunScript.enabled = true;
        equipped = true;
        slotFull = true;

        transform.SetParent(gunContainer);
        Vector3 local = new Vector3(13.69402f, -9.778916f, 2.686877f);
        transform.localPosition = local;
        Vector3 Rot = new Vector3(0, 12.111f, 0);
        transform.localRotation = Quaternion.Euler(Rot);
        Vector3 Scale = new Vector3(0.4f, 0.4f, 0.4f);
        transform.localScale = Scale;

        rb.isKinematic = true;
        coll.isTrigger = true;
    }

    private void Drop()
    {
        gunScript.enabled = false;
        equipped = false;
        slotFull = false;

        transform.SetParent(null);
        
        //rb.velocity = player.GetComponent<Rigidbody>().velocity;

        //rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        //rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);
        //float random = Random.Range(-1f, 1f);
        //rb.AddTorque(new Vector3(random, random, random)*10);

        rb.isKinematic = false;
        coll.isTrigger = false;
    }
}
