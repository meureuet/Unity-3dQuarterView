using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Transform target;

    public bool isChase;

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
        if (isChase)
        {
            navMeshAgent.SetDestination(target.position);
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
    private void FixedUpdate()
    {
        FreezeVelocity();
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
