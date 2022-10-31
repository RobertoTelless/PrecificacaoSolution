﻿using System;
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
using System.Threading.Tasks;

namespace ERP_Condominios_Solution.Controllers
{
    public class CRMController : Controller
    {
        private readonly ICRMAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        //private readonly IMensagemAppService menApp;
        private readonly IAgendaAppService ageApp;
        private readonly IClienteAppService cliApp;
        private readonly ITemplateEMailAppService temaApp;
        private readonly ITemplateAppService tempApp;
        private readonly ICRMDiarioAppService diaApp;
        //private readonly IFunilAppService funApp;
        private readonly ITemplatePropostaAppService tpApp;

        private String msg;
        private Exception exception;
        CRM objeto = new CRM();
        CRM objetoAntes = new CRM();
        List<CRM> listaMaster = new List<CRM>();
        CRM_PEDIDO_VENDA objetoVenda = new CRM_PEDIDO_VENDA();
        CRM_PEDIDO_VENDA objetoAntesVenda = new CRM_PEDIDO_VENDA();
        List<CRM_PEDIDO_VENDA> listaMasterVenda = new List<CRM_PEDIDO_VENDA>();
        List<DIARIO_PROCESSO> listaMasterDiario = new List<DIARIO_PROCESSO>();
        DIARIO_PROCESSO objetoDiario = new DIARIO_PROCESSO();
        DIARIO_PROCESSO objetoAntesDiario = new DIARIO_PROCESSO();
        String extensao;

        public CRMController(ICRMAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IAgendaAppService ageApps, IClienteAppService cliApps, ITemplateEMailAppService temaApps, ITemplateAppService tempApps, ICRMDiarioAppService diaApps, ITemplatePropostaAppService tpApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            //menApp = menApps;
            ageApp = ageApps;
            cliApp = cliApps;
            temaApp = temaApps;
            tempApp = tempApps;
            diaApp = diaApps;
            tpApp = tpApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {

            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaCRM()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
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
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CRM>)Session["ListaCRM"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCRM"] = listaMaster;
            }
            Session["CRM"] = null;
            List<CRM> list = (List<CRM>)Session["ListaCRM"];
            list = list.OrderByDescending(p => p.CRM1_DT_CRIACAO).ToList();
            ViewBag.Listas = list;
            ViewBag.Title = "CRM";

            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> visao = new List<SelectListItem>();
            visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
            visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
            ViewBag.Visao = new SelectList(visao, "Value", "Text");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Em Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivado", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelado", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhado", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");
            Session["IncluirCRM"] = 0;
            Session["CRMVoltaAtendimento"] = 0;
            Session["VoltaAgenda"] = 11;
            Session["VoltaCRMBase"] = 0;
            Session["LinkAprova"] = null;
            Session["VoltaPedido"] = 0;
            Session["VoltaHistorico"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 30)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 31)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 60)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 61)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 62)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 63)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture));
                }

            }

            // Abre view
            Session["IdCRM"] = null;
            Session["MensCRM"] = 0;
            Session["VoltaCRM"] = 1;
            Session["IncluirCliente"] = 0;
            objeto = new CRM();
            if (Session["FiltroCRM"] != null)
            {
                objeto = (CRM)Session["FiltroCRM"];
            }
            return View(objeto);
        }

        public ActionResult IncluirCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaClienteCRM"] = 1;
            return RedirectToAction("IncluirCliente", "Cliente");
        }

        [HttpGet]
        public ActionResult MontarTelaKanbanCRM()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
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
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            if ((List<CRM>)Session["ListaCRM"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCRM"] = listaMaster;
            }
            Session["CRM"] = null;
            ViewBag.Listas = (List<CRM>)Session["ListaCRM"];
            ViewBag.Title = "CRM";
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> visao = new List<SelectListItem>();
            visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
            visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
            ViewBag.Visao = new SelectList(visao, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            adic.Add(new SelectListItem() { Text = "Faturamento", Value = "6" });
            adic.Add(new SelectListItem() { Text = "Expedição", Value = "7" });
            adic.Add(new SelectListItem() { Text = "Entregue", Value = "8" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");
            Session["IncluirCRM"] = 0;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 30)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 31)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 60)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 61)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 62)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 63)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["IdCRM"] = null;
            Session["VoltaCRM"] = 2;
            Session["IncluirCliente"] = 0;
            objeto = new CRM();
            if (Session["FiltroCRM"] != null)
            {
                objeto = (CRM)Session["FiltroCRM"];
            }
            return View(objeto);
        }
        
        public ActionResult RetirarFiltroCRM()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCRM"] = null;
            Session["FiltroCRM"] = null;
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpPost]
        public ActionResult FiltrarCRM(CRM item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<CRM> listaObj = new List<CRM>();
                Session["FiltroCRM"] = item;
                Int32 volta = baseApp.ExecuteFilter(item.CRM1_IN_STATUS, item.CRM1_DT_CRIACAO, item.CRM1_DT_CANCELAMENTO, item.ORIG_CD_ID, item.CRM1_IN_ATIVO, item.CRM1_NM_NOME, item.CRM1_DS_DESCRICAO, item.CRM1_IN_ESTRELA, item.CRM1_NR_TEMPERATURA, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCRM"] = 1;
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaCRM"] = listaObj;
                return RedirectToAction("MontarTelaCRM");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCRM");
            }
        }

        public ActionResult VoltarBaseCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCRM"] = null;
            if ((Int32)Session["VoltaCRMBase"] == 99)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((Int32)Session["VoltaCRM"] == 2)
            {
                return RedirectToAction("MontarTelaKanbanCRM", "CRM");
            }
            if ((Int32)Session["VoltaCRMBase"] == 90)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            if ((Int32)Session["VoltaCRM"] == 11)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            if ((Int32)Session["VoltaCRM"] == 12)
            {
                return RedirectToAction("VoltarAnexoAtendimento", "Atendimento");
            }
            if ((Int32)Session["VoltaCRM"] == 30)
            {
                return RedirectToAction("VoltarAnexoAtendimento", "Atendimento");
            }
            if ((Int32)Session["VoltaCRM"] == 22)
            {
                return RedirectToAction("MontarTelaHistorico", "CRM");
            }
            Session["VoltaCRM"] = 0;
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpGet]
        public ActionResult ExcluirProcesso(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM item = baseApp.GetItemById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRM1_IN_ATIVO = 2;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCRM"] = 4;
                return RedirectToAction("MontarTelaCRM");
            }
            Session["ListaCRM"] = null;
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpGet]
        public ActionResult ReativarProcesso(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            List<CRM> lista = baseApp.GetAllItens(idAss);
            lista = lista.Where(p => p.CRM1_IN_ATIVO == 1).ToList();
            Int32 num = lista.Count;
            if ((Int32)Session["NumProc"] <= num)
            {
                Session["MensCRM"] = 50;
                return RedirectToAction("MontarTelaCRM", "CRM");
            }

            CRM item = baseApp.GetItemById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRM1_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaCRM"] = null;
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpGet]
        public ActionResult EstrelaSim(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM item = baseApp.GetItemById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRM1_IN_ESTRELA = 1;
            Int32 volta = baseApp.ValidateEdit(item, item);
            Session["ListaCRM"] = null;
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpGet]
        public ActionResult EstrelaNao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM item = baseApp.GetItemById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRM1_IN_ESTRELA = 0;
            Int32 volta = baseApp.ValidateEdit(item, item);
            Session["ListaCRM"] = null;
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpGet]
        public ActionResult EncerrarAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_ACAO item = baseApp.GetAcaoById(id);
            item.CRAC_IN_STATUS = 3;
            Int32 volta = baseApp.ValidateEditAcao(item);

            // Gera diario
            CRM crm = baseApp.GetItemById(item.CRM1_CD_ID);
            CLIENTE cli = cliApp.GetItemById(crm.CLIE_CD_ID);
            DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
            dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
            dia.USUA_CD_ID = usuario.USUA_CD_ID;
            dia.DIPR_DT_DATA = DateTime.Today.Date;
            dia.CRM1_CD_ID = item.CRM1_CD_ID;
            dia.CRAC_CD_ID = item.CRAC_CD_ID;
            dia.DIPR_NM_OPERACAO = "Encerramento de Ação";
            dia.DIPR_DS_DESCRICAO = "Encerramento de Ação " + item.CRAC_NM_TITULO + ". Processo: " + crm.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
            Int32 volta3 = diaApp.ValidateCreate(dia);

            return RedirectToAction("VoltarAcompanhamentoCRMBase");
        }

        public ActionResult GerarRelatorioListaCRM()
        {            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ProcessosLista" + "_" + data + ".pdf";
            List<CRM> lista = (List<CRM>)Session["ListaCRM"];
            CRM filtro = (CRM)Session["FiltroCRM"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontO = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.ORANGE);
            Font meuFontP = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
            Font meuFontE = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
            Font meuFontD = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);
            Font meuFontS = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

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
            Image image = Image.GetInstance(Server.MapPath("~/Images/Precificacao_Favicon.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Processos - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 150f, 160f, 80f, 150f, 80f, 80f, 80f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Processos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Favorito", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cliente", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Título", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Status", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Próxima Ação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data Prevista", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Origem", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Situação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CRM item in lista)
            {
                if (item.CRM1_IN_ESTRELA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Sim", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_ESTRELA == 0)
                {
                    cell = new PdfPCell(new Paragraph("Não", meuFontO))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.CLIENTE.CLIE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.CRM1_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                if (item.CRM1_IN_STATUS == 1)
                {
                    cell = new PdfPCell(new Paragraph("Prospecção", meuFontO))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_STATUS == 2)
                {
                    cell = new PdfPCell(new Paragraph("Contato Realizado", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_STATUS == 3)
                {
                    cell = new PdfPCell(new Paragraph("Proposta Apresentada", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_STATUS == 4)
                {
                    cell = new PdfPCell(new Paragraph("Negociação", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_STATUS == 5)
                {
                    cell = new PdfPCell(new Paragraph("Encerrado", meuFontP))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);

                if (item.CRM_ACAO.Count > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DS_DESCRICAO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFontP))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);

                if (item.CRM_ACAO.Count > 0)
                {
                    if (item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.Date >= DateTime.Today.Date)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFontE))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFontD))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFontP))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.CRM_ORIGEM.CROR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                if (item.CRM1_IN_ATIVO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Ativo", meuFontE))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_ATIVO == 2)
                {
                    cell = new PdfPCell(new Paragraph("Arquivado", meuFontO))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_ATIVO == 3)
                {
                    cell = new PdfPCell(new Paragraph("Cancelado", meuFontO))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_ATIVO == 4)
                {
                    cell = new PdfPCell(new Paragraph("Falhado", meuFontD))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                else if (item.CRM1_IN_ATIVO == 5)
                {
                    cell = new PdfPCell(new Paragraph("Sucesso", meuFontP))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                }
                table.AddCell(cell);
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
                if (filtro.CRM1_IN_STATUS > 0)
                {
                    if (filtro.CRM1_IN_STATUS == 1)
                    {
                        parametros += "Status: Prospecção";
                    }
                    else if (filtro.CRM1_IN_STATUS == 2)
                    {
                        parametros += "Status: Contato Realizado";
                    }
                    else if (filtro.CRM1_IN_STATUS == 3)
                    {
                        parametros += "Status: Proposta Apresentada";
                    }
                    else if (filtro.CRM1_IN_STATUS == 4)
                    {
                        parametros += "Status: Em Negociação";
                    }
                    else if (filtro.CRM1_IN_STATUS == 5)
                    {
                        parametros += "Status: Encerrado";
                    }
                    ja = 1;
                }

                if (filtro.CRM1_DT_CRIACAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data Início: " + filtro.CRM1_DT_CRIACAO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += "e Data Início: " + filtro.CRM1_DT_CRIACAO.Value.ToShortDateString();
                    }
                }

                if (filtro.CRM1_DT_CANCELAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data Final: " + filtro.CRM1_DT_CANCELAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += "e Data Final: " + filtro.CRM1_DT_CANCELAMENTO.Value.ToShortDateString();
                    }
                }

                if (filtro.ORIG_CD_ID > 0)
                {
                    if (ja == 0)
                    {
                        parametros += "Origem: " + filtro.CRM_ORIGEM.CROR_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Origem: " + filtro.CRM_ORIGEM.CROR_NM_NOME;
                    }
                }

                if (filtro.CRM1_IN_ATIVO > 0)
                {
                    if (ja == 0)
                    {
                        if (filtro.CRM1_IN_ATIVO == 1)
                        {
                            parametros += "Situação: Ativo";
                        }
                        else if (filtro.CRM1_IN_ATIVO == 2)
                        {
                            parametros += "Situação: Arquivado";
                        }
                        else if (filtro.CRM1_IN_ATIVO == 3)
                        {
                            parametros += "Situação: Cancelado";
                        }
                        else if (filtro.CRM1_IN_ATIVO == 4)
                        {
                            parametros += "Situação: Falhado";
                        }
                        else if (filtro.CRM1_IN_ATIVO == 5)
                        {
                            parametros += "Situação: Sucesso";
                        }
                        ja = 1;
                    }
                    else
                    {
                        if (filtro.CRM1_IN_ATIVO == 1)
                        {
                            parametros += "e Situação: Ativo";
                        }
                        else if (filtro.CRM1_IN_ATIVO == 2)
                        {
                            parametros += "e Situação: Arquivado";
                        }
                        else if (filtro.CRM1_IN_ATIVO == 3)
                        {
                            parametros += "e Situação: Cancelado";
                        }
                        else if (filtro.CRM1_IN_ATIVO == 4)
                        {
                            parametros += "e Situação: Falhado";
                        }
                        else if (filtro.CRM1_IN_ATIVO == 5)
                        {
                            parametros += "e Situação: Sucesso";
                        }
                    }
                }

                if (filtro.CRM1_IN_ESTRELA > 0)
                {
                    if (ja == 0)
                    {
                        if (filtro.CRM1_IN_ESTRELA == 1)
                        {
                            parametros += "Favorito: Sim";
                        }
                        else if (filtro.CRM1_IN_ESTRELA == 0)
                        {
                            parametros += "Favorito: Não";
                        }
                        ja = 1;
                    }
                    else
                    {
                        if (filtro.CRM1_IN_ESTRELA == 1)
                        {
                            parametros += "e Favorito: Sim";
                        }
                        else if (filtro.CRM1_IN_ESTRELA == 0)
                        {
                            parametros += "e Favorito: Não";
                        }
                    }
                }

                if (filtro.CRM1_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Título: " + filtro.CRM1_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Título: " + filtro.CRM1_NM_NOME;
                    }
                }

                if (filtro.CRM1_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Geral: " + filtro.CRM1_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Geral: " + filtro.CRM1_DS_DESCRICAO;
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
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpGet]
        public ActionResult IncluirProcessoCRM()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            List<CRM> lista = baseApp.GetAllItens(idAss);
            lista = lista.Where(p => p.CRM1_IN_ATIVO == 1).ToList();
            Int32 num = lista.Count;
            Int32 procBase = (Int32)Session["NumProcessosBase"];
            if ((Int32)Session["NumProcessosBase"] <= num)
            {
                Session["MensCRM"] = 50;
                return RedirectToAction("MontarTelaCRM", "CRM");
            }

            // Prepara listas
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Mensagem
            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 22)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0141", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            Session["CRMNovo"] = 0;
            Session["VoltaCliente"] = 8;
            CRM item = new CRM();
            CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CRM1_DT_CRIACAO = DateTime.Today.Date;
            vm.CRM1_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.CRM1_IN_STATUS = 1;
            return View(vm);
        }


        [HttpPost]
        public ActionResult IncluirProcessoCRM(CRMViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica cliente
                    if (vm.CLIE_CD_ID == null || vm.CLIE_CD_ID == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0141", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Carrega foto e processa alteracao
                    if (item.CRM1_AQ_IMAGEM == null)
                    {
                        item.CRM1_AQ_IMAGEM = "~/Images/icone_imagem.jpg";
                        volta = baseApp.ValidateEdit(item, item);
                    }

                    // Cria pasta
                    String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Listas
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRM1_CD_ID;
                    Session["IdCRM"] = item.CRM1_CD_ID;

                    // Processa Anexos
                    if (Session["FileQueueCRM"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCRM"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueCRM(file);
                            }
                            else
                            {
                                UploadFotoQueueCRM(file);
                            }
                        }
                        Session["FileQueueCRM"] = null;
                    }

                    // Processa voltas
                    if ((Int32)Session["VoltaCRM"] == 3)
                    {
                        Session["VoltaCRM"] = 0;
                        Session["CRMAtendimento"] = 0;
                        return RedirectToAction("IncluirProcessoCRM", "CRM");
                    }

                    Session["CRMAtendimento"] = 0;
                    Session["PontoProposta"] = 0;
                    return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
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
            Session["FileQueueCRM"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueCRM(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            CRM item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CRM_ANEXO foto = new CRM_ANEXO();
            foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRAN_DT_ANEXO = DateTime.Today;
            foto.CRAN_IN_ATIVO = 1;
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
            foto.CRAN_IN_TIPO = tipo;
            foto.CRAN_NM_TITULO = fileName;
            foto.CRM1_CD_ID = item.CRM1_CD_ID;

            item.CRM_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        [HttpPost]
        public ActionResult UploadFileCRM(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            CRM item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CRM_ANEXO foto = new CRM_ANEXO();
            foto.CRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRAN_DT_ANEXO = DateTime.Today;
            foto.CRAN_IN_ATIVO = 1;
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
            foto.CRAN_IN_TIPO = tipo;
            foto.CRAN_NM_TITULO = fileName;
            foto.CRM1_CD_ID = item.CRM1_CD_ID;

            item.CRM_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        public ActionResult VoltarAnexoCRMProposta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            return RedirectToAction("EditarProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        [HttpPost]
        public ActionResult UploadFileQueueCRMPedido(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRMPedido"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                if ((Int32)Session["FlagAnexo"] == 2)
                {
                    return RedirectToAction("VoltarEditarVenda");
                }
                return RedirectToAction("VoltarEditarPedidoCRM");
            }

            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                if ((Int32)Session["FlagAnexo"] == 2)
                {
                    return RedirectToAction("VoltarEditarVenda");
                }
                return RedirectToAction("VoltarEditarPedidoCRM");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Pedido/" + item.CRPV_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CRM_PEDIDO_VENDA_ANEXO foto = new CRM_PEDIDO_VENDA_ANEXO();
            foto.CRPA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRPA_DT_ANEXO = DateTime.Today;
            foto.CRPA_IN_ATIVO = 1;
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
            foto.CRPA_IN_TIPO = tipo;
            foto.CRPA_NM_TITULO = fileName;
            foto.CRPV_CD_ID = item.CRPV_CD_ID;

            item.CRM_PEDIDO_VENDA_ANEXO.Add(foto);
            Int32 volta = baseApp.ValidateEditPedido(item);
            return RedirectToAction("VoltarEditarPedidoCRM");
        }

        [HttpPost]
        public ActionResult UploadFileCRMPedido(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRMPedido"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                if ((Int32)Session["FlagAnexo"] == 2)
                {
                    return RedirectToAction("VoltarEditarVenda");
                }
                return RedirectToAction("VoltarEditarPedidoCRM");
            }

            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                if ((Int32)Session["FlagAnexo"] == 2)
                {
                    return RedirectToAction("VoltarEditarVenda");
                }
                return RedirectToAction("VoltarEditarPedidoCRM");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Pedido/" + item.CRPV_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CRM_PEDIDO_VENDA_ANEXO foto = new CRM_PEDIDO_VENDA_ANEXO();
            foto.CRPA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CRPA_DT_ANEXO = DateTime.Today;
            foto.CRPA_IN_ATIVO = 1;
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
            foto.CRPA_IN_TIPO = tipo;
            foto.CRPA_NM_TITULO = fileName;
            foto.CRPV_CD_ID = item.CRPV_CD_ID;

            item.CRM_PEDIDO_VENDA_ANEXO.Add(foto);
            Int32 volta = baseApp.ValidateEditPedido(item);
            return RedirectToAction("VoltarEditarPedidoCRM");
        }

        public ActionResult VoltarAnexoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCRM"] == 10)
            {
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            return RedirectToAction("EditarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarAcompanhamentoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PontoProposta"] == 99)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            if ((Int32)Session["PontoProposta"] == 92)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["VoltaPedido"] == 10)
            {
                return RedirectToAction("VerPedidosUsuarioCRMPrevia", "CRM");
            }
            if ((Int32)Session["VoltaPedido"] == 22)
            {
                return RedirectToAction("MontarTelaHistorico", "CRM");
            }
            Session["VoltaHistorico"] = 0;
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarAcompanhamentoCRMBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarPedidoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["SegueInclusao"] = 0;
            return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarAcaoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PontoAcao"] == 100)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 101)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 91)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["PontoAcao"] == 1)
                {
                return RedirectToAction("VerAcoesUsuarioCRMPrevia");
            }
            if ((Int32)Session["PontoAcao"] == 2)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            if ((Int32)Session["PontoAcao"] == 22)
            {
                return RedirectToAction("MontarTelaHistorico", "CRM");
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarPropostaCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["PontoProposta"] == 3)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["PontoProposta"] == 1)
            {
                return RedirectToAction("VerPropostasUsuarioCRMPrevia");
            }
            if ((Int32)Session["PontoProposta"] == 2)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarEditarPropostaCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaComentProposta"] == 1)
            {
                return RedirectToAction("EditarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            if ((Int32)Session["VoltaComentProposta"] == 2)
            {
                return RedirectToAction("CancelarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            if ((Int32)Session["VoltaComentProposta"] == 3)
            {
                return RedirectToAction("ReprovarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            if ((Int32)Session["VoltaComentProposta"] == 4)
            {
                return RedirectToAction("AprovarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            if ((Int32)Session["VoltaComentProposta"] == 5)
            {
                return RedirectToAction("EnviarProposta", new { id = (Int32)Session["IdCRMProposta"] });
            }
            return RedirectToAction("EditarProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        public ActionResult VoltarEditarPedidoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaComentPedido"] == 1)
            {
                return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 2)
            {
                return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 3)
            {
                return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 4)
            {
                return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 5)
            {
                return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
            }
            return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarEditarVenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaComentPedido"] == 1)
            {
                return RedirectToAction("EditarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 2)
            {
                return RedirectToAction("CancelarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 3)
            {
                return RedirectToAction("ReprovarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 4)
            {
                return RedirectToAction("AprovarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            if ((Int32)Session["VoltaComentPedido"] == 5)
            {
                return RedirectToAction("EnviarVenda", new { id = (Int32)Session["IdVenda"] });
            }
            return RedirectToAction("EditarVenda", new { id = (Int32)Session["IdVenda"] });
        }

        public ActionResult VoltarEditarPedidoCRMCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarEncerrarProcessoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EncerrarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarCancelarProcessoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CancelarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarCancelarPedido()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarEditarPedidoCRMDireto()
        {
            return RedirectToAction("EditarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult VoltarEditarVendaDireto()
        {
            return RedirectToAction("EditarVenda", new { id = (Int32)Session["IdVenda"] });
        }

        [HttpGet]
        public ActionResult VerAnexoCRM(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CRM_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult VerAnexoCRMPedido(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CRM_PEDIDO_VENDA_ANEXO item = baseApp.GetAnexoPedidoById(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult UploadFotoQueueCRM(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idUsu = (Int32)Session["IdCRM"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            CRM item = baseApp.GetItemById(idUsu);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                item.CRM1_AQ_IMAGEM = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        [HttpPost]
        public ActionResult UploadFotoCRM(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 10;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            CRM item = baseApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                Session["MensCRM"] = 11;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/CRM/" + item.CRM1_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                file.SaveAs(path);

                // Gravar registro
                item.CRM1_AQ_IMAGEM = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        public FileResult DownloadCRM(Int32 id)
        {
            CRM_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.CRAN_AQ_ARQUIVO;
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

        public FileResult DownloadCRMPedido(Int32 id)
        {
            CRM_PEDIDO_VENDA_ANEXO item = baseApp.GetAnexoPedidoById(id);
            String arquivo = item.CRPA_AQ_ARQUIVO;
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

        [HttpGet]
        public ActionResult CancelarProcessoCRM(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Motivos = new SelectList(baseApp.GetAllMotivoCancelamento(idAss).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM item = baseApp.GetItemById(id);
            Session["IdCRM"] = item.CRM1_CD_ID;

            // Checa ações
            Session["TemAcao"] = 0;
            if (item.CRM_ACAO.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
            {
                Session["TemAcao"] = 1;
            }

            // Prepara view
            CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
            vm.CRM1_DT_CANCELAMENTO = DateTime.Today.Date;
            vm.CRM1_IN_ATIVO = 3;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CancelarProcessoCRM(CRMViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Motivos = new SelectList(baseApp.GetAllMotivoCancelamento(idAss).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 30;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 31;
                        return RedirectToAction("MontarTelaCRM");
                    }

                    // Gera diario
                    CLIENTE cli = cliApp.GetItemById(item.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Cancelamento de Processo";
                    dia.DIPR_DS_DESCRICAO = "Cancelamento de Processo " + item.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Listas
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRM1_CD_ID;
                    Session["IdCRM"] = item.CRM1_CD_ID;
                    return RedirectToAction("MontarTelaCRM");
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
        //[ValidateAntiForgeryToken]
        public ActionResult EnviarParaExpedicaoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM item = (CRM)Session["CRM"];
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    item.CRM1_IN_ATIVO = 7;
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno

                    // Listas
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult EnviarExpedicaoCRM()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            CRM item = baseApp.GetItemById((Int32)Session["IdCRM"]);

            CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
            vm.CRM1_IN_STATUS = 5;
            vm.CRM1_IN_ATIVO = 7;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EnviarExpedicaoCRM(CRMViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];            
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno

                    // Listas
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRM1_CD_ID;
                    Session["IdCRM"] = item.CRM1_CD_ID;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult IncluirGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCliGrupo"] = 1;
            return RedirectToAction("IncluirGrupo", "Grupo");
        }

        [HttpGet]
        public ActionResult EncerrarProcessoChamada()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EncerrarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        [HttpGet]
        public ActionResult ConfirmarContato()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
            crm.CRM1_IN_STATUS = 2;
            Int32 volta = baseApp.ValidateEdit(crm, crm);
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        [HttpGet]
        public ActionResult ConfirmarElaboracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
            crm.CRM1_IN_STATUS = 1;
            Int32 volta = baseApp.ValidateEdit(crm, crm);
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        [HttpGet]
        public ActionResult ConfirmarEtapaAnterior()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Volta etapa anterior
            CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
            Int32 etapaAtual = crm.CRM1_IN_STATUS;
            if (etapaAtual == 1 || crm.CRM1_DT_ENCERRAMENTO != null)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            Int32 novaEtapa = etapaAtual - 1;
            crm.CRM1_IN_STATUS = novaEtapa;
            Int32 volta = baseApp.ValidateEdit(crm, crm);

            // Processa mensagem
            FUNIL_ETAPA etapa = funApp.GetEtapaById(novaEtapa);
            FUNIL funil = funApp.GetItemById(crm.FUNI_CD_ID.Value);
            if (etapa.FUET_IN_EMAIL == 1)
            {
                if (funil.FUNI_IN_CLIENTE == 1)
                {
                    MensagemViewModel vm = new MensagemViewModel();
                    vm.ASSI_CD_ID = idAss;
                    vm.MENS_DT_CRIACAO = DateTime.Now;
                    vm.MENS_IN_ATIVO = 1;
                    vm.NOME = null;
                    vm.ID = crm.CLIE_CD_ID;
                    vm.MODELO = null;
                    vm.USUA_CD_ID = usuario.USUA_CD_ID;
                    vm.MENS_NM_CABECALHO = null;
                    vm.MENS_NM_RODAPE = null;
                    vm.MENS_IN_TIPO = 1;
                    vm.MENS_TX_TEXTO = "Mudança de Status do processo " + crm.CRM1_NM_NOME + " do cliente " + crm.CLIENTE.CLIE_NM_NOME + " para " + etapa.FUET_NM_NOME;

                    Int32 volta1 = ProcessaEnvioEMailGeral(vm, usuario);
                }
                if (funil.FUNI_IN_RESPONSAVEL == 1)
                {
                    if (crm.USUA_CD_ID != usuario.USUA_CD_ID)
                    {
                        MensagemViewModel vm = new MensagemViewModel();
                        vm.ASSI_CD_ID = idAss;
                        vm.MENS_DT_CRIACAO = DateTime.Now;
                        vm.MENS_IN_ATIVO = 1;
                        vm.NOME = null;
                        vm.ID = crm.USUA_CD_ID;
                        vm.MODELO = null;
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                        vm.MENS_NM_CABECALHO = null;
                        vm.MENS_NM_RODAPE = null;
                        vm.MENS_IN_TIPO = 1;
                        vm.MENS_TX_TEXTO = "Mudança de Status do processo " + crm.CRM1_NM_NOME + " do cliente " + crm.CLIENTE.CLIE_NM_NOME + " para " + etapa.FUET_NM_NOME;

                        Int32 volta1 = ProcessaEnvioEMailGeral(vm, usuario);
                    }
                }
            }
            if (etapa.FUET_IN_SMS == 1)
            {
                if (funil.FUNI_IN_CLIENTE == 1)
                {
                    MensagemViewModel vm = new MensagemViewModel();
                    vm.ASSI_CD_ID = idAss;
                    vm.MENS_DT_CRIACAO = DateTime.Now;
                    vm.MENS_IN_ATIVO = 1;
                    vm.NOME = null;
                    vm.ID = crm.CLIE_CD_ID;
                    vm.MODELO = null;
                    vm.USUA_CD_ID = usuario.USUA_CD_ID;
                    vm.MENS_NM_CABECALHO = null;
                    vm.MENS_NM_RODAPE = null;
                    vm.MENS_IN_TIPO = 2;
                    vm.MENS_TX_TEXTO = "Mudança de Status do processo " + crm.CRM1_NM_NOME + " do cliente " + crm.CLIENTE.CLIE_NM_NOME + " para " + etapa.FUET_NM_NOME;

                    Int32 volta1 = ProcessaEnvioSMSGeral(vm, usuario);
                }
                if (funil.FUNI_IN_RESPONSAVEL == 1)
                {
                    if (crm.USUA_CD_ID != usuario.USUA_CD_ID)
                    {
                        MensagemViewModel vm = new MensagemViewModel();
                        vm.ASSI_CD_ID = idAss;
                        vm.MENS_DT_CRIACAO = DateTime.Now;
                        vm.MENS_IN_ATIVO = 1;
                        vm.NOME = null;
                        vm.ID = crm.USUA_CD_ID;
                        vm.MODELO = null;
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                        vm.MENS_NM_CABECALHO = null;
                        vm.MENS_NM_RODAPE = null;
                        vm.MENS_IN_TIPO = 2;
                        vm.MENS_TX_TEXTO = "Mudança de Status do processo " + crm.CRM1_NM_NOME + " do cliente " + crm.CLIENTE.CLIE_NM_NOME + " para " + etapa.FUET_NM_NOME;

                        Int32 volta1 = ProcessaEnvioSMSGeral(vm, usuario);
                    }
                }
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        [HttpGet]
        public ActionResult ConfirmarEtapaProxima()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa etapa
            CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
            Int32 etapaAtual = crm.CRM1_IN_STATUS;
            Int32 etapas = (Int32)Session["NumEtapas"];
            if (etapaAtual == etapas)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            Int32 novaEtapa = etapaAtual + 1;
            crm.CRM1_IN_STATUS = novaEtapa;
            Int32 volta = baseApp.ValidateEdit(crm, crm);

            // Processa mensagem
            FUNIL_ETAPA etapa = funApp.GetEtapaById(novaEtapa);
            FUNIL funil = funApp.GetItemById(crm.FUNI_CD_ID.Value);
            if (etapa.FUET_IN_EMAIL == 1)
            {
                if (funil.FUNI_IN_CLIENTE == 1)
                {
                    MensagemViewModel vm = new MensagemViewModel();
                    vm.ASSI_CD_ID = idAss;
                    vm.MENS_DT_CRIACAO = DateTime.Now;
                    vm.MENS_IN_ATIVO = 1;
                    vm.NOME = null;
                    vm.ID = crm.CLIE_CD_ID;
                    vm.MODELO = null;
                    vm.USUA_CD_ID = usuario.USUA_CD_ID;
                    vm.MENS_NM_CABECALHO = null;
                    vm.MENS_NM_RODAPE = null;
                    vm.MENS_IN_TIPO = 1;
                    vm.MENS_TX_TEXTO = "Mudança de Status do processo <b style='color: green'>" + crm.CRM1_NM_NOME + "</b> do cliente <b style='color: green'>" + crm.CLIENTE.CLIE_NM_NOME + "</b> para <b style='color: darkblue'>" + etapa.FUET_NM_NOME + "</b>";

                    Int32 volta1 = ProcessaEnvioEMailGeral(vm, usuario);
                }
                if (funil.FUNI_IN_RESPONSAVEL == 1)
                {
                    if (crm.USUA_CD_ID != usuario.USUA_CD_ID)
                    {
                        MensagemViewModel vm = new MensagemViewModel();
                        vm.ASSI_CD_ID = idAss;
                        vm.MENS_DT_CRIACAO = DateTime.Now;
                        vm.MENS_IN_ATIVO = 1;
                        vm.NOME = null;
                        vm.ID = crm.USUA_CD_ID;
                        vm.MODELO = null;
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                        vm.MENS_NM_CABECALHO = null;
                        vm.MENS_NM_RODAPE = null;
                        vm.MENS_IN_TIPO = 1;
                        vm.MENS_TX_TEXTO = "Mudança de Status do processo <b style='color: green'>" + crm.CRM1_NM_NOME + "</b> do cliente <b style='color: green'>" + crm.CLIENTE.CLIE_NM_NOME + "</b> para <b style='color: darkblue'>" + etapa.FUET_NM_NOME + "</b>";

                        Int32 volta1 = ProcessaEnvioEMailGeral(vm, usuario);
                    }
                }
            }
            if (etapa.FUET_IN_SMS == 1)
            {
                if (funil.FUNI_IN_CLIENTE == 1)
                {
                    MensagemViewModel vm = new MensagemViewModel();
                    vm.ASSI_CD_ID = idAss;
                    vm.MENS_DT_CRIACAO = DateTime.Now;
                    vm.MENS_IN_ATIVO = 1;
                    vm.NOME = null;
                    vm.ID = crm.CLIE_CD_ID;
                    vm.MODELO = null;
                    vm.USUA_CD_ID = usuario.USUA_CD_ID;
                    vm.MENS_NM_CABECALHO = null;
                    vm.MENS_NM_RODAPE = null;
                    vm.MENS_IN_TIPO = 2;
                    vm.MENS_TX_TEXTO = "Mudança de Status do processo " + crm.CRM1_NM_NOME + " do cliente " + crm.CLIENTE.CLIE_NM_NOME + " para " + etapa.FUET_NM_NOME;

                    Int32 volta1 = ProcessaEnvioSMSGeral(vm, usuario);
                }
                if (funil.FUNI_IN_RESPONSAVEL == 1)
                {
                    if (crm.USUA_CD_ID != usuario.USUA_CD_ID)
                    {
                        MensagemViewModel vm = new MensagemViewModel();
                        vm.ASSI_CD_ID = idAss;
                        vm.MENS_DT_CRIACAO = DateTime.Now;
                        vm.MENS_IN_ATIVO = 1;
                        vm.NOME = null;
                        vm.ID = crm.USUA_CD_ID;
                        vm.MODELO = null;
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                        vm.MENS_NM_CABECALHO = null;
                        vm.MENS_NM_RODAPE = null;
                        vm.MENS_IN_TIPO = 2;
                        vm.MENS_TX_TEXTO = "Mudança de Status do processo " + crm.CRM1_NM_NOME + " do cliente " + crm.CLIENTE.CLIE_NM_NOME + " para " + etapa.FUET_NM_NOME;

                        Int32 volta1 = ProcessaEnvioSMSGeral(vm, usuario);
                    }
                }
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult EditarCR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCRMCR"] = 10;
            return RedirectToAction("EditarCR", "ContaReceber", new { id = id });
        }

        public ActionResult VerCR(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaCRMCR"] = 10;
            return RedirectToAction("VerCR", "ContaReceber", new { id = id });
        }

        [HttpGet]
        public ActionResult EncerrarProcessoCRM(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Motivos = new SelectList(baseApp.GetAllMotivoEncerramento(idAss).OrderBy(p => p.MOEN_NM_NOME), "MOEN_CD_ID", "MOEN_NM_NOME");
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM item = baseApp.GetItemById(id);
            Session["IdCRM"] = item.CRM1_CD_ID;

            // Checa ações
            Session["TemAcao"] = 0;
            if (item.CRM_ACAO.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
            {
                Session["TemAcao"] = 1;
            }

            // Prepara view
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Falhado", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
            vm.CRM1_DT_ENCERRAMENTO = DateTime.Today.Date;
            vm.CRM1_IN_STATUS = (Int32)Session["EtapaEncerra"];
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EncerrarProcessoCRM(CRMViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            ViewBag.Motivos = new SelectList(baseApp.GetAllMotivoEncerramento(idAss).OrderBy(p => p.MOEN_NM_NOME), "MOEN_CD_ID", "MOEN_NM_NOME");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Falhado", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("MontarTelaCRM");
                    }

                    // Atualiza processo
                    item.CRM1_IN_STATUS = (Int32)Session["EtapaEncerra"];
                    Int32 volta1 = baseApp.ValidateEditSimples(item, item, usuario);

                    // Gera diario
                    CLIENTE cli = cliApp.GetItemById(item.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Encerramento de Processo";
                    dia.DIPR_DS_DESCRICAO = "Encerramento de Processo " + item.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Verifica se tem pedido aprovado
                    if (item.CRM1_IN_ATIVO == (Int32)Session["EtapaEncerra"])
                    {
                        CRM crm = baseApp.GetItemById(item.CRM1_CD_ID);
                        CRM_PEDIDO_VENDA pedAprov = crm.CRM_PEDIDO_VENDA.Where(p => p.CRPV_IN_STATUS == 5).FirstOrDefault();
                        Int32 idCRM = crm.CRM1_CD_ID;
                        Session["PedAprov"] = pedAprov;
                        Session["Tipo"] = 2;
                    }

                    // Listas
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRM1_CD_ID;
                    Session["IdCRM"] = item.CRM1_CD_ID;
                    return RedirectToAction("MontarTelaCRM");
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
        public ActionResult EditarProcessoCRM(Int32 id)
        {

            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Monta listas
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            adic.Add(new SelectListItem() { Text = "Faturamento", Value = "6" });
            adic.Add(new SelectListItem() { Text = "Expedição", Value = "7" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");

            // Recupera
            CRM item = baseApp.GetItemById(id);
            Session["CRM"] = item;
            ViewBag.Incluir = (Int32)Session["IncluirCRM"];

            // Mensagens
            if (Session["MensCliente"] != null)
            {
                if ((Int32)Session["MensCRM"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0039", CultureInfo.CurrentCulture));
                }
            }

            // Monta view
            Session["VoltaCRM1"] = 1;
            objetoAntes = item;
            Session["IdCRM"] = id;
            CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarProcessoCRM(CRMViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");

            // Indicadores
            ViewBag.Incluir = (Int32)Session["IncluirCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, (CRM)Session["CRM"], usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 62;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 63;
                        return RedirectToAction("MontarTelaCRM");
                    }

                    // Sucesso
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 0;

                    if (Session["FiltroCRM"] != null)
                    {
                        FiltrarCRM((CRM)Session["FiltroCRM"]);
                    }
                    return RedirectToAction("VoltarBaseCRM");
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
        public ActionResult VisualizarProcessoCRM(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");
            Session["IdCRM"] = id;
            CRM item = baseApp.GetItemById(id);
            CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");

            CRM_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CRM)Session["CRM"];
            Session["Contato"] = item;
            CRMContatoViewModel vm = Mapper.Map<CRM_CONTATO, CRMContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(CRMContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Checa principal
                    CRM_CONTATO cont = (CRM_CONTATO)Session["Contato"];
                    if (cont.CRCO_IN_PRINCIPAL == 0)
                    {
                        if (((CRM)Session["CRM"]).CRM_CONTATO.Where(p => p.CRCO_IN_PRINCIPAL == 1).ToList().Count > 0 & vm.CRCO_IN_PRINCIPAL == 1)
                        {
                            Session["MensCRM"] = 50;
                            return RedirectToAction("VoltarAcompanhamentoCRM");
                        }
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM_CONTATO item = Mapper.Map<CRMContatoViewModel, CRM_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateEditContato(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult ExcluirContato(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRCO_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CRM_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRCO_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        [HttpGet]
        public ActionResult IncluirContato()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");

            CRM_CONTATO item = new CRM_CONTATO();
            CRMContatoViewModel vm = Mapper.Map<CRM_CONTATO, CRMContatoViewModel>(item);
            vm.CRM1_CD_ID = (Int32)Session["IdCRM"];
            vm.CRCO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(CRMContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            List<SelectListItem> princ = new List<SelectListItem>();
            princ.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            princ.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Principal = new SelectList(princ, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Checa principal
                    CRM crm = (CRM)Session["CRM"];
                    if (crm.CRM_CONTATO != null)
                    {
                        if (((CRM)Session["CRM"]).CRM_CONTATO.Where(p => p.CRCO_IN_PRINCIPAL == 1).ToList().Count > 0 & vm.CRCO_IN_PRINCIPAL == 1)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0187", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Executa a operação
                    CRM_CONTATO item = Mapper.Map<CRMContatoViewModel, CRM_CONTATO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult AcompanhamentoProcessoCRM(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Mensagens
            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 42)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0203", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 43)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 44)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 52)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0122", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 53)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0123", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 12)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 82)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0140", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 91)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0146", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 92)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0147", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 93)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0148", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0187", CultureInfo.CurrentCulture));
                }
            }

            // Processa...
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Em Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");

            Session["IdCRM"] = id;
            CRM item = baseApp.GetItemById(id);
            CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
            List<CRM_ACAO> acoes = item.CRM_ACAO.ToList().OrderByDescending(p => p.CRAC_DT_CRIACAO).ToList();
            CRM_ACAO acao = acoes.Where(p => p.CRAC_IN_STATUS == 1).FirstOrDefault();

            List<CRM_PEDIDO_VENDA> peds = baseApp.GetAllPedidos(idAss).Where(p => p.CRM1_CD_ID == item.CRM1_CD_ID).ToList();
            CRM_PEDIDO_VENDA ped = peds.Where(p => p.CRPV_IN_STATUS == 2 || p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 5).FirstOrDefault();
            CRM_PEDIDO_VENDA pedAprov = peds.Where(p => p.CRPV_IN_STATUS == 5).FirstOrDefault();
            Session["PedAprov"] = pedAprov;
            ViewBag.PedAprov = pedAprov;
            Session["SegueInclusao"] = 0;
            Session["Tipo"] = 0;
            Session["TipoHistorico"] = 1;

            // Recupera dados do funil
            FUNIL funil = funApp.GetItemById(item.FUNI_CD_ID.Value);
            Session["TemProposta"] = funil.FUNI_IN_PROPOSTA;
            Session["Funil"] = funil.FUNI_NM_NOME;
            List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.ToList();
            ViewBag.Etapas = etapas.Count;
            Session["NumEtapas"] = etapas.Count;

            Int32 atual = item.CRM1_IN_STATUS;
            FUNIL_ETAPA etapaAtual = etapas.Where(p => p.FUET_IN_ORDEM == atual).FirstOrDefault();
            String nomeEtapa = etapaAtual.FUET_NM_NOME;
            ViewBag.NomeEtapa = nomeEtapa;
            Session["EtapaAtual"] = atual;
            ViewBag.EtapaProposta = etapaAtual.FUET_IN_PROPOSTA;

            Int32 encerra = etapaAtual.FUET_IN_ENCERRA;
            ViewBag.Encerra = encerra;
            Int32? etapaEncerra = etapas.Where(p => p.FUET_IN_ENCERRA == 1).FirstOrDefault().FUET_IN_ORDEM;
            Session["EtapaEncerra"] = etapaEncerra;
            ViewBag.EtapaEncerra = etapaEncerra;

            // Sessões
            Session["Acoes"] = acoes;
            Session["Peds"] = peds;
            Session["CRM"] = item;
            Session["VoltaCRM"] = 11;
            Session["VoltaAgendaCRMCalend"] = 10;
            Session["ClienteCRM"] = item.CLIENTE;
            ViewBag.Acoes = acoes;
            ViewBag.Acao = acao;
            ViewBag.Peds = peds;
            ViewBag.Ped = ped;
            Session["PontoAcao"] = 2;
            Session["PontoProposta"] = 2;
            Session["SegueInclusao"] = 0;

            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AcompanhamentoProcessoCRM(CRMViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Origem = new SelectList(baseApp.GetAllOrigens(idAss).OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta Apresentada", Value = "3" });
            status.Add(new SelectListItem() { Text = "Em Negociação", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "5" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Arquivados", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Falhados", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
            ViewBag.Adic = new SelectList(adic, "Value", "Text");
            List<SelectListItem> fav = new List<SelectListItem>();
            fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Favorito = new SelectList(fav, "Value", "Text");
            List<SelectListItem> temp = new List<SelectListItem>();
            temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
            temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
            temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
            temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
            ViewBag.Temp = new SelectList(temp, "Value", "Text");

            // Indicadores
            ViewBag.Incluir = (Int32)Session["IncluirCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, (CRM)Session["CRM"], usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 62;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 63;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }

                    // Sucesso
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 0;
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }
            }
            else
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
        }

        public ActionResult GerarRelatorioDetalheCRM()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExpedicaoProcessoCRM(CRMViewModel vm)
        //{
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Criticas
        //            if ((Int32)Session["FaseExpedicao"] == 1)
        //            {
        //                if (vm.TRAN_CD_ID == null)
        //                {
        //                    Session["MensCRM"] = 91;
        //                    return RedirectToAction("VoltarAcompanhamentoCRM");
        //                }
        //                if (vm.CRM1_DT_PREVISAO_ENTREGA == null)
        //                {
        //                    Session["MensCRM"] = 92;
        //                    return RedirectToAction("VoltarAcompanhamentoCRM");
        //                }
        //            }
        //            else
        //            {
        //                if (vm.CRM1_DT_DATA_SAIDA == null)
        //                {
        //                    Session["MensCRM"] = 93;
        //                    return RedirectToAction("VoltarAcompanhamentoCRM");
        //                }
        //            }

        //            // Confirma entrega
        //            if (vm.Entrega)
        //            {
        //                vm.CRM1_IN_ATIVO = 8;
        //            }

        //            // Executa a operação
        //            USUARIO usuario = (USUARIO)Session["UserCredentials"];
        //            CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
        //            Int32 volta = baseApp.ValidateEdit(item, (CRM)Session["CRM"], usuario);

        //            if (vm.Entrega)
        //            {
        //                // Baixa de estoque
        //                List<CRM_PEDIDO_VENDA> listaPed = baseApp.GetAllPedidos(idAss).Where(p => p.CRM1_CD_ID == item.CRM1_CD_ID).ToList();
        //                if (listaPed.Count > 0)
        //                {
        //                    CRM_PEDIDO_VENDA ped = baseApp.GetPedidoById(listaPed.Where(p => p.CRPV_IN_STATUS == 5).First().CRPV_CD_ID);
        //                    if (ped != null)
        //                    {
        //                        List<CRM_PEDIDO_VENDA_ITEM> prods = ped.CRM_PEDIDO_VENDA_ITEM.ToList();
        //                        foreach (CRM_PEDIDO_VENDA_ITEM prod in prods)
        //                        {
        //                            if (prod.CRPI_IN_TIPO_ITEM == 1)
        //                            {
        //                                PRODUTO_ESTOQUE_FILIAL pef = estApp.GetByProdFilial(prod.PROD_CD_ID.Value, ped.FILI_CD_ID);
        //                                pef.PREF_QN_QUANTIDADE_RESERVADA = pef.PREF_QN_QUANTIDADE_RESERVADA - prod.CRPI_IN_QUANTIDADE;
        //                                pef.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE - prod.CRPI_IN_QUANTIDADE;
        //                                Int32 volta1 = estApp.ValidateEdit(pef, pef, usuario);

        //                                // Grava movimentação
        //                                MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
        //                                mov.ASSI_CD_ID = usuario.ASSI_CD_ID;
        //                                mov.FILI_CD_ID = ped.FILI_CD_ID;
        //                                mov.PROD_CD_ID = prod.PROD_CD_ID.Value;
        //                                mov.USUA_CD_ID = usuario.USUA_CD_ID;
        //                                mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
        //                                mov.MOEP_DS_JUSTIFICATIVA = "Pedido Número: " + ped.CRPV_IN_NUMERO_GERADO.ToString();
        //                                mov.MOEP_IN_ATIVO = 1;
        //                                mov.MOEP_IN_CHAVE_ORIGEM = 4;
        //                                mov.MOEP_IN_ORIGEM = "-";
        //                                mov.MOEP_IN_OPERACAO = 2;
        //                                mov.MOEP_QN_QUANTIDADE = prod.CRPI_IN_QUANTIDADE;
        //                                Int32 volta3 = meApp.ValidateCreate(mov, usuario);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            // Sucesso
        //            listaMaster = new List<CRM>();
        //            Session["ListaCRM"] = null;
        //            Session["IncluirCRM"] = 0;
        //            Session["FaseExpedicao"] = 2;
        //            if (vm.Entrega)
        //            {
        //                return RedirectToAction("VoltarBaseCRM");
        //            }
        //            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        //    }
        //}

        [HttpGet]
        public ActionResult EnviarEMailContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 crm = (Int32)Session["IdCRM"];
            CRM item = baseApp.GetItemById(crm);
            CRM_CONTATO cont = baseApp.GetContatoById(id);
            Session["Contato"] = cont;
            ViewBag.Contato = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.CRCO_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.CRCO_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailContato(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailContato(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
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
        public ActionResult EnviarSMSContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 crm = (Int32)Session["IdCRM"];
            CRM item = baseApp.GetItemById(crm);
            CRM_CONTATO cont = baseApp.GetContatoById(id);
            Session["Contato"] = cont;
            ViewBag.Contato = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.CRCO_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.CRCO_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSContato(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSContato(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
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
        public ActionResult EnviarSMSCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 crm = (Int32)Session["IdCRM"];
            CRM item = baseApp.GetItemById(crm);
            CLIENTE cont = cliApp.GetItemById(id);
            Session["Cliente"] = cont;
            ViewBag.Cliente = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.CLIE_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.CLIE_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSCliente(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
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
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
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
        public ActionResult IncluirComentarioCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdCRM"];
            CRM item = baseApp.GetItemById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            CRM_COMENTARIO coment = new CRM_COMENTARIO();
            CRMComentarioViewModel vm = Mapper.Map<CRM_COMENTARIO, CRMComentarioViewModel>(coment);
            vm.CRCM_DT_COMENTARIO = DateTime.Now;
            vm.CRCM_IN_ATIVO = 1;
            vm.CRCM_CD_ID = item.CRM1_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentarioCRM(CRMComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_COMENTARIO item = Mapper.Map<CRMComentarioViewModel, CRM_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM not = baseApp.GetItemById(idNot);

                    item.USUARIO = null;
                    not.CRM_COMENTARIO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Gera diario
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRCM_CD_ID = item.CRCM_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Comentário de Processo";
                    dia.DIPR_DS_DESCRICAO = "Comentário de Processo " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
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
        public ActionResult EditarAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode editar ação
            CRM_ACAO item = baseApp.GetAcaoById(id);
            if (item.CRAC_IN_STATUS > 2)
            {
                Session["MensCRM"] = 43;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            // Prepara view
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipoAcao(idAss).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            // Monta Status
            List<SelectListItem> status = new List<SelectListItem>();
            if (item.CRAC_IN_STATUS == 1)
            {
                status.Add(new SelectListItem() { Text = "Pendente", Value = "2" });
                status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
            }
            else if (item.CRAC_IN_STATUS == 2)
            {
                status.Add(new SelectListItem() { Text = "Ativa", Value = "1" });
                status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
            }

            // Processa
            objetoAntes = (CRM)Session["CRM"];
            CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAcao(CRMAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipoAcao(idAss).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_ACAO item = Mapper.Map<CRMAcaoViewModel, CRM_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEditAcao(item);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRAC_CD_ID = item.CRAC_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Alteração de Ação";
                    dia.DIPR_DS_DESCRICAO = "Alteração de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Verifica retorno
                    Session["ListaCRM"] = null;
                    return RedirectToAction("VoltarAcaoCRM");
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
        public ActionResult ExcluirAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            CRM_ACAO item = baseApp.GetAcaoById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRAC_IN_ATIVO = 0;
            item.CRAC_IN_STATUS = 4;
            Int32 volta = baseApp.ValidateEditAcao(item);

            // Gera diario
            CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
            CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
            DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
            dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
            dia.USUA_CD_ID = usuario.USUA_CD_ID;
            dia.DIPR_DT_DATA = DateTime.Today.Date;
            dia.CRM1_CD_ID = item.CRM1_CD_ID;
            dia.CRAC_CD_ID = item.CRAC_CD_ID;
            dia.DIPR_NM_OPERACAO = "Exclusão de Ação";
            dia.DIPR_DS_DESCRICAO = "Exclusão de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
            Int32 volta3 = diaApp.ValidateCreate(dia);

            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        [HttpGet]
        public ActionResult ReativarAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = baseApp.GetAllAcoes(idAss).Count;
            if ((Int32)Session["NumAcoes"] <= num)
            {
                Session["MensCRM"] = 51;
                return RedirectToAction("MontarTelaCRM", "CRM");
            }

            // Verifica se pode reativar ação
            List<CRM_ACAO> acoes = (List<CRM_ACAO>)Session["Acoes"];
            if (acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
            {
                Session["MensCRM"] = 44;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            // Processa
            CRM_ACAO item = baseApp.GetAcaoById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRAC_IN_ATIVO = 1;
            item.CRAC_IN_STATUS = 1;
            Int32 volta = baseApp.ValidateEditAcao(item);

            // Gera diario
            CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
            CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
            DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
            dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
            dia.USUA_CD_ID = usuario.USUA_CD_ID;
            dia.DIPR_DT_DATA = DateTime.Today.Date;
            dia.CRM1_CD_ID = item.CRM1_CD_ID;
            dia.CRAC_CD_ID = item.CRAC_CD_ID;
            dia.DIPR_NM_OPERACAO = "Reativação de Ação";
            dia.DIPR_DS_DESCRICAO = "Reativação de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
            Int32 volta3 = diaApp.ValidateCreate(dia);

            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        [HttpGet]
        public ActionResult ConsultarEstoque()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaClienteCRM"] = 1;
            return RedirectToAction("MontarTelaEstoqueProduto", "Estoque");
        }

        [HttpGet]
        public ActionResult ConsultarCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaClienteCRM"] = 5;
            return RedirectToAction("EditarCliente", new { id = id });
        }

        [HttpGet]
        public ActionResult EncerrarAcaoNova(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            CRM_ACAO item = baseApp.GetAcaoById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRAC_IN_ATIVO = 0;
            item.CRAC_IN_STATUS = 3;
            Int32 volta = baseApp.ValidateEditAcao(item);

            // Gera diario
            CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
            CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
            DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
            dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
            dia.USUA_CD_ID = usuario.USUA_CD_ID;
            dia.DIPR_DT_DATA = DateTime.Today.Date;
            dia.CRM1_CD_ID = item.CRM1_CD_ID;
            dia.CRAC_CD_ID = item.CRAC_CD_ID;
            dia.DIPR_NM_OPERACAO = "Encerramento de Ação";
            dia.DIPR_DS_DESCRICAO = "Encerramento de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
            Int32 volta3 = diaApp.ValidateCreate(dia);

            return RedirectToAction("VoltarAcompanhamentoCRMBase");
        }

        [HttpGet]
        public ActionResult EditarCliente(Int32 id)
        {
            Session["VoltaCRM"] = 11;
            Session["IdCliente"] = id;
            return RedirectToAction("VoltarAnexoCliente", "Cliente");
        }

        [HttpGet]
        public ActionResult EditarAgenda(Int32 id)
        {
            Session["VoltaAgenda"] = 22;
            Session["IdVolta"] = id;
            Session["FiltroAgendaCalendario"] = 0;
            return RedirectToAction("VoltarAnexoAgenda", "Agenda");
        }

        public ActionResult VerAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            CRM_ACAO item = baseApp.GetAcaoById(id);
            objetoAntes = (CRM)Session["CRM"];
            CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
            return View(vm);
        }

        public ActionResult VerAcoesUsuarioCRM()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            List<CRM_ACAO> lista = baseApp.GetAllAcoes(idAss).Where(p => p.USUA_CD_ID2 == usuario.USUA_CD_ID & p.CRM.CRM1_IN_ATIVO > 0).OrderByDescending(m => m.CRAC_DT_PREVISTA).ToList();
            List<CRM_ACAO> totalPendente = lista.Where(p => p.CRAC_IN_STATUS == 2).OrderByDescending(m => m.CRAC_DT_PREVISTA).ToList();
            List<CRM_ACAO> totalEncerrada = lista.Where(p => p.CRAC_IN_STATUS == 3).OrderByDescending(m => m.CRAC_DT_PREVISTA).ToList();
            List<CRM_ACAO> totalAtrasada= lista.Where(p => p.CRAC_IN_STATUS != 3 & p.CRAC_DT_PREVISTA < DateTime.Today.Date).OrderByDescending(m => m.CRAC_DT_PREVISTA).ToList();

            if ((Int32)Session["AcoesUsuario"] == 1)
            {
                ViewBag.Lista = lista;
                ViewBag.TotalAcoes = lista.Count;
            }
            if ((Int32)Session["AcoesUsuario"] == 2)
            {
                ViewBag.Lista = totalPendente;
                ViewBag.TotalAcoes = totalPendente.Count;
            }
            if ((Int32)Session["AcoesUsuario"] == 3)
            {
                ViewBag.Lista = totalEncerrada;
                ViewBag.TotalAcoes = totalEncerrada.Count;
            }
            if ((Int32)Session["AcoesUsuario"] == 4)
            {
                ViewBag.Lista = totalAtrasada;
                ViewBag.TotalAcoes = totalAtrasada.Count;
            }

            ViewBag.TotalPendentes = totalPendente.Count;
            ViewBag.TotalEncerradas = totalEncerrada.Count;
            ViewBag.TotalAtrasadas = totalAtrasada.Count;

            ViewBag.Nome = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
            ViewBag.Foto = usuario.USUA_AQ_FOTO;
            ViewBag.Cargo = usuario.CARGO.CARG_NM_NOME;
            //Session["PontoAcao"] = 1;
            return View();
        }

        public ActionResult VerAcoesUsuarioCRMPrevia()
        {
            Session["AcoesUsuario"] = 1;
            Session["PontoAcao"] = 100;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerAcoesUsuarioCRMPendentePrevia()
        {
            Session["AcoesUsuario"] = 2;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerAcoesUsuarioCRMPrevia1()
        {
            Session["AcoesUsuario"] = 1;
            Session["PontoAcao"] = 101;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerAcoesUsuarioCRMEncerradaPrevia()
        {
            Session["AcoesUsuario"] = 3;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerAcoesUsuarioCRMAtrasadaPrevia()
        {
            Session["AcoesUsuario"] = 4;
            return RedirectToAction("VerAcoesUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMPrevia()
        {
            Session["PropostasUsuario"] = 1;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPedidosUsuarioCRMPrevia()
        {
            Session["PedidosUsuario"] = 1;
            Session["SegueInclusao"] = 0;
            Session["VoltaPedido"] = 10;
            Session["PontoProposta"] = 0;
            return RedirectToAction("VerPedidosUsuarioCRM");
        }

        public ActionResult EditarClienteForm(Int32 id)
        {
            Session["VoltaClienteCRM"] = 2;
            return RedirectToAction("EditarCliente", new { id = id});
        }

        public ActionResult VerPropostasUsuarioCRMElaboracaoPrevia()
        {
            Session["PropostasUsuario"] = 2;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPedidosUsuarioCRMElaboracaoPrevia()
        {
            Session["PedidosUsuario"] = 2;
            return RedirectToAction("VerPedidosUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMEnviadaPrevia()
        {
            Session["PropostasUsuario"] = 3;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPedidosUsuarioCRMEnviadaPrevia()
        {
            Session["PedidosUsuario"] = 3;
            return RedirectToAction("VerPedidosUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMAprovadaPrevia()
        {
            Session["PropostasUsuario"] = 6;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPedidosUsuarioCRMAprovadaPrevia()
        {
            Session["PedidosUsuario"] = 6;
            return RedirectToAction("VerPedidosUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMReprovadaPrevia()
        {
            Session["PropostasUsuario"] = 5;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMCanceladaPrevia()
        {
            Session["PropostasUsuario"] = 4;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        public ActionResult VerPropostasUsuarioCRMencerradaPrevia()
        {
            Session["PropostasUsuario"] = 7;
            return RedirectToAction("VerPropostasUsuarioCRM");
        }

        [HttpGet]
        public ActionResult IncluirAcao()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = baseApp.GetAllAcoes(idAss).Count;
            if ((Int32)Session["NumAcoes"] <= num)
            {
                Session["MensCRM"] = 51;
                return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
            }

            // Verifica se pode incluir ação
            List<CRM_ACAO> acoes = (List<CRM_ACAO>)Session["Acoes"];
            if (acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
            {
                Session["MensCRM"] = 42;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            // Prepara view
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipoAcao(idAss).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> agenda = new List<SelectListItem>();
            agenda.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            agenda.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Agenda = new SelectList(agenda, "Value", "Text");

            CRM_ACAO item = new CRM_ACAO();
            CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
            vm.CRM1_CD_ID = (Int32)Session["IdCRM"];
            vm.CRAC_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.CRAC_DT_CRIACAO = DateTime.Now;
            vm.CRAC_IN_STATUS = 1;
            vm.USUA_CD_ID1 = usuario.USUA_CD_ID;
            vm.CRAC_DT_PREVISTA = DateTime.Now.AddDays(Convert.ToDouble(conf.CONF_NR_DIAS_ACAO));
            vm.CRIA_AGENDA = 2;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcao(CRMAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipoAcao(idAss).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> agenda = new List<SelectListItem>();
            agenda.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            agenda.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Agenda = new SelectList(agenda, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica tipo de ação
                    if (vm.TIAC_CD_ID == null || vm.TIAC_CD_ID == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0142", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.USUA_CD_ID2 == null || vm.USUA_CD_ID2 == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0143", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    CRM_ACAO item = Mapper.Map<CRMAcaoViewModel, CRM_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateAcao(item, usuarioLogado);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRAC_CD_ID = item.CRAC_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Criação de Ação";
                    dia.DIPR_DS_DESCRICAO = "Criação de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Processa agenda
                    if (vm.CRIA_AGENDA == 1)
                    {
                        AGENDA ag = new AGENDA();
                        ag.AGEN_DS_DESCRICAO = "Ação: " + vm.CRAC_DS_DESCRICAO;
                        ag.AGEN_DT_DATA = vm.CRAC_DT_PREVISTA.Value;
                        ag.AGEN_HR_HORA = vm.CRAC_DT_PREVISTA.Value.TimeOfDay;
                        ag.AGEN_IN_ATIVO = 1;
                        ag.AGEN_IN_STATUS = 1;
                        ag.AGEN_NM_TITULO = vm.CRAC_NM_TITULO;
                        ag.ASSI_CD_ID = idAss;
                        ag.CAAG_CD_ID = 1;
                        ag.AGEN_CD_USUARIO = vm.USUA_CD_ID2;
                        ag.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                        ag.CRM1_CD_ID = item.CRM1_CD_ID;
                        Int32 voltaAg = ageApp.ValidateCreate(ag, usuarioLogado);

                        // Gera diario
                        dia = new DIARIO_PROCESSO();
                        dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                        dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                        dia.DIPR_DT_DATA = DateTime.Today.Date;
                        dia.CRM1_CD_ID = item.CRM1_CD_ID;
                        dia.CRAC_CD_ID = item.CRAC_CD_ID;
                        dia.AGEN_CD_ID = ag.AGEN_CD_ID;
                        dia.DIPR_NM_OPERACAO = "Agendamento de Ação";
                        dia.DIPR_DS_DESCRICAO = "Agendamento de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME + ". Data: " + ag.AGEN_DT_DATA.ToLongDateString();
                        Int32 volta4 = diaApp.ValidateCreate(dia);

                    }

                    // Verifica retorno
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
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
        public Int32 ProcessaEnvioSMSContato(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CRM_CONTATO cont = (CRM_CONTATO)Session["Contato"];

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
                String listaDest = "55" + Regex.Replace(cont.CRCO_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSGeral(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cont = null;
            USUARIO usu = null;
            if (vm.MENS_IN_TIPO == 1)
            {
                cont = cliApp.GetItemById(vm.ID.Value);
            }
            if (vm.MENS_IN_TIPO == 2)
            {
                usu = usuApp.GetItemById(vm.ID.Value);
            }

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
                String listaDest = null;
                if (vm.MENS_IN_TIPO == 1)
                {
                    listaDest = "55" + Regex.Replace(cont.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                }
                if (vm.MENS_IN_TIPO == 2)
                {
                    listaDest = "55" + Regex.Replace(usu.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                }
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailContato(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contato
            Int32 idAss = (Int32)Session["IdAssinante"];
            CRM_CONTATO cont = (CRM_CONTATO)Session["Contato"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + cont.CRCO_NM_NOME + "</b>";

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
            mensagem.EMAIL_TO_DESTINO = cont.CRCO_NM_EMAIL;
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailCliente(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera cliente
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
            mensagem.EMAIL_TO_DESTINO = cont.CLIE_NM_EMAIL;
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailGeral(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera destino
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cont = null;
            USUARIO usu = null;
            if (vm.MENS_IN_TIPO == 1)
            {
                cont = cliApp.GetItemById(vm.ID.Value);
            }
            if (vm.MENS_IN_TIPO == 2)
            {
                usu = usuApp.GetItemById(vm.ID.Value);
            }

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = String.Empty;
            if (vm.MENS_IN_TIPO == 1)
            {
                cab = "Prezado Sr(a). <b>" + cont.CLIE_NM_NOME + "</b>";
            }
            if (vm.MENS_IN_TIPO == 2)
            {
                cab = "Prezado Sr(a). <b>" + usu.USUA_NM_NOME + "</b>";
            }

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br />";
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
            String emailBody = cab + "<br /><br />" + body + "<br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
            Email mensagem = new Email();
            mensagem.ASSUNTO = "Processo - Mudança de Status";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            if (vm.MENS_IN_TIPO == 1)
            {
                mensagem.EMAIL_TO_DESTINO = cont.CLIE_NM_EMAIL;
            }
            if (vm.MENS_IN_TIPO == 2)
            {
                mensagem.EMAIL_TO_DESTINO = usu.USUA_NM_EMAIL;
            }
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

        [HttpPost]
        public JsonResult GetEtapaFunil(int idFunil)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
                        
            //int idFunil = (Int32)Session["IdFunil"];

            //int idFunil = 1;

            // Consulta as etpas do funil

            var listaEtapaFunil = funApp.GetById(idFunil).FUNIL_ETAPA;

            
            List<Hashtable> etapas = new List<Hashtable>();

            foreach(var item in listaEtapaFunil.OrderBy(x=> x.FUET_IN_ORDEM))
            {
                etapas.Add(new Hashtable()
                {
                    {"FUET_NM_NOME",item.FUET_NM_NOME }
                    ,{"FUET_DS_DESCRICAO",item.FUET_DS_DESCRICAO }
                    ,{"FUET_IN_ENCERRA",item.FUET_IN_ENCERRA }
                    ,{"FUET_CD_ID",item.FUET_CD_ID }
                    ,{"FUNI_CD_ID",item.FUNI_CD_ID }
                });
            }

            //Consultar os processos

            var listaProcessos = baseApp.GetAllItens(idAss).Where(p => p.FUNI_CD_ID == idFunil).ToList();
            
            var processos = new List<Hashtable>();

            foreach (var item in listaProcessos)
            {
                var hash = new Hashtable();
                hash.Add("CRM1_IN_STATUS", item.CRM1_IN_STATUS);
                hash.Add("CRM1_CD_ID", item.CRM1_CD_ID);
                hash.Add("CRM1_NM_NOME", item.CRM1_NM_NOME);
                hash.Add("CRM1_NR_TEMPERATURA", item.CRM1_NR_TEMPERATURA);
                hash.Add("CLIE_NM_NOME", item.CLIENTE.CLIE_NM_NOME);
                hash.Add("CLIE_IN_ATIVO", item.CLIENTE.CLIE_IN_ATIVO);
                hash.Add("CROR_NM_NOME", item.CRM_ORIGEM.CROR_NM_NOME);
                hash.Add("CRM1_DT_CRIACAO", item.CRM1_DT_CRIACAO.Value.ToString("dd/MM/yyyy"));
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    hash.Add("CRM1_DT_ENCERRAMENTO", item.CRM1_DT_ENCERRAMENTO.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    hash.Add("CRM1_DT_ENCERRAMENTO", "-");
                }
                hash.Add("CRM1_NM_CLIENTE", item.CLIENTE.CLIE_NM_NOME);
                processos.Add(hash);
            }


            return Json(new 
            {
                etapas = etapas
                ,
                processos = processos
            });
        }

        [HttpPost]
        public JsonResult GetProcessos()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            //listaMaster = baseApp.GetAllItens(idAss).Where(p => p.FUNI_CD_ID == (Int32)Session["IdFunil"]).ToList();
            listaMaster = baseApp.GetAllItens(idAss).Where(p => p.FUNI_CD_ID == 1).ToList();
            var listaHash = new List<Hashtable>();
            foreach (var item in listaMaster)
            {
                var hash = new Hashtable();
                hash.Add("CRM1_IN_STATUS", item.CRM1_IN_STATUS);
                hash.Add("CRM1_CD_ID", item.CRM1_CD_ID);
                hash.Add("CRM1_NM_NOME", item.CRM1_NM_NOME);
                hash.Add("CRM1_DT_CRIACAO", item.CRM1_DT_CRIACAO.Value.ToString("dd/MM/yyyy"));
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    hash.Add("CRM1_DT_ENCERRAMENTO", item.CRM1_DT_ENCERRAMENTO.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    hash.Add("CRM1_DT_ENCERRAMENTO", "-");
                }
                hash.Add("CRM1_NM_CLIENTE", item.CLIENTE.CLIE_NM_NOME);
                listaHash.Add(hash);
            }
            return Json(listaHash);
        }

        [HttpPost]
        public JsonResult EditarStatusCRM(Int32 id, Int32 status, DateTime? dtEnc)
        {
            CRM crm = baseApp.GetById(id);
            crm.CRM1_IN_STATUS = status;
            crm.CRM1_DT_ENCERRAMENTO = dtEnc;
            crm.MOEN_CD_ID = 1;
            crm.CRM1_DS_INFORMACOES_ENCERRAMENTO = "Processo Encerrado";

            //CRM item = new CRM();
            //item.TARE_CD_ID = tarefa.TARE_CD_ID;
            //item.TARE_DS_DESCRICAO = tarefa.TARE_DS_DESCRICAO;
            //item.TARE_DT_CADASTRO = tarefa.TARE_DT_CADASTRO;
            //item.TARE_DT_ESTIMADA = tarefa.TARE_DT_ESTIMADA;
            //item.TARE_DT_REALIZADA = dtEnc;
            //item.TARE_IN_ATIVO = tarefa.TARE_IN_ATIVO;
            //item.TARE_IN_AVISA = tarefa.TARE_IN_AVISA;
            //item.TARE_IN_PRIORIDADE = tarefa.TARE_IN_PRIORIDADE;
            //item.TARE_IN_STATUS = tarefa.TARE_IN_STATUS;
            //item.TARE_NM_LOCAL = tarefa.TARE_NM_LOCAL;
            //item.TARE_NM_TITULO = tarefa.TARE_NM_TITULO;
            //item.TARE_TX_OBSERVACOES = tarefa.TARE_TX_OBSERVACOES;
            //item.TITR_CD_ID = tarefa.TITR_CD_ID;
            //item.USUA_CD_ID = tarefa.USUA_CD_ID;

            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateEdit(crm, crm, usuarioLogado);

                // Gera diario
                CLIENTE cli = cliApp.GetItemById(crm.CLIE_CD_ID);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = crm.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Alteração de Status de Processo";
                dia.DIPR_DS_DESCRICAO = "Alteração de Status de processo " + crm.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Verifica retorno
                if (volta == 1)
                {
                    return Json(CRMSys_Base.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(CRMSys_Base.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                }

                Session["ListaCRM"] = null;
                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Json(ex.Message);
            }
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            // Prepara geração
            CRM aten = baseApp.GetItemById((Int32)Session["IdCRM"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "CRM" + aten.CRM1_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            Font meuFontVerde = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
            Font meuFontAzul= FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
            Font meuFontVermelho = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.RED);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Imagens/base/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Processo CRM - Detalhes", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);

            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Dados do Cliente
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados do Cliente", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.CLIENTE.CLIE_NM_NOME, meuFontVerde));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CLIENTE.CLIE_NM_ENDERECO != null)
            {
                cell = new PdfPCell(new Paragraph("Endereço: " + aten.CLIENTE.CLIE_NM_ENDERECO + " " + aten.CLIENTE.CLIE_NR_NUMERO + " " + aten.CLIENTE.CLIE_NM_COMPLEMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.CLIENTE.UF != null)
                {
                    cell = new PdfPCell(new Paragraph("          " + aten.CLIENTE.CLIE_NM_BAIRRO + " - " + aten.CLIENTE.CLIE_NM_CIDADE + " - " + aten.CLIENTE.UF.UF_SG_SIGLA + " - " + aten.CLIENTE.CLIE_NR_CEP, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("          " + aten.CLIENTE.CLIE_NM_BAIRRO + " - " + aten.CLIENTE.CLIE_NM_CIDADE + " - " + aten.CLIENTE.CLIE_NR_CEP, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Endereço: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Telefone: " + aten.CLIENTE.CLIE_NR_TELEFONE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular: " + aten.CLIENTE.CLIE_NR_CELULAR, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("E-Mail: " + aten.CLIENTE.CLIE_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Dados do Processo
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados do Processo", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.CRM1_NM_NOME, meuFontVerde));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.CRM1_IN_STATUS == 1)
            {
                cell = new PdfPCell(new Paragraph("Status: Prospecção", meuFontAzul));
            }
            else if (aten.CRM1_IN_STATUS == 2)
            {
                cell = new PdfPCell(new Paragraph("Status: Contato Realizado", meuFontAzul));
            }
            else if (aten.CRM1_IN_STATUS == 3)
            {
                cell = new PdfPCell(new Paragraph("Status: Proposta Enviada", meuFontAzul));
            }
            else if (aten.CRM1_IN_STATUS == 4)
            {
                cell = new PdfPCell(new Paragraph("Status: Em Negociação", meuFontAzul));
            }
            else if (aten.CRM1_IN_STATUS == 5)
            {
                cell = new PdfPCell(new Paragraph("Status: Encerrado", meuFontAzul));
            }
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Descrição: " + aten.CRM1_DS_DESCRICAO, meuFontVerde));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Informações: " + aten.CRM1_TX_INFORMACOES_GERAIS, meuFontVerde));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Criação: " + aten.CRM1_DT_CRIACAO.Value.ToShortDateString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Responsável: " + aten.USUARIO.USUA_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Origem: " + aten.CRM_ORIGEM.CROR_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CRM1_IN_ATIVO == 3)
            {
                cell = new PdfPCell(new Paragraph("Data Cancelamento: " + aten.CRM1_DT_CANCELAMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Motivo: " + aten.MOTIVO_CANCELAMENTO.MOCA_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Descrição: " + aten.CRM1_DS_MOTIVO_CANCELAMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.CRM1_IN_STATUS == 5)
            {
                cell = new PdfPCell(new Paragraph("Data Encerramento: " + aten.CRM1_DT_ENCERRAMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Motivo: " + aten.MOTIVO_ENCERRAMENTO.MOEN_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Descrição: " + aten.CRM1_DS_INFORMACOES_ENCERRAMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Contatos
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Contatos", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CRM_CONTATO.Count > 0)
            {
                table = new PdfPTable(new float[] { 130f, 100f, 100f, 80f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cargo", meuFont))
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
                cell = new PdfPCell(new Paragraph("Telefone", meuFont))
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

                foreach (CRM_CONTATO item in aten.CRM_CONTATO)
                {
                    cell = new PdfPCell(new Paragraph(item.CRCO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRCO_NM_CARGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRCO_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRCO_NR_TELEFONE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell = new PdfPCell(new Paragraph(item.CRCO_NR_CELULAR, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Dados Ações
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Ações", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CRM_ACAO.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 80f, 80f, 100f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Título", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Criação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Previsão", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Dias (Prevista)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Status", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CRM_ACAO item in aten.CRM_ACAO)
                {
                    cell = new PdfPCell(new Paragraph(item.CRAC_NM_TITULO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRAC_DT_CRIACAO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.CRAC_DT_PREVISTA > DateTime.Today.Date)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFontVerde))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRAC_DT_PREVISTA == DateTime.Today.Date)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFontAzul))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph(item.CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFontAzul))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);

                    if ((item.CRAC_DT_PREVISTA.Value.Date - DateTime.Today.Date).Days > 0)
                    {
                        cell = new PdfPCell(new Paragraph((item.CRAC_DT_PREVISTA.Value.Date - DateTime.Today.Date).Days.ToString(), meuFontVerde))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph((item.CRAC_DT_PREVISTA.Value.Date - DateTime.Today.Date).Days.ToString(), meuFontVermelho))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);

                    if (item.CRAC_IN_STATUS == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativa", meuFontVerde))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRAC_IN_STATUS == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Pendente", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRAC_IN_STATUS == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Encerrada", meuFontAzul))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRAC_IN_STATUS == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Excluída", meuFontVermelho))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoCRM");
        }

        public ActionResult GerarPropostaLine(Int32 id)
        {
            Session["IdCRMProposta"] = id;
            return RedirectToAction("GerarRelatorioProposta");
        }

        public ActionResult GerarPropostaLinePedido(Int32 id)
        {
            Session["IdCRMPedido"] = id;
            return RedirectToAction("GerarRelatorioPedido");
        }

        public ActionResult GerarRelatorioPedido()
        {
            // Prepara geração
            CRM_PEDIDO_VENDA aten = baseApp.GetPedidoById((Int32)Session["IdCRMPedido"]);
            CRM crm = baseApp.GetItemById(aten.CRM1_CD_ID.Value);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "CRM_Proposta_" + aten.CRPV_IN_NUMERO_GERADO.ToString() + "_" + data + ".pdf";
            CLIENTE cliente = cliApp.GetItemById(crm.CLIE_CD_ID);
            Session["VoltaCRM"] = 0;
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];

            // Define fontes
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            Font meuFontVerde = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.GREEN);
            Font meuFontAzul= FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
            Font meuFontVermelho = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.RED);
            Font meuFontOrange = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.ORANGE);
            Font meuFontTitulo = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            Font meuFontGreen = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY);

            // Preparar campos de texto HTML
            String intro = HtmlToPlainText(aten.CRPV_TX_INTRODUCAO);
            String corpo = HtmlToPlainText(aten.CRPV_TX_INFORMACOES_GERAIS);
            String rodape = HtmlToPlainText(aten.CRPV_TX_OUTROS_ITENS);
            String comercial = HtmlToPlainText(aten.CRPV_TX_CONDICOES_COMERCIAIS);

            String intro1 = CrossCutting.HtmlToText.ConvertHtml(aten.CRPV_TX_INTRODUCAO);
            String corpo1 = CrossCutting.HtmlToText.ConvertHtml(aten.CRPV_TX_INFORMACOES_GERAIS);
            String rodape1 = CrossCutting.HtmlToText.ConvertHtml(aten.CRPV_TX_OUTROS_ITENS);
            String comercial1 = CrossCutting.HtmlToText.ConvertHtml(aten.CRPV_TX_CONDICOES_COMERCIAIS);

            intro1 = intro1.Replace("\r\n\r\n", "\r\n");
            corpo1 = corpo1.Replace("\r\n\r\n", "\r\n");
            rodape1 = rodape1.Replace("\r\n\r\n", "\r\n");
            comercial1 = comercial1.Replace("\r\n\r\n", "\r\n");

            intro1 = intro1.Replace("{Nome}", cliente.CLIE_NM_NOME);
            rodape1 = rodape1.Replace("{Assinatura}", assi.ASSI_NM_NOME);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/CRM_Icon2.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Proposta - Especificações", meuFontTitulo))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Introdução
            Chunk chunk1 = new Chunk(intro1, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);
            Chunk chunk21 = new Chunk(corpo1, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk21);
            Chunk chunk22 = new Chunk(rodape1, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk22);

            // Dados do Cliente
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados do Cliente", meuFontGreen));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("    ", meuFontOrange));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + cliente.CLIE_NM_NOME, meuFontAzul));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (cliente.CLIE_NM_ENDERECO != null)
            {
                cell = new PdfPCell(new Paragraph("Endereço: " + cliente.CLIE_NM_ENDERECO + " " + cliente.CLIE_NR_NUMERO + " " + cliente.CLIE_NM_COMPLEMENTO, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (cliente.UF != null)
                {
                    cell = new PdfPCell(new Paragraph("                " + cliente.CLIE_NM_BAIRRO + " - " + cliente.CLIE_NM_CIDADE + " - " + cliente.UF.UF_SG_SIGLA + " - " + cliente.CLIE_NR_CEP, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("                " + cliente.CLIE_NM_BAIRRO + " - " + cliente.CLIE_NM_CIDADE + " - " + cliente.CLIE_NR_CEP, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Endereço: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Telefone: " + cliente.CLIE_NR_TELEFONE, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular: " + cliente.CLIE_NR_CELULAR, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("E-Mail: " + cliente.CLIE_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.WHITE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Dados da Proposta
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados da Proposta", meuFontGreen));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("    ", meuFontOrange));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Identificação: " + aten.CRPV_NM_NOME, meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Número: " + aten.CRPV_IN_NUMERO_GERADO.ToString(), meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Emissão: " + aten.CRPV_DT_PEDIDO.ToShortDateString(), meuFont1));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Validade: " + aten.CRPV_DT_VALIDADE.ToShortDateString(), meuFont1));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Responsável: " + aten.USUARIO.USUA_NM_NOME, meuFont1));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.WHITE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Comerciais", meuFontGreen));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            //line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.WHITE, Element.ALIGN_LEFT, 1)));
            //pdfDoc.Add(line1);
            Chunk chunk31 = new Chunk(comercial1, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk31);

            // Dados da Financeiros
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            //cell = new PdfPCell(new Paragraph("    ", meuFontOrange));
            //cell.Border = 0;
            //cell.Colspan = 4;
            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Dados Financeiros", meuFontGreen));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("    ", meuFontOrange));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Valor Proposta (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_VALOR.Value), meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Desconto (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_DESCONTO.Value), meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Frete (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_FRETE.Value), meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Taxas (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_ICMS.Value), meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Total (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.CRPV_VL_TOTAL.Value), meuFontAzul));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);
            text = text.Replace("<p>", "");
            text = text.Replace("</p>", "");
            return text;
        }

        public ActionResult VerCRMExpansao()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("Voltar");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            DateTime inicio = Convert.ToDateTime("01/" + DateTime.Today.Date.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Today.Date.Year.ToString());
            DateTime hoje = DateTime.Today.Date;
            Session["Hoje"] = hoje;
            if (Session["ListaCRMCheia"] == null)
            {
                List<CRM> listaCRMCheia = baseApp.GetAllItensAdm(idAss);
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            if (Session["ListaCRM"] == null)
            {
                List<CRM> listaCRM = baseApp.GetAllItens(idAss);
                Session["ListaCRM"] = listaCRM;
            }
            // Retorna
            return View();
        }

        public JsonResult GetDadosProcessosStatus()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            List<CRM> listaCRMCheia = new List<CRM>();

            if (Session["ListaCRMCheia"] == null)
            {
                listaCRMCheia = baseApp.GetAllItensAdm(idAss);
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            else
            {
                listaCRMCheia = (List<CRM>)Session["ListaCRMCheia"];
            }

            // Prepara
            List<CRMDTOViewModel> lista = new List<CRMDTOViewModel>();
            CRMDTOViewModel dto = new CRMDTOViewModel();

            // Carrega vetores
            Int32 prosp = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 1).ToList().Count;
            desc.Add("Prospecção");
            quant.Add(prosp);
            dto.DESCRICAO = "Prospecção";
            dto.QUANTIDADE = prosp;
            lista.Add(dto);

            Int32 cont = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 2).ToList().Count;
            desc.Add("Contato Realizado");
            quant.Add(cont);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Contato Realizado";
            dto.QUANTIDADE = cont;
            lista.Add(dto);

            Int32 prop = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 3).ToList().Count;
            desc.Add("Proposta Enviada");
            quant.Add(prop);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Proposta Enviada";
            dto.QUANTIDADE = prop;
            lista.Add(dto);

            Int32 neg = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 4).ToList().Count;
            desc.Add("Em Negociação");
            quant.Add(neg);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Em Negociação";
            dto.QUANTIDADE = neg;
            lista.Add(dto);

            Int32 enc = listaCRMCheia.Where(p => p.CRM1_IN_STATUS == 5).ToList().Count;
            desc.Add("Encerrado");
            quant.Add(enc);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Encerrado";
            dto.QUANTIDADE = enc;
            lista.Add(dto);
            Session["ListaProcessosStatus"] = lista;

            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            cor.Add("#D63131");
            cor.Add("#27A1C6");

            // retorna
            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosProcessosSituacao()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            List<CRM> listaCRMCheia = new List<CRM>();

            if (Session["ListaCRMCheia"] == null)
            {
                listaCRMCheia = baseApp.GetAllItensAdm(idAss);
                Session["ListaCRMCheia"] = listaCRMCheia;
            }
            else
            {
                listaCRMCheia = (List<CRM>)Session["ListaCRMCheia"];
            }

            // Prepara
            List<CRMDTOViewModel> listaSit = new List<CRMDTOViewModel>();
            CRMDTOViewModel dto = new CRMDTOViewModel();

            // Carrega vetores
            Int32 prosp = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 1).ToList().Count;
            desc.Add("Ativos");
            quant.Add(prosp);
            dto.DESCRICAO = "Ativos";
            dto.QUANTIDADE = prosp;
            listaSit.Add(dto);

            Int32 cont = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 2).ToList().Count;
            desc.Add("Arquivados");
            quant.Add(cont);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Arquivados";
            dto.QUANTIDADE = cont;
            listaSit.Add(dto);

            Int32 prop = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 3).ToList().Count;
            desc.Add("Cancelados");
            quant.Add(prop);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Cancelados";
            dto.QUANTIDADE = prop;
            listaSit.Add(dto);

            Int32 neg = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 4).ToList().Count;
            desc.Add("Falhados");
            quant.Add(neg);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Falhados";
            dto.QUANTIDADE = neg;
            listaSit.Add(dto);

            Int32 enc = listaCRMCheia.Where(p => p.CRM1_IN_ATIVO == 5).ToList().Count;
            desc.Add("Sucesso");
            quant.Add(enc);
            dto = new CRMDTOViewModel();
            dto.DESCRICAO = "Sucesso";
            dto.QUANTIDADE = enc;
            listaSit.Add(dto);
            Session["ListaProcessosSituacao"] = listaSit;

            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            cor.Add("#D63131");
            cor.Add("#27A1C6");

            // retorna
            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public ActionResult VerProcessosStatusExpansao()
        {
            // Prepara view
            List<CRMDTOViewModel> lista = (List<CRMDTOViewModel>)Session["ListaProcessosStatus"];
            ViewBag.Lista = lista;
            return View();
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult EnviarEMailCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            if (Session["MensMensagem"] != null)
            {
                if ((Int32)Session["MensMensagem"] == 66)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                }
            }

            // recupera cliente e assinante
            CLIENTE cli = cliApp.GetItemById(id);
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            Session["Cliente"] = cli;

            // Prepara mensagem
            String header = "Prezado <b>" + cli.CLIE_NM_NOME + "</b>";
            String body = String.Empty;
            String footer = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Monta vm
            MensagemViewModel vm = new MensagemViewModel();
            vm.ASSI_CD_ID = idAss;
            vm.MENS_DT_CRIACAO = DateTime.Now;
            vm.MENS_IN_ATIVO = 1;
            vm.NOME = cli.CLIE_NM_NOME;
            vm.ID = id;
            vm.MODELO = cli.CLIE_NM_EMAIL;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.MENS_NM_CABECALHO = header;
            vm.MENS_NM_RODAPE = footer;
            vm.MENS_IN_TIPO = 1;
            vm.ID = cli.CLIE_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarEMailCliente(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
          
            if (ModelState.IsValid)
            {
                Int32 idNot = (Int32)Session["IdCRM"];
                try
                {
                    // Checa corpo da mensagem
                    if (String.IsNullOrEmpty(vm.MENS_TX_TEXTO))
                    {
                        Session["MensMensagem"] = 66;
                        return RedirectToAction("EnviarEMailCliente");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailCliente(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
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
        public ActionResult MontarTelaDashboardCRMNovo()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
                //if ((Int32)Session["PermCRM"] == 0)
                //{
                //    Session["MensPermissao"] = 2;
                //    return RedirectToAction("CarregarBase", "BaseAdmin");
                //}
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera listas
            List<CRM> lt = baseApp.GetAllItens(idAss);
            List<CRM> lm = lt.Where(p => p.CRM1_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.CRM1_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            List<CRM> la = lt.Where(p => p.CRM1_IN_ATIVO == 1).ToList();
            List<CRM> lq = lt.Where(p => p.CRM1_IN_ATIVO == 2).ToList();
            List<CRM> ls = lt.Where(p => p.CRM1_IN_ATIVO == 5).ToList();
            List<CRM> lc = lt.Where(p => p.CRM1_IN_ATIVO == 3).ToList();
            List<CRM> lf = lt.Where(p => p.CRM1_IN_ATIVO == 4).ToList();
            List<CRM> lx = lt.Where(p => p.CRM1_IN_ATIVO == 6).ToList();
            List<CRM> ly = lt.Where(p => p.CRM1_IN_ATIVO == 7).ToList();
            List<CRM> lz = lt.Where(p => p.CRM1_IN_ATIVO == 8).ToList();
            List<CRM_ACAO> acoes = baseApp.GetAllAcoes(idAss);
            List<CRM_ACAO> acoesPend = acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList();
            List<CLIENTE> cli = cliApp.GetAllItens(idAss);
            //List<CRM_PROPOSTA> props = baseApp.GetAllPropostas(idAss);
            //List<CRM_PROPOSTA> lmp = props.Where(p => p.CRPR_DT_PROPOSTA.Month == DateTime.Today.Date.Month & p.CRPR_DT_PROPOSTA.Year == DateTime.Today.Date.Year).ToList();
            List<CRM_PEDIDO_VENDA> peds = baseApp.GetAllPedidos(idAss);
            List<CRM_PEDIDO_VENDA> lmp1 = peds.Where(p => p.CRPV_DT_PEDIDO.Month == DateTime.Today.Date.Month & p.CRPV_DT_PEDIDO.Year == DateTime.Today.Date.Year).ToList();

            // Estatisticas 
            ViewBag.Total = lt.Count;
            ViewBag.TotalAtivo = la.Count;
            ViewBag.TotalSucesso = ls.Count;
            ViewBag.TotalCancelado = lc.Count;
            ViewBag.Acoes = acoes.Count;
            ViewBag.AcoesPend = acoesPend.Count;
            ViewBag.Clientes = cli.Count;

            ViewBag.TotalPes = lt.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.TotalAtivoPes = la.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.TotalSucessoPes = ls.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.TotalCanceladoPes = lc.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.AcoesPes = acoes.Where(p => p.CRM.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;
            ViewBag.AcoesPendPes = acoesPend.Where(p => p.CRM.USUA_CD_ID == usuario.USUA_CD_ID).ToList().Count;

            Session["ListaCRM"] = lt;
            Session["ListaCRMMes"] = lm;
            Session["ListaCRMAtivo"] = la;
            Session["ListaCRMSucesso"] = ls;
            Session["ListaCRMCanc"] = lc;
            Session["ListaCRMAcoes"] = acoes;
            Session["ListaCRMAcoesPend"] = acoesPend;
            //Session["ListaPropostaMes"] = lmp;
            Session["ListaPedidosMes"] = lmp1;

            Session["CRMAtivos"] = la.Count;
            Session["CRMArquivados"] = lq.Count;
            Session["CRMCancelados"] = lc.Count;
            Session["CRMFalhados"] = lf.Count;
            Session["CRMSucessos"] = la.Count;
            Session["CRMFatura"] = lx.Count;
            Session["CRMExpedicao"] = ly.Count;
            Session["CRMEntregue"] = lz.Count;

            Session["CRMProsp"] = lt.Where(p => p.CRM1_IN_STATUS == 1).ToList().Count;
            Session["CRMCont"] =  lt.Where(p => p.CRM1_IN_STATUS == 2).ToList().Count;
            Session["CRMProp"] =  lt.Where(p => p.CRM1_IN_STATUS == 3).ToList().Count;
            Session["CRMNego"] =  lt.Where(p => p.CRM1_IN_STATUS == 4).ToList().Count;
            Session["CRMEnc"] =  lt.Where(p => p.CRM1_IN_STATUS == 5).ToList().Count;
            Session["IdCRM"] = null;

            Session["AcaoAtiva"] = acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count;
            Session["AcaoPendente"] = acoes.Where(p => p.CRAC_IN_STATUS == 2).ToList().Count;
            Session["AcaoEncerrada"] = acoes.Where(p => p.CRAC_IN_STATUS == 3).ToList().Count;
            Int32 x = acoes.Where(p => p.CRAC_IN_STATUS == 3).ToList().Count;
            Int32 y = acoes.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count;

            //Session["PropElaboracao"] = props.Where(p => p.CRPR_IN_STATUS == 1).ToList().Count;
            //Session["PropEnviada"] = props.Where(p => p.CRPR_IN_STATUS == 2).ToList().Count;
            //Session["PropCancelada"] = props.Where(p => p.CRPR_IN_STATUS == 3).ToList().Count;
            //Session["PropReprovada"] = props.Where(p => p.CRPR_IN_STATUS == 4).ToList().Count;
            //Session["PropAprovada"] = props.Where(p => p.CRPR_IN_STATUS == 5).ToList().Count;

            Session["PedElaboracao"] = peds.Where(p => p.CRPV_IN_STATUS == 1).ToList().Count;
            Session["PedEnviada"] = peds.Where(p => p.CRPV_IN_STATUS == 2).ToList().Count;
            Session["PedCancelada"] = peds.Where(p => p.CRPV_IN_STATUS == 3).ToList().Count;
            Session["PedReprovada"] = peds.Where(p => p.CRPV_IN_STATUS == 4).ToList().Count;
            Session["PedAprovada"] = peds.Where(p => p.CRPV_IN_STATUS == 5).ToList().Count;

            // Resumo Mes CRM
            List<DateTime> datas = lm.Select(p => p.CRM1_DT_CRIACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = lm.Where(p => p.CRM1_DT_CRIACAO.Value.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                lista.Add(mod);
            }
            ViewBag.ListaCRMMes = lista;
            ViewBag.ContaCRMMes = lm.Count;
            Session["ListaDatasCRM"] = datas;
            Session["ListaCRMMesResumo"] = lista;

            // Resumo Mes Prposta
            //List<DateTime> datasProp = lmp.Select(p => p.CRPR_DT_PROPOSTA.Date).Distinct().ToList();
            //List<ModeloViewModel> listaProp = new List<ModeloViewModel>();
            //foreach (DateTime item in datasProp)
            //{
            //    Int32 conta = lmp.Where(p => p.CRPR_DT_PROPOSTA.Date == item).Count();
            //    ModeloViewModel mod = new ModeloViewModel();
            //    mod.DataEmissao = item;
            //    mod.Valor = conta;
            //    lista.Add(mod);
            //}
            //ViewBag.ListaPropostaMes = lista;
            //ViewBag.ContaPropostaMes = lmp.Count;
            //Session["ListaDatasProp"] = datasProp;
            //Session["ListaPropostaMesResumo"] = lista;

            // Resumo Mes Pedidos
            List<DateTime> datasPed = lmp1.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
            List<ModeloViewModel> listaPed = new List<ModeloViewModel>();
            foreach (DateTime item in datasPed)
            {
                Int32 conta = lmp1.Where(p => p.CRPV_DT_PEDIDO.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                listaPed.Add(mod);
            }
            ViewBag.ListaPedidosMes = listaPed;
            ViewBag.ContaPedidosMes = lmp1.Count;
            Session["ListaDatasPed"] = datasPed;
            Session["ListaPedidosMesResumo"] = listaPed;

            // Resumo Situacao CRM 
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = lt.Where(p => p.CRM1_IN_ATIVO == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1? "Ativo" : (i == 2 ? "Arquivados" : (i == 3 ? "Cancelados" : (i == 4 ? "Falhados" : (i == 5 ? "Sucesso" : (i == 6 ? "Faturamento" : (i == 7 ? "Expedição" : "Entregue"))))));
                mod.Valor = conta;
                lista1.Add(mod);
            }
            ViewBag.ListaCRMSituacao = lista1;
            Session["ListaCRMSituacao"] = lista1;

            // Resumo Status CRM 
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = lt.Where(p => p.CRM1_IN_STATUS == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1 ? "Prospecção" : (i == 2 ? "Contato Realizado" : (i == 3 ? "Proposta Apresentada" : (i == 4 ? "Negociação" : "Encerrado")));
                mod.Valor = conta;
                lista2.Add(mod);
            }
            ViewBag.ListaCRMStatus = lista2;
            Session["ListaCRMStatus"] = lista2;

            // Resumo ações
            List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
            for (int i = 1; i < 4; i++)
            {
                Int32 conta = acoes.Where(p => p.CRAC_IN_STATUS == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1 ? "Ativa" : (i == 2 ? "Pendente" : "Encerrada");
                mod.Valor = conta;
                lista3.Add(mod);
            }
            ViewBag.ListaCRMAcao = lista3;
            Session["ListaCRMAcao"] = lista3;

            // Resumo Propostas
            //List<ModeloViewModel> lista4 = new List<ModeloViewModel>();
            //for (int i = 1; i < 6; i++)
            //{
            //    Int32 conta = props.Where(p => p.CRPR_IN_STATUS == i).Count();
            //    ModeloViewModel mod = new ModeloViewModel();
            //    mod.Data = i == 1 ? "Em Elaboração" : (i == 2 ? "Enviada" : (i == 3 ? "Cancelada" : (i == 4 ? "Reprovada" : "Aprovada")));
            //    mod.Valor = conta;
            //    lista3.Add(mod);
            //}
            //ViewBag.ListaCRMProp = lista4;
            //Session["ListaCRMProp"] = lista4;

            // Resumo Pedidos
            List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = peds.Where(p => p.CRPV_IN_STATUS == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1 ? "Em Elaboração" : (i == 2 ? "Enviado" : (i == 3 ? "Cancelado" : (i == 4 ? "Reprovado" : "Aprovado")));
                mod.Valor = conta;
                lista5.Add(mod);
            }
            ViewBag.ListaCRMPed = lista5;
            Session["ListaCRMPed"] = lista5;
            Session["VoltaProdutoDash"] = 1;
            return View(vm);
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardVendas()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("MontarTelaCRM", "CRM");
                }
                if ((Int32)Session["PermCRM"] == 0)
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera listas
            List<CRM_PEDIDO_VENDA> peds = baseApp.GetAllPedidosVenda(idAss);
            List<CRM_PEDIDO_VENDA> lmp1 = peds.Where(p => p.CRPV_DT_PEDIDO.Month == DateTime.Today.Date.Month & p.CRPV_DT_PEDIDO.Year == DateTime.Today.Date.Year).ToList();

            List<CRM_PEDIDO_VENDA> pedsPes = peds.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            List<CRM_PEDIDO_VENDA> lmp1Pes = pedsPes.Where(p => p.CRPV_DT_PEDIDO.Month == DateTime.Today.Date.Month & p.CRPV_DT_PEDIDO.Year == DateTime.Today.Date.Year).ToList();

            // Estatisticas 
            Session["ListaPedidosMes"] = lmp1;
            Session["PedElaboracao"] = peds.Where(p => p.CRPV_IN_STATUS == 1).ToList().Count;
            Session["PedCancelada"] = peds.Where(p => p.CRPV_IN_STATUS == 3).ToList().Count;
            Session["PedFatura"] = peds.Where(p => p.CRPV_IN_STATUS == 6).ToList().Count;
            Session["PedExpedicao"] = peds.Where(p => p.CRPV_IN_STATUS == 7).ToList().Count;
            Session["PedEntregue"] = peds.Where(p => p.CRPV_IN_STATUS == 8).ToList().Count;

            Session["ListaPedidosMesPes"] = lmp1Pes;
            Session["PedElaboracaoPes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 1).ToList().Count;
            Session["PedCanceladaPes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 3).ToList().Count;
            Session["PedFaturaPes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 6).ToList().Count;
            Session["PedExpedicaoPes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 7).ToList().Count;
            Session["PedEntreguePes"] = pedsPes.Where(p => p.CRPV_IN_STATUS == 8).ToList().Count;

            ViewBag.Elaboracao = (Int32)Session["PedElaboracao"];
            ViewBag.Cancelado = (Int32)Session["PedCancelada"];
            ViewBag.Fatura = (Int32)Session["PedFatura"];
            ViewBag.Expedicao = (Int32)Session["PedExpedicao"];
            ViewBag.Entregue = (Int32)Session["PedEntregue"];

            ViewBag.ElaboracaoPes = (Int32)Session["PedElaboracaoPes"];
            ViewBag.CanceladoPes = (Int32)Session["PedCanceladaPes"];
            ViewBag.FaturaPes = (Int32)Session["PedFaturaPes"];
            ViewBag.ExpedicaoPes = (Int32)Session["PedExpedicaoPes"];
            ViewBag.EntreguePes = (Int32)Session["PedEntreguePes"];

            // Resumo Mes Pedidos
            List<DateTime> datasPed = lmp1.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
            List<ModeloViewModel> listaPed = new List<ModeloViewModel>();
            foreach (DateTime item in datasPed)
            {
                Int32 conta = lmp1.Where(p => p.CRPV_DT_PEDIDO.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                listaPed.Add(mod);
            }
            ViewBag.ListaPedidosMes = listaPed;
            ViewBag.ContaPedidosMes = lmp1.Count;
            Session["ListaDatasPed"] = datasPed;
            Session["ListaPedidosMesResumo"] = listaPed;

            // Resumo Mes Pedidos Pessoal
            List<DateTime> datasPedPes = lmp1Pes.Select(p => p.CRPV_DT_PEDIDO.Date).Distinct().ToList();
            List<ModeloViewModel> listaPedPes = new List<ModeloViewModel>();
            foreach (DateTime item in datasPedPes)
            {
                Int32 conta = lmp1Pes.Where(p => p.CRPV_DT_PEDIDO.Date == item).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item;
                mod.Valor = conta;
                listaPedPes.Add(mod);
            }
            ViewBag.ListaPedidosMesPes = listaPedPes;
            ViewBag.ContaPedidosMesPes = lmp1Pes.Count;
            Session["ListaDatasPedPes"] = datasPedPes;
            Session["ListaPedidosMesResumoPes"] = listaPedPes;

            // Resumo Pedidos
            List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = peds.Where(p => p.CRPV_IN_STATUS == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1 ? "Em Elaboração" : (i == 2 ? "Enviado" : (i == 3 ? "Cancelado" : (i == 4 ? "Reprovado" : (i == 5 ? "Aprovado" : (i == 6 ? "Faturamento" : (i == 7 ? "Expedição" : "Entregue" ))))));
                mod.Valor = conta;
                lista5.Add(mod);
            }
            ViewBag.ListaCRMPed = lista5;
            Session["ListaCRMPed"] = lista5;

            // Resumo PedidosPessoal
            List<ModeloViewModel> lista5Pes = new List<ModeloViewModel>();
            for (int i = 1; i < 6; i++)
            {
                Int32 conta = pedsPes.Where(p => p.CRPV_IN_STATUS == i).Count();
                ModeloViewModel mod = new ModeloViewModel();
                mod.Data = i == 1 ? "Em Elaboração" : (i == 2 ? "Enviado" : (i == 3 ? "Cancelado" : (i == 4 ? "Reprovado" : (i == 5 ? "Aprovado" : (i == 6 ? "Faturamento" : (i == 7 ? "Expedição" : "Entregue"))))));
                mod.Valor = conta;
                lista5Pes.Add(mod);
            }
            ViewBag.ListaCRMPedPes = lista5Pes;
            Session["ListaCRMPedPes"] = lista5Pes;
            Session["VoltaProdutoDash"] = 1;
            return View(vm);
        }

        public JsonResult GetDadosGraficoCRMSituacao()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["CRMAtivos"];
            Int32 q2 = (Int32)Session["CRMArquivados"];
            Int32 q3 = (Int32)Session["CRMCancelados"];
            Int32 q4 = (Int32)Session["CRMFalhados"];
            Int32 q5 = (Int32)Session["CRMSucessos"];
            Int32 q6 = (Int32)Session["CRMFatura"];
            Int32 q7 = (Int32)Session["CRMExpedicao"];
            Int32 q8 = (Int32)Session["CRMEntregue"];

            desc.Add("Ativos");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Arquivados");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#a0cfff");
            desc.Add("Falhados");
            quant.Add(q4);
            cor.Add("#a5d3d4");
            desc.Add("Sucesso");
            quant.Add(q5);
            cor.Add("#bda5d4");
            desc.Add("Faturamento");
            quant.Add(q6);
            cor.Add("##ffffd8");
            desc.Add("Expedição");
            quant.Add(q7);
            cor.Add("#a5d3d4");
            desc.Add("Entregues");
            quant.Add(q8);
            cor.Add("#a5d3d4");


            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);

        }

        public JsonResult GetDadosGraficoCRMStatus()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["CRMProsp"];
            Int32 q2 = (Int32)Session["CRMCont"];
            Int32 q3 = (Int32)Session["CRMProp"];
            Int32 q4 = (Int32)Session["CRMNego"];
            Int32 q5 = (Int32)Session["CRMEnc"];

            desc.Add("Prospecção");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Contato Realizado");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Proposta Enviada");
            quant.Add(q3);
            cor.Add("#a0cfff");
            desc.Add("Em Negociação");
            quant.Add(q4);
            cor.Add("#bda5d4");
            desc.Add("Encerrado");
            quant.Add(q5);
            cor.Add("#ffffd8");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoAcaoStatus()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["AcaoAtiva"];
            Int32 q2 = (Int32)Session["AcaoPendente"];
            Int32 q3 = (Int32)Session["AcaoEncerrada"];

            desc.Add("Ativas");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Pendentes");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Encerradas");
            quant.Add(q3);
            cor.Add("#a0cfff");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoPropostaStatus()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["PedElaboracao"];
            Int32 q2 = (Int32)Session["PedEnviada"];
            Int32 q3 = (Int32)Session["PedCancelada"];
            Int32 q4 = (Int32)Session["PedReprovada"];
            Int32 q5 = (Int32)Session["PedAprovada"];

            desc.Add("Em Elaboração");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Enviados");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#a0cfff");
            desc.Add("Reprovados");
            quant.Add(q4);
            cor.Add("#bda5d4");
            desc.Add("Aprovados");
            quant.Add(q5);
            cor.Add("#ffffd8");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoVendasStatus()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["PedElaboracao"];
            Int32 q3 = (Int32)Session["PedCancelada"];
            Int32 q6 = (Int32)Session["PedFatura"];
            Int32 q7 = (Int32)Session["PedExpedicao"];
            Int32 q8 = (Int32)Session["PedEntregue"];

            desc.Add("Em Elaboração");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#FF7F00");
            desc.Add("Faturamento");
            quant.Add(q6);
            cor.Add("#359E18");
            desc.Add("Expedição");
            quant.Add(q7);
            cor.Add("#FFAE00");
            desc.Add("Entregues");
            quant.Add(q8);
            cor.Add("#FF7F00");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoVendasStatusPessoal()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["PedElaboracaoPes"];
            Int32 q3 = (Int32)Session["PedCanceladaPes"];
            Int32 q6 = (Int32)Session["PedFaturaPes"];
            Int32 q7 = (Int32)Session["PedExpedicaoPes"];
            Int32 q8 = (Int32)Session["PedEntreguePes"];

            desc.Add("Em Elaboração");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Cancelados");
            quant.Add(q3);
            cor.Add("#FF7F00");
            desc.Add("Faturamento");
            quant.Add(q6);
            cor.Add("#359E18");
            desc.Add("Expedição");
            quant.Add(q7);
            cor.Add("#FFAE00");
            desc.Add("Entregues");
            quant.Add(q8);
            cor.Add("#FF7F00");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCRM()
        {
            List<CRM> listaCP1 = (List<CRM>)Session["ListaCRMMes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasCRM"];
            List<CRM> listaDia = new List<CRM>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.CRM1_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoProposta()
        {
            List<CRM_PEDIDO_VENDA> listaCP1 = (List<CRM_PEDIDO_VENDA>)Session["ListaPedidosMes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasPed"];
            List<CRM_PEDIDO_VENDA> listaDia = new List<CRM_PEDIDO_VENDA>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.CRPV_DT_PEDIDO.Date == item).ToList();
                Int32 contaDia = listaDia.Count;
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoVendas()
        {
            List<CRM_PEDIDO_VENDA> listaCP1 = (List<CRM_PEDIDO_VENDA>)Session["ListaPedidosMes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasPed"];
            List<CRM_PEDIDO_VENDA> listaDia = new List<CRM_PEDIDO_VENDA>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.CRPV_DT_PEDIDO.Date == item).ToList();
                Int32 contaDia = listaDia.Count;
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoVendasPessoal()
        {
            List<CRM_PEDIDO_VENDA> listaCP1 = (List<CRM_PEDIDO_VENDA>)Session["ListaPedidosMesPes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasPedPes"];
            List<CRM_PEDIDO_VENDA> listaDia = new List<CRM_PEDIDO_VENDA>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.CRPV_DT_PEDIDO.Date == item).ToList();
                Int32 contaDia = listaDia.Count;
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult MostrarClientes()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 40;
            return RedirectToAction("MontarTelaCliente", "Cliente");
        }

        public ActionResult MontarTelaCRMKanbaChama()
        {
            // Prepara grid
            return RedirectToAction("MotarTelaCRMKanban_Nova", new { id = 1 });
        }

        public ActionResult MostrarTelaAgendaCalendario()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 50;
            return RedirectToAction("MontarTelaAgendaCalendario", "Agenda");
        }
        
        public ActionResult MostrarIncluirAgenda()
        {
            // Prepara grid
            Session["VoltaAgenda"] = 23;
            return RedirectToAction("IncluirAgenda", "Agenda");
        }

        public ActionResult MostrarClientesVenda()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 50;
            return RedirectToAction("MontarTelaCliente", "Cliente");
        }

        public ActionResult MostrarTransportadoras()
        {
            // Prepara grid
            Session["VoltaTransportadora"] = 40;
            return RedirectToAction("MontarTelaTransportadora", "Transportadora");
        }

        public ActionResult MostrarTransportadorasVenda()
        {
            // Prepara grid
            Session["VoltaTransportadora"] = 50;
            return RedirectToAction("MontarTelaTransportadora", "Transportadora");
        }

        public ActionResult MostrarServicos()
        {
            // Prepara grid
            Session["VoltaServico"] = 40;
            return RedirectToAction("MontarTelaServico", "Servico");
        }

        public ActionResult MostrarServicosVenda()
        {
            // Prepara grid
            Session["VoltaServico"] = 50;
            return RedirectToAction("MontarTelaServico", "Servico");
        }

        public ActionResult MostrarProdutos()
        {
            // Prepara grid
            Session["VoltaProduto"] = 40;
            return RedirectToAction("MontarTelaProduto", "Produto");
        }

        public ActionResult MostrarProdutosVenda()
        {
            // Prepara grid
            Session["VoltaProduto"] = 50;
            return RedirectToAction("MontarTelaProduto", "Produto");
        }

        public ActionResult IncluirClienteRapido()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 40;
            return RedirectToAction("IncluirClienteRapido", "Cliente");
        }

        public ActionResult VerPedidosUsuarioCRM()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            List<CRM_PEDIDO_VENDA> lista = baseApp.GetAllPedidos(idAss).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID & p.CRM.CRM1_IN_ATIVO > 0).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
            List<CRM_PEDIDO_VENDA> totalElaboracao = lista.Where(p => p.CRPV_IN_STATUS == 1).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
            List<CRM_PEDIDO_VENDA> totalEnviado = lista.Where(p => p.CRPV_IN_STATUS == 2).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
            List<CRM_PEDIDO_VENDA> totalCancelado = lista.Where(p => p.CRPV_IN_STATUS == 3).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
            List<CRM_PEDIDO_VENDA> totalAprovado = lista.Where(p => p.CRPV_IN_STATUS == 5).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
            List<CRM_PEDIDO_VENDA> totalReprovado = lista.Where(p => p.CRPV_IN_STATUS == 4).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
            List<CRM_PEDIDO_VENDA> totalEncerrado = lista.Where(p => p.CRPV_IN_STATUS == 6).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
            List<CRM_PEDIDO_VENDA> totalValidade = lista.Where(p => p.CRPV_IN_STATUS < 3 & p.CRPV_DT_VALIDADE < DateTime.Today.Date).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();

            if ((Int32)Session["PedidosUsuario"] == 1)
            {
                ViewBag.Lista = lista;
                ViewBag.TotalPedidos= lista.Count;
            }
            if ((Int32)Session["PedidosUsuario"] == 2)
            {
                ViewBag.Lista = totalElaboracao;
                ViewBag.TotalPedidos = totalElaboracao.Count;
            }
            if ((Int32)Session["PedidosUsuario"] == 3)
            {
                ViewBag.Lista = totalEnviado;
                ViewBag.TotalPedidos = totalEnviado.Count;
            }
            if ((Int32)Session["PedidosUsuario"] == 4)
            {
                ViewBag.Lista = totalCancelado;
                ViewBag.TotalPedidos = totalCancelado.Count;
            }
            if ((Int32)Session["PedidosUsuario"] == 5)
            {
                ViewBag.Lista = totalReprovado;
                ViewBag.TotalPedidos = totalReprovado.Count;
            }
            if ((Int32)Session["PedidosUsuario"] == 6)
            {
                ViewBag.Lista = totalAprovado;
                ViewBag.TotalPedidos = totalAprovado.Count;
            }
            if ((Int32)Session["PedidosUsuario"] == 7)
            {
                ViewBag.Lista = totalEncerrado;
                ViewBag.TotalPedidos = totalEncerrado.Count;
            }

            ViewBag.TotalElaboracao = totalElaboracao.Count;
            ViewBag.TotalEnviado = totalEnviado.Count;
            ViewBag.TotalAprovado = totalAprovado.Count;
            ViewBag.TotalReprovado = totalReprovado.Count;
            ViewBag.TotalEncerrado = totalEncerrado.Count;
            ViewBag.TotalCancelado = totalCancelado.Count;

            ViewBag.Nome = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
            ViewBag.Foto = usuario.USUA_AQ_FOTO;
            ViewBag.Cargo = usuario.CARGO.CARG_NM_NOME;
            Session["PontoPedido"] = 1;
            return View();
        }

        public ActionResult VerPedidosUsuarioCRMVelho()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            List<CRM_PEDIDO_VENDA> lista = baseApp.GetAllPedidos(idAss).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).OrderByDescending(m => m.CRPV_DT_PEDIDO).ToList();
            ViewBag.Lista = lista;
            return View();
        }

        public ActionResult EnviarPropostaEdicao()
        {
            return RedirectToAction("EnviarProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        public ActionResult EnviarPedidoEdicao()
        {
            return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult CancelarPedidoEdicao()
        {
            return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        public ActionResult CancelarPropostaEdicao()
        {
            return RedirectToAction("CancelarProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        public ActionResult VerPropostaEdita()
        {
            Session["PontoProposta"] = 77;
            return RedirectToAction("VerProposta", new { id = (Int32)Session["IdCRMProposta"] });
        }

        public ActionResult VerPedidoEdita()
        {
            Session["PontoPedido"] = 77;
            return RedirectToAction("VerPedido", new { id = (Int32)Session["IdCRMPedido"] });
        }

        [ValidateInput(false)]
        public Int32 ProcessarEnvioPedidoEMail(MensagemViewModel vm, CRM_PEDIDO_VENDA item, USUARIO usuario)
        {
            // Inicialização
            Int32? tem = 0;
            Int32? em = 0;
            item = baseApp.GetPedidoById(item.CRPV_CD_ID);
            TEMPLATE_PROPOSTA temp = null;
            TEMPLATE_PROPOSTA email = null;
            String sigla = String.Empty;
            String siglaEM = String.Empty;
            Int32 idAss = (Int32)Session["IdAssinante"];
            CLIENTE cliente = null;
            String erro = null;
            Int32 volta = 0;
            ERP_CRMEntities Db = new ERP_CRMEntities();

            // Recupera templates
            if (item.TEPR_CD_ID != null)
            {
                tem = item.TEPR_CD_ID.Value;
                temp = baseApp.GetTemplateById(tem.Value);
                sigla = temp.TEPR_SG_SIGLA;
            }
            if (item.CRPC_IN_EMAIL != null)
            {
                em = item.CRPC_IN_EMAIL.Value;
                email = baseApp.GetTemplateById(em.Value);
                siglaEM = email.TEPR_SG_SIGLA;
            }

            // Recupera Cliente
            cliente = cliApp.GetItemById(vm.ID.Value);

            // Configuração
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara mensagem
            String body = String.Empty;
            String header = String.Empty;
            String footer = String.Empty;
            String link = String.Empty;
            String comercial = String.Empty;
            String emailBody = String.Empty;    

            //Prepara proposta
            if (sigla == "NADA" || sigla == String.Empty)
            {
                body = item.CRPV_TX_INFORMACOES_GERAIS;
                header = item.CRPV_TX_INTRODUCAO;
                footer = item.CRPV_TX_OUTROS_ITENS;
                comercial = item.CRPV_TX_CONDICOES_COMERCIAIS;
            }
            else
            {
                body = temp.TEPR_TX_TEXTO;
                header = temp.TEPR_TX_CABECALHO;
                footer = temp.TEPR_TX_RODAPE;
                comercial = item.CRPV_TX_CONDICOES_COMERCIAIS;
            }
            link = vm.MENS_NM_LINK;

            // Prepara cabeçalho
            if (sigla != "NADA" & sigla != String.Empty)
            {
                header = header.Replace("{Nome}", cliente.CLIE_NM_NOME);
            }

            // Prepara corpo
            if (sigla != "NADA" & sigla != String.Empty)
            {
                body = body.Replace("{Comerciais}", item.CRPV_TX_CONDICOES_COMERCIAIS);
            }

            // Prepara rodape
            if (sigla != "NADA" & sigla != String.Empty)
            {
                ASSINANTE assi = (ASSINANTE)Session["Assinante"];
                footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);
            }

            // Trata corpo
            StringBuilder str = new StringBuilder();
            str.AppendLine(body);

            // Trata links
            if (!String.IsNullOrEmpty(link))
            {
                if (!link.Contains("www."))
                {
                    link = "www." + link;
                }
                if (!link.Contains("http://"))
                {
                    link = "http://" + link;
                }
                str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
            }
            body = str.ToString();
            if (sigla != "NADA" & sigla != String.Empty)
            {
                emailBody = header + body + footer;
            }
            else
            {
                emailBody = header + "<br />" + body+ "<br />" + footer;
            }

            // Checa e monta anexos
            List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
            if (item.CRM_PEDIDO_VENDA_ANEXO.Count > 0)
            {
                foreach (CRM_PEDIDO_VENDA_ANEXO ane in item.CRM_PEDIDO_VENDA_ANEXO)
                {
                    String fn = Server.MapPath(ane.CRPA_AQ_ARQUIVO);
                    System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                    listaAnexo.Add(anexo);
                }
            }

            // Monta Mensagem final
            if (siglaEM != "NADA" & sigla != String.Empty)
            {
                emailBody = email.TEPR_TX_TEXTO + "<br />" + emailBody;
            }
            else
            {
                emailBody = item.CRPV_DS_ENVIO + "<br />" + emailBody;
            }

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
            Email mensagem = new Email();
            mensagem.ASSUNTO = "Proposta #" + item.CRPV_IN_NUMERO_GERADO + " - " + cliente.CLIE_NM_NOME;
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = cliente.CLIE_NM_EMAIL;
            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = cliente.ASSINANTE.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ATTACHMENT = listaAnexo;

            // Envia mensagem
            try
            {
                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                if (ex.InnerException != null)
                {
                    erro += ex.InnerException.Message;
                }
                if (ex.GetType() == typeof(SmtpFailedRecipientException))
                {
                    var se = (SmtpFailedRecipientException)ex;
                    erro += se.FailedRecipient;
                }
                return 1;
            }
            erro = null;
            return 0;
        }

        [HttpGet]
        public ActionResult IncluirComentarioPedidoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 id = (Int32)Session["IdCRMPedido"];
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            CRM_PEDIDO_VENDA_ACOMPANHAMENTO coment = new CRM_PEDIDO_VENDA_ACOMPANHAMENTO();
            CRMPedidoComentarioViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA_ACOMPANHAMENTO, CRMPedidoComentarioViewModel>(coment);
            vm.CRPC_DT_ACOMPANHAMENTO = DateTime.Now;
            vm.CRPC_IN_ATIVO = 1;
            vm.CRPV_CD_ID = item.CRPV_CD_ID;
            vm.USUARIO = usuarioLogado;
            vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirComentarioPedidoCRM(CRMPedidoComentarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCRMPedido"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_PEDIDO_VENDA_ACOMPANHAMENTO item = Mapper.Map<CRMPedidoComentarioViewModel, CRM_PEDIDO_VENDA_ACOMPANHAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CRM_PEDIDO_VENDA not = baseApp.GetPedidoById(idNot);

                    item.USUARIO = null;
                    not.CRM_PEDIDO_VENDA_ACOMPANHAMENTO.Add(item);
                    Int32 volta = baseApp.ValidateEditPedido(not);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VoltarEditarPedidoCRM");
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
        public ActionResult IncluirPedido()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode incluir pedido
            List<CRM_PEDIDO_VENDA> peds = baseApp.GetAllPedidos(idAss).Where(p => p.CRM1_CD_ID == (Int32)Session["IdCRM"]).ToList();
            if (peds.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
            {
                Session["MensCRM"] = 82;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            // Prepara view
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplateProposta(idAss).Where(p => p.TEPR_IN_TIPO != 2).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            // Recupera número
            Int32 num = 0;
            List<CRM_PEDIDO_VENDA> ped1 = baseApp.GetAllPedidosGeral(idAss).ToList();
            if (ped1.Count == 0)
            {
                num = conf.CONF_IN_NUMERO_INICIAL_PEDIDO.Value;
            }
            else
            {
                num = ped1.OrderByDescending(p => p.CRPV_IN_NUMERO_GERADO).ToList().First().CRPV_IN_NUMERO_GERADO.Value;
                num++;
            }

            CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
            CRM_PEDIDO_VENDA item = new CRM_PEDIDO_VENDA();
            CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
            vm.CRM1_CD_ID = (Int32)Session["IdCRM"];
            vm.CRM = crm;
            vm.CRPV_IN_ATIVO = 1;
            vm.ASSI_CD_ID = idAss;
            vm.CRPV_DT_PEDIDO = DateTime.Now;
            vm.CRPV_IN_STATUS = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.CRPV_VL_VALOR = 0;
            vm.CRPV_VL_TOTAL_ITENS = 0;
            vm.CRPV_VL_DESCONTO = 0;
            vm.CRPV_VL_FRETE = 0;
            vm.CRPV_VL_ICMS = 0;
            vm.CRPV_VL_IPI = 0;
            vm.CRPV_IN_PRAZO_ENTREGA = 0;
            vm.CRPV_VL_PESO_BRUTO = 0;
            vm.CRPV_VL_PESO_LIQUIDO = 0;
            vm.CRPV_IN_GERAR_CR = 0;
            vm.CRPV_DT_VALIDADE = DateTime.Now.AddDays(Convert.ToDouble(conf.CONF_NR_DIAS_PROPOSTA));
            vm.CRPV_IN_NUMERO_GERADO = num;
            vm.CLIE_CD_ID = crm.CLIE_CD_ID;
            vm.CLIENTE_NOME = (CLIENTE)Session["ClienteCRM"];
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplateProposta(idAss).Where(p => p.TEPR_IN_TIPO != 2).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            vm.CLIENTE_NOME = (CLIENTE)Session["ClienteCRM"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Criticas 
                    if (vm.CRPV_NM_NOME == null)
                    {
                        vm.CRPV_NM_NOME = "Pedido - " + vm.CLIENTE_NOME.CLIE_NM_NOME;
                    }

                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreatePedido(item);

                    // Cria pasta
                    String caminho = "/Imagens/" + idAss.ToString() + "/Pedido/" + item.CRPV_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Carrega arquivos
                    Session["IdCRMPedido"] = item.CRPV_CD_ID;
                    Session["FlagAnexo"] = 1;
                    if (Session["FileQueueCRM"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCRM"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueCRMPedido(file);
                        }

                        Session["FileQueueCRM"] = null;
                    }

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Criação de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Criação de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Verifica retorno
                    Session["SegueInclusao"] = 1;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult CancelarPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Motivos = new SelectList(baseApp.GetAllMotivoCancelamento(idAss).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            List<CRM_PEDIDO_VENDA> lp = baseApp.GetAllPedidos(id);
            Session["IdCRMPedido"] = item.CRPV_CD_ID;

            // Checa pedidos
            Session["TemPed"] = 0;
            if (lp.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
            {
                Session["TemPed"] = 1;
            }

            // Mensagens
            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 30)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0124", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 31)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0125", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 32)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 33)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 80)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0145", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 81)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0149", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            Session["MensCRM"] = 0;
            Session["VoltaComentPedido"] = 2;
            CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
            vm.CRPV_DT_CANCELAMENTO = DateTime.Today.Date;
            vm.CRPV_IN_STATUS = 3;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CancelarPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Motivos = new SelectList(baseApp.GetAllMotivoCancelamento(idAss).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");

            if (ModelState.IsValid)
            {

                try
                {
                    // Verifica tipo de ação
                    if (vm.MOCA_CD_ID == null || vm.MOCA_CD_ID == 0)
                    {
                        Session["MensCRM"] = 80;
                        return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (vm.CRPV_DS_CANCELAMENTO == null)
                    {
                        Session["MensCRM"] = 81;
                        return RedirectToAction("CancelarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }

                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 statusAnt = item.CRPV_IN_STATUS;
                    Int32 volta = baseApp.ValidateCancelarPedido(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 30;
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 31;
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 32;
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 33;
                        return View(vm);
                    }

                    // Atualiza status do processo
                    CRM crm = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    crm.CRM1_IN_STATUS = 2;
                    Int32 volta1 = baseApp.ValidateEdit(crm, crm);
                    Session["Cliente"] = crm.CLIENTE;

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Cancelamento de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Cancelamento de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Listas
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult ReprovarPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            List<CRM_PEDIDO_VENDA> lp = baseApp.GetAllPedidos(id);
            Session["IdCRMPedido"] = item.CRPV_CD_ID;

            // Checa pedidos
            Session["TemPed"] = 0;
            if (lp.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
            {
                Session["TemPed"] = 1;
            }

            // Mensagens
            Session["VoltaComentPedido"] = 3;
            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0128", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 51)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0129", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 52)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0126", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 53)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0127", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
            vm.CRPV_DT_REPROVACAO = DateTime.Today.Date;
            vm.CRPV_IN_STATUS = 4;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReprovarPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateReprovarPedido(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 50;
                        return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 51;
                        return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 52;
                        return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 53;
                        return RedirectToAction("ReprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }

                    // Atualiza status do processo
                    CRM crm = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    crm.CRM1_IN_STATUS = 2;
                    Int32 volta1 = baseApp.ValidateEdit(crm, crm);
                    Session["Cliente"] = crm.CLIENTE;

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Reprovação de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Reprovação de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Listas
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult AprovarPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            List<CRM_PEDIDO_VENDA> lp = baseApp.GetAllPedidos(id);
            Session["IdCRMPedido"] = item.CRPV_CD_ID;

            // Checa pedidos
            Session["TemPed"] = 0;
            if (lp.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
            {
                Session["TemPed"] = 1;
            }

            // Mensagens
            Session["VoltaComentPedido"] = 4;
            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 60)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0132", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 61)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0133", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 62)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0130", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 63)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0131", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
            vm.CRPV_DT_APROVACAO = DateTime.Today.Date;
            vm.CRPV_IN_STATUS = 5;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AprovarPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateAprovarPedido(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 62;
                        return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 63;
                        return RedirectToAction("AprovarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }

                    // Atualiza status do processo
                    CRM crm = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    crm.CRM1_IN_STATUS = 4;
                    Int32 volta1 = baseApp.ValidateEdit(crm, crm);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Aprovação de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Aprovação de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Listas
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult EnviarPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            List<CRM_PEDIDO_VENDA> lp = baseApp.GetAllPedidos(id);
            Session["IdCRMPedido"] = item.CRPV_CD_ID;

            // Checa propostas
            Session["TemPed"] = 0;
            if (lp.Where(p => p.CRPV_IN_STATUS == 1 || p.CRPV_IN_STATUS == 2).ToList().Count > 0)
            {
                Session["TemPed"] = 1;
            }
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplateProposta(idAss).Where(p => p.TEPR_IN_TIPO != 1).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");

            // Mensagens
            Session["VoltaComentPedido"] = 5;
            if (Session["MensCRM"] != null)
            {
                if ((Int32)Session["MensCRM"] == 70)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0134", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 71)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0135", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 72)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0136", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 75)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0137", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 76)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0138", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCRM"] == 77)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0139", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
            vm.CRPV_DT_ENVIO = DateTime.Today.Date;
            vm.CRPV_IN_STATUS = 2;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplateProposta(idAss).Where(p => p.TEPR_IN_TIPO != 1).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Monta anexo
                    Session["LinkAprova"] = null;

                    // Processa envio
                    CRM_PEDIDO_VENDA pro = baseApp.GetPedidoById(item.CRPV_CD_ID);
                    Int32 volta = baseApp.ValidateEnviarPedido(item);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 70;
                        return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 71;
                        return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 72;
                        return RedirectToAction("EnviarPedido", new { id = (Int32)Session["IdCRMPedido"] });
                    }
                    // Atualiza status do processo
                    CRM crm = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    FUNIL funil = funApp.GetItemById(crm.FUNI_CD_ID.Value);
                    List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.ToList();
                    Int32? etapaProposta = etapas.Where(p => p.FUET_IN_PROPOSTA == 1).FirstOrDefault().FUET_IN_ORDEM;
                    crm.CRM1_IN_STATUS = etapaProposta.Value;
                    Int32 volta1 = baseApp.ValidateEdit(crm, crm);

                    // Envia pedido
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.ASSI_CD_ID = idAss;
                    mens.MENS_DT_CRIACAO = DateTime.Now;
                    mens.MENS_IN_ATIVO = 1;
                    mens.MENS_IN_TIPO = 1;
                    mens.ID = crm.CLIE_CD_ID;
                    mens.TEPR_CD_ID = item.CRPC_IN_EMAIL;
                    mens.MENS_NM_LINK = item.CRPV_LK_LINK;
                    Int32 retGrava = ProcessarEnvioPedidoEMail(mens, item, usuario);

                    // Verifica
                    if (retGrava == 1)
                    {
                        Session["MensCRM"] = 75;
                        return View(vm);
                    }

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Envio de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Envio de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Retorno
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult EditarPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica se pode editar ação
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            if (item.CRPV_IN_STATUS > 2)
            {
                Session["MensCRM"] = 53;
                return RedirectToAction("VoltarAcompanhamentoCRM");
            }

            // Prepara view
            Session["VoltaComentPedido"] = 1;
            Session["IdCRMPedido"] = item.CRPV_CD_ID;
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplateProposta(idAss).Where(p => p.TEPR_IN_TIPO == 1).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            // Mensagens
            if ((Int32)Session["MensCRM"] == 99)
            {
                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0150", CultureInfo.CurrentCulture));
            }

            // Processa
            ViewBag.Template = item.TEMPLATE_PROPOSTA.TEPR_SG_SIGLA;
            Session["MensCRM"] = 0;
            objetoAntes = (CRM)Session["CRM"];
            CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
            CRM proc = baseApp.GetItemById(item.CRM1_CD_ID.Value);
            CLIENTE cli = cliApp.GetItemById(proc.CLIE_CD_ID);
            vm.CLIENTE_NOME = cli;
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarPedido(CRMPedidoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Templates = new SelectList(baseApp.GetAllTemplateProposta(idAss).Where(p => p.TEPR_IN_TIPO == 1).OrderByDescending(p => p.TEPR_IN_FIXO), "TEPR_CD_ID", "TEPR_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CRM_PEDIDO_VENDA item = Mapper.Map<CRMPedidoViewModel, CRM_PEDIDO_VENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                    // Acerta valores
                    Decimal total = item.CRPV_VL_VALOR.Value;
                    //total += item.CRPV_VL_TOTAL_SERVICOS.Value;
                    if (item.CRPV_VL_DESCONTO != null & item.CRPV_VL_DESCONTO > 0)
                    {
                        total -= item.CRPV_VL_DESCONTO.Value;
                    }
                    if (item.CRPV_VL_FRETE != null & item.CRPV_VL_FRETE > 0)
                    {
                        total += item.CRPV_VL_FRETE.Value;
                    }
                    item.CRPV_TOTAL_PEDIDO = total;
                    Int32 volta = baseApp.ValidateEditPedido(item);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
                    CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = not.CRM1_CD_ID;
                    dia.CRPV_CD_ID = item.CRPV_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Alteração de Proposta";
                    dia.DIPR_DS_DESCRICAO = "Alteração de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Verifica retorno
                    Session["ListaCRM"] = null;
                    return RedirectToAction("VoltarPedidoCRM");
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
        public ActionResult ExcluirPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Processa
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRPV_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditPedido(item);

            // Gera diario
            CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
            CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
            DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
            dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
            dia.USUA_CD_ID = usuario.USUA_CD_ID;
            dia.DIPR_DT_DATA = DateTime.Today.Date;
            dia.CRM1_CD_ID = not.CRM1_CD_ID;
            dia.CRPV_CD_ID = item.CRPV_CD_ID;
            dia.DIPR_NM_OPERACAO = "Exclusão de Proposta";
            dia.DIPR_DS_DESCRICAO = "Exclusão de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
            Int32 volta3 = diaApp.ValidateCreate(dia);

            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        [HttpGet]
        public ActionResult ReativarPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Processa
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            objetoAntes = (CRM)Session["CRM"];
            item.CRPV_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditPedido(item);

            // Gera diario
            CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
            CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
            DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
            dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
            dia.USUA_CD_ID = usuario.USUA_CD_ID;
            dia.DIPR_DT_DATA = DateTime.Today.Date;
            dia.CRM1_CD_ID = not.CRM1_CD_ID;
            dia.CRPV_CD_ID = item.CRPV_CD_ID;
            dia.DIPR_NM_OPERACAO = "Reativação de Proposta";
            dia.DIPR_DS_DESCRICAO = "Reativação de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
            Int32 volta3 = diaApp.ValidateCreate(dia);

            return RedirectToAction("VoltarAcompanhamentoCRM");
        }

        public ActionResult ElaborarPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            Session["IdCRMPedido"] = item.CRPV_CD_ID;

            // Processa
            item.CRPV_IN_STATUS = 1;
            item.CRPV_DT_ENVIO = null;
            item.CRPV_DT_APROVACAO = null;
            item.CRPV_DT_CANCELAMENTO = null;
            item.CRPV_DT_REPROVACAO = null;
            item.CRPV_DS_APROVACAO = null;
            item.CRPV_DS_CANCELAMENTO = null;
            item.CRPV_DS_REPROVACAO = null;
            Int32 volta = baseApp.ValidateEditPedido(item);

            // Gera diario
            CRM not = baseApp.GetItemById(item.CRM1_CD_ID.Value);
            CLIENTE cli = cliApp.GetItemById(not.CLIE_CD_ID);
            DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
            dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
            dia.USUA_CD_ID = usuario.USUA_CD_ID;
            dia.DIPR_DT_DATA = DateTime.Today.Date;
            dia.CRM1_CD_ID = not.CRM1_CD_ID;
            dia.CRPV_CD_ID = item.CRPV_CD_ID;
            dia.DIPR_NM_OPERACAO = "Alteração de Status de Proposta";
            dia.DIPR_DS_DESCRICAO = "Alteração de Status de Proposta " + item.CRPV_NM_NOME + ". Processo: " + not.CRM1_NM_NOME + ". Cliente: " + cli.CLIE_NM_NOME;
            Int32 volta3 = diaApp.ValidateCreate(dia);

            return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
        }

        public ActionResult VerPedido(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            Session["IncluirCRM"] = 0;
            Session["CRM"] = null;
            Session["PontoPedido"] = 0;

            // Recupera
            Session["CRMNovo"] = 0;
            CRM_PEDIDO_VENDA item = baseApp.GetPedidoById(id);
            Session["IdCRMPedido"] = item.CRPV_CD_ID;
            CRMPedidoViewModel vm = Mapper.Map<CRM_PEDIDO_VENDA, CRMPedidoViewModel>(item);
            CRM proc = baseApp.GetItemById(item.CRM1_CD_ID.Value);
            ViewBag.Template = item.TEMPLATE_PROPOSTA.TEPR_SG_SIGLA;
            CLIENTE cli = cliApp.GetItemById(proc.CLIE_CD_ID);
            vm.CLIENTE_NOME = cli;
            return View(vm);
        }

        public ActionResult IncluirMotivoCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMotCancelamento"] = 2;
            return RedirectToAction("IncluirMotCancelamento", "TabelaAuxiliar");
        }

        public ActionResult IncluirMotivoCancelamento1()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMotCancelamento"] = 3;
            return RedirectToAction("IncluirMotCancelamento", "TabelaAuxiliar");
        }

        public ActionResult IncluirMotivoEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMotEncerramento"] = 2;
            return RedirectToAction("IncluirMotEncerramento", "TabelaAuxiliar");
        }

        [HttpGet]
        public ActionResult MontarTelaHistorico()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
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
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Carrega listas
            Int32 flag = (Int32)Session["TipoHistorico"];
            if ((Int32)Session["TipoHistorico"] == 1)
            {
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                Session["IdCRM"] = crm.CRM1_CD_ID;
                if ((List<DIARIO_PROCESSO>)Session["ListaDiario"] == null)
                {
                    listaMasterDiario = crm.DIARIO_PROCESSO.ToList();
                    Session["ListaDiario"] = listaMasterDiario;
                }
                Session["VoltaHistorico"] = 1;
            }
            else
            {
                if ((List<DIARIO_PROCESSO>)Session["ListaDiario"] == null)
                {
                    listaMasterDiario = diaApp.GetAllItens(idAss);
                    Session["ListaDiario"] = listaMasterDiario;
                }
                Session["VoltaHistorico"] = 2;
            }

            // Prepara lista
            Session["CRM"] = null;
            List<DIARIO_PROCESSO> list = (List<DIARIO_PROCESSO>)Session["ListaDiario"];
            list = list.Where(p => p.DIPR_DT_DATA == DateTime.Today.Date).ToList();
            list = list.OrderByDescending(p => p.DIPR_DT_DATA).ToList();
            ViewBag.Listas = list;
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.CRM = new SelectList(baseApp.GetAllItens(idAss).OrderBy(p => p.CRM1_NM_NOME), "CRM1_CD_ID", "CRM1_NM_NOME");

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["VoltaCRM"] = 22;
            Session["VoltaPedido"] = 22;
            Session["PontoAcao"] = 22;
            Session["VoltaAgenda"] = 44;

            // Abre view
            objetoDiario = new DIARIO_PROCESSO();
            if (Session["FiltroDiario"] != null)
            {
                objetoDiario = (DIARIO_PROCESSO)Session["FiltroDiario"];
            }
            objetoDiario.DIPR_DT_DATA = DateTime.Today.Date;
            objetoDiario.DIPR_DT_DUMMY = DateTime.Today.Date;
            return View(objetoDiario);
        }

        public ActionResult RetirarFiltroDiario()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaDiario"] = null;
            Session["FiltroDiario"] = null;
            return RedirectToAction("MontarTelaHistorico");
        }

        [HttpPost]
        public ActionResult FiltrarDiario(DIARIO_PROCESSO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Prepara processo
                if ((Int32)Session["TipoHistorico"] == 1)
                {
                    item.CRM1_CD_ID = (Int32)Session["IdCRM"];
                }

                // Executa a operação
                List<DIARIO_PROCESSO> listaObj = new List<DIARIO_PROCESSO>();
                Session["FiltroDiario"] = item;
                Int32 volta = diaApp.ExecuteFilter(item.CRM1_CD_ID, item.DIPR_DT_DATA, item.DIPR_DT_DUMMY, item.USUA_CD_ID, item.DIPR_NM_OPERACAO, item.DIPR_DS_DESCRICAO, idAss, out listaObj);

                // Sucesso
                listaMasterDiario = listaObj;
                Session["ListaDiario"] = listaObj;
                return RedirectToAction("MontarTelaHistorico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaHistorico");
            }
        }

        public ActionResult MontarTelaHistoricoGeral()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaDiario"] = null;
            Session["TipoHistorico"] = 2;
            return RedirectToAction("MontarTelaHistorico");
        }

        [HttpPost]
        public JsonResult GetAcoesAtraso()
        {
            var usu = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            List<CRM_ACAO> lista = baseApp.GetAllAcoes(idAss).Where(p => p.USUA_CD_ID1 == usu.USUA_CD_ID & p.CRAC_IN_ATIVO == 1 & p.CRAC_IN_STATUS == 1 & p.CRM.CRM1_IN_ATIVO == 1 & p.CRAC_DT_PREVISTA.Value.Date < DateTime.Today.Date).ToList();

            if (lista.Count == 1)
            {
                var hash = new Hashtable();
                hash.Add("msg", "Você tem 1 ação em atraso");

                return Json(hash);
            }
            else if (lista.Count > 1)
            {
                var hash = new Hashtable();
                hash.Add("msg", "Você tem " + lista.Count + " ações em atraso");
                return Json(hash);
            }
            else
            {
                return null; // Sem atrasos
            }
        }

        [HttpPost]
        public ActionResult AcoesAtrasoClick()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            var usu = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            List<CRM_ACAO> lista = baseApp.GetAllAcoes(idAss).Where(p => p.USUA_CD_ID1 == usu.USUA_CD_ID & p.CRAC_IN_ATIVO == 1 & p.CRAC_IN_STATUS == 1 & p.CRM.CRM1_IN_ATIVO == 1 & p.CRAC_DT_PREVISTA.Value.Date < DateTime.Today.Date).ToList();

            if (lista.Count == 1)
            {
                return Json(lista.FirstOrDefault().CRAC_CD_ID);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult GetModeloProposta(Int32 id)
        {
            TEMPLATE_PROPOSTA forn = tpApp.GetItemById(id);
            var hash = new Hashtable();
            hash.Add("intro", forn.TEPR_TX_CABECALHO);
            hash.Add("corpo", forn.TEPR_TX_TEXTO);
            hash.Add("rodape", forn.TEPR_TX_RODAPE);
            return Json(hash);
        }
    }
}