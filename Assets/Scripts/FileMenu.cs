using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FileMenu))]
class FileMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FileMenu com = (FileMenu)target;

        if (GUILayout.Button("EnumShapes"))
        {
            var dd = com.GetComponent<Dropdown>();
            dd.options.Clear();
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = "File";
                dd.options.Add(data);
            }

            string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Resources/shapes", "*.json");
            if (files!=null)
            {
                foreach(string file in files)
                {
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    var filename = System.IO.Path.GetFileNameWithoutExtension(file);
                    data.text = filename;
                    dd.options.Add(data);
                }
            }
        }
    }
}
#endif

public class FileMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFileMenu()
    {
        var dd = GetComponent<Dropdown>();
        int sel = dd.value;
        if (sel>0)
        {
            string name = dd.options[sel].text;
            StepGroup.Instance.HideAll();
            ShapeGroup.Instance.DestroyAll();
            ShapeGroup.Instance.Load(name);
            ViewPanelController.Instance.RestoreFromCameraViews();
        }
    }

}
