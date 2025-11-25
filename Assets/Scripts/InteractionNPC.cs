using UnityEngine;
using TMPro;

public class NPCDialogueStateMachine : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject interactUI;
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcText;
    public TextMeshProUGUI optionsText;

    [Header("Distance Settings")]
    public float tileSize = 1f;
    public float rangeInTiles = 2f;

    [Header("Dialogue Lines (Editable in Inspector)")]
    [TextArea] public string firstGreetingLine;
    [TextArea] public string greetingLine;
    [TextArea] public string whoAreYouAnswer;
    [TextArea] public string exitAnswer;
    [TextArea] public string shopAnswer;

    private bool isTalking;
    private bool inRange;
    private bool hasUnlockedShopQuestion;
    private bool hasGreetedPlayer;

    private enum DialogueState { None, NPCLine, PlayerChoice }
    private enum DialogueMode { None, Greeting, Who, Exit, Shop }

    private DialogueState state = DialogueState.None;
    private DialogueMode mode = DialogueMode.None;

    void Start()
    {
        interactUI.SetActive(false);
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        float maxDistance = rangeInTiles * tileSize;
        float distance = Vector2.Distance(player.position, transform.position);
        bool closeEnough = distance <= maxDistance;
        inRange = closeEnough;

        // End dialogue if player leaves range
        if (isTalking && !closeEnough)
            EndDialogue();

        // Show prompt only when in range and not talking
        interactUI.SetActive(inRange && !isTalking);

        // Start dialogue
        if (!isTalking && inRange && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
            return;
        }
            
        // Handle current mode
        if (!isTalking) return;

        if (state == DialogueState.NPCLine && Input.GetKeyDown(KeyCode.E))
            AdvanceNPCLine();
        else if (state == DialogueState.PlayerChoice)
            HandleChoiceInput();
    }

    /*void StartDialogue()
    {
        isTalking = true;
        state = DialogueState.NPCLine;
        mode = DialogueMode.Greeting;

        dialoguePanel.SetActive(true);
        ShowNPCLine(greetingLine);
    }*/
    void StartDialogue()
    {
        isTalking = true;
        state = DialogueState.NPCLine;
        mode = DialogueMode.Greeting;

        dialoguePanel.SetActive(true);

        // First time talking to this NPC?
        if (!hasGreetedPlayer && !string.IsNullOrEmpty(firstGreetingLine))
        {
            hasGreetedPlayer = true;
            ShowNPCLine(firstGreetingLine);
        }
        else
        {
            ShowNPCLine(greetingLine);
        }
    }
    void EndDialogue()
    {
        isTalking = false;
        state = DialogueState.None;
        mode = DialogueMode.None;

        dialoguePanel.SetActive(false);
    }

    /*void ShowNPCLine(string line)
    {
        state = DialogueState.NPCLine;

        npcText.gameObject.SetActive(true);
        optionsText.gameObject.SetActive(false);

        npcText.text = line;
    }*/
    void ShowNPCLine(string line)
    {
        state = DialogueState.NPCLine;

        npcText.gameObject.SetActive(true);
        optionsText.gameObject.SetActive(false);

        // Make sure main dialogue is top-left (nice for reading)
        npcText.alignment = TextAlignmentOptions.TopLeft;

        npcText.text =
            line +
            "\n\n<align=\"right\"><color=#000000><size=80%>Press 'E' to continue</size></color></align>";
    }

    void ShowPlayerChoices()
    {
        state = DialogueState.PlayerChoice;

        npcText.gameObject.SetActive(false);
        optionsText.gameObject.SetActive(true);

        string choices = "1) Who are you?\n" +
                         "2) How do I get out of this place?";

        if (hasUnlockedShopQuestion)
            choices += "\n3) What do you sell?";

        //choices += "\n\n(Press 1, 2, or 3 to choose.)";

        optionsText.text = choices;
    }

    void AdvanceNPCLine()
    {
        switch (mode)
        {
            case DialogueMode.Greeting:
                ShowPlayerChoices();
                break;

            case DialogueMode.Who:
            case DialogueMode.Exit:
            case DialogueMode.Shop:
                ShowPlayerChoices();
                break;

            default:
                ShowPlayerChoices();
                break;
        }
    }

    void HandleChoiceInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnWhoAreYou();
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            OnExit();
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            OnShop();
    }

    // ---- CHOICE RESPONSES ------------------

    void OnWhoAreYou()
    {
        hasUnlockedShopQuestion = true;
        mode = DialogueMode.Who;

        ShowNPCLine(whoAreYouAnswer);
    }

    void OnExit()
    {
        mode = DialogueMode.Exit;
        ShowNPCLine(exitAnswer);
    }

    void OnShop()
    {
        if (!hasUnlockedShopQuestion) return;

        mode = DialogueMode.Shop;
        ShowNPCLine(shopAnswer);
    }
}
