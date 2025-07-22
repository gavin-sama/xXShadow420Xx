using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerClassData playerClassData;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
}
