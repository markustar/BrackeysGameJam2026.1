using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public GameObject PressButtonCanvas;
    public int SceneIndex;

    private bool isPlayerInRange;

    void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("acaxs");
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().LoadScene(SceneIndex);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            PressButtonCanvas.SetActive(true);
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            PressButtonCanvas.SetActive(false);
            isPlayerInRange = false;
        }
    }
}
