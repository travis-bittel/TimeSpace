using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class AmmoTextHandler : MonoBehaviour
{
    #region Singleton
    private static AmmoTextHandler _instance;

    public static AmmoTextHandler Instance { get => _instance; }

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

    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        Assert.IsNotNull(text, "AmmoTextHandler couldn't find text component");
    }

    public void UpdateText(string newText)
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        text.text = newText;
    }
}
