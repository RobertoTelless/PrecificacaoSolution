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
        private readonly ICategoriaClienteAppService ccApp;
        private readonly ICategoriaFornecedorAppService cfApp;
        private readonly ITipoEmbalagemAppService teApp;
        private readonly ITipoAcaoAppService taApp;
        private readonly IMotivoCancelamentoAppService mcApp;
        private readonly IMotivoEncerramentoAppService meApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly ISubcategoriaProdutoAppService spApp;
        private readonly ICategoriaCustoFixoAppService cxApp;

        CARGO_USUARIO objetoCargo = new CARGO_USUARIO();
        CARGO_USUARIO objetoAntesCargo = new CARGO_USUARIO();
        List<CARGO_USUARIO> listaMasterCargo = new List<CARGO_USUARIO>();
        GRUPO_PLANO_CONTA objetoGrupo = new GRUPO_PLANO_CONTA();
        GRUPO_PLANO_CONTA objetoAntesGrupo = new GRUPO_PLANO_CONTA();
        List<GRUPO_PLANO_CONTA> listaMasterGrupo = new List<GRUPO_PLANO_CONTA>();
        SUBGRUPO_PLANO_CONTA objetoSubgrupo = new SUBGRUPO_PLANO_CONTA();
        SUBGRUPO_PLANO_CONTA objetoAntesSubgrupo = new SUBGRUPO_PLANO_CONTA();
        List<SUBGRUPO_PLANO_CONTA> listaMasterSubgrupo = new List<SUBGRUPO_PLANO_CONTA>();
        CATEGORIA_CLIENTE objetoCatCliente = new CATEGORIA_CLIENTE();
        CATEGORIA_CLIENTE objetoAntesCatCliente = new CATEGORIA_CLIENTE();
        List<CATEGORIA_CLIENTE> listaMasterCatCliente = new List<CATEGORIA_CLIENTE>();
        CATEGORIA_FORNECEDOR objetoCatFornecedor = new CATEGORIA_FORNECEDOR();
        CATEGORIA_FORNECEDOR objetoAntesCatFornecedor= new CATEGORIA_FORNECEDOR();
        List<CATEGORIA_FORNECEDOR> listaMasterCatFornecedor= new List<CATEGORIA_FORNECEDOR>();
        TIPO_EMBALAGEM objetoTipoEmbalagem = new TIPO_EMBALAGEM();
        TIPO_EMBALAGEM objetoAntesTipoEmbalagem = new TIPO_EMBALAGEM();
        List<TIPO_EMBALAGEM> listaMasterTipoEmbalagem = new List<TIPO_EMBALAGEM>();
        TIPO_ACAO objetoTipoAcao = new TIPO_ACAO();
        TIPO_ACAO objetoAntesTipoAcao = new TIPO_ACAO();
        List<TIPO_ACAO> listaMasterTipoAcao = new List<TIPO_ACAO>();
        MOTIVO_CANCELAMENTO objetoMotCancelamento = new MOTIVO_CANCELAMENTO();
        MOTIVO_CANCELAMENTO objetoAntesMotCancelamento = new MOTIVO_CANCELAMENTO();
        List<MOTIVO_CANCELAMENTO> listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
        MOTIVO_ENCERRAMENTO objetoMotEncerramento = new MOTIVO_ENCERRAMENTO();
        MOTIVO_ENCERRAMENTO objetoAntesMotEncerramento = new MOTIVO_ENCERRAMENTO();
        List<MOTIVO_ENCERRAMENTO> listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
        CATEGORIA_PRODUTO objetoCatProduto = new CATEGORIA_PRODUTO();
        CATEGORIA_PRODUTO objetoAntesCatProduto = new CATEGORIA_PRODUTO();
        List<CATEGORIA_PRODUTO> listaMasterCatProduto = new List<CATEGORIA_PRODUTO>();
        SUBCATEGORIA_PRODUTO objetoSubCatProduto = new SUBCATEGORIA_PRODUTO();
        SUBCATEGORIA_PRODUTO objetoAntesSubCatProduto = new SUBCATEGORIA_PRODUTO();
        List<SUBCATEGORIA_PRODUTO> listaMasterSubCatProduto = new List<SUBCATEGORIA_PRODUTO>();
        CATEGORIA_CUSTO_FIXO objetoCatCusto = new CATEGORIA_CUSTO_FIXO();
        CATEGORIA_CUSTO_FIXO objetoAntesCatCusto = new CATEGORIA_CUSTO_FIXO();
        List<CATEGORIA_CUSTO_FIXO> listaMasterCatCusto = new List<CATEGORIA_CUSTO_FIXO>();
        String extensao;

        public TabelaAuxiliarController(ICargoAppService carApps, ILogAppService logApps, IGrupoCCAppService gruApps, ISubgrupoAppService subApps, ICategoriaClienteAppService ccApps, ICategoriaFornecedorAppService cfApps, ITipoEmbalagemAppService teApps, ITipoAcaoAppService taApps, IMotivoCancelamentoAppService mcApps, IMotivoEncerramentoAppService meApps, ICategoriaProdutoAppService cpApps, ISubcategoriaProdutoAppService spApps, ICategoriaCustoFixoAppService cxApps)
        {
            carApp = carApps;
            logApp = logApps;
            gruApp = gruApps;
            subApp = subApps;
            ccApp = ccApps;
            cfApp = cfApps;
            teApp = teApps;
            taApp = taApps;
            mcApp = mcApps;
            meApp = meApps;
            cpApp = cpApps;
            spApp = spApps;
            cxApp = cxApps;
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
            if ((Int32)Session["VoltaMensagem"] == 80)
            {
                return RedirectToAction("MontarTelaDashboardVendas", "CRM");
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
            Session["VoltaCargo"] = 1;
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
            if ((Int32)Session["VoltaCargo"] == 2)
            {
                return RedirectToAction("IncluirUsuario", "Usuario");
            }
            if ((Int32)Session["VoltaCargo"] == 3)
            {
                return RedirectToAction("VoltarAnexoUsuario", "Usuario");
            }
            if ((Int32)Session["VoltaCargo"] == 10)
            {
                return RedirectToAction("IncluirPessoaExterna", "PessoaExterna");
            }
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
                    return RedirectToAction("VoltarBaseCargo");
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

        [HttpGet]
        public ActionResult MontarTelaCatCliente()
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
            if (Session["ListaCatCliente"] == null)
            {
                listaMasterCatCliente= ccApp.GetAllItens(idAss);
                Session["ListaCatCliente"] = listaMasterCatCliente;
            }
            ViewBag.Listas = (List<CATEGORIA_CLIENTE>)Session["ListaCatCliente"];
            ViewBag.Title = "CatCliente";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_CLIENTE>)Session["ListaCatCliente"]).Count;

            if (Session["MensCatCliente"] != null)
            {
                if ((Int32)Session["MensCatCliente"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0176", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCliente"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCliente"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0177", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatCliente"] = 1;
            Session["MensCatCliente"] = 0;
            objetoCatCliente = new CATEGORIA_CLIENTE();
            return View(objetoCatCliente);
        }

        public ActionResult RetirarFiltroCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCatCliente"] = null;
            Session["FiltroCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        public ActionResult MostrarTudoCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatCliente= ccApp.GetAllItensAdm(idAss);
            Session["FiltroCatCliente"] = null;
            Session["ListaCatCliente"] = listaMasterCatCliente;
            return RedirectToAction("MontarTelaCatCliente");
        }

        public ActionResult VoltarBaseCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCatCliente"] == 2)
            {
                return RedirectToAction("IncluirCliente", "Cliente");
            }
            if ((Int32)Session["VoltaCatCliente"] == 3)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult IncluirCatCliente()
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
                    return RedirectToAction("MontarTelaCatCliente");
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
            CATEGORIA_CLIENTE item = new CATEGORIA_CLIENTE();
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CACL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatCliente(CategoriaClienteViewModel vm)
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
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ccApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatCliente"] = 3;
                        return RedirectToAction("MontarTelaCatCliente");
                    }
                    Session["IdVolta"] = item.CACL_CD_ID;

                    // Sucesso
                    listaMasterCatCliente= new List<CATEGORIA_CLIENTE>();
                    Session["ListaCatCliente"] = null;
                    return RedirectToAction("VoltarBaseCatCliente");
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
        public ActionResult VerCatCliente(Int32 id)
        {
            
            // Prepara view
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            Session["CatCliente"] = item;
            Session["IdCatCliente"] = id;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarCatCliente(Int32 id)
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
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            objetoAntesCatCliente= item;
            Session["CatCliente"] = item;
            Session["IdCatCliente"] = id;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatCliente(CategoriaClienteViewModel vm)
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
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    Int32 volta = ccApp.ValidateEdit(item, objetoAntesCatCliente, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatCliente= new List<CATEGORIA_CLIENTE>();
                    Session["ListaCatCliente"] = null;
                    return RedirectToAction("MontarTelaCatCliente");
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
        public ActionResult ExcluirCatCliente(Int32 id)
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
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            item.CACL_IN_ATIVO = 0;
            Int32 volta = ccApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatCliente"] = 4;
                return RedirectToAction("MontarTelaCatCliente");
            }
            listaMasterCatCliente = new List<CATEGORIA_CLIENTE>();
            Session["ListaCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult ReativarCatCliente(Int32 id)
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
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            item.CACL_IN_ATIVO = 1;
            Int32 volta = ccApp.ValidateReativar(item, usuario);
            listaMasterCatCliente = new List<CATEGORIA_CLIENTE>();
            Session["ListaCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult MontarTelaCatFornecedor()
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
            if (Session["ListaCatFornecedor"] == null)
            {
                listaMasterCatFornecedor= cfApp.GetAllItens(idAss);
                Session["ListaCatFornecedor"] = listaMasterCatFornecedor;
            }
            ViewBag.Listas = (List<CATEGORIA_FORNECEDOR>)Session["ListaCatFornecedor"];
            ViewBag.Title = "CatFornecedor";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_FORNECEDOR >)Session["ListaCatFornecedor"]).Count;

            if (Session["MensCatFornecedor"] != null)
            {
                if ((Int32)Session["MensCatFornecedor"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0178", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatFornecedor"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatFornecedor"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0179", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatFornecedor"] = 1;
            Session["MensCatFornecedor"] = 0;
            objetoCatFornecedor = new CATEGORIA_FORNECEDOR();
            return View(objetoCatFornecedor);
        }

        public ActionResult RetirarFiltroCatFornecedor()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCatFornecedor"] = null;
            Session["FiltroCatFornecedor"] = null;
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        public ActionResult MostrarTudoCatFornecedor()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatFornecedor = cfApp.GetAllItensAdm(idAss);
            Session["FiltroCatFornecedor"] = null;
            Session["ListaCatFornecedor"] = listaMasterCatFornecedor;
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        public ActionResult VoltarBaseCatFornecedor()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCatFornecedor"] == 2)
            {
                return RedirectToAction("IncluirFornecedor", "Fornecedor");
            }
            if ((Int32)Session["VoltaCatFornecedor"] == 3)
            {
                return RedirectToAction("VoltarAnexoFornecedor", "Fornecedor");
            }
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        [HttpGet]
        public ActionResult IncluirCatFornecedor()
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
                    return RedirectToAction("MontarTelaCatFornecedor");
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
            CATEGORIA_FORNECEDOR item = new CATEGORIA_FORNECEDOR();
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CAFO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatFornecedor(CategoriaFornecedorViewModel vm)
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
                    CATEGORIA_FORNECEDOR item = Mapper.Map<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cfApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatFornecedor"] = 3;
                        return RedirectToAction("MontarTelaCatFornecedor");
                    }
                    Session["IdVolta"] = item.CAFO_CD_ID;

                    // Sucesso
                    listaMasterCatFornecedor = new List<CATEGORIA_FORNECEDOR>();
                    Session["ListaCatFornecedor"] = null;
                    return RedirectToAction("VoltarBaseCatFornecedor");
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
        public ActionResult VerCatFornecedor(Int32 id)
        {
            
            // Prepara view
            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            Session["CatFornecedor"] = item;
            Session["IdCatFornecedor"] = id;
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarCatFornecedor(Int32 id)
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
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            objetoAntesCatFornecedor = item;
            Session["CatFornecedor"] = item;
            Session["IdCatFornecedor"] = id;
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatFornecedor(CategoriaFornecedorViewModel vm)
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
                    CATEGORIA_FORNECEDOR item = Mapper.Map<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>(vm);
                    Int32 volta = cfApp.ValidateEdit(item, objetoAntesCatFornecedor, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatFornecedor= new List<CATEGORIA_FORNECEDOR>();
                    Session["ListaCatFornecedor"] = null;
                    return RedirectToAction("MontarTelaCatFornecedor");
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
        public ActionResult ExcluirCatFornecedor(Int32 id)
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
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            item.CAFO_IN_ATIVO = 0;
            Int32 volta = cfApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatFornecedor"] = 4;
                return RedirectToAction("MontarTelaCatFornecedor");
            }
            listaMasterCatFornecedor = new List<CATEGORIA_FORNECEDOR>();
            Session["ListaCatFornecedor"] = null;
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        [HttpGet]
        public ActionResult ReativarCatFornecedor(Int32 id)
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
                    return RedirectToAction("MontarTelaCatFornecedor");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            item.CAFO_IN_ATIVO = 1;
            Int32 volta = cfApp.ValidateReativar(item, usuario);
            listaMasterCatFornecedor= new List<CATEGORIA_FORNECEDOR>();
            Session["ListaCatFornecedor"] = null;
            return RedirectToAction("MontarTelaCatFornecedor");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoEmbalagem()
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
            if (Session["ListaTipoEmbalagem"] == null)
            {
                listaMasterTipoEmbalagem = teApp.GetAllItens(idAss);
                Session["ListaTipoEmbalagem"] = listaMasterTipoEmbalagem;
            }
            ViewBag.Listas = (List<TIPO_EMBALAGEM>)Session["ListaTipoEmbalagem"];
            ViewBag.Title = "TipoEmbalagem";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoEmbalagem = ((List<TIPO_EMBALAGEM>)Session["ListaTipoEmbalagem"]).Count;

            if (Session["MensTipoEmbalagem"] != null)
            {
                if ((Int32)Session["MensTipoEmbalagem"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0185", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoEmbalagem"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoEmbalagem"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0186", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoEmbalagem"] = 1;
            Session["MensTipoEmbalagem"] = 0;
            objetoTipoEmbalagem = new TIPO_EMBALAGEM();
            return View(objetoTipoEmbalagem);
        }

        public ActionResult RetirarFiltroTipoEmbalagem()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaTipoEmbalagem"] = null;
            Session["FiltroTipoEmbalagem"] = null;
            return RedirectToAction("MontarTelaTipoEmbalagem");
        }

        public ActionResult MostrarTudoTipoEmbalagem()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTipoEmbalagem= teApp.GetAllItensAdm(idAss);
            Session["FiltroTipoEmbalagem"] = null;
            Session["ListaTipoEmbalagem"] = listaMasterTipoEmbalagem;
            return RedirectToAction("MontarTelaTipoEmbalagem");
        }

        public ActionResult VoltarBaseTipoEmbalagem()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaTipoEmbalagem"] == 2)
            {
                return RedirectToAction("IncluirProduto", "Produto");
            }
            if ((Int32)Session["VoltaTipoEmbalagem"] == 3)
            {
                return RedirectToAction("VoltarAnexoProduto", "Produto");
            }
            return RedirectToAction("MontarTelaTipoEmbalagem");
        }

        [HttpGet]
        public ActionResult IncluirTipoEmbalagem()
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
            TIPO_EMBALAGEM item = new TIPO_EMBALAGEM();
            TipoEmbalagemViewModel vm = Mapper.Map<TIPO_EMBALAGEM, TipoEmbalagemViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIEM_IN_ATIVO = 1;
            vm.TIEM_NR_ESTOQUE = 0;
            vm.TIIEM_VL_CUSTO_UNITARIO = 0;
            vm.TIEM_VLCUSTO_REPASSADO = 0;
            vm.TIEM_ESTOQUE_INICIAL = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoEmbalagem(TipoEmbalagemViewModel vm)
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
                    TIPO_EMBALAGEM item = Mapper.Map<TipoEmbalagemViewModel, TIPO_EMBALAGEM>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = teApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoEmbalagem"] = 3;
                        return RedirectToAction("MontarTelaTipoEmbalagem");
                    }
                    Session["IdVolta"] = item.TIEM_CD_ID;

                    // Sucesso
                    listaMasterTipoEmbalagem = new List<TIPO_EMBALAGEM>();
                    Session["ListaTipoEmbalagem"] = null;
                    return RedirectToAction("VoltarBaseTipoEmbalagem");
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
        public ActionResult EditarTipoEmbalagem(Int32 id)
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
            TIPO_EMBALAGEM item = teApp.GetItemById(id);
            objetoAntesTipoEmbalagem = item;
            Session["TipoEmbalagem"] = item;
            Session["IdTipoEmbalagem"] = id;
            TipoEmbalagemViewModel vm = Mapper.Map<TIPO_EMBALAGEM, TipoEmbalagemViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoEmbalagem(TipoEmbalagemViewModel vm)
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
                    TIPO_EMBALAGEM item = Mapper.Map<TipoEmbalagemViewModel, TIPO_EMBALAGEM>(vm);
                    Int32 volta = teApp.ValidateEdit(item, objetoAntesTipoEmbalagem, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTipoEmbalagem = new List<TIPO_EMBALAGEM>();
                    Session["ListaTipoEmbalagem"] = null;
                    return RedirectToAction("MontarTelaTipoEmbalagem");
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
        public ActionResult ExcluirTipoEmbalagem(Int32 id)
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
            TIPO_EMBALAGEM item = teApp.GetItemById(id);
            objetoAntesTipoEmbalagem = item;
            item.TIEM_IN_ATIVO = 0;
            Int32 volta = teApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoEmbalagem"] = 4;
                return RedirectToAction("MontarTelaTipoEmbalagem");
            }
            listaMasterTipoEmbalagem = new List<TIPO_EMBALAGEM>();
            Session["ListaTipoEmbalagem"] = null;
            return RedirectToAction("MontarTelaTipoEmbalagem");
        }

        [HttpGet]
        public ActionResult ReativarTipoEmbalagem(Int32 id)
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
            TIPO_EMBALAGEM item = teApp.GetItemById(id);
            objetoAntesTipoEmbalagem = item;
            item.TIEM_IN_ATIVO = 1;
            Int32 volta = teApp.ValidateReativar(item, usuario);
            listaMasterTipoEmbalagem = new List<TIPO_EMBALAGEM>();
            Session["ListaTipoEmbalagem"] = null;
            return RedirectToAction("MontarTelaTipoEmbalagem");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoAcao()
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
            if (Session["ListaTipoAcao"] == null)
            {
                listaMasterTipoAcao = taApp.GetAllItens(idAss);
                Session["ListaTipoAcao"] = listaMasterTipoAcao;
            }
            ViewBag.Listas = (List<TIPO_ACAO>)Session["ListaTipoAcao"];
            ViewBag.Title = "TipoAcao";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoAcao = ((List<TIPO_ACAO>)Session["ListaTipoAcao"]).Count;

            if (Session["MensTipoAcao"] != null)
            {
                if ((Int32)Session["MensTipoAcao"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0187", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoAcao"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoAcao"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0188", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoAcao"] = 1;
            Session["MensTipoAcao"] = 0;
            objetoTipoAcao = new TIPO_ACAO();
            return View(objetoTipoAcao);
        }

        public ActionResult RetirarFiltroTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoAcao"] = null;
            Session["FiltroTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        public ActionResult MostrarTudoTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTipoAcao= taApp.GetAllItensAdm(idAss);
            Session["FiltroTipoAcao"] = null;
            Session["ListaTipoAcao"] = listaMasterTipoAcao;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        public ActionResult VoltarBaseTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaTipoAcao"] == 2)
            {
                return RedirectToAction("IncluirAcao", "CRM");
            }
            if ((Int32)Session["VoltaTipoAcao"] == 3)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult IncluirTipoAcao()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_ACAO item = new TIPO_ACAO();
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIAC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoAcao(TipoAcaoViewModel vm)
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
                    TIPO_ACAO item = Mapper.Map<TipoAcaoViewModel, TIPO_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = taApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoAcao"] = 3;
                        return RedirectToAction("MontarTelaTipoAcao");
                    }
                    Session["IdVolta"] = item.TIAC_CD_ID;

                    // Sucesso
                    listaMasterTipoAcao = new List<TIPO_ACAO>();
                    Session["ListaTipoAcao"] = null;
                    return RedirectToAction("MontarTelaTipoAcao");
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
        public ActionResult VerTipoAcao(Int32 id)
        {
            
            // Prepara view
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            Session["TipoAcao"] = item;
            Session["IdTipoAcao"] = id;
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarTipoAcao(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            Session["TipoAcao"] = item;
            Session["IdTipoAcao"] = id;
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoAcao(TipoAcaoViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_ACAO item = Mapper.Map<TipoAcaoViewModel, TIPO_ACAO>(vm);
                    Int32 volta = taApp.ValidateEdit(item, objetoAntesTipoAcao, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTipoAcao = new List<TIPO_ACAO>();
                    Session["ListaTipoAcao"] = null;
                    return RedirectToAction("MontarTelaTipoAcao");
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
        public ActionResult ExcluirTipoAcao(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            item.TIAC_IN_ATIVO = 0;
            Int32 volta = taApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoAcao"] = 4;
                return RedirectToAction("MontarTelaTipoAcao");
            }
            listaMasterTipoAcao = new List<TIPO_ACAO>();
            Session["ListaTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult ReativarTipoAcao(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TIPO_ACAO item = taApp.GetItemById(id);
            item.TIAC_IN_ATIVO = 1;
            objetoAntesTipoAcao = item;
            Int32 volta = taApp.ValidateReativar(item, usuario);
            listaMasterTipoAcao = new List<TIPO_ACAO>();
            Session["ListaTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult MontarTelaMotCancelamento()
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
            if (Session["ListaMotCancelamento"] == null)
            {
                listaMasterMotCancelamento = mcApp.GetAllItens(idAss);
                Session["ListaMotCancelamento"] = listaMasterMotCancelamento;
            }
            ViewBag.Listas = (List<MOTIVO_CANCELAMENTO>)Session["ListaMotCancelamento"];
            ViewBag.Title = "MotCancelamento";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<MOTIVO_CANCELAMENTO>)Session["ListaMotCancelamento"]).Count;

            if (Session["MensMotCancelamento"] != null)
            {
                if ((Int32)Session["MensMotCancelamento"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0189", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotCancelamento"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotCancelamento"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0190", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMotCancelamento"] = 1;
            Session["MensMotCancelamento"] = 0;
            objetoMotCancelamento = new MOTIVO_CANCELAMENTO();
            return View(objetoMotCancelamento);
        }

        public ActionResult RetirarFiltroMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        public ActionResult MostrarTudoMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterMotCancelamento = mcApp.GetAllItensAdm(idAss);
            Session["ListaMotCancelamento"] = listaMasterMotCancelamento;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        public ActionResult VoltarBaseMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaMotCancelamento"] == 2)
            {
                return RedirectToAction("VoltarCancelarPedido", "CRM");
            }
            if ((Int32)Session["VoltaMotCancelamento"] == 3)
            {
                return RedirectToAction("VoltarCancelarProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult IncluirMotCancelamento()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_CANCELAMENTO item = new MOTIVO_CANCELAMENTO();
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.MOCA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirMotCancelamento(MotivoCancelamentoViewModel vm)
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
                    MOTIVO_CANCELAMENTO item = Mapper.Map<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = mcApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMotCancelamento"] = 3;
                        return RedirectToAction("MontarTelaMotCancelamento");
                    }
                    Session["IdVolta"] = item.MOCA_CD_ID;

                    // Sucesso
                    listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
                    Session["ListaMotCancelamento"] = null;
                    return RedirectToAction("VoltarBaseMotCancelamento");
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
        public ActionResult VerMotCancelamento(Int32 id)
        {
            
            // Prepara view
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            Session["MotCancelamento"] = item;
            Session["IdMotCancelamento"] = id;
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarMotCancelamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            Session["MotCancelamento"] = item;
            Session["IdMotCancelamento"] = id;
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMotCancelamento(MotivoCancelamentoViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MOTIVO_CANCELAMENTO item = Mapper.Map<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>(vm);
                    Int32 volta = mcApp.ValidateEdit(item, objetoAntesMotCancelamento, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
                    Session["ListaMotCancelamento"] = null;
                    return RedirectToAction("MontarTelaMotCancelamento");
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
        public ActionResult ExcluirMotCancelamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            item.MOCA_IN_ATIVO = 0;
            Int32 volta = mcApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMotCancelamento"] = 4;
                return RedirectToAction("MontarTelaMotCancelamento");
            }
            listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult ReativarMotCancelamento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            item.MOCA_IN_ATIVO = 1;
            objetoAntesMotCancelamento = item;
            Int32 volta = mcApp.ValidateReativar(item, usuario);
            listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult MontarTelaMotEncerramento()
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
            if (Session["ListaMotEncerramento"] == null)
            {
                listaMasterMotEncerramento = meApp.GetAllItens(idAss);
                Session["ListaMotEncerramento"] = listaMasterMotEncerramento;
            }
            ViewBag.Listas = (List<MOTIVO_ENCERRAMENTO>)Session["ListaMotEncerramento"];
            ViewBag.Title = "MotEncerramento";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<MOTIVO_ENCERRAMENTO>)Session["ListaMotEncerramento"]).Count;

            if (Session["MensMotEncerramento"] != null)
            {
                if ((Int32)Session["MensMotEncerramento"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0191", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotEncerramento"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotEncerramento"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0192", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMotEncerramento"] = 1;
            Session["MensMotEncerramento"] = 0;
            objetoMotEncerramento = new MOTIVO_ENCERRAMENTO();
            return View(objetoMotEncerramento);
        }

        public ActionResult RetirarFiltroMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        public ActionResult MostrarTudoMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterMotEncerramento = meApp.GetAllItensAdm(idAss);
            Session["ListaMotEncerramento"] = listaMasterMotEncerramento;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        public ActionResult VoltarBaseMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaMotEncerramento"] == 2)
            {
                return RedirectToAction("VoltarEncerrarProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult IncluirMotEncerramento()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_ENCERRAMENTO item = new MOTIVO_ENCERRAMENTO();
            MotivoEncerramentoViewModel vm = Mapper.Map<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.MOEN_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirMotEncerramento(MotivoEncerramentoViewModel vm)
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
                    MOTIVO_ENCERRAMENTO item = Mapper.Map<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = meApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMotEncerramento"] = 3;
                        return RedirectToAction("MontarTelaMotEncerramento");
                    }
                    Session["IdVolta"] = item.MOEN_CD_ID;

                    // Sucesso
                    listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
                    Session["ListaMotEncerramento"] = null;
                    return RedirectToAction("VoltarBaseMotEncerramento");
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
        public ActionResult EditarMotEncerramento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            objetoAntesMotEncerramento = item;
            Session["MotEncerramento"] = item;
            Session["IdMotEncerramento"] = id;
            MotivoEncerramentoViewModel vm = Mapper.Map<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMotEncerramento(MotivoEncerramentoViewModel vm)
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
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MOTIVO_ENCERRAMENTO item = Mapper.Map<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>(vm);
                    Int32 volta = meApp.ValidateEdit(item, objetoAntesMotEncerramento, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
                    Session["ListaMotEncerramento"] = null;
                    return RedirectToAction("MontarTelaMotEncerramento");
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
        public ActionResult ExcluirMotEncerramento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            objetoAntesMotEncerramento = item;
            item.MOEN_IN_ATIVO = 0;
            Int32 volta = meApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMotEncerramento"] = 4;
                return RedirectToAction("MontarTelaMotEncerramento");
            }
            listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult ReativarMotEncerramento(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            item.MOEN_CD_ID = 1;
            objetoAntesMotEncerramento = item;
            Int32 volta = meApp.ValidateReativar(item, usuario);
            listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult MontarTelaCatProduto()
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
            if (Session["ListaCatProduto"] == null)
            {
                listaMasterCatProduto= cpApp.GetAllItens(idAss);
                Session["ListaCatProduto"] = listaMasterCatProduto;
            }
            ViewBag.Listas = (List<CATEGORIA_PRODUTO>)Session["ListaCatProduto"];
            ViewBag.Title = "CatProduto";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_PRODUTO>)Session["ListaCatProduto"]).Count;

            if (Session["MensCatProduto"] != null)
            {
                if ((Int32)Session["MensCatProduto"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0225", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatProduto"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatProduto"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0177", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatProduto"] = 1;
            Session["MensCatProduto"] = 0;
            objetoCatProduto = new CATEGORIA_PRODUTO();
            return View(objetoCatProduto);
        }

        public ActionResult RetirarFiltroCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCatProduto"] = null;
            Session["FiltroCatProduto"] = null;
            return RedirectToAction("MontarTelaCatProduto");
        }

        public ActionResult MostrarTudoCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatProduto= cpApp.GetAllItensAdm(idAss);
            Session["FiltroCatProduto"] = null;
            Session["ListaCatProduto"] = listaMasterCatProduto;
            return RedirectToAction("MontarTelaCatProduto");
        }

        public ActionResult VoltarBaseCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCatProduto"] == 2)
            {
                return RedirectToAction("IncluirProduto", "Produto");
            }
            if ((Int32)Session["VoltaCatProduto"] == 3)
            {
                return RedirectToAction("VoltarAnexoProduto", "Produto");
            }
            return RedirectToAction("MontarTelaCatProduto");
        }

        [HttpGet]
        public ActionResult IncluirCatProduto()
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
                    return RedirectToAction("MontarTelaCatProduto");
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
            CATEGORIA_PRODUTO item = new CATEGORIA_PRODUTO();
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CAPR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatProduto(CategoriaProdutoViewModel vm)
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
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cpApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatProduto"] = 3;
                        return RedirectToAction("MontarTelaCatProduto");
                    }
                    Session["IdVolta"] = item.CAPR_CD_ID;

                    // Sucesso
                    listaMasterCatProduto= new List<CATEGORIA_PRODUTO>();
                    Session["ListaCatProduto"] = null;
                    return RedirectToAction("VoltarBaseCatProduto");
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
        public ActionResult EditarCatProduto(Int32 id)
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
                    return RedirectToAction("MontarTelaCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            objetoAntesCatProduto= item;
            Session["CatProduto"] = item;
            Session["IdCatProduto"] = id;
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatProduto(CategoriaProdutoViewModel vm)
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
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    Int32 volta = cpApp.ValidateEdit(item, objetoAntesCatProduto, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatProduto= new List<CATEGORIA_PRODUTO>();
                    Session["ListaCatProduto"] = null;
                    return RedirectToAction("MontarTelaCatProduto");
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
        public ActionResult ExcluirCatProduto(Int32 id)
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
                    return RedirectToAction("MontarTelaCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            item.CAPR_IN_ATIVO = 0;
            Int32 volta = cpApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatProduto"] = 4;
                return RedirectToAction("MontarTelaCatProduto");
            }
            listaMasterCatProduto= new List<CATEGORIA_PRODUTO>();
            Session["ListaCatProduto"] = null;
            return RedirectToAction("MontarTelaCatProduto");
        }

        [HttpGet]
        public ActionResult ReativarCatProduto(Int32 id)
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
                    return RedirectToAction("MontarTelaCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            item.CAPR_IN_ATIVO = 1;
            Int32 volta = cpApp.ValidateReativar(item, usuario);
            listaMasterCatProduto= new List<CATEGORIA_PRODUTO>();
            Session["ListaCatProduto"] = null;
            return RedirectToAction("MontarTelaCatProduto");
        }

        [HttpGet]
        public ActionResult MontarTelaSubCatProduto()
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
            if (Session["ListaSubCatProduto"] == null)
            {
                listaMasterSubCatProduto= spApp.GetAllItens(idAss);
                Session["ListaSubCatProduto"] = listaMasterSubCatProduto;
            }
            ViewBag.Listas = (List<SUBCATEGORIA_PRODUTO>)Session["ListaSubCatProduto"];
            ViewBag.CatProduto = new SelectList(spApp.GetAllCategorias(idAss), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Title = "SubCatProduto";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.SubCatProduto = ((List<SUBCATEGORIA_PRODUTO>)Session["ListaSubCatProduto"]).Count;

            if (Session["MensSubCatProduto"] != null)
            {
                if ((Int32)Session["MensSubCatProduto"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0226", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubCatProduto"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubCatProduto"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0227", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensSubCatProduto"] = 0;
            objetoSubCatProduto = new SUBCATEGORIA_PRODUTO();
            return View(objetoSubCatProduto);
        }

        public ActionResult RetirarFiltroSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaSubCatProduto"] = null;
            Session["FiltroSubCatProduto"] = null;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        public ActionResult MostrarTudoSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSubCatProduto = spApp.GetAllItensAdm(idAss);
            Session["FiltroSubCatProduto"] = null;
            Session["ListaSubCatProduto"] = listaMasterSubgrupo;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        public ActionResult VoltarBaseSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaSubCatProduto"] == 2)
            {
                return RedirectToAction("IncluirProduto", "Produto");
            }
            if ((Int32)Session["VoltaSubCatProduto"] == 3)
            {
                return RedirectToAction("VoltarAnexoProduto", "Produto");
            }
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        [HttpGet]
        public ActionResult IncluirSubCatProduto()
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
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.CatProduto = new SelectList(spApp.GetAllCategorias(idAss), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            SUBCATEGORIA_PRODUTO item = new SUBCATEGORIA_PRODUTO();
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SCPR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirSubCatProduto(SubCategoriaProdutoViewModel vm)
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
                    SUBCATEGORIA_PRODUTO item = Mapper.Map<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = spApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensSubCatProduto"] = 3;
                        return RedirectToAction("MontarTelaSubCatProduto");
                    }
                    Session["IdSubCatProduto"] = item.SCPR_CD_ID;

                    // Sucesso
                    listaMasterSubCatProduto= new List<SUBCATEGORIA_PRODUTO>();
                    Session["ListaSubCatProduto"] = null;
                    return RedirectToAction("MontarTelaSubCatProduto");
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
        public ActionResult EditarSubCatProduto(Int32 id)
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
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            objetoAntesSubCatProduto = item;
            Session["SubCatProduto"] = item;
            Session["IdSubCatProduto"] = id;
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarSubCatProduto(SubCategoriaProdutoViewModel vm)
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
                    SUBCATEGORIA_PRODUTO item = Mapper.Map<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>(vm);
                    Int32 volta = spApp.ValidateEdit(item, objetoAntesSubCatProduto, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSubCatProduto = new List<SUBCATEGORIA_PRODUTO>();
                    Session["ListaSubCatProduto"] = null;
                    return RedirectToAction("MontarTelaSubCatProduto");
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
        public ActionResult ExcluirSubCatProduto(Int32 id)
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
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            objetoAntesSubCatProduto = item;
            item.SCPR_IN_ATIVO = 0;
            Int32 volta = spApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensSubCatProduto"] = 4;
                return RedirectToAction("MontarTelaSubCatProduto");
            }
            listaMasterSubCatProduto= new List<SUBCATEGORIA_PRODUTO>();
            Session["ListaSubCatProduto"] = null;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        [HttpGet]
        public ActionResult ReativarSubCatProduto(Int32 id)
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
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            objetoAntesSubCatProduto = item;
            item.SCPR_IN_ATIVO = 1;
            Int32 volta = spApp.ValidateDelete(item, usuario);
            listaMasterSubCatProduto = new List<SUBCATEGORIA_PRODUTO>();
            Session["ListaSubCatProduto"] = null;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        [HttpGet]
        public ActionResult MontarTelaCatCusto()
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
            if (Session["ListaCatCusto"] == null)
            {
                listaMasterCatCusto = cxApp.GetAllItens(idAss);
                Session["ListaCatCusto"] = listaMasterCatCusto;
            }
            ViewBag.Listas = (List<CATEGORIA_CUSTO_FIXO>)Session["ListaCatCusto"];
            ViewBag.Title = "CatCusto";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_CUSTO_FIXO>)Session["ListaCatCusto"]).Count;

            if (Session["MensCatCusto"] != null)
            {
                if ((Int32)Session["MensCatCusto"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0236", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCusto"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCusto"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0237", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatCusto"] = 1;
            Session["MensCatCusto"] = 0;
            objetoCatCusto= new CATEGORIA_CUSTO_FIXO();
            return View(objetoCatCusto);
        }

        public ActionResult RetirarFiltroCatCusto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCatCusto"] = null;
            Session["FiltroCatCusto"] = null;
            return RedirectToAction("MontarTelaCatCusto");
        }

        public ActionResult MostrarTudoCatCusto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatCusto = cxApp.GetAllItensAdm(idAss);
            Session["FiltroCatCusto"] = null;
            Session["ListaCatCusto"] = listaMasterCatCusto;
            return RedirectToAction("MontarTelaCatCusto");
        }

        public ActionResult VoltarBaseCatCusto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCatCusto"] == 2)
            {
                return RedirectToAction("IncluirCustoFixo", "CustoFixo");
            }
            if ((Int32)Session["VoltaCatCusto"] == 3)
            {
                return RedirectToAction("VoltarAnexoCustoFixo", "CustoFixo");
            }
            return RedirectToAction("MontarTelaCatCusto");
        }

        [HttpGet]
        public ActionResult IncluirCatCusto()
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
                    return RedirectToAction("MontarTelaCatCliente");
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
            CATEGORIA_CUSTO_FIXO item = new CATEGORIA_CUSTO_FIXO();
            CategoriaCustoFixoViewModel vm = Mapper.Map<CATEGORIA_CUSTO_FIXO, CategoriaCustoFixoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CACF_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatCusto(CategoriaCustoFixoViewModel vm)
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
                    CATEGORIA_CUSTO_FIXO item = Mapper.Map<CategoriaCustoFixoViewModel, CATEGORIA_CUSTO_FIXO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cxApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatCusto"] = 3;
                        return RedirectToAction("MontarTelaCatCusto");
                    }
                    Session["IdVolta"] = item.CACF_CD_ID;

                    // Sucesso
                    listaMasterCatCusto= new List<CATEGORIA_CUSTO_FIXO>();
                    Session["ListaCatCusto"] = null;
                    return RedirectToAction("VoltarBaseCatCusto");
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
        public ActionResult VerCatCusto(Int32 id)
        {
            
            // Prepara view
            CATEGORIA_CUSTO_FIXO item = cxApp.GetItemById(id);
            Session["CatCusto"] = item;
            Session["IdCatCusto"] = id;
            CategoriaCustoFixoViewModel vm = Mapper.Map<CATEGORIA_CUSTO_FIXO, CategoriaCustoFixoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarCatCusto(Int32 id)
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
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_CUSTO_FIXO item = cxApp.GetItemById(id);
            objetoAntesCatCusto= item;
            Session["CatCusto"] = item;
            Session["IdCatCusto"] = id;
            CategoriaCustoFixoViewModel vm = Mapper.Map<CATEGORIA_CUSTO_FIXO, CategoriaCustoFixoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatCusto(CategoriaCustoFixoViewModel vm)
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
                    CATEGORIA_CUSTO_FIXO item = Mapper.Map<CategoriaCustoFixoViewModel, CATEGORIA_CUSTO_FIXO>(vm);
                    Int32 volta = cxApp.ValidateEdit(item, objetoAntesCatCusto, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatCusto= new List<CATEGORIA_CUSTO_FIXO>();
                    Session["ListaCatCusto"] = null;
                    return RedirectToAction("MontarTelaCatCusto");
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
        public ActionResult ExcluirCatCusto(Int32 id)
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
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_CUSTO_FIXO item = cxApp.GetItemById(id);
            item.CACF_IN_ATIVO = 0;
            Int32 volta = cxApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatCusto"] = 4;
                return RedirectToAction("MontarTelaCatCusto");
            }
            listaMasterCatCusto= new List<CATEGORIA_CUSTO_FIXO>();
            Session["ListaCatCusto"] = null;
            return RedirectToAction("MontarTelaCatCusto");
        }

        [HttpGet]
        public ActionResult ReativarCatCusto(Int32 id)
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
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_CUSTO_FIXO item = cxApp.GetItemById(id);
            item.CACF_IN_ATIVO = 1;
            Int32 volta = cxApp.ValidateReativar(item, usuario);
            listaMasterCatCusto = new List<CATEGORIA_CUSTO_FIXO>();
            Session["ListaCatCusto"] = null;
            return RedirectToAction("MontarTelaCatCusto");
        }


    }
}