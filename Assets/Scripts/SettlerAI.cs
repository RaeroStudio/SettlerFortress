using UnityEngine;
using UnityEngine.AI;

public class SettlerAI : MonoBehaviour
{
    [Header("Settings")]
    public float patrolRadius = 10f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float damage = 10f;
    public float health = 100f;

    private NavMeshAgent agent;
    private Transform citadel;
    private Transform target;
    private float lastAttackTime;
    private Vector3 basePosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        citadel = GameObject.FindGameObjectWithTag("Citadel").transform;
        basePosition = citadel.position;
        Patrol();
    }

    void Update()
    {
        if (target == null)
        {
            // Поиск врагов в радиусе обнаружения
            Collider[] enemies = Physics.OverlapSphere(
                transform.position, 
                detectionRange, 
                LayerMask.GetMask("Enemy")
            );

            if (enemies.Length > 0)
            {
                target = GetClosestEnemy(enemies);
                agent.SetDestination(target.position);
            }
            else
            {
                // Патрулирование если нет целей
                if (agent.remainingDistance < 1f)
                {
                    Patrol();
                }
            }
        }
        else
        {
            // Проверка если цель умерла или ушла
            if (!target.gameObject.activeSelf || 
                Vector3.Distance(transform.position, target.position) > detectionRange)
            {
                target = null;
                Patrol();
                return;
            }

            // Движение к цели
            agent.SetDestination(target.position);

            // Атака при приближении
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                if (Time.time - lastAttackTime > attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    Transform GetClosestEnemy(Collider[] enemies)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                closest = enemy.transform;
                minDistance = distance;
            }
        }
        return closest;
    }

    void Patrol()
    {
        Vector3 randomPoint = basePosition + Random.insideUnitSphere * patrolRadius;
        randomPoint.y = basePosition.y; // Сохраняем высоту цитадели
        
        // Ищем ближайшую точку на NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void Attack()
    {
        if (target != null) 
        {
            EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
            if (enemyHealth != null) 
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

      public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Логика смерти поселенца
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация зон в редакторе
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}