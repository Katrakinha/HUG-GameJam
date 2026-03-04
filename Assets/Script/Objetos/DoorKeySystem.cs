using UnityEngine;

public class DoorKeySystem : MonoBehaviour
{
    [Header("Visuais da Porta")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteAberta;
    public Sprite spriteFechada;
    
    [Header("Física")]
    public BoxCollider2D colisorFisico; 

    private bool aberta = false;

    void Start()
    {
        AtualizarVisualDaPorta(); 
    }

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

    private void AtualizarVisualDaPorta()
    {
        if (spriteRenderer != null) spriteRenderer.sprite = aberta ? spriteAberta : spriteFechada;
        if (colisorFisico != null) colisorFisico.enabled = !aberta; 
    }
}