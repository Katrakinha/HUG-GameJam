using UnityEngine;

public class QuantumObject : MonoBehaviour
{
    [Header("Conexão")]
    public PlayerController player; 
    public PhotoManager photoManager;

    [Header("Configurações de Salto")]
    public Transform[] possibleLocations; 
    
    private bool wasObservedLastFrame = false; 
    
    // VARIÁVEL NOVA: Guarda a memória de onde ele está agora!
    private int currentIndex = -1; 

    void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>(); 
        }
        if (photoManager == null)
        {
            photoManager = FindFirstObjectByType<PhotoManager>();
        }
    }

    void Update()
    {
        // 1. A NOVA CHECAGEM REFINADA: O Objeto manda as suas próprias coordenadas pro player avaliar!
        bool isSeenByPlayer = player.IsSeeing(transform.position);
        
        // 2. A foto está vendo? (Continua igual)
        bool isSeenByPhoto = photoManager != null && photoManager.IsPhotoSeeing(transform.position);

        bool isCurrentlyObserved = isSeenByPlayer || isSeenByPhoto;

        if (wasObservedLastFrame == true && isCurrentlyObserved == false)
        {
            JumpToRandomLocation();
        }

        wasObservedLastFrame = isCurrentlyObserved;
    }

    void JumpToRandomLocation()
    {
        if (possibleLocations.Length == 0) return;

        // Se só tiver 1 ponto no mapa, não tem o que fazer, ele vai ter que pular pra lá mesmo.
        if (possibleLocations.Length == 1)
        {
            currentIndex = 0;
            transform.position = possibleLocations[0].position;
            return;
        }

        // --- SISTEMA ANTI-REPETIÇÃO ---
        int randomIndex = Random.Range(0, possibleLocations.Length);

        // Enquanto o número sorteado for IGUAL ao lugar que ele já está, ele sorteia de novo!
        while (randomIndex == currentIndex)
        {
            randomIndex = Random.Range(0, possibleLocations.Length);
        }

        // Atualiza a memória para o novo lugar e teleporta
        currentIndex = randomIndex;
        transform.position = possibleLocations[randomIndex].position;
        
        Debug.Log("BAM! O Objeto Quântico pulou para um NOVO ponto: " + randomIndex);
    }

    private void OnDrawGizmos()
    {
        if (possibleLocations == null || possibleLocations.Length == 0) return;

        Gizmos.color = Color.cyan; 

        foreach (Transform ponto in possibleLocations)
        {
            if (ponto != null) 
            {
                Gizmos.DrawWireSphere(ponto.position, 0.5f); 
                Gizmos.DrawLine(transform.position, ponto.position); 
            }
        }
    }
}