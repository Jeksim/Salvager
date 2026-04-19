using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float normalScale = 1f;
    public float hoverScale = 1.2f;
    public float otherScale = 0.9f;

    private static bool anyHovered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        anyHovered = true;

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject btn in buttons)
        {
            if (btn == gameObject)
            {
                btn.transform.localScale = Vector3.one * hoverScale;
            }
            else
            {
                btn.transform.localScale = Vector3.one * otherScale;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anyHovered = false;

        ResetAllButtons();
    }

    void ResetAllButtons()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject btn in buttons)
        {
            btn.transform.localScale = Vector3.one * normalScale;
        }
    }
}