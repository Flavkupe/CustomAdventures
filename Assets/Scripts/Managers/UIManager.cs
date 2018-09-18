using Assets.Scripts.UI;
using Assets.Scripts.UI.State;
using JetBrains.Annotations;
using UnityEngine;

public class UIManager : SingletonObject<UIManager>
{
    public PlayerPanel PlayerPanel;

    public EntityPanel EntityPanel;

    public InventoryPanel InventoryPanel;

    public MulliganPanel MulliganPanel;

    public MessageDialog MessageDialog;

    public UIPrefabTemplates Templates;

    public Canvas MainCanvas;

    private UIEvent? _currentUIEvent;

    public UIStateController UIStateController { get; } = new UIStateController();

    private GameContext GameContext => Game.Dungeon.GetGameContext();

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;

        if (PlayerPanel == null)
        {
            PlayerPanel = FindObjectOfType<PlayerPanel>();
        }

        if (EntityPanel == null)
        {
            EntityPanel = FindObjectOfType<EntityPanel>();
        }

        if (InventoryPanel == null)
        {
            InventoryPanel = FindObjectOfType<InventoryPanel>();
            InventoryPanel.gameObject.SetActive(false);
        }

        if (MulliganPanel == null)
        {
            MulliganPanel = FindObjectOfType<MulliganPanel>();
            MulliganPanel.gameObject.SetActive(false);
        }

        if (MessageDialog == null)
        {
            MessageDialog = FindObjectOfType<MessageDialog>();
            MessageDialog.gameObject.SetActive(false);
        }
    }

    [UsedImplicitly]
    private void Start ()
    {
        UpdateEntityPanels();
        UIStateController.Start();
    }

    [UsedImplicitly]
    private void LateUpdate()
    {
        _currentUIEvent = null;
    }

    [UsedImplicitly]
    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TryToggleInventoryMenu();
        }
    }

    private void TryToggleInventoryMenu()
    {
        if (IsMenuActive || UIStateController.CanPerformAction(DungeonActionType.OpenMenu))
        {
            // Toggle inventory pane
            var uiEvent = IsMenuActive ? UIEventType.InterfaceClosed : UIEventType.InterfaceOpened;
            UIStateController.HandleNewEvent(uiEvent, GameContext);
            InventoryPanel.gameObject.SetActive(!InventoryPanel.gameObject.activeSelf);
            if (IsMenuActive)
            {
                // Note: activeSelf doesn't activate until next frame, so this happens when the UI
                //  is actually shown.
                StartCoroutine(this.DoNextFrame(UpdateInventory));
            }
        }
    }

    public void ShowDialog(string message)
    {
        MessageDialog.Show(message);
        UIStateController.HandleNewEvent(UIEventType.DialogShown, GameContext);
    }

    public void CloseDialog()
    {
        MessageDialog.Close();
        UIStateController.HandleNewEvent(UIEventType.DialogClosed, GameContext);
    }

    public void UpdateEntityPanels()
    {
        PlayerPanel.UpdatePanel();
        if (selectedEntity == null)
        {
            ToggleEntityPanel(false);
        }
    }

    public bool IsMenuActive => InventoryPanel.gameObject.activeSelf;

    private TileEntity selectedEntity;

    public void UpdateEntityPanel(Enemy enemy)
    {
        if (enemy != null)
        {
            selectedEntity = enemy;
            ToggleEntityPanel(true);
            EntityPanel.ShowEnemyData(enemy);
        }
    }

    public void ToggleEntityPanel(bool show)
    {
        EntityPanel.gameObject.SetActive(show);
    }

    public void ToggleMulliganPanel(bool show)
    {
        MulliganPanel.gameObject.SetActive(show);
    }

    public void UpdateInventory()
    {
        if (InventoryPanel.gameObject.activeSelf)
        {
            InventoryPanel.UpdateInventory();
        }
    }

    public UIEvent? GetCurrentUIEvent()
    {
        return _currentUIEvent;
    }

    public void SetCurrentUIEvent(UIEvent? uiEvent)
    {
        _currentUIEvent = uiEvent;
    }
}

public enum UIEvent
{
    MulliganPressed,
    TakePressed,
}
