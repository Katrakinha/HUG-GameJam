using UnityEngine;

public class QuantumWall : MonoBehaviour
{
    // Criamos as opções pro Game Designer escolher na Unity
    public enum TipoMuro { Horizontal, Vertical }

    [Header("Conexões")]
    public PlayerController player;
    public PhotoManager photoManager;

    [Header("Configurações do Muro")]
    [Tooltip("Horizontal: Corredores cima/baixo. Vertical: Corredores lados.")]
    public TipoMuro tipoDoMuro = TipoMuro.Horizontal;
    
    [Tooltip("Distância que ele fica das costas (Só precisa de uma agora!)")]
    public float distanciaTraseira = 1.5f; 
    
    public bool playerInZone = false;         

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    
    private bool wasObserved = true; 

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (!playerInZone)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            wasObserved = true; 
            return; 
        }

        bool currentlyObserved = IsBeingObserved();

        if (currentlyObserved)
        {
            wasObserved = true;
        }
        else
        {
            if (wasObserved)
            {
                // Copiamos a posição atual do muro
                Vector3 novaPosicao = transform.position;

                if (tipoDoMuro == TipoMuro.Horizontal)
                {
                    // MURO HORIZONTAL (No trilho do eixo Y)
                    // Se o player está olhando pra cima (Y > 0), as costas são pra baixo (-1). Senão, pra cima (+1).
                    float direcaoCostasY = player.transform.up.y > 0 ? -1f : 1f;
                    
                    // Mantemos o X original do muro para ele não bater nas paredes do corredor!
                    novaPosicao = new Vector3(originalPosition.x, player.transform.position.y + (distanciaTraseira * direcaoCostasY), originalPosition.z);
                }
                else if (tipoDoMuro == TipoMuro.Vertical)
                {
                    // MURO VERTICAL (No trilho do eixo X)
                    // Se o player está olhando pra direita (X > 0), as costas são pra esquerda (-1). Senão, pra direita (+1).
                    float direcaoCostasX = player.transform.up.x > 0 ? -1f : 1f;
                    
                    // Mantemos o Y original do muro!
                    novaPosicao = new Vector3(player.transform.position.x + (distanciaTraseira * direcaoCostasX), originalPosition.y, originalPosition.z);
                }
                
                transform.position = novaPosicao;
                transform.rotation = originalRotation; 
                
                wasObserved = false; 
            }
        }
    }

    bool IsBeingObserved()
    {
        if (photoManager != null && photoManager.IsPhotoSeeing(transform.position))
            return true;

        Vector3 dirToWall = (transform.position - player.transform.position).normalized;
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= player.viewRadius)
        {
            if (Vector3.Angle(player.transform.up, dirToWall) < player.viewAngle / 2f)
            {
                if (!Physics2D.Raycast(player.transform.position, dirToWall, distance, player.obstacleLayer))
                {
                    return true; 
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInZone = false;
    }
}