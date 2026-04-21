using UnityEngine;

public class CertButtOn : MonoBehaviour
{
  //  public GlobalProgressManager GlobalProgress;
    public bool isCerActive;
    public GameObject DaCertButton;
    public GameObject LockedCertButton;
    public GameObject cert;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DaCertButton.SetActive(false);
        LockedCertButton.SetActive(true);
    }
   
    public void CertBUtton()
    {
        if(DaCertButton==true)
        {
            cert.SetActive(true);
        }
        
    }
    public void UnCertBUtton()
    {
        if(DaCertButton == false)
        {
            cert.SetActive(false);
        }
        
    }

}
