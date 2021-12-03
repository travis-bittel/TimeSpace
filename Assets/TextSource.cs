using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to a GameObject to trigger the appearance of pop-up text or dialogue when the player moves into its collider.  
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TextSource : MonoBehaviour
{
    [Tooltip("What happens when the player enters the collider. Popup text is displayed below the player; dialogue text halts gameplay.")]
    [SerializeField] private Types type;

    [Tooltip("Text to display. If of type Dialogue, each entry is one line. If of type Popup, all entries after the first are ignored.")]
    [SerializeField] private string[] text;

    [Tooltip("Whether to \"type\" the popup text line, meaning show additional characters of the line over time. Has no effect if of type dialogue.")]
    [SerializeField] private bool typePopupTextLine;

    [SerializeField] private float cooldownPeriod;

    private float currentCooldown;

    private void Start()
    {
        currentCooldown = cooldownPeriod;
    }

    private void Update()
    {
        currentCooldown += Time.deltaTime;
        if (currentCooldown > cooldownPeriod)
        {
            currentCooldown = cooldownPeriod;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentCooldown >= cooldownPeriod && collision.gameObject.CompareTag("Player"))
        {
            switch (type)
            {
                case Types.Popup:
                    PopupTextHandler.Instance.UpdatePopupText(text[0], typePopupTextLine);
                    break;
                case Types.Dialogue:
                    Dialogue.Instance.DisplayDialogue(text);
                    break;
            }

            currentCooldown = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (type)
            {
                case Types.Popup:
                    PopupTextHandler.Instance.StopAllCoroutines();
                    PopupTextHandler.Instance.UpdatePopupText();
                    break;
            }
        }
    }

    private enum Types
    {
        Popup,
        Dialogue
    }
}
