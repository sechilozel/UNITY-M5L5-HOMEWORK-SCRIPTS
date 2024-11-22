using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Enemy : MonoBehaviourPunCallbacks
{
    [SerializeField] protected int health, damage;
    [SerializeField] protected float attackDistance, cooldown;

    protected GameObject[] players;

    protected GameObject player;
    protected Animator anim;
    protected Rigidbody rb;

    protected float distance, timer;
    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        player = FindObjectOfType<PlayerController>().gameObject;

        CheckPlayers();
    }

    void FixedUpdate()
    {
        if (!isDead && player != null)
        {
            Move();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float closestDistance = Mathf.Infinity;

        foreach (GameObject closestPlayer in players)
        {
            float checkDistance = Vector3.Distance(closestPlayer.transform.position,
                                                    transform.position);
            
            if (checkDistance < closestDistance)
            {
                if (closestPlayer.GetComponent<PlayerController>().isDead == false)
                {
                    player = closestPlayer;
                    closestDistance = checkDistance;
                }
            }
        }

        if (player != null)
        {
            distance = Vector3.Distance(this.transform.position, player.transform.position);

            if (!isDead)
            {
                Attack();
            }
        }

    }

    public virtual void Move()
    {
        // çocuklarım bunu istedikleri gibi değiştirebilir
    }
    public virtual void Attack()
    {

    }

    [PunRPC]
    public void ChangeHealth(int count)
    {
        health -= count;

        if (health <= 0)
        {
            // enemy öldü
            isDead = true;

            anim.enabled = true;
            anim.SetBool("Die", true);
        }
    }

    public void GetDamage(int count)
    {
        photonView.RPC("ChangeHealth", RpcTarget.All, count);
    }

    void CheckPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Invoke(nameof(CheckPlayers), 3f);
    }
}
