using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private Animator animator;
    private bool _canAttack = true;
    public Dictionary<string, int> collectedItems = new();
    public Dictionary<string, int> defeatedEnemies = new();
    public static event Action OnObjectivesUpdated;

    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EnemyAIController.OnEnemyDefeated += OnEnemyDefeated;
        Collectable.OnItemCollected += OnItemCollected;
    }

    private void OnDisable()
    {
        EnemyAIController.OnEnemyDefeated -= OnEnemyDefeated;
        Collectable.OnItemCollected -= OnItemCollected;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && _canAttack)
        {
            animator.SetTrigger("Attack");
            _canAttack = false;
            Invoke(nameof(EnableAttack), 0.7f);
            DealDamage();
        }
    }

    private void EnableAttack()
    {
        _canAttack = true;
    }

    // Este método puede ser llamado desde un evento de animación en el momento del impacto
    public void DealDamage()
    {
        // Detectar enemigos en rango de ataque
        Collider[] hitEnemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayers
        );
        foreach (Collider enemyCol in hitEnemies)
        {
            // Aplicar daño a cada enemigo alcanzado
            EnemyAIController enemy = enemyCol.GetComponent<EnemyAIController>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
    }

    private void OnEnemyDefeated(string enemyName)
    {
        if (defeatedEnemies.ContainsKey(enemyName))
        {
            defeatedEnemies[enemyName]++;
        }
        else
        {
            defeatedEnemies[enemyName] = 1;
        }
        OnObjectivesUpdated?.Invoke();
    }

    private void OnItemCollected(string itemName, int quantity)
    {
        if (collectedItems.ContainsKey(itemName))
        {
            collectedItems[itemName] += quantity;
        }
        else
        {
            collectedItems[itemName] = quantity;
        }
        OnObjectivesUpdated?.Invoke();
    }

    public int GetItemCount(string itemName)
    {
        return collectedItems.TryGetValue(itemName, out var qty) ? qty : 0;
    }

    public int GetEnemyCount(string enemyName)
    {
        return defeatedEnemies.TryGetValue(enemyName, out var qty) ? qty : 0;
    }
}
