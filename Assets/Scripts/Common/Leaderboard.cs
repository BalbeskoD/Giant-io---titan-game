using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Zenject.Signals;

namespace Common
{
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] private Color playerColor = Color.red;
        [SerializeField] private Color otherColor = Color.black;
        [SerializeField] private Color imageColor = Color.black;
        [SerializeField] private LeaderboardItem prefab;
        private Dictionary<string, SortItem> _sortDictionary = new();
        private readonly Dictionary<string, SortItem> _dictionary = new();
        private SignalBus _signalBus;
        private Crown _currentCrown;

        private LeaderboardItem[] array = new LeaderboardItem[4];

        public Color PlayerColor => playerColor;

        public Color OtherColor => otherColor;

        public Color ImageColor => imageColor;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _signalBus.Subscribe<PointsChangeSignal>(OnPointsChange);
            _signalBus.Subscribe<GameRestartSignal>(OnRestart);
        }

        private void Start()
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Instantiate(prefab, transform);
                array[i].gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<PointsChangeSignal>(OnPointsChange);
            _signalBus.Unsubscribe<GameRestartSignal>(OnRestart);
        }

        private void OnRestart(GameRestartSignal signal)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].gameObject.SetActive(false);
                array[i].OnRestart();
            }
            _dictionary.Clear();
            _sortDictionary.Clear();
        }

        private void OnPointsChange(PointsChangeSignal signal)
        {
            if (!_dictionary.ContainsKey(signal.Name))
            {
                _dictionary.Add(signal.Name, signal.SortItem);
            }
            else
            {
                _dictionary[signal.Name] = signal.SortItem;
            }

            Sort();
        }

        private void Sort()
        {
            _sortDictionary = _dictionary.OrderByDescending(x => x.Value.Points).ToDictionary(x => x.Key, x => x.Value);
            UpdateView();
        }

        private void UpdateView()
        {
            var first = _sortDictionary.FirstOrDefault();
            if (_currentCrown != first.Value.Crown)
            {
                if (_currentCrown)
                {
                    _currentCrown.gameObject.SetActive(false);
                }

                _currentCrown = first.Value.Crown;
                _currentCrown.gameObject.SetActive(true);
            }

            for (int i = 0; i < array.Length; i++)
            {
                array[i].gameObject.SetActive(false);
            }

            var counter = 0;

            foreach (var valuePair in _sortDictionary)
            {
                bool isPlayer = valuePair.Key == "Player";

                if (counter < 3)
                {
                    array[counter].gameObject.SetActive(true);
                    array[counter].Setup(this, new LeaderboardItemData() {Position = (counter + 1).ToString(), Name = valuePair.Key, Points = valuePair.Value.Points.ToString()}, isPlayer);
                }

                if (counter >= 3 && isPlayer)
                {
                    array[3].gameObject.SetActive(true);
                    array[3].Setup(this, new LeaderboardItemData() {Position = (counter + 1).ToString(), Name = valuePair.Key, Points = valuePair.Value.Points.ToString()}, isPlayer);
                }


                counter++;
            }
        }
    }

    [Serializable]
    public class SortItem
    {
        public int Points;
        public Crown Crown;
    }
}