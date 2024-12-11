using Steamworks.Data;
using TMPro;
using UnityEngine;

public class SteamController : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI name;
    void Awake()
    {
        DontDestroyOnLoad(this);
        
        try
        {
            Steamworks.SteamClient.Init( 2292030 );
            name.text = Steamworks.SteamClient.Name;

            var ach = new Achievement("First_Plant_1_0");
            ach.Trigger();
        }
        catch ( System.Exception e )
        {
            Debug.Log("Baglanamadi");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    private void OnDisable()
    {
        Steamworks.SteamClient.Shutdown();
    }
}
