using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata;

namespace EcomerceCon_Apis.Services
{
    public class WhattsapServices : IWhatsappService
    {
        public async Task EnviarRespuesta(string NumeroDestino, string Mensaje)
        {
            try
            {
                string idnumeroremitente = "1291986937341080";
                string tokenacceso = "EAAgxUk2nLocBSZBeqvRf6kHmKCG1rzMylPQQtdscNTJ0oejHKHyAVX8Px8VXiBwZBs3BQ73SWLzukWAsoFMJpVkQZCWCblxXvPvRqk6rywGYoBMGSs3HLs7Gkvx6xzZBzBTw1TrxOXQoKhtZCBwYyUHnY3j5UNVYgDVb8fKLVWM31NlWYTz33ZBAmcQmfkmCYuB0EEZBId4Y3zqJF3KaLqMBw9pZCy16p1GZCggKMwXmrm8F0nY7zyfzxZAcu3WYQYWYZBQmd95uVlnqTmDyuui4nJM9E5vwc1AH42R2Br5g8wZD";

                string numeroDestinoApi = NumeroDestino.Trim();

                if (numeroDestinoApi.StartsWith("549"))
                {
                    numeroDestinoApi = "54" + numeroDestinoApi.Substring(3);
                }

                using (var httpclient = new HttpClient())
                {
                    httpclient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenacceso);
                    var payload = new
                    {
                        messaging_product = "whatsapp",
                        to = numeroDestinoApi,
                        text = new { body = Mensaje }
                    };

                    var response = await httpclient.PostAsJsonAsync($"https://graph.facebook.com/v25.0/{idnumeroremitente}/messages", payload);
                    Console.WriteLine($"ID emisor: {idnumeroremitente}");
                    Console.WriteLine($"Número destinatario: {NumeroDestino}");

                    var contenido = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"json: {contenido}");


                    var respuestaMeta = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"Estado: {response.StatusCode}");
                    Console.WriteLine($"Respuesta: {respuestaMeta}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw ;
            }

            }
        }
    }

