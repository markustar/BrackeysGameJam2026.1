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
        
        if (isTorchOn)
        {
            UpdateUI();
        }
    }

    private void GenerateBars()
    {
        // Clear existing bars if any
        foreach (Transform child in batteryContainer)
        {
            Destroy(child.gameObject);
        }
        instantiatedBars.Clear();

        // Generate new bars
        for (int i = 0; i < batteryCount; i++)
        {
            GameObject barObj = Instantiate(batteryBarPrefab, batteryContainer);
            
            // Apply spacing to spread to the left
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
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            torchLight.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
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
        
        GenerateBars(); // Re-generate to remove the spent bar
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
            if (i < batteryCount - 1)
            {
                instantiatedBars[i].fillAmount = 1f;
            }
            else if (i == batteryCount - 1)
            {
                instantiatedBars[i].fillAmount = currentBatteryLife / batteryLifeSeconds;
            }
            else
            {
                instantiatedBars[i].fillAmount = 0f;
            }
        }
    }

    public void AddBatteries(int amount)
    {
        batteryCount += amount;
        GenerateBars(); // Re-generate bars to show the increase
        UpdateUI();
    }
}
