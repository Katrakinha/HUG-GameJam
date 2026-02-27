using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    public float maxSpeed = 6f;
    public float acceleration = 40f;
    public float deceleration = 40f;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    [Header("Visão e Mira")]
    public float viewRadius = 5f;
    [UnityEngine.Range(0, 360)] public float viewAngle = 90f;
    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    [Header("Status Quantico")]
    public bool isSeeingQuantumObject = false;

    [Header("Suavização da Mira")]
    public float rotationSpeed = 15f;    // O quão rápido ele vira o corpo
    public float aimDeadzone = 0.1f;     // Distância mínima do mouse para ele tentar virar

    private Camera mainCam;

    private Vector2 mouseWorldPosition;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        movementInput.Normalize();

        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.z = Mathf.Abs(mainCam.transform.position.z); 
        mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);

        
        FindVisibleTargets();
    }

    void FixedUpdate()
    {
        MovePlayer();
        AimMouse();
    }

    void MovePlayer()
    {
        Vector2 targetVelocity = movementInput * maxSpeed;

        float currentAccelerationRate = (movementInput.magnitude > 0.01f) ? acceleration : deceleration;

        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, currentAccelerationRate * Time.fixedDeltaTime);
    }

    void AimMouse()
    {
        // 1. Descobre a direção e a distância do player para o mouse
        Vector2 lookDirection = mouseWorldPosition - (Vector2)transform.position;
        
        // 2. A ZONA MORTA: O "sqrMagnitude" mede a distância (de forma mais leve pro PC).
        // Se o mouse estiver muito perto do centro do player (menor que a deadzone), ele simplesmente ignora e não gira.
        if (lookDirection.sqrMagnitude > aimDeadzone)
        {
            // Calcula o ângulo alvo que queremos chegar
            float targetAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f; 

            // 3. A SUAVIZAÇÃO (LerpAngle): 
            // Em vez de pular direto para o ângulo, nós arrastamos o ângulo atual do corpo até o ângulo alvo aos poucos.
            // O LerpAngle é especial porque ele sabe que depois de 360 graus volta para o 0, evitando que o boneco dê giros ao contrário do nada.
            float smoothedAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);

            // Gira o corpo para esse novo ângulo suavizado
            rb.MoveRotation(smoothedAngle);
        }
    }

    void FindVisibleTargets()
    {
        isSeeingQuantumObject = false; 

        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetLayer);

        foreach (Collider2D target in targetsInViewRadius)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleLayer))
                {
                    isSeeingQuantumObject = true;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius); 

        Vector3 rightLimit = Quaternion.Euler(0, 0, -viewAngle / 2) * transform.up;
        Vector3 leftLimit = Quaternion.Euler(0, 0, viewAngle / 2) * transform.up;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + rightLimit * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + leftLimit * viewRadius);
    }
}
