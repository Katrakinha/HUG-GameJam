using UnityEngine;
using UnityEngine.Events; // A mágica para conectar coisas no Inspector!

public class PressureButton : MonoBehaviour
{
    [Header("Quem pode apertar?")]
    // Uma lista para você poder adicionar quantas Tags quiser no futuro
    public string[] tagsPermitidas = { "Player", "Quantum" }; 

    [Header("O que o botão faz?")]
    public UnityEvent aoApertar;
    public UnityEvent aoSoltar;

    [Header("Visual do Botão (Opcional)")]
    public SpriteRenderer spriteBotao;
    public Color corApertado = Color.green;
    private Color corOriginal;

    // O nosso contador salva-vidas
    private int objetosEmCima = 0;

    void Start()
    {
        if (spriteBotao == null) spriteBotao = GetComponent<SpriteRenderer>();
        if (spriteBotao != null) corOriginal = spriteBotao.color;
    }

    // Função rápida para checar se quem pisou tem a carteirinha VIP
    bool TemTagPermitida(string tagParaChecar)
    {
        foreach (string tag in tagsPermitidas)
        {
            if (tagParaChecar == tag) return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se quem pisou tem uma das tags permitidas...
        if (TemTagPermitida(collision.tag))
        {
            objetosEmCima++;
            
            // Se foi o PRIMEIRO a pisar, a gente afunda o botão e abre a porta!
            if (objetosEmCima == 1) 
            {
                if (spriteBotao != null) spriteBotao.color = corApertado;
                aoApertar.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (TemTagPermitida(collision.tag))
        {
            objetosEmCima--;
            
            // Se o ÚLTIMO objeto saiu de cima, a gente solta o botão e fecha a porta!
            if (objetosEmCima <= 0) 
            {
                objetosEmCima = 0; // Prevenção de bugs de física
                if (spriteBotao != null) spriteBotao.color = corOriginal;
                aoSoltar.Invoke();
            }
        }
    }
}