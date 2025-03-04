using System.Collections;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    private int WaitTime = 3;
    public Light Spotlight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WorldGreeterCoroutine());
        if (Spotlight != null)
        {
            StartCoroutine(LightBlinkerCoroutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator WorldGreeterCoroutine()
    {
        Debug.Log("World?");
        while (true)
        {
            yield return new WaitForSeconds(WaitTime);
            Debug.Log("Hello World");
        }
    }

    public IEnumerator LightBlinkerCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            Spotlight.gameObject.SetActive(!Spotlight.gameObject.activeSelf);
            yield return new WaitForEndOfFrame();
            
        }
    }
}
