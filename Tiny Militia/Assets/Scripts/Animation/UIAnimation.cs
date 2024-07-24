using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "T";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TI";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TIN";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TINY";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TINY \nM";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TINY \nMI";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TINY \nMIL";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TINY \nMILI";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TINY \nMILIT";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TINY \nMILITI";
        yield return new WaitForSeconds(0.5f);
        Heading.transform.GetComponent<TextMeshProUGUI>().text = "TINY \nMILITIA";
        yield return new WaitForSeconds(30f);
        StartCoroutine(HeadingAnimation());

    }
}
