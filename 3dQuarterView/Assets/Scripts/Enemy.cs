using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    Rigidbody rb;
    BoxCollider boxCollider;
    Material material;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        // �ٷ� ��������
        material = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            
            // ���� ��ġ - ���� ���� = ���Ͱ� ����
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVector, false));
        }

        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;

            Vector3 reactVector = transform.position - other.transform.position;
            // ���� �Ѿ� ����
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

            if (isGrenade)
            {
                reactVector = reactVector.normalized;
                reactVector += Vector3.up*3;

                rb.freezeRotation = false;
                // ���� �̿��� �˹�
                rb.AddForce(reactVector * 5, ForceMode.Impulse);
                rb.AddTorque(reactVector * 15, ForceMode.Impulse);
            }
            else
            {
                reactVector = reactVector.normalized;
                reactVector += Vector3.up;

                // ���� �̿��� �˹�
                rb.AddForce(reactVector * 5, ForceMode.Impulse);
            }

            
            // 4�� �ڿ� �����
            Destroy(gameObject, 4);
        }
    }
}
