using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    public float maxSpeed = 6f;
    public float acceleration = 40f;
    public float deceleration = 40f;

    [Header("Visão e Mira")]
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask obstacleLayer;

    [Header("Suavização da Mira")]
    public float rotationSpeed = 15f;    
    public float aimDeadzone = 0.1f;     

    private Rigidbody2D rb;
    private Vector2 movementInput;
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
    }

    void FixedUpdate()
    {
        MovePlayer();
        AimMouse();
    }

    void MovePlayer()
    {
        Vector2 targetVelocity = movementInput * maxSpeed;
        float currentAccelerationRate = (movementInput.sqrMagnitude > 0.0001f) ? acceleration : deceleration;
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

    public bool IsSeeing(Vector3 targetPosition)
    {
        Vector3 dirToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget <= viewRadius && Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2f)
        {
            if (!Physics2D.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleLayer))
            {
                return true; 
            }
        }
        return false; 
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