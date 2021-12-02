using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Assertions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Dialogue : MonoBehaviour
{
    #region Singleton
    private static Dialogue _instance;

    public static Dialogue Instance { get => _instance; }

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
    [SerializeField]  private List<string> lines;
    [SerializeField] private float textSpeed;
    [SerializeField]  private int index;

    // Start is called before the first frame update
    void Start()
    {
        //textComponent.text = string.Empty;
        //StartDialogue();
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        Assert.IsNotNull(textComponent, "textComponent was null for Dialogue");
        if (lines.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }

    void StartDialogue() {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine() {
        foreach (char c in lines[index].ToCharArray()) {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    /// <summary>
    /// Clear the current set of dialogue lines and start a new one. 
    /// Also causes the dialogue box to become active if it was not already.
    /// </summary>
    /// <param name="dialogue"></param>
    public void DisplayDialogue(params string[] dialogue)
    {
        lines = new List<string>();
        foreach (string str in dialogue)
        {
            lines.Add(str);
        }
        gameObject.SetActive(true);
        textComponent.text = "";
        StartDialogue();
    }

    /// <summary>
    /// Advance to the next line of dialogue. Should typically be called by the Player script when the correct input is registered.
    /// </summary>
    public void NextLine() {

        // If the current line is completely typed
        if (textComponent.text == lines[index])
        {
            if (index < lines.Count - 1)
            {
                index++;
                textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
    }
}
