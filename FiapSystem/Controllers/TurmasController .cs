using FiapSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoTurmasFiap.Entities;
using System.Data;
using System.Text;

namespace FiapSystem.Controllers
{
    public class TurmasController : Controller
    {
        #region Propriedades
        private readonly string ENDPOINT = "";
        private readonly HttpClient httpClient = null;
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        public TurmasController(IConfiguration configuration)
        {
            _configuration = configuration;
            ENDPOINT = _configuration["AppConfig:Endpoints:Url_Api_Turmas"];
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ENDPOINT);
        }
        #endregion

        #region CRUD Turmas
        public async Task<IActionResult> Index()
        {
            try
            {
                List<TurmaViewModel> Turmas = null;

                HttpResponseMessage response = await httpClient.GetAsync(ENDPOINT);

                if (response.IsSuccessStatusCode) 
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Turmas = JsonConvert.DeserializeObject<List<TurmaViewModel>>(content);
                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação");
                }

                return View(Turmas);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw ex;
            }
        }

        public async Task<IActionResult> Detalhe(int id)
        {
            try
            {
                TurmaViewModel result = await GetTurma(id);
                return View(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> Novo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Novo([Bind("Nome, Ano")] TurmaViewModel turma)
        {
            try
            {
                ByteArrayContent byteContent = ConverterObjetoByte(turma);

                string url = ENDPOINT;
                HttpResponseMessage response = await httpClient.PostAsync(url, byteContent);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação de post.");
                }

                return RedirectToAction("Index");
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                TurmaViewModel result = await GetTurma(id);
                return View(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar([Bind("Id, Nome, Ano")] TurmaViewModel turma)
        {
            try
            {
                ByteArrayContent byteContent = ConverterObjetoByte(turma);

                string url = ENDPOINT;
                HttpResponseMessage response = await httpClient.PutAsync(url, byteContent);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação de post.");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> Inativar(int id)
        {
            try
            {
                TurmaViewModel result = await GetTurma(id);
                return View(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Inativar(string id)
        {
            int idTurma = Convert.ToInt32(id);
            try
            {
                string url = $"{ENDPOINT}{idTurma}";
                HttpResponseMessage response = await httpClient.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação de post.");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Métodos Auxiliares
        private async Task<TurmaViewModel> GetTurma(int id)
        {
            try
            {
                TurmaViewModel result = null;

                string url = $"{ENDPOINT}{id}";
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<TurmaViewModel>(content);
                }

                return result;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
        private ByteArrayContent ConverterObjetoByte<T>(T objeto)
        {
            string json = JsonConvert.SerializeObject(objeto);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return byteContent;
        }
        #endregion
    }
}
