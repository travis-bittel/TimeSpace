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
    /// Change the currently displayed popup text.  
    /// Call with no args to hide text.
    /// </summary>
    public void UpdatePopupText(string text = "")
    {
        textComponent.text = text;
    }
}
