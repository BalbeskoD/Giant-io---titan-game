using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.Signals;
using Random = UnityEngine.Random;

namespace UI
{
    public class GamePanel : MonoBehaviour
    {
        private const int ARROW_COUNT = 6;

        //private static Vector3 CENTER = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject tutorial;
        private Enemy.Enemy[] _enemies;
        private Player.Player _player;
        private Transform[] _arrows;
        private Image[] _images;
        private SignalBus _signalBus;
        private bool _isGameActive;
        private Camera _camera;
        private Joystick _joystick;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _joystick = GetComponentInChildren<Joystick>();
            _camera = FindObjectOfType<Camera>();
            _signalBus.Subscribe<GameStartSignal>(OnGameStart);
            _signalBus.Subscribe<GameEndSignal>(OnGameEnd);
        }

        private void Start()
        {
            _arrows = new Transform[ARROW_COUNT];
            _images = new Image[ARROW_COUNT];
            for (int i = 0; i < ARROW_COUNT; i++)
            {
                _arrows[i] = Instantiate(arrowPrefab, transform).transform;
                _images[i] = _arrows[i].GetComponent<Image>();
            }
            _player = FindObjectOfType<Player.Player>();
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameStartSignal>(OnGameStart);
            _signalBus.Unsubscribe<GameEndSignal>(OnGameEnd);
        }

        private void OnEnable()
        {
            _enemies = FindObjectsOfType<Enemy.Enemy>();
            _player = FindObjectOfType<Player.Player>();
            tutorial.SetActive(true);
        }

        private void Update()
        {
            if (!_isGameActive)
                return;
            if (_joystick.Direction != Vector2.zero)
            {
                tutorial.SetActive(false);
            }

            SetupArrows();
        }

        private void OnGameStart()
        {
            _isGameActive = true;
        }

        private void OnGameEnd()
        {
            _isGameActive = false;
        }

        private void SetupArrows()
        {
            var sortArray = _enemies.OrderBy(x => Vector3.SqrMagnitude(_player.transform.position - x.transform.position)).ToArray();
            var positions = sortArray.Select(t => t.transform.position).ToArray();

            for (int i = 0; i < ARROW_COUNT; i++)
            {
                var pos = _camera.WorldToScreenPoint(positions[i]);

                pos.x = Mathf.Clamp(pos.x, 0f, Screen.width);
                pos.y = Mathf.Clamp(pos.y, 0f, Screen.height);
                pos.z = 0;
                _arrows[i].gameObject.SetActive(true);
                _arrows[i].position = pos;
                _images[i].color = sortArray[i].Color;
                if (pos.y > Screen.height - 1 && pos.x > Screen.width - 1)
                {
                    _arrows[i].rotation = Quaternion.Euler(new Vector3(0, 0, -45));
                }
                else if (pos.y > Screen.height - 1 && pos.x < 1)
                {
                    _arrows[i].rotation = Quaternion.Euler(new Vector3(0, 0, 45));
                }
                else if (pos.y > Screen.height - 1)
                {
                    _arrows[i].rotation = Quaternion.Euler(Vector3.zero);
                }
                else if (pos.y < 1 && pos.x > Screen.width - 1)
                {
                    _arrows[i].rotation = Quaternion.Euler(new Vector3(0, 0, -135));
                }
                else if (pos.y < 1 && pos.x < 1)
                {
                    _arrows[i].rotation = Quaternion.Euler(new Vector3(0, 0, 135));
                }
                else if (pos.y < 1)
                {
                    _arrows[i].rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                }
                else if (pos.x > Screen.width - 1)
                {
                    _arrows[i].rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                }
                else if (pos.x < 1)
                {
                    _arrows[i].rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }
                else
                {
                    _arrows[i].gameObject.SetActive(false);
                }
            }
        }
    }
}