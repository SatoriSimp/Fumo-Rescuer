using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidePaper : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text text;

    private bool isShowingPaper = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            text.gameObject.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            text.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ShowPaper();
            }
        }
    }

    void ShowPaper()
    {
        isShowingPaper = !isShowingPaper;
        canvas.gameObject.SetActive(isShowingPaper);
    }

    public void HidePaper()
    {
        canvas.gameObject.SetActive(false);
        isShowingPaper = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            text.gameObject.SetActive(false);
    }
}
