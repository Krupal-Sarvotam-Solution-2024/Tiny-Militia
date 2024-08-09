using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAnimation : MonoBehaviour
{
    public GameObject Heading;
    public Gradient PersantageColor;
    public TextMeshProUGUI audioText,soundText;

    public Image audio_filler,sound_filler;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HeadingAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ValueChangeofSound(float value)
    {
        soundText.text = value.ToString("0") + " %" ;
        soundText.color = PersantageColor.Evaluate(value/100);
        sound_filler.color = PersantageColor.Evaluate(value / 100);
    } 
    public void ValueChangeofAudio(float value)
    {
        audioText.text = value.ToString("0") + " %" ;
        audioText.color = PersantageColor.Evaluate(value/100);
        audio_filler.color = PersantageColor.Evaluate(value / 100);
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
        Heading.transform.GetComponent<Text>().text = "TINY MILITIA";
        yield return new WaitForSeconds(30f);
        StartCoroutine(HeadingAnimation());

    }
}
