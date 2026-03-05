using UnityEngine;

public class BuildAPromtGen : MonoBehaviour
{
    public GameObject fly;
    public int Fly;
    public int Bee;
    public int Wasp;
    public GameObject bee;
    public GameObject wasp;
    public bool IsPromtSet = false;
    public int Promt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Fly = 1;
        Bee = 2;
        Wasp = 3;
        fly.SetActive(false);
        bee.SetActive(false);
        wasp.SetActive(false);
        Promt = Random.Range(1, 4);
        //DeBug.log(Promt);
        IsPromtSet = true;
        if(IsPromtSet && Promt == 1)
        {
            fly.SetActive(true);
        }
        if (IsPromtSet && Promt == 2)
        {
            bee.SetActive(true);
        }
        if (IsPromtSet && Promt == 3)
        {
            wasp.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
 