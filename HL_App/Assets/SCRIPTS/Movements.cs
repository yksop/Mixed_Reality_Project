using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour
{
    public AudioSource source_scanning;
    public AudioClip clip_scanning;

    public GameObject PALLET_obj;
    public GameObject HANDLEPACK_obj;

    public GameObject TOF1_GO;
    public GameObject TOF2_GO;
    public GameObject TOF3_GO;
    private MiroRGB3DCameraV7 m_camera1;
    private MiroRGB3DCameraV7 m_camera2;
    private MiroRGB3DCameraV7 m_camera3;

    public GameObject CubeCheck_obj;
    public GameObject CubeErrorF_obj;
    public GameObject CubeErrorL_obj;


    public Material Mwhite;
    public Material Mgreen;
    public Material Myellow;
    public Material Mred;

    public GameObject TextOK;
    public GameObject TextNONok;

    private bool firstcheck_outside = false;
    private bool secondcheck_outside = false;

    private Vector3 startPos = new Vector3(0.0f, 0.441f, 1.3115f);
    private Vector3 endPos = new Vector3(0.0f, 0.441f, -1.27f);
    private Vector3 handlepack_startPos = new Vector3(-0.484f, -0.250f, -0.437f);
    private bool isCRrunning_1 = false;
    private bool isCRrunning_2 = false;

    private void Start()
    {
        m_camera1 = TOF1_GO.GetComponent<MiroRGB3DCameraV7>();
        m_camera2 = TOF2_GO.GetComponent<MiroRGB3DCameraV7>();
        m_camera3 = TOF3_GO.GetComponent<MiroRGB3DCameraV7>();
        CubeCheck_obj.SetActive(value: false);
        CubeErrorF_obj.SetActive(value: false);
        CubeErrorL_obj.SetActive(value: false);

        TextOK.SetActive(value: false);
        TextNONok.SetActive(value: false);


        ResetObject();
    }

    public void MoveObject()
    {
        StartCoroutine("MoveOverSeconds");
    }

    public void ScanObject()
    {
        StartCoroutine("ScanOverSeconds");
    }

    public void ResetObject()
    {
        if(isCRrunning_1)
        {
            StopCoroutine("MoveOverSeconds");
            isCRrunning_1 = false;
        }

        if (isCRrunning_2)
        {
            StopCoroutine("ScanOverSeconds");
            isCRrunning_2 = false;
        }

        PALLET_obj.transform.localPosition = startPos;
        HANDLEPACK_obj.transform.localPosition = handlepack_startPos;
    }

    private IEnumerator MoveOverSeconds()
    {
        isCRrunning_1 = true;

        float elapsedTime = 0;
        float seconds = 4.0f;
        while (elapsedTime < seconds)
        {
            PALLET_obj.transform.localPosition = Vector3.Lerp(startPos, endPos, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PALLET_obj.transform.localPosition = endPos;

        // Lock gravity
        HANDLEPACK_obj.GetComponent<Rigidbody>().useGravity = false;



        // TOF SCAN 
        source_scanning.PlayOneShot(clip_scanning);

        Vector3[] pointCloud;
        Vector3[] pointCloud_color;

        m_camera1.CalcPointCloud(out pointCloud, out pointCloud_color);
        m_camera1.DrawLastCapturedPoints(true);

        yield return new WaitForSeconds(1);
        m_camera3.CalcPointCloud(out pointCloud, out pointCloud_color);
        m_camera3.DrawLastCapturedPoints(true);

        yield return new WaitForSeconds(1);
        m_camera2.CalcPointCloud(out pointCloud, out pointCloud_color);
        m_camera2.DrawLastCapturedPoints(true);

        yield return new WaitForSeconds(1);
        CubeCheck_obj.GetComponent<Renderer>().material = Mwhite;
        CubeCheck_obj.SetActive(value: true);

        yield return new WaitForSeconds(5);
        m_camera1.DrawLastCapturedPoints(false);
        m_camera2.DrawLastCapturedPoints(false);
        m_camera3.DrawLastCapturedPoints(false);


        //Debug.Log(HANDLEPACK_obj.transform.localPosition.x);
        //Debug.Log(HANDLEPACK_obj.transform.localPosition.y);
        //Debug.Log(HANDLEPACK_obj.transform.localPosition.z);

        //if (HANDLEPACK_obj.transform.localPosition.z < -0.59f+0.4f)
        //{
        //CubeError_obj.transform.rotation = Quaternion.Euler(0,0,-90);
        //   CubeErrorF_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, HANDLEPACK_obj.transform.localPosition.y, ((HANDLEPACK_obj.transform.localPosition.z - 0.4f) + 0.59f) / 2.0f - 0.59f);
        //   CubeErrorF_obj.transform.localScale = new Vector3(HANDLEPACK_obj.transform.localScale.y+0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, -((HANDLEPACK_obj.transform.localPosition.z - 0.4f) + 0.59f) + 0.005f);        
        //   CubeErrorF_obj.SetActive(value: true);
        //   firstcheck_outside = true;
        //} else if (HANDLEPACK_obj.transform.localPosition.z > 0.59f - 0.4f)
        //{
        //    CubeErrorF_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, HANDLEPACK_obj.transform.localPosition.y, ((HANDLEPACK_obj.transform.localPosition.z + 0.4f) - 0.59f) / 2.0f + 0.59f);
        //    CubeErrorF_obj.transform.localScale = new Vector3(HANDLEPACK_obj.transform.localScale.y + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, ((HANDLEPACK_obj.transform.localPosition.z + 0.4f) - 0.59f) + 0.005f);
        //   CubeErrorF_obj.SetActive(value: true);
        //    firstcheck_outside = true;
        //}
        //else
        //{
        //    CubeErrorF_obj.SetActive(value: false);
        //    firstcheck_outside = false;

        //}

        if (HANDLEPACK_obj.transform.localPosition.z < -0.176f)
        {
            //CubeError_obj.transform.rotation = Quaternion.Euler(0,0,-90);
            CubeErrorF_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, HANDLEPACK_obj.transform.localPosition.y, (HANDLEPACK_obj.transform.localPosition.z + 0.176f) / 2.0f - (0.4f+ 0.176f));
            CubeErrorF_obj.transform.localScale = new Vector3(HANDLEPACK_obj.transform.localScale.y + 0.003f, HANDLEPACK_obj.transform.localScale.x + 0.003f, -(HANDLEPACK_obj.transform.localPosition.z + 0.176f) + 0.003f);
            CubeErrorF_obj.SetActive(value: true);
            firstcheck_outside = true;
        }
        else if (HANDLEPACK_obj.transform.localPosition.z > 0.178f)
        {
            CubeErrorF_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, HANDLEPACK_obj.transform.localPosition.y, (HANDLEPACK_obj.transform.localPosition.z - 0.178f) / 2.0f + (0.4f+ 0.178f));
            CubeErrorF_obj.transform.localScale = new Vector3(HANDLEPACK_obj.transform.localScale.y + 0.003f, HANDLEPACK_obj.transform.localScale.x + 0.003f, (HANDLEPACK_obj.transform.localPosition.z - 0.178f) + 0.003f);
            CubeErrorF_obj.SetActive(value: true);
            firstcheck_outside = true;
        }
        else
        {
            CubeErrorF_obj.SetActive(value: false);
            firstcheck_outside = false;

        }



        //if (HANDLEPACK_obj.transform.localPosition.y < -0.363f + 0.2f)
        //{
            //CubeError_obj.transform.rotation = Quaternion.Euler(0,0,-90);
        //    CubeErrorL_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, ((HANDLEPACK_obj.transform.localPosition.y - 0.2f) + 0.363f) / 2.0f - 0.363f, HANDLEPACK_obj.transform.localPosition.z);
        //    CubeErrorL_obj.transform.localScale = new Vector3(-((HANDLEPACK_obj.transform.localPosition.y - 0.2f) + 0.363f) + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, HANDLEPACK_obj.transform.localScale.z + 0.005f);
        //    CubeErrorL_obj.SetActive(value: true);
        //    secondcheck_outside = true;
        //}
        //else if (HANDLEPACK_obj.transform.localPosition.y > 0.363f - 0.2f)
        //{
        //    CubeErrorL_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, ((HANDLEPACK_obj.transform.localPosition.y + 0.2f) - 0.363f) / 2.0f + 0.363f , HANDLEPACK_obj.transform.localPosition.z);
        //    CubeErrorL_obj.transform.localScale = new Vector3( ((HANDLEPACK_obj.transform.localPosition.y + 0.2f) - 0.363f) + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, HANDLEPACK_obj.transform.localScale.z + 0.005f);
        //    CubeErrorL_obj.SetActive(value: true);
        //    secondcheck_outside = true;
        //}
        //else
        //{
        //    CubeErrorL_obj.SetActive(value: false);
        //     secondcheck_outside = false;
        //}


        if (HANDLEPACK_obj.transform.localPosition.y < -0.185f)
        {
            //CubeError_obj.transform.rotation = Quaternion.Euler(0,0,-90);
            CubeErrorL_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, (HANDLEPACK_obj.transform.localPosition.y + 0.185f) / 2.0f - (+0.185f + 0.2f), HANDLEPACK_obj.transform.localPosition.z);
            CubeErrorL_obj.transform.localScale = new Vector3(-(HANDLEPACK_obj.transform.localPosition.y + 0.185f) + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, HANDLEPACK_obj.transform.localScale.z + 0.005f);
            CubeErrorL_obj.SetActive(value: true);
            secondcheck_outside = true;
        }
        else if (HANDLEPACK_obj.transform.localPosition.y > 0.180f)
        {
            CubeErrorL_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, (HANDLEPACK_obj.transform.localPosition.y - 0.180f) / 2.0f + (+0.180f + 0.2f), HANDLEPACK_obj.transform.localPosition.z);
            CubeErrorL_obj.transform.localScale = new Vector3((HANDLEPACK_obj.transform.localPosition.y - 0.180f) + 0.003f, HANDLEPACK_obj.transform.localScale.x + 0.003f, HANDLEPACK_obj.transform.localScale.z + 0.003f);
            CubeErrorL_obj.SetActive(value: true);
            secondcheck_outside = true;
        }
        else
        {
            CubeErrorL_obj.SetActive(value: false);
            secondcheck_outside = false;
        }


        if ((firstcheck_outside == true) || (secondcheck_outside == true))
        {
            CubeCheck_obj.GetComponent<Renderer>().material = Myellow;            
            TextNONok.SetActive(value: true);
            TextOK.SetActive(value: false);
        }
        else
        {
            CubeCheck_obj.GetComponent<Renderer>().material = Mgreen;
            TextOK.SetActive(value: true);
            TextNONok.SetActive(value: false);
        }

        isCRrunning_1 = false;
    }


    private IEnumerator ScanOverSeconds()
    {
        isCRrunning_2 = true;


        // Lock gravity
        HANDLEPACK_obj.GetComponent<Rigidbody>().useGravity = false;

        // TOF SCAN 
        source_scanning.PlayOneShot(clip_scanning);

        Vector3[] pointCloud;
        Vector3[] pointCloud_color;

        m_camera1.CalcPointCloud(out pointCloud, out pointCloud_color);
        m_camera1.DrawLastCapturedPoints(true);

        yield return new WaitForSeconds(1);
        m_camera3.CalcPointCloud(out pointCloud, out pointCloud_color);
        m_camera3.DrawLastCapturedPoints(true);

        yield return new WaitForSeconds(1);
        m_camera2.CalcPointCloud(out pointCloud, out pointCloud_color);
        m_camera2.DrawLastCapturedPoints(true);

        yield return new WaitForSeconds(1);
        CubeCheck_obj.GetComponent<Renderer>().material = Mwhite;
        CubeCheck_obj.SetActive(value: true);

        yield return new WaitForSeconds(1);
        m_camera1.DrawLastCapturedPoints(false);
        m_camera2.DrawLastCapturedPoints(false);
        m_camera3.DrawLastCapturedPoints(false);


        //Debug.Log(HANDLEPACK_obj.transform.localPosition.x);
        //Debug.Log(HANDLEPACK_obj.transform.localPosition.y);
        //Debug.Log(HANDLEPACK_obj.transform.localPosition.z);

        //if (HANDLEPACK_obj.transform.localPosition.z < -0.59f + 0.4f)
        //{
        //CubeError_obj.transform.rotation = Quaternion.Euler(0,0,-90);
        //    CubeErrorF_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, HANDLEPACK_obj.transform.localPosition.y, ((HANDLEPACK_obj.transform.localPosition.z - 0.4f) + 0.59f) / 2.0f - 0.59f);
        //    CubeErrorF_obj.transform.localScale = new Vector3(HANDLEPACK_obj.transform.localScale.y + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, -((HANDLEPACK_obj.transform.localPosition.z - 0.4f) + 0.59f) + 0.005f);
        //    CubeErrorF_obj.SetActive(value: true);
        //    firstcheck_outside = true;
        //}
        //else if (HANDLEPACK_obj.transform.localPosition.z > 0.59f - 0.4f)
        //{
        //    CubeErrorF_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, HANDLEPACK_obj.transform.localPosition.y, ((HANDLEPACK_obj.transform.localPosition.z + 0.4f) - 0.59f) / 2.0f + 0.59f);
        //    CubeErrorF_obj.transform.localScale = new Vector3(HANDLEPACK_obj.transform.localScale.y + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, ((HANDLEPACK_obj.transform.localPosition.z + 0.4f) - 0.59f) + 0.005f);
        //    CubeErrorF_obj.SetActive(value: true);
        //    firstcheck_outside = true;
        //}
        //else
        //{
        //   CubeErrorF_obj.SetActive(value: false);
        //    firstcheck_outside = false;

        //}

        if (HANDLEPACK_obj.transform.localPosition.z < -0.176f)
        {
            //CubeError_obj.transform.rotation = Quaternion.Euler(0,0,-90);
            CubeErrorF_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, HANDLEPACK_obj.transform.localPosition.y, (HANDLEPACK_obj.transform.localPosition.z + 0.176f) / 2.0f - (0.4f + 0.176f));
            CubeErrorF_obj.transform.localScale = new Vector3(HANDLEPACK_obj.transform.localScale.y + 0.003f, HANDLEPACK_obj.transform.localScale.x + 0.003f, -(HANDLEPACK_obj.transform.localPosition.z + 0.176f) + 0.003f);
            CubeErrorF_obj.SetActive(value: true);
            firstcheck_outside = true;
        }
        else if (HANDLEPACK_obj.transform.localPosition.z > 0.178f)
        {
            CubeErrorF_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, HANDLEPACK_obj.transform.localPosition.y, (HANDLEPACK_obj.transform.localPosition.z - 0.178f) / 2.0f + (0.4f + 0.178f));
            CubeErrorF_obj.transform.localScale = new Vector3(HANDLEPACK_obj.transform.localScale.y + 0.003f, HANDLEPACK_obj.transform.localScale.x + 0.003f, (HANDLEPACK_obj.transform.localPosition.z - 0.178f) + 0.003f);
            CubeErrorF_obj.SetActive(value: true);
            firstcheck_outside = true;
        }
        else
        {
            CubeErrorF_obj.SetActive(value: false);
            firstcheck_outside = false;

        }


        //if (HANDLEPACK_obj.transform.localPosition.y < -0.363f + 0.2f)
        //{
        //CubeError_obj.transform.rotation = Quaternion.Euler(0,0,-90);
        //    CubeErrorL_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, ((HANDLEPACK_obj.transform.localPosition.y - 0.2f) + 0.363f) / 2.0f - 0.363f, HANDLEPACK_obj.transform.localPosition.z);
        //    CubeErrorL_obj.transform.localScale = new Vector3(-((HANDLEPACK_obj.transform.localPosition.y - 0.2f) + 0.363f) + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, HANDLEPACK_obj.transform.localScale.z + 0.005f);
        //    CubeErrorL_obj.SetActive(value: true);
        //    secondcheck_outside = true;
        //}
        //else if (HANDLEPACK_obj.transform.localPosition.y > 0.363f - 0.2f)
        //{
        //    CubeErrorL_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, ((HANDLEPACK_obj.transform.localPosition.y + 0.2f) - 0.363f) / 2.0f + 0.363f, HANDLEPACK_obj.transform.localPosition.z);
        //    CubeErrorL_obj.transform.localScale = new Vector3(((HANDLEPACK_obj.transform.localPosition.y + 0.2f) - 0.363f) + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, HANDLEPACK_obj.transform.localScale.z + 0.005f);
        //    CubeErrorL_obj.SetActive(value: true);
        //    secondcheck_outside = true;
        //}
        //else
        //{
        //    CubeErrorL_obj.SetActive(value: false);
        //    secondcheck_outside = false;
        //}


        if (HANDLEPACK_obj.transform.localPosition.y < -0.185f)
        {
            //CubeError_obj.transform.rotation = Quaternion.Euler(0,0,-90);
            CubeErrorL_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, (HANDLEPACK_obj.transform.localPosition.y + 0.185f) / 2.0f - (+0.185f + 0.2f), HANDLEPACK_obj.transform.localPosition.z);
            CubeErrorL_obj.transform.localScale = new Vector3(-(HANDLEPACK_obj.transform.localPosition.y + 0.185f) + 0.005f, HANDLEPACK_obj.transform.localScale.x + 0.005f, HANDLEPACK_obj.transform.localScale.z + 0.005f);
            CubeErrorL_obj.SetActive(value: true);
            secondcheck_outside = true;
        }
        else if (HANDLEPACK_obj.transform.localPosition.y > 0.180f)
        {
            CubeErrorL_obj.transform.localPosition = new Vector3(HANDLEPACK_obj.transform.localPosition.x, (HANDLEPACK_obj.transform.localPosition.y - 0.180f) / 2.0f + (+0.180f + 0.2f), HANDLEPACK_obj.transform.localPosition.z);
            CubeErrorL_obj.transform.localScale = new Vector3((HANDLEPACK_obj.transform.localPosition.y - 0.180f) + 0.003f, HANDLEPACK_obj.transform.localScale.x + 0.003f, HANDLEPACK_obj.transform.localScale.z + 0.003f);
            CubeErrorL_obj.SetActive(value: true);
            secondcheck_outside = true;
        }
        else
        {
            CubeErrorL_obj.SetActive(value: false);
            secondcheck_outside = false;
        }





        if ((firstcheck_outside == true) || (secondcheck_outside == true))
        {
            CubeCheck_obj.GetComponent<Renderer>().material = Myellow;
            TextNONok.SetActive(value: true);
            TextOK.SetActive(value: false);
        }
        else
        {
            CubeCheck_obj.GetComponent<Renderer>().material = Mgreen;
            TextOK.SetActive(value: true);
            TextNONok.SetActive(value: false);
        }

        isCRrunning_2 = false;
    }

}
