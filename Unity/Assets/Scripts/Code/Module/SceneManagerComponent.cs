using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ETModel
{
    public class SceneManagerComponent: Component
    {
        private AsyncOperation prog;

        public async Task<bool> LoadSceneAsync(string sceneName)
        {
            prog = SceneManager.LoadSceneAsync(sceneName);

            return await LoadingScene(sceneName);
        }

        public async Task<bool> LoadingScene(string sceneName, bool allowSceneActivation = true)
        {
            prog.allowSceneActivation = allowSceneActivation;

            TimerComponent timerComponent = ETModel.Game.Scene.GetComponent<TimerComponent>();

            while (prog.progress < 0.9f) await timerComponent.WaitAsync(1);

            while (SceneManager.GetActiveScene().name != sceneName) await timerComponent.WaitAsync(1);

            prog.allowSceneActivation = true;

            return true;
        }
    }
}