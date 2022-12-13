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
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Canducci.Zip;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ERP_Condominios_Solution.Controllers
{
    public class CustoFixoController : Controller
    {
        private readonly ICustoFixoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IEmpresaAppService empApp;
        private readonly IFornecedorAppService forApp;
        private readonly ICentroCustoAppService ccApp;

        private String msg;
        private Exception exception;
        CUSTO_FIXO objeto = new CUSTO_FIXO();
        CUSTO_FIXO objetoAntes = new CUSTO_FIXO();
        List<CUSTO_FIXO> listaMaster = new List<CUSTO_FIXO>();
        String extensao;

        public CustoFixoController(ICustoFixoAppService baseApps, ILogAppService logApps, IEmpresaAppService empApps, IFornecedorAppService forApps, ICentroCustoAppService ccApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            empApp = empApps;
            forApp = forApps;
            ccApp = ccApps;
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
        public ActionResult MontarTelaCustoFixo()
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
            if ((List<CUSTO_FIXO>)Session["ListaCustoFixo"] == null || ((List<CUSTO_FIXO>)Session["ListaCustoFixo"]).Count == 0)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCustoFixo"] = listaMaster;
            }
            ViewBag.Listas = (List<CUSTO_FIXO>)Session["ListaCustoFixo"];
            ViewBag.Title = "CustoFixo";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACF_CD_ID", "CACF_NM_NOME");
            Session["CustoFixo"] = null;

            // Indicadores
            ViewBag.CustoFixo = ((List<CUSTO_FIXO>)Session["ListaCustoFixo"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCustoFixo"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCustoFixo"] == 1)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0230", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0231", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 5)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0232", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 6)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0233", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0234", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 98)
                {
                    ModelState.AddModelError("", "Foram excluídos " + ((Int32)Session["Conta"]).ToString() + " lançamentos de contas a pagar");
                }
                if ((Int32)Session["MensCustoFixo"] == 99)
                {
                    ModelState.AddModelError("", "Foram criados " + ((Int32)Session["Conta"]).ToString() + " lançamentos de contas a pagar");
                }
            }

            // Abre view
            Session["MensCustoFixo"] = 0;
            Session["VoltaCustoFixo"] = 1;
            objeto = new CUSTO_FIXO();
            if (Session["FiltroCustoFixo"] != null)
            {
                objeto = (CUSTO_FIXO)Session["FiltroCustoFixo"];
            }
            objeto.CUFX_IN_ATIVO = 1;
            return View(objeto);
        }

        public ActionResult RetirarFiltroCustoFixo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCustoFixo"] = null;
            Session["FiltroCustoFixo"] = null;
            return RedirectToAction("MontarTelaCustoFixo");
        }

        public ActionResult MostrarTudoCustoFixo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["FiltroCustoFixo"] = null;
            Session["ListaCustoFixo"] = listaMaster;
            return RedirectToAction("MontarTelaCustoFixo");
        }

        [HttpPost]
        public ActionResult FiltrarCustoFixo(CUSTO_FIXO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CUSTO_FIXO> listaObj = new List<CUSTO_FIXO>();
                Session["FiltroCustoFixo"] = item;
                Int32 volta = baseApp.ExecuteFilter(item.CACF_CD_ID, item.CUFX_NM_NOME, item.CUFX_DT_INICIO, item.CUFX_DT_TERMINO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCustoFixo"] = 1;
                    return RedirectToAction("MontarTelaCustoFixo");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaCustoFixo"]  = listaObj;
                return RedirectToAction("MontarTelaCustoFixo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCustoFixo");
            }
        }

        public ActionResult VoltarBaseCustoFixo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaCustoFixo");
        }

        public ActionResult IncluirCatCustoFixo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaCatCustoFixo"] = 2;
            return RedirectToAction("IncluirCatCustoFixo", "TabelaAuxiliar");
        }

        [HttpGet]
        public ActionResult IncluirCustoFixo()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACF_CD_ID", "CACF_NM_NOME");
            ViewBag.Periodicidades = new SelectList(baseApp.GetAllPeriodicidades(idAss), "PETA_CD_ID", "PETA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.PlanoContas = new SelectList(ccApp.GetAllItens(idAss), "CECU_CD_ID", "CECU_NM_EXIBE");

            // Recupera empresa
            EMPRESA emp = empApp.GetAllItens(idAss).FirstOrDefault();

            // Prepara view
            CUSTO_FIXO item = new CUSTO_FIXO();
            CustoFixoViewModel vm = Mapper.Map<CUSTO_FIXO, CustoFixoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CUFX_IN_ATIVO = 1;
            vm.CUFX_DT_CADASTRO = DateTime.Today.Date;
            vm.EMPR_CD_ID = emp.EMPR_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCustoFixo(CustoFixoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACF_CD_ID", "CACF_NM_NOME");
            ViewBag.Periodicidades = new SelectList(baseApp.GetAllPeriodicidades(idAss), "PETA_CD_ID", "PETA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.PlanoContas = new SelectList(ccApp.GetAllItens(idAss), "CECU_CD_ID", "CECU_NM_EXIBE");

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CUSTO_FIXO item = Mapper.Map<CustoFixoViewModel, CUSTO_FIXO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario, out Int32 conta);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCustoFixo"] = 3;
                        return RedirectToAction("MontarTelaCustoFixo", "CustoFixo");
                    }
                    if (volta == 2)
                    {
                        Session["MensCustoFixo"] = 4;
                        return RedirectToAction("MontarTelaCustoFixo", "CustoFixo");
                    }
                    if (volta == 3)
                    {
                        Session["MensCustoFixo"] = 5;
                        return RedirectToAction("MontarTelaCustoFixo", "CustoFixo");
                    }
                    if (volta == 4)
                    {
                        Session["MensCustoFixo"] = 6;
                        return RedirectToAction("MontarTelaCustoFixo", "CustoFixo");
                    }

                    // Sucesso
                    listaMaster = new List<CUSTO_FIXO>();
                    Session["ListaCustoFixo"] = null;
                    Session["IdVolta"] = item.CUFX_CD_ID;
                    Session["IdCustoFixo"] = item.CUFX_CD_ID;
                    Session["MensCustoFixo"] = 99;
                    Session["Conta"] = conta;
                    return RedirectToAction("MontarTelaCustoFixo");
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
        public ActionResult EditarCustoFixo(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACF_CD_ID", "CACF_NM_NOME");
            ViewBag.Periodicidades = new SelectList(baseApp.GetAllPeriodicidades(idAss), "PETA_CD_ID", "PETA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.PlanoContas = new SelectList(ccApp.GetAllItens(idAss), "CECU_CD_ID", "CECU_NM_EXIBE");

            // Recupera custo
            CUSTO_FIXO item = baseApp.GetItemById(id);

            // Mensagens
            if (Session["MensCustoFixo"] != null)
            {
                if ((Int32)Session["MensCustoFixo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0231", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 5)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0232", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCustoFixo"] == 6)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0233", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            objetoAntes = item;
            Session["CustoFixo"] = item;
            Session["IdCustoFixo"] = id;
            Session["IdVolta"] = id;
            CustoFixoViewModel vm = Mapper.Map<CUSTO_FIXO, CustoFixoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCustoFixo(CustoFixoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACF_CD_ID", "CACF_NM_NOME");
            ViewBag.Periodicidades = new SelectList(baseApp.GetAllPeriodicidades(idAss), "PETA_CD_ID", "PETA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.PlanoContas = new SelectList(ccApp.GetAllItens(idAss), "CECU_CD_ID", "CECU_NM_EXIBE");

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CUSTO_FIXO item = Mapper.Map<CustoFixoViewModel, CUSTO_FIXO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 2)
                    {
                        Session["MensCustoFixo"] = 4;
                        return RedirectToAction("EditarCustoFixo", "CustoFixo");
                    }
                    if (volta == 3)
                    {
                        Session["MensCustoFixo"] = 5;
                        return RedirectToAction("EditarCustoFixo", "CustoFixo");
                    }
                    if (volta == 4)
                    {
                        Session["MensCustoFixo"] = 6;
                        return RedirectToAction("EditarCustoFixo", "CustoFixo");
                    }

                    // Sucesso
                    listaMaster = new List<CUSTO_FIXO>();
                    Session["ListaCustoFixo"] = null;
                    return RedirectToAction("MontarTelaCustoFixo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult ExcluirCustoFixo(Int32 id)
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
                //Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CUSTO_FIXO item = baseApp.GetItemById(id);
            objetoAntes = (CUSTO_FIXO)Session["CustoFixo"];
            item.CUFX_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario, out Int32 conta);
            if (volta == 1)
            {
                Session["MensCustoFixo"] = 10;
                return RedirectToAction("MontarTelaCustoFixo", "CustoFixo");
            }
            listaMaster = new List<CUSTO_FIXO>();
            Session["ListaCustoFixo"] = null;
            Session["FiltroCustoFixo"] = null;
            Session["MensCustoFixo"] = 98;
            Session["Conta"] = conta;
            return RedirectToAction("MontarTelaCustoFixo");
        }

        [HttpGet]
        public ActionResult ReativarCustoFixo(Int32 id)
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
                //Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CLIENTE item = baseApp.GetItemById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            item.CLIE_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            Session["FiltroCliente"] = null;
            return RedirectToAction("MontarTelaCliente");
        }

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
            if ((List<CLIENTE>)Session["ListaCliente"] == null || ((List<CLIENTE>)Session["ListaCliente"]).Count == 0)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCliente"] = listaMaster;
            }

            ViewBag.Listas = (List<CLIENTE>)Session["ListaCliente"];
            ViewBag.Title = "Clientes";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["Cliente"] = null;
            //List<SelectListItem> ativo = new List<SelectListItem>();
            //ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            //ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            //ViewBag.Ativos = new SelectList(ativo, "Value", "Text");

            // Indicadores
            ViewBag.Clientes = ((List<CLIENTE>)Session["ListaCliente"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            if (Session["MensCliente"] != null)
            {
            }

            // Abre view
            Session["VoltaCliente"] = 2;
            objeto = new CLIENTE();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult VerAnexoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_ANEXO item = baseApp.GetAnexoById(id);
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }










    }
}