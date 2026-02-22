using UnityEngine;

public class PlayerIneract : MonoBehaviour
{
   [SerializeField] float interactDistance;
   [SerializeField] private LayerMask IsInteractable;
   InputManager inputManager;

    void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
    }
    void Start()
    {
        InteractionUI.Instance.Hide();
    }

   
    void Update()
    {   
       
        CheckIfInteractable();
    }

    void CheckIfInteractable()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactDistance, IsInteractable);

        if(hit != null)
        {   
            if(hit.TryGetComponent(out EngineStart engineStart))
            {   
                Debug.Log(hit.name);
                InteractionUI.Instance.Show();
                if(Input.GetKeyDown(KeyCode.E))
                engineStart.Interact();
            }
            
            
        }
        
    }
}
