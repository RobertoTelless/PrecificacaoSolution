using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution;
using PrecificacaoPresentation.App_Start;
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
using System.Text;
using System.Net;
using CrossCutting;
using Canducci.Zip;

namespace ERP_Condominios_Solution.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IEmpresaAppService baseApp;
        private readonly IFornecedorAppService fornApp;
        private readonly IPlataformaEntregaAppService platApp;
        private String msg;
        private Exception exception;
        EMPRESA objeto = new EMPRESA();
        EMPRESA objetoAntes = new EMPRESA();
        List<EMPRESA> listaMaster = new List<EMPRESA>();
        String extensao;

        public EmpresaController(IEmpresaAppService baseApps, IFornecedorAppService fornApps, IPlataformaEntregaAppService platApps)
        {
            baseApp = baseApps;
            fornApp = fornApps;
            platApp = platApps;
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

        [HttpGet]
        public ActionResult MontarTelaEmpresa(Int32? id)
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
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega Dados
            objeto = baseApp.GetItemById(usuario.ASSI_CD_ID);
            Session["Empresa"] = objeto;
            ViewBag.Empresa = objeto;
            ViewBag.Title = "Empresa";
            Session["IdEmpresa"] = objeto.EMPR_CD_ID;

            // Indicadores
            ViewBag.Regimes = new SelectList(baseApp.GetAllRegimes(), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Maquinas = new SelectList(baseApp.GetAllMaquinas(idAss), "MAQN_CD_ID", "MAQN_NM_NOME");
            List<SelectListItem> opera = new List<SelectListItem>();
            opera.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            opera.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Opera = new SelectList(opera, "Value", "Text");
            List<SelectListItem> comissao = new List<SelectListItem>();
            comissao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            comissao.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Comissao = new SelectList(comissao, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;


            // Mensagem
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 1)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 4)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0020", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 10)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0112", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensEmpresa"] == 11)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0113", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensEmpresa"] = 0;
            EmpresaViewModel vm = Mapper.Map<EMPRESA, EmpresaViewModel>(objeto);
            objetoAntes = objeto;
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaEmpresa(EmpresaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Regimes = new SelectList(baseApp.GetAllRegimes(), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Maquinas = new SelectList(baseApp.GetAllMaquinas(idAss), "MAQN_CD_ID", "MAQN_NM_NOME");
            List<SelectListItem> opera = new List<SelectListItem>();
            opera.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            opera.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Opera = new SelectList(opera, "Value", "Text");
            List<SelectListItem> comissao = new List<SelectListItem>();
            comissao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            comissao.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Comissao = new SelectList(comissao, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    EMPRESA item = Mapper.Map<EmpresaViewModel, EMPRESA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 10;
                        return RedirectToAction("MontarTelaEmpresa", "Empresa");
                    }
                    if (volta == 2)
                    {
                        Session["MensEmpresa"] = 11;
                        return RedirectToAction("MontarTelaEmpresa", "Empresa");
                    }

                    // Sucesso
                    listaMaster = new List<EMPRESA>();
                    Session["Empresa"] = null;
                    Session["MensEmpresa"] = 0;
                    return RedirectToAction("MontarTelaEmpresa");
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

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEmpresa");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult VerAnexoEmpresa(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoEmpresa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEmpresa");
        }

        public FileResult DownloadEmpresa(Int32 id)
        {
            EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.EMAN_AQ_ARQUIVO;
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
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpPost]
        public ActionResult UploadFileEmpresa(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensEmpresa"] = 1;
                return RedirectToAction("VoltarAnexoEmpresa");
            }

            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            EMPRESA item = baseApp.GetItemById(usuario.ASSI_CD_ID);
            var fileName = Path.GetFileName(file.FileName);

            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensEmpresa"] = 3;
                return RedirectToAction("VoltarAnexoEmpresa");
            }

            String caminho = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Empresa/" + item.ASSI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            EMPRESA_ANEXO foto = new EMPRESA_ANEXO();
            foto.EMAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.EMAN_DT_ANEXO = DateTime.Today;
            foto.EMAN_IN_ATIVO = 1;
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
            foto.EMAN_IN_TIPO = tipo;
            foto.EMAN_NM_TITULO = fileName;
            foto.EMPR_CD_ID = item.EMPR_CD_ID;

            item.EMPRESA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoEmpresa");
        }

        [HttpGet]
        public ActionResult ExcluirEmpresaMaquina(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            EMPRESA_MAQUINA item = baseApp.GetMaquinaById(id);
            item.EMMA_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditMaquina(item);
            return RedirectToAction("VoltarAnexoEmpresa");
        }

        [HttpGet]
        public ActionResult ReativarEmpresaMaquina(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            EMPRESA_MAQUINA item = baseApp.GetMaquinaById(id);
            item.EMMA_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditMaquina(item);
            return RedirectToAction("VoltarAnexoEmpresa");
        }

        [HttpGet]
        public ActionResult ExcluirEmpresaPlataforma(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            EMPRESA_PLATAFORMA item = baseApp.GetPlataformaById(id);
            item.EMPL_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditPlataforma(item);
            return RedirectToAction("VoltarAnexoEmpresa");
        }

        [HttpGet]
        public ActionResult ReativarEmpresaPlataforma(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            EMPRESA_PLATAFORMA item = baseApp.GetPlataformaById(id);
            item.EMPL_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditPlataforma(item);
            return RedirectToAction("VoltarAnexoEmpresa");
        }

        [HttpGet]
        public ActionResult IncluirEmpresaMaquina()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 20)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0180", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            ViewBag.Maquinas = new SelectList(baseApp.GetAllMaquinas(idAss).OrderBy(p => p.MAQN_NM_NOME), "MAQN_CD_ID", "MAQN_NM_EXIBE");
            EMPRESA_MAQUINA item = new EMPRESA_MAQUINA();
            EmpresaMaquinaViewModel vm = Mapper.Map<EMPRESA_MAQUINA, EmpresaMaquinaViewModel>(item);
            vm.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
            vm.EMMA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEmpresaMaquina(EmpresaMaquinaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Maquinas = new SelectList(baseApp.GetAllMaquinas(idAss).OrderBy(p => p.MAQN_NM_NOME), "MAQN_CD_ID", "MAQN_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    EMPRESA_MAQUINA item = Mapper.Map<EmpresaMaquinaViewModel, EMPRESA_MAQUINA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateMaquina(item, idAss);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 20;
                        return RedirectToAction("IncluirEmpresaMaquina", "Empresa");
                    }

                    // Retorna
                    return RedirectToAction("VoltarAnexoEmpresa");
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
        public ActionResult IncluirEmpresaPlataforma()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Mensagens
            if (Session["MensEmpresa"] != null)
            {
                if ((Int32)Session["MensEmpresa"] == 30)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0238", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            ViewBag.Plataformas = new SelectList(platApp.GetAllItens(idAss).OrderBy(p => p.PLEN_NM_NOME), "PLEN_CD_ID", "PLEN_NM_EXIBE");
            EMPRESA_PLATAFORMA item = new EMPRESA_PLATAFORMA();
            EmpresaPlataformaViewModel vm = Mapper.Map<EMPRESA_PLATAFORMA, EmpresaPlataformaViewModel>(item);
            vm.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
            vm.EMPL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEmpresaPlataforma(EmpresaPlataformaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Plataformas = new SelectList(platApp.GetAllItens(idAss).OrderBy(p => p.PLEN_NM_NOME), "PLEN_CD_ID", "PLEN_NM_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    EMPRESA_PLATAFORMA item = Mapper.Map<EmpresaPlataformaViewModel, EMPRESA_PLATAFORMA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreatePlataforma(item, idAss);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEmpresa"] = 30;
                        return RedirectToAction("IncluirEmpresaPlataforma", "Empresa");
                    }

                    // Retorna
                    return RedirectToAction("VoltarAnexoEmpresa");
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

        public JsonResult GetRegime(Int32 id)
        {
            var forn = baseApp.GetRegimeById(id);
            var hash = new Hashtable();
            hash.Add("aliquota", forn.RETR_VL_ALIQUOTA);
            return Json(hash);
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
                hash.Add("EMPR_NM_ENDERECO", end.Address);
                hash.Add("EMPR_NM_UMERO", end.Complement);
                hash.Add("EMPR_NM_BAIRRO", end.District);
                hash.Add("EMPR_NM_CIDADE", end.City);
                hash.Add("UF_CD_ID", baseApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("EMPR_NR_CEP", cep);
            }

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

    }
}