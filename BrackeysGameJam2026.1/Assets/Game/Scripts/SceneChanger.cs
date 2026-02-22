using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public int SceneIndex;

    private bool isPlayerInRange;

    void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("acaxs");
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().LoadScene(SceneIndex);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
