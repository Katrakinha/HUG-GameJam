using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static Sprite[] paineisAtuais;
    public static int proximaCenaIndex;

    public Image telaDeImagem;
    public float tempoDeFade = 1f;

    private int painelAtualIndex = 0;
    private bool emTransicao = false;

    void Start()
    {
        if (paineisAtuais != null && paineisAtuais.Length > 0 && telaDeImagem != null)
        {
            telaDeImagem.gameObject.SetActive(true);
            telaDeImagem.sprite = paineisAtuais[0];

            Color corInicial = telaDeImagem.color;
            corInicial.a = 0f;
            telaDeImagem.color = corInicial;

            StartCoroutine(FadeIn());
        }
        else
        {
            PularCena();
        }
    }

    void Update()
    {
        if (emTransicao) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            AvancarPainel();
        }
    }

    private void AvancarPainel()
    {
        painelAtualIndex++;

        if (painelAtualIndex < paineisAtuais.Length)
        {
            telaDeImagem.sprite = paineisAtuais[painelAtualIndex];
        }
        else
        {
            StartCoroutine(FadeOutEPular());
        }
    }

    private IEnumerator FadeIn()
    {
        emTransicao = true;
        Color cor = telaDeImagem.color;

        float tempo = 0f;
        while (tempo < tempoDeFade)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(0f, 1f, tempo / tempoDeFade);
            telaDeImagem.color = cor;
            yield return null;
        }

        cor.a = 1f;
        telaDeImagem.color = cor;
        emTransicao = false;
    }

    private IEnumerator FadeOutEPular()
    {
        emTransicao = true;
        Color cor = telaDeImagem.color;
        
        float tempo = 0f;
        while (tempo < tempoDeFade)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(1f, 0f, tempo / tempoDeFade);
            telaDeImagem.color = cor;
            yield return null;
        }

        cor.a = 0f;
        telaDeImagem.color = cor;
        PularCena();
    }

    private void PularCena()
    {
        SceneManager.LoadScene(proximaCenaIndex);
    }

    public static void PrepararEIniciarCutscene(Sprite[] paineis, int proximaCena)
    {
        paineisAtuais = paineis;
        proximaCenaIndex = proximaCena;
        SceneManager.LoadScene("SalaDeCutscene");
    }
}