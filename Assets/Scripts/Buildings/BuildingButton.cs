using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floormask = new LayerMask();

    private Camera mainCamera;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;

    private void Awake()
    {
        mainCamera = Camera.main;

        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
    }

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (buildingPreviewInstance == null) { return; }

        UpdateBuildingPreview();
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floormask)) { return; }

        buildingPreviewInstance.transform.position = hit.point;

        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }
    }    

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floormask))
        {
            // place building
            Debug.Log("Building will be placed here");
        }

        Destroy(buildingPreviewInstance);
    }
}