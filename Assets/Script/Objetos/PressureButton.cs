using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [Header("Configuração")]
    public string targetTag = "Quantum"; // A etiqueta de quem pode apertar o botão

    // Quando algo ENTRA na área do botão
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checa se quem pisou tem a Tag que queremos
        if (collision.CompareTag(targetTag))
        {
            Debug.Log("🟢 O objeto quântico PISOU no botão!");
        }
    }

    // Quando algo SAI da área do botão
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            Debug.Log("🔴 O objeto quântico SAIU do botão!");
        }
    }
}