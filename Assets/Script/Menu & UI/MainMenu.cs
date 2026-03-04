using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Debug.Log("O jogo fechou!");
        Application.Quit();
    }

    public void Credits()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void Controles()
    {
        SceneManager.LoadScene("Controles");
    }

    public void Continuar()
    {
        SceneManager.LoadScene("Fase 1");
    }
}