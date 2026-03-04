using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite spriteAberta;
    public Sprite spriteFechada;
    public GameObject iconeE;
    public BoxCollider2D colisorFisico; 

    private bool perto;
    private bool aberta;

    void Start()
    {
        if (iconeE != null) iconeE.SetActive(false);
        AtualizarVisualDaPorta();
    }

    void Update()
    {
        if (perto && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor(); 
        }
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