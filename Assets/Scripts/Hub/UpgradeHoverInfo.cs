using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
    [SerializeField] private string description;

    private PermUpMenu permUpMenu;

    void Start()
    {
        permUpMenu = FindFirstObjectByType<PermUpMenu>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (permUpMenu != null)
            permUpMenu.ShowUpgradeDescription(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (permUpMenu != null)
            permUpMenu.ClearUpgradeDescription();
    }
}