using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A,B,C};
    public Type enemyType;

    public int maxHealth;
    public int currentHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;

    public bool isChase;
    public bool isAttack;

    Rigidbody rb;
    BoxCollider boxCollider;
    Material material;
    NavMeshAgent navMeshAgent;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        // ¹Ù·Î ¸ø°¡Á®¿È
        material = GetComponentInChildren<MeshRenderer>().material;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    private void ChaseStart()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(target.position);
            navMeshAgent.isStopped = !isChase;
        }
       
    }

    private void FreezeVelocity()
    {
        if (isChase)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch (enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
        }

        RaycastHit[] raycastHits =
            Physics.SphereCastAll(transform.position,
                targetRadius,
                transform.forward,
                targetRange,
                LayerMask.GetMask("Player"));

        if(raycastHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rb.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rb.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);

                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidbodyBullet = instantBullet.GetComponent<Rigidbody>();
                rigidbodyBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }

        

        isChase = true;
        isAttack = false;
        animator.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
        Targeting();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            
            // ÇöÀç À§Ä¡ - ´êÀº ÁöÁ¡ = º¤ÅÍ°ª ±¸ÇÔ
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVector, false));
        }

        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;

            Vector3 reactVector = transform.position - other.transform.position;
            // ´êÀº ÃÑ¾Ë »èÁ¦
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVector, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPosition)
    {
        currentHealth -= 100;
        Vector3 reactVector = transform.position - explosionPosition;
        StartCoroutine(OnDamage(reactVector, true));
    }

    IEnumerator OnDamage(Vector3 reactVector, bool isGrenade)
    {
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(currentHealth > 0)
        {
            material.color = Color.white;
        }
        else
        {
            material.color = Color.gray;
            gameObject.layer = 12;
            isChase = false;
            navMeshAgent.enabled = false;
            animator.SetTrigger("doDie");

            if (isGrenade)
            {
                reactVector = reactVector.normalized;
                reactVector += Vector3.up*3;

                rb.freezeRotation = false;
                // º¤ÅÍ ÀÌ¿ëÇÑ ³Ë¹é
                rb.AddForce(reactVector * 5, ForceMode.Impulse);
                rb.AddTorque(reactVector * 15, ForceMode.Impulse);
            }
            else
            {
                reactVector = reactVector.normalized;
                reactVector += Vector3.up;

                // º¤ÅÍ ÀÌ¿ëÇÑ ³Ë¹é
                rb.AddForce(reactVector * 5, ForceMode.Impulse);
            }

            
            // 4ÃÊ µÚ¿¡ »ç¶óÁü
            Destroy(gameObject, 4);
        }
    }
}
