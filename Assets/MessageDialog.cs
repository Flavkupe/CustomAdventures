using UnityEngine;
using TMPro;

public class MessageDialog : MonoBehaviour
{
    public GameObject Contents;

    public TextMeshProUGUI Text;

    private DialogSelection _selection;

    private DialogSelection Selection { get { return _selection; } }

    public void Show(string message)
    {
        Game.States.SetState(GameState.AwaitingDialogClose);
        Contents.SetActive(true);
        Text.text = message;
    }

    public void Close()
    {
        Contents.SetActive(false);
        Game.States.SetState(GameState.AwaitingCommand);
    }

    public void SelectionOK()
    {
        _selection = DialogSelection.OK;
        Close();
    }
}

public enum DialogSelection
{
    OK,
    Cancel,
}