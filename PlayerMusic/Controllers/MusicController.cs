using Microsoft.AspNetCore.Mvc;
using PlayerMusic.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PlayerMusic.Controllers
{
    public class MusicController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;


        public MusicController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:ApiMusicUrl"];

            try
            {
                httpClient.BaseAddress = new Uri(apiBaseUrl);
                var response = await httpClient.GetFromJsonAsync<List<MusicModel>>("api/music"); // Substitua pela URL da sua API
                return View(response);
            }
            catch (HttpRequestException e)
            {
                // Trate erros de solicitação aqui
                return View(new List<MusicModel>());
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:ApiMusicUrl"];
            

            try
            {
               
                httpClient.BaseAddress = new Uri(apiBaseUrl);
                var response = await httpClient.DeleteAsync($"api/music/{id}");

                if (response.IsSuccessStatusCode)
                {
                   
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Falha ao excluir a música.";
                    return RedirectToAction("Index");
                }
            }
            catch (HttpRequestException e)
            {
              
                TempData["ErrorMessage"] = "Erro na solicitação: " + e.Message;
                return RedirectToAction("Index");
            }
        }
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Salvar([Bind("Autor,Genero,AlbumDiretorio,MusicaDiretorio,NomeMusica")] MusicModel musica)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    var apiBaseUrl = _configuration["ApiSettings:ApiMusicUrl"];

                    httpClient.BaseAddress = new Uri(apiBaseUrl);

                    var content = new StringContent(JsonConvert.SerializeObject(musica), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("api/music", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index"); 
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Falha ao salvar a música.";
                        return View(musica);
                    }
                }
            }
            return View(musica);
        }


    }
}
