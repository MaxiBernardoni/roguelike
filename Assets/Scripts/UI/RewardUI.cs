using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tres botones con mejoras, reroll una vez por oleada y cierre al elegir.
/// </summary>
public class RewardUI : MonoBehaviour
{
    [SerializeField] GameObject panelRoot;
    [SerializeField] Text titleText;
    [SerializeField] Text[] nameTexts;
    [SerializeField] Text[] descTexts;
    [SerializeField] Button[] buttons;
    [SerializeField] Button rerollButton;
    [SerializeField] Text rerollButtonLabel;

    Upgrade[] currentOptions;
    System.Action<Upgrade> onPicked;
    System.Func<Upgrade[]> rerollProvider;
    int rerollsRemaining;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (buttons == null)
        {
            Debug.LogError("RewardUI: asigna el array de Button (3 entradas).");
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int idx = i;
            if (buttons[i] != null)
                buttons[i].onClick.AddListener(() => Pick(idx));
        }

        if (rerollButton != null)
            rerollButton.onClick.AddListener(OnReroll);
    }

    public void Show(int waveNumber, Upgrade[] options, System.Action<Upgrade> callback)
    {
        Show(waveNumber, options, callback, null);
    }

    public void Show(
        int waveNumber,
        Upgrade[] options,
        System.Action<Upgrade> callback,
        System.Func<Upgrade[]> rerollProvider)
    {
        if (buttons == null)
        {
            Debug.LogError("RewardUI.Show: faltan referencias a Button.");
            callback?.Invoke(null);
            return;
        }

        currentOptions = options;
        onPicked = callback;
        this.rerollProvider = rerollProvider;
        rerollsRemaining = rerollProvider != null ? 1 : 0;

        if (titleText != null)
            titleText.text = "Oleada " + waveNumber + " completada — elige una mejora";

        FillOptionSlots();
        RefreshRerollUi();

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    void FillOptionSlots()
    {
        var options = currentOptions;
        int slotCount = buttons != null ? buttons.Length : 0;
        for (int i = 0; i < slotCount; i++)
        {
            bool ok = options != null && i < options.Length;
            if (nameTexts != null && i < nameTexts.Length && nameTexts[i] != null)
                nameTexts[i].text = ok ? options[i].Name : "—";
            if (descTexts != null && i < descTexts.Length && descTexts[i] != null)
                descTexts[i].text = ok ? options[i].Description : "";
            if (buttons[i] != null)
                buttons[i].interactable = ok;
        }
    }

    void RefreshRerollUi()
    {
        if (rerollButton == null)
            return;

        bool can = rerollsRemaining > 0 && rerollProvider != null;
        rerollButton.interactable = can;

        if (rerollButtonLabel != null)
            rerollButtonLabel.text = "Reroll (" + rerollsRemaining + ")";
    }

    void OnReroll()
    {
        if (rerollsRemaining <= 0 || rerollProvider == null)
            return;

        Upgrade[] next = rerollProvider.Invoke();
        if (next == null || next.Length == 0)
            return;

        currentOptions = next;
        rerollsRemaining = 0;
        FillOptionSlots();
        RefreshRerollUi();
    }

    void Pick(int index)
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        Upgrade chosen = null;
        if (currentOptions != null && index >= 0 && index < currentOptions.Length)
            chosen = currentOptions[index];

        currentOptions = null;
        rerollProvider = null;
        onPicked?.Invoke(chosen);
        onPicked = null;
    }
}
