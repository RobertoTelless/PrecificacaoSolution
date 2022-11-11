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
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;

namespace ERP_Condominios_Solution.Controllers
{
    public class ControleAcessoController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();


        public ControleAcessoController(IUsuarioAppService baseApps)
        {
            baseApp = baseApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            USUARIO item = new USUARIO();
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult Login()
        {
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario;
                Session["UserCredentials"] = null;
                ViewBag.Usuario = null;
                USUARIO login = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateLogin(login.USUA_NM_LOGIN, login.USUA_NM_SENHA, out usuario);
                if (volta == 1)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 5)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 6)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 7)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 9)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 10)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 11)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0012", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 20)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Armazena credenciais para autorização
                Session["UserCredentials"] = usuario;
                Session["Usuario"] = usuario;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["Assinante"] = usuario.ASSINANTE;
                Session["PlanosVencidos"] = null;

                // Reseta flags de permissao e totais
                Session["PermMens"] = 0;
                Session["PermCRM"] = 0;
                Session["PermServDesk"] = 0;
                Session["NumSMS"] = 0;
                Session["NumEMail"] = 0;
                Session["NumZap"] = 0;
                Session["NumClientes"] = 0;
                Session["NumAcoes"] = 0;
                Session["NumProcessos"] = 0;
                Session["NumProcessosBase"] = 0;
                Session["NumUsuarios"] = 0;
                Session["MensagemLogin"] = 0;
                Session["MensPermissao"] = 0;
                Session["NumProduto"] = 0;
                Session["NumFornecedor"] = 0;

                // Recupera Planos do assinante
                List<PlanoVencidoViewModel> vencidos = new List<PlanoVencidoViewModel>();
                List<ASSINANTE_PLANO> plAss = usuario.ASSINANTE.ASSINANTE_PLANO.ToList();
                plAss = plAss.Where(p => p.ASPL_IN_ATIVO == 1).ToList();
                
                List<PLANO> planos = new List<PLANO>();
                if (plAss.Count == 0)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0215", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                foreach (ASSINANTE_PLANO item in plAss)
                {
                    // Verifica validade
                    if (item.ASPL_DT_VALIDADE < DateTime.Today.Date)
                    {
                        PlanoVencidoViewModel plan = new PlanoVencidoViewModel();
                        plan.Plano = item.PLAN_CD_ID;
                        plan.Assinante = item.ASSI_CD_ID;
                        plan.Vencimento = item.ASPL_DT_VALIDADE;
                        plan.Tipo = 1;
                        vencidos.Add(plan);                      
                        continue;
                    }
                    if (item.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30))
                    {
                        PlanoVencidoViewModel plan = new PlanoVencidoViewModel();
                        plan.Plano = item.PLAN_CD_ID;
                        plan.Assinante = item.ASSI_CD_ID;
                        plan.Vencimento = item.ASPL_DT_VALIDADE;
                        plan.Tipo = 2;
                        vencidos.Add(plan);
                    }

                    // Processa planos
                    planos.Add(item.PLANO);
                    Session["NumUsuarios"] = item.PLANO.PLAN_NR_USUARIOS;
                    if (item.PLANO.PLAN_IN_MENSAGENS == 1)
                    {
                        Session["PermMens"] = 1;
                        if ((Int32)Session["NumSMS"] < item.PLANO.PLAN_NR_SMS)
                        {
                            Session["NumSMS"] = item.PLANO.PLAN_NR_SMS;
                        }
                        if ((Int32)Session["NumEMail"] < item.PLANO.PLAN_NR_EMAIL)
                        {
                            Session["NumEMail"] = item.PLANO.PLAN_NR_EMAIL;
                        }
                        if ((Int32)Session["NumZap"] < item.PLANO.PLAN_NR_WHATSAPP)
                        {
                            Session["NumZap"] = item.PLANO.PLAN_NR_WHATSAPP;
                        }
                        if ((Int32)Session["NumClientes"] < item.PLANO.PLAN_NR_CONTATOS)
                        {
                            Session["NumClientes"] = item.PLANO.PLAN_NR_CONTATOS;
                        }
                    }
                    if (item.PLANO.PLAN_IN_CRM == 1)
                    {
                        Session["PermCRM"] = 1;
                        if ((Int32)Session["NumProcessos"] < item.PLANO.PLAN_NR_PROCESSOS)
                        {
                            Session["NumProcessos"] = item.PLANO.PLAN_NR_PROCESSOS;
                        }
                        if ((Int32)Session["NumAcoes"] < item.PLANO.PLAN_NR_ACOES)
                        {
                            Session["NumAcoes"] = item.PLANO.PLAN_NR_ACOES;
                        }
                        if ((Int32)Session["NumClientes"] < item.PLANO.PLAN_NR_CONTATOS)
                        {
                            Session["NumClientes"] = item.PLANO.PLAN_NR_CONTATOS;
                        }
                        if ((Int32)Session["NumProcessosBase"] < item.PLANO.PLAN_NR_PROCESSOS)
                        {
                            Session["NumProcessosBase"] = item.PLANO.PLAN_NR_PROCESSOS;
                        }
                    }
                    if ((Int32)Session["NumProduto"] < item.PLANO.PLAN_NR_PRODUTO)
                    {
                        Session["NumProduto"] = item.PLANO.PLAN_NR_PRODUTO;
                    }
                    if ((Int32)Session["NumFornecedor"] < item.PLANO.PLAN_NR_FORNECEDOR)
                    {
                        Session["NumFornecedor"] = item.PLANO.PLAN_NR_FORNECEDOR;
                    }
                }

                // Verifica Acesso
                Session["PlanosVencidosModel"] = vencidos;
                if (planos.Count == 0)
                {
                    Session["MensagemLogin"] = 1;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Session["Planos"] = planos;
                Session["ListaMensagem"] = null;

                // Atualiza view
                String frase = String.Empty;
                String nome = usuario.USUA_NM_NOME;
                if (DateTime.Now.Hour <= 12)
                {
                    frase = "Bom dia, " + nome;
                }
                else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
                {
                    frase = "Boa tarde, " + nome;
                }
                else
                {
                    frase = "Boa noite, " + nome;
                }

                ViewBag.Greeting = frase;
                ViewBag.Nome = usuario.USUA_NM_NOME;
                if (usuario.CARGO_USUARIO != null)
                {
                    ViewBag.Cargo = usuario.CARGO_USUARIO.CARG_NM_NOME;
                    Session["Cargo"] = usuario.CARGO_USUARIO.CARG_NM_NOME;
                }
                else
                {
                    ViewBag.Cargo = "-";
                    Session["Cargo"] = "-";
                }
                ViewBag.Foto = usuario.USUA_AQ_FOTO;

                // Trata Nome
                String nomeMax = String.Empty;
                if (usuario.USUA_NM_NOME.Contains(" "))
                {
                    nomeMax = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
                }
                else
                {
                    nomeMax = usuario.USUA_NM_NOME;
                }

                // Carrega Sessions
                Session["NomeMax"] = nomeMax;
                Session["Greeting"] = frase;
                Session["Nome"] = usuario.USUA_NM_NOME;
                Session["Foto"] = usuario.USUA_AQ_FOTO;
                Session["Perfil"] = usuario.PERFIL;
                Session["PerfilSigla"] = usuario.PERFIL.PERF_SG_SIGLA;
                Session["FlagInicial"] = 0;
                Session["FiltroData"] = 1;
                Session["FiltroStatus"] = 1;
                Session["Ativa"] = "1";
                Session["Login"] = 1;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["IdUsuario"] = usuario.USUA_CD_ID;

                // Grava flag de logado
                usuario.USUA_IN_LOGADO = 1;
                Int32 volta5 = baseApp.ValidateEdit(usuario, usuario);

                // Route
                if (usuario.USUA_IN_PROVISORIO == 1)
                {
                    return RedirectToAction("TrocarSenhaInicio", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        public ActionResult Logout()
        {
            Session["UserCredentials"] = null;
            Session.Clear();
            return RedirectToAction("Login", "ControleAcesso");
        }

        public ActionResult Cancelar()
        {
            return RedirectToAction("Login", "ControleAcesso");
        }

        [HttpGet]
        public ActionResult TrocarSenha()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Reseta senhas
            usuario.USUA_NM_NOVA_SENHA = null;
            usuario.USUA_NM_SENHA_CONFIRMA = null;
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usuario);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                ViewBag.Message = ERP_Condominio_Resource.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture);
                Session["UserCredentials"] = null;
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaInicio()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Reseta senhas
            usuario.USUA_NM_NOVA_SENHA = null;
            usuario.USUA_NM_SENHA_CONFIRMA = null;
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usuario);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenhaInicio(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                ViewBag.Message = ERP_Condominio_Resource.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture);
                Session["UserCredentials"] = null;
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult GerarSenha()
        {
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GerarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                // Processa
                Session["UserCredentials"] = null;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.GenerateNewPassword(item.USUA_EM_EMAIL);
                if (volta == 1)
                {
                    return Json(ERP_Condominio_Resource.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(ERP_Condominio_Resource.ResourceManager.GetString("M0096", CultureInfo.CurrentCulture));
                }
                if (volta == 3)
                {
                    return Json(ERP_Condominio_Resource.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                }
                if (volta == 4)
                {
                    return Json(ERP_Condominio_Resource.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                }
                return Json(1);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Json(ex.Message);
            }
        }
    }
}