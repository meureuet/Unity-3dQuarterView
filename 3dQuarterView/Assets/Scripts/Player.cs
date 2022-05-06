using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private float hAxis;
    private float vAxis;
    private bool walkKeyDown;
    private bool jumpKeyDown;

    private bool isJump;
    private bool isDodge;

    private Vector3 moveVector;
    private Vector3 dodgeVector;

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
        Dodge();
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

        if (isDodge)
        {
            moveVector = dodgeVector;
        }

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
        if (jumpKeyDown && !isJump && moveVector == Vector3.zero && !isDodge)
        {   
            // �������� ��, ��� 
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
        }
    }
    
    private void Dodge()
    {   
        if (jumpKeyDown && !isDodge && moveVector != Vector3.zero && !isJump)
        {
            dodgeVector = moveVector;
            speed += 2;
            animator.SetTrigger("doDodge");
            isDodge = true;

            // 0.4�� �ڿ� ����
            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        isDodge = false;
        speed -= 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }
}
