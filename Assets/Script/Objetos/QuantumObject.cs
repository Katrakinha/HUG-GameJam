using UnityEngine;

public class QuantumObject : MonoBehaviour
{
    [Header("Conexões")]
    public PlayerController player; 
    public PhotoManager photoManager;

    [Header("Configurações de Salto")]
    public Transform[] possibleLocations; 
    
    private bool wasObservedLastFrame; 
    private int currentIndex = -1; 

    void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>(); 
        if (photoManager == null) photoManager = FindFirstObjectByType<PhotoManager>();
    }

    void Update()
    {
        bool isSeenByPlayer = player != null && player.IsSeeing(transform.position);
        bool isSeenByPhoto = photoManager != null && photoManager.IsPhotoSeeing(transform.position);
        
        bool isCurrentlyObserved = isSeenByPlayer || isSeenByPhoto;

        if (wasObservedLastFrame && !isCurrentlyObserved)
        {
            JumpToRandomLocation();
        }

        wasObservedLastFrame = isCurrentlyObserved;
    }

    void JumpToRandomLocation()
    {
        int length = possibleLocations.Length; 
        
        if (length == 0) return;

        if (length == 1)
        {
            currentIndex = 0;
            transform.position = possibleLocations[0].position;
            return;
        }

        int randomIndex = Random.Range(0, length);

        while (randomIndex == currentIndex)
        {
            randomIndex = Random.Range(0, length);
        }

        currentIndex = randomIndex;
        transform.position = possibleLocations[randomIndex].position;
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