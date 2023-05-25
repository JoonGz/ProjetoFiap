using FiapSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace FiapSystem.Controllers
{
    public class AlunosController : Controller
    {
        #region Propriedades
        private readonly string ENDPOINT = "";
        private readonly HttpClient httpClient = null;
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        public AlunosController(IConfiguration configuration)
        {
            _configuration = configuration;
            ENDPOINT = _configuration["AppConfig:Endpoints:Url_Api_Alunos"];
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ENDPOINT);
                     
        }
        #endregion

        #region CRUD Alunos
        public async Task<IActionResult> Index()
        {
            try
            {
                List<AlunoViewModel> Alunos = null;

                HttpResponseMessage response = await httpClient.GetAsync(ENDPOINT);

                if (response.IsSuccessStatusCode) 
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Alunos = JsonConvert.DeserializeObject<List<AlunoViewModel>>(content);
                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação");
                }

                return View(Alunos);
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
                AlunoViewModel result = await GetAluno(id);
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
        public async Task<IActionResult> Novo([Bind("Nome, Usuario, Senha")]AlunoViewModel aluno)
        {
            try
            {
                ByteArrayContent byteContent = ConverterObjetoByte(aluno);

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
                AlunoViewModel result = await GetAluno(id);
                return View(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar([Bind("Id, Nome, Usuario, Senha")] AlunoViewModel aluno)
        {
            try
            {
                ByteArrayContent byteContent = ConverterObjetoByte(aluno);

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
                AlunoViewModel result = await GetAluno(id);
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
            int idAluno = Convert.ToInt32(id);
            try
            {
                string url = $"{ENDPOINT}{idAluno}";
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
        private async Task<AlunoViewModel> GetAluno(int id)
        {
            try
            {
                AlunoViewModel result = null;

                string url = $"{ENDPOINT}{id}";
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<AlunoViewModel>(content);
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
