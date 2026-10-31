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
                string tokenacceso = "EAAgxUk2nLocBS4tuQOU2Y4me1S5halGmRIIM7jyWAWALXg3qZBlUsFzBpqc6XbmBo4YGNkVnJzfLMm8bUlkk4WNYOwGIxC2Cpwc5IXaEdHYuEryuJBdUBs6cSHLQrWcmnO6JtNo0CAXoZAs8zsBUg9IABnRrgZAPQhMhZAnWrr9PRxOOnZBliHZCtXMpkRnpZAj6G1T5aD2h79aaqSPw9wfEtGUK0HHCFTIICgdbGOFh7Fc9GFOEx7ZAKACSOvd3K1MCCImlYG3gpEnUN5B4wUukXTGc91VMvzqevHO6DwZDZD";

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

