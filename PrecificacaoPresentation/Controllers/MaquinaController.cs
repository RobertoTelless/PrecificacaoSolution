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
    public class MaquinaController : Controller
    {
        private readonly IMaquinaAppService servApp;
        private readonly ILogAppService logApp;
        MAQUINA objeto = new MAQUINA();
        MAQUINA objetoAntes = new MAQUINA();
        List<MAQUINA> listaMaster = new List<MAQUINA>();
        String extensao;

        public MaquinaController(IMaquinaAppService servApps, ILogAppService logApps)
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
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaMaquina()
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
            if (Session["ListaMaquina"] == null)
            {
                listaMaster = servApp.GetAllItens(idAss);
                Session["ListaMaquina"] = listaMaster;
            }
            ViewBag.Listas = (List<MAQUINA>)Session["ListaMaquina"];
            ViewBag.Title = "Maquina";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Maquinas = ((List<MAQUINA>)Session["ListaMaquina"]).Count;

            if (Session["MensMaquina"] != null)
            {
                if ((Int32)Session["MensMaquina"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0150", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMaquina"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMaquina"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0151", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMaquina"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0124", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensMaquina"] = 0;
            Session["VoltaMaquina"] = 1;
            objeto = new MAQUINA();
            return View(objeto);
        }

        public ActionResult RetirarFiltroMaquina()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaMaquina"] = null;
            Session["FiltroMaquina"] = null;
            return RedirectToAction("MontarTelaMaquina");
        }

        public ActionResult MostrarTudoMaquina()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = servApp.GetAllItensAdm(idAss);
            Session["FiltroMaquina"] = null;
            Session["ListaMaquina"] = listaMaster;
            return RedirectToAction("MontarTelaMaquina");
        }

        [HttpPost]
        public ActionResult FiltrarMaquina(MAQUINA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<MAQUINA> listaObj = new List<MAQUINA>();
                Session["FiltroMaquina"] = item;
                Int32 volta = servApp.ExecuteFilter(item.MAQN_NM_PROVEDOR, item.MAQN_NM_NOME, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("MontarTelaMaquina");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaMaquina"] = listaObj;
                return RedirectToAction("MontarTelaMaquina");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMaquina");
            }
        }

        public ActionResult VoltarBaseMaquina()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltarMaquina"] == 10)
            {
                return RedirectToAction("IncluirEmpresaMaquina", "Empresa");
            }
            return RedirectToAction("MontarTelaMaquina");
        }

        public ActionResult IncluirMaquinaForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaMaquina"] = 10;
            return RedirectToAction("IncluirMaquina");
        }

        [HttpGet]
        public ActionResult IncluirMaquina()
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
                    return RedirectToAction("MontarTelaMaquina");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MAQUINA item = new MAQUINA();
            MaquinaViewModel vm = Mapper.Map<MAQUINA, MaquinaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.MAQN_DT_CADASTRO = DateTime.Today.Date;
            vm.MAQN_IN_ATIVO = 1;
            vm.MAQN_PC_DEBITO = 0;
            vm.MAQN_PC_CREDITO = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirMaquina(MaquinaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    MAQUINA item = Mapper.Map<MaquinaViewModel, MAQUINA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = servApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMaquina"] = 3;
                        return RedirectToAction("MontarTelaMaquina");
                    }
                    Session["IdVolta"] = item.MAQN_CD_ID;

                    // Sucesso
                    listaMaster = new List<MAQUINA>();
                    Session["ListaMaquina"] = null;
                    return RedirectToAction("VoltarBaseMaquina");
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
        public ActionResult VerMaquina(Int32 id)
        {
            
            // Prepara view
            MAQUINA item = servApp.GetItemById(id);
            objetoAntes = item;
            Session["Maquina"] = item;
            Session["IdMaquina"] = id;
            MaquinaViewModel vm = Mapper.Map<MAQUINA, MaquinaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarMaquina(Int32 id)
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
                    return RedirectToAction("MontarTelaMaquina");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            MAQUINA item = servApp.GetItemById(id);
            //ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            //ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            //ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            //ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            //List<SelectListItem> local = new List<SelectListItem>();
            //local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            //local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            //local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            //ViewBag.Local = new SelectList(local, "Value", "Text");
            ////ViewBag.LstPedidos = pedvApp.GetAllItens();
            objetoAntes = item;

            if (Session["MensMaquina"] != null)
            {
                if ((Int32)Session["MensMaquina"] == 1)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
                    Session["MensPreco"] = 0;
                }
                if ((Int32)Session["MensMaquina"] == 30)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMaquina"] == 31)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
            }

            Session["Maquina"] = item;
            Session["IdMaquina"] = id;
            MaquinaViewModel vm = Mapper.Map<MAQUINA, MaquinaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMaquina(MaquinaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            //ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            //ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            //ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            //ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            //List<SelectListItem> local = new List<SelectListItem>();
            //local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            //local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            //local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            //ViewBag.Local = new SelectList(local, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MAQUINA item = Mapper.Map<MaquinaViewModel, MAQUINA>(vm);
                    Int32 volta = servApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<MAQUINA>();
                    Session["ListaMaquina"] = null;
                    return RedirectToAction("MontarTelaMaquina");
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
        public ActionResult ExcluirMaquina(Int32 id)
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
                    return RedirectToAction("MontarTelaMaquina");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MAQUINA item = servApp.GetItemById(id);
            objetoAntes = item;
            item.MAQN_IN_ATIVO = 0;
            Int32 volta = servApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMaquina"] = 4;
                return RedirectToAction("MontarTelaMaquina");
            }
            listaMaster = new List<MAQUINA>();
            Session["ListaMaquina"] = null;
            return RedirectToAction("MontarTelaMaquina");
        }

        [HttpGet]
        public ActionResult ReativarMaquina(Int32 id)
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
                    return RedirectToAction("MontarTelaMaquina");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MAQUINA item = servApp.GetItemById(id);
            item.MAQN_IN_ATIVO = 1;
            objetoAntes = item;
            Int32 volta = servApp.ValidateReativar(item, usuario);
            listaMaster = new List<MAQUINA>();
            Session["ListaMaquina"] = null;
            return RedirectToAction("MontarTelaMaquina");
        }

        public ActionResult VoltarAnexoMaquina()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarMaquina", new { id = (Int32)Session["IdMaquina"] });
        }

        public JsonResult GetMaquina(Int32 id)
        {
            var forn = servApp.GetItemById(id);
            var hash = new Hashtable();
            hash.Add("provedor", forn.MAQN_NM_PROVEDOR);
            hash.Add("credito", forn.MAQN_PC_CREDITO);
            hash.Add("debito", forn.MAQN_PC_DEBITO);
            hash.Add("antecipacao", forn.MAQN_PC_ANTECIPACAO);
            return Json(hash);
        }

    }
}