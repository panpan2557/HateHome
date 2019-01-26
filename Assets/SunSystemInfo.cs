using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSystemInfo : MonoBehaviour {

    public GameObject building;
    public GameObject sky;
    public GameObject sun;
    public GameObject directLight;
    public int numberOfBgs =3;
    public List<GameObject> skys;
    public List<GameObject> buildings;
    public float skysSpeed;
    public float buildingsSpeed;
    public GameObject skysParent;
    public GameObject buildingsParent;
    public GameObject detectPoint;
    // Use this for initialization
    void Start () {
        GameObject bg;
        buildings = new List<GameObject>();
        for (int i = 0; i < numberOfBgs; i++)
        {
            bg = Instantiate(building);
            bg.transform.parent = skysParent.transform;
            Vector3 newPos = this.transform.position;
            newPos.x += bg.GetComponent<SpriteRenderer>().bounds.size.x * i -10;
            newPos.y = -0.7f;
            bg.transform.position = newPos;
            buildings.Add(bg);
        }

        skys = new List<GameObject>();
        for (int i = 0; i < numberOfBgs; i++)
        {
            bg = Instantiate(sky);
            bg.transform.parent = buildingsParent.transform;
            Vector3 newPos = this.transform.position;
            newPos.x += bg.GetComponent<SpriteRenderer>().bounds.size.x * i -10;
            newPos.y = -0.5f;
            bg.transform.position = newPos;
            skys.Add(bg);
        }

    }

    public void MoveBackground()
    {
        foreach (GameObject bg in skys)
        {
            Vector3 pos = bg.transform.position;
            pos.x -= (skysSpeed * Time.deltaTime);
            bg.transform.position = pos;
        }

        foreach (GameObject bg in buildings)
        {
            Vector3 pos = bg.transform.position;
            pos.x -= (buildingsSpeed * Time.deltaTime);
            bg.transform.position = pos;
        }
    }

    public void DetectBackground(List<GameObject> list)
    {
        GameObject bg = list[0]; // 1st bg only collides with the detect point
        if (isCollidedWithDetectPoint(bg))
        {
            // prepare a position of the last bg
            Vector3 lastBgPos = list[numberOfBgs - 1].transform.position;
            lastBgPos.x += bg.GetComponent<SpriteRenderer>().bounds.size.x;
            bg.transform.position = lastBgPos;
            SwapBackground(list);
        }
    }

    bool isCollidedWithDetectPoint(GameObject bg)
    {
        return bg.transform.position.x + bg.GetComponent<SpriteRenderer>().bounds.size.x < detectPoint.transform.position.x;
    }

    void SwapBackground(List<GameObject> list)
    {
        GameObject firstBg = list[0];
        list.RemoveAt(0);
        list.Add(firstBg);
    }
    // Update is called once per frame
    void Update () {
        MoveBackground();
        DetectBackground(skys);
        DetectBackground(buildings);
    }
}
