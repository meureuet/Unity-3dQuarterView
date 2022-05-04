using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int speed;
    float hAxis;
    float vAxis;
    bool walkDown;

    Vector3 moveVector;

    Animator animator;

    void Awake()
    {
        // �ڽ��� Animator ��������
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ű �Է�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");

        moveVector = new Vector3(hAxis, 0, vAxis).normalized;

        // Ű �Է� �� �̵�
        transform.position += moveVector * speed * (walkDown ? 0.5f : 1f) * Time.deltaTime;

        // Ű �Է� �� �ִϸ��̼�
        animator.SetBool("isRun", moveVector != Vector3.zero);
        animator.SetBool("isWalk", walkDown);

        // Ű �Է� ���� �ٶ󺸱�
        transform.LookAt(transform.position + moveVector);


    }
}
