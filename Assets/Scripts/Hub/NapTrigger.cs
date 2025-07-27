using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
public class NapTrigger : MonoBehaviour
{
    public string tutorialSceneName = "TutorialScene";
    public GameObject napPromptUI;

    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear)
        {
            Debug.Log("Player is near");

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E was pressed");
                StartCoroutine(FadeAndLoad());
            }
        }
    }


    IEnumerator FadeAndLoad()
    {
        napPromptUI.SetActive(false); // hide prompt
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(tutorialSceneName);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            isPlayerNear = true;
            napPromptUI.SetActive(true);
        }
        else
        {
            Debug.Log("Player not entered");
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            napPromptUI.SetActive(false);
        }
    }
}
