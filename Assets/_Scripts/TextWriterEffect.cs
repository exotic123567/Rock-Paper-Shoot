using System.Collections;
using TMPro;
using UnityEngine;

public class TextWriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.05f;
    private string fullText;
    public float fadeOutDuration = 1.0f; // Duration of the fade-out effect

    // Set text for this specific TextWriterEffect based on player class and text position
    public void SetTextForClass(uint playerType, int position)
    {
        // Assign the text based on player type and position (index in sequence)
        fullText = (playerType, position) switch
        {
            (0, 0) => "YOU ARE THE MIGHTY ROCK!",
            (0, 1) => "YOU CAN DEFEAT SCISSORS BUT...",
            (0, 2) => "BEWARE OF PAPER BULLETS!",
            (1, 0) => "YOU ARE THE FLEXIBLE PAPER!",
            (1, 1) => "YOU CAN DEFEAT ROCK BUT...",
            (1, 2) => "BEWARE OF SCISSOR BULLETS!",
            (2, 0) => "YOU ARE THE SHARP SCISSORS!",
            (2, 1) => "YOU CAN DEFEAT PAPER BUT...",
            (2, 2) => "BEWARE OF ROCK BULLETS!",
            _ => "Unknown class or position"
        };

        textComponent.text = ""; // Clear text initially
    }

    // Typing effect for each text component
    public IEnumerator StartTyping()
    {
        textComponent.text = ""; // Clear text before starting

        // Display each letter one by one
        foreach (char letter in fullText)
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Coroutine to fade out the text gradually
    public IEnumerator FadeOutText()
    {
        Color originalColor = textComponent.color;
        float startAlpha = originalColor.a;
        float time = 0;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, time / fadeOutDuration);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure the text is completely transparent at the end
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }
}
