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
                string tokenacceso = "EAAgxUk2nLocBS7ZBZAtRKgpNxLfqBnQbtth5C6JgZAoGiLxty4vUX6RiGnfsPa4O9YZA4TFbibUjbHgqHOJjbIdWMZC2cixgT4XNElEm4FVHZAa3gFIWx9uRoy84zeExGUjq1lwEvjHptRjusZAEKPUn84jaFHOx71FyaiFmZAiJiCGxtyOMZCaPgREJfvqZCU5fQWZC36QnJBqXu62ViBZCxV8Yf0mMNvW83N9YfcUA4r09AHiqOx5qNUYwCXOAaISXCNlhYM3g5CZAhaoPfZAC6SZBK8ekA8sZCoogZAkq7pr8gxuMc";

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

