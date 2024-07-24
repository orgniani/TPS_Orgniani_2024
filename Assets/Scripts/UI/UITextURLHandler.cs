using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITextURLHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text messageText;
    private string url = "https://lapiedranumerodos.itch.io/";

    private void Start()
    {
        messageText.text = "This level is currently unavailable! \n\n" +
                           "Stay tuned for the latest releases and updates by following me on <link=\"" + url + 
                           "\"><color=#3FA1FE><u>itch.io</u></color></link>";
        messageText.richText = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(messageText, Input.mousePosition, null);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = messageText.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
