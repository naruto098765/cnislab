using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
public class RoomButton : MonoBehaviour
{

    public TMP_Text buttonText;

    private RoomInfo info;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void SetButtonDetails(RoomInfo inputInfo)
    {
        info = inputInfo;

        buttonText.text = info.Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
