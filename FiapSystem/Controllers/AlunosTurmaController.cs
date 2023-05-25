using FiapSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoTurmasFiap.Entities;
using System.Data;
using System.Text;

namespace FiapSystem.Controllers
{
    public class AlunosTurmaController : Controller
    {
        #region Propriedades
        private readonly string ENDPOINT = "";
        private readonly HttpClient httpClient = null;
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        public AlunosTurmaController(IConfiguration configuration)
        {
            _configuration = configuration;
            ENDPOINT = _configuration["AppConfig:Endpoints:Url_Api_AlunosTurma"];
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ENDPOINT);
                     
        }
        #endregion

        #region CRUD Alunos Turma
        public async Task<IActionResult> Index()
        {
            try
            {
                List<AlunoTurmaViewModel> AlunosTurma = null;

                HttpResponseMessage response = await httpClient.GetAsync(ENDPOINT);

                if (response.IsSuccessStatusCode) 
                {
                    string content = await response.Content.ReadAsStringAsync();
                    AlunosTurma = JsonConvert.DeserializeObject<List<AlunoTurmaViewModel>>(content);
                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação");
                }

                return View(AlunosTurma);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw ex;
            }
        }

        public async Task<IActionResult> Detalhe(int idTurma)
        {
            try
            {
                List<AlunoTurmaViewModel> AlunosTurma = null;

                string url = $"{ENDPOINT}{idTurma}";
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    AlunosTurma = JsonConvert.DeserializeObject<List<AlunoTurmaViewModel>>(content);
                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação");
                }

                return View(AlunosTurma);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw ex;
            }
        }

        public async Task<IActionResult> Novo()
        {
            ViewBag.Turmas = await listarTurmas();
            ViewBag.Alunos = await listarAlunos();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Novo([Bind("Aluno_id, Turma_id")]AlunoTurmaViewModel alunoTurma)
        {
            try
            {
                ByteArrayContent byteContent = ConverterObjetoByte(alunoTurma);

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

        public async Task<IActionResult> Editar(int idAluno, int idTurma)
        {
            try
            {
                ViewBag.Turmas = await listarTurmas();
                ViewBag.Alunos = await listarAlunos();

                AlunoTurmaViewModel obj = new AlunoTurmaViewModel();
                obj.Aluno_id = idAluno;
                obj.Turma_id = idTurma;

                return View(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar([Bind("Aluno_id, Turma_id, Aluno_idOld, Turma_idOld")] AlunoTurmaViewModel alunoTurma)
        {
            try
            {
                ByteArrayContent byteContent = ConverterObjetoByte(alunoTurma);

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

        public async Task<IActionResult> Inativar(int idAluno, int idTurma)
        {
            try
            {
                ViewBag.Turmas = await listarTurmas();
                ViewBag.Alunos = await listarAlunos();

                AlunoTurmaViewModel obj = new AlunoTurmaViewModel();
                obj.Aluno_id = idAluno;
                obj.Turma_id = idTurma;

                return View(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Inativar(string Aluno_id, string Turma_id)
        {
            int idAluno = Convert.ToInt32(Aluno_id);
            int idTurma = Convert.ToInt32(Turma_id);
            try
            {
                string url = $"{ENDPOINT}{idAluno}/{idTurma}";
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
        private ByteArrayContent ConverterObjetoByte<T>(T objeto)
        {
            string json = JsonConvert.SerializeObject(objeto);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return byteContent;
        }
        private async Task<List<TurmaViewModel>> listarTurmas()
        {
            try
            {
                List<TurmaViewModel> Turmas = null;

                HttpResponseMessage response = await httpClient.GetAsync(_configuration["AppConfig:Endpoints:Url_Api_Turmas"]);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Turmas = JsonConvert.DeserializeObject<List<TurmaViewModel>>(content);
                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação");
                }

                return Turmas;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw ex;
            }
        }

        private async Task<List<AlunoViewModel>> listarAlunos()
        {
            try
            {
                List<AlunoViewModel> Alunos = null;

                HttpResponseMessage response = await httpClient.GetAsync(_configuration["AppConfig:Endpoints:Url_Api_Alunos"]);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Alunos = JsonConvert.DeserializeObject<List<AlunoViewModel>>(content);
                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação");
                }

                return Alunos;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw ex;
            }
        }
        #endregion
    }
}
