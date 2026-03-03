using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PhotoManager : MonoBehaviour
{
    [Header("Configurações da Câmera")]
    public int maxPhotos = 1;       
    public PlayerController player; 
    public Camera previewCamera;
    public float viewfinderDistance = 3f;

    [Header("Interface (UI)")]
    public RawImage photoUIElement; 
    public RenderTexture viewfinderTexture; // Arraste a sua Render Texture aqui

    [Header("Efeitos (Game Feel)")]
    public Image flashPanel;         // Arraste a sua TelaDeFlash aqui
    public float flashDuration = 0.2f; // Tempo que o flash demora pra sumir (0.2s é bem rápido e legal)

    public class PhotoData
    {
        public Vector3 position;
        public Vector3 upDirection;
        public Texture2D screenshot;
    }

    private List<PhotoData> activePhotos = new List<PhotoData>();
    private bool isAiming = false;

    void Start()
    {
        // Garante que a câmera de preview comece desligada
        if (previewCamera != null) previewCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1. O DESCARTAR (Botão Direito)
        // Se clicar com o direito e tiver foto na tela, apaga.
        if (Input.GetMouseButtonDown(1))
        {
            if (activePhotos.Count > 0)
            {
                ClearAllPhotos();
            }
        }

        // 2. A MIRA E A FOTO (Botão Esquerdo)
        // Só permitimos usar a câmera se a tela estiver livre (sem foto ativa)
        if (activePhotos.Count == 0)
        {
            // Apertou o esquerdo: Liga a câmera
            if (Input.GetMouseButtonDown(0))
            {
                StartAiming();
            }

            // Enquanto estiver segurando o esquerdo: a câmera segue o mouse
            if (Input.GetMouseButton(0) && isAiming)
            {
                MoveViewfinder(); 
            }

            // Soltou o botão esquerdo: Tira a foto! (Em vez de chamar StopAiming, agora chama a foto)
            if (Input.GetMouseButtonUp(0) && isAiming)
            {
                StartCoroutine(TakePhotoCoroutine()); 
            }
        }
    }

    void StartAiming()
    {
        isAiming = true;
        previewCamera.gameObject.SetActive(true); // Liga a câmera de zoom
        
        // Coloca o feed ao vivo (Render Texture) na tela
        if (photoUIElement != null)
        {
            photoUIElement.texture = viewfinderTexture;
            photoUIElement.color = Color.white;
        }
    }

    void StopAiming()
    {
        // Só desliga a mira se não tivermos uma foto congelada ativa
        if (activePhotos.Count == 0)
        {
            isAiming = false;
            previewCamera.gameObject.SetActive(false);
            
            if (photoUIElement != null)
            {
                photoUIElement.texture = null;
                photoUIElement.color = Color.clear;
            }
        }
    }

    IEnumerator TakePhotoCoroutine()
    {
        // 1. O SEGREDO: Espera o jogo terminar de desenhar todas as luzes e sprites deste frame
        yield return new WaitForEndOfFrame();

        // 2. Extrai a foto da Render Texture ENQUANTO a câmera ainda está ligada
        RenderTexture.active = viewfinderTexture;
        Texture2D frozenImage = new Texture2D(viewfinderTexture.width, viewfinderTexture.height, TextureFormat.RGB24, false);
        frozenImage.ReadPixels(new Rect(0, 0, viewfinderTexture.width, viewfinderTexture.height), 0, 0);
        frozenImage.Apply();
        RenderTexture.active = null;

        // 3. SÓ AGORA que a foto tá salva no PC, nós desligamos a mira
        isAiming = false; 
        previewCamera.gameObject.SetActive(false); 

        // 4. Salva as informações pro Objeto Quântico
        PhotoData newPhoto = new PhotoData();
        newPhoto.position = player.transform.position;
        newPhoto.upDirection = player.transform.up;
        newPhoto.screenshot = frozenImage;

        activePhotos.Add(newPhoto);

        GameObject[] keyObjects = GameObject.FindGameObjectsWithTag("KeyObject");

        foreach (GameObject keyObj in keyObjects)
        {
            // A nossa função mágica: A foto que acabamos de tirar conseguiu ver a chave?
            if (IsPhotoSeeing(keyObj.transform.position))
            {
                Debug.Log("📸 Capturou o Objeto Chave! Iniciando salto quântico para a próxima fase...");
                
                // Pula automaticamente para a próxima cena na fila do Build Settings
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        // 5. Cola a foto congelada na UI
        if (photoUIElement != null)
        {
            photoUIElement.texture = frozenImage;
        }

        if (flashPanel != null)
        {
            StartCoroutine(FlashEffect());
        }
        
        Debug.Log("📸 Foto Tirada! Realidade congelada na Render Texture.");
    }

    IEnumerator FlashEffect()
    {
        // 1. Acende a tela inteira instantaneamente (Alpha = 1)
        Color flashColor = flashPanel.color;
        flashColor.a = 1f;
        flashPanel.color = flashColor;

        float elapsedTime = 0f;

        // 2. Vai diminuindo o Alpha aos poucos até dar o tempo do flashDuration
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // O Lerp calcula a transição suave do 1 (visível) para o 0 (invisível)
            flashColor.a = Mathf.Lerp(1f, 0f, elapsedTime / flashDuration);
            flashPanel.color = flashColor;
            
            // Espera o próximo frame para continuar o ciclo
            yield return null; 
        }

        // 3. Garante que no final terminou 100% invisível para não deixar a tela cinza
        flashColor.a = 0f;
        flashPanel.color = flashColor;
    }

    void ClearAllPhotos()
    {
        foreach (PhotoData photo in activePhotos)
        {
            Destroy(photo.screenshot); // Libera memória
        }
        activePhotos.Clear();

        // Game Feel: Se o player ainda estiver segurando o esquerdo quando apagar a foto, volta a mirar direto!
        if (Input.GetMouseButton(0))
        {
            StartAiming();
        }
        else
        {
            StopAiming();
        }
        
        Debug.Log("🗑️ Foto Descartada!");
    }

    // A lógica de checagem do Objeto Quântico continua idêntica!
    public bool IsPhotoSeeing(Vector3 targetPosition)
    {
        foreach (PhotoData photo in activePhotos)
        {
            Vector3 dirToTarget = (targetPosition - photo.position).normalized;
            float distance = Vector3.Distance(photo.position, targetPosition);

            if (distance <= player.viewRadius)
            {
                if (Vector3.Angle(photo.upDirection, dirToTarget) < player.viewAngle / 2)
                {
                    if (!Physics2D.Raycast(photo.position, dirToTarget, distance, player.obstacleLayer))
                    {
                        return true; 
                    }
                }
            }
        }
        return false;
    }

    void MoveViewfinder()
    {
        // 1. Acha EXATAMENTE onde a pontinha do mouse está no mundo do jogo
        Vector3 mouseScreenPosition = Input.mousePosition;
        
        // Garante que o conversor entenda a profundidade da câmera principal
        mouseScreenPosition.z = Mathf.Abs(Camera.main.transform.position.z); 
        
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // 2. Trava o eixo Z em -10 para a câmera fotográfica não entrar na terra
        mouseWorldPosition.z = -10f;

        // 3. A mágica: A câmera teleporta exatamente para a posição do mouse!
        previewCamera.transform.position = mouseWorldPosition;
        
        // 4. Garante que ela não vai dar piruetas, ficando travada no eixo reto
        previewCamera.transform.rotation = Quaternion.identity; 
    }
}