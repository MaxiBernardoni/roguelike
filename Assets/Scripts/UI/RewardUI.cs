using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tres botones con nombre y descripción de mejoras (estilo ROUNDS).
/// </summary>
public class RewardUI : MonoBehaviour
{
    [SerializeField] GameObject panelRoot;
    [SerializeField] Text titleText;
    [SerializeField] Text[] nameTexts;
    [SerializeField] Text[] descTexts;
    [SerializeField] Button[] buttons;

    Upgrade[] currentOptions;
    System.Action<Upgrade> onPicked;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        for (int i = 0; i < buttons.Length; i++)
        {
            int idx = i;
            if (buttons[i] != null)
                buttons[i].onClick.AddListener(() => Pick(idx));
        }
    }

    public void Show(int waveNumber, Upgrade[] options, System.Action<Upgrade> callback)
    {
        currentOptions = options;
        onPicked = callback;

        if (titleText != null)
            titleText.text = "Oleada " + waveNumber + " completada — elige una mejora";

        for (int i = 0; i < 3; i++)
        {
            bool ok = options != null && i < options.Length;
            if (nameTexts != null && i < nameTexts.Length && nameTexts[i] != null)
                nameTexts[i].text = ok ? options[i].Name : "—";
            if (descTexts != null && i < descTexts.Length && descTexts[i] != null)
                descTexts[i].text = ok ? options[i].Description : "";
            if (buttons != null && i < buttons.Length && buttons[i] != null)
                buttons[i].interactable = ok;
        }

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    void Pick(int index)
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        Upgrade chosen = null;
        if (currentOptions != null && index >= 0 && index < currentOptions.Length)
            chosen = currentOptions[index];

        currentOptions = null;
        onPicked?.Invoke(chosen);
        onPicked = null;
    }
}
