using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;



public class SVGSelectButton : MonoBehaviour
{
    Unity.VectorGraphics.Scene svg;

    // Start is called before the first frame update
    void Start()
    {
        svg = LoadSVG(name);
        var pair = BuildSprite(svg);
        var img = GetComponentInChildren<SVGImage>(true);
        if (img != null)
        {
            img.sprite = pair.Key;
            var rt = img.GetComponent<RectTransform>();
            var rc = pair.Value;
            rt.sizeDelta = rc.size * 80;
            Vector2 center = rc.center;
            center.y = -center.y;
            rt.localPosition = center * 80;
        }
        var b = GetComponentInChildren<Button>();
        b.onClick.AddListener(OnClick);
    }

    KeyValuePair<Sprite, Rect> BuildSprite(Unity.VectorGraphics.Scene svg)
    {
        var tessOptions = new VectorUtils.TessellationOptions()
        {
            StepDistance = 100.0f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        var geoms = VectorUtils.TessellateScene(svg, tessOptions);


        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(-float.MaxValue, -float.MaxValue);
        foreach (var geo in geoms)
        {
            foreach (var v in geo.Vertices)
            {
                Vector2 vv = geo.WorldTransform * v;
                min = Vector2.Min(min, vv);
                max = Vector2.Max(max, vv);
            }
        }

        Rect rc = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

        // Build a sprite with the tessellated geometry.
        var sprite = VectorUtils.BuildSprite(geoms, 10.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
        
        return new KeyValuePair<Sprite, Rect>(sprite, rc);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick()
    {
        var panel = GetComponentInParent<SingleViewPanel>();
        if (panel != null)
        {
            panel.SetSVG(svg);
            ShapeGroup.Instance.DestroyAll();
            ShapeGroup.Instance.GenerateFromSVGs();

            var sr = panel.GetComponentInChildren<ScrollRect>();
            if (sr!=null)
            {
                sr.gameObject.SetActive(false);
            }
        }


    }

    private void modifySVG(SceneNode node)
    {
        if (node.Shapes != null)
        {
            foreach (var shape in node.Shapes)
            {
                if (shape.PathProps.Stroke != null)
                    shape.PathProps.Stroke.HalfThickness *= 0.01f;
            }
        }

        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                modifySVG(child);
            }
        }
    }

    
    private Unity.VectorGraphics.Scene LoadSVG(string name)
    {
        string path1 = name;
        TextAsset ta = Resources.Load<TextAsset>("svgs/" + path1);

        if (ta != null)
        {
            // Dynamically import the SVG data, and tessellate the resulting vector scene.
            var sceneInfo = SVGParser.ImportSVG(new System.IO.StringReader(ta.text));
            foreach (var child in sceneInfo.Scene.Root.Children)
            {
                modifySVG(child);
            }
            return sceneInfo.Scene;
        }

        return null;
    }

  
}
