using UnityEngine;
using UnityEngine.EventSystems;

public class CheckEventSystem : MonoBehaviour
{
	void Awake ()
    {
        EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();

        if (sceneEventSystem == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            DontDestroyOnLoad(eventSystem);
        }
    }
}
