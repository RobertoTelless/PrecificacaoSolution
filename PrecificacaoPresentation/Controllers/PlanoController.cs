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
using EntitiesServices.DTO;

namespace ERP_Condominios_Solution.Controllers
{
    public class PlanoController : Controller
    {
        private readonly IAssinanteAppService baseApp;
        private readonly IPlanoAppService planApp;
        private readonly ILogAppService logApp;
        private readonly INotificacaoAppService notiApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        ASSINANTE objeto = new ASSINANTE();
        ASSINANTE objetoAntes = new ASSINANTE();
        List<ASSINANTE> listaMaster = new List<ASSINANTE>();
        PLANO objetoPlano = new PLANO();
        PLANO objetoAntesPlano = new PLANO();
        List<PLANO> listaMasterPlano = new List<PLANO>();
        String extensao;

        public PlanoController(IAssinanteAppService baseApps, ILogAppService logApps, INotificacaoAppService notiApps, IConfiguracaoAppService confApps, IPlanoAppService planApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notiApps;
            confApp = confApps;
            planApp = planApps;
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAssinantes", "BaseAdmin");
        }

       [HttpGet]
        public ActionResult MontarTelaPlano()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if ((List<PLANO>)Session["ListaPlano"] == null || ((List<PLANO>)Session["ListaPlano"]).Count == 0)
            {
                listaMasterPlano = planApp.GetAllItens();
                Session["ListaPlano"] = listaMasterPlano;
            }
            ViewBag.Listas = (List<PLANO>)Session["ListaPlano"];
            ViewBag.Title = "Planos";

            // Indicadores
            ViewBag.Planos = ((List<PLANO>)Session["ListaPlano"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensPlano"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPlano"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0201", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0202", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0080", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensPlano"] = 0;
            Session["VoltaPlano"] = 1;
            Session["VoltaCompPlano"] = 1;
            objetoPlano = new PLANO();
            if (Session["FiltroPlano"] != null)
            {
                objetoPlano = (PLANO)Session["FiltroPlano"];
            }
            objetoPlano.PLAN_IN_ATIVO = 1;
            return View(objetoPlano);
        }

        [HttpGet]
        public ActionResult VerComparativoPlanos()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if ((List<PLANO>)Session["ListaPlanoAll"] == null || ((List<PLANO>)Session["ListaPlanoAll"]).Count == 0)
            {
                listaMasterPlano = planApp.GetAllItens();
                Session["ListaPlanoAll"] = listaMasterPlano;
            }
            ViewBag.Listas = (List<PLANO>)Session["ListaPlanoAll"];
            ViewBag.Title = "Planos";

            // Indicadores
            ViewBag.Planos = ((List<PLANO>)Session["ListaPlanoAll"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensPlano"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPlano"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0201", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0202", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 50)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0080", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensPlano"] = 0;
            Session["VoltaPlano"] = 1;
            objetoPlano = new PLANO();
            objetoPlano.PLAN_IN_ATIVO = 1;
            return View(objetoPlano);
        }

        public ActionResult RetirarFiltroPlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaPlano"] = null;
            Session["FiltroPlano"] = null;
            return RedirectToAction("MontarTelaPlano");
        }

        public ActionResult MostrarTudoPlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            listaMasterPlano = planApp.GetAllItensAdm();
            Session["FiltroPlano"] = null;
            Session["ListaPlano"] = listaMasterPlano;
            return RedirectToAction("MontarTelaPlano");
        }

        [HttpPost]
        public ActionResult FiltrarPlano(PLANO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                List<PLANO> listaObj = new List<PLANO>();
                Session["FiltroPlano"] = item;
                Int32 volta = planApp.ExecuteFilter(item.PLAN_NM_NOME, item.PLAN_DS_DESCRICAO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("MontarTelaPlano");
                }

                // Sucesso
                listaMasterPlano = listaObj;
                Session["ListaPlano"]  = listaObj;
                return RedirectToAction("MontarTelaPlano");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaPlano");
            }
        }

        public ActionResult VoltarBasePlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPlano");
        }

        public ActionResult VoltarComparativoPlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCompPlano"] == 1)
            {
                return RedirectToAction("MontarTelaPlano", "Plano");
            }
            if ((Int32)Session["VoltaCompPlano"] == 2)
            {
                return RedirectToAction("MontarTelaCentralAssinante", "BaseAdmin");
            }
            if ((Int32)Session["VoltaCompPlano"] == 3)
            {
                return RedirectToAction("SolicitarIncluirAssinantePlano", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaPlano");
        }

        [HttpGet]
        public ActionResult IncluirPlano()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara listas
            ViewBag.Periodicidade = new SelectList(planApp.GetAllPeriodicidades(), "PLPE_CD_ID", "PLPE_NM_NOME");

            //Mensagens
            if (Session["MensPlano"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPlano"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0201", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 8)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0215", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            PLANO item = new PLANO();
            PlanoViewModel vm = Mapper.Map<PLANO, PlanoViewModel>(item);
            vm.PLAN_DT_CRIACAO = DateTime.Today.Date;
            vm.PLAN_DT_VALIDADE = DateTime.Today.Date.AddMonths(12);
            vm.PLAN_IN_ATIVO = 1;
            vm.PLAN_IN_ATENDIMENTOS = 0;
            vm.PLAN_IN_COMPRA = 0;
            vm.PLAN_IN_CRM = 0;
            vm.PLAN_IN_ESTOQUE = 0;
            vm.PLAN_IN_FATURA = 0;
            vm.PLAN_IN_FINANCEIRO = 0;
            vm.PLAN_IN_MENSAGENS = 0;
            vm.PLAN_IN_OS = 0;
            vm.PLAN_IN_PATRIMONIO = 0;
            vm.PLAN_IN_SERVICOS = 0;
            vm.PLAN_IN_VENDAS = 0;
            vm.PLAN_NR_ACOES = 0;
            vm.PLAN_NR_COMPRA = 0;
            vm.PLAN_NR_CONTATOS = 0;
            vm.PLAN_NR_EMAIL = 0;
            vm.PLAN_NR_FORNECEDOR = 0;
            vm.PLAN_NR_PATRIMONIO = 0;
            vm.PLAN_NR_PROCESSOS = 0;
            vm.PLAN_NR_PRODUTO = 0;
            vm.PLAN_NR_SMS = 0;
            vm.PLAN_NR_USUARIOS = 0;
            vm.PLAN_NR_VENDA = 0;
            vm.PLAN_NR_WHATSAPP = 0;
            vm.PLAN_VL_PRECO = 0;
            vm.PLAN_VL_PROMOCAO = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirPlano(PlanoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ViewBag.Periodicidade = new SelectList(planApp.GetAllPeriodicidades(), "PLPE_CD_ID", "PLPE_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PLANO item = Mapper.Map<PlanoViewModel, PLANO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = planApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPlano"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0201", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensPlano"] = 8;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0215", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterPlano = new List<PLANO>();
                    Session["ListaPlano"] = null;

                    // Volta
                    return RedirectToAction("MontarTelaPlano");
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
        public ActionResult EditarPlano(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            ViewBag.Periodicidade = new SelectList(planApp.GetAllPeriodicidades(), "PLPE_CD_ID", "PLPE_NM_NOME");
            PLANO item = planApp.GetItemById(id);

            // Mensagens
            if (Session["MensPlano"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensPlano"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPlano"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
            }

            Session["VoltaPlano"] = 1;
            objetoAntesPlano = item;
            Session["Plano"] = item;
            Session["IdPlano"] = id;
            Session["IdVolta"] = id;
            PlanoViewModel vm = Mapper.Map<PLANO, PlanoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarPlano(PlanoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ViewBag.Periodicidade = new SelectList(planApp.GetAllPeriodicidades(), "PLPE_CD_ID", "PLPE_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PLANO item = Mapper.Map<PlanoViewModel, PLANO>(vm);
                    Int32 volta = planApp.ValidateEdit(item, objetoAntesPlano, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterPlano = new List<PLANO>();
                    Session["ListaPlano"] = null;
                    if (Session["FiltroPlano"] != null)
                    {
                        FiltrarPlano((PLANO)Session["FiltroPlano"]);
                    }
                    return RedirectToAction("MontarTelaPlano");
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
        public ActionResult ExcluirPlano(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Processa
            PLANO item = planApp.GetItemById(id);
            objetoAntesPlano = (PLANO)Session["Plano"];
            item.PLAN_IN_ATIVO = 0;
            Int32 volta = planApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensPlano"] = 4;
                return RedirectToAction("MontarTelaPlano", "Plano");
            }
            listaMasterPlano = new List<PLANO>();
            Session["ListaPlano"] = null;
            Session["FiltroPlano"] = null;
            return RedirectToAction("MontarTelaPlano");
        }

        [HttpGet]
        public ActionResult ReativarPlano(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Processa
            PLANO item = planApp.GetItemById(id);
            objetoAntesPlano = (PLANO)Session["Plano"];
            item.PLAN_IN_ATIVO = 1;
            Int32 volta = planApp.ValidateReativar(item, usuario);
            listaMasterPlano = new List<PLANO>();
            Session["ListaPlano"] = null;
            Session["FiltroPlano"] = null;
            return RedirectToAction("MontarTelaPlano");
        }

        public ActionResult VoltarAnexoPlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarPlano", new { id = (Int32)Session["IdPlano"] });
        }


    }
}