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
        // 자식의 Animator 가져오기
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 키 입력
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");

        moveVector = new Vector3(hAxis, 0, vAxis).normalized;

        // 키 입력 시 이동
        transform.position += moveVector * speed * (walkDown ? 0.5f : 1f) * Time.deltaTime;

        // 키 입력 시 애니메이션
        animator.SetBool("isRun", moveVector != Vector3.zero);
        animator.SetBool("isWalk", walkDown);

        // 키 입력 방향 바라보기
        transform.LookAt(transform.position + moveVector);


    }
}
