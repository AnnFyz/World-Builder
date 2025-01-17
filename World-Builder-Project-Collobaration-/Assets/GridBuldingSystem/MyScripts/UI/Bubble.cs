using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Bubble : MonoBehaviour
{
    public static Bubble Instance { get; private set; }
    [SerializeField] Transform bubblePf;
    [SerializeField] Transform popupTextPf;
    private void Awake()
    {
        Instance = this;
    }

    public void CreateBubble(Vector3 pos, string message)
    {
        Transform bubble = Instantiate(bubblePf, Camera.main.WorldToScreenPoint(pos), Quaternion.identity, transform);
        TMP_Text text = bubble.gameObject.GetComponentInChildren<TMP_Text>();
        text.SetText(message);
     
        Destroy(bubble.gameObject, 5f);
    }

    public void CreatePopupText(Vector3 pos, string message)
    {
        Transform popupText = Instantiate(popupTextPf, Camera.main.WorldToScreenPoint(pos), Quaternion.identity, transform);
        TMP_Text text = popupText.gameObject.GetComponentInChildren<TMP_Text>();
        text.SetText(message);

        Destroy(popupText.gameObject, 5f);
    }
}
