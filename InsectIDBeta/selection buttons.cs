using UnityEngine;

public class selectionbuttons : MonoBehaviour
{
    //Head vars
    public GameObject beeHead;
    public GameObject waspHead;
    public GameObject flyHead;
    private int BeeHead;
    private int WaspHead;
    private int FlyHead;
    private int WaspHead2;
    private int FlyHead2;
    private int partSlected;
    //wing vars
    public GameObject beeWings;
    public GameObject waspWings;
    public GameObject flyWings;
    private int BeeWings;
    private int WaspWings;
    private int FlyWings;
    private int WaspWings2;
    private int FlyWings2;
    private int partSlected1;
    // thorax vars
    public GameObject beeThorax;
    public GameObject waspThorax;
    public GameObject flyThorax;
    private int BeeThorax;
    private int WaspThorax;
    private int FlyThorax;
    private int WaspThorax2;
    private int FlyThorax2;
    private int partSlected2;
    //legs vars
    public GameObject beeLegs;
    public GameObject waspLegs;
    public GameObject flyLegs;
    private int BeeLegs;
    private int WaspLegs;
    private int FlyLegs;
    private int WaspLegs2;
    private int FlyLegs2;
    private int partSlected3;

    private void Start()
    {
        //Heads
        BeeHead = 0;
        WaspHead = 1;
        FlyHead = 2;
        WaspHead2 = -1;
        FlyHead2 = -2;
        partSlected = 0;
        //wings
        BeeWings = 0;
        WaspWings = 1;
        FlyWings = 2;
        WaspWings2 = -1;
        FlyWings2 = -2;
        partSlected1 = 0;
        //thorax
        BeeThorax = 0;
        WaspThorax = 1;
        FlyThorax = 2;
        WaspThorax2 = -1;
        FlyThorax2 = -2;
        partSlected2 = 0;
        //legs
        BeeLegs = 0;
        WaspLegs = 1;
        FlyLegs = 2;
        WaspLegs2 = -1;
        FlyLegs2 = -2;
        partSlected3 = 0;

    }
    // scroll buttons
    public void right()
    {
        partSlected++;
        if(partSlected == 3)
        {
            partSlected = 0;
        }
        if(partSlected == BeeHead)
        {
            beeHead.SetActive(true);
            waspHead.SetActive(false);
            flyHead.SetActive(false);
        }
        if (partSlected == WaspHead)
        {
            beeHead.SetActive(false);
            waspHead.SetActive(true);
            flyHead.SetActive(false);
        }
        if (partSlected == FlyHead)
        {
            beeHead.SetActive(false);
            waspHead.SetActive(false);
            flyHead.SetActive(true);
        }
    }
    public void left()
    {
        partSlected--;
        if (partSlected == -3)
        {
            partSlected = 0;
        }
        if (partSlected == BeeHead)
        {
            beeHead.SetActive(true);
            waspHead.SetActive(false);
            flyHead.SetActive(false);
        }
        if (partSlected == WaspHead2)
        {
            beeHead.SetActive(false);
            waspHead.SetActive(true);
            flyHead.SetActive(false);
        }
        if (partSlected == FlyHead2)
        {
            beeHead.SetActive(false);
            waspHead.SetActive(false);
            flyHead.SetActive(true);
        }
    }
         public void right1()
       {
        partSlected1++;
        if (partSlected1 == 3)
        {
            partSlected1 = 0;
        }
        if (partSlected1 == BeeWings)
        {
            beeWings.SetActive(true);
            waspWings.SetActive(false);
            flyWings.SetActive(false);
        }
        if (partSlected1 == WaspWings)
        {
            beeWings.SetActive(false);
            waspWings.SetActive(true);
            flyWings.SetActive(false);
        }
        if (partSlected1 == FlyWings)
        {
            beeWings.SetActive(false);
            waspWings.SetActive(false);
            flyWings.SetActive(true);
        }
       }
    public void left1()
    {
        partSlected1--;
        if (partSlected1 == -3)
        {
            partSlected1 = 0;
        }
        if (partSlected1 == BeeWings)
        {
            beeWings.SetActive(true);
            waspWings.SetActive(false);
            flyWings.SetActive(false);
        }
        if (partSlected1 == WaspWings2)
        {
            beeWings.SetActive(false);
            waspWings.SetActive(true);
            flyWings.SetActive(false);
        }
        if (partSlected1 == FlyWings2)
        {
            beeWings.SetActive(false);
            waspWings.SetActive(false);
            flyWings.SetActive(true);
        }
    }
    public void right2()
    {
        partSlected2++;
        if (partSlected2 == 3)
        {
            partSlected2 = 0;
        }
        if (partSlected2 == BeeThorax)
        {
            beeThorax.SetActive(true);
            waspThorax.SetActive(false);
            flyThorax.SetActive(false);
        }
        if (partSlected2 == WaspThorax)
        {
            beeThorax.SetActive(false);
            waspThorax.SetActive(true);
            flyThorax.SetActive(false);
        }
        if (partSlected2 == FlyThorax)
        {
            beeThorax.SetActive(false);
            waspThorax.SetActive(false);
            flyThorax.SetActive(true);
        }
    }
    public void left2()
    {
        partSlected2--;
        if (partSlected2 == -3)
        {
            partSlected2 = 0;
        }
        if (partSlected2 == BeeThorax)
        {
            beeThorax.SetActive(true);
            waspThorax.SetActive(false);
            flyThorax.SetActive(false);
        }
        if (partSlected2 == WaspThorax2)
        {
            beeThorax.SetActive(false);
            waspThorax.SetActive(true);
            flyThorax.SetActive(false);
        }
        if (partSlected2 == FlyThorax2)
        {
            beeThorax.SetActive(false);
            waspThorax.SetActive(false);
            flyThorax.SetActive(true);
        }
    }
    public void right3()
    {
        partSlected3++;
        if (partSlected3 == 3)
        {
            partSlected3 = 0;
        }
        if (partSlected3 == BeeLegs)
        {
            beeLegs.SetActive(true);
            waspLegs.SetActive(false);
            flyLegs.SetActive(false);
        }
        if (partSlected3 == WaspLegs)
        {
            beeLegs.SetActive(false);
            waspLegs.SetActive(true);
            flyLegs.SetActive(false);
        }
        if (partSlected3 == FlyLegs)
        {
            beeLegs.SetActive(false);
            waspLegs.SetActive(false);
            flyLegs.SetActive(true);
        }
    }
    public void left3()
    {
        partSlected3--;
        if (partSlected3 == -3)
        {
            partSlected3 = 0;
        }
        if (partSlected3 == BeeLegs)
        {
            beeLegs.SetActive(true);
            waspLegs.SetActive(false);
            flyLegs.SetActive(false);
        }
        if (partSlected3 == WaspLegs2)
        {
            beeLegs.SetActive(false);
            waspLegs.SetActive(true);
            flyLegs.SetActive(false);
        }
        if (partSlected3 == FlyLegs2)
        {
            beeLegs.SetActive(false);
            waspLegs.SetActive(false);
            flyLegs.SetActive(true);
        }
    }
}
