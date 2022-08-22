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
    public class TabelaAuxiliarController : Controller
    {
        private readonly ICargoAppService carApp;
        private readonly ILogAppService logApp;
        private readonly IGrupoCCAppService gruApp;
        private readonly ISubgrupoAppService subApp;
        CARGO_USUARIO objetoCargo = new CARGO_USUARIO();
        CARGO_USUARIO objetoAntesCargo = new CARGO_USUARIO();
        List<CARGO_USUARIO> listaMasterCargo = new List<CARGO_USUARIO>();
        GRUPO_PLANO_CONTA objetoGrupo = new GRUPO_PLANO_CONTA();
        GRUPO_PLANO_CONTA objetoAntesGrupo = new GRUPO_PLANO_CONTA();
        List<GRUPO_PLANO_CONTA> listaMasterGrupo = new List<GRUPO_PLANO_CONTA>();
        SUBGRUPO_PLANO_CONTA objetoSubgrupo = new SUBGRUPO_PLANO_CONTA();
        SUBGRUPO_PLANO_CONTA objetoAntesSubgrupo = new SUBGRUPO_PLANO_CONTA();
        List<SUBGRUPO_PLANO_CONTA> listaMasterSubgrupo = new List<SUBGRUPO_PLANO_CONTA>();
        String extensao;

        public TabelaAuxiliarController(ICargoAppService carApps, ILogAppService logApps, IGrupoCCAppService gruApps, ISubgrupoAppService subApps)
        {
            carApp = carApps;
            logApp = logApps;
            gruApp = gruApps;
            subApp = subApps;
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
            return RedirectToAction("MontarTelaTabelasAuxiliares", "BaseAdmin");
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
        public ActionResult MontarTelaCargo()
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
            if (Session["ListaCargo"] == null)
            {
                listaMasterCargo = carApp.GetAllItens(idAss);
                Session["ListaCargo"] = listaMasterCargo;
            }
            ViewBag.Listas = (List<CARGO_USUARIO>)Session["ListaCargo"];
            ViewBag.Title = "Cargo";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CARGO_USUARIO>)Session["ListaCargo"]).Count;

            if (Session["MensCargo"] != null)
            {
                if ((Int32)Session["MensCargo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0154", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCargo"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCargo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0155", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensCargo"] = 0;
            objetoCargo = new CARGO_USUARIO();
            return View(objetoCargo);
        }

        public ActionResult RetirarFiltroCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCargo"] = null;
            Session["FiltroCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }

        public ActionResult MostrarTudoCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCargo = carApp.GetAllItensAdm(idAss);
            Session["FiltroCargo"] = null;
            Session["ListaCargo"] = listaMasterCargo;
            return RedirectToAction("MontarTelaCargo");
        }

        public ActionResult VoltarBaseCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult IncluirCargo()
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
                    return RedirectToAction("MontarTelaCargo");
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
            CARGO_USUARIO item = new CARGO_USUARIO();
            CargoViewModel vm = Mapper.Map<CARGO_USUARIO, CargoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CARG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCargo(CargoViewModel vm)
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
                    CARGO_USUARIO item = Mapper.Map<CargoViewModel, CARGO_USUARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = carApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCargo"] = 3;
                        return RedirectToAction("MontarTelaCargo");
                    }
                    Session["IdVolta"] = item.CARG_CD_ID;

                    // Sucesso
                    listaMasterCargo = new List<CARGO_USUARIO>();
                    Session["ListaCargo"] = null;
                    return RedirectToAction("MontarTelaCargo");
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
        public ActionResult VerCargo(Int32 id)
        {
            
            // Prepara view
            CARGO_USUARIO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            Session["Cargo"] = item;
            Session["IdCargo"] = id;
            CargoViewModel vm = Mapper.Map<CARGO_USUARIO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarCargo(Int32 id)
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
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CARGO_USUARIO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            Session["Cargo"] = item;
            Session["IdCargo"] = id;
            CargoViewModel vm = Mapper.Map<CARGO_USUARIO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCargo(CargoViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CARGO_USUARIO item = Mapper.Map<CargoViewModel, CARGO_USUARIO>(vm);
                    Int32 volta = carApp.ValidateEdit(item, objetoAntesCargo, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCargo = new List<CARGO_USUARIO>();
                    Session["ListaCargo"] = null;
                    return RedirectToAction("MontarTelaCargo");
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
        public ActionResult ExcluirCargo(Int32 id)
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
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CARGO_USUARIO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            item.CARG_IN_ATIVO = 0;
            Int32 volta = carApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCargo"] = 4;
                return RedirectToAction("MontarTelaCargo");
            }
            listaMasterCargo = new List<CARGO_USUARIO>();
            Session["ListaCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult ReativarCargo(Int32 id)
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
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CARGO_USUARIO item = carApp.GetItemById(id);
            item.CARG_IN_ATIVO = 1;
            objetoAntesCargo = item;
            Int32 volta = carApp.ValidateReativar(item, usuario);
            listaMasterCargo = new List<CARGO_USUARIO>();
            Session["ListaCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }
    
        [HttpGet]
        public ActionResult MontarTelaGrupo()
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
            if (Session["ListaGrupo"] == null)
            {
                listaMasterGrupo = gruApp.GetAllItens(idAss);
                Session["ListaGrupo"] = listaMasterGrupo;
            }
            ViewBag.Listas = (List<GRUPO_PLANO_CONTA>)Session["ListaGrupo"];
            ViewBag.Title = "Grupo";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<GRUPO_PLANO_CONTA>)Session["ListaGrupo"]).Count;

            if (Session["MensGrupo"] != null)
            {
                if ((Int32)Session["MensGrupo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0159", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0160", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensGrupo"] = 0;
            objetoGrupo = new GRUPO_PLANO_CONTA();
            return View(objetoGrupo);
        }

        public ActionResult RetirarFiltroGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaGrupo"] = null;
            Session["FiltroGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult MostrarTudoGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterGrupo = gruApp.GetAllItensAdm(idAss);
            Session["FiltroGrupo"] = null;
            Session["ListaGrupo"] = listaMasterGrupo;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult VoltarBaseGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult IncluirGrupo()
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
                    return RedirectToAction("MontarTelaGrupo");
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
            GRUPO_PLANO_CONTA item = new GRUPO_PLANO_CONTA();
            GrupoViewModel vm = Mapper.Map<GRUPO_PLANO_CONTA, GrupoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.GRCC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirGrupo(GrupoViewModel vm)
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
                    GRUPO_PLANO_CONTA item = Mapper.Map<GrupoViewModel, GRUPO_PLANO_CONTA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = gruApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensGrupo"] = 3;
                        return RedirectToAction("MontarTelaGrupo");
                    }
                    Session["IdGrupo"] = item.GRCC_CD_ID;

                    // Sucesso
                    listaMasterGrupo = new List<GRUPO_PLANO_CONTA>();
                    Session["ListaGrupo"] = null;
                    return RedirectToAction("MontarTelaGrupo");
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
        public ActionResult VerGrupo(Int32 id)
        {
            
            // Prepara view
            GRUPO_PLANO_CONTA item = gruApp.GetItemById(id);
            objetoAntesGrupo = item;
            Session["Grupo"] = item;
            Session["IdGrupo"] = id;
            GrupoViewModel vm = Mapper.Map<GRUPO_PLANO_CONTA, GrupoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarGrupo(Int32 id)
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
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            GRUPO_PLANO_CONTA item = gruApp.GetItemById(id);
            objetoAntesGrupo = item;
            Session["Grupo"] = item;
            Session["IdGrupo"] = id;
            GrupoViewModel vm = Mapper.Map<GRUPO_PLANO_CONTA, GrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarGrupo(GrupoViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    GRUPO_PLANO_CONTA item = Mapper.Map<GrupoViewModel, GRUPO_PLANO_CONTA>(vm);
                    Int32 volta = gruApp.ValidateEdit(item, objetoAntesGrupo, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterGrupo= new List<GRUPO_PLANO_CONTA>();
                    Session["ListaGrupo"] = null;
                    return RedirectToAction("MontarTelaGrupo");
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
        public ActionResult ExcluirGrupo(Int32 id)
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
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            GRUPO_PLANO_CONTA item = gruApp.GetItemById(id);
            objetoAntesGrupo = item;
            item.GRCC_IN_ATIVO = 0;
            Int32 volta = gruApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensGrupo"] = 4;
                return RedirectToAction("MontarTelaGrupo");
            }
            listaMasterGrupo = new List<GRUPO_PLANO_CONTA>();
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult ReativarGrupo(Int32 id)
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
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            GRUPO_PLANO_CONTA item = gruApp.GetItemById(id);
            item.GRCC_IN_ATIVO = 1;
            objetoAntesGrupo = item;
            Int32 volta = gruApp.ValidateReativar(item, usuario);
            listaMasterGrupo = new List<GRUPO_PLANO_CONTA>();
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult MontarTelaSubgrupo()
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
            if (Session["ListaSubgrupo"] == null)
            {
                listaMasterSubgrupo = subApp.GetAllItens(idAss);
                Session["ListaSubgrupo"] = listaMasterSubgrupo;
            }
            ViewBag.Listas = (List<SUBGRUPO_PLANO_CONTA>)Session["ListaSubgrupo"];
            ViewBag.Grupos = new SelectList(subApp.GetAllGrupos(idAss), "GRCC_CD_ID", "GRCC_NM_EXIBE");
            ViewBag.Title = "Subgrupo";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Subgrupo = ((List<SUBGRUPO_PLANO_CONTA>)Session["ListaSubgrupo"]).Count;

            if (Session["MensSubgrupo"] != null)
            {
                if ((Int32)Session["MensSubgrupo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0161", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubgrupo"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubgrupo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0162", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensSubgrupo"] = 0;
            objetoSubgrupo = new SUBGRUPO_PLANO_CONTA();
            return View(objetoSubgrupo);
        }

        public ActionResult RetirarFiltroSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaSubgrupo"] = null;
            Session["FiltroSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public ActionResult MostrarTudoSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSubgrupo = subApp.GetAllItensAdm(idAss);
            Session["FiltroSubgrupo"] = null;
            Session["ListaSubgrupo"] = listaMasterSubgrupo;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public ActionResult VoltarBaseSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult IncluirSubgrupo()
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
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Grupos = new SelectList(subApp.GetAllGrupos(idAss), "GRCC_CD_ID", "GRCC_NM_EXIBE");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            SUBGRUPO_PLANO_CONTA item = new SUBGRUPO_PLANO_CONTA();
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO_PLANO_CONTA, SubgrupoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SGCC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirSubgrupo(SubgrupoViewModel vm)
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
                    SUBGRUPO_PLANO_CONTA item = Mapper.Map<SubgrupoViewModel, SUBGRUPO_PLANO_CONTA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = subApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensSubgrupo"] = 3;
                        return RedirectToAction("MontarTelaSubgrupo");
                    }
                    Session["IdSubgrupo"] = item.SGCC_CD_ID;

                    // Sucesso
                    listaMasterSubgrupo = new List<SUBGRUPO_PLANO_CONTA>();
                    Session["ListaSubgrupo"] = null;
                    return RedirectToAction("MontarTelaSubgrupo");
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
        public ActionResult VerSubgrupo(Int32 id)
        {
            
            // Prepara view
            SUBGRUPO_PLANO_CONTA item = subApp.GetItemById(id);
            objetoAntesSubgrupo = item;
            Session["Subgrupo"] = item;
            Session["IdSubgrupo"] = id;
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO_PLANO_CONTA, SubgrupoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarSubgrupo(Int32 id)
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
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            SUBGRUPO_PLANO_CONTA item = subApp.GetItemById(id);
            objetoAntesSubgrupo = item;
            Session["Subgrupo"] = item;
            Session["IdSubgrupo"] = id;
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO_PLANO_CONTA, SubgrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarSubgrupo(SubgrupoViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SUBGRUPO_PLANO_CONTA item = Mapper.Map<SubgrupoViewModel, SUBGRUPO_PLANO_CONTA>(vm);
                    Int32 volta = subApp.ValidateEdit(item, objetoAntesSubgrupo, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSubgrupo = new List<SUBGRUPO_PLANO_CONTA>();
                    Session["ListaSubgrupo"] = null;
                    return RedirectToAction("MontarTelaSubgrupo");
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
        public ActionResult ExcluirSubgrupo(Int32 id)
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
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            SUBGRUPO_PLANO_CONTA item = subApp.GetItemById(id);
            objetoAntesSubgrupo = item;
            item.SGCC_IN_ATIVO = 0;
            Int32 volta = subApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensSubgrupo"] = 4;
                return RedirectToAction("MontarTelaSubgrupo");
            }
            listaMasterSubgrupo = new List<SUBGRUPO_PLANO_CONTA>();
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult ReativarSubgrupo(Int32 id)
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
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            SUBGRUPO_PLANO_CONTA item = subApp.GetItemById(id);
            item.SGCC_IN_ATIVO = 1;
            objetoAntesSubgrupo = item;
            Int32 volta = subApp.ValidateReativar(item, usuario);
            listaMasterSubgrupo = new List<SUBGRUPO_PLANO_CONTA>();
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

    
    
    
    }
}