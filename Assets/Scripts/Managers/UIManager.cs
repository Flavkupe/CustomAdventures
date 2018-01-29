using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonObject<UIManager>
{
    public PlayerPanel PlayerPanel;

    public EntityPanel EntityPanel;

    public InventoryPanel InventoryPanel;

    void Awake()
    {
        Instance = this;

        if (this.PlayerPanel == null)
        {
            this.PlayerPanel = GameObject.FindObjectOfType<PlayerPanel>();
        }

        if (this.EntityPanel == null)
        {
            this.EntityPanel = GameObject.FindObjectOfType<EntityPanel>();
        }

        if (this.InventoryPanel == null)
        {
            this.InventoryPanel = GameObject.FindObjectOfType<InventoryPanel>();
            this.InventoryPanel.gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        this.UpdateUI();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle inventory pane
            this.InventoryPanel.gameObject.SetActive(!this.InventoryPanel.gameObject.activeSelf);
            if (this.InventoryPanel.gameObject.activeSelf)
            {
                // Note: activeSelf doesn't activate until next frame, so this happens when the UI
                //  is actually shown.
                StartCoroutine(this.DoNextFrame(() =>
                {
                    this.UpdateInventory();
                }));
            }
        }
    }

    public void UpdateUI()
    {
        PlayerPanel.UpdatePanel();
        if (selectedEntity == null)
        {
            this.ToggleEntityPanel(false);
        }
    }

    private TileEntity selectedEntity = null;
    public void UpdateEntityPanel(Enemy enemy)
    {
        if (enemy != null)
        {
            selectedEntity = enemy;
            this.ToggleEntityPanel(true);
            EntityPanel.ShowEnemyData(enemy);
        }
    }

    public void ToggleEntityPanel(bool show)
    {
        this.EntityPanel.gameObject.SetActive(show);
    }

    public void UpdateInventory()
    {
        if (this.InventoryPanel.gameObject.activeSelf)
        {
            this.InventoryPanel.UpdateInventory();
        }
    }
}
