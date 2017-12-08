using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {


    [SerializeField]
    bool singleton;

    static GameObject singletonGO;

    void Awake() {

        if (singleton && singletonGO)
        {
            Destroy(gameObject);
            return;
        }

        singletonGO = gameObject;

        DontDestroyOnLoad(gameObject);
    
        
            
	}
}
