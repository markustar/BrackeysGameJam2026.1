

using System;

using UnityEngine;

public class EngineStart : MonoBehaviour, IInteractable
{   
    
   [SerializeField] AudioClip trainStartEngine;
   [Range(0,1)] [SerializeField] float volume;


   
    public bool hasStarted = false;
    public void Interact()
    {
        if (hasStarted) return;
        hasStarted = true;
        SoundFXManager.instance.PlaySoundFXClip(trainStartEngine, this.transform, volume);
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    

    public string GetInteracttext()
    {
        if(hasStarted) return "";
        return "E - Push To Start The Engine";

    }

    public bool HasStarted()
    {
        return hasStarted;
    }
}
