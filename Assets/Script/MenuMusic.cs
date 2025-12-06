using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    void Awake()
    {

        AkSoundEngine.LoadBank("MusicMenuBank", out uint musicbankID);

        AkSoundEngine.PostEvent("MusicMenu", gameObject);
    }


}
