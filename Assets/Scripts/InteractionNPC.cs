using UnityEngine;
using TMPro;   // Only if you use TextMeshPro

public class NPCInteractionPrompt : MonoBehaviour
{
    [Header("References")]
    public Transform player;            // Drag your Player here
    public GameObject interactUI;       // "Press E" prompt
    public GameObject dialoguePanel;    // Dialogue panel UI
    public TextMeshProUGUI dialogueText; // Text inside the panel

    [Header("Distance Settings")]
    public float tileSize = 1f;
    public float rangeInTiles = 2f;

    [Header("Dialogue Lines")]
    [TextArea] 
    public string[] dialogueLines;      // Fill these in in Inspector

    private bool inRange;
    private bool isTalking;
    private int currentLineIndex;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        // ----- RANGE / PROMPT LOGIC -----
        float maxDistance = rangeInTiles * tileSize;
        float distance = Vector2.Distance(player.position, transform.position);
        bool shouldBeVisible = distance <= maxDistance;

        if (interactUI != null && shouldBeVisible != inRange && !isTalking)
        {
            inRange = shouldBeVisible;
            interactUI.SetActive(inRange);
        }

        // ----- INPUT / DIALOGUE LOGIC -----
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            else
            {
                AdvanceDialogue();
            }
        }
    }

    void StartDialogue()
    {
        if (dialogueLines.Length == 0 || dialoguePanel == null || dialogueText == null)
            return;

        isTalking = true;
        currentLineIndex = 0;

        interactUI.SetActive(false);         // Hide "Press E"
        dialoguePanel.SetActive(true);       // Show dialogue box
        dialogueText.text = dialogueLines[currentLineIndex];
    }

    void AdvanceDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
        }
        else
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }
    }

    void EndDialogue()
    {
        isTalking = false;
        dialoguePanel.SetActive(false);

        // Show prompt again if still in range
        if (inRange && interactUI != null)
            interactUI.SetActive(true);
    }
}
