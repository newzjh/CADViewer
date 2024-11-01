using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
public class ShapeGroup : MonoBehaviour
{

    public static ShapeGroup Instance = null;
    public Material mat;
    private GameObject groupgo = null;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        groupgo = GameObject.Find("ShapeGroup");
        InitSTL();
    }

    private void OnDestroy()
    {
        entries.Clear();
        entries = null;

        root = null;
    }

    string[] keys;
    Dictionary<string, ZipEntry> entries;
    string root;

    private void InitSTL()
    {
        TextAsset ta = Resources.Load<TextAsset>("stls.zip");
        if (ta == null)
            return;
        MemoryStream zipms = new MemoryStream(ta.bytes);

        entries = new Dictionary<string, ZipEntry>();
        ZipInputStream zis = new ZipInputStream(zipms);
        root = null;

        ZipEntry zipEntry;
        while ((zipEntry = zis.GetNextEntry()) != null)
        {
            string key = zipEntry.Name;
            if (zipEntry.IsDirectory)
            {
                if (key.EndsWith("/"))
                    key = key.Substring(0, key.Length - 1);
            }

            entries[key] = zipEntry;

            if (root == null)
                root = key;

        }

        zis.Close();
        zis.Dispose();
        zipms.Dispose();
        zipms = null;

        keys = new string[entries.Keys.Count];
        entries.Keys.CopyTo(keys, 0);
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = keys[i].Replace("mesh/", "");
            keys[i] = keys[i].Replace(".stl", "");
        }
    }

    private void OnEnable()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyAll()
    {
        List<GameObject> deletelist = new List<GameObject>();
        for (int i = 0; i < groupgo.transform.childCount; i++)
        {
            deletelist.Add(groupgo.transform.GetChild(i).gameObject);
        }
        foreach(var go in deletelist)
        {
            GameObject.DestroyImmediate(go);
        }
    }

    public struct ShapeProgram
    {
        public string name;
        public string id;
        public List<double[]> planks;
        public List<double[]> normal;
        public List<int[]> attach;
    };

    public struct ShapeUnity
    {
        public string name;
        public string id;
        public List<Bounds> planks;
        public List<Vector3> normals;
        public List<int[]> attach;
    }

    public static ShapeUnity ShapeProgramToUnity(ShapeProgram sp)
    {
        ShapeUnity su = new ShapeUnity();
        su.name = sp.name;
        su.id = sp.id;
        su.normals = new List<Vector3>();
        su.planks = new List<Bounds>();
        foreach (var b in sp.planks)
        {
            Bounds bb = new Bounds();
            Vector3 bbmin = Vector3.zero;
            bbmin.x = (float)b[0];
            bbmin.y = (float)b[1];
            bbmin.z = (float)b[2];
            Vector3 bbmax = Vector3.zero;
            bbmax.x = (float)b[3];
            bbmax.y = (float)b[4];
            bbmax.z = (float)b[5];
            bb.SetMinMax(bbmin, bbmax);
            su.planks.Add(bb);
        }
        su.normals = new List<Vector3>();
        foreach (var n in sp.normal)
        {
            Vector3 nn = Vector3.zero;
            nn.x = (float)n[0];
            nn.y = (float)n[1];
            nn.z = (float)n[2];
            su.normals.Add(nn);
        }
        su.attach = sp.attach;
        return su;
    }

    private void modifySVG(SceneNode node)
    {
        if (node.Shapes!=null)
        {
            foreach (var shape in node.Shapes)
            {
                if (shape.PathProps.Stroke != null)
                    shape.PathProps.Stroke.HalfThickness *= 0.01f;
            }
        }

        if (node.Children!=null)
        {
            foreach (var child in node.Children)
            {
                modifySVG(child);
            }
        }
    }

    private Scene LoadSVG(string name,string view)
    {
#if UNITY_EDITOR
        string path2 = "E:/Projects/PlankAssembly/data/data/complete/svgs/" + name + "_"+view+".svg";
        string path3 = Application.dataPath + "/Resources/svgs/" + name + "_" + view + ".bytes";
        if (System.IO.File.Exists(path2) && !System.IO.File.Exists(path3))
        {
            System.IO.File.Copy(path2, path3);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif

        string path1 = name + "_" + view;
        TextAsset ta = Resources.Load<TextAsset>("svgs/" + path1);

        if (ta != null)
        {
            // Dynamically import the SVG data, and tessellate the resulting vector scene.
            var sceneInfo = SVGParser.ImportSVG(new System.IO.StringReader(ta.text));
            foreach(var child in sceneInfo.Scene.Root.Children)
            {
                modifySVG(child);
            }
            return sceneInfo.Scene;
        }

        return null;
    }

    private Mesh LoadSVG2(string name, string view)
    {
#if UNITY_EDITOR
        string path2 = "E:/Projects/PlankAssembly/data/data/complete/svgs/" + name + "_" + view + ".svg";
        string path3 = Application.dataPath + "/Resources/svgs/" + name + "_" + view + ".bytes";
        if (System.IO.File.Exists(path2) && !System.IO.File.Exists(path3))
        {
            System.IO.File.Copy(path2, path3);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif

        string path1 = name + "_" + view;
        TextAsset ta = Resources.Load<TextAsset>("svgs/" + path1);

        if (ta != null)
        {
            var tessOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = 100.0f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };


            // Dynamically import the SVG data, and tessellate the resulting vector scene.
            var sceneInfo = SVGParser.ImportSVG(new System.IO.StringReader(ta.text));
            foreach (var child in sceneInfo.Scene.Root.Children)
            {
                modifySVG(child);
            }
            var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);

            Mesh mesh = new Mesh();
            VectorUtils.FillMesh(mesh, geoms, 1.0f);
            return mesh;
        }

        return null;
    }

    private MemoryStream extractEntryFromZip(string name)
    {
        string key = "mesh/" + name + ".stl";
        if (entries.ContainsKey(key))
        {
            TextAsset ta = Resources.Load<TextAsset>("stls.zip");
            if (ta == null)
                return null;
            MemoryStream zipms = new MemoryStream(ta.bytes);

            ZipInputStream zis = new ZipInputStream(zipms);

            MemoryStream ms = null;

            ZipEntry zipEntry;
            while ((zipEntry = zis.GetNextEntry()) != null)
            {
                if (zipEntry.IsFile)
                {
                    if (zipEntry.Name == key)
                    {
                        ms = new MemoryStream();
                        int size = 2048;
                        byte[] bytes = new byte[2048];
                        while (true)
                        {
                            size = zis.Read(bytes, 0, bytes.Length);
                            if (size > 0)
                            {
                                ms.Write(bytes, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        ms.Position = 0;
                    }
                }
            }

            ms.Seek(0, SeekOrigin.Begin);
            zis.Dispose();
            zipms.Dispose();
            return ms;
        }

        return null;
    }

    public Mesh LoadSTL(string name)
    {
        MemoryStream ms = extractEntryFromZip(name);
        if (ms!=null)
        {
            BinaryReader _binaryReader = new BinaryReader(ms);
            //Å×ÆúÇ°84¸ö×Ö½Ú
            _binaryReader.ReadBytes(80);
            int _total = _binaryReader.ReadInt32() * 3;

            List<Vector3> _vertices = new List<Vector3>();
            List<int> _triangles = new List<int>();
            int _number = 0;

            while (_number < _total)
            {
                byte[] bytes;
                bytes = _binaryReader.ReadBytes(50);

                if (bytes.Length < 50)
                {
                    _number += 1;
                    continue;
                }

                Vector3 vec1 = new Vector3(BitConverter.ToSingle(bytes, 12), BitConverter.ToSingle(bytes, 16), BitConverter.ToSingle(bytes, 20));
                Vector3 vec2 = new Vector3(BitConverter.ToSingle(bytes, 24), BitConverter.ToSingle(bytes, 28), BitConverter.ToSingle(bytes, 32));
                Vector3 vec3 = new Vector3(BitConverter.ToSingle(bytes, 36), BitConverter.ToSingle(bytes, 40), BitConverter.ToSingle(bytes, 44));

                int tri1 = _vertices.FindIndex(delegate (Vector3 v) { return v == vec1; });
                if (tri1 < 0)
                {
                    _vertices.Add(vec1);
                    tri1 = _vertices.Count - 1;
                }
                int tri2 = _vertices.FindIndex(delegate (Vector3 v) { return v == vec2; });
                if (tri2 < 0)
                {
                    _vertices.Add(vec2);
                    tri2 = _vertices.Count - 1;
                }
                int tri3 = _vertices.FindIndex(delegate (Vector3 v) { return v == vec3; });
                if (tri3 < 0)
                {
                    _vertices.Add(vec3);
                    tri3 = _vertices.Count - 1;
                }

                _triangles.Add(tri1);
                _triangles.Add(tri2);
                _triangles.Add(tri3);

                _number += 1;
            }

            Mesh m = new Mesh();
            m.vertices = _vertices.ToArray();
            m.triangles = _triangles.ToArray();
            m.RecalculateNormals();

            ms.Dispose();

            return m;
        }

        return null;
    }

    public Texture fsvgtex;
    public Texture2D tsvgtex;
    public Texture2D lsvgtex;

    public void GenerateFromSVGs()
    {
        int index = UnityEngine.Random.Range(0, keys.Length - 1);
        var key = keys[index];
        Mesh m = LoadSTL(key);
        GameObject sugo = new GameObject();
        sugo.name = name;
        sugo.transform.parent = groupgo.transform;
        sugo.transform.localScale = Vector3.one;
        sugo.transform.localPosition = Vector3.zero;
        sugo.layer = LayerMask.NameToLayer("Orth");
        sugo.AddComponent<MeshFilter>().mesh = m;
        sugo.AddComponent<MeshRenderer>().sharedMaterial = mat;
    }

    public void Load(string name)
    {
        //TextAsset ta = Resources.Load<TextAsset>("shapes/" + name);
        //if (ta != null)
        //{
        //    ShapeProgram sp = LitJson.JsonMapper.ToObject<ShapeProgram>(ta.text);
        //    var su = ShapeProgramToUnity(sp);
        //    GameObject sugo = new GameObject();
        //    sugo.name = name;
        //    sugo.transform.parent = groupgo.transform;
        //    sugo.transform.localScale = Vector3.one;
        //    sugo.transform.localPosition = Vector3.zero;
        //    sugo.layer = LayerMask.NameToLayer("Orth");
        //    float maxscale = 1.0f;
        //    foreach (var b in su.planks)
        //    {
        //        GameObject subgo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //        subgo.transform.parent = sugo.transform;
        //        subgo.transform.localScale = b.extents;
        //        subgo.transform.localPosition = b.center;
        //        subgo.layer = LayerMask.NameToLayer("Orth");
        //        subgo.GetComponent<MeshRenderer>().sharedMaterial = mat;
        //        if (b.extents.x > maxscale)
        //            maxscale = b.extents.x;
        //        if (b.extents.y > maxscale)
        //            maxscale = b.extents.y;
        //        if (b.extents.z > maxscale)
        //            maxscale = b.extents.z;
        //    }
        //    sugo.transform.localScale = Vector3.one * 1.0f / maxscale;
        //}

        var fsvg = LoadSVG(name, "f");
        if (fsvg != null)
            ViewPanelController.Instance.frontView.GetComponentInChildren<SingleViewPanel>(true).SetSVG(fsvg);

        var lsvg = LoadSVG(name, "s");
        if (lsvg != null)
            ViewPanelController.Instance.leftView.GetComponentInChildren<SingleViewPanel>(true).SetSVG(lsvg);

        var tsvg = LoadSVG(name, "t");
        if (tsvg != null)
            ViewPanelController.Instance.topView.GetComponentInChildren<SingleViewPanel>(true).SetSVG(tsvg);

        ViewPanelController.Instance.ShowSVG(true);


        GenerateFromSVGs();
    }

}
