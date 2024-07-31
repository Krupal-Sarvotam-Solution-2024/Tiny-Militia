using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public GameObject Heading;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HeadingAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator HeadingAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "T";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TI";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TIN";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TINY";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TINY \nM";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TINY \nMI";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TINY \nMIL";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TINY \nMILI";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TINY \nMILIT";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TINY \nMILITI";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<Text>().text = "TINY \nMILITIA";
        yield return new WaitForSeconds(30f);
        StartCoroutine(HeadingAnimation());

    }
}
