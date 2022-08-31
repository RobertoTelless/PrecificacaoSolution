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
    public class PlataformaEntregaController : Controller
    {
        private readonly IPlataformaEntregaAppService servApp;
        private readonly ILogAppService logApp;
        PLATAFORMA_ENTREGA objeto = new PLATAFORMA_ENTREGA();
        PLATAFORMA_ENTREGA objetoAntes = new PLATAFORMA_ENTREGA();
        List<PLATAFORMA_ENTREGA> listaMaster = new List<PLATAFORMA_ENTREGA>();
        String extensao;

        public PlataformaEntregaController(IPlataformaEntregaAppService servApps, ILogAppService logApps)
        {
            servApp = servApps;
            logApp = logApps;
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEmpresa", "Empresa");
        }

        [HttpGet]
        public ActionResult MontarTelaPlataformaEntrega()
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

            // Carrega listas
            if (Session["ListaPlataformaEntrega"] == null)
            {
                listaMaster = servApp.GetAllItens(idAss);
                Session["ListaPlataformaEntrega"] = listaMaster;
            }
            ViewBag.Listas = (List<PLATAFORMA_ENTREGA>)Session["ListaPlataformaEntrega"];
            ViewBag.Title = "PlataformaEntrega";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.PlataformaEntrega = ((List<PLATAFORMA_ENTREGA>)Session["ListaPlataformaEntrega"]).Count;

            if (Session["MensPlataformaEntrega"] != null)
            {
                if ((Int32)Session["MensPlataformaEntrega"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0152", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlataformaEntrega"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlataformaEntrega"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0153", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlataformaEntrega"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0124", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensPlataformaEntrega"] = 0;
            Session["VoltaPlataformaEntrega"] = 1;
            objeto = new PLATAFORMA_ENTREGA();
            return View(objeto);
        }

        public ActionResult RetirarFiltroPlataformaEntrega()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaPlataformaEntrega"] = null;
            Session["FiltroPlataformaEntrega"] = null;
            return RedirectToAction("MontarTelaPlataformaEntrega");
        }

        public ActionResult MostrarTudoPlataformaEntrega()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = servApp.GetAllItensAdm(idAss);
            Session["FiltroPlataformaEntrega"] = null;
            Session["ListaPlataformaEntrega"] = listaMaster;
            return RedirectToAction("MontarTelaPlataformaEntrega");
        }

        [HttpPost]
        public ActionResult FiltrarPlataformaEntrega(PLATAFORMA_ENTREGA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<PLATAFORMA_ENTREGA> listaObj = new List<PLATAFORMA_ENTREGA>();
                Session["FiltroPlataformaEntrega"] = item;
                Int32 volta = servApp.ExecuteFilter(item.PLEN_NM_NOME, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("MontarTelaPlataformaEntrega");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaPlataformaEntrega"] = listaObj;
                return RedirectToAction("MontarTelaPlataformaEntrega");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaPlataformaEntrega");
            }
        }

        public ActionResult VoltarBasePlataformaEntrega()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaPlataformaEntrega");
        }

        [HttpGet]
        public ActionResult IncluirPlataformaEntrega()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPlataformaEntrega");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            List<SelectListItem> antecipa = new List<SelectListItem>();
            antecipa.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            antecipa.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Antecipa = new SelectList(antecipa, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            PLATAFORMA_ENTREGA item = new PLATAFORMA_ENTREGA();
            PlataformaEntregaViewModel vm = Mapper.Map<PLATAFORMA_ENTREGA, PlataformaEntregaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.PLEN_DT_CADASTRO = DateTime.Today.Date;
            vm.PLEN_IN_ATIVO = 1;
            vm.PLEN_IN_ANTECIPACAO = 0;
            vm.PLEN_PC_ANTECIPACAO = 0;
            vm.PLEN_PC_VENDA = 0;
            vm.PLEN_VL_FIXO = 0;
            vm.PLEN_VL_LIMITE_FIXO = 0;
            vm.PLEN_VL_TAXA_CARTAO = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirPlataformaEntrega(PlataformaEntregaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> antecipa = new List<SelectListItem>();
            antecipa.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            antecipa.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Antecipa = new SelectList(antecipa, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PLATAFORMA_ENTREGA item = Mapper.Map<PlataformaEntregaViewModel, PLATAFORMA_ENTREGA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = servApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPlataformaEntrega"] = 3;
                        return RedirectToAction("MontarTelaPlataformaEntrega");
                    }
                    Session["IdVolta"] = item.PLEN_CD_ID;

                    // Sucesso
                    listaMaster = new List<PLATAFORMA_ENTREGA>();
                    Session["ListaPlataformaEntrega"] = null;
                    return RedirectToAction("MontarTelaPlataformaEntrega");
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
        public ActionResult VerPlataformaEntrega(Int32 id)
        {
            
            // Prepara view
            PLATAFORMA_ENTREGA item = servApp.GetItemById(id);
            objetoAntes = item;
            Session["PlataformaEntrega"] = item;
            Session["IdPlataformaEntrega"] = id;
            PlataformaEntregaViewModel vm = Mapper.Map<PLATAFORMA_ENTREGA, PlataformaEntregaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarPlataformaEntrega(Int32 id)
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
                    return RedirectToAction("MontarTelaPlataformaEntrega");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            PLATAFORMA_ENTREGA item = servApp.GetItemById(id);
            List<SelectListItem> antecipa = new List<SelectListItem>();
            antecipa.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            antecipa.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Antecipa = new SelectList(antecipa, "Value", "Text");
            objetoAntes = item;

            if (Session["MensPlataformaEntrega"] != null)
            {
                if ((Int32)Session["MensPlataformaEntrega"] == 1)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlataformaEntrega"] == 30)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlataformaEntrega"] == 31)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
            }

            Session["PlataformaEntrega"] = item;
            Session["IdPlataformaEntrega"] = id;
            PlataformaEntregaViewModel vm = Mapper.Map<PLATAFORMA_ENTREGA, PlataformaEntregaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarPlataformaEntrega(PlataformaEntregaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> antecipa = new List<SelectListItem>();
            antecipa.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            antecipa.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Antecipa = new SelectList(antecipa, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PLATAFORMA_ENTREGA item = Mapper.Map<PlataformaEntregaViewModel, PLATAFORMA_ENTREGA>(vm);
                    Int32 volta = servApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<PLATAFORMA_ENTREGA>();
                    Session["ListaPlataformaEntrega"] = null;
                    return RedirectToAction("MontarTelaPlataformaEntrega");
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
        public ActionResult ExcluirPlataformaEntrega(Int32 id)
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
                    return RedirectToAction("MontarTelaPlataformaEntrega");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            PLATAFORMA_ENTREGA item = servApp.GetItemById(id);
            objetoAntes = item;
            item.PLEN_IN_ATIVO = 0;
            Int32 volta = servApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensPlataformaEntrega"] = 4;
                return RedirectToAction("MontarTelaPlataformaEntrega");
            }
            listaMaster = new List<PLATAFORMA_ENTREGA>();
            Session["ListaPlataformaEntrega"] = null;
            return RedirectToAction("MontarTelaPlataformaEntrega");
        }

        [HttpGet]
        public ActionResult ReativarPlataformaEntrega(Int32 id)
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
                    return RedirectToAction("MontarTelaPlataformaEntrega");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            PLATAFORMA_ENTREGA item = servApp.GetItemById(id);
            item.PLEN_IN_ATIVO = 1;
            objetoAntes = item;
            Int32 volta = servApp.ValidateReativar(item, usuario);
            listaMaster = new List<PLATAFORMA_ENTREGA>();
            Session["ListaPlataformaEntrega"] = null;
            return RedirectToAction("MontarTelaPlataformaEntrega");
        }

        public ActionResult VoltarAnexoPlataformaEntrega()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarPlataformaEntrega", new { id = (Int32)Session["IdPlataformaEntrega"] });
        }

    }
}