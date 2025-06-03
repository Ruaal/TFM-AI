using System;
using System.Collections;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField]
    private EnemyAttributes enemyAttributes;

    public static event Action<string> OnEnemyDefeated;

    public Animator animator;
    private bool isDead = false;
    private float life;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        life = enemyAttributes.health;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;
        life -= damage;
        if (life <= 0)
        {
            OnDeath();
        }
        if (isDead)
            return;
        animator.SetTrigger("Hit");
    }

    private void OnDeath()
    {
        if (isDead)
            return;

        isDead = true;
        animator.SetBool("Death", true);
        OnEnemyDefeated?.Invoke(enemyAttributes.enemyName);
        StartCoroutine(RespawnAfterDelay(2f));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isDead = false;
        life = enemyAttributes.health;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        animator.SetBool("Death", false);
        animator.Rebind();
        animator.Update(0f);
    }
}
