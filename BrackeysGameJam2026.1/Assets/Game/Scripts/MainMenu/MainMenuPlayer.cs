using UnityEngine;

public class MainMenuPlayer : MonoBehaviour
{
    [SerializeField] Animator anim;
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }
    }
}
