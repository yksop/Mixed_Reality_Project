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
    // Initialize the server
    _httpListener = new HttpListener();
    _httpListener.Prefixes.Add(Url);
    _httpListener.Start();
    Debug.Log($"Server started. Listening on {Url}");

    // Start the server thread
    _serverThread = new Thread(ListenForRequests);
    _serverThread.Start();
  }

  void OnApplicationQuit()
  {
    // Stop the server when the application quits
    _httpListener.Stop();
    _serverThread.Abort();
  }

  private void ListenForRequests()
  {
    while (_httpListener.IsListening)
    {
      Debug.Log("Waiting for requests...");
      try
      {
        var context = _httpListener.GetContext();
        var request = context.Request;

        // Check if it is a reset request
        if (request.HttpMethod == "POST" && request.RawUrl == "/reset-scene")
        {
          Debug.Log("Received scene reset command!");
          ResetScene();

          // Respond to the client
          var response = context.Response;
          string responseString = "Scene reset!";
          byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
          response.ContentLength64 = buffer.Length;
          response.OutputStream.Write(buffer, 0, buffer.Length);
          response.OutputStream.Close();
        }
        else
        {
          Debug.Log("Received request that is NOT a scene reset command!");
        }
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"Server error: {ex.Message}");
      }
    }
  }

  private void ResetScene()
  {
    // Reload the current scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}