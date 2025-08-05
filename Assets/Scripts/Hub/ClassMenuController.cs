using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ClassMenuController : MonoBehaviour
{
    public GameObject classMenuUI;
    public Image outfitDisplay;
    public TextMeshProUGUI outfitNameText;
    public Sprite[] outfitSprites;
    public string[] outfitNames; 

    private int currentIndex = 0;

    public int selectedOutfitIndex = 0; // store the confirmed outfit


    void Start()
    {
        classMenuUI.SetActive(false);
        UpdateOutfit();
    }

    public void OpenMenu()
    {
        Debug.Log($"Before open, activeSelf: {classMenuUI.activeSelf}");
        classMenuUI.SetActive(true);

        // Show & unlock cursor so UI can be clicked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
        Debug.Log($"After open, activeSelf: {classMenuUI.activeSelf}");
    }

    public void CloseMenu()
    {
        Debug.Log("CloseMenu called");
        classMenuUI.SetActive(false);

        // Lock & hide cursor again to resume gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void ShowNext()
    {
        Debug.Log("ShowNext called");
        currentIndex = (currentIndex + 1) % outfitSprites.Length;
        UpdateOutfit();
    }

    public void ShowPrevious()
    {
        Debug.Log("ShowPrevious called");
        currentIndex = (currentIndex - 1 + outfitSprites.Length) % outfitSprites.Length;
        UpdateOutfit();
    }

    void UpdateOutfit()
    {
        outfitDisplay.sprite = outfitSprites[currentIndex];
        outfitNameText.text = outfitNames[currentIndex]; // <- ADD THIS
    }

    public void ConfirmOutfit()
    {
        selectedOutfitIndex = currentIndex;
        Debug.Log($"Outfit confirmed: {outfitNames[selectedOutfitIndex]}");
        Debug.Log($"Outfit index confirmed: {selectedOutfitIndex}");
        PlayerData.SelectedOutfitIndex = currentIndex;
        CloseMenu();  // optionally close menu right after confirming
    }
}

