using Assets.Scripts.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI.StatsOverlay
{
    public class ActionIcons : MonoBehaviour, IUpdatesWhen
    {
        private bool _showIcons;
        private int _actions;
        private int _moves;

        public GameObject MovementIcon;
        public GameObject ActionIcon;        

        public SimpleWorldEvent RequiredCondition => SimpleWorldEvent.PlayerStatsChange;

        public void ToggleIcons(bool show)
        {
            _showIcons = show;
            Refresh();
        }

        public void Updated()
        {
            Refresh();
        }

        private void AddIcon(GameObject template)
        {
            var icon = Instantiate(template);
            icon.transform.SetParent(transform);
            icon.transform.localScale = Vector3.one;
        }

        private void Refresh()
        {
            var stats = Game.Player.CurrentStats;
            foreach (Transform obj in transform)
            {
                Destroy(obj.gameObject);
            }

            if (_showIcons && (stats.FullActions != _actions || stats.FreeMoves != _moves))
            {
                for (var i = 0; i < stats.FreeMoves.Value; ++i)
                {
                    AddIcon(MovementIcon);
                }

                for (var i = 0; i < stats.FullActions.Value; ++i)
                {
                    AddIcon(ActionIcon);
                }

                _actions = stats.FullActions.Value;
                _moves = stats.FreeMoves.Value;
            }
            else
            {
                _actions = 0;
                _moves = 0;
            }
        }
    }
}
