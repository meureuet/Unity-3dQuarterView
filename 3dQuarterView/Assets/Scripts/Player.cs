using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int speed;
    private float hAxis;
    private float vAxis;
    private bool walkKeyDown;
    private bool jumpKeyDown;

    private bool isJump;

    private Vector3 moveVector;

    private Rigidbody rigid;
    private Animator animator;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        // �ڽ��� Animator ��������
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    private void GetInput()
    {
        // Ű �Է�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkKeyDown = Input.GetButton("Walk");
        jumpKeyDown = Input.GetButtonDown("Jump");
    }

    private void Move()
    {
        moveVector = new Vector3(hAxis, 0, vAxis).normalized;

        // �̵�
        transform.position += moveVector * speed * (walkKeyDown ? 0.5f : 1f) * Time.deltaTime;

        // Ű �Է� �� �ִϸ��̼�
        animator.SetBool("isRun", moveVector != Vector3.zero);
        animator.SetBool("isWalk", walkKeyDown);
    }

    private void Turn()
    {
        // Ű �Է� ���� �ٶ󺸱�
        transform.LookAt(transform.position + moveVector);
    }

    private void Jump()
    {   
        if (jumpKeyDown && !isJump)
        {   
            // �������� ��, ��� 
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            isJump = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }
}
