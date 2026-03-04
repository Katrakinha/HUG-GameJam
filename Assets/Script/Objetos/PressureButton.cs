using UnityEngine;
using UnityEngine.Events;

public class PressureButton : MonoBehaviour
{
    [Header("Configurações")]
    public string[] tagsPermitidas = { "Player", "Quantum" }; 

    [Header("Eventos")]
    public UnityEvent aoApertar;
    public UnityEvent aoSoltar;

    [Header("Visual")]
    public SpriteRenderer spriteBotao;
    public Color corApertado = Color.green;
    
    private Color corOriginal;
    private int objetosEmCima;

    void Start()
    {
        if (spriteBotao == null) spriteBotao = GetComponent<SpriteRenderer>();
        if (spriteBotao != null) corOriginal = spriteBotao.color;
    }

    private bool TemTagPermitida(string tagParaChecar)
    {
        for (int i = 0; i < tagsPermitidas.Length; i++)
        {
            if (tagParaChecar == tagsPermitidas[i]) return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TemTagPermitida(collision.tag))
        {
            objetosEmCima++;
            
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
            
            if (objetosEmCima <= 0) 
            {
                objetosEmCima = 0; 
                if (spriteBotao != null) spriteBotao.color = corOriginal;
                aoSoltar.Invoke();
            }
        }
    }
}