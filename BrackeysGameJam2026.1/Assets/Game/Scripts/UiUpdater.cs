using UnityEngine;


public class UiUpdater : MonoBehaviour
{
    
    [SerializeField] GameObject TorchUnlit;
    [SerializeField] GameObject TorchLit;
  
    bool isLit = false;
    PlayerMovement player;
    [SerializeField] public bool hasTorch;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
    }
    void Start()
    {
        TorchLit.SetActive(false);
        
        isLit = false;
    }

    // Update is called once per frame
    void Update()
    {
        TorchOnandOffandUiUpdater();
        
    }

    private void TorchOnandOffandUiUpdater()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isLit && hasTorch)
        {
            isLit = true;
            TorchLit.SetActive(true);
            
            TorchUnlit.SetActive(false);
        }

        else if (Input.GetKeyDown(KeyCode.F) && isLit)
        {
            isLit = false;
            TorchLit.SetActive(false);
            
            TorchUnlit.SetActive(true);
        }
    }
}
