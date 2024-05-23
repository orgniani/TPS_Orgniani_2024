using UnityEngine;
using TMPro;

public class UIHowToPlayMenu : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI instructionText;
    private string[] instructions = new string[]
    {
        "INSTRUCTION 1",
        "INSTRUCTION 2",
        "INSTRUCTION 3",
        "INSTRUCTION 4",
        "INSTRUCTION 5"
    };

    private int currentInstructionIndex = 0;

    private void OnEnable()
    {
        currentInstructionIndex = 0;
        UpdateInstructionText();
    }

    public void OnNextPage()
    {
        currentInstructionIndex = Mathf.Min(currentInstructionIndex + 1, instructions.Length - 1);
        UpdateInstructionText();
    }


    public void OnBackPage()
    {
        currentInstructionIndex = Mathf.Max(currentInstructionIndex - 1, 0);
        UpdateInstructionText();
    }

    private void UpdateInstructionText()
    {
        instructionText.text = instructions[currentInstructionIndex];
    }
}
