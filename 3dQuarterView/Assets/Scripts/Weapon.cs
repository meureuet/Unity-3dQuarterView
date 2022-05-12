using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{   
    public enum WeaponType
    {
        Melee,
        Range
    };
    public WeaponType weaponType;
    public int damage;
    public float attackRate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPosition;
    public GameObject bullet;
    public Transform bulletCasePosition;
    public GameObject bulletCase;

    public int maxAmmo;
    public int currentAmmo;


    public void Use()
    {
        if(weaponType == WeaponType.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (weaponType == WeaponType.Range && currentAmmo > 0)
        {
            currentAmmo--;
            StopCoroutine("Fire");
            StartCoroutine("Fire");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Fire()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPosition.forward * 50;

        yield return null;
        GameObject instantBulletCase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
        Rigidbody bulletCaseRigid = instantBulletCase.GetComponent<Rigidbody>();
        Vector3 caseVector = bulletCasePosition.forward * Random.Range(-3, -1) + Vector3.up * Random.Range(2, 3);
        bulletCaseRigid.AddForce(caseVector , ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
