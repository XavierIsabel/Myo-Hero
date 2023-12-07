using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _classes = 3;
    public GameObject _ringPrefab;
    private List<GameObject> _rings = new List<GameObject>();
    private List<Material> _colors = new List<Material>();
    // Start is called before the first frame update
    void Awake()
    {
        GeneratePlayArea();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if game is still going
    }

    void GeneratePlayArea() {
        for (int i=0;i<_classes;i++) {
            GameObject _ring = Instantiate (_ringPrefab, new Vector3(i,-4, 0), Quaternion.identity);
            //_ring.Find("Circle").SpriteRenderer.Color = _colors[i];
            _rings.Add(_ring);
        }
    }
}
