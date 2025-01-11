using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Threading;

public class SceneResetServer : MonoBehaviour
{
	private HttpListener _httpListener;
	private Thread _serverThread;
	private const string Url = "http://localhost:8080/";

	void Start()
	{
		// Inizializza il server
		_httpListener = new HttpListener();
		_httpListener.Prefixes.Add(Url);
		_httpListener.Start();
		Debug.Log($"Server avviato. Ascolto su {Url}");

		// Avvia il thread del server
		_serverThread = new Thread(ListenForRequests);
		_serverThread.Start();
	}

	void OnApplicationQuit()
	{
		// Ferma il server quando l'app si chiude
		_httpListener.Stop();
		_serverThread.Abort();
	}

	private void ListenForRequests()
	{
		while (_httpListener.IsListening)
		{
			Debug.Log("In attesa di richieste...");
			try
			{
				var context = _httpListener.GetContext();
				var request = context.Request;

				// Controlla se è una richiesta di reset
				if (request.HttpMethod == "POST" && request.RawUrl == "/reset-scene")
				{
					Debug.Log("Ricevuto comando di reset scena!");
					ResetScene();

					// Risponde al client
					var response = context.Response;
					string responseString = "Scena resettata!";
					byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
					response.ContentLength64 = buffer.Length;
					response.OutputStream.Write(buffer, 0, buffer.Length);
					response.OutputStream.Close();
				}
				else
				{
					Debug.Log("Ricevuta richiesta che NON è comando di reset scena!");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Errore nel server: {ex.Message}");
			}
		}
	}

	private void ResetScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}