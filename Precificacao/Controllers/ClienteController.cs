using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Canducci.Zip;
using CrossCutting;

namespace ERP_Condominios_Solution.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly ITipoPessoaAppService tpApp;
        private readonly IClienteCnpjAppService fcnpjApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        CLIENTE objetoForn = new CLIENTE();
        CLIENTE objetoFornAntes = new CLIENTE();
        List<CLIENTE> listaMasterForn = new List<CLIENTE>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public ClienteController(IClienteAppService fornApps, ILogAppService logApps, ITipoPessoaAppService tpApps, IClienteCnpjAppService fcnpjApps, IConfiguracaoAppService confApps)
        {
            fornApp = fornApps;
            logApp = logApps;
            tpApp = tpApps;
            fcnpjApp = fcnpjApps;
            confApp = confApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["Clientes"] = fornApp.GetAllItens(idAss);
            if (nome != null)
            {
                List<CLIENTE> lstForn = ((List<CLIENTE>)Session["Clientes"]).Where(x => x.CLIE_NM_NOME != null && x.CLIE_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();
                if (lstForn == null || lstForn.Count == 0)
                {
                    isRazao = 1;
                    lstForn = ((List<CLIENTE>)Session["Clientes"]).Where(x => x.CLIE_NM_RAZAO_SOCIAL != null).ToList<CLIENTE>();
                    lstForn = lstForn.Where(x => x.CLIE_NM_RAZAO_SOCIAL.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();
                }

                if (lstForn != null)
                {
                    foreach (var item in lstForn)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.CLIE_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.CLIE_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.CLIE_NM_NOME + " (" + item.CLIE_NM_RAZAO_SOCIAL + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }

            return Json(listResult);
        }

        public ActionResult EnviarSmsCliente(Int32 id, String mensagem)
        {
            try
            {
                CLIENTE forn = fornApp.GetById(id);
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica existencia prévia
                if (forn == null)
                {
                    Session["MensCliente"] = 8;
                    return RedirectToAction("MontarTelaCliente");
                }

                // Criticas
                if (forn.CLIE_NR_CELULAR == null)
                {
                    Session["MensCliente"] = 9;
                    return RedirectToAction("MontarTelaCliente");
                }

                // Processa SMS
                CONFIGURACAO conf = confApp.GetItemById(idAss);

                // Monta token
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = mensagem;

                // Prepara corpo do SMS e trata link
                StringBuilder str = new StringBuilder();
                str.AppendLine(mensagem);
                String body = str.ToString();
                String smsBody = body;
                String erro = null;

                // inicia processo
                String resposta = String.Empty;

                // Monta destinatarios
                try
                {
                    String listaDest = "55" + Regex.Replace(forn.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                    httpWebRequest.Headers["Authorization"] = auth;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    String customId = Cryptography.GenerateRandomPassword(8);
                    String data = String.Empty;
                    String json = String.Empty;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"GED On-Line\"}]}");
                        streamWriter.Write(json);
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        resposta = result;
                    }
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                }
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                Session["MensCliente"] = 11;
                Session["MensSMSErro"] = ex.Message;
                return RedirectToAction("MontarTelaCliente");
            }
        }

        [HttpPost]
        public JsonResult PesquisaCNPJ(string cnpj)
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lstQs = new List<CLIENTE_QUADRO_SOCIETARIO>();
            var url = "https://api.cnpja.com.br/companies/" + Regex.Replace(cnpj, "[^0-9]", "");
            String json = String.Empty;

            WebRequest request = WebRequest.Create(url);
            request.Headers["Authorization"] = "df3c411d-bb44-41eb-9304-871c45d72978-cd751b62-ff3d-4421-a9d2-b97e01ca6d2b";
            try
            {
                WebResponse response = request.GetResponse();
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }

                var jObject = JObject.Parse(json);
                if (jObject["membership"].Count() == 0)
                {
                    CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();
                    qs.CLIENTE = new CLIENTE();
                    qs.CLIENTE.CLIE_NM_RAZAO_SOCIAL = jObject["name"].ToString();
                    qs.CLIENTE.CLIE_NM_NOME = jObject["alias"].ToString() == "" ? jObject["name"].ToString() : jObject["alias"].ToString();
                    qs.CLIENTE.CLIE_NR_CEP = jObject["address"]["zip"].ToString();
                    qs.CLIENTE.CLIE_NM_ENDERECO = jObject["address"]["street"].ToString() + ", " + jObject["address"]["number"].ToString();
                    qs.CLIENTE.CLIE_NM_BAIRRO = jObject["address"]["neighborhood"].ToString();
                    qs.CLIENTE.CLIE_NM_CIDADE = jObject["address"]["city"].ToString();
                    qs.CLIENTE.UF_CD_ID = fornApp.GetAllUF().Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                    qs.CLIENTE.CLIE_NR_INSCRICAO_ESTADUAL = jObject["sintegra"]["home_state_registration"].ToString();
                    qs.CLIENTE.CLIE_NR_TELEFONE = jObject["phone"].ToString();
                    qs.CLIENTE.CLIE_NM_EMAIL = jObject["email"].ToString();
                    qs.CLIENTE.CLIE_NM_SITUACAO = jObject["registration"]["status"].ToString();
                    qs.FOQS_IN_ATIVO = 0;
                    lstQs.Add(qs);
                }
                else
                {
                    foreach (var s in jObject["membership"])
                    {
                        CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();

                        qs.CLIENTE = new CLIENTE();
                        qs.CLIENTE.CLIE_NM_RAZAO_SOCIAL = jObject["name"].ToString();
                        qs.CLIENTE.CLIE_NM_NOME = jObject["alias"].ToString() == "" ? jObject["name"].ToString() : jObject["alias"].ToString();
                        qs.CLIENTE.CLIE_NR_CEP = jObject["address"]["zip"].ToString();
                        qs.CLIENTE.CLIE_NM_ENDERECO = jObject["address"]["street"].ToString() + ", " + jObject["address"]["number"].ToString();
                        qs.CLIENTE.CLIE_NM_BAIRRO = jObject["address"]["neighborhood"].ToString();
                        qs.CLIENTE.CLIE_NM_CIDADE = jObject["address"]["city"].ToString();
                        qs.CLIENTE.UF_CD_ID = fornApp.GetAllUF().Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                        qs.CLIENTE.CLIE_NR_INSCRICAO_ESTADUAL = jObject["sintegra"]["home_state_registration"].ToString();
                        qs.CLIENTE.CLIE_NR_TELEFONE = jObject["phone"].ToString();
                        qs.CLIENTE.CLIE_NM_EMAIL = jObject["email"].ToString();
                        qs.CLIENTE.CLIE_NM_SITUACAO = jObject["registration"]["status"].ToString();
                        qs.FOQS_NM_QUALIFICACAO = s["role"]["description"].ToString();
                        qs.FOQS_NM_NOME = s["name"].ToString();

                        // CNPJá não retorna esses valores
                        qs.FOQS_NM_PAIS_ORIGEM = String.Empty;
                        qs.FOQS_NOME_REP_LEGAL = String.Empty;
                        qs.FOQS_QUALIFICACAO_REP_LEGAL = String.Empty;
                        lstQs.Add(qs);
                    }
                }
                return Json(lstQs);
            }
            catch (WebException ex)
            {
                var hash = new Hashtable();
                hash.Add("status", "ERROR");

                if ((ex.Response as HttpWebResponse)?.StatusCode.ToString() == "BadRequest")
                {
                    hash.Add("public", 1);
                    hash.Add("message", "CNPJ inválido");
                    return Json(hash);
                }
                if ((ex.Response as HttpWebResponse)?.StatusCode.ToString() == "NotFound")
                {
                    hash.Add("public", 1);
                    hash.Add("message", "O CNPJ consultado não está registrado na Receita Federal");
                    return Json(hash);
                }
                else
                {
                    hash.Add("public", 1);
                    hash.Add("message", ex.Message);
                    return Json(hash);
                }
            }
        }

        private List<CLIENTE_QUADRO_SOCIETARIO> PesquisaCNPJ(CLIENTE fornecedor)
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lstQs = new List<CLIENTE_QUADRO_SOCIETARIO>();

            var url = "https://api.cnpja.com.br/companies/" + Regex.Replace(fornecedor.CLIE_NR_CNPJ, "[^0-9]", "");
            String json = String.Empty;

            WebRequest request = WebRequest.Create(url);
            request.Headers["Authorization"] = "df3c411d-bb44-41eb-9304-871c45d72978-cd751b62-ff3d-4421-a9d2-b97e01ca6d2b";

            WebResponse response = request.GetResponse();

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
            {
                json = reader.ReadToEnd();
            }

            var jObject = JObject.Parse(json);

            foreach (var s in jObject["membership"])
            {
                CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();

                qs.FOQS_NM_QUALIFICACAO = s["role"]["description"].ToString();
                qs.FOQS_NM_NOME = s["name"].ToString();
                qs.CLIE_CD_ID = fornecedor.CLIE_CD_ID;

                // CNPJá não retorna esses valores
                qs.FOQS_NM_PAIS_ORIGEM = String.Empty;
                qs.FOQS_NOME_REP_LEGAL = String.Empty;
                qs.FOQS_QUALIFICACAO_REP_LEGAL = String.Empty;
                lstQs.Add(qs);
            }
            return lstQs;
        }

        [HttpGet]
        public ActionResult MontarTelaCliente()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaCliente"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaCliente"] = listaMasterForn;
            }
            ViewBag.Listas = (List<CLIENTE>)Session["ListaCliente"];
            ViewBag.Title = "Clientes";
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Tipos = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["IncluirForn"] = 0;

            // Indicadores
            ViewBag.Clientes = ((List<CLIENTE>)Session["ListaCliente"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Inativos = fornApp.GetAllItensAdm(idAss).Where(p => p.CLIE_IN_ATIVO == 0).ToList().Count;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");

            if (Session["MensCliente"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCliente"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0027", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 8)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0053", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 9)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 11)
                {
                    String msg = ERP_Condominios_Resource.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture) + " - " + (String)Session["MensSMSError"];
                    ModelState.AddModelError("", msg);
                }
                if ((Int32)Session["MensCliente"] == 10)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0052", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new CLIENTE();
            objetoForn.CLIE_IN_ATIVO = 1;
            Session["MensCliente"] = 0;
            Session["VoltaCliente"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCliente"] = null;
            Session["FiltroCliente"] = null;
            if ((Int32)Session["VoltaFornecedor"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        public ActionResult MostrarTudoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroCliente"] = null;
            Session["ListaCliente"] = listaMasterForn;
            if ((Int32)Session["VoltaCliente"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpPost]
        public ActionResult FiltrarCliente(CLIENTE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CLIENTE> listaObj = new List<CLIENTE>();
                Session["FiltroCliente"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.CACL_CD_ID, item.CLIE_NM_RAZAO_SOCIAL, item.CLIE_NM_NOME, item.CLIE_NR_CPF, item.CLIE_NR_CNPJ, item.CLIE_NM_EMAIL, item.CLIE_NM_CIDADE, item.UF_CD_ID, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCliente"] = 1;
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaCliente"] = listaObj;
                if ((Int32)Session["VoltaCliente"] == 2)
                {
                    return RedirectToAction("VerCardsCliente");
                }
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCliente");
            }
        }

        public ActionResult VoltarBaseCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCliente"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpGet]
        public ActionResult IncluirCliente()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Tipos = new SelectList(fornApp.GetAllTiposPessoa(), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["VoltaProp"] = 4;

            // Prepara view
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            vm.CLIE_DT_CRIACAO = DateTime.Today;
            vm.CLIE_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCliente(ClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Tipos = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCliente"] = 3;
                        return RedirectToAction("MontarTelaCliente", "Cliente");
                    }

                    // Carrega foto e processa alteracao
                    if (item.CLIE_AQ_FOTO == null)
                    {
                        item.CLIE_AQ_FOTO = "~/Imagens/Base/FotoBase.jpg";
                        volta = fornApp.ValidateEdit(item, item, usuarioLogado);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Cliente/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Cliente/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterForn = new List<CLIENTE>();
                    Session["ListaCliente"] = null;
                    Session["IncluirForn"] = 1;
                    Session["idCliente"] = item.CLIE_CD_ID;
                    Session["Clientes"] = fornApp.GetAllItens(idAss);
                    if (item.TIPE_CD_ID == 2)
                    {
                        var lstQs = PesquisaCNPJ(item);

                        foreach (var qs in lstQs)
                        {
                            Int32 voltaQS = fcnpjApp.ValidateCreate(qs, usuarioLogado);
                        }
                    }

                    Session["IdVolta"] = item.CLIE_CD_ID;
                    if (Session["FileQueueCliente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCliente"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueCliente(file);
                            }
                            else
                            {
                                UploadFotoQueueCliente(file);
                            }
                        }

                        Session["FileQueueCliente"] = null;
                    }
                    if ((Int32)Session["VoltaCliente"] == 2)
                    {
                        return RedirectToAction("IncluirCliente");
                    }
                    return RedirectToAction("MontarTelaCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCliente(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Tipos = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Incluir = (Int32)Session["IncluirForn"];

            if ((Int32)Session["MensCliente"] == 5)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensCliente"] == 6)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            CLIENTE item = fornApp.GetItemById(id);
            ViewBag.QuadroSoci = fcnpjApp.GetByFornecedor(item);
            objetoFornAntes = item;
            Session["Cliente"] = item;
            Session["IdVolta"] = id;
            Session["IdCliente"] = id;
            Session["VoltaCEP"] = 1;
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCliente(ClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Tipos = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<CLIENTE>();
                    Session["ListaCliente"] = null;
                    Session["IncluirForn"] = 0;
                    if ((Int32)Session["VoltaCliente"] == 2)
                    {
                        return RedirectToAction("VerCardsCliente");
                    }
                    return RedirectToAction("MontarTelaCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerCliente(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Incluir = (Int32)Session["IncluirForn"];

            CLIENTE item = fornApp.GetItemById(id);
            ViewBag.QuadroSoci = fcnpjApp.GetByFornecedor(item);
            objetoFornAntes = item;
            Session["Cliente"] = item;
            Session["IdVolta"] = id;
            Session["IdCliente"] = id;
            Session["VoltaCEP"] = 1;
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirCliente(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente", "Cliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CLIENTE item = fornApp.GetItemById(id);
            objetoFornAntes = (CLIENTE)Session["Cliente"];
            item.CLIE_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCliente"] = 4;
                return RedirectToAction("MontarTelaCliente", "Cliente");
            }
            listaMasterForn = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            Session["FiltroCliente"] = null;
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpGet]
        public ActionResult ReativarCliente(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente", "Cliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CLIENTE item = fornApp.GetItemById(id);
            objetoFornAntes = (CLIENTE)Session["Cliente"];
            item.CLIE_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateDelete(item, usuario);
            listaMasterForn = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            Session["FiltroCliente"] = null;
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpGet]
        public ActionResult VerCardsCliente()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaCliente"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaCliente"] = listaMasterForn;
            }
            ViewBag.Listas = (List<CLIENTE>)Session["ListaCliente"];
            ViewBag.Title = "Clientes";
            ViewBag.Cats = new SelectList(fornApp.GetAllTipos(idAss).OrderBy(x => x.CACL_NM_NOME), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Tipos = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");

            // Indicadores
            ViewBag.Fornecedores = ((List<CLIENTE>)Session["ListaCliente"]).Count;

            // Abre view
            objetoForn = new CLIENTE();
            Session["VoltaCliente"] = 2;
            return View(objetoForn);
        }

        [HttpGet]
        public ActionResult VerAnexoCliente(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CLIENTE_ANEXO item = fornApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }

        public FileResult DownloadCliente(Int32 id)
        {
            CLIENTE_ANEXO item = fornApp.GetAnexoById(id);
            String arquivo = item.CLAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            else if (arquivo.Contains(".jpeg"))
            {
                contentType = "image/jpeg";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();

            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }

                queue.Add(f);
            }

            Session["FileQueueCliente"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueCliente(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCliente"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }

            CLIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Cliente/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CLIENTE_ANEXO foto = new CLIENTE_ANEXO();
            foto.CLAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CLAN_DT_ANEXO = DateTime.Today;
            foto.CLAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.CLAN_IN_TIPO = tipo;
            foto.CLAN_NM_TITULO = fileName;
            foto.CLIE_CD_ID = item.CLIE_CD_ID;

            item.CLIENTE_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpPost]
        public ActionResult UploadFileCliente(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCliente"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }

            CLIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Cliente/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CLIENTE_ANEXO foto = new CLIENTE_ANEXO();
            foto.CLAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CLAN_DT_ANEXO = DateTime.Today;
            foto.CLAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.CLAN_IN_TIPO = tipo;
            foto.CLAN_NM_TITULO = fileName;
            foto.CLIE_CD_ID = item.CLIE_CD_ID;

            item.CLIENTE_ANEXO.Add(foto);
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueCliente(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCliente"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }
            CLIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Cliente/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.CLIE_AQ_FOTO = "~" + caminho + fileName;
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            listaMasterForn = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpPost]
        public ActionResult UploadFotoCliente(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCliente"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }
            CLIENTE item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Cliente/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.CLIE_AQ_FOTO = "~" + caminho + fileName;
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes);
            listaMasterForn = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult EditarContatoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_CONTATO item = fornApp.GetContatoById(id);
            objetoFornAntes = (CLIENTE)Session["Cliente"];
            ClienteContatoViewModel vm = Mapper.Map<CLIENTE_CONTATO, ClienteContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContatoFornecedor(ClienteContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CLIENTE_CONTATO item = Mapper.Map<ClienteContatoViewModel, CLIENTE_CONTATO>(vm);
                    Int32 volta = fornApp.ValidateEditContato(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirContatoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE_CONTATO item = fornApp.GetContatoById(id);
            objetoFornAntes = (CLIENTE)Session["Cliente"];
            item.CLCO_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult ReativarContatoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE_CONTATO item = fornApp.GetContatoById(id);
            objetoFornAntes = (CLIENTE)Session["Cliente"];
            item.CLCO_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult IncluirContatoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CLIENTE_CONTATO item = new CLIENTE_CONTATO();
            ClienteContatoViewModel vm = Mapper.Map<CLIENTE_CONTATO, ClienteContatoViewModel>(item);
            vm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            vm.CLCO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContatoCliente(ClienteContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE_CONTATO item = Mapper.Map<ClienteContatoViewModel, CLIENTE_CONTATO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreateContato(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public JsonResult PesquisaCEP_Javascript(String cep, int tipoEnd)
        {
            // Chama servico ECT
            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(cep);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza
            var hash = new Hashtable();

            if (tipoEnd == 1)
            {
                hash.Add("CLIE_NM_ENDERECO", end.Address + "/" + end.Complement);
                hash.Add("CLIE_NM_BAIRRO", end.District);
                hash.Add("CLIE_NM_CIDADE", end.City);
                hash.Add("UF_CD_ID", fornApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("CLIE_NR_CEP", cep);
            }

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        public ActionResult GerarRelatorioLista(Int32? tipo)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ClienteLista" + "_" + data + ".pdf";
            List<CLIENTE> lista = new List<CLIENTE>();
            String titulo = String.Empty;
            if (tipo == null)
            {
                titulo = "Clientes - Listagem";
                lista = (List<CLIENTE>)Session["ListaCliente"];
            }
            else if (tipo == 2)
            {
                titulo = "Clientes Inativos - Listagem";
                lista = (List<CLIENTE>)Session["ListaClientesInativos"];
            }

            CLIENTE filtro = (CLIENTE)Session["FiltroCliente"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/ecm_favicon.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(titulo, meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 150f, 50f, 50f, 20f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Clientes selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CPF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CNPJ", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("UF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CLIENTE item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_CLIENTE.CACL_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CLIE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CLIE_NR_CPF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_CPF, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CLIE_NR_CNPJ != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_CNPJ, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CLIE_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CLIE_NR_CELULAR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_CELULAR, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CLIE_NM_CIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NM_CIDADE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.UF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.UF.UF_SG_SIGLA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CACL_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CACL_CD_ID;
                    ja = 1;
                }
                if (filtro.CLIE_CD_ID > 0)
                {
                    CLIENTE cli = fornApp.GetItemById(filtro.CLIE_CD_ID);
                    if (ja == 0)
                    {
                        parametros += "Nome: " + cli.CLIE_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + cli.CLIE_NM_NOME;
                    }
                }
                if (filtro.CLIE_NR_CPF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CPF: " + filtro.CLIE_NR_CPF;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CPF: " + filtro.CLIE_NR_CPF;
                    }
                }
                if (filtro.CLIE_NR_CNPJ != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CNPJ: " + filtro.CLIE_NR_CNPJ;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CNPJ: " + filtro.CLIE_NR_CNPJ;
                    }
                }
                if (filtro.CLIE_NM_EMAIL != null)
                {
                    if (ja == 0)
                    {
                        parametros += "E-Mail: " + filtro.CLIE_NM_EMAIL;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e E-Mail: " + filtro.CLIE_NM_EMAIL;
                    }
                }
                if (filtro.CLIE_NM_CIDADE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Cidade: " + filtro.CLIE_NM_CIDADE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Cidade: " + filtro.CLIE_NM_CIDADE;
                    }
                }
                if (filtro.UF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "UF: " + filtro.UF.UF_SG_SIGLA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e UF: " + filtro.UF.UF_SG_SIGLA;
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            if (tipo == null)
            {
                return RedirectToAction("MontarTelaCliente");
            }
            else if (tipo == 2)
            {
                return RedirectToAction("VerClientesInativos");
            }

            return RedirectToAction("MontarTelaCliente");
        }

        public ActionResult VerClientesInativos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (Session["ListaClientesInativo"] == null)
            {
                Session["ListaClientesInativo"] = fornApp.GetAllItensAdm(idAss).Where(x => x.CLIE_IN_ATIVO == 0).ToList();
            }
            if (((List<CLIENTE>)Session["ListaClientesInativo"]).Count == 0)
            {
                Session["ListaClientesInativo"] = fornApp.GetAllItensAdm(idAss).Where(x => x.CLIE_IN_ATIVO == 0).ToList();
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Inativos = fornApp.GetAllItensAdm(idAss).Where(p => p.CLIE_IN_ATIVO == 0).ToList().Count;
            ViewBag.Listas = (List<CLIENTE>)Session["ListaClientesInativo"];
            ViewBag.Title = "Clientes";
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");

            if (Session["MensClienteInativos"] != null)
            {
                if ((Int32)Session["MensClienteInativos"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensClienteInativos"] = 0;
                }
            }

            // Abre view
            CLIENTE forn = new CLIENTE();
            return View(forn);
        }

        [HttpPost]
        public ActionResult FiltrarInativos(CLIENTE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CLIENTE> listaObj = new List<CLIENTE>();
                Int32 volta = fornApp.ExecuteFilter(null, null, item.CLIE_NM_NOME, null, null, null, item.CLIE_NM_CIDADE, item.UF_CD_ID, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensClienteInativos"] = 1;
                }

                // Sucesso
                Session["ListaClientesInativo"] = listaObj;
                return RedirectToAction("VerClientesInativos");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerClientesInativos");
            }
        }

        public ActionResult RetirarFiltroInativos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaClientesInativo"] = null;
            return RedirectToAction("VerClientesInativos");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaUnidade"] == 1)
            {
                return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
            }
            if ((Int32)Session["VoltaUnidade"] == 2)
            {
                return RedirectToAction("CarregarPortaria", "BaseAdmin");
            }
            if ((Int32)Session["VoltaUnidade"] == 3)
            {
                return RedirectToAction("CarregarSindico", "BaseAdmin");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult EnviarSMSFornecedorFormVai()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EnviarSMSClienteForm", new { id = (Int32)Session["IdCliente"] });
        }

        public ActionResult EnviarEMailClienteVai()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EnviarEMailCliente", new { id = (Int32)Session["IdCliente"] });
        }

        public ActionResult VerDocumento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("VerDocumento", "Documento", new { id = id });
        }

        [HttpGet]
        public ActionResult EnviarSMSClienteForm(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE item = fornApp.GetItemById(id);
            Session["Cliente"] = item;
            ViewBag.Fornecedor = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.CLIE_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.CLIE_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSClienteForm(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSCliente(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSCliente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cont = (CLIENTE)Session["Cliente"];

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Monta token
            String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = vm.MENS_TX_SMS;

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(vm.MENS_TX_SMS);
            if (!String.IsNullOrEmpty(vm.LINK))
            {
                if (!vm.LINK.Contains("www."))
                {
                    vm.LINK = "www." + vm.LINK;
                }
                if (!vm.LINK.Contains("http://"))
                {
                    vm.LINK = "http://" + vm.LINK;
                }
                str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                texto += "  " + vm.LINK;
            }
            String body = str.ToString();
            String smsBody = body;
            String erro = null;

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                String listaDest = "55" + Regex.Replace(cont.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult EnviarEMailCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE cont = fornApp.GetItemById(id);
            Session["Cliente"] = cont;
            ViewBag.Fornecedor = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.CLIE_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.CLIE_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailCliente(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailCliente(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailCliente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contato
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cont = (CLIENTE)Session["Cliente"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + cont.CLIE_NM_NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
            }
            String body = str.ToString();
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
            Email mensagem = new Email();
            mensagem.ASSUNTO = "Contato";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_DESTINO = cont.CLIE_NM_EMAIL;
            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;

            // Envia mensagem
            try
            {
                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
            }
            catch (Exception ex)
            {
                String erro = ex.Message;
            }
            return 0;
        }
    }
}