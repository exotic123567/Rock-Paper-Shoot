using System.Collections;
using UnityEngine;

public class TextWriterController : MonoBehaviour
{
    public GameObject[] textWriterObjects; // Assign GameObjects with TextWriterEffect in the Inspector
    private TextWriterEffect[] textWriters;

    public void InitializeTextWriters(uint playerType)
    {
        // Initialize the textWriters array based on textWriterObjects size
        textWriters = new TextWriterEffect[textWriterObjects.Length];
        
        // Populate textWriters array with TextWriterEffect components from each GameObject
        for (int i = 0; i < textWriterObjects.Length; i++)
        {
            textWriters[i] = textWriterObjects[i].GetComponent<TextWriterEffect>();
            if (textWriters[i] == null)
            {
                Debug.LogError("TextWriterEffect component missing on GameObject: " + textWriterObjects[i].name);
            }
            else
            {
                // Set the appropriate text based on player class type
                textWriters[i].SetTextForClass(playerType, i);
            }
        }

        // Start typing each text sequentially
        StartCoroutine(TypeTextsSequentially());
    }

    private IEnumerator TypeTextsSequentially()
    {
        // Type each text sequentially, waiting until each completes before moving to the next
        foreach (TextWriterEffect textWriter in textWriters)
        {
            yield return StartCoroutine(textWriter.StartTyping());
        }

        // Wait for 2 seconds after all texts are done showing
        yield return new WaitForSeconds(2);

        // Fade out each text
        foreach (TextWriterEffect textWriter in textWriters)
        {
            StartCoroutine(textWriter.FadeOutText());
        }

        // Wait for the fade-out duration before deactivating the GameObject
        yield return new WaitForSeconds(textWriters[0].fadeOutDuration);

        // Deactivate the TextWriterController GameObject (or each text object)
        gameObject.SetActive(false); // Deactivate the entire TextWriterController GameObject
    }
}
