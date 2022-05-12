using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera cam;

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

    private float attackDelay;

    private bool walkKeyDown;
    private bool jumpKeyDown;
    private bool interactionKeyDown;
    private bool attackKeyDown;
    private bool reloadKeyDown;

    bool swapKeyDown1;
    bool swapKeyDown2;
    bool swapKeyDown3;

    private bool isJump;
    private bool isDodge;
    private bool isSwap;
    private bool isAttackReady;
    private bool isReload;
    private bool isBorder;

    private Vector3 moveVector;
    private Vector3 dodgeVector;

    private Rigidbody rigid;
    private Animator animator;

    GameObject nearObject;
    Weapon equipWeapon;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        // �ڽ��� Animator ��������
        animator = GetComponentInChildren<Animator>();
        isAttackReady = true;
    }

    // Update is called once per frame
    private void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Reload();
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
        attackKeyDown = Input.GetButton("Fire1");
        reloadKeyDown = Input.GetButtonDown("Reload");
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

        // ���� ����, ����, ���� �� �̵� �Ұ�
        if (isSwap || !isAttackReady || isReload)
        {   
            // ȸ���� ����
            moveVector = Vector3.zero;
        }

        // �����ڸ� �ƴ� �� �̵� ����
        if (!isBorder)
        {
            // �̵�
            transform.position += moveVector * speed * (walkKeyDown ? 0.5f : 1f) * Time.deltaTime;
        }
        

        // Ű �Է� �� �ִϸ��̼�
        animator.SetBool("isRun", moveVector != Vector3.zero);
        animator.SetBool("isWalk", walkKeyDown);
    }

    private void Turn()
    {
        // Ű �Է� ���� �ٶ󺸱�
        transform.LookAt(transform.position + moveVector);

        // ���콺 Ŭ�� ���� �ٶ󺸱�
        if(attackKeyDown)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 100))
            {
                Vector3 nextVector = raycastHit.point - transform.position;
                // ������ ���� �����ϰ�
                nextVector.y = 0f;
                transform.LookAt(transform.position + nextVector);
            }
        }
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
    
    private void Attack()
    {
        if(equipWeapon == null)
        {
            return;
        }

        attackDelay += Time.deltaTime;
        isAttackReady = equipWeapon.attackRate < attackDelay;

        if(attackKeyDown && isAttackReady && !isDodge && !isSwap && !isReload)
        {
            equipWeapon.Use();
            if(equipWeapon.weaponType == Weapon.WeaponType.Melee)
            {
                animator.SetTrigger("doSwing");
            }
            else if(equipWeapon.weaponType == Weapon.WeaponType.Range)
            {
                animator.SetTrigger("doShot");
            }
            
            attackDelay = 0; 
        }
    }

    private void Reload()
    {
        if(equipWeapon == null)
        {
            return;
        }

        if (equipWeapon.weaponType == Weapon.WeaponType.Melee)
        {
            return;
        }

        if (ammo == 0)
        {
            return;
        }

        if(reloadKeyDown && !isJump && !isDodge && !isSwap && isAttackReady)
        {
            animator.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2.7f);
        }
    }

    private void ReloadOut()
    {
        // �ִ� ���� �Ұ����ϸ�
        if (ammo < equipWeapon.maxAmmo)
        {
            equipWeapon.currentAmmo = ammo;  
        }
        // �ִ� ���� �����ϸ�
        else
        {
            equipWeapon.currentAmmo = equipWeapon.maxAmmo;
        }
        ammo -= equipWeapon.currentAmmo;
        isReload = false;
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
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

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

    private void FreezeRotation()
    {
        // ȸ���ӵ� 0���� ����
        rigid.angularVelocity = Vector3.zero;
    }

    private void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
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
