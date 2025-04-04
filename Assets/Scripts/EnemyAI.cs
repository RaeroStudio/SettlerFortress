using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour 
{
    public static Citadel Instance;
    [Header("Настройки")]
    public float detectionRadius = 10f; // Радиус обнаружения поселенцев
    public float attackRadius = 2f;     // Радиус атаки
    public float settlerDamage = 15f;   // Урон по поселенцам
    public float citadelDamage = 30f;   // Урон по цитадели
    public float attackCooldown = 1f;   // Задержка между атаками

    private NavMeshAgent agent;
    private Transform citadel;
    private Transform currentTarget;
    private float lastAttackTime;
    private bool isActive = true;

    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        citadel = Citadel.Instance.transform;
        SetDestinationToCitadel();
    }

    void Update() 
    {
        if (!isActive || !agent.isOnNavMesh) return;

        // Поиск новых целей каждые 0.5 секунды для оптимизации
        if (Time.frameCount % 30 == 0) 
        {
            FindNewTarget();
        }

        if (currentTarget != null) 
        {
            HandleTarget();
        }
        else 
        {
            AttackCitadel();
        }
    }

    void FindNewTarget() 
    {
        // Поиск всех поселенцев в радиусе
        Collider[] settlers = Physics.OverlapSphere(
            transform.position, 
            detectionRadius, 
            LayerMask.GetMask("Settler")
        );

        // Выбор ближайшего поселенца
        Transform closestSettler = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider settler in settlers) 
        {
            float distance = Vector3.Distance(transform.position, settler.transform.position);
            if (distance < minDistance) 
            {
                closestSettler = settler.transform;
                minDistance = distance;
            }
        }

        // Переключение цели при обнаружении
        if (closestSettler != null) 
        {
            currentTarget = closestSettler;
            agent.SetDestination(currentTarget.position);
        }
        else 
        {
            currentTarget = null;
            SetDestinationToCitadel();
        }
    }

    void HandleTarget() 
    {
        // Проверка на валидность цели
        if (!currentTarget.gameObject.activeSelf || 
            Vector3.Distance(transform.position, currentTarget.position) > detectionRadius) 
        {
            currentTarget = null;
            return;
        }

        // Движение к цели
        agent.SetDestination(currentTarget.position);

        // Атака при приближении
        if (Vector3.Distance(transform.position, currentTarget.position) <= attackRadius) 
        {
            if (Time.time - lastAttackTime > attackCooldown) 
            {
                Attack(currentTarget);
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack(Transform target) 
    {
        Debug.Log($"Атакую цель: {target.name}"); // Добавьте это
    
        if (target.CompareTag("Settler")) 
       {
            if (target.TryGetComponent<SettlerAI>(out var settler)) 
               {
                    Debug.Log($"Нанесено урона поселенцу: {settlerDamage}");
                settler.TakeDamage(settlerDamage);
               }
        }
        else if (target.CompareTag("Citadel")) 
        {
            Debug.Log($"Нанесено урона цитадели: {citadelDamage}");
            Citadel.Instance.TakeDamage(citadelDamage);
        }
    }

    void AttackCitadel() 
    {
        if (Vector3.Distance(transform.position, citadel.position) <= attackRadius) 
        {
            if (Time.time - lastAttackTime > attackCooldown) 
            {
                Citadel.Instance.TakeDamage(citadelDamage);
                lastAttackTime = Time.time;
            }
        }
        else 
        {
            SetDestinationToCitadel();
        }
    }

    void SetDestinationToCitadel() 
    {
        agent.SetDestination(citadel.position);
    }

    void OnDestroy() 
    {
        isActive = false;
        GoldManager.Instance?.AddGold(10);
    }

void OnDrawGizmos() 
{
    // Радиус обнаружения (желтый)
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);

    // Радиус атаки (красный)
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRadius);
}
}