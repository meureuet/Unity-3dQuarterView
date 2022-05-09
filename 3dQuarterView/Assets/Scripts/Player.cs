using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] weapons;
    public bool[] hasWeapons;

    public GameObject[] grenades;
    public int hasGrenades;

    public int ammo;
    public int coin;
    public int heart;

    public int maxAmmo;
    public int maxCoin;
    public int maxHeart;
    public int maxHasGrenades;

    public float speed;
    private float hAxis;
    private float vAxis;

    private bool walkKeyDown;
    private bool jumpKeyDown;
    private bool interactionKeyDown;

    bool swapKeyDown1;
    bool swapKeyDown2;
    bool swapKeyDown3;

    private bool isJump;
    private bool isDodge;
    private bool isSwap;

    private Vector3 moveVector;
    private Vector3 dodgeVector;

    private Rigidbody rigid;
    private Animator animator;

    GameObject nearObject;
    GameObject equipWeapon;

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
        Interaction();
        Swap();
    }

    private void GetInput()
    {
        // Ű �Է�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkKeyDown = Input.GetButton("Walk");
        jumpKeyDown = Input.GetButtonDown("Jump");
        interactionKeyDown = Input.GetButtonDown("Interaction");
        swapKeyDown1 = Input.GetButtonDown("Swap1");
        swapKeyDown2 = Input.GetButtonDown("Swap2");
        swapKeyDown3 = Input.GetButtonDown("Swap3");
    }

    private void Move()
    {
        moveVector = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            moveVector = dodgeVector;
        }

        // ���� ���� �� �̵� �Ұ�
        if (isSwap)
        {
            moveVector = Vector3.zero;
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
        if (jumpKeyDown && !isJump && moveVector == Vector3.zero && !isDodge && !isSwap)
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
        if (jumpKeyDown && !isDodge && moveVector != Vector3.zero && !isJump && !isSwap)
        {
            dodgeVector = moveVector;
            speed += 2;
            animator.SetTrigger("doDodge");
            isDodge = true;

            // 0.4�� �ڿ� ����
            Invoke("DodgeOut", 0.4f);
        }
    }

    private void DodgeOut()
    {
        isDodge = false;
        speed -= 2;
    }

    private void Swap()
    {
        int weaponIndex = -1;

        if (swapKeyDown1) weaponIndex = 0;
        else if (swapKeyDown2) weaponIndex = 1;
        else if (swapKeyDown3) weaponIndex = 2;

        if ((swapKeyDown1 || swapKeyDown2 || swapKeyDown3) && hasWeapons[weaponIndex] == true)
        {
            if(equipWeapon != null)
            {
                equipWeapon.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            animator.SetTrigger("doSwap");

            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    private void SwapOut()
    {
        isSwap = false;
    }

    private void Interaction()
    {
        if(interactionKeyDown && nearObject != null && !isDodge && !isJump)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                // �ʵ� ������ ����
                Destroy(nearObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    if(ammo < maxAmmo)
                    {
                        ammo += item.value;
                    }
                    break;
                case Item.Type.Coin:
                    if (coin < maxCoin)
                    {
                        coin += item.value;
                    }
                    break;
                case Item.Type.Heart:
                    if (heart < maxHeart)
                    {
                        heart += item.value;
                    }
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    if (hasGrenades < maxHasGrenades)
                    {
                        hasGrenades += item.value;
                    }
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
}
