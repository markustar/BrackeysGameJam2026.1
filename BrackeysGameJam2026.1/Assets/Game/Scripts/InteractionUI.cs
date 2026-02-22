using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
     public static InteractionUI Instance;
    [SerializeField] GameObject interactPanel;
   [SerializeField] TextMeshProUGUI interactText;
   

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Show()
    {
        
        interactPanel.SetActive(true);
       
    }

    public void Hide()
    {
        interactPanel.SetActive(false);
    }
}
