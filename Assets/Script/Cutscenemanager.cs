using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static Sprite[] paineisAtuais;
    public static Color corDaSetaAtual = Color.white;
    public static int proximaCenaIndex;

    public Image telaDeImagem;
    public Image setaAvancar;
    public float tempoDeFade = 1f;

    private int painelAtualIndex = 0;
    private bool emTransicao = false;

    void Start()
    {
        if (setaAvancar != null)
        {
            setaAvancar.color = corDaSetaAtual;
            setaAvancar.gameObject.SetActive(false);
        }

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

    public void AvancarPainel()
    {
        if (emTransicao) return; 

        painelAtualIndex++;

        if (painelAtualIndex < paineisAtuais.Length)
        {
            telaDeImagem.sprite = paineisAtuais[painelAtualIndex];
        }
        else
        {
            if (setaAvancar != null) setaAvancar.gameObject.SetActive(false);
            StartCoroutine(FadeOutEPular());
        }
    }

    private IEnumerator FadeIn()
    {
        emTransicao = true;
        if (setaAvancar != null) setaAvancar.gameObject.SetActive(false);

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

        if (setaAvancar != null) setaAvancar.gameObject.SetActive(true);
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

    public static void PrepararEIniciarCutscene(Sprite[] paineis, Color corSeta, int proximaCena)
    {
        paineisAtuais = paineis;
        corDaSetaAtual = corSeta;
        proximaCenaIndex = proximaCena;
        SceneManager.LoadScene("SalaDeCutscene");
    }
}