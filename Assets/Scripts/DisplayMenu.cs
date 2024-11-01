using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDisplayMenu()
    {
        int sel = GetComponent<Dropdown>().value;
        Debug.Log("OnDisplayMenu:"+sel);

        if (sel==1)
        {
            ViewPanelController.Instance.RestoreFromCameraViews();
        }
        else if (sel==2)
        {
            
        }
    }

}
