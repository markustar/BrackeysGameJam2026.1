using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{

    [SerializeField] Color _flashColor = Color.white;
    [SerializeField] float _flashTime = 0.25f;

    SpriteRenderer spriteRenderer;
    Material material;

    private Coroutine damageFlashCoroutine;

    void Awake()
    {   
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Innit();
    }

    void Innit()
    {
        material = spriteRenderer.material;
    }

    public void CallCouroutine()
    {
        damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }
    
    IEnumerator DamageFlasher()
    {
        SetFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < _flashTime)
        {
            //
            elapsedTime += Time.deltaTime;
            //
            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / _flashTime));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    void SetFlashColor()
    {
        material.SetColor("_FlashColor", _flashColor);
    }

    void SetFlashAmount(float amount)
    {
        material.SetFloat("_FlashAmount", amount);
    }
}
