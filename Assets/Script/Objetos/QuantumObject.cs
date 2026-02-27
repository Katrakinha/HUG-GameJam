using UnityEngine;

public class QuantumObject : MonoBehaviour
{
    [Header("Conexão")]
    public PlayerController player; // Referência ao nosso super script do player
    public PhotoManager photoManager;

    [Header("Configurações de Salto")]
    public Transform[] possibleLocations; // Lista de lugares para onde ele pode ir
    
    // Essa é a memória de curto prazo do objeto. 
    // Ele precisa lembrar se estava sendo olhado no milissegundo anterior.
    private bool wasObservedLastFrame = false; 

    void Start()
    {
        // Truque de Game Jam: Se você esquecer de arrastar o Player lá no Inspector, 
        // o código procura e acha ele automaticamente na cena pra você não ter erros!
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>(); 
            if (photoManager == null) photoManager = FindFirstObjectByType<PhotoManager>();
        }
    }

    void Update()
    {
        // 1. O Player está olhando?
        bool isSeenByPlayer = player.isSeeingQuantumObject;
        
        // 2. Tem alguma FOTO olhando?
        bool isSeenByPhoto = photoManager.IsPhotoSeeing(transform.position);

        // O objeto está sendo observado se QUALQUER UM DOS DOIS for verdadeiro
        bool isCurrentlyObserved = isSeenByPlayer || isSeenByPhoto;

        if (wasObservedLastFrame == true && isCurrentlyObserved == false)
        {
            JumpToRandomLocation();
        }

        wasObservedLastFrame = isCurrentlyObserved;
    }

    void JumpToRandomLocation()
    {
        // Prevenção de erro: Se você não cadastrou nenhum ponto na Unity, ele não faz nada
        if (possibleLocations.Length == 0)
        {
            Debug.LogWarning("Faltou colocar os pontos de spawn no Objeto Quântico!");
            return;
        }

        // Sorteia um número de zero até a quantidade de pontos que você criou
        int randomIndex = Random.Range(0, possibleLocations.Length);

        // Teleporta o objeto para a posição do ponto sorteado
        transform.position = possibleLocations[randomIndex].position;
        
        Debug.Log("BAM! O Objeto Quântico pulou para o ponto " + randomIndex);
    }

    private void OnDrawGizmos()
    {
        // Se a lista não existir ou estiver vazia, a gente para por aqui para não dar erro
        if (possibleLocations == null || possibleLocations.Length == 0) return;

        // Escolhemos uma cor "quântica" bem chamativa para os pontos
        Gizmos.color = Color.cyan; 

        // Vamos olhar para CADA ponto dentro da nossa lista...
        foreach (Transform ponto in possibleLocations)
        {
            // O "if" garante que a gente não tente desenhar um ponto que foi deletado sem querer
            if (ponto != null) 
            {
                // Desenha uma bolinha de tamanho 0.5 na posição do ponto
                Gizmos.DrawWireSphere(ponto.position, 0.5f); 
                
                // BÔNUS VISUAL: Desenha uma linha ligando o Objeto Quântico até o ponto de spawn!
                // Isso ajuda muito a ver o "alcance" dos teletransportes
                Gizmos.DrawLine(transform.position, ponto.position); 
            }
        }
    }
}