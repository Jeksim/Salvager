using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int id;
    public int amount;

    public GameObject hoverObject;
    public GameObject purchased;

    public float normalScale = 1f;
    public float hoverScale = 1.08f;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        CheckPurchased();
        UpdateInteractable();
        ResetVisuals();
    }

    void OnEnable()
    {
        CheckPurchased();
        UpdateInteractable();
        ResetVisuals();
    }

    public void UpdateInteractable()
    {
        if (button == null) return;

        string key = "PlayerUpgrade" + id;
        bool hasUpgrade = PlayerPrefs.GetInt(key, 0) == 1;
        int coins = PlayerPrefs.GetInt("Coins", 0);

        if (hasUpgrade)
        {
            button.interactable = false;
            return;
        }

        button.interactable = coins >= amount;
    }

    public void CheckPurchased()
    {
        string key = "PlayerUpgrade" + id;
        bool hasUpgrade = PlayerPrefs.GetInt(key, 0) == 1;

        if (hasUpgrade)
        {
            gameObject.SetActive(false);

            if (purchased != null)
            {
                purchased.SetActive(true);
            }

            ResetVisuals();
        }
        else
        {
            if (purchased != null)
            {
                purchased.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null) return;

        // ONLY block if purchased
        string key = "PlayerUpgrade" + id;
        bool hasUpgrade = PlayerPrefs.GetInt(key, 0) == 1;

        if (hasUpgrade) return;

        if (hoverObject != null)
        {
            hoverObject.SetActive(true);
        }

        transform.localScale = Vector3.one * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetVisuals();
    }

    void ResetVisuals()
    {
        if (hoverObject != null)
        {
            hoverObject.SetActive(false);
        }

        transform.localScale = Vector3.one * normalScale;
    }
    public void RefreshButton()
    {
        CheckPurchased();
        UpdateInteractable();
        ResetVisuals();
    }
}