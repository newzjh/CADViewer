using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepGroup : MonoBehaviour
{

    public static StepGroup Instance = null;
    private GameObject groupgo = null;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        groupgo = GameObject.Find("StepGroup");
    }


    private void OnEnable()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideAll()
    {
        for (int i = 0; i < groupgo.transform.childCount; i++)
        {
            groupgo.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Show(int index)
    {
        if (index >= 0 && index < groupgo.transform.childCount)
        {
            groupgo.transform.GetChild(index).gameObject.SetActive(true);
        }
    }

}
