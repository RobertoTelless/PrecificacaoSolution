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
    public class FormaPagRecController : Controller
    {
        private readonly IFormaPagRecAppService fpApp;
        private readonly IContaBancariaAppService cbApp;

        private String msg;
        private Exception exception;
        FORMA_PAGTO_RECTO objetoFP = new FORMA_PAGTO_RECTO();
        FORMA_PAGTO_RECTO objetoFPAntes = new FORMA_PAGTO_RECTO();
        List<FORMA_PAGTO_RECTO> listaMasterFP = new List<FORMA_PAGTO_RECTO>();
        String extensao;

        public FormaPagRecController(IFormaPagRecAppService fpApps, IContaBancariaAppService cbApps)
        {
            fpApp = fpApps;
            cbApp = cbApps;
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

        [HttpGet]
        public ActionResult MontarTelaFormaPagamento()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
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

            // Carrega listas
            if (Session["ListaForma"] == null)
            {
                listaMasterFP = fpApp.GetAllItens(idAss);
                Session["ListaForma"] = listaMasterFP;
            }

            ViewBag.Listas = (List<FORMA_PAGTO_RECTO>)Session["ListaForma"];
            ViewBag.Title = "Formas de Pagamento";
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");

            // Indicadores
            ViewBag.Itens = listaMasterFP.Count;

            if (Session["MensFormaPag"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensFormaPag"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFormaPag"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0173", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFormaPag"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0172", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoFP = new FORMA_PAGTO_RECTO();
            return View(objetoFP);
        }

        public ActionResult RetirarFiltroFormaPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaForma"] = null;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        public ActionResult MostrarTudoFormaPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFP = fpApp.GetAllItensAdm(idAss);
            Session["ListaForma"] = listaMasterFP;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        public ActionResult VoltarBaseFormaPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["Formas"] = fpApp.GetAllItens(idAss);
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        [HttpGet]
        public ActionResult IncluirFormaPagamento(Int32? id)
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
                    Session["MensFormaPag"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            // Prepara view
            FORMA_PAGTO_RECTO item = new FORMA_PAGTO_RECTO();
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            Session["VoltaPop"] = id.Value;
            FormaPagRecViewModel vm = Mapper.Map<FORMA_PAGTO_RECTO, FormaPagRecViewModel>(item);
            vm.FOPR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirFormaPagamento(FormaPagRecViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Contas = new SelectList(cbApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMA_PAGTO_RECTO item = Mapper.Map<FormaPagRecViewModel, FORMA_PAGTO_RECTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fpApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensFormaPag"] = 3;
                        return RedirectToAction("MontarTelaFormaPagamento");
                    }

                    // Sucesso
                    listaMasterFP = new List<FORMA_PAGTO_RECTO>();
                    Session["ListaForma"] = null;
                    return RedirectToAction("MontarTelaFormaPagamento");
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
        public ActionResult EditarFormaPagamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensFormaPag"] = 2;
                    return RedirectToAction("MontarTelaFormaPagamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            FORMA_PAGTO_RECTO item = fpApp.GetItemById(id);
            objetoFPAntes = item;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            Session["Forma"] = item;
            Session["IdForma"] = id;
            FormaPagRecViewModel vm = Mapper.Map<FORMA_PAGTO_RECTO, FormaPagRecViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFormaPagamento(FormaPagRecViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pagamento", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Ambos", Value = "3" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FORMA_PAGTO_RECTO item = Mapper.Map<FormaPagRecViewModel, FORMA_PAGTO_RECTO>(vm);
                    Int32 volta = fpApp.ValidateEdit(item, objetoFPAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFP = new List<FORMA_PAGTO_RECTO>();
                    Session["ListaForma"] = null;
                    return RedirectToAction("MontarTelaFormaPagamento");
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
        public ActionResult ExcluirFormaPagamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensFormaPag"] = 2;
                    return RedirectToAction("MontarTelaFormaPagamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            FORMA_PAGTO_RECTO item = fpApp.GetItemById(id);
            objetoFPAntes = (FORMA_PAGTO_RECTO)Session["Forma"];
            item.FOPR_IN_ATIVO = 0;
            Int32 volta = fpApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensFormaPag"] = 4;
                return RedirectToAction("MontarTelaFormaPagamento");
            }
            listaMasterFP = new List<FORMA_PAGTO_RECTO>();
            Session["ListaForma"] = null;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

        [HttpGet]
        public ActionResult ReativarFormaPagamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensFormaPag"] = 2;
                    return RedirectToAction("MontarTelaFormaPagamento");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            FORMA_PAGTO_RECTO item = fpApp.GetItemById(id);
            objetoFPAntes = (FORMA_PAGTO_RECTO)Session["Forma"];
            item.FOPR_IN_ATIVO = 1;
            Int32 volta = fpApp.ValidateReativar(item, usuario);
            listaMasterFP = new List<FORMA_PAGTO_RECTO>();
            Session["ListaForma"] = null;
            return RedirectToAction("MontarTelaFormaPagamento");
        }

    }
}