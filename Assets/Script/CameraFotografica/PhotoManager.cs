using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public RenderTexture viewfinderTexture;
    public GameObject molduraPolaroid;

    [Header("Efeitos (Game Feel)")]
    public Image flashPanel;         
    public float flashDuration = 0.2f; 

    public class PhotoData
    {
        public Vector3 position;
        public Vector3 upDirection;
        public Texture2D screenshot;
    }

    private List<PhotoData> activePhotos = new List<PhotoData>();
    private bool isAiming;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        if (previewCamera != null) previewCamera.gameObject.SetActive(false);
        if (molduraPolaroid != null) molduraPolaroid.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && activePhotos.Count > 0)
        {
            ClearAllPhotos();
        }

        if (activePhotos.Count == 0)
        {
            if (Input.GetMouseButtonDown(0)) StartAiming();
            if (Input.GetMouseButton(0) && isAiming) MoveViewfinder(); 
            if (Input.GetMouseButtonUp(0) && isAiming) StartCoroutine(TakePhotoCoroutine()); 
        }
    }

    void StartAiming()
    {
        isAiming = true;
        previewCamera.gameObject.SetActive(true); 
        
        if (photoUIElement != null)
        {
            photoUIElement.texture = viewfinderTexture;
            photoUIElement.color = Color.white;
        }
    }

    void StopAiming()
    {
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
        yield return new WaitForEndOfFrame();

        RenderTexture.active = viewfinderTexture;
        Texture2D frozenImage = new Texture2D(viewfinderTexture.width, viewfinderTexture.height, TextureFormat.RGB24, false);
        frozenImage.ReadPixels(new Rect(0, 0, viewfinderTexture.width, viewfinderTexture.height), 0, 0);
        frozenImage.Apply();
        RenderTexture.active = null;

        isAiming = false; 
        previewCamera.gameObject.SetActive(false); 

        PhotoData newPhoto = new PhotoData
        {
            position = player.transform.position,
            upDirection = player.transform.up,
            screenshot = frozenImage
        };

        activePhotos.Add(newPhoto);

        GameObject[] keyObjects = GameObject.FindGameObjectsWithTag("KeyObject");

        foreach (GameObject keyObj in keyObjects)
        {
            if (IsPhotoSeeing(keyObj.transform.position))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        if (photoUIElement != null) photoUIElement.texture = frozenImage;
        if (flashPanel != null) StartCoroutine(FlashEffect());
        if (molduraPolaroid != null) molduraPolaroid.SetActive(true);
    }

    IEnumerator FlashEffect()
    {
        Color flashColor = flashPanel.color;
        flashColor.a = 1f;
        flashPanel.color = flashColor;

        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            flashColor.a = Mathf.Lerp(1f, 0f, elapsedTime / flashDuration);
            flashPanel.color = flashColor;
            yield return null; 
        }

        flashColor.a = 0f;
        flashPanel.color = flashColor;
    }

    void ClearAllPhotos()
    {
        foreach (PhotoData photo in activePhotos)
        {
            Destroy(photo.screenshot); 
        }
        activePhotos.Clear();

        if (Input.GetMouseButton(0)) StartAiming();
        else StopAiming();

        if (molduraPolaroid != null) molduraPolaroid.SetActive(false); 
    }

    public bool IsPhotoSeeing(Vector3 targetPosition)
    {
        foreach (PhotoData photo in activePhotos)
        {
            Vector3 dirToTarget = (targetPosition - photo.position).normalized;
            float distance = Vector3.Distance(photo.position, targetPosition);

            if (distance <= player.viewRadius && Vector3.Angle(photo.upDirection, dirToTarget) < player.viewAngle / 2)
            {
                if (!Physics2D.Raycast(photo.position, dirToTarget, distance, player.obstacleLayer))
                {
                    return true; 
                }
            }
        }
        return false;
    }

    void MoveViewfinder()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(mainCam.transform.position.z); 
        
        Vector3 mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = -10f;

        previewCamera.transform.position = mouseWorldPosition;
        previewCamera.transform.rotation = Quaternion.identity; 
    }
}