using System;
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
    // targetLayer não é mais necessária aqui, mas vou deixar se você usar pra outra coisa
    public LayerMask targetLayer; 
    public LayerMask obstacleLayer;

    // REMOVEMOS A VARIÁVEL GLOBAL "isSeeingQuantumObject" DAQUI!

    [Header("Suavização da Mira")]
    public float rotationSpeed = 15f;    
    public float aimDeadzone = 0.1f;     

    private Camera mainCam;
    private Vector2 mouseWorldPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput.Normalize();

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(mainCam.transform.position.z); 
        mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);
        
        // Removemos o "FindVisibleTargets()" daqui, o player não procura mais sozinho.
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
        Vector2 lookDirection = mouseWorldPosition - (Vector2)transform.position;
        
        if (lookDirection.sqrMagnitude > aimDeadzone)
        {
            float targetAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f; 
            float smoothedAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedAngle);
        }
    }

    // --- A NOVA MÁGICA DE VISÃO REFINADA ---
    // Agora o objeto pergunta diretamente pro player se ele está sendo visto!
    public bool IsSeeing(Vector3 targetPosition)
    {
        Vector3 dirToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 1. Está dentro do raio da lanterna?
        if (distanceToTarget <= viewRadius)
        {
            // 2. Está dentro do ângulo da lanterna?
            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2f)
            {
                // 3. O Raycast garante que não tem parede no caminho!
                if (!Physics2D.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleLayer))
                {
                    return true; // Bingo! O player tá olhando fixo pra essa coordenada!
                }
            }
        }
        return false; // Ninguém tá olhando...
    }
    // ----------------------------------------

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