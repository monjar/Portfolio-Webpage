using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
   

    public List<GameObject> mobileUI;

    private void Start()
    {
        if (!Application.isMobilePlatform)
        {
            mobileUI.ForEach(ui =>
            {
                ui.SetActive(false);
            });
        }
    }
}