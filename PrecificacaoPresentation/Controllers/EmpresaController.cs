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

namespace ERP_Condominios_Solution.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IEmpresaAppService baseApp;
        private String msg;
        private Exception exception;
        EMPRESA objeto = new EMPRESA();
        EMPRESA objetoAntes = new EMPRESA();
        List<EMPRESA> listaMaster = new List<EMPRESA>();
        String extensao;

        public EmpresaController(IEmpresaAppService baseApps)
        {
            baseApp = baseApps;
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

            // Indicadores
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
            ViewBag.Perfil = usuario.PERF_CD_ID;

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
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
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
    }
}