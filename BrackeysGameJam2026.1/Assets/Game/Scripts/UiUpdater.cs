using UnityEngine;


public class UiUpdater : MonoBehaviour
{
    [SerializeField] GameObject BatteryUI;
    [SerializeField] GameObject TorchUnlit;
    [SerializeField] GameObject TorchLit;
    [SerializeField] GameObject torchLightSpot;
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
        torchLightSpot.SetActive(false);
        isLit = false;
    }

    // Update is called once per frame
    void Update()
    {
        TorchOnandOffandUiUpdater();
        player.TorchRotater(torchLightSpot);
    }

    private void TorchOnandOffandUiUpdater()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isLit && hasTorch)
        {
            isLit = true;
            TorchLit.SetActive(true);
            torchLightSpot.SetActive(true);
            TorchUnlit.SetActive(false);
        }

        else if (Input.GetKeyDown(KeyCode.J) && isLit)
        {
            isLit = false;
            TorchLit.SetActive(false);
            torchLightSpot.SetActive(false);
            TorchUnlit.SetActive(true);
        }
    }
}
