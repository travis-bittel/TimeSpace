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
    [SerializeField]  private string[] lines;
    [SerializeField] private float textSpeed;
    [SerializeField]  private int index;
    [SerializeField] private bool _dialogueActive;
    /// <summary>
    /// We perform each of these actions when the current dialogue ends. These are passed in to DisplayDialogue().
    /// </summary>
    [SerializeField] private DialogueEndAction[] endActions;
    public bool DialogueActive { get => _dialogueActive; }

    // Start is called before the first frame update
    void Start()
    {
        //textComponent.text = string.Empty;
        //StartDialogue();
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        Assert.IsNotNull(textComponent, "textComponent was null for Dialogue");
        if (lines.Length == 0)
        {
            gameObject.SetActive(false);
            _dialogueActive = false;
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
        DisplayDialogue(null, dialogue);
    }

    /// <summary>
    /// Clear the current set of dialogue lines and start a new one. 
    /// Also causes the dialogue box to become active if it was not already.
    /// </summary>
    /// <param name="endActions"></param>
    /// <param name="dialogue"></param>
    public void DisplayDialogue(DialogueEndAction[] endActions, params string[] dialogue)
    {
        lines = new string[dialogue.Length];
        dialogue.CopyTo(lines, 0);

        if (endActions != null)
        {
            this.endActions = new DialogueEndAction[endActions.Length];
            endActions.CopyTo(this.endActions, 0);
        }

        gameObject.SetActive(true);
        textComponent.text = "";
        _dialogueActive = true;
        StartDialogue();
    }

    /// <summary>
    /// Advance to the next line of dialogue. Should typically be called by the Player script when the correct input is registered.
    /// </summary>
    public void NextLine()
    {
        if (_dialogueActive && index < lines.Length)
        {
            // If the current line is completely typed
            if (textComponent.text == lines[index])
            {
                if (index < lines.Length - 1)
                {
                    index++;
                    textComponent.text = string.Empty;
                    StartCoroutine(TypeLine());
                }
                else
                {
                    // End Dialogue
                    foreach (DialogueEndAction action in endActions)
                    {
                        if (action != null)
                        {
                            action.gameObject.SetActive(action.active);
                        }
                    }
                    gameObject.SetActive(false);
                    _dialogueActive = false;
                }
            }
            else
            {
                // Fully type the line immediately
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }
}

/// <summary>
/// Right now this only allows us to enable/disable objects. More might be added in the future.
/// </summary>
[System.Serializable]
public class DialogueEndAction
{
    public GameObject gameObject;
    public bool active;
}
