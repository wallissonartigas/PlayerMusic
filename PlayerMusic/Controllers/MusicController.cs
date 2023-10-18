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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace PlayerMusic.Controllers
{
    public class MusicController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;


        public MusicController(IHttpClientFactory httpClientFactory, IConfiguration configuration , IHostingEnvironment hostingEnvironment)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:ApiMusicUrl"];

            try
            {
                httpClient.BaseAddress = new Uri(apiBaseUrl);
                var response = await httpClient.GetFromJsonAsync<List<MusicModel>>("api/music"); 
                return View(response);
            }
            catch (HttpRequestException e)
            {
              
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
        public async Task<IActionResult> Salvar([Bind("Autor,Genero,AlbumDiretorio,MusicaDiretorio,NomeMusica")] MusicModel musica, IFormFile AlbumImageFile, IFormFile MusicaDiretorioFile)
        {
            if (ModelState.IsValid)
            {
                if (AlbumImageFile != null && AlbumImageFile.Length > 0 && MusicaDiretorioFile != null && MusicaDiretorioFile.Length > 0)
                {
          
                    var uniqueFileName = Guid.NewGuid() + "_" + AlbumImageFile.FileName ;
                    var uniqueFileNameMusic = Guid.NewGuid() + "_" + MusicaDiretorioFile.FileName;
               
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", uniqueFileName);
                    var filePathMusic = Path.Combine(@"C:\Users\dev\source\repos\GitHub\PlayerMusic\MusicasAlbum", uniqueFileNameMusic);
                  
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        AlbumImageFile.CopyTo(stream);
                    }

                    using (var stream = new FileStream(filePathMusic, FileMode.Create))
                    {
                        await MusicaDiretorioFile.CopyToAsync(stream);
                    }

                 
                    musica.AlbumDiretorio = uniqueFileName;
                    musica.MusicaDiretorio = uniqueFileNameMusic;

                    using (var httpClient = new HttpClient())
                    {
                        var apiBaseUrl = _configuration["ApiSettings:ApiMusicUrl"];

                        httpClient.BaseAddress = new Uri(apiBaseUrl);

                        var musicaContent = new StringContent(JsonConvert.SerializeObject(musica), Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync("api/music", musicaContent);

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
                else
                {
                    TempData["ErrorMessage"] = "Imagem do álbum não foi fornecida.";
                    return View(musica);
                }
            }
            return View(musica);
        }




    }
}
