using UnityEngine;
using System.Collections;

public class MainMenuEnemyLogic : MonoBehaviour
{
    [SerializeField] private int health = 2;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("References")]
    private Transform player;
    private Animator anim;
    private Camera mainCam;
    private float nextAttackTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        mainCam = Camera.main;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else if (Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        anim.SetBool("isMoving", true);
    }

    void AttackPlayer()
    {
        anim.SetBool("isMoving", false);
        anim.SetTrigger("Attack");

        nextAttackTime = Time.time + attackCooldown;

        StartCoroutine(ShakeCamera(0.15f, 0.2f));
    }

    public void TakeDamage()
    {
        health -= 1;
        if (health <= 0) Destroy(gameObject);
    }

    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = mainCam.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCam.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        mainCam.transform.localPosition = originalPos;
    }
}
