using UnityEngine;

public class DoorKeySystem : MonoBehaviour
{
    [Header("Visuais da Porta")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteAberta;
    public Sprite spriteFechada;
    
    [Header("Física")]
    [Tooltip("Colisor que bloqueia a passagem do jogador")]
    public BoxCollider2D colisorFisico; 

    private bool aberta = false;

    void Start()
    {
        // Garante que a porta comece no estado certo assim que o jogo rodar
        AtualizarVisualDaPorta(); 
    }

    // --- AS FUNÇÕES QUE O BOTÃO DE PRESSÃO VAI CHAMAR LÁ PELO INSPECTOR ---

    public void AbrirPorta()
    {
        aberta = true;
        AtualizarVisualDaPorta();
    }

    public void FecharPorta()
    {
        aberta = false;
        AtualizarVisualDaPorta();
    }

    // ------------------------------------------------------------------------

    // A função que faz o trabalho sujo de trocar a imagem e desligar a parede
    private void AtualizarVisualDaPorta()
    {
        if (spriteRenderer != null) 
        {
            spriteRenderer.sprite = aberta ? spriteAberta : spriteFechada;
        }
        
        if (colisorFisico != null) 
        {
            // Se a porta está aberta (!aberta fica false), desliga o colisor
            colisorFisico.enabled = !aberta; 
        }
    }
}