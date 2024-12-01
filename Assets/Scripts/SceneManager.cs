using UnityEngine;

public class SceneManager : MonoBehaviour {
    public void Reload() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}