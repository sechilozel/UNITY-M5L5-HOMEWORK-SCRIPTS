using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] float movementSpeed = 5f, shiftSpeed = 10f; // yürüme ve koşma hızı
    [SerializeField] float jumpForce = 10f; // zıplama yüksekliği
    [SerializeField] float stamina = 13f;
    [SerializeField] GameObject Pistol, Rifle, MiniGun;
    [SerializeField] Image PistolUI, RifleUI, MiniGunUI, CursorUI;
    [SerializeField] AudioSource playerSounds;
    [SerializeField] AudioClip jumpSFX;
    bool isPistol, isRifle, isMiniGun;
    bool isGrounded = true;

    Rigidbody rb;
    Animator anim;
    Vector3 direction;
    float currentSpeed;
    int health;
    public bool isDead;

    public enum Weapons
    {
        None,
        Pistol,
        Rifle,
        MiniGun
    }
    Weapons weapons = Weapons.None;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        currentSpeed = movementSpeed;

        health = 100;

        if (!photonView.IsMine)
        {
            transform.Find("Main Camera").gameObject.SetActive(false);
            transform.Find("Canvas").gameObject.SetActive(false);
            this.enabled = false; // hareket kodunu kapat
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);
    }


    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        direction = new Vector3(moveHorizontal, 0f, moveVertical);
        direction = transform.TransformDirection(direction);

        if (direction.x != 0 || direction.z !=0)
        {
            // herhangi bir hareketim var
            anim.SetBool("Run", true);
            if (!playerSounds.isPlaying && isGrounded)
            {
                playerSounds.Play();
            }
        }
        else if (direction.x == 0 && direction.z == 0)
        {
            // duruyoruz
            anim.SetBool("Run", false);
            playerSounds.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            isGrounded = false;
            
            playerSounds.Stop();
            AudioSource.PlayClipAtPoint(jumpSFX,transform.position);

            anim.SetBool("Jump", true);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = shiftSpeed;

            if (stamina > 0)
            {
                stamina -= Time.deltaTime;
                currentSpeed = shiftSpeed;
            }
            else
            {
                currentSpeed = movementSpeed;
            }
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            stamina += Time.deltaTime;
            currentSpeed = movementSpeed;
        }

        if (stamina > 13f)
        {
            stamina = 13f;
        }
        else if (stamina < 0)
        {
            stamina = 0;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) && isPistol)
        {
            // ChooseWeapon(Weapons.Pistol);
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Pistol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && isRifle)
        {
            // ChooseWeapon(Weapons.Rifle);
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.Rifle);
        }
        //Minigun ve silahsız durum için kodu buraya yazın
        if(Input.GetKeyDown(KeyCode.Alpha3) && isMiniGun)
        {
            // ChooseWeapon(Weapons.MiniGun);
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.MiniGun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // ChooseWeapon(Weapons.None);
            photonView.RPC("ChooseWeapon", RpcTarget.All, Weapons.None);
        }
    }

    void OnCollisionEnter(Collision other) 
    {
        isGrounded = true;
        anim.SetBool("Jump", false);
    }

    private void OnTriggerEnter(Collider other)
  {
      switch (other.gameObject.tag)
      {
          case "Pistol":
              if (!isPistol)
              {
                  isPistol = true;
                  ChooseWeapon(Weapons.Pistol);
                  PistolUI.color = Color.blue;
              }
              break;
          case "Rifle":
              if (!isRifle)
              {
                  isRifle = true;
                  ChooseWeapon(Weapons.Rifle);
                  RifleUI.color = Color.blue;
              }
              break;
          case "MiniGun":
              if (!isMiniGun)
              {
                  isMiniGun = true;
                  ChooseWeapon(Weapons.MiniGun);
                  MiniGunUI.color = Color.blue;
              }
              break;
          default:
              break;
      }
      Destroy(other.gameObject);
  }  

    [PunRPC]
    void ChooseWeapon(Weapons weapons)
    {
        anim.SetBool("Pistol", weapons == Weapons.Pistol);
        anim.SetBool("Rifle", weapons == Weapons.Rifle);
        anim.SetBool("MiniGun", weapons == Weapons.MiniGun);
        anim.SetBool("NoWeapon", weapons == Weapons.None);
        Pistol.SetActive(weapons == Weapons.Pistol);
        Rifle.SetActive(weapons == Weapons.Rifle);
        MiniGun.SetActive(weapons == Weapons.MiniGun);

        if (weapons != Weapons.None)
        {
            // Elimde herhangi bir silah varsa
            CursorUI.enabled = true;
        }
        else
        {
            CursorUI.enabled = false;
        }
    }

    public void ChangeHealth(int count)
    {
        health -= count; // health = health - count;

        if (health <= 0)
        {
            // ÖLDÜ
            isDead = true;
            anim.SetBool("Die", true);
            ChooseWeapon(Weapons.None);
            this.enabled = false; // hareket kodu kapatılır
        }
    }
}
