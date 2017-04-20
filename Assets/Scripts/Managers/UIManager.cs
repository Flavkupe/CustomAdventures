using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonObject<UIManager>
{
    public PlayerPanel PlayerPanel;

    public EntityPanel EntityPanel;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        if (this.PlayerPanel == null)
        {
            this.PlayerPanel = GameObject.FindObjectOfType<PlayerPanel>();
        }

        if (this.EntityPanel == null)
        {
            this.EntityPanel = GameObject.FindObjectOfType<EntityPanel>(); 
        }          
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.UpdateUI();
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
}
