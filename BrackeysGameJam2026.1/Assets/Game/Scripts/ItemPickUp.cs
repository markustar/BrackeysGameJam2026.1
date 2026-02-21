using Unity.Mathematics;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    PlayerMovement player;
    UiUpdater ui;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        ui = FindAnyObjectByType<UiUpdater>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {   
        
        if(player)
        {   
            player.hasWeapon = true;
            Destroy(this.gameObject);
        }    

          
    }

    
}
