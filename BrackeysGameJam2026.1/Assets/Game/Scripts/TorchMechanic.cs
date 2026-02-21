using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections.Generic;

public class TorchMechanic : MonoBehaviour
{
    [Header("Torch Settings")]
    [SerializeField] private Light2D torchLight;
    [SerializeField] private int batteryCount = 3;
    [SerializeField] private float batteryLifeSeconds = 15f;
    
    [Header("UI References")]
    [SerializeField] private GameObject batteryBarPrefab;
    [SerializeField] private Transform batteryContainer;
    [SerializeField] private float barSpacing = 40f;

    private float currentBatteryLife;
    private bool isTorchOn = false;
    private InputManager inputManager;
    private List<Image> instantiatedBars = new List<Image>();

    void Start()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        currentBatteryLife = batteryLifeSeconds;
        
        GenerateBars();
        UpdateTorchState();
        UpdateUI();
    }

    void Update()
    {
        HandleInput();
        HandleRotation();
        HandleBatteryConsumption();
        
        UpdateUI();
    }

    private void GenerateBars()
    {
        foreach (Transform child in batteryContainer)
        {
            Destroy(child.gameObject);
        }
        instantiatedBars.Clear();

        for (int i = 0; i < batteryCount; i++)
        {
            GameObject barObj = Instantiate(batteryBarPrefab, batteryContainer);
            
            RectTransform rect = barObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(i * -barSpacing, 0);
            }

            Image barImage = barObj.GetComponent<Image>();
            if (barImage != null)
            {
                instantiatedBars.Add(barImage);
            }
        }
    }

    private void HandleRotation()
    {
        if (inputManager == null) return;

        Vector2 moveInput = inputManager.GetInput();
        if (moveInput != Vector2.zero && torchLight != null)
        {
            float targetAngle = 0f;

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                // Horizontal priority
                targetAngle = moveInput.x > 0 ? -90f : 90f; // Right : Left
            }
            else
            {
                // Vertical priority
                targetAngle = moveInput.y > 0 ? 0f : 180f; // Up : Down
            }

            torchLight.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleTorch();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            AddBatteries(1);
        }
    }

    private void HandleBatteryConsumption()
    {
        if (isTorchOn && batteryCount > 0)
        {
            currentBatteryLife -= Time.deltaTime;

            if (currentBatteryLife <= 0)
            {
                ConsumeBattery();
            }
        }
        else if (isTorchOn && batteryCount <= 0)
        {
            isTorchOn = false;
            UpdateTorchState();
        }
    }

    private void ToggleTorch()
    {
        if (batteryCount > 0)
        {
            isTorchOn = !isTorchOn;
        }
        else
        {
            isTorchOn = false;
        }
        UpdateTorchState();
    }

    private void ConsumeBattery()
    {
        batteryCount--;
        if (batteryCount > 0)
        {
            currentBatteryLife = batteryLifeSeconds;
        }
        else
        {
            currentBatteryLife = 0;
            isTorchOn = false;
            UpdateTorchState();
        }
        
        GenerateBars();
        UpdateUI();
    }

    private void UpdateTorchState()
    {
        if (torchLight != null)
        {
            torchLight.enabled = isTorchOn;
        }
    }

    private void UpdateUI()
    {
        if (instantiatedBars.Count == 0) return;

        for (int i = 0; i < instantiatedBars.Count; i++)
        {
            Image barFill = instantiatedBars[i].transform.GetChild(0).GetComponent<Image>();

            if (i < batteryCount - 1)
            {
                barFill.fillAmount = 1f;
            }
            else if (i == batteryCount - 1)
            {
                barFill.fillAmount = currentBatteryLife / batteryLifeSeconds;
            }
            else
            {
                barFill.fillAmount = 0f;
            }
        }
    }

    public void AddBatteries(int amount)
    {
        batteryCount += amount;
        GenerateBars();
        UpdateUI();
    }
}
