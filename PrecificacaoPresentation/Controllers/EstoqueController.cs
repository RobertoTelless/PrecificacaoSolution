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
    public class EstoqueController : Controller
    {
        private readonly IProdutoAppService prodApp;
        private readonly ILogAppService logApp;
        private readonly IUnidadeAppService unApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly ISubcategoriaProdutoAppService scpApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IMovimentoEstoqueProdutoAppService moepApp;
        private readonly IFornecedorAppService fornApp;

        private String msg;
        private Exception exception;
        PRODUTO objetoProd = new PRODUTO();
        PRODUTO objetoProdAntes = new PRODUTO();
        List<PRODUTO> listaMasterProd = new List<PRODUTO>();
        String extensao;


        public EstoqueController(
            IProdutoAppService prodApps
            , ILogAppService logApps
            , IUnidadeAppService unApps
            , ICategoriaProdutoAppService cpApps
            , ISubcategoriaProdutoAppService scpApps
            , IMovimentoEstoqueProdutoAppService moepApps
            , IFornecedorAppService fornApps
            , IConfiguracaoAppService confApps)
        {
            prodApp = prodApps;
            logApp = logApps;
            unApp = unApps;
            cpApp = cpApps;
            scpApp = scpApps;
            confApp = confApps;
            moepApp = moepApps;
            fornApp = fornApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            PRODUTO item = new PRODUTO();
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaProduto"] == 40)
            {
                Session["VoltaProduto"] = 1;
                return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaDashboardEstoque", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult FiltrarSubCategoriaProduto(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaSubFiltrada = prodApp.GetAllSubs(idAss);

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaSubFiltrada = prodApp.GetAllSubs(idAss).Where(x => x.CAPR_CD_ID == id).ToList();
            }
            return Json(listaSubFiltrada.Select(x => new { value = x.SCPR_CD_ID, text = x.SCPR_NM_NOME }));
        }

        [HttpPost]
        public JsonResult FiltrarCategoriaProduto(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaFiltrada = cpApp.GetAllItens(idAss);

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaFiltrada = listaFiltrada.Where(x => x.SUBCATEGORIA_PRODUTO.Any(s => s.SCPR_CD_ID == id)).ToList();
            }

            return Json(listaFiltrada.Select(x => new { value = x.CAPR_CD_ID, text = x.CAPR_NM_NOME }));
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardEstoque()
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
                    Session["MensEstoque"] = 2;
                    return RedirectToAction("MontarTelaEstoqueProduto", "Estoque");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera listas e contagem
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaTotal = moepApp.GetAllItens(idAss);
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaTotalEntrada = listaTotal.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 1).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaTotalSaida= listaTotal.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 2).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMes = listaTotal.Where(p => p.MOEP_DT_MOVIMENTO.Month == DateTime.Today.Date.Month & p.MOEP_DT_MOVIMENTO.Year == DateTime.Today.Date.Year).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesEntrada = listaMes.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 1).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesSaida = listaMes.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 2).ToList();

            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesEntradaAvulsa = listaMesEntrada.Where(p => p.MOEP_IN_OPERACAO == 1).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesEntradaDevol = listaMesEntrada.Where(p => p.MOEP_IN_OPERACAO == 2).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesEntradaAcerto = listaMesEntrada.Where(p => p.MOEP_IN_OPERACAO == 3).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesEntradaOutra = listaMesEntrada.Where(p => p.MOEP_IN_OPERACAO == 4).ToList();

            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesSaidaAvulsa = listaMesSaida.Where(p => p.MOEP_IN_OPERACO_SAIDA == 1).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesSaidaDevol = listaMesSaida.Where(p => p.MOEP_IN_OPERACO_SAIDA == 2).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesSaidaPerda = listaMesSaida.Where(p => p.MOEP_IN_OPERACO_SAIDA == 3).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesSaidaDescarte = listaMesSaida.Where(p => p.MOEP_IN_OPERACO_SAIDA == 4).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesSaidaAcerto = listaMesSaida.Where(p => p.MOEP_IN_OPERACO_SAIDA == 5).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesSaidaOutra = listaMesSaida.Where(p => p.MOEP_IN_OPERACO_SAIDA == 6).ToList();

            // Produtos
            List<PRODUTO> prodTotal = prodApp.GetAllItens(idAss);
            Int32 prodTotalNum = prodTotal.Count;
            Int32 prods = prodTotal.Where(p => p.PROD_IN_TIPO_PRODUTO == 1).ToList().Count;
            Int32 ins = prodTotal.Where(p => p.PROD_IN_TIPO_PRODUTO == 2).ToList().Count;
            ViewBag.ProdTotal = prodTotalNum;
            ViewBag.Prods = prods;
            ViewBag.Ins = ins;

            List<PRODUTO> pontoPedido = prodTotal.Where(x => x.PROD_QN_ESTOQUE < x.PROD_QN_QUANTIDADE_MINIMA).ToList();
            List<PRODUTO> estoqueZerado = prodTotal.Where(x => x.PROD_QN_ESTOQUE == 0).ToList();
            List<PRODUTO> estoqueNegativo = prodTotal.Where(x => x.PROD_QN_ESTOQUE < 0).ToList();

            ViewBag.PontoPedido = pontoPedido.Count;
            ViewBag.EstoqueZerado = estoqueZerado.Count;
            ViewBag.EstoqueNegativo = estoqueNegativo.Count;

            // Prepara View 
            ViewBag.Total = listaTotal;
            ViewBag.TotalSum = listaTotal.Count;
            ViewBag.TotalEntrada = listaTotalEntrada;
            ViewBag.TotalEntradaSum = listaTotalEntrada.Count;
            ViewBag.TotalSaida = listaTotalSaida;
            ViewBag.TotalSaidaSum = listaTotalSaida.Count;
            
            ViewBag.Mes = listaMes;
            ViewBag.MesSum = listaMes.Count;
            ViewBag.MesEntrada = listaMesEntrada;
            ViewBag.MesEntradaSum = (listaMesEntrada.Count);
            ViewBag.MesSaida = listaMesSaida;
            ViewBag.MesSaidaSum = (listaMesSaida.Count);

            // Resumo Mes 
            List<DateTime> datas = listaMes.Select(p => p.MOEP_DT_MOVIMENTO.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaMes.Where(p => p.MOEP_DT_MOVIMENTO.Date == item).Count();
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.Valor = conta;
                lista.Add(mod1);
            }
            ViewBag.ListaMov = lista;
            ViewBag.ListaMovSum = lista.Count;
            Session["ListaMes"] = listaMes;
            Session["ListaDatas"] = datas;
            Session["ListaMovResumo"] = lista;
            Session["Entradas"] = listaMesEntrada.Count;
            Session["EntradasAvulsa"] = listaMesEntradaAvulsa.Count;
            Session["EntradasDevol"] = listaMesEntradaDevol.Count;
            Session["EntradasAcerto"] = listaMesEntradaAcerto.Count;
            Session["EntradasOutra"] = listaMesEntradaOutra.Count;

            Session["Saidas"] = (listaMesSaida.Count);
            Session["SaidasAvulsa"] = listaMesSaidaAvulsa.Count;
            Session["SaidasDevol"] = listaMesSaidaDevol.Count;
            Session["SaidasAcerto"] = listaMesSaidaAcerto.Count;
            Session["SaidasPerdas"] = listaMesSaidaPerda.Count;
            Session["SaidasDescarte"] = listaMesSaidaDescarte.Count;
            Session["SaidasOutra"] = listaMesSaidaOutra.Count;

            // Resumo Tipo  
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            ModeloViewModel mod = new ModeloViewModel();
            mod.Data = "Entradas";
            mod.Valor = listaMesEntrada.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Entradas Avulsas";
            mod.Valor = listaMesEntradaAvulsa.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Devolução Cliente";
            mod.Valor = listaMesEntradaDevol.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Entradas - Acerto";
            mod.Valor = listaMesEntradaAcerto.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Outras Entradas";
            mod.Valor = listaMesEntradaOutra.Count;
            lista1.Add(mod);


            mod = new ModeloViewModel();
            mod.Data = "Saídas";
            mod.Valor = listaMesSaida.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Saídas Avulsas";
            mod.Valor = listaMesSaidaAvulsa.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Devolução Fornecedor";
            mod.Valor = listaMesSaidaDevol.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Saídas - Acerto";
            mod.Valor = listaMesSaidaAcerto.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Perdas";
            mod.Valor = listaMesSaidaPerda.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Descartes";
            mod.Valor = listaMesSaidaDescarte.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Outras Saídas";
            mod.Valor = listaMesSaidaOutra.Count;
            lista1.Add(mod);

            ViewBag.ListaSituacao = lista1;
            Session["ListaSituacao"] = lista1;
            Session["VoltaDash"] = 3;
            Session["VoltaProdutoDash"] = 5;
            Session["VoltaFTDash"] = 5;
            Session["VoltaEstoqueCRM"] = 0;
            return View(vm);
        }

        public JsonResult GetDadosGraficoMovimentosTipo()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["EntradasAvulsa"];
            Int32 q2 = (Int32)Session["EntradasDevol"];
            Int32 q3 = (Int32)Session["EntradasAcerto"];
            Int32 q4 = (Int32)Session["EntradasOutra"];

            Int32 q5 = (Int32)Session["SaidasAvulsa"];
            Int32 q6 = (Int32)Session["SaidasDevol"];
            Int32 q7 = (Int32)Session["SaidasAcerto"];
            Int32 q8 = (Int32)Session["SaidasPerdas"];
            Int32 q9 = (Int32)Session["SaidasDescarte"];
            Int32 q10 = (Int32)Session["SaidasOutra"];


            desc.Add("Entradas Avulsas");
            quant.Add(q1);
            cor.Add("#cd9d6d");
            desc.Add("Devoluções Clientes");
            quant.Add(q2);
            cor.Add("#cdc36d");
            desc.Add("Entradas - Acertos");
            quant.Add(q3);
            cor.Add("#a0cfff");
            desc.Add("Entradas - Outras");
            quant.Add(q4);
            cor.Add("#bda5d4");

            desc.Add("Saídas Avulsas");
            quant.Add(q5);
            cor.Add("#cd9aa3");
            desc.Add("Devoluções Fornecedores");
            quant.Add(q6);
            cor.Add("#cdc9e3");
            desc.Add("Saídas - Acertos");
            quant.Add(q7);
            cor.Add("#a0c112");
            desc.Add("Saídas - Perdas");
            quant.Add(q8);
            cor.Add("#a0b23e");
            desc.Add("Saídas - Descartes");
            quant.Add(q9);
            cor.Add("#a0e25a");
            desc.Add("Saídas - Outras");
            quant.Add(q10);
            cor.Add("#bdb2af");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoMovimentos()
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaCP1 = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatas"];
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaDia = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MOEP_DT_MOVIMENTO.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaEstoqueProduto()
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
            if (Session["ListaProdEstoque"] == null)
            {
                listaMasterProd = prodApp.GetAllItens(idAss);
                Session["ListaProdEstoque"] = listaMasterProd;
            }
            ViewBag.Listas = ((List<PRODUTO>)Session["ListaProdEstoque"]).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>();
            ViewBag.Title = "Estoque";
            ViewBag.Cats = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            List<PRODUTO> prods = (List<PRODUTO>)Session["ListaProdEstoque"];

            // Indicadores
            ViewBag.Produtos = ((List<PRODUTO>)Session["ListaProdEstoque"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> prodIns = new List<SelectListItem>();
            prodIns.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            prodIns.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.ProdutoInsumo = new SelectList(prodIns, "Value", "Text");

            List<PRODUTO> pontoPedido = prods.Where(x => x.PROD_QN_ESTOQUE < x.PROD_QN_QUANTIDADE_MINIMA).ToList();
            List<PRODUTO> estoqueZerado = prods.Where(x => x.PROD_QN_ESTOQUE == 0).ToList();
            List<PRODUTO> estoqueNegativo = prods.Where(x => x.PROD_QN_ESTOQUE < 0).ToList();
            ViewBag.PontoPedido = pontoPedido.Count;
            ViewBag.EstoqueZerado = estoqueZerado.Count;
            ViewBag.EstoqueNegativo= estoqueNegativo.Count;

            // Mansagem
            if ((Int32)Session["MensEstoque"] == 1)
            {
                Session["MensEstoque"] = 0;
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["VoltaEstoque"] = 0;
            Session["MensEstoque"] = 0;
            objetoProd = new PRODUTO();
            listaMasterProd = null;
            Session["FiltroMvmtProd"] = false;
            Session["FiltroMov"] = false;
            return View(objetoProd);
        }

        public ActionResult RetirarFiltroProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaProdEstoque"] = null;
            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        public ActionResult RetirarFiltroMovimentacaoEstoqueProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroMvmtProd"] = false;
            return RedirectToAction("VerMovimentacaoEstoqueProduto", new { id = id });
        }

        public ActionResult RetirarFiltroMovimentacaoEstoqueProdutoNova(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroMvmtProd"] = false;
            return RedirectToAction("VerMovimentacaoEstoqueProduto", new { id = (Int32)Session["IdMovimento"] });
        }

        [HttpPost]
        public ActionResult FiltrarProduto(PRODUTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PRODUTO> listaObj = new List<PRODUTO>();
                Session["FiltroProduto"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 volta = prodApp.ExecuteFilterEstoque(1, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_BC_BARCODE, item.CAPR_CD_ID, item.PROD_IN_TIPO_PRODUTO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensEstoque"] = 1;
                }

                // Sucesso
                listaMasterProd = listaObj;
                Session["ListaProdEstoque"] = listaObj;
                return RedirectToAction("MontarTelaEstoqueProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaEstoqueProduto");
            }
        }

        public ActionResult VoltarBaseProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        public ActionResult VoltarDashEstoque()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaEstoqueCRM"] == 0)
            {
                return RedirectToAction("MontarTelaDashboardEstoque");
            }
            if ((Int32)Session["VoltaEstoqueCRM"] == 1)
            {
                return RedirectToAction("IncluirItemPedido", "CRM");
            }
            return RedirectToAction("MontarTelaDashboardEstoque");
        }

        public ActionResult GerarRelatorioEstoqueProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            var usuario = (USUARIO)Session["UserCredentials"];

            Int32 perfil = 0;
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                perfil = 1;
            }

            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ProdutoEstoqueLista" + "_" + data + ".pdf";
            List<PRODUTO> lista = prodApp.GetAllItens(usuario.ASSI_CD_ID);
            PRODUTO filtro = (PRODUTO)Session["FiltroProduto"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
            Font meuFont4 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.GREEN);

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

            cell = new PdfPCell(new Paragraph("Estoque de Produtos - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 50f, 150f, 60f, 60f, 60f, 50f, 50f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Produtos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);


            cell = new PdfPCell(new Paragraph("Tipo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Codigo de Barra", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Estoque", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Minima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Máxima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Última Movimentação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (PRODUTO item in lista)
            {
                if (item.PROD_IN_TIPO_PRODUTO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Produto", meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Insumo", meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.PROD_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PROD_BC_BARCODE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PROD_QN_ESTOQUE.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PROD_QN_QUANTIDADE_MINIMA.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PROD_QN_QUANTIDADE_MAXIMA.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PROD_DT_ULTIMA_MOVIMENTACAO.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                //if (item.PROD_AQ_FOTO != null)
                //{
                //    Image foto = Image.GetInstance(Server.MapPath(item.PROD_AQ_FOTO));
                //    cell = new PdfPCell(foto, true);
                //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //    table.AddCell(cell);
                //}
                //else
                //{
                //    cell = new PdfPCell(new Paragraph(" - ", meuFont))
                //    {
                //        VerticalAlignment = Element.ALIGN_MIDDLE,
                //        HorizontalAlignment = Element.ALIGN_LEFT
                //    };
                //    table.AddCell(cell);
                //}
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
                if (filtro.PROD_BC_BARCODE != null)
                {
                    parametros += "Código de Barras: " + filtro.PROD_BC_BARCODE;
                    ja = 1;
                }
                if (filtro.PROD_CD_CODIGO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Código: " + filtro.PROD_CD_CODIGO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Código: " + filtro.PROD_CD_CODIGO;
                    }
                }
                if (filtro.CATEGORIA_PRODUTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Categoria: " + filtro.CATEGORIA_PRODUTO.CAPR_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Categoria: " + filtro.CATEGORIA_PRODUTO.CAPR_NM_NOME;
                    }
                }
                if (filtro.PROD_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.PROD_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.PROD_NM_NOME;
                    }
                }
                if (filtro.PROD_NM_MARCA != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Marca: " + filtro.PROD_NM_MARCA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Marca: " + filtro.PROD_NM_MARCA;
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

            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        [HttpGet]
        public ActionResult VerEstoqueProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            Session["VoltaEstoque"] = 1;
            return RedirectToAction("ConsultarProduto", "Produto", new { id = id });

        }

        [HttpGet]
        public ActionResult AcertoManualProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            var usu = (USUARIO)Session["UserCredentials"];
            PRODUTO item = prodApp.GetItemById(id);
            objetoProdAntes = item;
            Session["ProdutoEstoque"] = item;
            Session["IdVolta"] = id;
            item.PROD_QN_QUANTIDADE_ALTERADA = item.PROD_QN_ESTOQUE;
            item.PROD_DS_JUSTIFICATIVA = String.Empty;
            return View(item);
        }

        [HttpPost]
        public ActionResult AcertoManualProduto(PRODUTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (item.PROD_QN_QUANTIDADE_ALTERADA == null)
            {
                ModelState.AddModelError("", "Campo NOVA CONTAGEM não pode ser nulo");
                return View(item);
            }
            if (item.PROD_QN_ESTOQUE == item.PROD_QN_QUANTIDADE_ALTERADA)
            {
                ModelState.AddModelError("", "Campo NOVA CONTAGEM com mesmo valor de ESTOQUE");
                return View(item);
            }
            if (item.PROD_DS_JUSTIFICATIVA == null)
            {
                ModelState.AddModelError("", "Campo JUSTIFICATIVA obrigatorio");
                return View(item);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = prodApp.ValidateEditEstoque(item, objetoProdAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterProd = new List<PRODUTO>();
                    Session["ListaProdEstoque"] = null;
                    return RedirectToAction("MontarTelaEstoqueProduto");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(objetoProdAntes);
                }
            }
            else
            {
                return View(objetoProdAntes);
            }
        }

        public ActionResult VerMovimentacaoEstoqueProdutoNova(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            List<SelectListItem> filtroEntradaSaida = new List<SelectListItem>();
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.FiltroEntradaSaida = new SelectList(filtroEntradaSaida, "Value", "Text");
            USUARIO usu = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PRODUTO pef = prodApp.GetItemById(id);
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = pef.MOVIMENTO_ESTOQUE_PRODUTO.ToList();

            // Filtro
            if ((Boolean)Session["FiltroMvmtProd"])
            {
                Session["FiltroMvmtProd"] = false;
                lista = lista.Where(x => x.MOEP_IN_TIPO_MOVIMENTO == (Int32)Session["EntradaSaida"]).ToList();
                objetoProdAntes = pef;
            }

            Session["ItemMovimento"] = pef;
            Session["IdMovimento"] = id;
            Session["ListaMovimento"] = lista;
            ViewBag.Lista = lista;
            objetoProdAntes = pef;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(pef);
            return View(vm);
        }

        public ActionResult VoltarAnexoProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("VerEstoqueProduto", new { id = (Int32)Session["IdVolta"] });
        }

        public SelectList GetTipoEntrada()
        {
            List<SelectListItem> tipoEntrada = new List<SelectListItem>();
            tipoEntrada.Add(new SelectListItem() { Text = "Entrada Avulsa", Value = "1" }); 
            tipoEntrada.Add(new SelectListItem() { Text = "Devolução de Cliente", Value = "2" });
            tipoEntrada.Add(new SelectListItem() { Text = "Acerto de Estoque", Value = "3" });
            tipoEntrada.Add(new SelectListItem() { Text = "Outras Entradas", Value = "4" });
            return new SelectList(tipoEntrada, "Value", "Text");
        }

        public SelectList GetTipoSaida()
        {
            List<SelectListItem> tipoSaida = new List<SelectListItem>();
            tipoSaida.Add(new SelectListItem() { Text = "Saída Avulsa", Value = "1" });
            tipoSaida.Add(new SelectListItem() { Text = "Devolução para Fornecedor", Value = "2" });
            tipoSaida.Add(new SelectListItem() { Text = "Perda ou Roubo", Value = "3" });
            tipoSaida.Add(new SelectListItem() { Text = "Descarte", Value = "4" });
            tipoSaida.Add(new SelectListItem() { Text = "Acerto de Estoque", Value = "5" });
            tipoSaida.Add(new SelectListItem() { Text = "Outras Saídas", Value = "6" });
            return new SelectList(tipoSaida, "Value", "Text");
        }

        [HttpGet]
        public ActionResult MontarTelaMovimentacaoAvulsa()
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

            var vm = new MovimentacaoAvulsaGridViewModel();
            if (Session["FiltroMovEstoque"] != null)
            {
                vm = (MovimentacaoAvulsaGridViewModel)Session["FiltroMovEstoque"];
                Session["FiltroMovEstoque"] = null;
            }
            else
            {
                vm.ProdutoInsumo = 1;
                vm.MOVMT_DT_MOVIMENTO_INICIAL = DateTime.Now;
                FiltrarMovimentacaoAvulsa(vm);
            }

            ViewBag.Title = "Movimentações Avulsas";
            List<SelectListItem> prodIns = new List<SelectListItem>();
            prodIns.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            prodIns.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.ProdutoInsumo = new SelectList(prodIns, "Value", "Text");
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            if (Session["FlagMovmtAvulsa"] != null)
            {
                if ((Int32)Session["FlagMovmtAvulsa"] == 1)
                {
                    ViewBag.TipoRegistro = 1;
                    ViewBag.ListaMovimento = ((List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoProduto"]).Where(x => x.PRODUTO != null && x.PRODUTO.PROD_IN_COMPOSTO == 0).Where(x => x.MOEP_IN_CHAVE_ORIGEM == 1 || x.MOEP_IN_CHAVE_ORIGEM == 5).ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }
            }
            ViewBag.LstProd = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");

            if (Session["MensAvulsa"] != null)
            {
                if ((Int32)Session["MensAvulsa"] == 1)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensAvulsa"] = 0;
                }
            }            

            return View(vm);
        }

        public ActionResult RetirarFiltroMovimentacaoAvulsa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FlagMovmtAvulsa"] = null;
            Session["ListaMovimentoProduto"] = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        public ActionResult MostrarTudoProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMovmtAvulsa"] = 1;
            Session["ListaMovimentoProduto"] = moepApp.GetAllItensAdm(idAss).Where(x => x.PRODUTO != null && x.PRODUTO.PROD_IN_COMPOSTO == 0).ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacaoAvulsa(MovimentacaoAvulsaGridViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroMovimentacaoAvulsa"] = vm;
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                Int32 volta = 0;
                var lstProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                var usuario = (USUARIO)Session["UserCredentials"];
                Session["FiltroMovEstoque"] = vm;

                if (vm.ProdutoInsumo == 1)
                {
                    Session["FlagMovmtAvulsa"] = 1;
                    volta = moepApp.ExecuteFilterAvulso(vm.MOVMT_IN_OPERACAO, vm.MOVMT_IN_TIPO_MOVIMENTO, vm.MOVMT_DT_MOVIMENTO_INICIAL, vm.MOVMT_DT_MOVIMENTO_FINAL, 1, vm.PROD_CD_ID, idAss, out lstProd);
                    Session["ListaMovimentoProduto"] = lstProd;
                }
                if (volta == 1)
                {
                    Session["MensAvulsa"] = 1;
                }

                return RedirectToAction("MontarTelaMovimentacaoAvulsa");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMovimentacaoAvulsa");
            }
        }

        [HttpGet]
        public ActionResult ExcluirMovimentoProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Verifica se tem usuario logado
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Recupera movimento
            MOVIMENTO_ESTOQUE_PRODUTO item = moepApp.GetItemById(id);
            Int32 volta = moepApp.ValidateDelete(item, usuario);

            // Acerta estoque
            PRODUTO pef = prodApp.GetItemById(item.PROD_CD_ID);
            if (pef != null)
            {
                pef.PROD_DS_JUSTIFICATIVA = item.MOEP_DS_JUSTIFICATIVA;
                pef.PROD_DT_ULTIMA_MOVIMENTACAO = DateTime.Today.Date;

                //Efetua Operações
                if (item.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    pef.PROD_QN_ESTOQUE = pef.PROD_QN_ESTOQUE - (Int32)item.MOEP_QN_QUANTIDADE;
                    Int32 ve = prodApp.ValidateEdit(pef, pef, usuario);
                }
                else
                {
                    pef.PROD_QN_ESTOQUE = pef.PROD_QN_ESTOQUE + (Int32)item.MOEP_QN_QUANTIDADE;
                    Int32 vs = prodApp.ValidateEdit(pef, pef, usuario);
                }
            }

            Session["FlagMovmtAvulsa"] = null;
            Session["ListaMovimentoProduto"] = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpGet]
        public ActionResult ReativarMovimentoProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Verifica se tem usuario logado
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Recupera movimento
            MOVIMENTO_ESTOQUE_PRODUTO item = moepApp.GetItemById(id);
            Int32 volta = moepApp.ValidateReativar(item, usuario);

            // Acerta estoque
            PRODUTO pef = prodApp.GetItemById(item.PROD_CD_ID);
            if (pef != null)
            {
                pef.PROD_DS_JUSTIFICATIVA = item.MOEP_DS_JUSTIFICATIVA;
                pef.PROD_DT_ULTIMA_MOVIMENTACAO = DateTime.Today.Date;

                //Efetua Operações
                if (item.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    pef.PROD_QN_ESTOQUE = pef.PROD_QN_ESTOQUE + (Int32)item.MOEP_QN_QUANTIDADE;
                    Int32 ve = prodApp.ValidateEdit(pef, pef, usuario);
                }
                else
                {
                    pef.PROD_QN_ESTOQUE = pef.PROD_QN_ESTOQUE - (Int32)item.MOEP_QN_QUANTIDADE;
                    Int32 vs = prodApp.ValidateEdit(pef, pef, usuario);
                }
            }

            Session["FlagMovmtAvulsa"] = null;
            Session["ListaMovimentoProduto"] = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpGet]
        public ActionResult IncluirMovimentacaoAvulsa()
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

            ViewBag.Title = "Movimentações Avulsas - Lançamento";
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.ListaProdutos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");

            MovimentoEstoqueProdutoViewModel vm = new MovimentoEstoqueProdutoViewModel();
            vm.MOEP_DT_MOVIMENTO = DateTime.Now;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.MOEP_IN_ATIVO = 1;
            vm.MOEP_IN_CHAVE_ORIGEM = 1;
            vm.MOEP_IN_ORIGEM = "Movimentação Avulsa";
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirMovimentacaoAvulsa(MovimentoEstoqueProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            var usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Title = "Lançamentos - Produtos";
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.ListaMovimentoProd = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoProduto"];
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.ListaProdutos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");

            if (ModelState.IsValid)
            {
                MOVIMENTO_ESTOQUE_PRODUTO item = Mapper.Map<MovimentoEstoqueProdutoViewModel, MOVIMENTO_ESTOQUE_PRODUTO>(vm);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                Int32 volta = moepApp.ValidateCreate(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensProduto"] = 3;
                    return RedirectToAction("MontarTelaProduto");
                }

                // Acerta estoque
                PRODUTO prod = prodApp.GetItemById(item.PROD_CD_ID);
                if (item.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    prod.PROD_QN_ESTOQUE = prod.PROD_QN_ESTOQUE + item.MOEP_QN_QUANTIDADE;                  
                }
                else
                {
                    prod.PROD_QN_ESTOQUE = prod.PROD_QN_ESTOQUE - item.MOEP_QN_QUANTIDADE;
                }
                prod.PROD_DT_ULTIMA_MOVIMENTACAO = DateTime.Now;
                Int32 volta1 = prodApp.ValidateEdit(prod, prod);


                // Retorno
                Session["ListaProdEstoque"] = null;
                return RedirectToAction("IncluirMovimentacaoAvulsa");
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult ConsultarProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("ConsultarProduto", "Produto", new { id = id });
        }


        [HttpPost]
        public ActionResult FiltrarMovimentacaoEstoqueProdutoNova(ProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroMvmtProd"] = true;
            Session["EntradaSaida"] = vm.EntradaSaida;
            return RedirectToAction("VerMovimentacaoEstoqueProdutoNova", new { id = (Int32)Session["IdMovimento"] });
        }

    }
}