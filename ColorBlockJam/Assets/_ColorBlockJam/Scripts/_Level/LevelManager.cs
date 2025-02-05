using System;
using System.Globalization;
using _ColorBlockJam.Scripts._SFX;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _ColorBlockJam.Scripts._Level
{
    public class LevelManager : MonoBehaviour
    {
        [Header("----- Time Settings----- ")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private float startTime = 60f;
        private float _timeLeft;

        [Header("----- Game Win & Lose Settings -----")] 
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        
        public GameObject[] blocks;
        public bool gameEnded = false;

        public static LevelManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _timeLeft = startTime;
            Timer().Forget();
            UpdateTimerText();
        }

        private async UniTask Timer()
        {
            while (_timeLeft > 0 && !gameEnded)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                _timeLeft -= 1f;
                UpdateTimerText();
                
                if (AllBlocksInactive())
                {
                    GameWon().Forget();
                    await UniTask.Yield();
                }
            }
            
            if (_timeLeft <= 0 && !gameEnded)
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.TimeFreeze);
                ShowLosePanel().Forget();
            }
        }
        
        private void UpdateTimerText()
        {
            int minutes = Mathf.FloorToInt(_timeLeft / 60f);
            int seconds = Mathf.FloorToInt(_timeLeft % 60f);
            timeText.text = $"{minutes}:{seconds:00}";
        }
        
        private bool AllBlocksInactive()
        {
            if (blocks != null)
            {
                foreach (GameObject block in blocks)
                {
                    if (!block)
                        continue;

                    if (block.activeSelf)
                        return false;
                }
            }
            return true;
        }

        private async UniTaskVoid GameWon()
        {
            if (gameEnded) return;
            gameEnded = true;
            
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            
            if (winPanel)
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.WinSound);
                winPanel.SetActive(true);
            }
        }

        private async UniTaskVoid ShowLosePanel()
        {
            if (gameEnded) return;
            gameEnded = true;
            
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            
            if (losePanel)
            {
                SoundManager.Instance.PlaySound(SoundManager.SoundType.LoseSound);
                losePanel.SetActive(true);
            }
        }
        
        public void LoadNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int totalScenes = SceneManager.sceneCountInBuildSettings;
        
            if (currentSceneIndex + 1 < totalScenes)
            {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
            else
            {
                SceneManager.LoadScene(currentSceneIndex);
            }
        }

        public void RetryScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}