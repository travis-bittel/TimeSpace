using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PopupTextHandler : MonoBehaviour
{
    #region Singleton
    private static PopupTextHandler _instance;

    public static PopupTextHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (this == _instance) { _instance = null; }
    }
    #endregion

    [SerializeField] private TextMeshProUGUI textComponent;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(textComponent, "textComponent was null for Dialogue");
    }

    /// <summary>
    /// Change the currently displayed popup text. Call with no args to hide text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="typeLine">Whether to "type" the text one character at a time or display it all immediately.</param>
    public void UpdatePopupText(string text = "", bool typeLine = false)
    {
        StopAllCoroutines();
        if (typeLine)
        {
            StartCoroutine(TypeLine(text));
        } else
        {
            textComponent.text = text;
        }
    }

    IEnumerator TypeLine(string text)
    {
        textComponent.text = "";
        foreach (char c in text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
