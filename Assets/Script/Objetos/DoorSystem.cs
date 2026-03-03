using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite spriteAberta;
    public Sprite spriteFechada;
    public GameObject iconeE;
    
    [Tooltip("Colisor que bloqueia a passagem do jogador")]
    public BoxCollider2D colisorFisico; 

    private bool perto = false;
    private bool aberta = false;

    void Start()
    {
        if (iconeE != null) iconeE.SetActive(false);
        AtualizarVisualDaPorta(); // Garante que ela comece com o visual e colisão certos
    }

    void Update()
    {
        // O jogador ainda pode abrir/fechar na mão se quiser!
        if (perto && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor(); 
        }
    }

    // --- AS FUNÇÕES PÚBLICAS PARA O BOTÃO USAR ---

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

    // ----------------------------------------------

    // Função interna só para não repetir código
    public void ToggleDoor()
    {
        aberta = !aberta;
        AtualizarVisualDaPorta();
    }

    private void AtualizarVisualDaPorta()
    {
        if (spriteRenderer != null) spriteRenderer.sprite = aberta ? spriteAberta : spriteFechada;
        if (colisorFisico != null) colisorFisico.enabled = !aberta;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            perto = true;
            if (iconeE != null) iconeE.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            perto = false;
            if (iconeE != null) iconeE.SetActive(false);
        }
    }
}