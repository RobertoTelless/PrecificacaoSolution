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
    public class CentroCustoController : Controller
    {
        private readonly ICentroCustoAppService ccApp;
        private readonly ILogAppService logApp;
        private readonly IGrupoCCAppService gruApp;
        private readonly ISubgrupoAppService sgApp;

        private String msg;
        private Exception exception;
        PLANO_CONTA objCC = new PLANO_CONTA();
        PLANO_CONTA objCCAntes = new PLANO_CONTA();
        List<PLANO_CONTA> listaMasterCC = new List<PLANO_CONTA>();
        String extensao;

        public CentroCustoController(ICentroCustoAppService ccApps, ILogAppService logApps, IGrupoCCAppService gruApps, ISubgrupoAppService sgApps)
        {
            ccApp = ccApps;
            logApp = logApps;
            gruApp = gruApps;
            sgApp = sgApps;
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
        public ActionResult MontarTelaCC()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "USU")
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
            if (Session["ListaCC"] == null)
            {
                listaMasterCC = ccApp.GetAllItens(idAss);
                Session["ListaCC"] = listaMasterCC;
            }
            ViewBag.Listas = ((List<PLANO_CONTA>)Session["ListaCC"]).ToList();
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Centros de Custos";
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(x => x.GRCC_NM_NOME).ToList<GRUPO_PLANO_CONTA>(), "GRCC_CD_ID", "GRCC_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(idAss).OrderBy(x => x.SGCC_NM_NOME).ToList<SUBGRUPO_PLANO_CONTA>(), "SGCC_CD_ID", "SGCC_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Receita", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Despesa", Value = "2" });
            ViewBag.Tipos = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");

            if (Session["MensCC"] != null)
            {
                if ((Int32)Session["MensCC"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCC"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0163", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCC"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0164", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objCC = new PLANO_CONTA();
            Session["MensCC"] = 0;
            return View(objCC);
        }

        public ActionResult RetirarFiltroCC()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCC"] = null;
            Session["FiltroCC"] = null;
            return RedirectToAction("MontarTelaCC");
        }

        public ActionResult MostrarTudoCC()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCC = ccApp.GetAllItensAdm(idAss);
            Session["ListaCC"] = listaMasterCC;
            return RedirectToAction("MontarTelaCC");
        }

        [HttpPost]
        public ActionResult FiltrarCC(PLANO_CONTA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PLANO_CONTA> listaObj = new List<PLANO_CONTA>();
                Session["FiltroCC"] = item;
                Int32 volta = ccApp.ExecuteFilter(item.GRCC_CD_ID, item.SGCC_CD_ID, item.CECU_IN_TIPO, 1, item.CECU_NR_NUMERO, item.CECU_NM_NOME, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCC"] = 1;
                }

                // Sucesso
                listaMasterCC = listaObj;
                Session["ListaCC"] = listaObj;
                return RedirectToAction("MontarTelaCC");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCC");
            }
        }

        // Filtro em cascata de subgrupo
        [HttpPost]
        public JsonResult FiltroSubGrupoCC(Int32? id)
        {
            var listaSubFiltrada = new List<SUBGRUPO_PLANO_CONTA>();
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Filtro para caso o placeholder seja selecionado
            if (id == null)
            {
                listaSubFiltrada = sgApp.GetAllItens(idAss);
            }
            else
            {
                listaSubFiltrada = sgApp.GetAllItens(idAss).Where(x => x.GRCC_CD_ID == id).ToList();
            }

            return Json(listaSubFiltrada.Select(x => new { x.SGCC_CD_ID, x.SGCC_NM_EXIBE }));
        }

        public ActionResult VoltarBaseCC()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaCC"] = ccApp.GetAllItens(idAss);
            return RedirectToAction("MontarTelaCC");
        }

        [HttpGet]
        public ActionResult IncluirCC()
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
                    Session["MensCC"] = 2;
                    return RedirectToAction("MontarTelaCC", "CentroCusto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(x => x.GRCC_NM_NOME).ToList<GRUPO_PLANO_CONTA>(), "GRCC_CD_ID", "GRCC_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(idAss).OrderBy(x => x.SGCC_NM_NOME).ToList<SUBGRUPO_PLANO_CONTA>(), "SGCC_CD_ID", "SGCC_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Receita", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Despesa", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");

            // Prepara view
            PLANO_CONTA item = new PLANO_CONTA();
            CentroCustoViewModel vm = Mapper.Map<PLANO_CONTA, CentroCustoViewModel>(item);
            vm.CECU_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CECU_IN_MOVTO = 1;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCC(CentroCustoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(x => x.GRCC_NM_NOME).ToList<GRUPO_PLANO_CONTA>(), "GRCC_CD_ID", "GRCC_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(idAss).OrderBy(x => x.SGCC_NM_NOME).ToList<SUBGRUPO_PLANO_CONTA>(), "SGCC_CD_ID", "SGCC_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Receita", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Despesa", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PLANO_CONTA item = Mapper.Map<CentroCustoViewModel, PLANO_CONTA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ccApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCC"] = 3;
                        return RedirectToAction("MontarTelaCC", "CentroCusto");
                    }

                    // Sucesso
                    listaMasterCC = new List<PLANO_CONTA>();
                    Session["ListaCC"] = null;
                    return RedirectToAction("MontarTelaCC");
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
        public ActionResult EditarCC(Int32 id)
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
                    Session["MensCC"] = 2;
                    return RedirectToAction("MontarTelaCC", "CentroCusto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(x => x.GRCC_NM_NOME).ToList<GRUPO_PLANO_CONTA>(), "GRCC_CD_ID", "GRCC_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(idAss).OrderBy(x => x.SGCC_NM_NOME).ToList<SUBGRUPO_PLANO_CONTA>(), "SGCC_CD_ID", "SGCC_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Receita", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Despesa", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");

            // Prepara view
            PLANO_CONTA item = ccApp.GetItemById(id);
            objCCAntes = item;
            Session["CentroCusto"] = item;
            Session["IdVolta"] = id;
            Session["IdCC"] = id;
            CentroCustoViewModel vm = Mapper.Map<PLANO_CONTA, CentroCustoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCC(CentroCustoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Grupos = new SelectList(gruApp.GetAllItens(idAss).OrderBy(x => x.GRCC_NM_NOME).ToList<GRUPO_PLANO_CONTA>(), "GRCC_CD_ID", "GRCC_NM_EXIBE");
            ViewBag.Subs = new SelectList(sgApp.GetAllItens(idAss).OrderBy(x => x.SGCC_NM_NOME).ToList<SUBGRUPO_PLANO_CONTA>(), "SGCC_CD_ID", "SGCC_NM_EXIBE");
            List<SelectListItem> tipoCC = new List<SelectListItem>();
            tipoCC.Add(new SelectListItem() { Text = "Receita", Value = "1" });
            tipoCC.Add(new SelectListItem() { Text = "Despesa", Value = "2" });
            ViewBag.TiposLancamento = new SelectList(tipoCC, "Value", "Text");
            List<SelectListItem> tipoMov = new List<SelectListItem>();
            tipoMov.Add(new SelectListItem() { Text = "Todas", Value = "1" });
            tipoMov.Add(new SelectListItem() { Text = "Compras", Value = "2" });
            tipoMov.Add(new SelectListItem() { Text = "Vendas", Value = "3" });
            ViewBag.TipoMov = new SelectList(tipoMov, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PLANO_CONTA item = Mapper.Map<CentroCustoViewModel, PLANO_CONTA>(vm);
                    Int32 volta = ccApp.ValidateEdit(item, objCCAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCC = new List<PLANO_CONTA>();
                    Session["ListaCC"] = null;
                    return RedirectToAction("MontarTelaCC");
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
        public ActionResult ExcluirCC(Int32 id)
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
                    Session["MensCC"] = 2;
                    return RedirectToAction("MontarTelaCC", "CentroCusto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            PLANO_CONTA item = ccApp.GetItemById(id);
            objCCAntes = (PLANO_CONTA)Session["CentroCusto"];
            item.CECU_IN_ATIVO = 0;
            item.GRUPO_PLANO_CONTA = null;
            item.SUBGRUPO_PLANO_CONTA = null;
            item.ASSINANTE = null;
            Int32 volta = ccApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCC"] = 4;
                return RedirectToAction("MontarTelaCC", "CentroCusto");
            }
            listaMasterCC = new List<PLANO_CONTA>();
            Session["ListaCC"] = null;
            return RedirectToAction("MontarTelaCC");
        }

        [HttpGet]
        public ActionResult ReativarCC(Int32 id)
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
                    Session["MensCC"] = 2;
                    return RedirectToAction("MontarTelaCC", "CentroCusto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            PLANO_CONTA item = ccApp.GetItemById(id);
            objCCAntes = (PLANO_CONTA)Session["CentroCusto"];
            item.CECU_IN_ATIVO = 1;
            item.GRUPO_PLANO_CONTA = null;
            item.SUBGRUPO_PLANO_CONTA = null;
            item.ASSINANTE = null;
            Int32 volta = ccApp.ValidateReativar(item, usuario);
            listaMasterCC = new List<PLANO_CONTA>();
            Session["ListaCC"] = null;
            return RedirectToAction("MontarTelaCC");
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "PlanoContaLista" + "_" + data + ".pdf";
            List<PLANO_CONTA> lista = ((List<PLANO_CONTA>)Session["ListaCC"]).ToList();
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(6);
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

            cell = new PdfPCell(new Paragraph("Plano de Contas - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 6;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Número", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Grupo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Subgrupo", meuFont))
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
            cell.Colspan = 2;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Tipo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (PLANO_CONTA item in lista)
            {
                cell = new PdfPCell(new Paragraph((item.GRUPO_PLANO_CONTA == null ? "-" : item.GRUPO_PLANO_CONTA.GRCC_NR_NUMERO) + "." + (item.SUBGRUPO_PLANO_CONTA == null ? "-" : item.SUBGRUPO_PLANO_CONTA.SGCC_NR_NUMERO) + "." + item.CECU_NR_NUMERO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.GRUPO_PLANO_CONTA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.GRUPO_PLANO_CONTA.GRCC_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
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
                if (item.SUBGRUPO_PLANO_CONTA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.SUBGRUPO_PLANO_CONTA.SGCC_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
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
                cell = new PdfPCell(new Paragraph(item.CECU_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                table.AddCell(cell);
                if (item.CECU_IN_TIPO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Receita", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Despesa", meuFont))
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

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("MontarTelaCC");
        }
    }
}