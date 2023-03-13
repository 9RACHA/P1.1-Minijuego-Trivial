using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

[Serializable]
public class TriviaQuestion {
    public string category;
    public string type;
    public string difficulty;
    public string question;
    public string correct_answer;
    public string[] incorrect_answers;
}

[Serializable]
public class TriviaResponse {
    public int response_code;
    public List<TriviaQuestion> results;
}

public class TriviaAPI : MonoBehaviour {

    public TriviaResponse triviaResponse;
    private int currentQuestionIndex = 0;

    IEnumerator Start() {
        string url = "https://opentdb.com/api.php?amount=10&type=multiple";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + webRequest.error);
            } else {
                string json = webRequest.downloadHandler.text;
                triviaResponse = JsonUtility.FromJson<TriviaResponse>(json);

                Debug.Log("Bienvenido al juego de trivia! Responde las preguntas correctamente para ganar.");
                Debug.Log("Número de preguntas: " + triviaResponse.results.Count);

                while (currentQuestionIndex < triviaResponse.results.Count) {
                    TriviaQuestion currentQuestion = triviaResponse.results[currentQuestionIndex];
                    Debug.Log("Pregunta: " + currentQuestion.question);
                    Debug.Log("Opciones:");

                    List<string> allAnswers = new List<string>();
                    allAnswers.AddRange(currentQuestion.incorrect_answers);
                    allAnswers.Add(currentQuestion.correct_answer);
                    allAnswers.Shuffle(); // Mezclar las opciones de respuesta

                    for (int i = 0; i < allAnswers.Count; i++) {
                        Debug.Log((i + 1) + ") " + allAnswers[i]);
                    }

                    string playerInput = Console.ReadLine();

                    if (int.TryParse(playerInput, out int playerAnswerIndex)) {
                        if (playerAnswerIndex >= 1 && playerAnswerIndex <= allAnswers.Count) {
                            string playerAnswer = allAnswers[playerAnswerIndex - 1];

                            if (playerAnswer == currentQuestion.correct_answer) {
                                Debug.Log("¡Correcto!");
                            } else {
                                Debug.Log("Respuesta incorrecta. La respuesta correcta era: " + currentQuestion.correct_answer);
                            }

                            currentQuestionIndex++;
                        } else {
                            Debug.Log("Ingresa un número válido entre 1 y " + allAnswers.Count);
                        }
                    } else {
                        Debug.Log("Ingresa un número válido entre 1 y " + allAnswers.Count);
                    }
                }

                Debug.Log("¡Felicidades! Has respondido todas las preguntas correctamente.");
            }
        }
    }
}

public static class ListExtensions {
    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
