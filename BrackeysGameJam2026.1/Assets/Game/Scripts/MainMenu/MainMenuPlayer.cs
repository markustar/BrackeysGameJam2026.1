using UnityEngine;

public class MainMenuPlayer : MonoBehaviour
{
    [SerializeField] GameObject Text;
    [SerializeField] Animator anim;
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(Text != null)
            {
                Destroy(Text);
            }
            anim.SetTrigger("Attack");
        }
    }
}
