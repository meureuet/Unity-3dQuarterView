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
        // 바로 못가져옴
        material = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            
            // 현재 위치 - 닿은 지점 = 벡터값 구함
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVector));
        }

        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;

            Vector3 reactVector = transform.position - other.transform.position;
            // 닿은 총알 삭제
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVector));
        }
    }

    IEnumerator OnDamage(Vector3 reactVector)
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

            reactVector = reactVector.normalized;
            reactVector += Vector3.up;

            // 벡터 이용한 넉백
            rb.AddForce(reactVector * 5, ForceMode.Impulse);
            // 4초 뒤에 사라짐
            Destroy(gameObject, 4);
        }
    }
}
