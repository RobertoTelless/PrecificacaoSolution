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
using System.Threading.Tasks;

namespace ERP_Condominios_Solution.Controllers
{
    public class FichaTecnicaController : Controller
    {
        private readonly IFichaTecnicaAppService ftApp;
        private readonly IProdutoAppService prodApp;
        private readonly ICategoriaProdutoAppService cpApp;

        private String msg;
        private Exception exception;
        FICHA_TECNICA objetoFt = new FICHA_TECNICA();
        FICHA_TECNICA objetoFtAntes = new FICHA_TECNICA();
        List<FICHA_TECNICA> listaMasterFt = new List<FICHA_TECNICA>();
        PRODUTO objetoMat = new PRODUTO();
        PRODUTO objetoMatAntes = new PRODUTO();
        List<PRODUTO> listaMasterMat = new List<PRODUTO>();
        String extensao = String.Empty;

        public FichaTecnicaController(IFichaTecnicaAppService ftApps, IProdutoAppService prodApps, ICategoriaProdutoAppService cpApps)
        {
            ftApp = ftApps;
            prodApp = prodApps;
            cpApp = cpApps;
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

        public JsonResult GetUnidadeInsumo(Int32 id)
        {
            var result = new Hashtable();
            result.Add("unidade", prodApp.GetById(id).UNIDADE.UNID_NM_NOME);

            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaFT()
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
            Session["Produto"] = null;

            // Carrega listas
            if (Session["ListaFT"] == null)
            {
                listaMasterFt = ftApp.GetAllItens(idAss);
                Session["ListaFT"] = listaMasterFt;
            }
            ViewBag.Listas = (List<FICHA_TECNICA>)Session["ListaFT"];
            ViewBag.Title = "Fichas Técnicas";
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 1).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Categorias = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            Session["VoltaComposto"] = 0;

            // Indicadores
            ViewBag.FT = ((List<FICHA_TECNICA>)Session["ListaFT"]).Count;
            if (Session["MensFT"] != null)
            {
                if ((Int32)Session["MensFT"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0120", CultureInfo.CurrentCulture));
                    Session["MensFT"] = 0;
                }
                if ((Int32)Session["MensFT"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    Session["MensFT"] = 0;
                }
            }

            // Abre view
            objetoFt = new FICHA_TECNICA();
            return View(objetoFt);
        }

        [HttpGet]
        public ActionResult MontarTelaFTProd()
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
            if (Session["ListaFT"] == null)
            {
                listaMasterFt = ftApp.GetAllItens(idAss);
                Session["ListaFT"] = listaMasterFt;
            }
            ViewBag.Listas = (List<FICHA_TECNICA>)Session["ListaFT"];
            ViewBag.Title = "Fichas Técnicas";
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 1).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");

            // Indicadores
            ViewBag.FT = ((List<FICHA_TECNICA>)Session["ListaFT"]).Count;

            // Abre view
            objetoFt = new FICHA_TECNICA();
            return View(objetoFt);
        }


        public ActionResult RetirarFiltroFT()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaFT"] = null;
            Session["FiltroFT"] = null;
            return RedirectToAction("MontarTelaFT");
        }

        public ActionResult MostrarTudoFT()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFt = ftApp.GetAllItensAdm(idAss);
            Session["ListaFT"] = listaMasterFt;
            Session["FiltroFT"] = null;
            return RedirectToAction("MontarTelaFT");
        }

        [HttpPost]
        public ActionResult FiltrarFT(FICHA_TECNICA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<FICHA_TECNICA> listaObj = new List<FICHA_TECNICA>();
                Session["FiltroFT"] = item;
                Int32 volta = ftApp.ExecuteFilter(item.PROD_CD_ID, item.PRODUTO.CAPR_CD_ID, item.FITE_DS_DESCRICAO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensFT"] = 1;
                }

                // Sucesso
                listaMasterFt = listaObj;
                Session["ListaFT"] = listaObj;
                return RedirectToAction("MontarTelaFT");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaFT");
            }
        }

        public ActionResult VoltarBaseFT()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            Session["ListaFTd"] = null;

            if (Session["Produto"] != null)
            {
                return RedirectToAction("EditarProduto", "Produto", new { id = (Int32)Session["IdProduto"]});
            }
            return RedirectToAction("MontarTelaFT");
        }

        public ActionResult VoltarBaseFTDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaFTd"] = null;

            if ((Int32)Session["VoltaFTDash"] == 5)
            {
                return RedirectToAction("MontarTelaDashboardEstoque", "Estoque");
            }
            return RedirectToAction("MontarTelaFT");
        }

        [HttpPost]
        public void MontaListaInsumos(FICHA_TECNICA_DETALHE item)
        {
            if (Session["ListaFTd"] == null)
            {
                Session["ListaFTd"] = new List<FICHA_TECNICA_DETALHE>();
            }
            List<FICHA_TECNICA_DETALHE> lista = (List<FICHA_TECNICA_DETALHE>)Session["ListaFTd"];
            lista.Add(item);
            Session["ListaFTd"] = lista; 
        }

        [HttpPost]
        public void RemoveInsumoTabela(Int32 id)
        {
            if (Session["ListaFTd"] != null)
            {
                List<FICHA_TECNICA_DETALHE> lista = (List<FICHA_TECNICA_DETALHE>)Session["ListaFTd"];
                lista.RemoveAll(x => x.PROD_CD_ID == id);
                Session["ListaFTd"] = lista;
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult IncluirFT()
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

            // Prepara listas
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 1).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Insumos = new SelectList(prodApp.GetAllItens(idAss).Where(p => p.PROD_IN_TIPO_PRODUTO == 2).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");

            PRODUTO prod = new PRODUTO();
            if (Session["IdProduto"] != null)
            {
                prod = prodApp.GetItemById((Int32)Session["IdProduto"]);
            }

            // Prepara view
            Session["IdProduto"] = prod.PROD_CD_ID;
            Session["ListaFtd"] = null;
            FICHA_TECNICA item = new FICHA_TECNICA();
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.FITE_DT_CADASTRO = DateTime.Today.Date;
            vm.FITE_IN_ATIVO = 1;
            if (prod.PROD_CD_ID != 0)
            {
                vm.PROD_CD_ID = prod.PROD_CD_ID;
            }
            vm.FITE_NM_NOME = prod.PROD_NM_NOME;  
            return View(vm);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult IncluirFT(FichaTecnicaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 1).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Insumos = new SelectList(prodApp.GetAllItens(idAss).Where(p => p.PROD_IN_TIPO_PRODUTO == 2).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {

                try
                {
                    // Executa a operação
                    FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ftApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensFT"] = 3;
                        return RedirectToAction("MontarTelaFT");
                    }

                    // Carrega foto e processa alteracao
                    item.FITE_AQ_FOTO_APRESENTACAO = "~/Imagens/Base/icone_imagem.jpg";
                    volta = ftApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/1/FichaTecnica/" + item.FITE_CD_ID.ToString() + "/Apresentacao/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVolta"] = item.FITE_CD_ID;
                    if (Session["FileQueueFT"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueFT"];

                        foreach (var file in fq)
                        {
                            UploadFotoQueueFT(file);
                        }

                        Session["FileQueueFT"] = null;
                    }

                    List<FICHA_TECNICA_DETALHE> lista = (List<FICHA_TECNICA_DETALHE>)Session["ListaFtd"];
                    foreach (var i in (List<FICHA_TECNICA_DETALHE>)Session["ListaFtd"])
                    {
                        var ftd = new FICHA_TECNICA_DETALHE
                        {
                            FITD_CD_ID = i.FITD_CD_ID,
                            FITE_CD_ID = item.FITE_CD_ID,
                            PROD_CD_ID = i.PROD_CD_ID,
                            FITD_QN_QUANTIDADE = i.FITD_QN_QUANTIDADE,
                            FITD_PC_PERDA = i.PRODUTO.PROD_VL_FATOR_CORRECAO,
                            FITD_QN_QUANTIDADE_BRUTA = (i.FITD_QN_QUANTIDADE * ((i.FITD_PC_PERDA / 100) + 1)),
                            FITD_VL_CUSTO_UNITARIO = CalculoCustoUnitario(i.PROD_CD_ID, (i.FITD_QN_QUANTIDADE.Value * ((i.PRODUTO.PROD_VL_FATOR_CORRECAO.Value / 100) + 1))),
                            FITD_IN_ATIVO = 1
                        };
                        IncluirTabelaInsumoFT(ftd);
                    }

                    // Calcula CMVs
                    FICHA_TECNICA fite = ftApp.GetItemById(item.FITE_CD_ID);
                    List<FICHA_TECNICA_DETALHE> fitd = fite.FICHA_TECNICA_DETALHE.ToList();
                    Decimal cmvReceita = fitd.Sum(p => p.FITD_VL_CUSTO_UNITARIO.Value);
                    Decimal cmvUnitario = cmvReceita / fite.FITE_NR_PORCOES.Value;
                    Decimal cmvPeso = cmvReceita / fite.FITE_NR_PESO.Value;
                    Int32 volta1 = ftApp.ValidateEdit(fite, fite, usuarioLogado);
                    Session["ListaFtd"] = null;

                    // Sucesso
                    if ((Int32)Session["VoltaFTDash"] == 10)
                    {
                        return RedirectToAction("EditarProduto", "Produto", new { id = (Int32)Session["IdProduto"] });
                    }
                    if ((Int32)Session["VoltaComposto"] == 0)
                    {
                        listaMasterFt = new List<FICHA_TECNICA>();
                        Session["ListaFt"] = null;
                        return RedirectToAction("MontarTelaFT");
                    }
                    else
                    {
                        Session["ListaProduto"] = null;
                        return RedirectToAction("MontarTelaProduto", "Produto");
                    }
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

        public Decimal CalculoCustoUnitario(Int32 idProd, Decimal idFite)
        {
            PRODUTO prod = prodApp.GetItemById(idProd);
            Decimal valor = 0;
            if (prod.PROD_VL_ULTIMO_CUSTO != null & idFite != null)
            {
                if (prod.PROD_VL_ULTIMO_CUSTO != 0 & idFite != 0)
                {
                    valor = prod.PROD_VL_ULTIMO_CUSTO.Value / idFite;
                }
                else
                {
                    valor = 0;
                }
                return valor;
            }
            else
            {
                return 0;
            }
        }

        [HttpGet]
        public ActionResult EditarInsumoFT(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Insumos = new SelectList(prodApp.GetAllItens(idAss).Where(p => p.PROD_IN_TIPO_PRODUTO == 2).OrderBy(p => p.PROD_NM_NOME), "PROD_CD_ID", "PROD_NM_NOME");
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            FICHA_TECNICA_DETALHE item = ftApp.GetDetalheById(id);
            FichaTecnicaDetalheViewModel vm = Mapper.Map<FICHA_TECNICA_DETALHE, FichaTecnicaDetalheViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarInsumoFT(FichaTecnicaDetalheViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Insumos = new SelectList(prodApp.GetAllItens(idAss).Where(p => p.PROD_IN_TIPO_PRODUTO == 2).OrderBy(p => p.PROD_NM_NOME), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FICHA_TECNICA_DETALHE item = Mapper.Map<FichaTecnicaDetalheViewModel, FICHA_TECNICA_DETALHE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ftApp.ValidateEditInsumo(item);

                    // Calcula CMVs
                    FICHA_TECNICA fite = ftApp.GetItemById(item.FITE_CD_ID);
                    List<FICHA_TECNICA_DETALHE> fitd = fite.FICHA_TECNICA_DETALHE.ToList();
                    Decimal cmvReceita = fitd.Sum(p => p.FITD_VL_CUSTO_UNITARIO.Value);
                    Decimal cmvUnitario = cmvReceita / fite.FITE_NR_PORCOES.Value;
                    Decimal cmvPeso = cmvReceita / fite.FITE_NR_PESO.Value;
                    Int32 volta1 = ftApp.ValidateEdit(fite, fite, usuarioLogado);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoFT");
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
        public ActionResult EditarFT(Int32 id)
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
            
            // Prepara view
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            FICHA_TECNICA item = ftApp.GetItemById(id);
            objetoFtAntes = item;
            Session["VoltaFT"] = 1;
            Session["FichaTecnica"] = item;
            Session["IdVolta"] = id;
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarFT(FichaTecnicaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                    Int32 volta = ftApp.ValidateEdit(item, objetoFtAntes, usuarioLogado);

                    // Verifica retorno

                    // Calcula CMVs
                    FICHA_TECNICA fite = ftApp.GetItemById(item.FITE_CD_ID);
                    List<FICHA_TECNICA_DETALHE> fitd = fite.FICHA_TECNICA_DETALHE.ToList();
                    Decimal cmvReceita = fitd.Sum(p => p.FITD_VL_CUSTO_UNITARIO.Value);
                    Decimal cmvUnitario = cmvReceita / fite.FITE_NR_PORCOES.Value;
                    Decimal cmvPeso = cmvReceita / fite.FITE_NR_PESO.Value;
                    Int32 volta1 = ftApp.ValidateEdit(fite, fite, usuarioLogado);

                    // Sucesso
                    listaMasterFt = new List<FICHA_TECNICA>();
                    Session["ListaFT"] = null;

                    if (Session["Produto"] != null)
                    {
                        return RedirectToAction("EditarProduto", "Produto", new { id = ((PRODUTO)Session["Produto"]).PROD_CD_ID });
                    }

                    if ((Int32)Session["VoltaFTDash"] == 10)
                    {
                        return RedirectToAction("EditarProduto", new { id = (Int32)Session["IdProduto"] });
                    }
                    return RedirectToAction("MontarTelaFT");
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
        public ActionResult VisualizarEditarFTProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            FICHA_TECNICA item = ftApp.GetItemById(prodApp.GetItemById((Int32)Session["IdVolta"]).FICHA_TECNICA.FirstOrDefault().FITE_CD_ID);
            objetoFtAntes = item;
            Session["idFicha"] = item.FITE_CD_ID;
            Session["FichaTecnica"] = item;
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisualizarEditarFTProduto(FichaTecnicaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                    Int32 volta = ftApp.ValidateEdit(item, objetoFtAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFt = new List<FICHA_TECNICA>();
                    Session["ListaFT"] = null;
                    return RedirectToAction("VoltarAnexoProduto", "Produto");
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
        public ActionResult ExcluirFT(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensFT"] = 2;
                    return RedirectToAction("MontarTelaFT");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            FICHA_TECNICA item = ftApp.GetItemById(id);
            Int32 volta = ftApp.ValidateDelete(item, usuario);

            // Verifica retorno

            // Calcula CMVs
            FICHA_TECNICA fite = ftApp.GetItemById(item.FITE_CD_ID);
            List<FICHA_TECNICA_DETALHE> fitd = fite.FICHA_TECNICA_DETALHE.ToList();
            Decimal cmvReceita = fitd.Sum(p => p.FITD_VL_CUSTO_UNITARIO.Value);
            Decimal cmvUnitario = cmvReceita / fite.FITE_NR_PORCOES.Value;
            Decimal cmvPeso = cmvReceita / fite.FITE_NR_PESO.Value;
            Int32 volta1 = ftApp.ValidateEdit(fite, fite, usuario);

            // Sucesso
            listaMasterFt = new List<FICHA_TECNICA>();
            Session["ListaFT"] = null;
            return RedirectToAction("MontarTelaFT");
        }

        [HttpGet]
        public ActionResult ReativarFT(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensFT"] = 2;
                    return RedirectToAction("MontarTelaFT");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            FICHA_TECNICA item = ftApp.GetItemById(id);
            Int32 volta = ftApp.ValidateReativar(item, usuario);

            // Calcula CMVs
            FICHA_TECNICA fite = ftApp.GetItemById(item.FITE_CD_ID);
            List<FICHA_TECNICA_DETALHE> fitd = fite.FICHA_TECNICA_DETALHE.ToList();
            Decimal cmvReceita = fitd.Sum(p => p.FITD_VL_CUSTO_UNITARIO.Value);
            Decimal cmvUnitario = cmvReceita / fite.FITE_NR_PORCOES.Value;
            Decimal cmvPeso = cmvReceita / fite.FITE_NR_PESO.Value;
            Int32 volta1 = ftApp.ValidateEdit(fite, fite, usuario);

            // Sucesso
            listaMasterFt = new List<FICHA_TECNICA>();
            Session["ListaFT"] = null;
            return RedirectToAction("MontarTelaFT");
        }

        [HttpGet]
        public ActionResult IncluirInsumoFT()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Insumos = new SelectList(prodApp.GetAllItens(idAss).Where(p => p.PROD_IN_TIPO_PRODUTO == 2).OrderBy(p => p.PROD_NM_NOME), "PROD_CD_ID", "PROD_NM_NOME");
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            FICHA_TECNICA_DETALHE item = new FICHA_TECNICA_DETALHE();
            FichaTecnicaDetalheViewModel vm = Mapper.Map<FICHA_TECNICA_DETALHE, FichaTecnicaDetalheViewModel>(item);
            vm.FITE_CD_ID = (Int32)Session["IdVolta"];
            vm.FITD_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirInsumoFT(FichaTecnicaDetalheViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Insumos = new SelectList(prodApp.GetAllItens(idAss).Where(p => p.PROD_IN_TIPO_PRODUTO == 2).OrderBy(p => p.PROD_NM_NOME), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FICHA_TECNICA_DETALHE item = Mapper.Map<FichaTecnicaDetalheViewModel, FICHA_TECNICA_DETALHE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ftApp.ValidateCreateInsumo(item);

                    // Calcula CMVs
                    FICHA_TECNICA fite = ftApp.GetItemById(item.FITE_CD_ID);
                    List<FICHA_TECNICA_DETALHE> fitd = fite.FICHA_TECNICA_DETALHE.ToList();
                    Decimal cmvReceita = fitd.Sum(p => p.FITD_VL_CUSTO_UNITARIO.Value);
                    Decimal cmvUnitario = cmvReceita / fite.FITE_NR_PORCOES.Value;
                    Decimal cmvPeso = cmvReceita / fite.FITE_NR_PESO.Value;
                    Int32 volta1 = ftApp.ValidateEdit(fite, fite, usuarioLogado);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoFT");
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
        public void IncluirTabelaInsumoFT(FICHA_TECNICA_DETALHE item)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];                
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Int32 volta = ftApp.ValidateCreateInsumo(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
        }

        [HttpGet]
        public ActionResult ExcluirInsumoFT(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

            FICHA_TECNICA_DETALHE item = ftApp.GetDetalheById(id);
            objetoFtAntes = (FICHA_TECNICA)Session["FichaTecnica"];
            item.FITD_IN_ATIVO = 0;
            Int32 volta = ftApp.ValidateEditInsumo(item);

            // Calcula CMVs
            FICHA_TECNICA fite = ftApp.GetItemById(item.FITE_CD_ID);
            List<FICHA_TECNICA_DETALHE> fitd = fite.FICHA_TECNICA_DETALHE.ToList();
            Decimal cmvReceita = fitd.Sum(p => p.FITD_VL_CUSTO_UNITARIO.Value);
            Decimal cmvUnitario = cmvReceita / fite.FITE_NR_PORCOES.Value;
            Decimal cmvPeso = cmvReceita / fite.FITE_NR_PESO.Value;
            Int32 volta1 = ftApp.ValidateEdit(fite, fite, usuarioLogado);

            return RedirectToAction("VoltarAnexoFT");
        }

        [HttpGet]
        public ActionResult ReativarInsumoFT(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

            FICHA_TECNICA_DETALHE item = ftApp.GetDetalheById(id);
            objetoFtAntes = (FICHA_TECNICA)Session["FichaTecnica"];
            item.FITD_IN_ATIVO = 1;
            Int32 volta = ftApp.ValidateEditInsumo(item);

            // Calcula CMVs
            FICHA_TECNICA fite = ftApp.GetItemById(item.FITE_CD_ID);
            List<FICHA_TECNICA_DETALHE> fitd = fite.FICHA_TECNICA_DETALHE.ToList();
            Decimal cmvReceita = fitd.Sum(p => p.FITD_VL_CUSTO_UNITARIO.Value);
            Decimal cmvUnitario = cmvReceita / fite.FITE_NR_PORCOES.Value;
            Decimal cmvPeso = cmvReceita / fite.FITE_NR_PESO.Value;
            Int32 volta1 = ftApp.ValidateEdit(fite, fite, usuarioLogado);

            return RedirectToAction("VoltarAnexoFT");
        }

        [HttpGet]
        public ActionResult CriarFichaTecnicaProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).Where(p => p.PROD_IN_TIPO_PRODUTO == 1).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");

            // Prepara view
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            FICHA_TECNICA item = new FICHA_TECNICA();
            FichaTecnicaViewModel vm = Mapper.Map<FICHA_TECNICA, FichaTecnicaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.FITE_DT_CADASTRO = DateTime.Today.Date;
            vm.FITE_IN_ATIVO = 1;
            vm.PROD_CD_ID = (Int32)Session["IdVolta"];
            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CriarFichaTecnicaProduto(FichaTecnicaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).Where(p => p.PROD_IN_TIPO_PRODUTO == 1).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FICHA_TECNICA item = Mapper.Map<FichaTecnicaViewModel, FICHA_TECNICA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ftApp.ValidateCreateProduto(item, item.PRODUTO, usuarioLogado);

                    // Verifica retorno

                    // Carrega foto e processa alteracao
                    item.FITE_AQ_FOTO_APRESENTACAO = "~/Imagens/Base/icone_imagem.jpg";
                    volta = ftApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/1/FichaTecnica/" + item.FITE_CD_ID.ToString() + "/Apresentacao/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMasterFt = new List<FICHA_TECNICA>();
                    Session["ListaFT"] = null;
                    return RedirectToAction("VoltarAnexoProduto", "Produto");
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

        public ActionResult VoltarAnexoFT()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("EditarFT", new { id = (Int32)Session["IdVolta"] });
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files)
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
                queue.Add(f);
            }
            Session["FileQueueFT"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFotoQueueFT(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            string random = CrossCutting.RandomStringGenerator.RandomString(10);
            if (file == null)
            {
                Session["MensFT"] = 4;
                return RedirectToAction("VoltarAnexoFT");
            }

            FICHA_TECNICA item = ftApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = random + file.Name;
            if (fileName.Length > 250)
            {
                Session["MensFT"] = 5;
                return RedirectToAction("VoltarAnexoFT");
            }
            String caminho = "/Imagens/1/FichaTecnica/" + item.FITE_CD_ID.ToString() + "/Apresentacao/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.FITE_AQ_FOTO_APRESENTACAO = "~" + caminho + fileName;
            objetoFtAntes = item;
            Int32 volta = ftApp.ValidateEdit(item, objetoFtAntes);
            return RedirectToAction("VoltarAnexoFT");
        }

        [HttpPost]
        public ActionResult UploadFotoFT(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            string random = CrossCutting.RandomStringGenerator.RandomString(10);

            if (file == null)
            {
                Session["MensFT"] = 4;
                return RedirectToAction("VoltarAnexoFT");
            }

            FICHA_TECNICA item = ftApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = random + Path.GetExtension(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensFT"] = 5;
                return RedirectToAction("VoltarAnexoFT");
            }
            String caminho = "/Imagens/1/FichaTecnica/" + item.FITE_CD_ID.ToString() + "/Apresentacao/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.FITE_AQ_FOTO_APRESENTACAO = "~" + caminho + fileName;
            objetoFtAntes = item;
            Int32 volta = ftApp.ValidateEdit(item, objetoFtAntes);
            return RedirectToAction("VoltarAnexoFT");
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "FTLista" + "_" + data + ".pdf";
            List<FICHA_TECNICA> lista = (List<FICHA_TECNICA>)Session["ListaFT"];
            FICHA_TECNICA filtro = (FICHA_TECNICA)Session["FiltroFT"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

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

            cell = new PdfPCell(new Paragraph("Fichas Técnicas - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 100f, 120f, 120f, 70f, 70f, 90f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Fichas Técnicas selecionadas pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 6;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Produto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome Da Composição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Porções", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Peso (Kg)", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Foto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (FICHA_TECNICA item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.PRODUTO.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.FITE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.FITE_NR_PORCOES.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.FITE_NR_PESO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.FITE_AQ_FOTO_APRESENTACAO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.FITE_AQ_FOTO_APRESENTACAO));
                    image.ScaleAbsolute(40, 40);
                    cell.AddElement(image);
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
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
                if (filtro.PROD_CD_ID > 0)
                {
                    PRODUTO pro = prodApp.GetItemById(filtro.PROD_CD_ID);
                    parametros += "Produto: " + pro.PROD_NM_NOME;
                    ja = 1;
                }
                if (filtro.FITE_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.FITE_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros +=  " e Nome: " + filtro.FITE_NM_NOME;
                    }
                }
                if (filtro.FITE_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.FITE_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.FITE_DS_DESCRICAO;
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

            return RedirectToAction("MontarTelaFT");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            
            // Prepara geração
            FICHA_TECNICA aten = ftApp.GetItemById((Int32)Session["IdVolta"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "FichaTecnica_" + aten.FITE_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

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
            Image image = Image.GetInstance(Server.MapPath("~/Images/Precificacao_Favicon.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Ficha Técnica - Detalhes", meuFont2))
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

            // Dados Gerais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (System.IO.File.Exists(Server.MapPath(aten.FITE_AQ_FOTO_APRESENTACAO)))
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                image = Image.GetInstance(Server.MapPath(aten.FITE_AQ_FOTO_APRESENTACAO));
                image.ScaleAbsolute(150, 150);
                cell.AddElement(image);
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(" " , meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Produto: " + aten.PRODUTO.PROD_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome da Composição: " + aten.FITE_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Num.Porções: " + aten.FITE_NR_PORCOES.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Peso (Kg): " + aten.FITE_NR_PESO.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Fator Correção: " + aten.FITE_NR_FATOR_CORRECAO.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quant.Líquida: " + aten.FITE_QN_QUANTIDADE_LIQUIDA.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Perdas (%): " + aten.FITE_PC_PERCENTUAL_PERDA.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Instruções de Montagem: ", meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            var pattern = @"<(img|a)[^>]*>(?<content>[^<]*)<";
            var regex = new Regex(pattern);
            var m = regex.Match(aten.FITE_DS_DESCRICAO);
            if (m.Success)
            {
                aten.FITE_DS_DESCRICAO = m.Groups["content"].Value;
            }
            cell = new PdfPCell(new Paragraph(Regex.Replace(aten.FITE_DS_DESCRICAO, @"<.*?>", ""), meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Apresentação: ", meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(Regex.Replace(aten.FITE_DS_APRESENTACAO, @"<.*?>", ""), meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);


            // Lista de Insumos
            if (aten.FICHA_TECNICA_DETALHE.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 70f, 70f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Insumos", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Insumo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Unidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (FICHA_TECNICA_DETALHE item in aten.FICHA_TECNICA_DETALHE)
                {
                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PRODUTO.UNIDADE.UNID_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FITD_QN_QUANTIDADE.ToString(), meuFont))
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

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("VoltarAnexoFT");
        }

        public JsonResult GetProduto(Int32 id)
        {
            PRODUTO forn = prodApp.GetItemById(id);
            String nome = "Preparação de " + forn.PROD_NM_NOME;

            var hash = new Hashtable();
            hash.Add("nome", nome);
            return Json(hash);
        }

    }
}