using JetBrains.Annotations;
using UnityEngine;

public class UIManager : SingletonObject<UIManager>
{
    public PlayerPanel PlayerPanel;

    public EntityPanel EntityPanel;

    public InventoryPanel InventoryPanel;

    public MulliganPanel MulliganPanel;

    public MessageDialog MessageDialog;

    private UIEvent? _currentUIEvent;

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
        UpdateUI();
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
            // Toggle inventory pane
            InventoryPanel.gameObject.SetActive(!InventoryPanel.gameObject.activeSelf);
            if (InventoryPanel.gameObject.activeSelf)
            {
                // Note: activeSelf doesn't activate until next frame, so this happens when the UI
                //  is actually shown.
                StartCoroutine(this.DoNextFrame(UpdateInventory));
            }
        }
    }

    public void UpdateUI()
    {
        PlayerPanel.UpdatePanel();
        if (selectedEntity == null)
        {
            ToggleEntityPanel(false);
        }
    }

    public bool IsMenuActive { get { return InventoryPanel.gameObject.activeSelf; } }

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
